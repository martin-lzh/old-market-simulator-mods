using System.Reflection;
using System.Text.RegularExpressions;
using OldMarket.CheckoutAll.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using static Localization.Tests.FakeOperations;

namespace CheckoutLocalization.Tests;

internal static class Program
{
    private static readonly string[] Locales =
        ["en", "zh", "zh-Hant", "fr", "de", "it", "es", "pt", "ja", "ko", "ru", "tr", "uk"];
    private static readonly string[] Keys = ["checkout_hold", "checkout_toggle", "hold_repeat"];
    private static readonly Regex Placeholder = new(@"\{(?:#([a-z_]+)|(\d+))\}", RegexOptions.Compiled);

    private static void Main()
    {
        Equal(Locales.Order(), Catalog.Languages.Order(), "locales");
        Equal(Keys, Catalog.Keys.Order(), "keys");
        var catalogs = (Dictionary<string, Dictionary<string, string>>)typeof(Catalog)
            .GetField("Messages", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        foreach (string locale in Locales)
        {
            Equal(Keys, catalogs[locale].Keys.Order(), $"{locale} keys");
            foreach (string key in Keys)
            {
                Equal(Signature(Catalog.Template("en", key)), Signature(Catalog.Template(locale, key)), $"{locale}.{key} placeholders");
                string formatted = Catalog.Format(locale, key, term => "native:" + term, 1, 2, 3);
                Check(!string.IsNullOrWhiteSpace(formatted) && !Placeholder.IsMatch(formatted), $"{locale}.{key} formatting");
            }
        }
        Check(Catalog.Normalize("zh-TW") == "zh-Hant" && Catalog.Normalize("pt-BR") == "pt" && Catalog.Normalize("unknown") == "en", "normalization");

        var pending = Pending<StringTable>();
        LocalizationSettings.StringDatabase.OnGetTable = (name, _) =>
        {
            Check(name == "Translations", "native table name");
            return pending.Handle;
        };
        LocalizationSettings.SelectedLocaleAsync = Locale("fr");
        Check(GameText.Native("put_to_bag") == "Put in bag", "pending fallback");
        pending.Succeed(new StringTable(new Dictionary<string, string>
        {
            ["put_to_bag"] = "  Mettre {dans} le sac  ", ["take_pouch"] = "Prendre la bourse",
            ["turn_on"] = "Activer", ["turn_off"] = "Désactiver"
        }));
        Check(GameText.Native("put_to_bag") == "Mettre {dans} le sac", "loaded native term");
        Check(GameText.Native("take_pouch") == "Prendre la bourse", "supported native term");
        Check(GameText.Native("turn_on") == "Activer" && GameText.Native("turn_off") == "Désactiver", "toggle native terms");
        string withNativeBraces = Catalog.Format("fr", "checkout_toggle", GameText.Native, "F9");
        Check(withNativeBraces.Contains("Désactiver"), "native placeholder formatting");
        Console.WriteLine("All Checkout All localization tests passed.");
    }

    private static string[] Signature(string value) => Placeholder.Matches(value).Select(x => x.Value).Order().ToArray();
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
