# Stack All localization

This directory is the complete localization component for Stack All. It is intentionally private to this Mod so the `stack-all-mod/` directory can be copied and built without any repository-level localization project.

`messages.txt` contains only Stack All's original hold-to-repeat template in the 13 locales configured by Old Market Simulator 2.1.6: English, Simplified Chinese, Traditional Chinese, French, German, Italian, Spanish, Portuguese, Japanese, Korean, Russian, Turkish, and Ukrainian.

The game-owned actions `drop` and `throw` are read at runtime from the active `Translations` table and passed into the template. `GameText.cs` provides English fallbacks while that table is unavailable and reloads the terms after a locale change. The Mod message resource and helper classes are embedded directly in `OldMarket.StackAll.dll`; installation requires no language files or shared runtime DLL.

Run the standalone .NET 8 checks from `stack-all-mod/`:

```powershell
dotnet run --project localization/tests/Localization.Tests.csproj -c Release
```

The checks validate the exact locale and message sets, positional placeholders, formatting, locale normalization, and runtime lookup of the two supported native action terms. They do not impose native-term placeholder tests on the Mod template because it contains only positional placeholders. Tests use local API substitutes and do not load game files.

Automated checks cannot verify Unity layout, glyph coverage, line wrapping, input-binding labels, or live language switching. Those remain in-game validation items. Supplemental translations have not been reviewed by native speakers of every language.

## License

Copyright © 2026 Zhaohan Liu. The original files in this directory are available under the [MIT License](LICENSE). Old Market Simulator translations and other game assets are read at runtime and are not included here.

## 中文

此目录包含 Stack All 的完整本地化组件，并且只属于本 Mod。单独复制 `stack-all-mod/` 后即可构建，不依赖仓库根目录的共用本地化项目。

`messages.txt` 只保存 Stack All 自有的长按重复提示模板，覆盖 Old Market Simulator 2.1.6 配置的 13 种语言。游戏已有的 `drop` 和 `throw` 动作词会在运行时从当前 `Translations` 表读取，再作为参数填入模板；语言表尚不可用时使用英文回退，切换语言后会重新加载。

资源和辅助源码直接嵌入 `OldMarket.StackAll.dll`，安装时不需要额外语言文件或共享 DLL。在 `stack-all-mod/` 运行上方命令可执行独立 .NET 8 检查。Stack 模板只有位置参数，因此测试不会添加与资源无关的原生词占位符检查。测试不读取游戏文件；Unity 中的布局、字形、换行、键位文字和实时切换仍需实机验证。补充译文未经所有语言的母语者审校。

本目录原创文件版权归 Zhaohan Liu 所有，并采用 [MIT License](LICENSE)。游戏原生翻译及其他游戏资源只在运行时读取，不包含在本目录中。
