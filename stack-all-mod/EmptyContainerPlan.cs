using System;

namespace OldMarket.StackAll
{
    internal static class EmptyContainerPlan
    {
        internal static int Count(int count, int width, int group, long itemId, bool reusable,
            Func<int, InventorySlot> read)
        {
            if (!reusable || itemId == -1 || width <= 0 || group < 0 || group >= width ||
                count != ContainerPlan.StorageSize(width)) return 0;
            int result = 0;
            for (int p = 0; p < ContainerPlan.Limit; p++)
            {
                var slot = read(ContainerPlan.Index(width, group, p));
                if (slot.itemId == itemId && slot.amount == 0) result++;
            }
            return result;
        }

        internal static int Find(int count, int width, int group, long itemId, bool reusable,
            Func<int, InventorySlot> read)
        {
            if (!reusable || itemId == -1 || width <= 0 || group < 0 || group >= width ||
                count != ContainerPlan.StorageSize(width)) return -1;
            for (int p = 0; p < ContainerPlan.Limit; p++)
            {
                int index = ContainerPlan.Index(width, group, p);
                var slot = read(index);
                if (slot.itemId == itemId && slot.amount == 0) return index;
            }
            return -1;
        }

        internal static bool Take(InventorySlot[] slots, int width, int group, long itemId,
            bool reusable, out InventorySlot removed)
        {
            removed = ContainerPlan.Empty();
            int index = Find(slots.Length, width, group, itemId, reusable, i => slots[i]);
            if (index < 0) return false;
            removed = slots[index];
            slots[index] = ContainerPlan.Empty();
            ContainerPlan.Promote(slots, width, group);
            return true;
        }
    }
}
