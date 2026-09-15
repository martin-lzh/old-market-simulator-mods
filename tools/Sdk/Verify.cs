using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Security.Cryptography;
using System.Text.Json;

static class Verify
{
    public static void Game(Api api, string managed) {
        foreach (var a in api.Assemblies) {
            var path = Path.Combine(managed, AssemblyNameReference.Parse(a.Name).Name + ".dll");
            if (Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))) != a.Sha256)
                throw new InvalidOperationException($"Game dependency changed: {Path.GetFileName(path)}. Create and validate a new SDK revision.");
        }
        Console.WriteLine($"Verified {api.Assemblies.Count} original dependency hashes (read-only).");
    }
    public static void References(Api api, string directory) {
        foreach (var entry in api.Assemblies) {
            using var a = AssemblyDefinition.ReadAssembly(Path.Combine(directory, AssemblyNameReference.Parse(entry.Name).Name + ".dll"));
            if (a.Name.FullName != entry.Name || a.MainModule.Resources.Count != 0 ||
                !a.CustomAttributes.Any(c => c.AttributeType.Name == "ReferenceAssemblyAttribute")) throw new InvalidOperationException("Invalid compiler reference.");
            foreach (var t in AllTypes(a.MainModule.Types)) {
                foreach (var f in t.Fields) if (f.InitialValue.Length != 0) throw new InvalidOperationException("Reference contains field data.");
                foreach (var m in t.Methods) if (m.HasBody && (m.Body.Instructions.Count != 2 ||
                    m.Body.Instructions[0].OpCode != OpCodes.Ldnull || m.Body.Instructions[1].OpCode != OpCodes.Throw))
                    throw new InvalidOperationException("Reference contains an unexpected implementation.");
            }
        }
        Console.WriteLine("Reference identities and implementation/resource exclusion verified.");
    }
    static IEnumerable<TypeDefinition> AllTypes(IEnumerable<TypeDefinition> ts) => ts.SelectMany(t => new[] { t }.Concat(AllTypes(t.NestedTypes)));
    public static void Plugin(string path, string name, string version) {
        using var a = AssemblyDefinition.ReadAssembly(path);
        if (a.Name.Name != name || a.Name.Version != new Version(version + ".0") ||
            a.CustomAttributes.Any(c => c.AttributeType.Name == "ReferenceAssemblyAttribute")) throw new InvalidOperationException("Plugin identity mismatch.");
        var versions = AllTypes(a.MainModule.Types).SelectMany(t => t.CustomAttributes)
            .Where(c => c.AttributeType.FullName == "BepInEx.BepInPlugin").Select(c => c.ConstructorArguments[2].Value?.ToString())
            .Concat(a.CustomAttributes.Where(c => c.AttributeType.FullName == "MelonLoader.MelonInfoAttribute")
                .Select(c => c.ConstructorArguments[2].Value?.ToString())).ToArray();
        if (versions.Length != 1 || versions[0] != version) throw new InvalidOperationException("Loader version mismatch.");
        Console.WriteLine($"Verified plugin {name} {version}.");
    }
    public static void Compare(string original, string sdk) {
        string[] Fingerprint(string path) {
            using var a = AssemblyDefinition.ReadAssembly(path);
            string Operand(object? o, MethodDefinition m) => o switch {
                null => "", Instruction i => "@" + m.Body.Instructions.IndexOf(i),
                Instruction[] list => string.Join(',', list.Select(i => Operand(i, m))),
                MemberReference r => r.FullName,
                VariableDefinition v => "local:" + v.Index,
                ParameterDefinition p => "arg:" + p.Index,
                _ => Convert.ToString(o, System.Globalization.CultureInfo.InvariantCulture)!
            };
            return AllTypes(a.MainModule.Types).SelectMany(t => t.Methods).Where(m => m.HasBody).OrderBy(m => m.FullName)
                .Select(m => m.FullName + "|" + string.Join(';', m.Body.Variables.Select(v => v.VariableType.FullName)) + "|" +
                    string.Join(';', m.Body.Instructions.Select(i => i.OpCode.Name + ":" + Operand(i.Operand, m))) + "|" +
                    string.Join(';', m.Body.ExceptionHandlers.Select(e => $"{e.HandlerType}:{e.CatchType}:{Operand(e.TryStart, m)}:{Operand(e.TryEnd, m)}:{Operand(e.HandlerStart, m)}:{Operand(e.HandlerEnd, m)}:{Operand(e.FilterStart, m)}")))
                .Concat(a.MainModule.Resources.OfType<EmbeddedResource>().OrderBy(r => r.Name).Select(r => r.Name + ":" + Convert.ToHexString(SHA256.HashData(r.GetResourceData()))))
                .Concat(a.MainModule.AssemblyReferences.Select(r => r.FullName).OrderBy(n => n)).ToArray();
        }
        var left = Fingerprint(original); var right = Fingerprint(sdk);
        if (!left.SequenceEqual(right)) {
            var difference = left.Except(right).Concat(right.Except(left)).Select(s => s.Split('|')[0]);
            throw new InvalidOperationException("Symbolic IL/resource differences: " + string.Join(", ", difference));
        }
        Console.WriteLine($"Symbolic method IL and embedded resources match: {Path.GetFileName(sdk)}.");
    }
}
