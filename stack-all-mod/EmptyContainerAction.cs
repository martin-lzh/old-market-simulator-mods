using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;

namespace OldMarket.StackAll
{
    internal static class EmptyContainerAction
    {
        private static readonly ConditionalWeakTable<PlayerInventory, RepeatGate> Gates =
            new ConditionalWeakTable<PlayerInventory, RepeatGate>();
        internal static bool IsReusable(PlayerInventory inventory) => GameManager.Instance != null &&
            GameManager.Instance.GetItemById(inventory.GetCurrentInventorySlot().itemId) is ProductSO product &&
            !product.destroyWhenEmpty;

        internal static bool Available(PlayerInventory inventory, int group) =>
            EmptyContainerPlan.Find(inventory.slots.Count, inventory.maxSlots, group,
                inventory.GetCurrentInventorySlot().itemId, IsReusable(inventory), i => inventory.slots[i]) >= 0;

        internal static bool Tick(PlayerInventory inventory, int group, bool batching)
        {
            bool held = false, pressed = false;
            var keyboard = Keyboard.current;
            if (keyboard != null)
                foreach (var key in keyboard.allKeys)
                    if (key != null && key.keyCode == Key.G)
                    { held = key.isPressed; pressed = key.wasPressedThisFrame; break; }
            int count = EmptyContainerPlan.Count(inventory.slots.Count, inventory.maxSlots, group,
                inventory.GetCurrentInventorySlot().itemId, IsReusable(inventory), i => inventory.slots[i]);
            return Gates.GetValue(inventory, _ => new RepeatGate()).Tick(held, pressed,
                !batching && Allowed(inventory), group, inventory.GetCurrentInventorySlot().itemId, count, Time.unscaledTime);
        }

        private static bool Allowed(PlayerInventory inventory)
        {
            if (!inventory.IsOwner || !inventory.IsSpawned || !Application.isFocused || Time.timeScale <= 0 ||
                Cursor.lockState != CursorLockMode.Locked || inventory.NetworkManager == null ||
                !inventory.NetworkManager.IsListening || InputManager.Instance == null ||
                UIManager.Instance == null || UIManager.Instance.IsAnyPanelActive() ||
                inventory.controlHintPanel == null || !inventory.controlHintPanel.activeInHierarchy) return false;
            var controls = InputManager.Instance.inputMaster.Player;
            // Do not combine container disposal with an ongoing native inventory action.
            if (!controls.enabled || controls.Drop.IsPressed() || controls.Throw.IsPressed() ||
                controls.Place.IsPressed() || controls.Refill.IsPressed() ||
                controls.PrimaryInteraction.IsPressed() || controls.SecondaryInteraction.IsPressed()) return false;
            return true;
        }
    }
}
