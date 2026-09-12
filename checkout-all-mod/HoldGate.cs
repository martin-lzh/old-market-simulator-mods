namespace OldMarket.CheckoutAll
{
    // Activate one continuous session per hold; cancellation requires releasing the key.
    public sealed class HoldGate
    {
        private object target;
        private float elapsed;
        private bool latched;
        public void BlockUntilRelease() { latched = true; }
        public bool Tick(bool held, object current, float seconds, float threshold)
        {
            if (!held) { target = null; elapsed = 0; latched = false; return false; }
            if (latched) return false;
            if (!ReferenceEquals(target, current)) { target = current; elapsed = 0; }
            if (current == null) { elapsed = 0; return false; }
            elapsed += seconds;
            if (elapsed < threshold) return false;
            latched = true;
            return true;
        }
    }
}
