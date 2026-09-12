using System.Text.RegularExpressions;
using System.Reflection;
using OldMarket.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using static Localization.Tests.FakeOperations;

namespace Localization.Tests;

internal static class Program
{
    private static readonly string[] ExpectedLocales =
        ["en", "zh", "zh-Hant", "fr", "de", "it", "es", "pt", "ja", "ko", "ru", "tr", "uk"];
    private static readonly Regex Positional = new(@"(?<!\{)\{(\d+)(?:[^}]*)\}(?!\})", RegexOptions.Compiled);
    private static readonly Regex Native = new(@"\{#([a-z_]+)\}", RegexOptions.Compiled);
    private static int failures;

    private static int Main()
    {
        Run("catalog has exactly the supported locales", SupportedLocales);
        Run("every locale has the same complete message set", CompleteMessageSets);
        Run("placeholder signatures match English", PlaceholderSignatures);
        Run("all messages format successfully", EveryMessageFormats);
        Run("locale normalization and English fallback", LocaleNormalization);
        Run("native text containing braces is preserved", NativeBracesAreLiteral);
        Run("pending locale selection resolves after startup", PendingLocaleSelectionResolves);
        Run("pending native table fallback is not cached", PendingFallbackIsNotCached);
        Run("failed native table stays on safe fallback", FailedTableUsesFallback);
        Run("switching away and back reloads a failed table", LocaleReselectionRecovers);
        Run("switching locales invalidates native text cache", LocaleSwitchInvalidatesCache);

        Console.WriteLine(failures == 0 ? "All localization tests passed." : $"{failures} localization test(s) failed.");
        return failures == 0 ? 0 : 1;
    }

    private static void Run(string name, Action test)
    {
        try { test(); Console.WriteLine($"PASS {name}"); }
        catch (Exception error) { failures++; Console.Error.WriteLine($"FAIL {name}: {error.Message}"); }
    }

    private static void SupportedLocales() =>
        EqualSequence(ExpectedLocales.Order(), Catalog.Languages.Order(), "locale set");

    private static void CompleteMessageSets()
    {
        var catalogs = (Dictionary<string, Dictionary<string, string>>)typeof(Catalog)
            .GetField("Messages", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        var keys = catalogs["en"].Keys.Order().ToArray();
        True(keys.Length > 0, "English catalog is empty");
        foreach (var locale in ExpectedLocales)
        {
            EqualSequence(keys, catalogs[locale].Keys.Order(), $"keys for {locale}");
            foreach (var (key, value) in catalogs[locale])
                True(!string.IsNullOrWhiteSpace(value), $"empty value for {locale}.{key}");
        }
    }

    private static void PlaceholderSignatures()
    {
        foreach (var key in Catalog.Keys)
        {
            var expected = Signature(Catalog.Template("en", key));
            foreach (var locale in ExpectedLocales)
                EqualSequence(expected, Signature(Catalog.Template(locale, key)), $"placeholder signature for {locale}.{key}");
        }
    }

    private static string[] Signature(string template) =>
        Positional.Matches(template).Select(x => "{" + x.Groups[1].Value + "}")
            .Concat(Native.Matches(template).Select(x => "{#" + x.Groups[1].Value + "}"))
            .Order().ToArray();

    private static void EveryMessageFormats()
    {
        foreach (var locale in ExpectedLocales)
        foreach (var key in Catalog.Keys)
        {
            var template = Catalog.Template(locale, key);
            var positions = Positional.Matches(template).Select(x => int.Parse(x.Groups[1].Value));
            int count = positions.Any() ? positions.Max() + 1 : 0;
            var args = Enumerable.Range(0, count).Select(index => (object)(index + 1)).ToArray();
            var formatted = Catalog.Format(locale, key, term => $"native:{term}", args);
            True(!string.IsNullOrWhiteSpace(formatted), $"formatted result empty for {locale}.{key}");
            True(!Native.IsMatch(formatted), $"native placeholder remained in {locale}.{key}");
        }
    }

    private static void LocaleNormalization()
    {
        Equal("en", Catalog.Normalize("xx-ZZ"), "unknown locale");
        Equal("en", Catalog.Normalize(null), "null locale");
        Equal("pt", Catalog.Normalize("pt-BR"), "Portuguese regional locale");
        Equal("de", Catalog.Normalize("de_DE"), "underscore regional locale");
        Equal("zh", Catalog.Normalize("zh-CN"), "Simplified Chinese locale");
        foreach (var locale in new[] { "zh-Hant", "zh-Hant-HK", "zh-TW", "zh-HK", "zh-MO" })
            Equal("zh-Hant", Catalog.Normalize(locale), locale);
        foreach (var key in Catalog.Keys)
            Equal(Catalog.Template("en", key), Catalog.Template("unknown", key), $"English fallback for {key}");
    }

    private static void NativeBracesAreLiteral()
    {
        var candidate = Catalog.Keys.FirstOrDefault(key => Native.IsMatch(Catalog.Template("en", key)));
        True(candidate != null, "catalog has no native game-term placeholder");
        const string native = "Game {translation} and {{braces}}";
        string result = Catalog.Format("en", candidate, _ => native, 1, 2, 3, 4, 5, 6, 7, 8);
        True(result.Contains(native, StringComparison.Ordinal), "native braces were altered or reinterpreted");
    }

    private static void PendingFallbackIsNotCached()
    {
        var table = Pending<StringTable>();
        LocalizationSettings.StringDatabase.OnGetTable = (name, _) =>
        {
            Equal("Translations", name, "native table name");
            return table.Handle;
        };
        LocalizationSettings.SelectedLocaleAsync = Locale("fr");
        Equal("Average cost", GameText.Native("avg_cost"), "fallback while loading");
        True(GameText.Stamp.EndsWith(":pending", StringComparison.Ordinal), "pending stamp");
        table.Succeed(new StringTable(new Dictionary<string, string> { ["avg_cost"] = "  Coût {natif}  " }));
        Equal("Coût {natif}", GameText.Native("avg_cost"), "loaded native value");
        Equal("fr:ready", GameText.Stamp, "ready stamp");
    }

    private static void PendingLocaleSelectionResolves()
    {
        var selection = Pending<Locale>();
        int requests = 0;
        LocalizationSettings.StringDatabase.OnGetTable = (_, locale) =>
        {
            requests++;
            return Completed(new StringTable(new Dictionary<string, string> { ["recommended"] = locale.Identifier.Code + " native" }));
        };
        LocalizationSettings.SelectedLocaleAsync = selection.Handle;
        Equal("en", GameText.Code, "startup locale while selection is pending");
        Equal("en:pending", GameText.Stamp, "startup pending stamp");
        Equal("Recommended", GameText.Native("recommended"), "startup fallback");
        Equal(0, requests, "table requests before locale selection completes");

        selection.Succeed(new Locale("uk"));
        Equal("uk", GameText.Code, "resolved startup locale");
        Equal("uk:ready", GameText.Stamp, "resolved startup stamp");
        Equal("uk native", GameText.Native("recommended"), "resolved startup native value");
        Equal(1, requests, "table requests after locale selection completes");
    }

    private static void FailedTableUsesFallback()
    {
        var failed = Pending<StringTable>();
        failed.Fail();
        int requests = 0;
        LocalizationSettings.StringDatabase.OnGetTable = (_, _) => { requests++; return failed.Handle; };
        LocalizationSettings.SelectedLocaleAsync = Locale("tr");

        Equal("tr:failed", GameText.Stamp, "failed table stamp");
        Equal("Profit", GameText.Native("profit"), "known-key fallback after failure");
        Equal("unknown_native_key", GameText.Native("unknown_native_key"), "unknown-key fallback after failure");
        Equal("tr:failed", GameText.Stamp, "failed table remains failed");
        Equal(1, requests, "failed table is not requested every frame");
    }

    private static void LocaleReselectionRecovers()
    {
        int frenchRequests = 0;
        LocalizationSettings.StringDatabase.OnGetTable = (_, locale) =>
        {
            if (locale.Identifier.Code == "fr")
            {
                frenchRequests++;
                if (frenchRequests == 1)
                {
                    var failure = Pending<StringTable>();
                    failure.Fail();
                    return failure.Handle;
                }
                return Completed(new StringTable(new Dictionary<string, string> { ["profit"] = "Bénéfice rétabli" }));
            }
            return Completed(new StringTable(new Dictionary<string, string> { ["profit"] = "Gewinn" }));
        };

        LocalizationSettings.SelectedLocaleAsync = Locale("fr");
        Equal("fr:failed", GameText.Stamp, "first French table request");
        Equal("Profit", GameText.Native("profit"), "fallback after first French failure");
        LocalizationSettings.SelectedLocaleAsync = Locale("de");
        Equal("Gewinn", GameText.Native("profit"), "intermediate locale native value");
        LocalizationSettings.SelectedLocaleAsync = Locale("fr");
        Equal("fr:ready", GameText.Stamp, "reselected French table stamp");
        Equal("Bénéfice rétabli", GameText.Native("profit"), "reselected French native value");
        Equal(2, frenchRequests, "French table request count after reselection");
    }

    private static void LocaleSwitchInvalidatesCache()
    {
        LocalizationSettings.StringDatabase.OnGetTable = (_, locale) => Completed(new StringTable(
            new Dictionary<string, string> { ["profit"] = locale.Identifier.Code == "de" ? "Gewinn" : "Bénéfice" }));
        LocalizationSettings.SelectedLocaleAsync = Locale("de");
        Equal("Gewinn", GameText.Native("profit"), "German native value");
        LocalizationSettings.SelectedLocaleAsync = Locale("fr");
        Equal("Bénéfice", GameText.Native("profit"), "French value after locale switch");
        Equal("missing_term", GameText.Native("missing_term"), "unknown native key fallback");
    }

    private static void True(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Equal<T>(T expected, T actual, string context)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"{context}: expected '{expected}', got '{actual}'");
    }

    private static void EqualSequence(IEnumerable<string> expected, IEnumerable<string> actual, string context)
    {
        var left = expected.ToArray();
        var right = actual.ToArray();
        if (!left.SequenceEqual(right, StringComparer.Ordinal))
            throw new InvalidOperationException($"{context}: expected [{string.Join(", ", left)}], got [{string.Join(", ", right)}]");
    }
}
