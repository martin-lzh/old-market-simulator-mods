using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OldMarket.StackAll.Localization
{
    // Original Mod messages only. Native game translations are resolved at runtime.
    internal static class Catalog
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Messages = Load();
        private sealed class Prepared
        {
            internal string Text;
            internal readonly List<string> Terms = new List<string>();
        }
        private static readonly Dictionary<string, Prepared> PreparedMessages = new Dictionary<string, Prepared>();
        internal static IEnumerable<string> Languages => Messages.Keys;
        internal static IEnumerable<string> Keys => Messages["en"].Keys;
        internal static string Normalize(string code)
        {
            code = (code ?? "en").Replace('_', '-').ToLowerInvariant();
            if (code == "zh-hant" || code.StartsWith("zh-hant-") || code == "zh-tw" || code == "zh-hk" || code == "zh-mo") return "zh-Hant";
            if (code == "zh" || code.StartsWith("zh-")) return "zh";
            string language = code.Split('-')[0];
            return Messages.ContainsKey(language) ? language : "en";
        }
        internal static string Template(string locale, string key) => Messages[Normalize(locale)][key];
        internal static string Format(string locale, string key, Func<string, string> native, params object[] args)
        {
            // Format numeric placeholders first so braces in a game translation are never reinterpreted.
            string cacheKey = Normalize(locale) + "/" + key;
            if (!PreparedMessages.TryGetValue(cacheKey, out var prepared))
            {
                prepared = new Prepared();
                prepared.Text = Regex.Replace(Template(locale, key), @"\{#([a-z_]+)\}", match =>
                {
                    prepared.Terms.Add(match.Groups[1].Value);
                    return "\u001f" + (prepared.Terms.Count - 1) + "\u001f";
                });
                PreparedMessages.Add(cacheKey, prepared);
            }
            string formatted = string.Format(CultureInfo.CurrentCulture, prepared.Text, args);
            for (int i = 0; i < prepared.Terms.Count; i++)
                formatted = formatted.Replace("\u001f" + i + "\u001f", native(prepared.Terms[i]));
            return formatted;
        }
        private static Dictionary<string, Dictionary<string, string>> Load()
        {
            var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OldMarket.Messages"))
            using (var reader = new StreamReader(stream ?? throw new InvalidOperationException("Missing Mod messages.")))
            {
                Dictionary<string, string> section = null;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        section = new Dictionary<string, string>();
                        result.Add(line.Substring(1, line.Length - 2), section);
                    }
                    else
                    {
                        int split = line.IndexOf('=');
                        if (section == null || split <= 0) throw new InvalidDataException("Invalid Mod message.");
                        section.Add(line.Substring(0, split), line.Substring(split + 1).Replace(@"\n", "\n"));
                    }
                }
            }
            return result;
        }
    }
}
