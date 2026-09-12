using OldMarket.MaterialCost.Localization;
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace OldMarket.MaterialCost
{
    public sealed class RecipeCostView : MonoBehaviour
    {
        private GameObject footer;
        private RectTransform scroll;
        private Vector2 originalOffsetMin;
        private TextMeshProUGUI costs, note;
        private UIManager boundUi;
        private RecipeSO boundRecipe;
        private GameManager boundGame;
        private bool localeSubscribed;
        private RecipeQuote displayedQuote;
        private RecipeProfit displayedProfit;
        private string displayedLocale, labelsLocale;
        private string amount, averageCost, recommended, profitLabel;

        public void Bind(UIManager ui, RecipeSO recipe)
        {
            // Calculate before touching UI so invalid data cannot display the preceding recipe's quote.
            var game = GameManager.Instance;
            var quote = RuntimeRules.QuoteRecipe(game, recipe);
            var profit = RuntimeRules.QuoteRecipeProfit(game, recipe);
            string locale = GameText.Stamp;
            // Native selection + click often invoke this twice. Avoid text allocation,
            // localization lookup and TMP/layout dirties when the displayed data is unchanged.
            if (footer != null && boundUi == ui && boundRecipe == recipe && boundGame == game &&
                ReferenceEquals(displayedQuote, quote) && ReferenceEquals(displayedProfit, profit) &&
                displayedLocale == locale && labelsLocale == locale &&
                costs.font == ui.textRecipeOutput.font && costs.color == ui.textRecipeOutput.color)
                return;
            if (footer == null) Create(ui);
            if (labelsLocale != locale)
            {
                amount = GameText.Native("amount"); averageCost = GameText.Native("avg_cost");
                recommended = GameText.Native("recommended"); profitLabel = GameText.Native("profit");
                labelsLocale = locale;
            }
            costs.font = note.font = ui.textRecipeOutput.font;
            costs.color = note.color = ui.textRecipeOutput.color;
            string Money(decimal value) => value.ToString("0.##", CultureInfo.CurrentCulture);
            string batch = Money(quote.BatchCost), unit = Money(quote.UnitCost);
            string price = profit == null ? "—" : Money(profit.UnitSellingPrice);
            string batchProfit = profit == null ? "—" : Money(profit.BatchProfit);
            string unitProfit = profit == null ? "—" : Money(profit.UnitProfit);
            string rate = profit?.ProfitPercent is decimal percent ? Money(percent) + "%" : "—";
            string unitLabel = GameText.Get("unit"), batchLabel = GameText.Get("batch");
            var costText = $"{amount}: {quote.OutputUnits} | {GameText.Get("batch_cost")}: {batch} C\n" +
                $"{averageCost}: {unit} C | {recommended}: {price} C / {unitLabel}\n" +
                $"{profitLabel}: {batchProfit} C / {batchLabel} ({unitProfit} C / {unitLabel})\n" +
                $"{GameText.Get("return_cost")}: {rate}";
            var noteText = GameText.Get(profit == null ? "no_price" : "sale_assumption") + "\n" + GameText.Get("material_basis");
            if (costs.text != costText) costs.text = costText;
            if (note.text != noteText) note.text = noteText;
            displayedQuote = quote; displayedProfit = profit; displayedLocale = locale;
            boundUi = ui;
            boundRecipe = recipe;
            if (boundGame != game)
            {
                if (boundGame != null) boundGame.OnDayChangedGlobal -= Refresh;
                boundGame = game;
                boundGame.OnDayChangedGlobal += Refresh;
            }
            if (!localeSubscribed)
            {
                LocalizationSettings.SelectedLocaleChanged += LocaleChanged;
                localeSubscribed = true;
            }
        }

        private void LateUpdate()
        {
            if (footer == null || !footer.activeInHierarchy || boundUi == null) return;
            if (displayedLocale != GameText.Stamp) Refresh();
            if (footer == null || boundUi == null) return;
            costs.font = note.font = boundUi.textRecipeOutput.font;
            costs.fontSharedMaterial = note.fontSharedMaterial = boundUi.textRecipeOutput.fontSharedMaterial;
        }
        private void LocaleChanged(UnityEngine.Localization.Locale locale) => Refresh();
        private void Refresh()
        {
            if (!isActiveAndEnabled || boundUi == null || boundRecipe == null) return;
            try { Bind(boundUi, boundRecipe); }
            catch (Exception error) { Plugin.Trace($"Recipe estimate refresh failed: {error}"); Restore(); }
        }

        private void Create(UIManager ui)
        {
            var view = ui.listRecipeIngredients.GetComponentInParent<ScrollRect>(true);
            if (view == null) throw new InvalidOperationException("Recipe ingredients ScrollRect not found.");
            scroll = (RectTransform)view.transform;
            originalOffsetMin = scroll.offsetMin;
            scroll.offsetMin = originalOffsetMin + new Vector2(0, 158);
            footer = new GameObject("RecipeMaterialCost", typeof(RectTransform), typeof(LayoutElement), typeof(VerticalLayoutGroup));
            footer.transform.SetParent(scroll.parent, false);
            footer.GetComponent<LayoutElement>().ignoreLayout = true;
            var rect = (RectTransform)footer.transform;
            rect.anchorMin = Vector2.zero; rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(.5f, 0); rect.sizeDelta = new Vector2(-16, 152);
            rect.anchoredPosition = new Vector2(0, 4);
            var layout = footer.GetComponent<VerticalLayoutGroup>();
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 4;
            costs = MakeText("Costs", 20, 104);
            note = MakeText("Basis", 16, 44);
        }

        private TextMeshProUGUI MakeText(string name, float size, float height)
        {
            var text = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(footer.transform, false);
            var element = text.gameObject.AddComponent<LayoutElement>();
            element.minHeight = element.preferredHeight = height;
            text.fontSize = size; text.fontSizeMin = 12; text.fontSizeMax = size;
            text.enableAutoSizing = true;
            text.alignment = TextAlignmentOptions.TopLeft;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.raycastTarget = false;
            return text;
        }

        public void Restore()
        {
            if (boundGame != null) boundGame.OnDayChangedGlobal -= Refresh;
            if (localeSubscribed) LocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
            boundGame = null; boundUi = null; boundRecipe = null; localeSubscribed = false;
            displayedQuote = null; displayedProfit = null; displayedLocale = labelsLocale = null;
            if (scroll != null) scroll.offsetMin = originalOffsetMin;
            if (footer != null) { footer.SetActive(false); Destroy(footer); }
            footer = null;
        }
        private void OnDestroy() => Restore();
    }
}
