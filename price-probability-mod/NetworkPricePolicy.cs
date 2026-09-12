namespace OldMarket.PriceProbability
{
    public static class NetworkPricePolicy
    {
        // Every client keeps the native request path; only the authoritative host normalizes anchored prices.
        public static int Resolve(bool server, bool spawned, int requested, int? anchored)
            => server && spawned && requested > 0 && anchored.HasValue ? anchored.Value : requested;
    }
}
