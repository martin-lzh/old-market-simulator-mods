using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OldMarket.Navigation
{
    internal static class NativePoiText
    {
        private static Locale locale;
        private static AsyncOperationHandle<StringTable> operation;
        private static readonly Dictionary<string,string> cache=new Dictionary<string,string>();

        // The localization database owns this handle: never wait synchronously or release it.
        internal static string Get(string requestedLocale,string key)
        {
            var selection=LocalizationSettings.SelectedLocaleAsync;
            var current=selection.IsDone?selection.Result:null;
            if(current!=locale)
            {
                locale=current;
                cache.Clear();
                operation=locale==null?default:LocalizationSettings.StringDatabase.GetTableAsync("Translations",locale);
            }
            if(locale==null || !string.Equals(locale.Identifier.Code.Replace('_','-'),
                (requestedLocale??"").Replace('_','-'),StringComparison.OrdinalIgnoreCase)) return "";
            if(cache.TryGetValue(key,out var value))return value;
            if(operation.IsValid() && operation.IsDone && operation.Status==AsyncOperationStatus.Succeeded)
            {
                value=operation.Result?.GetEntry(key)?.GetLocalizedString();
                if(!string.IsNullOrWhiteSpace(value))return cache[key]=value;
            }
            // Pending or absent entries leave the semantic icon visible without fabricated text.
            return "";
        }
    }
}
