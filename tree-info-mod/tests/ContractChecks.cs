using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Security.Cryptography;

string managed = Path.Combine(args[0], "Old Market Simulator_Data", "Managed");
string gamePath = Path.Combine(managed, "Assembly-CSharp.dll");
using var game = AssemblyDefinition.ReadAssembly(gamePath);
using var plugin = AssemblyDefinition.ReadAssembly(args[1]);
int checks = 0;
void Check(bool value, string name) { if (!value) throw new Exception(name); checks++; }
TypeDefinition Type(string name) => game.MainModule.Types.Single(t => t.Name == name);
Check(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(gamePath))) ==
    "FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296", "reviewed game baseline");
foreach (var spec in new[] {
    ("BlockTree", "dayCounter", "Unity.Netcode.NetworkVariable`1<System.Int32>"),
    ("BlockTree", "isWatered", "Unity.Netcode.NetworkVariable`1<System.Boolean>"),
    ("PlayerInteraction", "mainCamera", "UnityEngine.Camera"),
    ("PlayerInteraction", "isInteractionEnabled", "System.Boolean") })
    Check(Type(spec.Item1).Fields.Any(f => f.Name == spec.Item2 && f.FieldType.FullName == spec.Item3), "reflection field " + spec);
var ray = Type("PlayerInteraction").Methods.Single(m => m.Name == "InteractionRay");
Check(!ray.IsStatic && ray.Parameters.Count == 0 && ray.ReturnType.FullName == "System.Void", "Harmony target signature");
foreach (string name in new[] { "ViewportPointToRay", "Raycast", "GetComponentInParent" })
    Check(ray.Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == name), "native ray operation " + name);
foreach (string name in new[] { "interactionDistance", "layerMask", "textSubtitle" })
    Check(ray.Body.Instructions.Any(i => i.Operand is FieldReference f && f.Name == name), "native interaction field " + name);
var day = Type("BlockTree").Methods.Single(m => m.Name == "OnDayChanged");
foreach (string name in new[] { "dayCounter", "isWatered", "season" })
    Check(day.Body.Instructions.Any(i => i.Operand is FieldReference f && f.Name == name), "native growth state " + name);
Check(day.Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "GetCurrentSeason"), "growth uses current season");
var mod = plugin.MainModule.Types.Single(t => t.Name == "Plugin");
var show = mod.Methods.Single(m => m.Name == "ShowTreeInfo");
Check(show.Parameters.Select(p => p.Name).SequenceEqual(new[] { "__instance", "___mainCamera", "___isInteractionEnabled" }), "Harmony injected names");
var instructions = mod.Methods.Where(m => m.HasBody).SelectMany(m => m.Body.Instructions).ToArray();
Check(!instructions.Any(i => (i.OpCode == OpCodes.Stfld || i.OpCode == OpCodes.Stsfld) &&
    i.Operand is FieldReference f && f.DeclaringType.Scope.Name == "Assembly-CSharp"), "no game field writes");
Check(!instructions.Any(i => i.Operand is MethodReference m &&
    (m.Name.EndsWith("ServerRpc") || m.Name.EndsWith("ClientRpc") ||
     (m.Name == "set_Value" && m.DeclaringType.FullName.StartsWith("Unity.Netcode.NetworkVariable")))), "no RPC or network value writes");
Check(show.Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "get_IsOwner"), "local owner gate");
Check(show.Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "set_text" && m.DeclaringType.Namespace == "TMPro"), "subtitle output");
Console.WriteLine($"PASS: {checks} Tree Info game and plugin contracts (no Unity execution)");
