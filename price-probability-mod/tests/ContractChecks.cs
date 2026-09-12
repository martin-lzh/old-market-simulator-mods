using Mono.Cecil;
using System.Security.Cryptography;

string managed = Path.Combine(args[0], "Old Market Simulator_Data", "Managed");
using var game = AssemblyDefinition.ReadAssembly(Path.Combine(managed, "Assembly-CSharp.dll"));
int checks = 0;
void Check(bool pass, string label) { if (!pass) throw new Exception(label); checks++; }
TypeDefinition Type(string name) => game.MainModule.Types.Single(x => x.Name == name);
MethodDefinition Method(string type, string name) => Type(type).Methods.Single(x => x.Name == name);
Check(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(managed, "Assembly-CSharp.dll")))) ==
    "FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296", "reviewed game version");
Check(Method("PricePanel", "SetData").Parameters.Select(x => x.Name).SequenceEqual(new[] { "productSO", "sellingPrice" }), "panel binding");
Check(Method("PricePanel", "SubmitPrice").Body.Instructions.Any(x => x.Operand is MethodReference m && m.Name == "UpdatePriceServerRpc"), "native confirmation sends price");
Check(Method("GameManager", "GetSellingPriceServer").Parameters.Single().Name == "productSO", "customer price argument");
Check(Method("GameManager", "LoadWorldServer").HasBody, "world load hook");
Check(Type("SaveManager").Fields.Any(x => x.Name == "currentSlot" && x.FieldType.FullName == "System.String"), "slot identity");
Check(Method("SaveManager", "DeleteSlot").Parameters.Single().Name == "slot", "delete slot hook");
Check(Method("SaveManager", "DeleteSave").HasBody, "delete current hook");
var customer = Method("StateTakeProduct", "OnEnter").Body.Instructions;
foreach (string name in new[] { "Range", "RoundToInt", "GetWholesalePrice", "GetSellingPriceServer" })
    Check(customer.Any(x => x.Operand is MethodReference m && m.Name == name), "native acceptance " + name);
Check(customer.Any(x => x.Operand is FieldReference f && f.Name == "maxProfitMultiplier"), "runtime multiplier");
using var plugin = AssemblyDefinition.ReadAssembly(args[1]);
foreach (var type in plugin.MainModule.Types)
foreach (var method in type.Methods.Where(x => x.HasBody))
    Check(!method.Body.Instructions.Any(x => x.Operand is MethodReference m && m.DeclaringType.Name == "SaveManager"
        && (m.Name.Contains("Save") || m.Name.Contains("Delete"))), "no direct save mutation " + method.Name);
var update = Method("GameManager", "UpdatePriceServerRpc");
Check(update.CustomAttributes.Any(a => a.AttributeType.Name == "ServerRpcAttribute" &&
    a.Fields.Any(f => f.Name == "RequireOwnership" && Equals(f.Argument.Value, false))), "vanilla clients may submit prices");
Check(update.Body.Instructions.Any(x => x.Operand is FieldReference f && f.Name == "sellingPriceForTag"), "server updates native price tag variable");
Check(Type("Item").Fields.Single(x => x.Name == "sellingPriceForTag").FieldType.Name.StartsWith("NetworkVariable"), "native price synchronization");
var mod = plugin.MainModule.Types.Single(x => x.Name == "Plugin");
var normalize = mod.Methods.Single(x => x.Name == "NormalizeRequest");
Check(normalize.ReturnType.FullName == "System.Void", "does not cancel native RPC");
Check(normalize.Parameters.Single(x => x.Name == "price").ParameterType.IsByReference, "normalizes price argument");
Check(!normalize.Body.Instructions.Any(x => x.Operand is MethodReference m && m.Name == "UpdatePriceServerRpc"), "no nested correction RPC");
Check(!plugin.MainModule.Types.Any(t => t.BaseType?.Name == "NetworkBehaviour"), "no added network behaviours");
Check(!plugin.MainModule.Types.SelectMany(t => t.Fields).Any(f => f.FieldType.Name.StartsWith("NetworkVariable")), "no added network variables");
Check(!plugin.MainModule.Types.SelectMany(t => t.Methods).SelectMany(m => m.CustomAttributes)
    .Any(a => a.AttributeType.Name == "ServerRpcAttribute" || a.AttributeType.Name == "ClientRpcAttribute"), "no custom RPC protocol");
Check(Method("PlayerInteraction", "OpenPriceTagServerRpc").Body.Instructions.Any(x =>
    x.Operand is MethodReference m && m.Name == "GetSellingPriceServer"), "opening UI reads host price");
Check(Method("CustomerController", "Update").Body.Instructions.Any(x =>
    x.Operand is MethodReference m && m.Name == "get_IsServer"), "customer logic is server-side");
Console.WriteLine($"Passed {checks} installed-assembly contract checks; no Unity execution.");
