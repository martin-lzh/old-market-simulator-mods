using System;

namespace OldMarket.StackAll
{
    // Game-independent arithmetic; quantities here are contents, never tool durability.
    public static class StackRules
    {
        public const int Limit = 64;

        public static int Transfer(int target, int source) =>
            target < 0 || source <= 0 ? 0 : Math.Min(Math.Max(0, Limit - target), source);

        public static int Cost(int oldAmount, int oldCost, int added, int addedCost)
        {
            if (oldAmount <= 0) return addedCost;
            return (int)(((long)oldAmount * oldCost + (long)added * addedCost) / (oldAmount + added));
        }

        public static int Age(int oldAmount, int oldAge, int added, int addedAge) =>
            Cost(oldAmount, oldAge, added, addedAge);

        public static bool Fresh(int amount, int age, int maxDays) =>
            amount <= 0 || maxDays == -1 || age < maxDays;

        public static bool Compatible(int targetAmount, int targetAge, int sourceAmount, int sourceAge,
            bool product, int maxDays) => product
                ? Fresh(targetAmount, targetAge, maxDays) && Fresh(sourceAmount, sourceAge, maxDays)
                : targetAge == sourceAge;
    }
}
