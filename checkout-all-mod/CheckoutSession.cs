namespace OldMarket.CheckoutAll
{
    public sealed class CheckoutSession
    {
        private readonly HoldGate hold = new HoldGate();
        public object Target { get; private set; }
        public bool Toggled { get; private set; }

        public void Tick(bool held, bool togglePressed, object target, float seconds, float threshold)
        {
            bool start = hold.Tick(held, target, seconds, threshold);
            if (Target != null && !ReferenceEquals(Target, target)) { Cancel(); return; }
            if (target == null) return;
            if (togglePressed)
            {
                if (Toggled) { Cancel(); return; }
                Target = target;
                Toggled = true;
                return;
            }
            if (!held && !Toggled) { Target = null; return; }
            if (start) Target = target;
        }

        public void Cancel()
        {
            Target = null;
            Toggled = false;
            hold.BlockUntilRelease();
        }
    }
}
