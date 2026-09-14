using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace OldMarket.StackAll
{
    internal static class ContainerView
    {
        private sealed class Labels
        {
            public TextMeshProUGUI Right;
            public TextAlignmentOptions Alignment;
            public float FontSize, FontMin, FontMax;
            public bool AutoSize;
            public Vector2 AnchorMin, AnchorMax, OffsetMin, OffsetMax, Pivot;
        }
        private static readonly Dictionary<ItemSlot, Labels> Views = new Dictionary<ItemSlot, Labels>();

        internal static void Update(ItemSlot __instance, InventorySlot inventorySlot, int index)
        {
            if (__instance.textAmount == null) return;
            var network = NetworkManager.Singleton;
            var player = network != null && network.LocalClient != null ? network.LocalClient.PlayerObject : null;
            var inventory = player != null ? player.GetComponent<PlayerInventory>() : null;
            var product = inventorySlot.itemId != -1 && GameManager.Instance != null
                ? GameManager.Instance.GetItemById(inventorySlot.itemId) as ProductSO : null;
            if (inventory == null || inventory.itemSlots == null || !__instance.transform.IsChildOf(inventory.itemSlots.transform)) return;
            if (product == null || index < 0 || index >= inventory.maxSlots)
            {
                Restore(__instance);
                return;
            }
            if (!Views.TryGetValue(__instance, out var view))
            {
                var original = __instance.textAmount;
                var rect = original.rectTransform;
                view = new Labels { Alignment = original.alignment, FontSize = original.fontSize, FontMin = original.fontSizeMin,
                    FontMax = original.fontSizeMax, AutoSize = original.enableAutoSizing, AnchorMin = rect.anchorMin, AnchorMax = rect.anchorMax,
                    OffsetMin = rect.offsetMin, OffsetMax = rect.offsetMax, Pivot = rect.pivot };
                view.Right = Object.Instantiate(original, original.transform.parent);
                view.Right.name = "StackAllContainerCount";
                view.Right.raycastTarget = false;
                Views.Add(__instance, view);
            }
            int count = 0;
            long total = 0;
            for (int p = 0; p < ContainerPlan.Limit; p++)
            {
                int backing = ContainerPlan.Index(inventory.maxSlots, index, p);
                if (backing >= inventory.slots.Count) break;
                var entry = inventory.slots[backing];
                if (entry.itemId == -1) continue;
                count++;
                total += System.Math.Max(0, entry.amount);
            }
            Place(__instance.textAmount, true, view.FontSize, total >= 100);
            Place(view.Right, false, view.FontSize, false);
            view.Right.font = __instance.textAmount.font;
            view.Right.fontSharedMaterial = __instance.textAmount.fontSharedMaterial;
            __instance.textAmount.text = total.ToString();
            view.Right.text = count.ToString();
            // Whole fish and other single-use products are individual items, not reusable
            // containers. Keep their physical count on the right without duplicating it.
            __instance.textAmount.gameObject.SetActive(product.amount != 1 || !product.destroyWhenEmpty);
            view.Right.gameObject.SetActive(true);
        }

        private static void Place(TextMeshProUGUI label, bool left, float fontSize, bool fitContents)
        {
            var rect = label.rectTransform;
            float height = Mathf.Max(20f, fontSize * 1.4f);
            // Two-digit counts keep native size. Only longer goods totals may shrink
            // within the left half, leaving room for the right count in a 64px slot.
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(fitContents ? .5f : 1, 0);
            rect.pivot = new Vector2(.5f, 0);
            rect.offsetMin = new Vector2(3, 2);
            rect.offsetMax = new Vector2(-3, height + 2);
            label.alignment = left ? TextAlignmentOptions.BottomLeft : TextAlignmentOptions.BottomRight;
            label.enableAutoSizing = fitContents;
            label.fontSize = fontSize;
            label.fontSizeMin = fitContents ? Mathf.Min(9, fontSize) : fontSize;
            label.fontSizeMax = fontSize;
        }
        private static void Restore(ItemSlot slot)
        {
            if (!Views.TryGetValue(slot, out var view)) return;
            if (view.Right != null) view.Right.gameObject.SetActive(false);
            if (slot != null && slot.textAmount != null)
            {
                var rect = slot.textAmount.rectTransform;
                slot.textAmount.alignment = view.Alignment;
                slot.textAmount.enableAutoSizing = view.AutoSize;
                slot.textAmount.fontSize = view.FontSize;
                slot.textAmount.fontSizeMin = view.FontMin;
                slot.textAmount.fontSizeMax = view.FontMax;
                rect.anchorMin = view.AnchorMin; rect.anchorMax = view.AnchorMax;
                rect.pivot = view.Pivot; rect.offsetMin = view.OffsetMin; rect.offsetMax = view.OffsetMax;
            }
        }
        internal static void Clear()
        {
            foreach (var entry in Views)
            {
                Restore(entry.Key);
                if (entry.Value.Right != null) Object.Destroy(entry.Value.Right.gameObject);
            }
            Views.Clear();
        }
    }
}
