using System;
using OldMarket.TreeInfo;

static class Program
{
    static void Main()
    {
        // Verified Apple Tree asset: full growth 3, harvest threshold 7.
        Check("seedling", 0, 7, true, 1, true, "预计还有 7 天成熟（每天浇水）");
        Check("after harvest", 3, 7, true, 10, true, "预计还有 4 天成熟（每天浇水）");
        Check("dry", 9, 11, true, 20, false, "预计还有 2 天成熟（每天浇水） · 今日未浇水");
        Check("last valid night", 10, 11, true, 27, true, "预计还有 1 天成熟（每天浇水）");
        Check("season rolls before growth", 10, 11, true, 28, true, "本季剩余天数不足以成熟");
        Check("too late", 0, 11, true, 20, true, "本季剩余天数不足以成熟");
        Check("off season", 0, 11, false, 1, true, "本季不生产");
        Check("ripe off season", 11, 11, false, 1, false, "已成熟，可采收");
        Check("overripe", 20, 11, true, 28, false, "已成熟，可采收");
        Console.WriteLine("9 harvest estimate cases passed.");
        foreach (string locale in new[] { "en", "zh", "zh-hant", "de", "fr", "it", "ja", "ko", "pt", "ru", "es", "tr", "uk" })
        {
            var text = Texts.For(locale);
            foreach (var value in new[] { text.Season, text.AllSeasons, text.Ready, text.OffSeason, text.TooLate, text.Days, text.Dry })
                if (string.IsNullOrWhiteSpace(value)) throw new Exception("Missing translation: " + locale);
            string header = string.Format(text.Season, "TEST_SEASON");
            string dry = HarvestEstimate.Describe(0, 7, true, 1, false, text);
            if (!header.Contains("TEST_SEASON") || !dry.Contains("7") || !dry.Contains(text.Dry) || dry.Contains("{0}"))
                throw new Exception("Invalid placeholders: " + locale);
            if (HarvestEstimate.Describe(7, 7, false, 28, false, text) != text.Ready ||
                HarvestEstimate.Describe(0, 7, false, 1, true, text) != text.OffSeason ||
                HarvestEstimate.Describe(0, 7, true, 28, true, text) != text.TooLate)
                throw new Exception("Untranslated state: " + locale);
        }
        foreach (var pair in new[] { ("EN-us", "en"), ("pt_BR", "pt"), ("zh-TW", "zh-hant"),
            ("zh-Hant-HK", "zh-hant"), ("zh-Hans-TW", "zh"), ("zh-CN", "zh"), ("unknown", "en"), ("", "en"), (null, "en") })
            if (!ReferenceEquals(Texts.For(pair.Item1), Texts.For(pair.Item2))) throw new Exception("Locale resolution: " + pair.Item1);
        if (Texts.For("en").Ready == Texts.For("de").Ready) throw new Exception("Locale switch did not refresh");
        Console.WriteLine("13 languages and 9 locale aliases/fallbacks passed.");
    }

    static void Check(string name, int counter, int threshold, bool season, int day, bool water, string expected)
    {
        var actual = HarvestEstimate.Describe(counter, threshold, season, day, water, Texts.For("zh"));
        if (actual != expected) throw new Exception(name + ": " + actual);
    }
}
