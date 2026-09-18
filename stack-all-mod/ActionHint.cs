using OldMarket.StackAll.Localization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OldMarket.StackAll
{
    internal sealed class ActionHint
    {
        private TextMeshProUGUI text;
        private RectTransform parent;
        private readonly Vector3[] corners = new Vector3[4];
        private float nextBindings;
        private TextMeshProUGUI native;
        private string language;
        private bool lastRepeat, lastContainer, lastEmpty;

        internal void Show(PlayerInventory inventory, bool hasEmpty)
        {
            bool repeat = RepeatActions.Units(inventory) > 1;
            bool container = EmptyContainerAction.IsReusable(inventory);
            var panel = inventory.controlHintPanel != null ? inventory.controlHintPanel.transform as RectTransform : null;
            if (panel == null || !panel.gameObject.activeInHierarchy || !Application.isFocused || Time.timeScale <= 0 ||
                InputManager.Instance == null || UIManager.Instance == null || UIManager.Instance.IsAnyPanelActive() ||
                (!repeat && !container))
            { if (text != null) text.gameObject.SetActive(false); return; }
            var root = panel.parent as RectTransform;
            if (root == null) return;
            if (text == null || parent != root)
            {
                Destroy();
                parent = root;
                native = panel.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(t => t.GetComponentInParent<ControlSlot>() == null);
                if (native == null) return;
                var host = new GameObject("StackAllHoldHints", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
                host.transform.SetParent(root, false);
                host.GetComponent<LayoutElement>().ignoreLayout = true;
                text = host.GetComponent<TextMeshProUGUI>();
                text.font = native.font;
                text.fontSharedMaterial = native.fontSharedMaterial;
                text.fontSize = native.fontSize;
                text.fontStyle = native.fontStyle;
                text.fontWeight = native.fontWeight;
                text.color = native.color;
                text.raycastTarget = false;
                text.textWrappingMode = TextWrappingModes.NoWrap;
                text.alignment = TextAlignmentOptions.BottomRight;
                nextBindings = 0;
            }
            text.gameObject.SetActive(true);
            // Native CheckHint can replace its TMP child without replacing the parent panel.
            if (native == null)
                native = panel.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(t => t.GetComponentInParent<ControlSlot>() == null);
            if (native != null)
            {
                text.font = native.font;
                text.fontSharedMaterial = native.fontSharedMaterial;
                text.fontSize = native.fontSize;
                text.fontStyle = native.fontStyle;
                text.fontWeight = native.fontWeight;
                text.color = native.color;
            }
            if (Time.unscaledTime >= nextBindings || language != GameText.Stamp ||
                repeat != lastRepeat || container != lastContainer || hasEmpty != lastEmpty)
            {
                var actions = InputManager.Instance.inputMaster.Player;
                string message = repeat ? GameText.Get("hold_repeat", Binding(actions.Drop), GameText.Native("drop")) + "\n" +
                    GameText.Get("hold_repeat", Binding(actions.Throw), GameText.Native("throw")) : "";
                if (container)
                {
                    string emptyHint = GameText.Get("drop_empty", "G");
                    if (!hasEmpty) emptyHint = "<color=#888888>" + emptyHint + "</color>";
                    message += (message.Length == 0 ? "" : "\n") + emptyHint;
                }
                lastRepeat = repeat; lastContainer = container; lastEmpty = hasEmpty;
                language = GameText.Stamp;
                if (text.text != message) text.text = message;
                nextBindings = Time.unscaledTime + .25f;
            }
            // A sibling above the existing bottom-right controls survives native CheckHint rebuilds.
            panel.GetWorldCorners(corners);
            Vector3 topRight = root.InverseTransformPoint(corners[2]);
            Vector2 size = text.GetPreferredValues(text.text) + new Vector2(8, 4);
            var rect = text.rectTransform;
            rect.anchorMin = rect.anchorMax = root.pivot;
            rect.pivot = new Vector2(1, 0);
            rect.sizeDelta = size;
            rect.anchoredPosition = new Vector2(Mathf.Min(topRight.x, root.rect.xMax - 4),
                Mathf.Min(topRight.y + 6, root.rect.yMax - size.y - 4));
        }
        private static string Binding(InputAction action)
        {
            bool gamepad = InputManager.Instance.isUsingGamepad;
            for (int i = 0; i < action.bindings.Count; i++)
            {
                string groups = action.bindings[i].groups ?? "";
                if (gamepad ? groups.Contains("Gamepad") : groups.Contains("Keyboard") || groups.Contains("Mouse"))
                    return action.GetBindingDisplayString(i);
            }
            return action.GetBindingDisplayString();
        }
        internal void Destroy()
        {
            if (text != null) { text.gameObject.SetActive(false); Object.Destroy(text.gameObject); }
            text = null; parent = null; native = null; language = null;
        }
    }
}
