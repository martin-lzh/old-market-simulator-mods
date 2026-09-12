using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OldMarket.StackAll.Localization
{
    internal static class GameText
    {
        private static Locale locale;
        private static AsyncOperationHandle<StringTable> operation;
        private static readonly Dictionary<string, string> NativeCache = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> Fallback = new Dictionary<string, string>
        {
            ["drop"] = "Drop", ["throw"] = "Throw"
        };
        internal static string Code
        {
            get
            {
                var selection = LocalizationSettings.SelectedLocaleAsync;
                var current = selection.IsDone ? selection.Result : null;
                if (current != locale)
                {
                    locale = current;
                    NativeCache.Clear();
                    operation = locale == null ? default : LocalizationSettings.StringDatabase.GetTableAsync("Translations", locale);
                }
                return locale?.Identifier.Code ?? "en";
            }
        }
        // Database owns the table handle. Never block the main thread or release its shared handle.
        internal static string Stamp
        {
            get
            {
                string code = Code;
                return code + (!operation.IsValid() || !operation.IsDone ? ":pending" :
                    operation.Status == AsyncOperationStatus.Succeeded ? ":ready" : ":failed");
            }
        }
        internal static string Native(string key)
        {
            _ = Code;
            if (NativeCache.TryGetValue(key, out var value)) return value;
            if (operation.IsValid() && operation.IsDone && operation.Status == AsyncOperationStatus.Succeeded)
            {
                value = operation.Result?.GetEntry(key)?.GetLocalizedString();
                if (!string.IsNullOrWhiteSpace(value)) return NativeCache[key] = value.Trim();
            }
            // Do not cache a fallback while the selected table is still loading.
            return Fallback.TryGetValue(key, out value) ? value : key;
        }
        internal static string Get(string key, params object[] args) => Catalog.Format(Code, key, Native, args);
    }
}
