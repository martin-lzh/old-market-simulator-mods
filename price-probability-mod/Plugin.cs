using System;
using System.Collections.Generic;
using System.Globalization;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace OldMarket.PriceProbability
{
    [BepInPlugin(Id, "Smart Pricing", "0.1.2")]
    [BepInProcess("Old Market Simulator.exe")]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Id = "local.oldmarket.priceprobability";
        internal static Plugin Instance;
        private Harmony harmony;
        private GameManager session;
        private float nextCheck;
        private bool applying;
        private readonly Dictionary<long, Rule> rules = new Dictionary<long, Rule>();
        internal sealed class Rule
        {
            public bool Probability;
            public double Target;
            public int Price;
        }

        private void Awake()
        {
            Instance = this;
            harmony = new Harmony(Id);
            try
            {
                Patch(typeof(GameManager), nameof(GameManager.LoadWorldServer), prefix: nameof(BeforeLoad), postfix: nameof(Loaded));
                Patch(typeof(GameManager), nameof(GameManager.GetSellingPriceServer), prefix: nameof(BeforeRead));
                Patch(typeof(GameManager), nameof(GameManager.UpdatePriceServerRpc), prefix: nameof(NormalizeRequest));
                Patch(typeof(PricePanel), nameof(PricePanel.SetData), prefix: nameof(Preparing), postfix: nameof(Show));
                Patch(typeof(PricePanel), nameof(PricePanel.SubmitPrice), prefix: nameof(Submit));
                Patch(typeof(SaveManager), nameof(SaveManager.DeleteSlot), postfix: nameof(DeletedSlot));
                Patch(typeof(SaveManager), nameof(SaveManager.DeleteSave), postfix: nameof(DeletedSave));
            }
            catch (Exception error) { harmony.UnpatchSelf(); Logger.LogError(error); enabled = false; return; }
            Logger.LogInfo("Smart Pricing ready; automatic anchors are host-owned.");
        }

        private void Patch(Type type, string method, string prefix = null, string postfix = null)
        {
            var original = AccessTools.Method(type, method) ?? throw new MissingMethodException(type.Name, method);
            harmony.Patch(original,
                prefix: prefix == null ? null : new HarmonyMethod(typeof(Plugin), prefix),
                postfix: postfix == null ? null : new HarmonyMethod(typeof(Plugin), postfix));
        }

        internal Rule GetRule(long id) => rules.TryGetValue(id, out var value) ? value : null;
        internal static bool Host => GameManager.Instance != null && GameManager.Instance.IsSpawned && GameManager.Instance.IsServer;
        private string Section(long id)
        {
            var save = SaveManager.Instance;
            string slot = save == null ? null : AccessTools.Field(typeof(SaveManager), "currentSlot").GetValue(save) as string;
            if (string.IsNullOrEmpty(slot)) throw new InvalidOperationException("Save slot unavailable.");
            return "Anchor_" + Uri.EscapeDataString(slot) + "_" + id.ToString(CultureInfo.InvariantCulture);
        }
        private ConfigEntry<string> Entry(long id) => Config.Bind(Section(id), "Rule", "", "price:C or probability:0..1; empty disables the anchor.");

        internal void Commit(ProductSO product, int price, int mode, double target)
        {
            if (!Host) return;
            var rule = mode == 0 ? null : new Rule { Probability = mode == 2, Target = target, Price = price };
            if (rule == null) rules.Remove(product.id); else rules[product.id] = rule;
            Entry(product.id).Value = rule == null ? "" : rule.Probability
                ? "probability:" + target.ToString("R", CultureInfo.InvariantCulture)
                : "price:" + price.ToString(CultureInfo.InvariantCulture);
        }

        private void Load(GameManager game)
        {
            rules.Clear();
            session = game;
            if (!Host) return;
            foreach (var item in game.itemDatabase)
            {
                if (!(item is ProductSO product)) continue;
                string value = Entry(product.id).Value;
                if (value.StartsWith("probability:", StringComparison.Ordinal) &&
                    double.TryParse(value.Substring(12), NumberStyles.Float, CultureInfo.InvariantCulture, out double target) &&
                    !double.IsNaN(target) && target >= 0 && target <= 1)
                    rules[product.id] = new Rule { Probability = true, Target = target };
                else if (value.StartsWith("price:", StringComparison.Ordinal) && int.TryParse(value.Substring(6), out int price) && price > 0)
                    rules[product.id] = new Rule { Price = price };
            }
            Apply();
        }

        private void Update()
        {
            if (session != GameManager.Instance) { rules.Clear(); session = GameManager.Instance; }
            if (Time.unscaledTime < nextCheck) return;
            nextCheck = Time.unscaledTime + .5f;
            try { Apply(); }
            catch (Exception error) { Logger.LogError(error); rules.Clear(); }
        }

        private void Apply()
        {
            if (!Host || session != GameManager.Instance || applying) return;
            applying = true;
            try
            {
                foreach (var pair in rules)
                {
                    var product = session.GetItemById(pair.Key) as ProductSO;
                    if (product == null) continue;
                    var rule = pair.Value;
                    int price = rule.Probability ? PriceMath.Price(rule.Target, session.GetWholesalePrice(product),
                        product.recommendedProfitPercentage, session.maxProfitMultiplier, session.GetRecommendedPrice(product)) : rule.Price;
                    if (session.GetSellingPriceServer(product) != price) session.UpdatePriceServerRpc(product.id, price);
                }
            }
            finally { applying = false; }
        }

        private static void BeforeLoad(GameManager __instance)
        {
            if (!__instance.IsServer) return;
            Instance.rules.Clear();
            Instance.session = null;
        }
        private static void Loaded(GameManager __instance)
        {
            if (__instance.IsServer) Instance.Load(__instance);
        }
        // Re-evaluate before the native customer reads a price, avoiding a stale-day purchase between polls.
        private static void BeforeRead(ProductSO productSO)
        {
            if (!Host || Instance.applying || Instance.session != GameManager.Instance || productSO == null) return;
            if (!Instance.rules.TryGetValue(productSO.id, out var rule)) return;
            Instance.applying = true;
            try
            {
                var game = GameManager.Instance;
                int price = rule.Probability ? PriceMath.Price(rule.Target, game.GetWholesalePrice(productSO),
                    productSO.recommendedProfitPercentage, game.maxProfitMultiplier, game.GetRecommendedPrice(productSO)) : rule.Price;
                if (game.GetSellingPriceServer(productSO) != price) game.UpdatePriceServerRpc(productSO.id, price);
            }
            catch (Exception error) { Instance.Logger.LogError(error); Instance.rules.Remove(productSO.id); }
            finally { Instance.applying = false; }
        }

        // Keep the generated RPC body, execution-stage reset and normal NetworkVariable synchronization intact.
        // Normalize its argument instead of cancelling the RPC or sending a corrective RPC from inside it.
        private static void NormalizeRequest(GameManager __instance, long productId, ref int price)
        {
            if (!__instance.IsServer || !__instance.IsSpawned || Instance.session != __instance || price <= 0) return;
            if (!Instance.rules.TryGetValue(productId, out var rule)) return;
            var product = __instance.GetItemById(productId) as ProductSO;
            if (product == null) return;
            int anchored = rule.Probability ? PriceMath.Price(rule.Target, __instance.GetWholesalePrice(product),
                product.recommendedProfitPercentage, __instance.maxProfitMultiplier, __instance.GetRecommendedPrice(product)) : rule.Price;
            price = NetworkPricePolicy.Resolve(__instance.IsServer, __instance.IsSpawned, price, anchored);
        }

        private static void Preparing(PricePanel __instance) { __instance.GetComponent<ProbabilityView>()?.Prepare(); }

        private static void Show(PricePanel __instance, ProductSO productSO)
        {
            var view = __instance.GetComponent<ProbabilityView>() ?? __instance.gameObject.AddComponent<ProbabilityView>();
            view.Bind(__instance, productSO);
        }
        private static void Submit(PricePanel __instance)
        {
            var view = __instance.GetComponent<ProbabilityView>();
            if (view != null) view.Commit();
        }

        private static void DeletedSlot(string slot) { Instance.ClearSlot(slot); }
        private static void DeletedSave(string ___currentSlot) { Instance.ClearSlot(___currentSlot); }
        private void ClearSlot(string slot)
        {
            string prefix = "Anchor_" + Uri.EscapeDataString(slot) + "_";
            var entries = new List<ConfigDefinition>(Config.Keys);
            foreach (var key in entries)
                if (key.Section.StartsWith(prefix, StringComparison.Ordinal)) Config.Remove(key);
            Config.Save();
            // A subsequent LoadWorldServer rebuilds the active policies.
            rules.Clear();
        }

        private void OnDestroy()
        {
            foreach (var view in Resources.FindObjectsOfTypeAll<ProbabilityView>()) Destroy(view);
            harmony?.UnpatchSelf();
            if (Instance == this) Instance = null;
        }
    }
}
