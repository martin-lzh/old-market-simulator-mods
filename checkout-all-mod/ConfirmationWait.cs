namespace OldMarket.CheckoutAll
{
    // Bound unacknowledged interactions, not the time between customers.
    public sealed class ConfirmationWait
    {
        private float? started;
        public bool TimedOut(bool pending, float now)
        {
            if (!pending) { Reset(); return false; }
            if (!started.HasValue) started = now;
            return now - started.Value >= 10f;
        }
        public void Reset() { started = null; }
    }
}
