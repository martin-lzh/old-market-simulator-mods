using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OldMarket.StackAll
{
    internal static class RepeatActions
    {
        private sealed class Gates { public readonly RepeatGate Drop = new RepeatGate(), Throw = new RepeatGate(); }
        private static readonly ConditionalWeakTable<PlayerInventory, Gates> State = new ConditionalWeakTable<PlayerInventory, Gates>();
        private static readonly FieldInfo Current = AccessTools.Field(typeof(PlayerInventory), "currentSlot");

        public static bool ShouldAct(InputAction action, PlayerInventory inventory)
        {
            var controls = InputManager.Instance.inputMaster.Player;
            var gates = State.GetValue(inventory, _ => new Gates());
            var gate = action == controls.Drop ? gates.Drop : gates.Throw;
            int index = ((NetworkVariable<int>)Current.GetValue(inventory)).Value;
            var slot = inventory.GetCurrentInventorySlot();
            int units = Units(inventory);
            bool allowed = inventory.IsOwner && inventory.IsSpawned && Application.isFocused && Time.timeScale > 0 &&
                Cursor.lockState == CursorLockMode.Locked && UIManager.Instance != null && !UIManager.Instance.IsAnyPanelActive() &&
                !(controls.Drop.IsPressed() && controls.Throw.IsPressed());
            return gate.Tick(action.IsPressed(), action.WasPerformedThisFrame(), allowed, index, slot.itemId, units, Time.unscaledTime);
        }

        internal static int Units(PlayerInventory inventory)
        {
            int index = ((NetworkVariable<int>)Current.GetValue(inventory)).Value;
            var slot = inventory.GetCurrentInventorySlot();
            var definition = GameManager.Instance.GetItemById(slot.itemId);
            int units = definition is ToolSO ? 1 : System.Math.Max(0, slot.amount);
            if (definition is ProductSO)
            {
                units = 0;
                for (int p = 0; p < ContainerPlan.Limit; p++)
                {
                    int backing = ContainerPlan.Index(inventory.maxSlots, index, p);
                    if (backing >= inventory.slots.Count) break;
                    if (inventory.slots[backing].itemId != -1) units++;
                }
            }
            return units;
        }

        internal static IEnumerable<CodeInstruction> InputGate(IEnumerable<CodeInstruction> instructions)
        {
            var result = instructions.ToList();
            var performed = AccessTools.Method(typeof(InputAction), "WasPerformedThisFrame");
            int replaced = 0;
            for (int i = 0; i < result.Count; i++)
                if (result[i].Calls(performed))
                {
                    // Keep the original instruction's branch labels on the new argument load.
                    result[i].opcode = OpCodes.Ldarg_0;
                    result[i].operand = null;
                    result.Insert(i + 1, new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(RepeatActions), nameof(ShouldAct))));
                    i++; replaced++;
                }
            if (replaced != 1) throw new System.InvalidOperationException("Drop/throw input branch changed");
            return result;
        }
    }
}
