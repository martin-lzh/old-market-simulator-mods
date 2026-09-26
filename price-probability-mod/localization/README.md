# Price Probability localization

This is the Mod's independent localization source. Its release DLL embeds `messages.txt`; no shared runtime DLL or sibling localization project is required.

Seven original UI messages are supplied in 13 languages: English, Simplified Chinese, Traditional Chinese, French, German, Italian, Spanish, Portuguese, Japanese, Korean, Russian, Turkish, and Ukrainian. Native `recommended`, `wholesale`, and `turn_off` terms load asynchronously from the game's `Translations` table, with safe English fallback while it is loading or unavailable.

Run the standalone checks from `price-probability-mod/` with the .NET 8 SDK:

```powershell
dotnet run --project localization/tests/Localization.Tests.csproj
```

The tests use small fakes and do not access game files. They cover messages, placeholders, formatting, locale normalization, startup loading, failure fallback, cache invalidation, and reselection recovery. Unity layout, glyphs, live UI switching, and multiplayer still require manual testing.

Original localization files use the [MIT License](LICENSE). Game translation tables are read at runtime and are not bundled.

## 中文

此目录是 Smart Pricing 独立的本地化组件，发行 DLL 内嵌 `messages.txt`，无需共享运行时 DLL 或其他 Mod 的本地化项目。七条原创界面消息覆盖游戏 2.1.6 的 13 种语言：英语、简体中文、繁体中文、法语、德语、意大利语、西班牙语、葡萄牙语、日语、韩语、俄语、土耳其语及乌克兰语。

原生 `recommended`、`wholesale` 和 `turn_off` 词条从游戏的 `Translations` 表异步读取，尚未加载或不可用时回退英语。在 `price-probability-mod/` 目录使用 .NET 8 SDK 运行上方命令即可执行独立检查。

测试不读取游戏文件，覆盖消息、占位符、格式化、语言归一化、启动加载、失败回退、缓存失效和重选恢复；Unity 布局、字形、实时语言切换及联机仍需人工测试。原创本地化文件采用 [MIT License](LICENSE)，游戏翻译表仅在运行时读取，不随 Mod 分发。
