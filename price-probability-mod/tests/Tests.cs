using System;
using OldMarket.PriceProbability;
class Tests
{
    static int checks;
    static void Check(bool pass, string name) { checks++; if (!pass) throw new Exception(name); }
    static void Main()
    {
        Check(PriceMath.Price(1, 2000, 25, 3, 2500) == 2500, "100% uses recommended");
        Check(Math.Abs(PriceMath.Probability(3000, 2000, 25, 3) - .5005) < 1e-9, "rounding-aware shark probability");
        Check(PriceMath.Probability(3501, 2000, 25, 3) == 0, "zero above threshold");
        foreach (int wholesale in new[] { 1, 3, 20, 450, 550, 2000, 2800, 4000 })
        foreach (int markup in new[] { 0, 20, 25, 50 })
        foreach (float multiplier in new[] { 1f, 1.5f, 3f })
        {
            int recommended = (int)Math.Round(wholesale * (100f + markup) / 100f);
            int previous = int.MaxValue;
            for (int percent = 0; percent <= 100; percent++)
            {
                int price = PriceMath.Price(percent / 100d, wholesale, markup, multiplier, recommended);
                Check(price >= 1 && price <= previous, "inverse monotonic");
                previous = price;
                double actual = PriceMath.Probability(price, wholesale, markup, multiplier);
                Check(actual >= 0 && actual <= 1, "bounded");
                if (percent == 0) Check(actual == 0, "zero endpoint");
                if (percent == 100) Check(price == recommended, "native recommendation");
                if (percent > 0 && percent < 100 && markup > 0 && multiplier > 1)
                {
                    double step = 1 / (wholesale * markup / 100d * (multiplier - 1));
                    Check(Math.Abs(actual - percent / 100d) <= step / 2 + 1e-9, "nearest integer probability");
                }
            }
        }
        Check(PriceMath.Price(.5, 2800, 25, 3, 3500) > PriceMath.Price(.5, 2000, 25, 3, 2500), "season repricing");
        Check(PriceMath.Price(1, 4000, 25, 3, 5000) == 5000, "event 100% recommendation");
                foreach (bool server in new[] { false, true })
        foreach (bool spawned in new[] { false, true })
        foreach (int? anchor in new int?[] { null, 2500, 3500 })
        foreach (int request in new[] { -1, 0, 1, 1000, 5000 })
        {
            int result = NetworkPricePolicy.Resolve(server, spawned, request, anchor);
            Check(result == (server && spawned && request > 0 && anchor.HasValue ? anchor.Value : request), "authority matrix");
            Check(NetworkPricePolicy.Resolve(server, spawned, result, anchor) == result, "send/execute idempotence");
        }
        int effective = 2500;
        foreach (int remoteRequest in new[] { 100, 9000, 2500, 700 })
        {
            effective = NetworkPricePolicy.Resolve(true, true, remoteRequest, 2500);
            Check(effective == 2500, "concurrent vanilla client cannot transiently overwrite anchor");
        }
        Check(NetworkPricePolicy.Resolve(false, true, 9000, 2500) == 9000, "client-only mod preserves request");
        Check(NetworkPricePolicy.Resolve(true, true, 9000, null) == 9000, "anchor removed permits manual update");
        Console.WriteLine($"Passed {checks} pricing checks.");
    }
}
