using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OldMarket.CheckoutAll
{
    internal sealed class CheckoutHint
    {
        private TextMeshProUGUI text;
        private RectTransform parent;
        private readonly Vector3[] corners = new Vector3[4];

        public void Show(PlayerInteraction player, string message)
        {
            var native = player != null ? player.textPrimaryInteraction : null;
            if (message == null || native == null || !native.gameObject.activeInHierarchy || player.interactionUI == null)
            {
                if (text != null) text.gameObject.SetActive(false);
                return;
            }
            var root = player.interactionUI.transform as RectTransform;
            if (root == null) return;
            if (text == null || parent != root)
            {
                Destroy();
                parent = root;
                var host = new GameObject("CheckoutAllHint", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
                host.transform.SetParent(root, false);
                host.GetComponent<LayoutElement>().ignoreLayout = true;
                text = host.GetComponent<TextMeshProUGUI>();
                text.raycastTarget = false;
                text.enableAutoSizing = false;
                text.textWrappingMode = TextWrappingModes.NoWrap;
                text.alignment = TextAlignmentOptions.TopLeft;
                text.overflowMode = TextOverflowModes.Overflow;
            }
            text.gameObject.SetActive(true);
            text.font = native.font;
            text.fontSharedMaterial = native.fontSharedMaterial;
            text.fontSize = native.fontSize;
            text.fontStyle = native.fontStyle;
            text.fontWeight = native.fontWeight;
            text.characterSpacing = native.characterSpacing;
            // Native interaction availability must not tint our independent hold/toggle hint.
            // Only the toggle-off fragment supplies its own status color.
            text.color = Color.white;
            if (text.text != message) text.text = message;

            float left = float.PositiveInfinity, bottom = float.PositiveInfinity;
            void Include(RectTransform rect)
            {
                if (rect == null || !rect.gameObject.activeInHierarchy) return;
                rect.GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    Vector3 point = root.InverseTransformPoint(corner);
                    left = Mathf.Min(left, point.x);
                    bottom = Mathf.Min(bottom, point.y);
                }
            }
            Include(native.rectTransform);
            if (player.controlSlotPrimary != null) Include(player.controlSlotPrimary.transform as RectTransform);
            if (player.textSecondaryInteraction != null && !string.IsNullOrEmpty(player.textSecondaryInteraction.text))
                Include(player.textSecondaryInteraction.rectTransform);
            if (player.controlSlotSecondary != null) Include(player.controlSlotSecondary.transform as RectTransform);
            if (player.textSubtitle != null && !string.IsNullOrEmpty(player.textSubtitle.text)) Include(player.textSubtitle.rectTransform);

            var target = text.rectTransform;
            // Anchor to the parent's local origin so the measured native button bounds are in the same coordinates.
            target.anchorMin = target.anchorMax = root.pivot;
            target.pivot = new Vector2(0, 1);
            target.anchoredPosition = new Vector2(left, bottom - 8);
            Vector2 size = text.GetPreferredValues(message);
            target.sizeDelta = size + new Vector2(8, 8);
        }

        public void Destroy()
        {
            if (text != null) { text.gameObject.SetActive(false); Object.Destroy(text.gameObject); }
            text = null; parent = null;
        }
    }
}
