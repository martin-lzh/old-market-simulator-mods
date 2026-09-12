using OldMarket.Localization;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OldMarket.PriceProbability
{
    public sealed class ProbabilityView : MonoBehaviour
    {
        private PricePanel panel;
        private ProductSO product;
        private RectTransform root;
        private TextMeshProUGUI summary, modeText, explanation, endpoints;
        private string language;
        private string displayedInput;
        private double displayedTarget = double.NaN;
        private int displayedMode = -1;
        private bool displayedHost;
        private TextMeshProUGUI[] labels;
        private Slider slider;
        private Button modeButton;
        private int mode;
        private double target = 1;
        private bool changing;
        private int wholesale = -1, recommended = -1;
        private float multiplier = -1;
        private UnityAction<string> listener;
        private readonly Vector3[] corners = new Vector3[4];
        internal void Prepare() { changing = true; }

        internal void Bind(PricePanel owner, ProductSO selected)
        {
            panel = owner;
            product = selected;
            if (root == null) Build();
            var rule = Plugin.Instance.GetRule(product.id);
            mode = rule == null ? 0 : rule.Probability ? 2 : 1;
            target = rule != null && rule.Probability ? rule.Target : Probability();
            changing = false;
            wholesale = -1;
            displayedInput = null;
            Refresh();
        }

        private RectTransform Rect(string name, Transform parent, float x, float y, float width, float height)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, 1);
            rect.pivot = new Vector2(.5f, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
            return rect;
        }
        private TextMeshProUGUI Label(string name, RectTransform rect, string content, float size)
        {
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = panel.textRecommendedPrice.font;
            text.fontSharedMaterial = panel.textRecommendedPrice.fontSharedMaterial;
            text.fontSize = size;
            text.enableAutoSizing = true; text.fontSizeMin = 11; text.fontSizeMax = size;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            text.text = content;
            return text;
        }

        private void Build()
        {
            root = Rect("PriceProbability", transform, 0, 0, 360, 275);
            root.anchorMin = root.anchorMax = new Vector2(1, .5f);
            root.pivot = new Vector2(0, .5f);
            root.anchoredPosition = new Vector2(12, 0);
            root.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            root.gameObject.AddComponent<Image>().color = new Color(.09f, .12f, .15f, .98f);
            summary = Label("Probability", Rect("Probability", root, 0, 12, 340, 60), "", 19);
            endpoints = Label("Endpoints", Rect("Endpoints", root, 0, 75, 320, 24), "100%                                  0%", 17);
            var track = Rect("ProbabilitySlider", root, 0, 106, 310, 28);
            var background = track.gameObject.AddComponent<Image>();
            background.color = new Color(.27f, .32f, .37f);
            slider = track.gameObject.AddComponent<Slider>();
            var handle = Rect("Handle", track, 0, 0, 18, 28);
            var handleImage = handle.gameObject.AddComponent<Image>();
            handleImage.color = new Color(.4f, .85f, .65f);
            slider.handleRect = handle;
            slider.targetGraphic = handleImage;
            slider.minValue = 0; slider.maxValue = 100;
            slider.wholeNumbers = true;
            slider.direction = Slider.Direction.RightToLeft;
            slider.onValueChanged.AddListener(Slide);
            var buttonRect = Rect("AnchorMode", root, 0, 148, 330, 35);
            buttonRect.gameObject.AddComponent<Image>().color = new Color(.2f, .28f, .35f);
            modeButton = buttonRect.gameObject.AddComponent<Button>();
            modeText = Label("Mode", Rect("Text", buttonRect, 0, 0, 330, 35), "", 18);
            modeButton.onClick.AddListener(() =>
            {
                mode = (mode + 1) % 3;
                if (mode == 2) { target = Probability(); SetPriceFromTarget(); }
                Refresh();
            });
            explanation = Label("Explanation", Rect("Explanation", root, 0, 187, 340, 76), "", 14);
            labels = new[] { summary, modeText, explanation, endpoints };
            listener = _ =>
            {
                if (changing) return;
                target = Probability();
                Refresh();
            };
            panel.inputFieldPrice.onValueChanged.AddListener(listener);
        }

        private double Probability()
        {
            var game = GameManager.Instance;
            return int.TryParse(panel.inputFieldPrice.text, out int price) && price > 0
                ? PriceMath.Probability(price, game.GetWholesalePrice(product), product.recommendedProfitPercentage, game.maxProfitMultiplier)
                : 0;
        }

        private void Slide(float value)
        {
            if (changing) return;
            target = value / 100d;
            SetPriceFromTarget();
            Refresh();
        }
        private void SetPriceFromTarget()
        {
            var game = GameManager.Instance;
            int price = PriceMath.Price(target, game.GetWholesalePrice(product), product.recommendedProfitPercentage,
                game.maxProfitMultiplier, game.GetRecommendedPrice(product));
            changing = true;
            panel.inputFieldPrice.text = price.ToString();
            changing = false;
        }
        private void Refresh()
        {
            if (product == null || root == null || GameManager.Instance == null) return;
            var game = GameManager.Instance;
            int currentWholesale = game.GetWholesalePrice(product);
            int currentRecommended = game.GetRecommendedPrice(product);
            bool pricesChanged = wholesale != currentWholesale || recommended != currentRecommended || multiplier != game.maxProfitMultiplier;
            if (pricesChanged)
            {
                wholesale = currentWholesale; recommended = currentRecommended; multiplier = game.maxProfitMultiplier;
                if (mode == 2) SetPriceFromTarget();
                // Retain the game's localized labels.
                ReplaceAmount(panel.textRecommendedPrice, "recommended", recommended);
                ReplaceAmount(panel.textAdjustedPrice, "wholesale", wholesale);
            }
            string stamp = GameText.Stamp;
            bool host = Plugin.Host;
            if (!pricesChanged && displayedInput == panel.inputFieldPrice.text && displayedTarget == target &&
                displayedMode == mode && displayedHost == host && language == stamp) return;
            displayedInput = panel.inputFieldPrice.text; displayedTarget = target;
            displayedMode = mode; displayedHost = host;
            bool valid = int.TryParse(panel.inputFieldPrice.text, out int price) && price > 0;
            double actual = valid ? Probability() : 0;
            changing = true;
            slider.SetValueWithoutNotify((float)((mode == 2 ? target : actual) * 100));
            changing = false;
            string content = valid ? GameText.Get("price_summary", (actual * 100).ToString("0.00"),
                (target * 100).ToString("0.##"), recommended) : GameText.Get("invalid_price");
            if (summary.text != content) summary.text = content;
            modeButton.interactable = Plugin.Host;
            content = GameText.Get(!Plugin.Host ? "host_only" : mode == 0 ? "anchor_off" : mode == 1 ? "anchor_price" : "anchor_probability");
            if (modeText.text != content) modeText.text = content;
            content = GameText.Get("price_explanation");
            if (explanation.text != content) explanation.text = content;
            if (language != GameText.Stamp)
            {
                language = GameText.Stamp;
                panel.textTitle.text = product.GetLocalizedName();
                ReplaceAmount(panel.textRecommendedPrice, "recommended", recommended);
                ReplaceAmount(panel.textAdjustedPrice, "wholesale", wholesale);
            }
        }
        private static void ReplaceAmount(TextMeshProUGUI text, string key, int value)
        {
            string content = GameText.Native(key) + ": " + value + " C";
            if (text.text != content) text.text = content;
        }

        private void Update() { if (panel != null && product != null) Refresh(); }
        private void LateUpdate()
        {
            if (root == null || panel == null) return;
            foreach (var label in labels)
            {
                label.font = panel.textRecommendedPrice.font;
                label.fontSharedMaterial = panel.textRecommendedPrice.fontSharedMaterial;
            }
            // Clamp the extension into the actual canvas, including small windows / UI scaling.
            var canvas = GetComponentInParent<Canvas>()?.rootCanvas;
            if (canvas == null) return;
            var bounds = (RectTransform)canvas.transform;

            root.GetWorldCorners(corners);
            var a = bounds.InverseTransformPoint(corners[0]);
            var b = bounds.InverseTransformPoint(corners[2]);
            var r = bounds.rect;
            float dx = a.x < r.xMin + 8 ? r.xMin + 8 - a.x : b.x > r.xMax - 8 ? r.xMax - 8 - b.x : 0;
            float dy = a.y < r.yMin + 8 ? r.yMin + 8 - a.y : b.y > r.yMax - 8 ? r.yMax - 8 - b.y : 0;
            root.position += bounds.TransformVector(new Vector3(dx, dy, 0));
        }

        internal void Commit()
        {
            if (int.TryParse(panel.inputFieldPrice.text, out int price) && price > 0)
                Plugin.Instance.Commit(product, price, mode, target);
        }
        private void OnDestroy()
        {
            if (panel != null && listener != null) panel.inputFieldPrice.onValueChanged.RemoveListener(listener);
            if (root != null) Destroy(root.gameObject);
        }
    }
}
