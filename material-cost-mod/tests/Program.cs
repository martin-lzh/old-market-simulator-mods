using System;
using OldMarket.MaterialCost;

int checks = 0;
void Equal(decimal expected, decimal actual, string name)
{
    checks++;
    if (expected != actual) throw new Exception($"{name}: expected {expected}, got {actual}");
}
void Check(bool condition, string name)
{
    checks++;
    if (!condition) throw new Exception(name);
}
// Pack counts, multi-pack output, non-integral costs, market price changes.
Equal(15, CostMath.Recipe(new[] { (10m, 24, 2), (20m, 12, 1) }, 24, 2), "recipe units");
Equal(18, CostMath.Recipe(new[] { (12m, 24, 2), (24m, 12, 1) }, 24, 2), "season/event input prices");
// Verified serialized Aged Cheese recipe: 2 goat-milk packs + eggs + cumin -> 6 units.
Equal(190, CostMath.Recipe(new[] { (30m, 12, 2), (12m, 10, 1), (30m, 10, 1) }, 6, 1), "actual Aged Cheese recipe");
Equal(2.5m, CostMath.Seed(100, 10, 4), "consumed seed");
Equal(1m / 3m, CostMath.Recipe(new[] { (1m, 1, 1) }, 3, 1), "retain fractions");
// Honey has no recurring consumable: high original cost is irrelevant to estimate.
var honey = CostMath.Summarize(new[] { (0m, 60), (0m, 80) });
Equal(0, honey.AverageCost, "honey material cost");
Equal(140, honey.Profit, "honey revenue minus cost");
Check(honey.ProfitPercent == null, "zero-cost percentage must be undefined");
var mixed = CostMath.Summarize(new[] { (10m, 30), (20m, 60) });
Equal(15, mixed.AverageCost, "mixed costs");
Equal(60, mixed.Profit, "aggregate profit");
Equal(200, mixed.ProfitPercent.Value, "aggregate percentage");
Equal(-5, CostMath.Summarize(new[] { (10m, 5) }).Profit, "loss");
Check(CostMath.Summarize(Array.Empty<(decimal, int)>()).Count == 0, "empty report");
var rules = new CostRules();
rules.Add(1, 0); rules.Add(1, 0);
Check(rules.TryGet(1, out var zero) && zero == 0, "duplicate identical routes");
rules.Add(2, 10); rules.Add(2, 20);
Check(!rules.TryGet(2, out _) && rules.IsAmbiguous(2), "ambiguous routes fall back");
Check(!rules.TryGet(3, out _), "unknown product falls back");
try { CostMath.Recipe(new[] { (1m, 1, 1) }, 0, 1); throw new Exception("accepted zero output"); }
catch (ArgumentOutOfRangeException) { checks++; }
var quote = CostMath.Quote(new[] { (30m, 12, 2), (12m, 10, 1), (30m, 10, 1) }, 6, 1);
Equal(1140, quote.BatchCost, "recipe page batch cost");
Equal(6, quote.OutputUnits, "recipe page individual output count");
Equal(190, quote.UnitCost, "recipe page/report same unit cost");
var fractionalQuote = CostMath.Quote(new[] { (1m, 1, 1) }, 3, 2);
Equal(1, fractionalQuote.BatchCost, "batch retains exact total despite fractional unit cost");
Equal(6, fractionalQuote.OutputUnits, "output packs multiplied by units");
Equal(CostMath.Recipe(new[] { (1m, 1, 1) }, 3, 2), fractionalQuote.UnitCost, "shared fractional quote");
var freeQuote = CostMath.Quote(Array.Empty<(decimal, int, int)>(), 24, 1);
Equal(0, freeQuote.BatchCost, "no consumable inputs");
Equal(0, freeQuote.UnitCost, "zero material quote");
var forecast = CostMath.EstimateProfit(quote, 285);
Equal(1710, forecast.BatchRevenue, "recommended unit price times all six outputs");
Equal(570, forecast.BatchProfit, "batch revenue less exact ingredient cost");
Equal(95, forecast.UnitProfit, "per-unit estimated profit");
Equal(50, forecast.ProfitPercent.Value, "profit percentage uses cost denominator, not sales");
Equal(1140, CostMath.EstimateProfit(quote, 380).BatchProfit, "changed daily output price refreshes profit");
Equal(0, CostMath.EstimateProfit(quote, 190).ProfitPercent.Value, "break-even quote");
Equal(-50, CostMath.EstimateProfit(quote, 95).ProfitPercent.Value, "loss remains a negative percentage");
Equal(5, CostMath.EstimateProfit(fractionalQuote, 1).BatchProfit, "batch profit never uses rounded unit cost");
var freeForecast = CostMath.EstimateProfit(freeQuote, 60);
Equal(1440, freeForecast.BatchProfit, "zero-cost output retains full forecast revenue");
Check(freeForecast.ProfitPercent == null, "zero-cost forecast percentage is undefined");
Equal(-1140, CostMath.EstimateProfit(quote, 0).BatchProfit, "zero sale price is a full material loss");
try { CostMath.EstimateProfit(quote, -1); throw new Exception("accepted negative selling price"); }
catch (ArgumentOutOfRangeException) { checks++; }
var order = CostMath.EstimateProfit(new RecipeQuote(7m * 12, 12), 10);
Equal(36, order.BatchProfit, "order uses integer recommended price, not nominal markup");
Equal(42.86m, decimal.Round(order.ProfitPercent.Value, 2), "order compares unit sale price with unit wholesale price");
Check(CostMath.EstimateProfit(new RecipeQuote(0, 12), 10).ProfitPercent == null, "free order has no defined return percentage");
Console.WriteLine($"PASS: {checks} accounting checks");
Console.WriteLine($"PASS: {RuntimeRulesTests.Run()} lazy-loading and cache checks");
