# Material Cost localization

Old Market Material Cost maintains its localization sources in this directory. Each BepInEx, MelonLoader, or standalone build embeds `messages.txt` into its own DLL; installation does not require a language pack or shared localization assembly.

The catalog contains the 14 Mod-specific messages used by the daily report, recipe details, and dock order details in 13 languages: English, Simplified Chinese, Traditional Chinese, French, German, Italian, Spanish, Portuguese, Japanese, Korean, Russian, Turkish, and Ukrainian. Locale selection follows the game's active Unity Localization locale, including normalization of regional locale codes and an English fallback for unsupported languages.

The four native terms used by this Mod—`amount`, `avg_cost`, `profit`, and `recommended`—are read at runtime from the game's `Translations` table. A safe English fallback is used while the table is unavailable. The lookup is asynchronous and refreshes after the selected locale changes. Mod config identifiers and developer logs remain stable.

Run the independent .NET 8 test project from this directory:

```powershell
dotnet run --project tests/Localization.Tests.csproj
```

Its 11 suites verify the exact locale set, key and placeholder parity, formatting, regional normalization, English fallback, asynchronous native-table loading, failure recovery, and cache invalidation after a language change. Test doubles do not load or modify game files.

Automated checks cannot verify Unity layout, font glyph coverage, wrapping, or whether every label fits at each resolution and UI scale. The supplemental translations have not been reviewed by native speakers of every supported language.

These original localization sources and translations use the [MIT License](LICENSE), Copyright (c) 2026 Zhaohan Liu. The game's native string tables are neither copied nor redistributed.

---

# Material Cost 本地化（中文）

Old Market Material Cost 在本目录独立维护本地化源码。BepInEx、MelonLoader 和独立目标构建时都会把 `messages.txt` 嵌入各自 DLL；安装时不需要语言包或共用本地化程序集。

语言表包含日报、配方详情和码头订购详情实际使用的 14 条 Mod 专有文字，覆盖英语、简体中文、繁体中文、法语、德语、意大利语、西班牙语、葡萄牙语、日语、韩语、俄语、土耳其语和乌克兰语。语言选择跟随游戏当前 Unity Localization 区域设置，会规范化地区代码；不支持的语言回退到英语。

本 Mod 使用的四个原生词条 `amount`、`avg_cost`、`profit` 和 `recommended` 在运行时从游戏 `Translations` 表读取。语言表不可用时采用安全的英语回退；读取过程不会阻塞，切换语言后会重新加载。Mod 配置标识和开发日志不会随语言变化。

在本目录运行独立的 .NET 8 测试项目：

```powershell
dotnet run --project tests/Localization.Tests.csproj
```

11 组测试核对准确的语言集合、词条与占位符一致性、格式化、地区代码规范化、英语回退、异步原生词表加载、失败恢复，以及切换语言后的缓存失效。测试使用替身，不读取或修改游戏文件。

自动化检查无法验证 Unity 布局、字体字形、换行，以及不同分辨率和 UI 缩放下的文字容纳情况。补充译文尚未全部经过对应语言母语者审校。

这些原创本地化源码和译文采用 [MIT License](LICENSE)，Copyright (c) 2026 Zhaohan Liu。游戏原生词表不会被复制或再分发。
