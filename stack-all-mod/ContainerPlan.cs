using System;

namespace OldMarket.StackAll
{
    // First 'width' entries remain native hotbar slots; each owns 63 backing entries.
    // Each entry is one physical container or seed packet; reusable containers may be empty.
    public static class ContainerPlan
    {
        public const int Limit = 64;
        public static InventorySlot Empty() => new InventorySlot { itemId = -1, amount = -1, cost = -1, dayCounter = -1 };
        public static int StorageSize(int width) => checked(width * Limit);
        public static int Index(int width, int group, int position) => position == 0 ? group : width + group * (Limit - 1) + position - 1;
        public static int Group(int width, int index) => index < width ? index : (index - width) / (Limit - 1);
        public static int Count(InventorySlot[] slots, int width, int group)
        {
            int count = 0;
            for (int p = 0; p < Limit; p++)
                if (slots[Index(width, group, p)].itemId != -1) count++;
            return count;
        }
        public static long Total(InventorySlot[] slots, int width, int group)
        {
            long total = 0;
            for (int p = 0; p < Limit; p++)
            {
                var slot = slots[Index(width, group, p)];
                if (slot.itemId != -1) total += Math.Max(0, slot.amount);
            }
            return total;
        }
        public static bool Add(InventorySlot[] slots, int width, int active, int preferred,
            InventorySlot incoming, int capacity, int maxDays, bool mergeContents = true)
        {
            if (incoming.itemId == -1 || incoming.amount < 0) throw new ArgumentException("Expected a product container");
            int group = -1;
            for (int i = 0; i < active; i++)
                if (slots[i].itemId == incoming.itemId && Count(slots, width, i) < Limit) { group = i; break; }
            if (group < 0 && preferred >= 0 && preferred < active && Count(slots, width, preferred) == 0) group = preferred;
            for (int i = 0; group < 0 && i < active; i++) if (Count(slots, width, i) == 0) group = i;
            if (group < 0) return false;

            // Exactly the native V transfer rule, per physical container. Moving contents leaves
            // the source container present even when its contents reach zero.
            for (int p = 0; mergeContents && p < Limit && incoming.amount > 0; p++)
            {
                int index = Index(width, group, p);
                var target = slots[index];
                if (target.itemId != incoming.itemId || target.amount < 0 ||
                    !StackRules.Fresh(target.amount, target.dayCounter, maxDays) ||
                    !StackRules.Fresh(incoming.amount, incoming.dayCounter, maxDays)) continue;
                int moved = Math.Min(Math.Max(0, capacity - target.amount), incoming.amount);
                if (moved == 0) continue;
                target.cost = StackRules.Cost(target.amount, target.cost, moved, incoming.cost);
                target.dayCounter = StackRules.Age(target.amount, target.dayCounter, moved, incoming.dayCounter);
                target.amount += moved;
                slots[index] = target;
                incoming.amount -= moved;
            }
            for (int p = 0; p < Limit; p++)
            {
                int index = Index(width, group, p);
                if (slots[index].itemId != -1) continue;
                slots[index] = incoming;
                Promote(slots, width, group);
                return true;
            }
            throw new InvalidOperationException("Container slot reservation failed");
        }
        public static bool PutSingle(InventorySlot[] slots, int width, int active, int preferred, InventorySlot item)
        {
            if (preferred >= 0 && preferred < active && Count(slots, width, preferred) == 0)
            { slots[preferred] = item; return true; }
            for (int i = 0; i < active; i++)
                if (Count(slots, width, i) == 0) { slots[i] = item; return true; }
            return false;
        }
        public static void Promote(InventorySlot[] slots, int width, int group)
        {
            var front = slots[group];
            if (front.itemId != -1 && front.amount > 0) return;
            int candidate = -1;
            for (int p = 1; p < Limit; p++)
            {
                int index = Index(width, group, p);
                if (slots[index].itemId == -1) continue;
                if (candidate < 0) candidate = index;
                if (slots[index].amount > 0) { candidate = index; break; }
            }
            if (candidate < 0 || (front.itemId != -1 && slots[candidate].amount <= 0)) return;
            slots[group] = slots[candidate];
            slots[candidate] = front;
        }
        public static InventorySlot[] Trim(InventorySlot[] slots, int width)
        {
            int length = slots.Length;
            while (length > width && slots[length - 1].itemId == -1) length--;
            var result = new InventorySlot[length];
            Array.Copy(slots, result, length);
            return result;
        }
    }
}
