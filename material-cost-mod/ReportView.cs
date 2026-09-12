using OldMarket.MaterialCost.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace OldMarket.MaterialCost
{
    public sealed class ReportView : MonoBehaviour
    {
        private sealed class Row
        {
            public SaleTile Tile;
            public string Cost, Profit;
            public bool HasRule;
            public Summary Material;
            public IGrouping<long, Sale> Sales;
        }
        private readonly List<Row> rows = new List<Row>();
        private GameObject footer;
        private RectTransform scroll, knob;
        private Vector2 originalOffsetMin;
        private Toggle toggle;
        private Image track;
        private TextMeshProUGUI label, detail;
        private UIManager boundUi;
        private string language;
        private int fallbackCount;
        private bool materialsReady;
        private GameManager boundGame;

        public void Bind(UIManager ui, Sale[] sales)
        {
            boundUi = ui;
            boundGame = GameManager.Instance;
            var groups = sales.GroupBy(x => x.ItemId).ToArray();
            // Original code schedules old rows for Destroy at frame end, then appends new ones.
            int first = ui.listSales.childCount - groups.Length;
            if (first < 0) throw new InvalidOperationException("Daily sales layout has changed.");
            rows.Clear();
            fallbackCount = 0;
            materialsReady = false;
            for (int i = 0; i < groups.Length; i++)
            {
                var tile = ui.listSales.GetChild(first + i).GetComponent<SaleTile>();
                if (tile == null) throw new InvalidOperationException("Expected native SaleTile.");
                rows.Add(new Row { Tile = tile, Cost = tile.textAvgCost.text,
                    Profit = tile.textProfit.text, Sales = groups[i] });
            }
            if (footer == null) CreateControls(ui);
            label.font = detail.font = ui.textTotal.font;
            label.text = GameText.Get("materials_only");
            toggle.SetIsOnWithoutNotify(Plugin.Instance.MaterialsOnly);
            Apply(toggle.isOn);
        }

        private void CreateControls(UIManager ui)
        {
            var scrollView = ui.listSales.GetComponentInParent<ScrollRect>(true);
            if (scrollView == null) throw new InvalidOperationException("Daily sales ScrollRect not found.");
            scroll = (RectTransform)scrollView.transform;
            originalOffsetMin = scroll.offsetMin;
            // Reserve space inside DailySales; do not cover its headings or the next-day button.
            scroll.offsetMin = originalOffsetMin + new Vector2(0, 82);
            footer = new GameObject("MaterialCostReport", typeof(RectTransform), typeof(LayoutElement));
            footer.transform.SetParent(scroll.parent, false);
            footer.GetComponent<LayoutElement>().ignoreLayout = true;
            var rect = (RectTransform)footer.transform;
            rect.anchorMin = new Vector2(0, 0); rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(.5f, 0); rect.sizeDelta = new Vector2(-16, 78);
            rect.anchoredPosition = new Vector2(0, 2);

            var switchObject = new GameObject("MaterialsSwitch", typeof(RectTransform), typeof(Image), typeof(Toggle));
            switchObject.transform.SetParent(footer.transform, false);
            Place((RectTransform)switchObject.transform, 0, 1, 0, -3, 265, 34);
            switchObject.GetComponent<Image>().color = Color.clear;
            toggle = switchObject.GetComponent<Toggle>();

            var rail = new GameObject("Track", typeof(RectTransform), typeof(Image));
            rail.transform.SetParent(switchObject.transform, false);
            Place((RectTransform)rail.transform, 0, .5f, 2, 0, 52, 26, new Vector2(0, .5f));
            track = rail.GetComponent<Image>();
            var nativeImage = ui.buttonNextDay.GetComponent<Image>();
            if (nativeImage != null) { track.sprite = nativeImage.sprite; track.type = Image.Type.Sliced; }
            track.raycastTarget = false;
            var thumb = new GameObject("Thumb", typeof(RectTransform), typeof(Image));
            thumb.transform.SetParent(rail.transform, false);
            knob = (RectTransform)thumb.transform;
            Place(knob, 0, .5f, 3, 0, 20, 20, new Vector2(0, .5f));
            thumb.GetComponent<Image>().color = new Color(.97f, .94f, .85f);
            thumb.GetComponent<Image>().raycastTarget = false;
            toggle.targetGraphic = track;
            toggle.transition = Selectable.Transition.ColorTint;
            toggle.colors = new ColorBlock { normalColor = Color.white, highlightedColor = new Color(1.15f, 1.15f, 1.15f),
                pressedColor = new Color(.7f, .7f, .7f), selectedColor = Color.white,
                disabledColor = Color.gray, colorMultiplier = 1, fadeDuration = .1f };
            // The thumb remains visible for both states; its position is the indicator.
            toggle.graphic = null;
            label = MakeText(switchObject.transform, ui.textTotal, 23);
            Place(label.rectTransform, 0, .5f, 66, 0, 195, 34, new Vector2(0, .5f));
            detail = MakeText(footer.transform, ui.textTotal, 16);
            detail.rectTransform.anchorMin = new Vector2(0, 0);
            detail.rectTransform.anchorMax = new Vector2(1, 0);
            detail.rectTransform.pivot = new Vector2(.5f, 0);
            detail.rectTransform.sizeDelta = new Vector2(-4, 38);
            detail.rectTransform.anchoredPosition = new Vector2(0, 0);
            toggle.onValueChanged.AddListener(value =>
            {
                try
                {
                    Apply(value);
                    Plugin.Instance.MaterialsOnly = value;
                }
                catch (Exception error)
                {
                    Plugin.Trace($"Material report failed: {error}");
                    toggle.SetIsOnWithoutNotify(false);
                    Apply(false);
                }
            });
        }

        private static void Place(RectTransform rect, float ax, float ay, float x, float y, float w, float h, Vector2? pivot = null)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(ax, ay);
            rect.pivot = pivot ?? new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, y); rect.sizeDelta = new Vector2(w, h);
        }

        private static TextMeshProUGUI MakeText(Transform parent, TextMeshProUGUI source, float size)
        {
            var text = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(parent, false);
            text.font = source.font; text.fontSize = size; text.color = source.color;
            text.enableAutoSizing = true; text.fontSizeMin = 12; text.fontSizeMax = size;
            text.alignment = TextAlignmentOptions.MidlineLeft; text.raycastTarget = false;
            text.textWrappingMode = TextWrappingModes.Normal;
            return text;
        }

        private static string Number(decimal value) => value.ToString("0.##", CultureInfo.CurrentCulture);
        private void Apply(bool materials)
        {
            if (materials && !materialsReady)
            {
                var rules = RuntimeRules.BuildReport(boundGame, rows.Select(row => row.Sales.Key));
                fallbackCount = 0;
                foreach (var row in rows)
                {
                    row.HasRule = rules.TryGet(row.Sales.Key, out var unitCost);
                    if (row.HasRule) row.Material = CostMath.Summarize(row.Sales.Select(s => (unitCost, s.SellingPrice)));
                    else fallbackCount++;
                }
                materialsReady = true;
            }
            foreach (var row in rows)
            {
                if (row.Tile == null) continue;
                bool estimate = materials && row.HasRule;
                row.Tile.textAvgCost.text = estimate ? Number(row.Material.AverageCost) + "C" : row.Cost;
                row.Tile.textProfit.text = estimate
                    ? Number(row.Material.Profit) + "C (" + (row.Material.ProfitPercent.HasValue ? Number(row.Material.ProfitPercent.Value) + "%" : "—") + ")"
                    : row.Profit;
            }
            if (footer == null) return;
            knob.anchoredPosition = new Vector2(materials ? 29 : 3, 0);
            track.color = materials ? new Color(.32f, .49f, .29f) : new Color(.38f, .35f, .29f);
            RefreshText(materials);
        }

        private void RefreshText(bool materials)
        {
            label.text = GameText.Get("materials_only");
            detail.text = GameText.Get(materials ? "report_estimate" : "report_original");
            if (materials && fallbackCount > 0) detail.text += "\n" + GameText.Get("report_fallback", fallbackCount);
            language = GameText.Stamp;
        }
        private void LateUpdate()
        {
            if (footer == null || !footer.activeInHierarchy || boundUi == null) return;
            label.font = detail.font = boundUi.textTotal.font;
            label.fontSharedMaterial = detail.fontSharedMaterial = boundUi.textTotal.fontSharedMaterial;
            if (language != GameText.Stamp) RefreshText(toggle.isOn);
        }

        public void Restore()
        {
            Apply(false);
            if (scroll != null) scroll.offsetMin = originalOffsetMin;
            if (footer != null) Destroy(footer);
            footer = null;
        }
        private void OnDestroy() => Restore();
    }
}
