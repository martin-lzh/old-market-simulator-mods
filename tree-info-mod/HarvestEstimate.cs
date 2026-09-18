using System;

namespace OldMarket.TreeInfo
{
    internal static class HarvestEstimate
    {
        // The game increments growth after advancing the calendar, using the NEW season.
        internal static string Describe(int counter, int threshold, bool inSeason, int seasonDay, bool watered, Texts text)
        {
            long remaining = Math.Max(0L, (long)threshold - counter);
            if (remaining == 0) return text.Ready;
            if (!inSeason) return text.OffSeason;
            if (remaining > 28 - seasonDay) return text.TooLate;
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, text.Days, remaining)
                + (watered ? "" : " · " + text.Dry);
        }
    }
}
