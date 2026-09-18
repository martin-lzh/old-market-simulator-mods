using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using BepInEx;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace OldMarket.StackAll
{
    [BepInPlugin(Id, "Old Market Stack All", "0.3.0")]
    [BepInProcess("Old Market Simulator.exe")]
    public sealed class Plugin : BaseUnityPlugin
    {
        private const string Id = "local.oldmarket.stackall";
        private const string GameHash = "FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296";
        private static readonly FieldInfo Stage = AccessTools.Field(typeof(NetworkBehaviour), "__rpc_exec_stage");
        private static readonly HashSet<PlayerInventory> Batching = new HashSet<PlayerInventory>();
        private static readonly MethodInfo Refresh = AccessTools.Method(typeof(PlayerInventory), "SetSlot");
        private Harmony harmony;
        private static readonly ActionHint Hint = new ActionHint();

        private void Awake()
        {
            try
            {
                using (var stream = File.OpenRead(typeof(PlayerInventory).Assembly.Location))
                using (var sha = SHA256.Create())
                    if (BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "") != GameHash)
                        throw new InvalidOperationException("Game assembly changed; Stack All requires a compatibility review.");
                if (Stage == null || !Stage.FieldType.IsEnum ||
                    !Enum.GetNames(Stage.FieldType).Contains("Execute") || !Enum.GetNames(Stage.FieldType).Contains("Send"))
                    throw new InvalidOperationException("Unsupported RPC execution stages.");
                harmony = new Harmony(Id);
                Patch(typeof(PlayerInventory), "LoadPlayerItemsServerRpc", prefix: nameof(PrepareStorage));
                Patch(typeof(PlayerInventory), "GiveItemClientRpc", prefix: nameof(Receive));
                Patch(typeof(PlayerInventory), "LoadPlayerItemsClientRpc", prefix: nameof(BeforeLoad), postfix: nameof(AfterLoad), finalizer: nameof(EndFailedBatch));
                Patch(typeof(PlayerInventory), "OnSlotsChanged", prefix: nameof(BeforeChanged));
                Patch(typeof(PlayerInventory), "LateUpdate", postfix: nameof(UpdateHints));
                Patch(typeof(PlayerInventory), "OnNetworkDespawn", prefix: nameof(ClearPlayerUi));
                Patch(typeof(PlayerInventory), "ReduceCurrentItemAmount", prefix: nameof(Reduce));
                Patch(typeof(PlayerInventory), "RefillCurrentItemServer", prefix: nameof(Refill));
                Patch(typeof(PlayerInventory), "RemoveItem", prefix: nameof(BeforeRemove), postfix: nameof(AfterRemove), finalizer: nameof(EndFailedBatch));
                Patch(typeof(PlayerInventory), "CheckBag", prefix: nameof(BeforeBag), postfix: nameof(AfterBag));
                foreach (string method in new[] { "SetSlot", "CheckBag", "OnCurrentSlotChanged", "OnNetworkSpawn" })
                    Patch(typeof(PlayerInventory), method, transpiler: nameof(VisibleLoops));
                Patch(typeof(SaveManager), "SavePlayerData", prefix: nameof(TrimSave));
                harmony.Patch(AccessTools.Method(typeof(ItemSlot), "UpdateSlot"),
                    postfix: new HarmonyMethod(typeof(ContainerView), "Update"));
                foreach (string method in new[] { "CheckBag", "HandleDrop", "HandleThrow", "HandleItemPlacement" })
                    Patch(typeof(PlayerInventory), method, transpiler: nameof(StackReads));
                foreach (string method in new[] { "HandleDrop", "HandleThrow" })
                    harmony.Patch(AccessTools.Method(typeof(PlayerInventory), method),
                        transpiler: new HarmonyMethod(typeof(RepeatActions), "InputGate"));
                // These interactions consume exactly one object. Whole-slot operations (horse cargo,
                // trash and network updates) intentionally keep the native UseItem implementation.
                Patch(typeof(PlayerInventory), "HandleNewBlockPlacement", transpiler: nameof(ConsumeCalls));
                foreach (var type in new[] { typeof(Aquarium), typeof(OrigamiStand), typeof(BlockBeeHive) })
                    Patch(type, "Interact", transpiler: nameof(ConsumeCalls));
                Patch(typeof(ItemCrate), "EnableDummyItems", prefix: nameof(CapPreview));
                Logger.LogInfo("Stack All 0.3.0 ready: goods on left, physical containers on right; 64 containers per product slot. All peers need 0.3.0.");
            }
            catch (Exception error)
            {
                harmony?.UnpatchSelf();
                Logger.LogError("Stack All disabled without modifying game assets: " + error);
            }
        }

        private void Patch(Type type, string method, string prefix = null, string postfix = null, string transpiler = null, string finalizer = null)
        {
            var target = AccessTools.Method(type, method) ?? throw new MissingMethodException(type.Name, method);
            harmony.Patch(target, prefix: prefix == null ? null : new HarmonyMethod(typeof(Plugin), prefix),
                postfix: postfix == null ? null : new HarmonyMethod(typeof(Plugin), postfix),
                transpiler: transpiler == null ? null : new HarmonyMethod(typeof(Plugin), transpiler),
                finalizer: finalizer == null ? null : new HarmonyMethod(typeof(Plugin), finalizer), ilmanipulator: null);
        }

        private void OnDestroy() { harmony?.UnpatchSelf(); Batching.Clear(); ContainerView.Clear(); Hint.Destroy(); }
        private static void UpdateHints(PlayerInventory __instance, NetworkVariable<int> ___currentSlot)
        {
            if (!__instance.IsOwner || !__instance.IsSpawned) return;
            int group = ___currentSlot.Value;
            if (GameManager.Instance == null || group < 0 || group >= __instance.maxSlots || group >= __instance.slots.Count)
            { Hint.Destroy(); return; }
            if (EmptyContainerAction.Tick(__instance, group, Batching.Contains(__instance)))
            {
                var next = Snapshot(__instance);
                if (EmptyContainerPlan.Take(next, __instance.maxSlots, group,
                    __instance.GetCurrentInventorySlot().itemId, EmptyContainerAction.IsReusable(__instance), out var empty))
                {
                    // Remove only the selected empty physical record, then use the existing spawn RPC.
                    // Filled containers and their metadata never pass through native UseItem.
                    Apply(__instance, next, true);
                    SpillContainer(__instance, empty);
                }
            }
            Hint.Show(__instance, EmptyContainerAction.Available(__instance, group));
        }
        private static void ClearPlayerUi(PlayerInventory __instance)
        {
            if (__instance.IsOwner) { Hint.Destroy(); ContainerView.Clear(); }
        }
        private static bool Executing(PlayerInventory inventory) => inventory.IsOwner && inventory.IsSpawned &&
            inventory.NetworkManager != null && inventory.NetworkManager.IsListening && Stage.GetValue(inventory).ToString() == "Execute";

        private static bool BeforeChanged(PlayerInventory __instance, NetworkListEvent<InventorySlot> changeEvent, int ___activeSlots)
        {
            if (Batching.Contains(__instance)) return false;
            if (__instance.IsOwner && __instance.slots.Count == ContainerPlan.StorageSize(__instance.maxSlots) && changeEvent.Index >= 0)
            {
                int group = ContainerPlan.Group(__instance.maxSlots, changeEvent.Index);
                if (group < Math.Min(___activeSlots, __instance.maxSlots)) Promote(__instance, group);
            }
            return true;
        }
        private static InventorySlot Empty() => new InventorySlot { itemId = -1, amount = -1, cost = -1, dayCounter = -1 };
        private static bool Eligible(InventorySlot slot) => slot.itemId != -1 && slot.amount >= 0 &&
            GameManager.Instance.GetItemById(slot.itemId) is ItemSO item && !(item is ToolSO) && (slot.amount > 0 || item is ProductSO);

        private static void PrepareStorage(PlayerInventory __instance)
        {
            if (!__instance.IsOwner || !__instance.IsSpawned) return;
            int size = ContainerPlan.StorageSize(__instance.maxSlots);
            if (__instance.slots.Count > size) throw new InvalidOperationException("Unexpected inventory storage size");
            Batching.Add(__instance);
            try { while (__instance.slots.Count < size) __instance.slots.Add(Empty()); }
            finally { Batching.Remove(__instance); }
        }

        // Prefix only intercepts the received client RPC. Sending, recipients and native network
        // serialization remain intact; restore Send before nested RPCs, just as the native body does.
        private static bool Receive(PlayerInventory __instance, long itemId, int amount, int cost, int dayCounter,
            int ___activeSlots, NetworkVariable<int> ___currentSlot)
        {
            var incoming = new InventorySlot { itemId = itemId, amount = amount, cost = cost, dayCounter = dayCounter };
            if (!Executing(__instance)) return true;
            if (GameManager.Instance.GetItemById(itemId) is ToolSO && amount >= 0)
            {
                Stage.SetValue(__instance, Enum.Parse(Stage.FieldType, "Send"));
                PrepareStorage(__instance);
                var toolSlots = Snapshot(__instance);
                if (ContainerPlan.PutSingle(toolSlots, __instance.maxSlots, ___activeSlots, ___currentSlot.Value, incoming)) Apply(__instance, toolSlots);
                else SpillContainer(__instance, incoming);
                return false;
            }
            if (!Eligible(incoming)) return true;
            Stage.SetValue(__instance, Enum.Parse(Stage.FieldType, "Send"));
            PrepareStorage(__instance);
            var slots = Snapshot(__instance);
            int remaining;
            var definition = GameManager.Instance.GetItemById(itemId);
            if (PacketRules.IsPacket(definition))
                remaining = PacketRules.Add(slots, __instance.maxSlots, ___activeSlots, ___currentSlot.Value,
                    incoming, definition) ? 0 : 1;
            else remaining = StackPlan.Add(slots, ___activeSlots, ___currentSlot.Value, incoming, CanMerge);
            Apply(__instance, slots);
            if (PacketRules.IsPacket(definition))
            {
                if (remaining > 0) SpillContainer(__instance, incoming);
            }
            else Spill(__instance, incoming, remaining);
            return false;
        }

        private static InventorySlot[] Snapshot(PlayerInventory inventory)
        {
            var result = new InventorySlot[inventory.slots.Count];
            for (int i = 0; i < result.Length; i++) result[i] = inventory.slots[i];
            return result;
        }

        private static void Apply(PlayerInventory inventory, InventorySlot[] slots, bool forceDisplay = false)
        {
            var held = inventory.GetCurrentInventorySlot();
            Batching.Add(inventory);
            try
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    var old = inventory.slots[i];
                    if (!old.Equals(slots[i]) || old.cost != slots[i].cost) inventory.slots[i] = slots[i];
                }
            }
            finally { Batching.Remove(inventory); }
            Refresh.Invoke(inventory, null);
            if (forceDisplay || !held.Equals(inventory.GetCurrentInventorySlot())) AccessTools.Method(typeof(PlayerInventory), "ShowItemBox").Invoke(inventory, null);
        }

        private static bool CanMerge(InventorySlot target, InventorySlot source)
        {
            return StackCompatibility.CanMerge(GameManager.Instance.GetItemById(source.itemId), target, source);
        }

        private static void Spill(PlayerInventory inventory, InventorySlot item, int amount)
        {
            // Spawn in native pack sizes; never destroy overflow or evict an unrelated held item.
            var definition = GameManager.Instance.GetItemById(item.itemId);
            int pack = definition is ProductSO ? Math.Max(1, definition.amount) : 1;
            while (amount > 0)
            {
                int count = Math.Min(pack, amount);
                GameManager.Instance.SpawnItemAtPositionServerRpc(item.itemId,
                    inventory.transform.position + inventory.transform.forward + Vector3.up,
                    true, count, item.cost, item.dayCounter);
                amount -= count;
            }
        }

        private static void BeforeLoad(PlayerInventory __instance, InventorySlot[] items, out bool __state)
        {
            __state = Executing(__instance);
            if (!__state) return;
            if (items == null || items.Length > ContainerPlan.StorageSize(__instance.maxSlots)) throw new InvalidOperationException("Unsupported inventory save length");
            PrepareStorage(__instance);
            Batching.Add(__instance);
            for (int i = 0; i < __instance.slots.Count; i++) __instance.slots[i] = Empty();
        }
        private static void AfterLoad(PlayerInventory __instance, bool __state, int ___activeSlots)
        {
            if (!__state) return;
            Batching.Remove(__instance);
            var original = Snapshot(__instance);
            var next = (InventorySlot[])original.Clone();
            int active = __instance.initialActiveSlots;
            foreach (var entry in original)
                if (GameManager.Instance.GetItemById(entry.itemId) is BagSO bag) active = Math.Max(active, bag.activeSlots);
            active = Math.Min(active, __instance.maxSlots);
            for (int i = 0; i < next.Length; i++) if (Eligible(next[i])) next[i] = Empty();
            var overflow = new List<InventorySlot>();
            for (int i = 0; i < original.Length; i++)
            {
                if (!Eligible(original[i])) continue;
                var rest = original[i];
                var definition = GameManager.Instance.GetItemById(rest.itemId);
                if (PacketRules.IsPacket(definition))
                {
                    if (!PacketRules.Add(next, __instance.maxSlots, active, ContainerPlan.Group(__instance.maxSlots, i), rest, definition)) overflow.Add(rest);
                }
                else
                {
                    rest.amount = StackPlan.Add(next, active, i, rest, CanMerge);
                    if (rest.amount > 0) overflow.Add(rest);
                }
            }
            Apply(__instance, next, true);
            foreach (var rest in overflow)
                if (PacketRules.IsPacket(GameManager.Instance.GetItemById(rest.itemId))) SpillContainer(__instance, rest);
                else Spill(__instance, rest, rest.amount);
        }

        private static bool Reduce(PlayerInventory __instance, NetworkVariable<int> ___currentSlot)
        {
            var slot = __instance.GetCurrentInventorySlot();
            if (!__instance.IsOwner || !Eligible(slot) || slot.amount <= 0) return true;
            slot.amount--;
            bool keepEmpty = GameManager.Instance.GetItemById(slot.itemId) is ProductSO product && !product.destroyWhenEmpty;
            __instance.slots[___currentSlot.Value] = slot.amount > 0 || keepEmpty ? slot : Empty();
            return false;
        }

        public static void ConsumeOne(PlayerInventory inventory)
        {
            if (Eligible(inventory.GetCurrentInventorySlot())) inventory.ReduceCurrentItemAmount();
            else inventory.UseItem();
        }

        private static bool Refill(PlayerInventory __instance, long sourceItemId, int sourceAmount,
            int sourceDayCounter, int sourceCost, ref bool __result, int ___activeSlots)
        {
            if (!__instance.IsServer || !(GameManager.Instance.GetItemById(sourceItemId) is ProductSO product) || sourceAmount <= 0)
                return true;
            // Harvest callers retain the harvest in the world if the whole request cannot fit.
            __result = false;
            for (int i = 0; i < __instance.slots.Count; i++)
            {
                if (ContainerPlan.Group(__instance.maxSlots, i) >= ___activeSlots) continue;
                var slot = __instance.slots[i];
                var source = new InventorySlot { itemId = sourceItemId, amount = sourceAmount, cost = sourceCost, dayCounter = sourceDayCounter };
                if (slot.itemId != sourceItemId || slot.amount < 0 || (long)slot.amount + sourceAmount > product.amount || !CanMerge(slot, source)) continue;
                __instance.UpdateSlotClientRpc(i, sourceItemId, slot.amount + sourceAmount,
                    StackRules.Cost(slot.amount, slot.cost, sourceAmount, sourceCost),
                    StackRules.Age(slot.amount, slot.dayCounter, sourceAmount, sourceDayCounter),
                    new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { __instance.OwnerClientId } } });
                __result = true;
                break;
            }
            return false;
        }

        private static void Promote(PlayerInventory inventory, int group)
        {
            var next = Snapshot(inventory);
            ContainerPlan.Promote(next, inventory.maxSlots, group);
            Batching.Add(inventory);
            try
            {
                for (int p = 0; p < ContainerPlan.Limit; p++)
                {
                    int i = ContainerPlan.Index(inventory.maxSlots, group, p);
                    var old = inventory.slots[i];
                    if (!old.Equals(next[i]) || old.cost != next[i].cost) inventory.slots[i] = next[i];
                }
            }
            finally { Batching.Remove(inventory); }
        }

        private static bool BeforeBag(PlayerInventory __instance) => !Batching.Contains(__instance);
        private static void AfterBag(PlayerInventory __instance, int ___activeSlots)
        {
            if (!__instance.IsOwner || Batching.Contains(__instance) || __instance.slots.Count != ContainerPlan.StorageSize(__instance.maxSlots)) return;
            Batching.Add(__instance);
            try
            {
                // Native CheckBag removes each locked front slot. Flush its backing containers
                // too, preserving empty containers and each container's original metadata.
                for (int group = ___activeSlots; group < __instance.maxSlots; group++)
                    for (int p = 1; p < ContainerPlan.Limit; p++)
                    {
                        int i = ContainerPlan.Index(__instance.maxSlots, group, p);
                        var slot = __instance.slots[i];
                        if (slot.itemId == -1) continue;
                        SpillContainer(__instance, slot);
                        __instance.slots[i] = Empty();
                    }
            }
            finally { Batching.Remove(__instance); }
        }

        private static void SpillContainer(PlayerInventory inventory, InventorySlot slot)
        {
            GameManager.Instance.SpawnItemAtPositionServerRpc(slot.itemId,
                inventory.transform.position + inventory.transform.forward + Vector3.up,
                true, slot.amount, slot.cost, slot.dayCounter);
        }

        private static void BeforeRemove(PlayerInventory __instance, out InventorySlot[] __state)
        {
            __state = __instance.IsOwner && !Batching.Contains(__instance) ? Snapshot(__instance) : null;
            if (__state != null) Batching.Add(__instance);
        }
        private static void AfterRemove(PlayerInventory __instance, InventorySlot[] __state)
        {
            if (__state == null) return;
            var next = Snapshot(__instance);
            for (int i = 0; i < next.Length; i++)
                if (__state[i].itemId != -1 && next[i].itemId == -1 &&
                    GameManager.Instance.GetItemById(__state[i].itemId) is ProductSO product && !product.destroyWhenEmpty)
                {
                    next[i] = __state[i];
                    next[i].amount = 0;
                }
            if (next.Length == ContainerPlan.StorageSize(__instance.maxSlots))
                for (int group = 0; group < __instance.maxSlots; group++) ContainerPlan.Promote(next, __instance.maxSlots, group);
            Batching.Remove(__instance);
            Apply(__instance, next, true);
        }
        private static Exception EndFailedBatch(PlayerInventory __instance, Exception __exception)
        {
            if (__exception != null) Batching.Remove(__instance);
            return __exception;
        }

        private static void TrimSave(ulong clientId, PlayerData playerData)
        {
            if (playerData?.items == null) return;
            int width = 8; // Verified native default; prefer this player's actual hotbar width.
            var network = NetworkManager.Singleton;
            if (network != null && network.ConnectedClients.TryGetValue(clientId, out var client) && client.PlayerObject != null)
            {
                var inventory = client.PlayerObject.GetComponent<PlayerInventory>();
                if (inventory != null) width = inventory.maxSlots;
            }
            playerData.items = ContainerPlan.Trim(playerData.items, width);
        }

        public static int VisibleCount(PlayerInventory inventory) => Math.Min(inventory.maxSlots, inventory.slots.Count);
        private static IEnumerable<CodeInstruction> VisibleLoops(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            var field = AccessTools.Field(typeof(PlayerInventory), "slots");
            var count = AccessTools.PropertyGetter(typeof(NetworkList<InventorySlot>), "Count");
            int replaced = 0;
            for (int i = 0; i + 1 < result.Count; i++)
                if (result[i].opcode == OpCodes.Ldfld && Equals(result[i].operand, field) && result[i + 1].Calls(count))
                {
                    result[i].opcode = OpCodes.Nop;
                    result[i].operand = null;
                    result[i + 1].opcode = OpCodes.Call;
                    result[i + 1].operand = AccessTools.Method(typeof(Plugin), nameof(VisibleCount));
                    replaced++;
                }
            if (replaced == 0) throw new InvalidOperationException("Hotbar loop shape changed");
            return result;
        }

        // Product and seed amounts are contents: native drop/place must move the packet intact.
        public static int StackLimit(ItemSO item) => PacketRules.StackLimit(item);

        private static IEnumerable<CodeInstruction> StackReads(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            var field = AccessTools.Field(typeof(ItemSO), "stackSize");
            int replaced = 0;
            foreach (var instruction in result)
                if (instruction.opcode == OpCodes.Ldfld && Equals(instruction.operand, field))
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = AccessTools.Method(typeof(Plugin), nameof(StackLimit));
                    replaced++;
                }
            if (replaced == 0) throw new InvalidOperationException("Inventory stack branch not found.");
            return result;
        }

        private static IEnumerable<CodeInstruction> ConsumeCalls(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            var use = AccessTools.Method(typeof(PlayerInventory), "UseItem");
            int replaced = 0;
            foreach (var instruction in result)
                if (instruction.Calls(use))
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = AccessTools.Method(typeof(Plugin), nameof(ConsumeOne));
                    replaced++;
                }
            if (replaced != 1) throw new InvalidOperationException("Single-item consumption branch changed.");
            return result;
        }

        private static void CapPreview(ItemCrate __instance, ref int amount)
        {
            if (__instance.dummyItems != null) amount = Math.Min(amount, __instance.dummyItems.childCount);
        }
    }
}
