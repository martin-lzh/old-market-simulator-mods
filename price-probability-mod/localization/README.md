# Price Probability localization

This is the Mod's independent localization source. Its release DLL embeds `messages.txt`; no shared runtime DLL or sibling localization project is required.

Seven original UI messages are supplied in 13 languages: English, Simplified Chinese, Traditional Chinese, French, German, Italian, Spanish, Portuguese, Japanese, Korean, Russian, Turkish, and Ukrainian. Native `recommended`, `wholesale`, and `turn_off` terms load asynchronously from the game's `Translations` table, with safe English fallback while it is loading or unavailable.

Run the standalone checks with the .NET 8 SDK:

```powershell
dotnet run --project localization/tests/Localization.Tests.csproj
```

The tests use small fakes and do not access game files. They cover messages, placeholders, formatting, locale normalization, startup loading, failure fallback, cache invalidation, and reselection recovery. Unity layout, glyphs, live UI switching, and multiplayer still require manual testing.
