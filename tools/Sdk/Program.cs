using System.Text.Json;
using System.Text.Json.Serialization;

var json = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
if (args.Length >= 6 && args[0] == "export") {
    if (File.Exists(args[4])) throw new InvalidOperationException("Snapshot exists; export to a new revision path and review the diff.");
    var api = new Export(args[1], args[2].Split(';')).Run(args.Skip(5),
        JsonSerializer.Deserialize<Dictionary<string, string[]>>(File.ReadAllText(args[3]))!);
    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(args[4]))!);
    File.WriteAllText(args[4], JsonSerializer.Serialize(api, json).Replace("\r\n", "\n") + "\n");
    Console.WriteLine($"Exported {api.Types.Count} types, {api.Types.Sum(t => t.Methods.Count)} methods, {api.Types.Sum(t => t.Fields.Count)} fields (metadata only).");
} else if (args.Length == 3 && args[0] == "build") {
    var api = JsonSerializer.Deserialize<Api>(File.ReadAllText(args[1]), json)!;
    Build.Run(api, args[2]);
    Verify.References(api, args[2]);
    Console.WriteLine("Built compiler-only references; never install or package these DLLs.");
} else if (args.Length == 3 && args[0] == "verify-game") {
    Verify.Game(JsonSerializer.Deserialize<Api>(File.ReadAllText(args[1]), json)!, args[2]);
} else if (args.Length == 3 && args[0] == "compare") {
    Verify.Compare(args[1], args[2]);
} else if (args.Length == 4 && args[0] == "verify-plugin") {
    Verify.Plugin(args[1], args[2], args[3]);
} else {
    throw new ArgumentException("export <managed> <loader-dirs separated by ;> <supplemental.json> <new-api.json> <plugin.dll> ... | build <api.json> <output-dir>");
}
