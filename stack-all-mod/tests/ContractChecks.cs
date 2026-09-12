using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Security.Cryptography;

string gameDir = args[0];
string managed = Path.Combine(gameDir, "Old Market Simulator_Data", "Managed");
using var game = AssemblyDefinition.ReadAssembly(Path.Combine(managed, "Assembly-CSharp.dll"));
using var network = AssemblyDefinition.ReadAssembly(Path.Combine(managed, "Unity.Netcode.Runtime.dll"));
using var plugin = AssemblyDefinition.ReadAssembly(args[1]);
int checks = 0;
void Check(bool value, string name) { if (!value) throw new Exception(name); checks++; }
TypeDefinition Type(string name) => game.MainModule.Types.Single(t => t.Name == name);
MethodDefinition Method(string type, string name) => Type(type).Methods.Single(m => m.Name == name);
Check(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(managed, "Assembly-CSharp.dll")))) ==
    "FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296", "reviewed game hash");
var behaviour = network.MainModule.Types.Single(t => t.Name == "NetworkBehaviour");
var stage = behaviour.Fields.Single(f => f.Name == "__rpc_exec_stage");
var enumType = behaviour.NestedTypes.Single(t => t.FullName == stage.FieldType.FullName);
Check(enumType.Fields.Any(f => f.Name == "Execute") && enumType.Fields.Any(f => f.Name == "Send"), "RPC stage contract");
foreach (string field in new[] { "activeSlots", "currentSlot", "slots" })
    Check(Type("PlayerInventory").Fields.Any(f => f.Name == field), "inventory field " + field);
foreach (var spec in new[] { ("CheckBag", 1), ("HandleDrop", 1), ("HandleThrow", 1), ("HandleItemPlacement", 2) })
    Check(Method("PlayerInventory", spec.Item1).Body.Instructions.Count(i => i.OpCode == OpCodes.Ldfld &&
        i.Operand is FieldReference f && f.DeclaringType.Name == "ItemSO" && f.Name == "stackSize") == spec.Item2, "stack branches " + spec.Item1);
foreach (var spec in new[] { ("PlayerInventory", "HandleNewBlockPlacement"), ("Aquarium", "Interact"), ("OrigamiStand", "Interact"), ("BlockBeeHive", "Interact") })
    Check(Method(spec.Item1, spec.Item2).Body.Instructions.Count(i => i.Operand is MethodReference m &&
        m.DeclaringType.Name == "PlayerInventory" && m.Name == "UseItem") == 1, "single consumption " + spec);
foreach (string name in new[] { "GiveItemClientRpc", "LoadPlayerItemsClientRpc" })
{
    var method = Method("PlayerInventory", name);
    Check(method.Body.Instructions.Any(i => i.OpCode == OpCodes.Stfld && i.Operand is FieldReference f && f.Name == "__rpc_exec_stage"), "RPC stage reset " + name);
    Check(method.CustomAttributes.Any(a => a.AttributeType.Name == "ClientRpcAttribute"), "RPC attribute " + name);
}
Check(Method("PlayerInventory", "GiveItemClientRpc").Parameters.Select(p => p.Name).SequenceEqual(new[] { "itemId", "amount", "cost", "dayCounter", "clientRpcParams" }), "receive parameter names");
Check(Method("PlayerInventory", "RefillCurrentItemServer").Parameters.Select(p => p.Name).SequenceEqual(new[] { "sourceItemId", "sourceAmount", "sourceDayCounter", "sourceCost" }), "refill parameter names");
Check(Method("PlayerInventory", "ReduceCurrentItemAmount").Body.Instructions.Any(i => i.Operand is TypeReference t && t.Name == "ToolSO"), "tool consumption remains in native branch");
var mod = plugin.MainModule.Types.Single(t => t.Name == "Plugin");
var receive = mod.Methods.Single(m => m.Name == "Receive");
Check(receive.Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "Executing"), "receive execution-stage gate");
Check(receive.Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "Eligible"), "tool exclusion in receiver");
Check(mod.Methods.Single(m => m.Name == "Eligible").Body.Instructions.Any(i => i.Operand is TypeReference t && t.Name == "ToolSO"), "eligibility excludes tools");
Check(!mod.Methods.Where(m => m.HasBody).SelectMany(m => m.Body.Instructions).Any(i =>
    i.OpCode == OpCodes.Stfld && i.Operand is FieldReference f && f.DeclaringType.Name == "ItemSO"), "no asset field writes");
foreach (var spec in new[] { ("SetSlot", 1), ("CheckBag", 2), ("OnCurrentSlotChanged", 1), ("OnNetworkSpawn", 1) })
{
    var il = Method("PlayerInventory", spec.Item1).Body.Instructions;
    Check(il.Count(i => i.OpCode == OpCodes.Ldfld && i.Operand is FieldReference f && f.Name == "slots" &&
        i.Next?.Operand is MethodReference m && m.Name == "get_Count") == spec.Item2, "visible loop rewrite " + spec.Item1);
}
Check(Type("InventorySlot").Fields.Where(f => !f.IsStatic).Select(f => f.Name).SequenceEqual(new[] { "itemId", "amount", "cost", "dayCounter" }), "native slot fields unchanged");
Check(Method("ItemSlot", "UpdateSlot").Parameters.Select(p => p.Name).SequenceEqual(new[] { "inventorySlot", "index" }), "HUD patch parameters");
Check(Method("SaveManager", "SavePlayerData").Parameters.Select(p => p.Name).SequenceEqual(new[] { "clientId", "playerData" }), "save trimming parameters");
Check(mod.Methods.Single(m => m.Name == "TrimSave").Body.Instructions.Any(i => i.Operand is MethodReference m && m.DeclaringType.Name == "ContainerPlan" && m.Name == "Trim"), "save trims only trailing empty backing entries");
Check(Method("PlayerInventory", "OnNetworkDespawn").Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "SavePlayerData"), "disconnect uses same save hook");
Check(!mod.Methods.Where(m => m.HasBody).SelectMany(m => m.Body.Instructions).Any(i =>
    i.Operand is MethodReference m && m.DeclaringType.Namespace == "System.IO" &&
    (m.Name.Contains("Write") || m.Name.Contains("Create") || m.Name.Contains("Delete"))), "plugin does not directly write save files");
Check(mod.Methods.Single(m => m.Name == "AfterBag").Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "SpillContainer"), "locked backing containers are returned to world");
Check(mod.Methods.Single(m => m.Name == "BeforeRemove").Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "Snapshot"), "bulk consume snapshots containers before native removal");
Check(mod.Methods.Single(m => m.Name == "AfterRemove").Body.Instructions.Any(i => i.Operand is FieldReference f && f.Name == "destroyWhenEmpty"), "bulk consume retains reusable empty baskets");
foreach (string name in new[] { "HandleDrop", "HandleThrow" })
{
    var method = Method("PlayerInventory", name);
    Check(method.Body.ExceptionHandlers.Count == 0, "input insertion has no exception boundaries " + name);
    Check(method.Body.Instructions.Count(i => i.Operand is MethodReference m && m.Name == "WasPerformedThisFrame") == 1, "single input gate " + name);
    Check(method.Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "UseItem") &&
        method.Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "ReduceCurrentItemAmount"), "separate container and item actions " + name);
}
var repeat = plugin.MainModule.Types.Single(t => t.Name == "RepeatActions");
Check(repeat.Methods.Single(m => m.Name == "Units").Body.Instructions.Any(i => i.Operand is TypeReference t && t.Name == "ProductSO"), "repeat count uses product containers");
Check(repeat.Methods.Single(m => m.Name == "ShouldAct").Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "IsAnyPanelActive"), "repeat cancels in menus");
var hint = plugin.MainModule.Types.Single(t => t.Name == "ActionHint");
Check(hint.Methods.Single(m => m.Name == "Show").Body.Instructions.Any(i => i.Operand is FieldReference f && f.Name == "controlHintPanel"), "hint follows native operation panel");
Check(hint.Methods.Single(m => m.Name == "Binding").Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "GetBindingDisplayString"), "hint uses actual bindings");
Console.WriteLine($"PASS: {checks} game IL and patch contract checks (no Unity methods executed)");
