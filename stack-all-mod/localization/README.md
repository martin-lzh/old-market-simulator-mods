# Stack All localization

This directory is the complete localization component for Stack All. It is intentionally private to this Mod so the `stack-all-mod/` directory can be copied and built without any repository-level localization project.

`messages.txt` contains Better Stacking's two original captions, `drop_empty` and `hold_repeat`, in the 13 locales configured by Old Market Simulator 2.1.6: English, Simplified Chinese, Traditional Chinese, French, German, Italian, Spanish, Portuguese, Japanese, Korean, Russian, Turkish, and Ukrainian.

The captions contain no key names or formatting placeholders. `ActionHint` displays G in a separate native keycap and keeps the game's own Q/F rows; the hold caption is a separate native-style row without a keycap. `GameText.cs` also retains runtime lookup and English fallbacks for the native `drop` and `throw` terms, but those terms are not inserted into the current captions. The Mod message resource and helper classes are embedded directly in `OldMarket.StackAll.dll`; installation requires no language files or shared runtime DLL.

Run the standalone .NET 8 checks from `stack-all-mod/`:

```powershell
dotnet run --project localization/tests/Localization.Tests.csproj -c Release
```

The checks validate the exact locale and message sets, the absence of positional key markup, nonempty captions, locale normalization, native-term lookup and cache invalidation. Tests use local API substitutes and do not load game files.

Automated checks cannot verify Unity layout, glyph coverage, line wrapping, input-binding labels, or live language switching. Those remain in-game validation items. Supplemental translations have not been reviewed by native speakers of every language.

## License

Copyright © 2026 Zhaohan Liu. The original files in this directory are available under the [MIT License](LICENSE). Old Market Simulator translations and other game assets are read at runtime and are not included here.

## 中文

此目录包含 Stack All 的完整本地化组件，并且只属于本 Mod。单独复制 `stack-all-mod/` 后即可构建，不依赖仓库根目录的共用本地化项目。

`messages.txt` 保存 Better Stacking 自有的 `drop_empty`（丢出空盒）和 `hold_repeat`（长按可连续操作）两条说明，覆盖 Old Market Simulator 2.1.6 配置的 13 种语言。

两条说明均不包含按键名称或格式占位符。`ActionHint` 将 G 显示在独立的原生键帽内，保留游戏原有 Q/F 行；长按说明单独显示为不带键帽的原生样式行。`GameText.cs` 仍保留原生 `drop` 和 `throw` 动作词的运行时读取及英文回退，但当前说明不再插入这些词条。

资源和辅助源码直接嵌入 `OldMarket.StackAll.dll`，安装时不需要额外语言文件或共享 DLL。在 `stack-all-mod/` 运行上方命令可执行独立 .NET 8 检查，覆盖语言和消息集合、无按键位置占位符、非空说明、语言代码归一化、原生词条读取和缓存失效。测试不读取游戏文件；Unity 中的布局、字形、换行、键位文字和实时切换仍需实机验证。补充译文未经所有语言的母语者审校。

本目录原创文件版权归 Zhaohan Liu 所有，并采用 [MIT License](LICENSE)。游戏原生翻译及其他游戏资源只在运行时读取，不包含在本目录中。
