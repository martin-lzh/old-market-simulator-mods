using OldMarket.StackAll;

int checks = 0;
void Check(bool result, string name) { if (!result) throw new Exception(name); checks++; }
InventorySlot Slot(long id, int amount, int cost = 10, int age = 0) =>
    new InventorySlot { itemId = id, amount = amount, cost = cost, dayCounter = age };
InventorySlot Empty() => Slot(-1, -1, -1, -1);
bool Merge(InventorySlot a, InventorySlot b) => StackRules.Compatible(a.amount, a.dayCounter, b.amount, b.dayCounter, true, 8);
var slots = new[] { Slot(1, 24, 10, 1), Empty() };
Check(StackPlan.Add(slots, 2, 1, Slot(1, 24, 20, 4), Merge) == 0, "two boxes fit");
Check(slots[0].amount == 48 && slots[1].itemId == -1, "two boxes use one inventory slot");
Check(slots[0].cost == 15 && slots[0].dayCounter == 2, "native V weighted cost and truncated age");
slots = new[] { Slot(1, 60, 10, 1), Empty() };
Check(StackPlan.Add(slots, 2, 0, Slot(1, 24, 30, 5), Merge) == 0, "split incoming box");
Check(slots[0].amount == 64 && slots[1].amount == 20 && slots[1].cost == 30 && slots[1].dayCounter == 5, "overflow retains its own metadata");
slots = new[] { Slot(1, 60), Slot(2, 9), Empty() };
Check(StackPlan.Add(slots, 2, 0, Slot(1, 24), Merge) == 20, "full inventory overflow returned");
Check(slots[0].amount == 64 && slots[1].itemId == 2 && slots[1].amount == 9 && slots[2].itemId == -1, "no eviction or locked-slot use");
slots = new[] { Slot(1, 24, 10, 8), Empty() };
StackPlan.Add(slots, 2, 0, Slot(1, 24, 10, 0), Merge);
Check(slots[0].amount == 24 && slots[0].dayCounter == 8 && slots[1].amount == 24, "expired box cannot rejuvenate");
slots = new[] { Slot(1, 24), Empty() };
StackPlan.Add(slots, 2, 0, Slot(1, 24, 10, 8), Merge);
Check(slots[0].amount == 24 && slots[1].dayCounter == 8, "expired source cannot contaminate fresh target");
slots = new[] { Slot(1, 0, 99, 99), Empty() };
StackPlan.Add(slots, 2, 1, Slot(1, 24, 12, 1), Merge);
Check(slots[0].amount == 24 && slots[0].cost == 12 && slots[0].dayCounter == 1, "empty box metadata carries no weight");
slots = new[] { Empty(), Empty(), Empty() };
StackPlan.Add(slots, 3, 2, Slot(1, 140), Merge);
Check(slots[2].amount == 64 && slots[0].amount == 64 && slots[1].amount == 12, "oversized batch splits, current empty slot first");
Check(StackRules.Cost(63, int.MaxValue, 1, int.MaxValue) == int.MaxValue, "weighted multiplication does not overflow");
Check(StackRules.Fresh(50, 999, -1), "nonperishable items merge");
Check(!StackRules.Compatible(1, 1, 1, 3, false, -1), "non-product state values are not averaged");
Check(StackRules.Compatible(1, 3, 1, 3, false, -1), "same non-product state can stack");
var random = new Random(64);
for (int trial = 0; trial < 2000; trial++)
{
    slots = Enumerable.Range(0, 8).Select(_ => random.Next(3) == 0 ? Empty() : Slot(random.Next(1, 4), random.Next(1, 65))).ToArray();
    int active = random.Next(1, 9), amount = random.Next(1, 1025);
    var before = (InventorySlot[])slots.Clone();
    int remainder = StackPlan.Add(slots, active, random.Next(active), Slot(1, amount), Merge);
    Check(slots.Where(s => s.itemId == 1).Sum(s => s.amount) + remainder == before.Where(s => s.itemId == 1).Sum(s => s.amount) + amount, "quantity conservation");
    Check(slots.All(s => s.amount <= 64), "capacity bound");
    Check(Enumerable.Range(active, 8 - active).All(i => slots[i].Equals(before[i])), "locked slots unchanged");
    Check(Enumerable.Range(0, 8).Where(i => before[i].itemId > 1).All(i => slots[i].Equals(before[i])), "other items unchanged");
}
InventorySlot[] Storage(int width) => Enumerable.Repeat(Empty(), ContainerPlan.StorageSize(width)).ToArray();
slots = Storage(8);
Check(ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 24), 24, 8), "first full basket");
Check(ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 24), 24, 8), "second full basket");
Check(ContainerPlan.Total(slots, 8, 0) == 48 && ContainerPlan.Count(slots, 8, 0) == 2, "HUD: 48 goods and 2 containers");
Check(slots[0].amount == 24, "native held object is one basket, not pooled goods");
slots = Storage(8);
for (int i = 0; i < 3; i++) Check(ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 0), 24, 8), "empty basket admitted");
Check(ContainerPlan.Total(slots, 8, 0) == 0 && ContainerPlan.Count(slots, 8, 0) == 3, "HUD: 0 goods and 3 empty containers");
slots = Storage(8);
ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 10, 10, 1), 24, 8);
ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 6, 30, 4), 24, 8);
Check(slots[0].amount == 16 && slots[0].cost == 17 && slots[0].dayCounter == 2, "V weighted values within first basket");
Check(ContainerPlan.Count(slots, 8, 0) == 2 && slots[8].amount == 0, "drained source basket is retained");
slots[0] = Empty(); // Native drop/place/cargo transfers exactly the front object.
ContainerPlan.Promote(slots, 8, 0);
Check(slots[0].amount == 0 && ContainerPlan.Count(slots, 8, 0) == 1, "taking full basket exposes the empty one");
slots = Storage(8);
ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 24, 9, 8), 24, 8);
ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 3, 30, 1), 24, 8);
Check(slots[0].amount == 24 && slots[0].dayCounter == 8 && slots[8].amount == 3 && slots[8].dayCounter == 1, "expired basket can share group without mixing contents");
slots = Storage(8);
for (int i = 0; i < 64; i++) Check(ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 85), 85, 8), "container capacity admission");
Check(ContainerPlan.Total(slots, 8, 0) == 5440 && ContainerPlan.Count(slots, 8, 0) == 64, "goods exceed 64; 64 physical containers");
var full = (InventorySlot[])slots.Clone();
Check(!ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 0), 85, 8) && slots.SequenceEqual(full), "65th container overflows atomically");
Check(ContainerPlan.Add(slots, 8, 2, 1, Slot(1, 0), 85, 8) && ContainerPlan.Count(slots, 8, 1) == 1, "65th container uses next unlocked slot");
var saved = ContainerPlan.Trim(slots, 8);
var restored = Storage(8);
Array.Copy(saved, restored, saved.Length);
Check(restored.SequenceEqual(slots), "save/reload retains every basket and field");
slots = Storage(8);
slots[0] = Slot(1, 48); // An existing 0.1.0 merged basket has no historical container count.
Check(ContainerPlan.Count(slots, 8, 0) == 1 && ContainerPlan.Total(slots, 8, 0) == 48, "legacy basket is not assigned invented container count");
Check(ContainerPlan.Trim(slots, 8).Length == 8, "no backing baskets produces native save length");
slots = Storage(8);
ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 24), 24, 8);
ContainerPlan.Add(slots, 8, 1, 0, Slot(1, 24), 24, 8);
full = (InventorySlot[])slots.Clone();
Check(!ContainerPlan.PutSingle(slots, 8, 1, 0, Slot(2, 50)) && slots.SequenceEqual(full), "full inventory tool pickup cannot evict a container group");
Check(ContainerPlan.PutSingle(slots, 8, 3, 1, Slot(2, 50)) && ContainerPlan.PutSingle(slots, 8, 3, 1, Slot(2, 10)), "same tool occupies separate visible slots");
Check(slots[1].amount == 50 && slots[2].amount == 10, "individual tool durability remains unchanged");
Check(Enumerable.Range(0, 8).SelectMany(g => Enumerable.Range(0, 64).Select(p => ContainerPlan.Index(8, g, p))).Distinct().Count() == 512, "all backing indices disjoint");
for (int trial = 0; trial < 150; trial++)
{
    slots = Storage(8);
    long expectedGoods = 0;
    int expectedContainers = 0;
    for (int step = 0; step < 180; step++)
    {
        int quantity = random.Next(0, 86);
        if (ContainerPlan.Add(slots, 8, 2, 0, Slot(1, quantity, random.Next(100), random.Next(10)), 85, 8))
        { expectedGoods += quantity; expectedContainers++; }
        if (random.Next(3) == 0)
        {
            int group = random.Next(2);
            if (slots[group].itemId != -1)
            {
                expectedGoods -= slots[group].amount;
                expectedContainers--;
                slots[group] = Empty();
                ContainerPlan.Promote(slots, 8, group);
            }
        }
        Check(ContainerPlan.Total(slots, 8, 0) + ContainerPlan.Total(slots, 8, 1) == expectedGoods, "container goods conserved after pickup/removal");
        Check(ContainerPlan.Count(slots, 8, 0) + ContainerPlan.Count(slots, 8, 1) == expectedContainers, "physical baskets conserved after pickup/removal");
        Check(ContainerPlan.Count(slots, 8, 0) <= 64 && ContainerPlan.Count(slots, 8, 1) <= 64, "container limit");
    }
}
var gate = new RepeatGate();
Check(gate.Tick(true, true, true, 0, 1, 3, 0), "short press emits one container immediately");
Check(!gate.Tick(true, false, true, 0, 1, 2, .59f), "hold threshold prevents accidental repeat");
Check(gate.Tick(true, false, true, 0, 1, 2, .61f), "long press emits second container");
Check(!gate.Tick(true, false, true, 0, 1, 1, .65f), "paced individually");
Check(gate.Tick(true, false, true, 0, 1, 1, .74f), "third container separately");
Check(!gate.Tick(true, false, true, 0, 1, 1, 2), "no restarting on new contents while held");
gate.Tick(false, false, true, 0, 1, 3, 3);
Check(gate.Tick(true, true, true, 0, 1, 3, 4), "release rearms");
Check(!gate.Tick(true, false, true, 1, 1, 2, 5), "changing slot cancels");
Check(!gate.Tick(true, false, true, 0, 1, 2, 6), "switching back cannot resume without release");
gate = new RepeatGate();
gate.Tick(true, true, true, 0, 1, 4, 0);
Check(!gate.Tick(true, false, false, 0, 1, 3, .8f), "menu or focus loss cancels");
Check(!gate.Tick(true, false, true, 0, 1, 3, 1.8f), "return from menu does not resume");
gate = new RepeatGate();
gate.Tick(true, true, true, 0, 1, 2, 0);
Check(!gate.Tick(true, false, true, 0, 1, 3, 1), "new pickup cannot extend original batch");
gate = new RepeatGate();
Check(!gate.Tick(true, false, true, 0, 1, 3, 1), "already-held key cannot start batch");
gate = new RepeatGate();
gate.Tick(true, true, true, 0, 1, 64, 0);
Check(gate.Tick(true, false, true, 0, 1, 63, 10), "low FPS still emits only one action");
Check(!gate.Tick(true, false, true, 0, 1, 62, 10), "no catch-up burst on same frame");
Console.WriteLine($"PASS: {checks} stack, container and input checks");

// Test stand-in for the native network value type: no Unity or game code is executed.
public struct InventorySlot { public long itemId; public int amount, cost, dayCounter; }
