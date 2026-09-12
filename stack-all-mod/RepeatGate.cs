namespace OldMarket.StackAll
{
    // One immediate action, then paced individual actions for the original stack only.
    public sealed class RepeatGate
    {
        private bool started, blocked;
        private int slot, expected;
        private long item;
        private float next;
        public bool Tick(bool held, bool pressed, bool allowed, int currentSlot, long currentItem, int count, float now)
        {
            if (!held) { started = blocked = false; return false; }
            if (blocked) return false;
            if (!allowed || currentItem == -1 || count <= 0) { blocked = true; return false; }
            if (!started)
            {
                if (!pressed) { blocked = true; return false; }
                started = true;
                slot = currentSlot;
                item = currentItem;
                expected = count - 1;
                next = now + .6f;
                return true;
            }
            if (slot != currentSlot || item != currentItem || count != expected || expected == 0)
            { blocked = true; return false; }
            if (now < next) return false;
            expected--;
            next = now + .12f;
            return true;
        }
    }
}
