using System;
using System.Collections.Generic;
using System.Linq;

namespace OldMarket.MaterialCost
{
    // Pure accounting code; no writes to game state and no integer truncation per unit.
    public static class CostMath
    {
        public static decimal Recipe(IEnumerable<(decimal UnitPrice, int PackSize, int Packs)> inputs,
            int outputPackSize, int outputPacks)
            => Quote(inputs, outputPackSize, outputPacks).UnitCost;

        public static RecipeQuote Quote(IEnumerable<(decimal UnitPrice, int PackSize, int Packs)> inputs,
            int outputPackSize, int outputPacks)
        {
            if (outputPackSize <= 0 || outputPacks <= 0) throw new ArgumentOutOfRangeException(nameof(outputPackSize));
            decimal total = 0;
            foreach (var input in inputs)
            {
                if (input.UnitPrice < 0 || input.PackSize <= 0 || input.Packs <= 0)
                    throw new ArgumentOutOfRangeException(nameof(inputs));
                total += input.UnitPrice * input.PackSize * input.Packs;
            }
            return new RecipeQuote(total, checked((long)outputPackSize * outputPacks));
        }

        public static decimal Seed(int packPrice, int seeds, int harvest)
        {
            if (packPrice < 0 || seeds <= 0 || harvest <= 0) throw new ArgumentOutOfRangeException(nameof(seeds));
            return (decimal)packPrice / seeds / harvest;
        }

        public static RecipeProfit EstimateProfit(RecipeQuote costs, decimal unitSellingPrice)
        {
            if (costs == null) throw new ArgumentNullException(nameof(costs));
            if (unitSellingPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitSellingPrice));
            return new RecipeProfit(costs, unitSellingPrice);
        }

        public static Summary Summarize(IEnumerable<(decimal Cost, int Price)> sales)
        {
            var values = sales.ToArray();
            if (values.Length == 0) return new Summary();
            decimal cost = values.Sum(x => x.Cost), revenue = values.Sum(x => (decimal)x.Price);
            return new Summary { Count = values.Length, AverageCost = cost / values.Length,
                AveragePrice = revenue / values.Length, Profit = revenue - cost,
                ProfitPercent = cost == 0 ? (decimal?)null : (revenue - cost) / cost * 100 };
        }
    }

    public sealed class RecipeQuote
    {
        public decimal BatchCost { get; }
        public long OutputUnits { get; }
        public decimal UnitCost => BatchCost / OutputUnits;
        internal RecipeQuote(decimal batchCost, long outputUnits)
        {
            BatchCost = batchCost;
            OutputUnits = outputUnits;
        }
    }

    public sealed class Summary
    {
        public int Count;
        public decimal AverageCost, AveragePrice, Profit;
        public decimal? ProfitPercent;
    }

    public sealed class RecipeProfit
    {
        public RecipeQuote Costs { get; }
        public decimal UnitSellingPrice { get; }
        public decimal BatchRevenue => UnitSellingPrice * Costs.OutputUnits;
        public decimal BatchProfit => BatchRevenue - Costs.BatchCost;
        public decimal UnitProfit => UnitSellingPrice - Costs.UnitCost;
        // Match the native daily report's profit / cost denominator, not profit / revenue.
        public decimal? ProfitPercent => Costs.BatchCost == 0 ? (decimal?)null : BatchProfit / Costs.BatchCost * 100;
        internal RecipeProfit(RecipeQuote costs, decimal unitSellingPrice)
        {
            Costs = costs;
            UnitSellingPrice = unitSellingPrice;
        }
    }

    public sealed class CostRules
    {
        private readonly Dictionary<long, List<decimal>> rules = new Dictionary<long, List<decimal>>();
        public void Add(long id, decimal cost)
        {
            if (cost < 0) throw new ArgumentOutOfRangeException(nameof(cost));
            if (!rules.TryGetValue(id, out var values)) rules[id] = values = new List<decimal>();
            if (!values.Contains(cost)) values.Add(cost);
        }
        public bool TryGet(long id, out decimal cost)
        {
            cost = 0;
            if (!rules.TryGetValue(id, out var values) || values.Count != 1) return false;
            cost = values[0];
            return true;
        }
        public bool IsAmbiguous(long id) => rules.TryGetValue(id, out var values) && values.Count > 1;
    }
}
