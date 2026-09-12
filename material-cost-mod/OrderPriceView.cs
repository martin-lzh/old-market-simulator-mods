using OldMarket.Localization;
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace OldMarket.MaterialCost
{
    public sealed class OrderPriceTarget : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        internal OrderPriceView View;
        internal ProductSO Product;
        public void OnPointerEnter(PointerEventData data) { if (View != null) View.Select(Product); }
        public void OnSelect(BaseEventData data) { if (View != null) View.Select(Product); }
    }

    public sealed class OrderPriceView : MonoBehaviour
    {
        private UIManager ui;
        private GameManager game;
        private ProductSO selected;
        private RectTransform scroll;
        private Vector2 originalOffset;
        private TextMeshProUGUI text;
        private bool subscribed;
        private string language;

        internal void Bind(UIManager manager, ProductSO product)
        {
            if (text == null)
            {
                ui = manager;
                var list = ui.listDockOrders.GetComponentInParent<ScrollRect>(true);
                if (list == null) throw new InvalidOperationException("Order ScrollRect not found.");
                scroll = (RectTransform)list.transform;
                originalOffset = scroll.offsetMin;
                var footer = new GameObject("OrderPriceDetails", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
                text = footer.GetComponent<TextMeshProUGUI>();
                footer.transform.SetParent(scroll.parent, false);
                footer.GetComponent<LayoutElement>().ignoreLayout = true;
                // Place the footer within the original viewport's bottom edge, preserving the checkout controls.
                var rect = (RectTransform)footer.transform;
                rect.anchorMin = scroll.anchorMin;
                rect.anchorMax = new Vector2(scroll.anchorMax.x, scroll.anchorMin.y);
                rect.pivot = new Vector2(.5f, 0);
                rect.offsetMin = originalOffset + new Vector2(8, 4);
                rect.offsetMax = new Vector2(scroll.offsetMax.x - 8, originalOffset.y + 112);
                scroll.offsetMin = originalOffset + new Vector2(0, 120);
                text.fontSize = 20;
                text.enableAutoSizing = true;
                text.fontSizeMin = 12;
                text.fontSizeMax = 20;
                text.textWrappingMode = TextWrappingModes.Normal;
                text.alignment = TextAlignmentOptions.TopLeft;
                text.raycastTarget = false;
                game = GameManager.Instance;
                game.OnDayChangedGlobal += Refresh;
                LocalizationSettings.SelectedLocaleChanged += LocaleChanged;
                subscribed = true;
            }
            // SetData is invoked for every row and after quantity changes; retain the user's selection.
            if (selected == null) selected = product;
            if (selected == product) Refresh();
        }

        internal void Select(ProductSO product)
        {
            selected = product;
            Refresh();
        }

        private void LateUpdate()
        {
            if (text == null || !text.gameObject.activeInHierarchy || ui == null) return;
            if (language != GameText.Stamp) Refresh();
            if (text == null || ui == null) return;
            text.font = ui.textDockOrderTotal.font;
            text.fontSharedMaterial = ui.textDockOrderTotal.fontSharedMaterial;
        }
        private void OnEnable() => Refresh();
        private void LocaleChanged(UnityEngine.Localization.Locale locale) => Refresh();
        private void Refresh()
        {
            if (text == null || selected == null || game == null || !isActiveAndEnabled) return;
            try
            {
                string suggested = GameText.Native("recommended");
                string profit = GameText.Native("profit");
                language = GameText.Stamp;
                string Money(decimal value) => value.ToString("0.##", CultureInfo.CurrentCulture);
                int wholesale = game.GetWholesalePrice(selected);
                int selling = game.GetRecommendedPrice(selected);
                if (selected.amount <= 0 || wholesale < 0 || selling < 0)
                {
                    text.text = selected.GetLocalizedName() + "\n—";
                    return;
                }
                var estimate = CostMath.EstimateProfit(new RecipeQuote((decimal)wholesale * selected.amount, selected.amount), selling);
                string rate = estimate.ProfitPercent.HasValue ? Money(estimate.ProfitPercent.Value) + "%" : "—";
                text.font = ui.textDockOrderTotal.font;
                text.color = ui.textDockOrderTotal.color;
                string pack = GameText.Get("pack"), unit = GameText.Get("unit");
                string content = $"{selected.GetLocalizedName()} ({selected.amount}/{pack}) | {suggested}: {Money(selling)} C/{unit}\n" +
                    $"{GameText.Get("purchase_cost")}: {Money(estimate.Costs.BatchCost)} C/{pack} | {profit}: {Money(estimate.BatchProfit)} C/{pack}\n" +
                    $"{GameText.Get("return_cost")}: {rate}\n" + GameText.Get("purchase_basis");
                if (text.text != content) text.text = content;
            }
            catch (Exception error) { Plugin.Trace("Order price display failed: " + error); Restore(); }
        }

        public void Restore()
        {
            if (subscribed)
            {
                if (game != null) game.OnDayChangedGlobal -= Refresh;
                LocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
            }
            subscribed = false;
            if (scroll != null) scroll.offsetMin = originalOffset;
            if (text != null) { text.gameObject.SetActive(false); Destroy(text.gameObject); }
            text = null; scroll = null; ui = null; game = null; selected = null;
        }
        private void OnDestroy() => Restore();
    }
}
