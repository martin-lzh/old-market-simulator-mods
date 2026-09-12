using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
    internal enum AsyncOperationStatus { None, Succeeded, Failed }

    internal sealed class OperationState<T>
    {
        internal bool Done;
        internal AsyncOperationStatus Status;
        internal T Result;
    }

    internal readonly struct AsyncOperationHandle<T>
    {
        private readonly OperationState<T> state;
        internal AsyncOperationHandle(OperationState<T> state) => this.state = state;
        internal bool IsDone => state?.Done == true;
        internal T Result => state == null ? default : state.Result;
        internal AsyncOperationStatus Status => state?.Status ?? AsyncOperationStatus.None;
        internal bool IsValid() => state != null;
    }
}

namespace UnityEngine.Localization
{
    internal sealed class LocaleIdentifier
    {
        internal LocaleIdentifier(string code) => Code = code;
        internal string Code { get; }
    }

    internal sealed class Locale
    {
        internal Locale(string code) => Identifier = new LocaleIdentifier(code);
        internal LocaleIdentifier Identifier { get; }
    }
}

namespace UnityEngine.Localization.Tables
{
    internal sealed class StringTableEntry
    {
        private readonly string value;
        internal StringTableEntry(string value) => this.value = value;
        internal string GetLocalizedString() => value;
    }

    internal sealed class StringTable
    {
        private readonly IReadOnlyDictionary<string, string> entries;
        internal StringTable(IReadOnlyDictionary<string, string> entries) => this.entries = entries;
        internal StringTableEntry GetEntry(string key) =>
            entries.TryGetValue(key, out var value) ? new StringTableEntry(value) : null;
    }
}

namespace UnityEngine.Localization.Settings
{
    using UnityEngine.Localization;

    internal sealed class FakeStringDatabase
    {
        internal Func<string, Locale, AsyncOperationHandle<StringTable>> OnGetTable { get; set; }
        internal AsyncOperationHandle<StringTable> GetTableAsync(string name, Locale locale) => OnGetTable(name, locale);
    }

    internal static class LocalizationSettings
    {
        internal static AsyncOperationHandle<Locale> SelectedLocaleAsync { get; set; }
        internal static FakeStringDatabase StringDatabase { get; } = new();
    }
}

namespace Localization.Tests
{
    using UnityEngine.Localization;

    internal static class FakeOperations
    {
        internal static (AsyncOperationHandle<T> Handle, Action<T> Succeed, Action Fail) Pending<T>()
        {
            var state = new OperationState<T>();
            return (new AsyncOperationHandle<T>(state),
                value => { state.Result = value; state.Status = AsyncOperationStatus.Succeeded; state.Done = true; },
                () => { state.Status = AsyncOperationStatus.Failed; state.Done = true; });
        }

        internal static AsyncOperationHandle<T> Completed<T>(T value)
        {
            var operation = Pending<T>();
            operation.Succeed(value);
            return operation.Handle;
        }

        internal static AsyncOperationHandle<Locale> Locale(string code) => Completed(new Locale(code));
    }
}
