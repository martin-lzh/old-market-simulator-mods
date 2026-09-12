# Localization

The four Mods with language-dependent UI share these localization sources at build time. Each of their release DLLs embeds its own copy of `messages.txt`; installation does not require a shared localization runtime DLL. Coordinates uses universal X/Y/Z labels, follows the native HUD font and number formatting, and does not embed unused messages.

Original Mod messages are provided in 13 languages: English, Simplified Chinese, Traditional Chinese, French, German, Italian, Spanish, Portuguese, Japanese, Korean, Russian, Turkish, and Ukrainian. Locale selection follows the game's active Unity Localization locale, including regional locale normalization and an English fallback for unsupported languages.

Terms already owned by the game, such as action names and pricing labels, are resolved at runtime from the game's `Translations` string table. This keeps wording consistent with the active game language. If that native table is still loading or unavailable, the UI uses a safe English fallback; switching languages or reopening the game starts a fresh native-table load. `messages.txt` contains translations for original Mod messages only; it is not a copy of the game's native string tables.

Run the standalone .NET 8 checks from this directory:

```powershell
dotnet run --project tests/Localization.Tests.csproj
```

The checks validate locale coverage, key and placeholder parity, message formatting, fallback behavior, asynchronous native-table loading, and cache invalidation after a language change. They use small test doubles and do not load or modify game files.

Automated checks cannot verify Unity layout, font glyph coverage, line wrapping, or whether every label fits each Mod's runtime UI. Before release, open each affected screen in the game, switch through all supported languages, and verify the visible text at the intended resolution and UI scale.

## Native term verification

The installed Old Market Simulator 2.1.6 resources were checked read-only on 2026-09-12. All 13 native `Translations` tables contain nonempty entries for these 11 referenced keys:

`amount`, `avg_cost`, `profit`, `recommended`, `wholesale`, `drop`, `throw`, `put_to_bag`, `take_pouch`, `turn_on`, `turn_off`.

The locale list was checked against `localization-locales_assets_all.bundle`; string-table bundles were checked separately. Extracted tables and game assets remain local and are not published. The native `stop` term belongs to the follow/stop-following interaction, so the continuous-checkout stop message uses an original translation instead.

Mod config identifiers and developer logs remain stable; localization applies to in-game UI. The supplemental translations have not been reviewed by native speakers of every supported language.
