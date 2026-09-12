using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OldMarket.MaterialCost
{
    internal static class RuntimeRules
    {
        private static GameManager cachedGame;
        private static List<ItemSO> cachedDatabase;
        private static int databaseCount, priceDay;
        private static readonly Dictionary<long, ItemSO> items = new Dictionary<long, ItemSO>();
        private static readonly Dictionary<RecipeSO, RecipeQuote> quotes = new Dictionary<RecipeSO, RecipeQuote>();
        private static readonly Dictionary<RecipeSO, RecipeProfit> profits = new Dictionary<RecipeSO, RecipeProfit>();
        private static ReportSources reportSources;

        // The native map loader replaces itemDatabase. Never eagerly index it or preload recipes.
        private static void EnsureContext(GameManager game)
        {
            if (game == null || game.itemDatabase == null) throw new ArgumentException("Item database is unavailable.");
            if (cachedGame != game || !ReferenceEquals(cachedDatabase, game.itemDatabase) || databaseCount != game.itemDatabase.Count)
            {
                Clear();
                cachedGame = game;
                cachedDatabase = game.itemDatabase;
                databaseCount = cachedDatabase.Count;
                priceDay = game.GetCurrentDayRaw();
            }
            int day = game.GetCurrentDayRaw();
            if (day != priceDay)
            {
                quotes.Clear();
                profits.Clear();
                priceDay = day;
            }
        }

        internal static void Clear()
        {
            items.Clear(); quotes.Clear(); profits.Clear();
            cachedGame = null; cachedDatabase = null; reportSources = null;
        }

        private static ItemSO ResolveItem(GameManager game, ItemSO fallback)
        {
            if (!items.TryGetValue(fallback.id, out var active))
            {
                // The game's lookup stops at the first match. Cache only IDs actually requested.
                active = game.GetItemById(fallback.id);
                items.Add(fallback.id, active);
            }
            return active != null ? active : fallback;
        }

        internal static RecipeProfit QuoteRecipeProfit(GameManager game, RecipeSO recipe)
        {
            var costs = QuoteRecipe(game, recipe);
            if (profits.TryGetValue(recipe, out var cached)) return cached;
            var product = ResolveItem(game, recipe.output) as ProductSO ?? recipe.output as ProductSO;
            // Tools and other non-product outputs have no native recommended selling price.
            var profit = product == null ? null : CostMath.EstimateProfit(costs, game.GetRecommendedPrice(product));
            profits.Add(recipe, profit);
            return profit;
        }

        // Called only when the daily report's materials switch is on. Discovery is deferred
        // until then, cached for this map, and prices are computed only for sold outputs.
        public static CostRules BuildReport(GameManager game, IEnumerable<long> soldIds)
        {
            EnsureContext(game);
            var rules = new CostRules();
            var requested = new HashSet<long>(soldIds);
            if (requested.Count == 0) return rules;
            if (reportSources == null) reportSources = DiscoverReportSources(game);
            foreach (var source in reportSources.FixedCosts)
                if (requested.Contains(source.Id)) rules.Add(source.Id, source.Cost);
            foreach (var id in requested)
                if (reportSources.Recipes.TryGetValue(id, out var recipes))
                    foreach (var recipe in recipes) rules.Add(id, QuoteRecipe(game, recipe).UnitCost);
            return rules;
        }

        private sealed class ReportSources
        {
            public readonly List<(long Id, decimal Cost)> FixedCosts = new List<(long, decimal)>();
            public readonly Dictionary<long, List<RecipeSO>> Recipes = new Dictionary<long, List<RecipeSO>>();
        }

        // No global recipe registry exists in the game. Discover production metadata once,
        // preserving all alternative routes so ambiguous costs still fall back safely.
        private static ReportSources DiscoverReportSources(GameManager game)
        {
            var sources = new ReportSources();
            var seenIds = new HashSet<long>();
            var recipes = new HashSet<RecipeSO>();
            var fields = new Dictionary<Type, FieldInfo>();
            foreach (var item in game.itemDatabase)
            {
                if (item == null || !seenIds.Add(item.id)) continue;
                if (item is SeedSO seed && seed.productSO != null)
                    sources.FixedCosts.Add((seed.productSO.id, CostMath.Seed(seed.basePrice, seed.amount, seed.harvestAmount)));
                if (item is TreeSO tree && tree.productSO != null) sources.FixedCosts.Add((tree.productSO.id, 0));
                if (item is AnimalSO animal)
                {
                    if (animal.productSO != null) sources.FixedCosts.Add((animal.productSO.id, 0));
                    // The animal is consumed by slaughter, so this is a material, not equipment.
                    foreach (var meat in animal.meats.Where(x => x != null))
                        sources.FixedCosts.Add((meat.id, (decimal)animal.basePrice / animal.meats.Count / meat.amount));
                }
                if (item.prefab == null) continue;
                foreach (var component in item.prefab.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (component == null) continue;
                    if (component is BlockBeeHive hive && hive.output != null) sources.FixedCosts.Add((hive.output.id, 0));
                    if (component is BlockFishTrap trap)
                        foreach (var fish in trap.commonFishes.Where(x => x != null)) sources.FixedCosts.Add((fish.id, 0));
                    if (component is FishingRod rod)
                        foreach (var fish in rod.commonFishes.Where(x => x != null)) sources.FixedCosts.Add((fish.id, 0));
                    var type = component.GetType();
                    if (!fields.TryGetValue(type, out var field))
                        fields.Add(type, field = type.GetField("recipes", BindingFlags.Public | BindingFlags.Instance));
                    if (field?.GetValue(component) is List<RecipeSO> found)
                        foreach (var recipe in found.Where(x => x != null)) recipes.Add(recipe);
                }
            }
            foreach (var zone in UnityEngine.Object.FindObjectsOfType<FishingZone>(true))
                foreach (var fish in zone.fishes)
                    if (fish.itemSO != null) sources.FixedCosts.Add((fish.itemSO.id, 0));
            foreach (var recipe in recipes)
            {
                if (recipe.output == null || !seenIds.Contains(recipe.output.id)) continue;
                if (!sources.Recipes.TryGetValue(recipe.output.id, out var routes))
                    sources.Recipes.Add(recipe.output.id, routes = new List<RecipeSO>());
                routes.Add(recipe);
            }
            return sources;
        }

        internal static RecipeQuote QuoteRecipe(GameManager game, RecipeSO recipe)
        {
            if (recipe == null || recipe.output == null || recipe.ingredients == null)
                throw new ArgumentException("Recipe data is incomplete.");
            EnsureContext(game);
            if (quotes.TryGetValue(recipe, out var cached)) return cached;
            // Shared by recipe details and daily report. Inputs count full packs.
            var inputs = recipe.ingredients.Select(ingredient =>
            {
                if (ingredient?.itemSO == null) throw new ArgumentException("Missing recipe ingredient.");
                var raw = ResolveItem(game, ingredient.itemSO);
                decimal unitPrice = raw is ProductSO product ? game.GetWholesalePrice(product) : (decimal)raw.basePrice / raw.amount;
                return (unitPrice, raw.amount, ingredient.amount);
            });
            var quote = CostMath.Quote(inputs, recipe.output.amount, recipe.amount);
            quotes.Add(recipe, quote);
            return quote;
        }
    }
}
