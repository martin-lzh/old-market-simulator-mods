using System;

namespace OldMarket.StackAll
{
    public static class StackPlan
    {
        public static int Add(InventorySlot[] slots, int active, int preferred, InventorySlot incoming,
            Func<InventorySlot, InventorySlot, bool> canMerge)
        {
            int remaining = incoming.amount;
            if (remaining <= 0) return remaining;
            for (int i = 0; i < active && remaining > 0; i++)
            {
                if (slots[i].itemId != incoming.itemId || slots[i].amount < 0 || !canMerge(slots[i], incoming)) continue;
                int moved = StackRules.Transfer(slots[i].amount, remaining);
                if (moved == 0) continue;
                var target = slots[i];
                target.cost = StackRules.Cost(target.amount, target.cost, moved, incoming.cost);
                target.dayCounter = StackRules.Age(target.amount, target.dayCounter, moved, incoming.dayCounter);
                target.amount += moved;
                slots[i] = target;
                remaining -= moved;
            }
            if (preferred >= 0 && preferred < active) Fill(slots, preferred, incoming, ref remaining);
            for (int i = 0; i < active && remaining > 0; i++) Fill(slots, i, incoming, ref remaining);
            return remaining;
        }

        private static void Fill(InventorySlot[] slots, int index, InventorySlot incoming, ref int remaining)
        {
            if (remaining <= 0 || slots[index].itemId != -1) return;
            incoming.amount = Math.Min(remaining, StackRules.Limit);
            slots[index] = incoming;
            remaining -= incoming.amount;
        }
    }
}
