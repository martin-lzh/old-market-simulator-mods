using System;
using System.Collections.Generic;
using OldMarket.MaterialCost;

internal static class RuntimeRulesTests
{
    internal static int Run()
    {
        int checks = 0;
        void Check(bool value, string name)
        {
            checks++;
            if (!value) throw new Exception(name);
        }
        RuntimeRules.Clear();
        var raw = new ProductSO { id = 1, basePrice = 10, amount = 12 };
        var output = new ProductSO { id = 2, basePrice = 100, amount = 6 };
        var otherRaw = new ProductSO { id = 3, basePrice = 20 };
        var otherOutput = new ProductSO { id = 4, basePrice = 50 };
        var honey = new ProductSO { id = 5, basePrice = 60, amount = 24 };
        var unrelated = new ProductSO { id = 6, basePrice = 1 };
        var recipe = new RecipeSO { output = output, ingredients = new() { new() { itemSO = raw } } };
        var other = new RecipeSO { output = otherOutput, ingredients = new() { new() { itemSO = otherRaw } } };
        // This unused recipe must never be evaluated when viewing/selling other outputs.
        var badUnused = new RecipeSO { output = unrelated, ingredients = new() { new() { itemSO = null } } };
        var machine = new TestRecipeMachine { recipes = new() { recipe, other, badUnused } };
        var prefab = new TestPrefab { Components = new UnityEngine.MonoBehaviour[] { machine, new BlockBeeHive { output = honey } } };
        var equipment = new ItemSO { id = 7, prefab = prefab };
        var game = new GameManager { itemDatabase = new() { raw, output, otherRaw, otherOutput, honey, unrelated, equipment } };

        var first = RuntimeRules.QuoteRecipe(game, recipe);
        var firstProfit = RuntimeRules.QuoteRecipeProfit(game, recipe);
        Check(first.BatchCost == 120 && first.UnitCost == 20, "selected recipe retains pack accounting");
        Check(firstProfit.BatchProfit == 1680 && firstProfit.UnitSellingPrice == 300, "selected output uses native recommended API");
        Check(game.Lookups == 2 && game.WholesaleCalls == 1 && game.RecommendedCalls == 1, "cold selection only looks up its input and output");
        Check(prefab.Scans == 0 && UnityEngine.Object.SceneScans == 0, "recipe details never scan prefabs or the scene");
        Check(ReferenceEquals(first, RuntimeRules.QuoteRecipe(game, recipe)), "duplicate native callback reuses the quote");
        Check(ReferenceEquals(firstProfit, RuntimeRules.QuoteRecipeProfit(game, recipe)), "duplicate native callback reuses profit");
        Check(game.Lookups == 2 && game.WholesaleCalls == 1 && game.RecommendedCalls == 1, "warm quote calls no lookup or pricing functions");
        RuntimeRules.QuoteRecipeProfit(game, other);
        Check(game.Lookups == 4 && game.WholesaleCalls == 2 && game.RecommendedCalls == 2, "next selection loads only its own data");
        Check(ReferenceEquals(firstProfit, RuntimeRules.QuoteRecipeProfit(game, recipe)), "returning to a viewed recipe reuses its quote");
        game.Day = 29; game.PriceMultiplier = 2;
        var nextDay = RuntimeRules.QuoteRecipeProfit(game, recipe);
        Check(nextDay.Costs.BatchCost == 240 && nextDay.UnitSellingPrice == 600, "same normalized day in another season invalidates both prices");
        Check(!ReferenceEquals(firstProfit, nextDay) && game.Lookups == 4, "day change keeps item references but replaces price results");
        Check(game.WholesaleCalls == 3 && game.RecommendedCalls == 3, "day change does not precompute other recipes");

        var newRaw = new ProductSO { id = 1, basePrice = 40, amount = 12 };
        var newOutput = new ProductSO { id = 2, basePrice = 150, amount = 6 };
        game.itemDatabase = new List<ItemSO> { newRaw, newOutput, otherRaw, otherOutput, honey, unrelated, equipment };
        var newMap = RuntimeRules.QuoteRecipeProfit(game, recipe);
        Check(newMap.Costs.BatchCost == 960 && newMap.UnitSellingPrice == 900, "database replacement with unchanged count uses active map products");
        Check(game.Lookups == 6, "new database clears requested-item cache");
        var otherGame = new GameManager { Day = 29, itemDatabase = game.itemDatabase };
        Check(RuntimeRules.QuoteRecipeProfit(otherGame, recipe).Costs.BatchCost == 480, "new game with same day/database cannot inherit old prices");
        otherGame.itemDatabase.Add(new ItemSO { id = 99 });
        RuntimeRules.QuoteRecipe(otherGame, recipe);
        Check(otherGame.Lookups == 3, "in-place database count change invalidates item cache");

        RuntimeRules.Clear();
        int scans = UnityEngine.Object.SceneScans;
        RuntimeRules.BuildReport(game, Array.Empty<long>());
        Check(prefab.Scans == 0 && UnityEngine.Object.SceneScans == scans, "empty material report does not discover production rules");
        var honeyReport = RuntimeRules.BuildReport(game, new[] { honey.id });
        Check(honeyReport.TryGet(honey.id, out var honeyCost) && honeyCost == 0, "deferred discovery preserves zero honey cost");
        Check(prefab.Scans == 1 && UnityEngine.Object.SceneScans == scans + 1, "first enabled report discovers sources once");
        int callsBeforeReport = game.WholesaleCalls;
        var report = RuntimeRules.BuildReport(game, new[] { output.id });
        Check(report.TryGet(output.id, out var cost) && cost == 160, "report quotes only its sold recipe, using active day/map");
        Check(game.WholesaleCalls == callsBeforeReport + 1, "unsold recipes have no pricing calls");
        Check(prefab.Scans == 1 && UnityEngine.Object.SceneScans == scans + 1, "repeated report reuses discovered metadata");
        game.Day++;
        game.PriceMultiplier = 1;
        Check(RuntimeRules.BuildReport(game, new[] { output.id }).TryGet(output.id, out cost) && cost == 80, "cached report metadata does not freeze daily ingredient prices");
        Check(prefab.Scans == 1, "day change does not rescan static prefab metadata");
        var alternate = new RecipeSO { output = output, ingredients = new() { new() { itemSO = otherRaw } } };
        machine.recipes.Add(alternate);
        RuntimeRules.Clear(); // The runtime does this on scene loads.
        var ambiguous = RuntimeRules.BuildReport(game, new[] { output.id });
        Check(!ambiguous.TryGet(output.id, out _) && ambiguous.IsAmbiguous(output.id), "deferred report preserves conflicting production routes");
        Check(prefab.Scans == 2 && UnityEngine.Object.SceneScans == scans + 2, "scene invalidation rediscovers production sources");
        Check(!ambiguous.TryGet(999, out _), "unknown sales retain native fallback");
        RuntimeRules.Clear();
        return checks;
    }
}
