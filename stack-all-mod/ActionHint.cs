using System.Collections.Generic;
using System.Linq;
using OldMarket.StackAll.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OldMarket.StackAll
{
    internal sealed class ActionHint
    {
        private GameObject row;
        private Transform parent;
        private TextMeshProUGUI caption;
        private ControlSlot key;
        private readonly Dictionary<Graphic, Color> colors = new Dictionary<Graphic, Color>();

        internal void Show(PlayerInventory inventory, bool hasEmpty)
        {
            var panel = inventory.controlHintPanel;
            if (panel == null || !panel.activeInHierarchy || !Application.isFocused || Time.timeScale <= 0 ||
                InputManager.Instance == null || UIManager.Instance == null || UIManager.Instance.IsAnyPanelActive() ||
                !EmptyContainerAction.IsReusable(inventory))
            { if (row != null) row.SetActive(false); return; }

            // Copy an actual native one-key row, preserving its layout, text and keycap styling.
            // Native CheckHint destroys/rebuilds its children; a destroyed custom row is recreated here.
            var source = panel.GetComponentsInChildren<ControlSlot>(false).FirstOrDefault(control =>
                control.textBinding != null && control.transform.parent != null && control.transform.parent.parent == panel.transform &&
                control.transform.parent.gameObject != row &&
                control.transform.parent.GetComponentsInChildren<ControlSlot>().Length == 1);
            if (source == null) { if (row != null) row.SetActive(false); return; }
            var sourceCaption = source.transform.parent.GetComponentsInChildren<TextMeshProUGUI>()
                .FirstOrDefault(text => text.GetComponentInParent<ControlSlot>() == null);
            if (sourceCaption == null) return;
            if (row == null || parent != panel.transform)
            {
                Destroy();
                parent = panel.transform;
                row = Object.Instantiate(source.transform.parent.gameObject, parent, false);
                row.name = "StackAllEmptyContainerHint";
                caption = row.GetComponentsInChildren<TextMeshProUGUI>(true)
                    .FirstOrDefault(text => text.GetComponentInParent<ControlSlot>() == null);
                key = row.GetComponentInChildren<ControlSlot>(true);
                if (caption == null || key == null || key.textBinding == null || key.imageBinding == null || key.imageBackground == null)
                { Destroy(); return; }
                foreach (var graphic in row.GetComponentsInChildren<Graphic>(true))
                { colors[graphic] = graphic.color; graphic.raycastTarget = false; }
            }
            row.SetActive(true);
            // Q/F remain the native rows. G joins the same layout group, without a separate text overlay.
            caption.text = GameText.Get("drop_empty");
            caption.font = sourceCaption.font;
            caption.fontSharedMaterial = sourceCaption.fontSharedMaterial;
            key.textBinding.font = source.textBinding.font;
            key.textBinding.fontSharedMaterial = source.textBinding.fontSharedMaterial;
            key.textBinding.text = "G";
            key.textBinding.gameObject.SetActive(true);
            key.imageBinding.gameObject.SetActive(false);
            key.imageBackground.enabled = true;
            foreach (var entry in colors)
            {
                if (entry.Key == null) continue;
                var original = entry.Value;
                entry.Key.color = hasEmpty ? original : new Color(original.r * .6f, original.g * .6f, original.b * .6f, original.a * .55f);
            }
        }

        internal void Destroy()
        {
            if (row != null) { row.SetActive(false); Object.Destroy(row); }
            row = null; parent = null; caption = null; key = null; colors.Clear();
        }
    }
}
