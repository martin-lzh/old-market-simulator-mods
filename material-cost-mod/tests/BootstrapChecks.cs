using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Loader;

var package = Path.GetFullPath(args[0]);
var scratch = Path.GetFullPath(args[1]);
Directory.CreateDirectory(scratch);
int checks = 0;
void Check(bool result, string name) { if (!result) throw new Exception(name); checks++; }
foreach (string file in Directory.GetFiles(Path.Combine(package,"OldMarketMod"),"*.dll"))
{
    using var stream=File.OpenRead(file);
    using var pe=new PEReader(stream);
    var md=pe.GetMetadataReader();
    var refs=md.AssemblyReferences.Select(h=>md.GetString(md.GetAssemblyReference(h).Name)).ToArray();
    Console.WriteLine(Path.GetFileName(file)+": "+string.Join(", ",refs));
}
foreach (string name in new[] {"OldMarket.Bootstrap.dll", "OldMarket.MaterialCost.dll"})
{
    using var stream = File.OpenRead(Path.Combine(package,"OldMarketMod",name));
    using var pe = new PEReader(stream);
    var md = pe.GetMetadataReader();
    var references = md.AssemblyReferences.Select(h=>md.GetString(md.GetAssemblyReference(h).Name)).ToArray();
    Check(!references.Any(r=>r.StartsWith("BepInEx") || r.StartsWith("MelonLoader")), name+" has a framework dependency");
    if (name.Contains("Bootstrap")) Check(!references.Any(r=>r.StartsWith("Unity") || r.Contains("Harmony")), "Bootstrap must not require Unity or Harmony");
}
var originalCulture = CultureInfo.CurrentCulture;
var originalUiCulture = CultureInfo.CurrentUICulture;
try
{
    foreach (bool enabled in new[] {false,true})
    {
        string folder = Path.Combine(scratch,enabled ? "missing-module" : "disabled");
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder,"OldMarket.Bootstrap.dll");
        File.Copy(Path.Combine(package,"OldMarketMod","OldMarket.Bootstrap.dll"),path,true);
        File.WriteAllText(Path.Combine(folder,"enabled.txt"),enabled ? "true" : "false");
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
        var context = new AssemblyLoadContext("bootstrap-check-"+enabled,true);
        var assembly = context.LoadFromAssemblyPath(path);
        var start = assembly.GetType("Doorstop.Entrypoint")!.GetMethod("Start")!;
        start.Invoke(null,null);
        string log = File.ReadAllText(Path.Combine(folder,"loader.log"));
        Check(log.Contains(enabled ? "Bootstrap failed:" : "Cost module disabled;"), "Expected safe branch was not logged");
        Check(CultureInfo.CurrentCulture.Name=="de-DE" && CultureInfo.CurrentUICulture.Name=="zh-CN", "Bootstrap changed culture");
        Check(log.Contains("decimal=,"), "Decimal format was not retained");
        start.Invoke(null,null);
        Check(File.ReadAllText(Path.Combine(folder,"loader.log"))==log,"Repeated entry was not idempotent");
        Check(context.Assemblies.All(a=>a.GetName().Name=="OldMarket.Bootstrap"), "Unexpected module loaded");
        context.Unload();
    }
}
finally { CultureInfo.CurrentCulture=originalCulture; CultureInfo.CurrentUICulture=originalUiCulture; }
Console.WriteLine($"PASS: {checks} bootstrap dependency, disabled mode, failure and culture checks");
