using System;

namespace OldMarket.PriceProbability
{
    // Continuous uniform model of the native random threshold, including integer rounding.
    public static class PriceMath
    {
        public static double Probability(int price, int wholesale, int markup, float multiplier)
        {
            Bounds(wholesale, markup, multiplier, out double low, out double high);
            if (high <= low) return price <= Math.Round(low, MidpointRounding.ToEven) ? 1 : 0;
            return Math.Max(0, Math.Min(1, (high - (price - .5)) / (high - low)));
        }

        public static int Price(double probability, int wholesale, int markup, float multiplier, int recommended)
        {
            if (double.IsNaN(probability) || double.IsInfinity(probability)) throw new ArgumentOutOfRangeException(nameof(probability));
            Bounds(wholesale, markup, multiplier, out double low, out double high);
            if (probability >= 1) return Math.Max(1, recommended);
            if (probability <= 0) return Clamp(Math.Floor(high + .5) + 1);
            if (high <= low) return Math.Max(1, recommended);
            double ideal = high + .5 - probability * (high - low);
            return Clamp(Math.Round(ideal, MidpointRounding.ToEven));
        }

        private static int Clamp(double value) => (int)Math.Max(1, Math.Min(int.MaxValue, value));
        private static void Bounds(int wholesale, int markup, float multiplier, out double low, out double high)
        {
            if (wholesale < 0 || markup < 0 || float.IsNaN(multiplier) || float.IsInfinity(multiplier) || multiplier < 1)
                throw new ArgumentOutOfRangeException("Unsupported pricing parameters");
            low = wholesale * (100d + markup) / 100d;
            high = wholesale * (100d + markup * (double)multiplier) / 100d;
        }
    }
}
