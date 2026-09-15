namespace OldMarket.StackAll
{
    internal static class StackCompatibility
    {
        // Verified game 2.1.6 uses this item's dayCounter for fish-trap harvest count.
        internal const long FishTrapId = 1725814069046L;

        internal static bool CanMerge(ItemSO item, InventorySlot target, InventorySlot source)
        {
            if (item == null || item is ToolSO) return false;
            var product = item as ProductSO;
            // World items accumulate elapsed days even when they have no freshness/state.
            // Only animals and the fish trap give this counter meaning outside products.
            if (product == null && !(item is AnimalSO) && item.id != FishTrapId) return true;
            return StackRules.Compatible(target.amount, target.dayCounter, source.amount, source.dayCounter,
                product != null, product != null ? product.maxDays : -1);
        }
    }
}
