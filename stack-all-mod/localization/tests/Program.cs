using System.Reflection;
using System.Text.RegularExpressions;
using OldMarket.StackAll.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using static Localization.Tests.FakeOperations;

namespace StackLocalization.Tests;

internal static class Program
{
    private static readonly string[] Locales =
        ["en", "zh", "zh-Hant", "fr", "de", "it", "es", "pt", "ja", "ko", "ru", "tr", "uk"];
    private static readonly Regex Positional = new(@"\{\d+\}", RegexOptions.Compiled);

    private static void Main()
    {
        Equal(Locales.Order(), Catalog.Languages.Order(), "locales");
        Equal(new[] { "hold_repeat", "drop_empty" }, Catalog.Keys, "keys");
        var catalogs = (Dictionary<string, Dictionary<string, string>>)typeof(Catalog)
            .GetField("Messages", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        foreach (string locale in Locales)
        {
            Equal(new[] { "hold_repeat", "drop_empty" }, catalogs[locale].Keys, $"{locale} keys");
            string empty = Catalog.Template(locale, "drop_empty");
            Equal(new[] { "{0}" }, Positional.Matches(empty).Select(x => x.Value), $"{locale} empty-container placeholder");
            Check(Catalog.Format(locale, "drop_empty", _ => "unused", "G").Contains("G"), $"{locale} empty-container binding");
            string template = Catalog.Template(locale, "hold_repeat");
            Equal(new[] { "{0}", "{1}" }, Positional.Matches(template).Select(x => x.Value).Order(), $"{locale} placeholders");
            Check(!string.IsNullOrWhiteSpace(Catalog.Format(locale, "hold_repeat", _ => "unused", "Q", "Drop")), $"{locale} formatting");
        }
        Check(Catalog.Normalize("zh-HK") == "zh-Hant" && Catalog.Normalize("de-DE") == "de" && Catalog.Normalize("unknown") == "en", "normalization");

        LocalizationSettings.StringDatabase.OnGetTable = (_, locale) => Completed(new StringTable(
            new Dictionary<string, string> { ["drop"] = locale.Identifier.Code + " drop", ["throw"] = locale.Identifier.Code + " throw" }));
        LocalizationSettings.SelectedLocaleAsync = Locale("de");
        Check(GameText.Native("drop") == "de drop" && GameText.Native("throw") == "de throw", "supported native terms");
        LocalizationSettings.SelectedLocaleAsync = Locale("ja");
        Check(GameText.Native("drop") == "ja drop", "locale cache invalidation");
        Console.WriteLine("All Stack All localization tests passed.");
    }

    private static void Check(bool condition, string context)
    {
        if (!condition) throw new InvalidOperationException(context);
    }
    private static void Equal(IEnumerable<string> expected, IEnumerable<string> actual, string context)
    {
        if (!expected.SequenceEqual(actual, StringComparer.Ordinal))
            throw new InvalidOperationException(context + ": expected [" + string.Join(", ", expected) + "] got [" + string.Join(", ", actual) + "]");
    }
}
