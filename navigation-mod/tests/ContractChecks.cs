using Mono.Cecil;
using System.Security.Cryptography;
using System.Text.Json;

if (args.Length != 2) throw new ArgumentException("Expected game directory and built Navigation DLL path.");
string managed = Path.Combine(args[0], "Old Market Simulator_Data", "Managed");
string gamePath = Path.Combine(managed, "Assembly-CSharp.dll");
using var game = AssemblyDefinition.ReadAssembly(gamePath);
using var plugin = AssemblyDefinition.ReadAssembly(args[1]);
using var input = AssemblyDefinition.ReadAssembly(Path.Combine(managed, "Unity.InputSystem.dll"));
using var physics = AssemblyDefinition.ReadAssembly(Path.Combine(managed, "UnityEngine.PhysicsModule.dll"));
int checks = 0;
void Check(bool pass, string label) { if (!pass) throw new Exception("Navigation contract: " + label); checks++; }
IEnumerable<TypeDefinition> Types(TypeDefinition type) { yield return type; foreach (var nested in type.NestedTypes) foreach (var item in Types(nested)) yield return item; }
IEnumerable<TypeDefinition> All(AssemblyDefinition assembly) => assembly.MainModule.Types.SelectMany(Types);
TypeDefinition Type(string name) => All(game).Single(x => x.Name == name);
MethodDefinition Method(string type, string name, int count = 0) => Type(type).Methods.Single(x => x.Name == name && x.Parameters.Count == count);
bool Calls(MethodDefinition method, string name) => method.HasBody && method.Body.Instructions.Any(x => x.Operand is MethodReference reference && reference.Name == name);
FieldDefinition Field(string type, string name) => Type(type).Fields.Single(x => x.Name == name);
void PublicField(string type, string name, string valueType) { var f = Field(type, name); Check(f.IsPublic && f.FieldType.Name == valueType, type + "." + name + " public " + valueType); }

Check(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(gamePath))) == "FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296", "reviewed 2.1.6 game assembly SHA256");
foreach (string name in new[] { "GameManager", "SaveManager", "RegionManager", "InputManager", "UIManager", "SceneSettings" })
{
    var instance = Type(name).Properties.Single(x => x.Name == "Instance");
    Check(instance.GetMethod.IsPublic && instance.GetMethod.IsStatic && instance.PropertyType.Name == name, name + ".Instance getter");
}
var slot = Field("SaveManager", "currentSlot");
Check(slot.IsPrivate && !slot.IsStatic && slot.FieldType.FullName == "System.String", "private instance string currentSlot reflection");
Check(Method("SaveManager", "SetCurrentSlot", 1).Body.Instructions.Any(x => x.Operand is FieldReference f && f.Name == "currentSlot"), "native setter uses currentSlot");
var region = Method("RegionManager", "GetLocalCurrentRegionSceneName");
Check(region.IsPrivate && !region.IsStatic && region.ReturnType.FullName == "System.String", "private parameterless region method reflection");
Check(Calls(region, "RefreshKnownRegions"), "region method updates native region cache");
Check(region.Body.Instructions.Any(x => x.Operand is FieldReference f && f.Name == "targetRegionSceneName"), "region method may return pending travel destination");
var active = Field("GameManager", "activeExpansions");
Check(active.IsPrivate && !active.IsStatic && active.FieldType.FullName == "Unity.Netcode.NetworkList`1<System.Int64>", "private replicated unlock list reflection");
var getExpansions = Method("GameManager", "GetActiveExpansions");
Check(getExpansions.IsPublic && getExpansions.ReturnType.FullName == "System.Collections.Generic.List`1<ExpansionSO>", "public unlock snapshot return type");
PublicField("ExpansionSO", "id", "Int64");
Check(Calls(Method("GameManager", "OnActiveExpansionsChanged", 1), "CheckExpansions"), "unlock event applies object activation");
var apply = Method("GameManager", "CheckExpansions");
foreach (string field in new[] { "gameObjectsToEnable", "gameObjectsToDisable" })
    Check(apply.Body.Instructions.Any(x => x.Operand is FieldReference f && f.Name == field), "expansion applies " + field);
Check(Calls(apply, "SetActive"), "expansion uses game object activation");
Check(Calls(Method("GameManager", "OnNetworkSpawn"), "add_OnListChanged"), "native spawn subscribes replicated unlock events");

PublicField("GameManager", "mapSO", "MapSO");
PublicField("GameManager", "exampleCharacterCamera", "ExampleCharacterCamera");
PublicField("MapSO", "id", "Int32");
PublicField("MapSO", "sceneName", "String");
PublicField("ExampleCharacterSetup", "customCharacterController", "CustomCharacterController");
PublicField("ExampleCharacterSetup", "overlayRegionLoading", "GameObject");
PublicField("UIManager", "textCoins", "TextMeshProUGUI");
foreach (string name in new[] { "panelNotifications", "prefabNotification", "panelTutorial", "toastHint", "panelStats" })
    PublicField("UIManager", name, "GameObject");
PublicField("UIManager", "hints", "Transform");
bool UsesField(MethodDefinition method, string type, string name) => method.HasBody && method.Body.Instructions.Any(i => i.Operand is FieldReference f && f.DeclaringType.Name == type && f.Name == name);
MethodDefinition Coroutine(string name) => Type("UIManager").NestedTypes.Single(t => t.Name.StartsWith("<" + name + ">d__", StringComparison.Ordinal)).Methods.Single(m => m.Name == "MoveNext");
var notifications = Coroutine("ShowNotificationEnum");
Check(UsesField(notifications, "UIManager", "panelNotifications") && UsesField(notifications, "UIManager", "prefabNotification") && Calls(notifications, "Instantiate"), "native notification coroutine instantiates under its public container");
Check(Calls(notifications, "Disable"), "native notification lifetime retains tween dismissal");
var hint = Coroutine("ShowHint");
Check(UsesField(hint, "UIManager", "toastHint") && Calls(hint, "Show") && Calls(hint, "Disable"), "native top hint uses toast tween lifecycle");
Check(UsesField(Method("UIManager", "ShowTutorialPanel", 1), "UIManager", "panelTutorial") && Calls(Method("UIManager", "ShowTutorialPanel", 1), "SetActive"), "native task tutorial uses its public panel");
foreach (string name in new[] { "from", "to" }) PublicField("UITweener", name, "Vector3");
PublicField("UITweener", "animationType", "UIAnimationTypes");
PublicField("UITweener", "objectToAnimate", "GameObject");
var tweenMove = Method("UITweener", "MoveAbsolute");
Check(Calls(tweenMove, "set_anchoredPosition") && Calls(tweenMove, "move"), "native move tween drives anchored position, so reflow must preserve it");
Check(!Calls(tweenMove, "set_anchorMin") && !Calls(tweenMove, "set_anchorMax"), "native move tween leaves anchors available for reversible HUD offsets");
var tweenSwap = Method("UITweener", "SwapDirection");
Check(UsesField(tweenSwap, "UITweener", "from") && UsesField(tweenSwap, "UITweener", "to"), "native dismissal swaps tween endpoints");
PublicField("SceneSettings", "panelLoading", "GameObject");
PublicField("SceneSettings", "panelMultiplayerLoading", "GameObject");
PublicField("InputManager", "inputMaster", "InputMaster");
Check(Type("ExampleCharacterSetup").BaseType.Name == "NetworkBehaviour", "player network ownership properties");
Check(Method("UIManager", "IsAnyPanelActive").ReturnType.FullName == "System.Boolean", "native panel gate");
var control = Method("ExampleCharacterSetup", "DisablePlayerControl", 1);
Check(control.IsPublic && control.Parameters[0].ParameterType.FullName == "System.Boolean" && control.ReturnType.FullName == "System.Void", "player control bool API");
foreach (string call in new[] { "IsAnyPanelActive", "set_lockState", "set_visible", "Disable", "Enable" }) Check(Calls(control, call), "control API behavior " + call);
foreach (string property in new[] { "Player", "UI" }) Check(Type("InputMaster").Properties.Any(x => x.Name == property && x.GetMethod.IsPublic), "input map " + property);
foreach (string property in new[] { "Settings", "Close", "Map" }) Check(Type("UIActions").Properties.Any(x => x.Name == property && x.PropertyType.FullName == "UnityEngine.InputSystem.InputAction"), "UI action " + property);
foreach (string method in new[] { "Enable", "Disable" }) Check(Type("PlayerActions").Methods.Any(x => x.Name == method && x.Parameters.Count == 0), "player actions " + method);
Check(Type("PlayerActions").Properties.Any(x => x.Name == "enabled" && x.PropertyType.FullName == "System.Boolean"), "player map enabled state");
var action = All(input).Single(x => x.FullName == "UnityEngine.InputSystem.InputAction");
foreach (string method in new[] { "Enable", "Disable", "WasPerformedThisFrame" }) Check(action.Methods.Any(x => x.Name == method && x.Parameters.Count == 0), "Unity input action " + method);
Check(action.Properties.Any(x => x.Name == "enabled"), "action enabled state preservation");
string json = Type("InputMaster").Methods.Where(x => x.IsConstructor && x.HasBody).SelectMany(x => x.Body.Instructions).Select(x => x.Operand).OfType<string>().Single(x => x.Contains("\"maps\""));
using (var document = JsonDocument.Parse(json))
{
    var ui = document.RootElement.GetProperty("maps").EnumerateArray().Single(x => x.GetProperty("name").GetString() == "UI");
    foreach (var pair in new[] { ("Settings", "<Keyboard>/escape"), ("Close", "<Keyboard>/escape"), ("Map", "<Keyboard>/m") })
        Check(ui.GetProperty("bindings").EnumerateArray().Any(x => x.GetProperty("action").GetString() == pair.Item1 && x.GetProperty("path").GetString() == pair.Item2), "native default " + pair.Item1 + " binding");
}
Check(Calls(Method("SettingsManager", "Update"), "OpenPausePanel"), "Escape native pause consumer");

var raycast = All(physics).Single(x => x.FullName == "UnityEngine.Physics").Methods;
Check(raycast.Any(x => x.Name == "Raycast" && x.Parameters.Select(p => p.ParameterType.FullName).SequenceEqual(new[] { "UnityEngine.Vector3", "UnityEngine.Vector3", "UnityEngine.RaycastHit&", "System.Single", "System.Int32", "UnityEngine.QueryTriggerInteraction" })), "bounded trigger-aware Physics.Raycast signature");
var ignore = All(physics).Single(x => x.FullName == "UnityEngine.QueryTriggerInteraction").Fields.Single(x => x.Name == "Ignore");
Check(Convert.ToInt32(ignore.Constant) == 1, "Ignore triggers enum value");
var pluginMethods = All(plugin).SelectMany(x => x.Methods).Where(x => x.HasBody).ToArray();
foreach (var method in pluginMethods)
{
    Check(!method.Body.Instructions.Any(x => x.Operand is MethodReference m && (m.Name.EndsWith("ServerRpc") || m.Name.EndsWith("ClientRpc"))), "no gameplay RPC: " + method.FullName);
    Check(!method.Body.Instructions.Any(x => x.Operand is MethodReference m && m.DeclaringType.Name == "SaveManager" && (m.Name.Contains("Save") || m.Name.Contains("Delete"))), "no native save mutation: " + method.FullName);
}
var projection = All(plugin).Single(x => x.Name == "WorldTargetProjection").Methods.Single(x => x.Name == "Refresh");
Check(Calls(projection, "Raycast") && Calls(projection, "WorldToViewportPoint"), "loaded-surface projection API");
var pluginType = All(plugin).Single(x => x.Name == "Plugin");
Check(Calls(pluginType.Methods.Single(x => x.Name == "SuppressEscapeActions"), "Disable"), "plugin suppresses native escape actions");
Check(Calls(pluginType.Methods.Single(x => x.Name == "RestoreEscapeActions"), "get_frameCount"), "escape restoration uses frame boundary");
Check(pluginMethods.Any(x => x.Body.Instructions.Any(i => i.Operand is MethodReference m && m.Name == "add_OnListChanged")), "plugin subscribes unlock event");
var reflow = All(plugin).Single(x => x.Name == "NativeHudReflow");
var reflowMethods = Types(reflow).SelectMany(x => x.Methods).Where(x => x.HasBody).ToArray();
var reflowCalls = reflowMethods.SelectMany(x => x.Body.Instructions).Select(x => x.Operand).OfType<MethodReference>().ToArray();
foreach (string name in new[] { "SetActive", "SetParent", "Destroy", "set_anchoredPosition", "set_sizeDelta", "set_pivot", "set_text", "Show", "Disable" })
    Check(!reflowCalls.Any(x => x.Name == name), "HUD reflow does not replace native visibility/content/tween ownership: " + name);
Check(reflowCalls.Any(x => x.Name == "set_anchorMin") && reflowCalls.Any(x => x.Name == "set_anchorMax"), "HUD reflow offsets native anchors");
Check(!reflowMethods.SelectMany(x => x.Body.Instructions).Any(i => i.OpCode.Code == Mono.Cecil.Cil.Code.Stfld && i.Operand is FieldReference f && f.DeclaringType.Name == "UITweener"), "HUD reflow never writes native tween endpoints or settings");
Check(Calls(reflow.Methods.Single(x => x.Name == "Dispose"), "Restore"), "HUD reflow disposal restores anchors");
var anchorEntry = reflow.NestedTypes.Single(x => x.Name == "Entry");
foreach (string name in new[] { "Apply", "Restore" })
    Check(Calls(anchorEntry.Methods.Single(x => x.Name == name), "CheckOwnership"), "HUD anchor ownership is checked before " + name);
var ownership = anchorEntry.Methods.Single(x => x.Name == "CheckOwnership");
Check(Calls(ownership, "get_anchorMin") && Calls(ownership, "get_anchorMax") && UsesField(ownership, "Entry", "Conflicted"), "HUD reflow detects another writer and relinquishes anchors");
Console.WriteLine($"Passed {checks} Navigation installed-assembly contract checks; metadata/IL only, no Unity execution.");
