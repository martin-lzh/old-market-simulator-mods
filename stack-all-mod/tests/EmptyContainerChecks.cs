using OldMarket.StackAll;

internal static class EmptyContainerChecks
{
    internal static void Run(Action<bool, string> check)
    {
        InventorySlot Slot(int amount, int cost, int day) => new() { itemId = 42, amount = amount, cost = cost, dayCounter = day };
        InventorySlot[] Storage() => Enumerable.Repeat(ContainerPlan.Empty(), ContainerPlan.StorageSize(8)).ToArray();
        int Count(InventorySlot[] slots) => EmptyContainerPlan.Count(slots.Length, 8, 2, 42, true, i => slots[i]);
        for (int position = 0; position < 64; position++)
        {
            var slots = Storage();
            for (int p = 0; p < 64; p++) slots[ContainerPlan.Index(8, 2, p)] = Slot(1 + p, 10 + p, p);
            int target = ContainerPlan.Index(8, 2, position);
            slots[target] = Slot(0, 99, 7);
            slots[0] = Slot(0, 55, 3); // A different visible group must not be touched.
            var filled = slots.Where(s => s.amount > 0).OrderBy(s => s.cost).ToArray();
            check(Count(slots) == 1, "empty count covers all backing positions");
            check(EmptyContainerPlan.Take(slots, 8, 2, 42, true, out var removed), "remove empty at any backing position");
            check(removed.amount == 0 && removed.cost == 99 && removed.dayCounter == 7, "empty metadata retained for spawn");
            check(slots.Where(s => s.amount > 0).OrderBy(s => s.cost).SequenceEqual(filled), "filled and partial containers conserved");
            check(slots[0].cost == 55 && slots[0].amount == 0, "other groups untouched");
            check(slots[2].amount > 0 && Count(slots) == 0, "last empty leaves filled selection and disables action");
            var before = slots.ToArray();
            check(!EmptyContainerPlan.Take(slots, 8, 2, 42, true, out _) && slots.SequenceEqual(before), "no empty is a no-op");
        }
        var batch = Storage();
        batch[2] = Slot(24, 6, 4);
        for (int p = 1; p <= 3; p++) batch[ContainerPlan.Index(8, 2, p)] = Slot(0, p, p);
        var gate = new RepeatGate();
        bool Act(bool held, bool pressed, bool allowed, float now)
        {
            if (!gate.Tick(held, pressed, allowed, 2, 42, Count(batch), now)) return false;
            return EmptyContainerPlan.Take(batch, 8, 2, 42, true, out _);
        }
        check(Act(true, true, true, 0) && Count(batch) == 2, "G press removes exactly one empty");
        check(!Act(true, false, true, .59f), "G waits before repeating");
        check(Act(true, false, true, .61f) && Count(batch) == 1, "G hold repeats only empties");
        check(!Act(true, false, true, .65f), "G repeat interval enforced");
        check(Act(true, false, true, .74f) && Count(batch) == 0, "G repeat exhausts empty count");
        check(!Act(true, false, true, 2) && batch[2].amount == 24, "held G never falls through to filled container");
        batch[ContainerPlan.Index(8, 2, 1)] = Slot(0, 9, 9);
        check(!Act(true, false, true, 3), "new empties cannot restart exhausted held key");
        Act(false, false, true, 4);
        check(Act(true, true, true, 5), "release and press rearms empty disposal");
        batch[ContainerPlan.Index(8, 2, 1)] = Slot(0, 9, 9);
        var copy = batch.ToArray();
        check(!EmptyContainerPlan.Take(batch, 8, 2, 42, false, out _) && batch.SequenceEqual(copy), "seed/tool/disposable records excluded");
        check(!EmptyContainerPlan.Take(batch, 8, -1, 42, true, out _) && batch.SequenceEqual(copy), "invalid selection is a no-op");
        check(!EmptyContainerPlan.Take(batch, 8, 2, 99, true, out _) && batch.SequenceEqual(copy), "different product excluded");
        check(EmptyContainerPlan.Count(8, 8, 2, 42, true, i => batch[i]) == 0, "uninitialized backing storage disabled");
        var onlyEmpty = Storage();
        onlyEmpty[2] = Slot(0, 4, 2);
        check(EmptyContainerPlan.Take(onlyEmpty, 8, 2, 42, true, out _) && onlyEmpty[2].itemId == -1, "single empty leaves vacant slot");
    }
}
