namespace OldMarket.StackAll
{
    internal static class PacketRules
    {
        // Seed amount is the remaining contents of a packet, not a count of packets.
        internal static bool IsPacket(ItemSO item) => item is ProductSO || item is SeedSO;

        internal static int StackLimit(ItemSO item) =>
            item is ToolSO || IsPacket(item) ? item.stackSize : StackRules.Limit;

        internal static bool Add(InventorySlot[] slots, int width, int active, int preferred,
            InventorySlot incoming, ItemSO item) => ContainerPlan.Add(slots, width, active, preferred,
                incoming, item.amount, item is ProductSO product ? product.maxDays : -1,
                mergeContents: item is ProductSO);
    }
}
