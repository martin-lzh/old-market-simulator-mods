# Old Market Material Cost

Current source version: **0.5.2**, authorized as a patch for SDK/build/documentation changes; publication is pending. Download links below still refer to the published 0.5.1 release. New local builds use 0.5.2; gameplay behavior and SDK 2.1.6/r1 remain unchanged.

当前源码版本：**0.5.2**，本次已授权将 SDK、构建及文档改动推进一个 patch，尚待发布。下方下载链接仍指向已公开的 0.5.1；本地新构建使用 0.5.2，游戏逻辑及 SDK 2.1.6/r1 不变。


[Risk notes / 风险提示](#risk-notes--风险提示) · [Change Log / 版本记录](CHANGELOG.md)

Old Market Material Cost adds cost and profit estimates to three Old Market Simulator screens:

- The end-of-day sales report gets a **Materials only** switch. Enabled rows use estimated recipe-material costs and recomputed profit; disabling it restores the native cost and profit text.
- Recipe details show batch and unit material cost, output quantity, the current day's recommended price, estimated batch and unit profit, and profit divided by material cost.
- The dock order screen shows recommended unit price, wholesale case cost, estimated case profit, and profit divided by purchase cost for the selected product.

The Mod changes displayed text only. It does not change cash, prices, inventory, recipes, customers, saves, network messages, or game assemblies. It can estimate old inventory because it reconstructs cost from current recipe and price data rather than purchase history.

## Download

The latest published build is on the [Material Cost 0.5.1 release page](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.1). Pending 0.5.2 builds use these package names:

- `OldMarket.MaterialCost-0.5.2-BepInEx.zip`
- `OldMarket.MaterialCost-0.5.2-MelonLoader.zip`
- `SHA256SUMS.txt`

GitHub's automatically generated **Source code** archives are repository snapshots, not installable packages. Choose one loader package; never run BepInEx and MelonLoader together. The standalone bootstrap remains available as source for local investigation but is not publicly distributed; see [Standalone bootstrap](STANDALONE.md).

## Requirements and installation

Development and automated checks target Old Market Simulator 2.1.6, Windows x64, Unity Mono. Other game or loader versions may require a compatibility review.

### BepInEx 5

1. Exit the game and install BepInEx 5 x64 if needed, following the [official guide](https://docs.bepinex.dev/articles/user_guide/installation/index.html).
2. Extract `OldMarket.MaterialCost-0.5.2-BepInEx.zip` into the game directory. The plugin should be at `BepInEx/plugins/OldMarket.MaterialCost/OldMarket.MaterialCost.dll`.
3. Start the game and confirm the load message in the BepInEx log.

Settings are in `BepInEx/config/local.oldmarket.materialcost.cfg`. `[Report] MaterialsOnly` remembers the report switch and defaults to `false`. `[Diagnostics] Enabled` controls optional frame sampling, defaults to `false`, and requires a restart after editing.

### MelonLoader 0.7.3

1. Exit the game, disable the BepInEx `winhttp.dll` entry point, and install Windows x64 MelonLoader from its [0.7.3 release](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.3).
2. Extract `OldMarket.MaterialCost-0.5.2-MelonLoader.zip` into the game directory. The plugin should be at `Mods/OldMarket.MaterialCost.dll`.
3. Start the game and confirm the load message in `MelonLoader/Logs`.

Settings are in `UserData/OldMarket.MaterialCost.cfg`, category `OldMarketMaterialCost`. `MaterialsOnly` remembers the report switch; `Diagnostics` controls optional diagnostic sampling. Both default to `false`, and changing diagnostics requires a restart.

To uninstall either edition, exit the game and remove only its `OldMarket.MaterialCost.dll`. Existing saves need no conversion.

## Calculation scope

This is a standard production-material estimate, not historical acquisition accounting. One production rule applies to every unit of a product, so purchased and crafted stock cannot be distinguished.

- Honey, orchard output, repeatable animal output such as milk and eggs, and caught fish have zero material cost. Beehives, trees, reusable animals, fishing equipment, durability, labor, and other operating costs are outside this scope.
- Seed-crop cost is seed-pack price divided by seeds per pack and yield per plant.
- Meat includes the consumed animal's base price, allocated across the game's output cases and units.
- Processed-product unit cost is `sum(ingredient unit price × units per case × recipe cases) ÷ (output cases × units per case)`. Direct ingredients use the current day's wholesale price, including seasonal and event adjustments. Ingredient costs are not recursively expanded into their own production chains.
- Non-product inputs use base pack price divided by pack size. Seed and animal prices are resource base prices, not historical transactions or shop-specific quotes.

Recipe and order profit is `recommended-price revenue − estimated cost`. The percentage is `profit ÷ cost × 100`, using the game's cost denominator rather than revenue. Calculations retain full precision and display up to two decimals. Zero cost makes the percentage undefined, so it displays `—`. If a product has no recommended price, recipe cost remains visible while price and profit display `—`.

An unrecognized product, or one with multiple different-cost production rules, keeps the game's original cost and profit in the report instead of assuming zero. The report shows the number of fallback rows. Average selling price, quantity, cash transactions, rent, taxes, and maintenance remain native values.

## Languages

The UI follows the game's active locale in English, Simplified Chinese, Traditional Chinese, French, German, Italian, Spanish, Portuguese, Japanese, Korean, Russian, Turkish, and Ukrainian. Native terms such as **Recommended** and **Profit** come from the game's `Translations` table; Mod-specific text is embedded in each DLL. Unsupported locales fall back to English. No language pack is required.

See the Mod's [localization documentation](localization/README.md). Supplemental translations have not been reviewed by native speakers of every language; in-game glyphs, wrapping, and live switching still require runtime verification.

## Build and verification

Building requires Windows, .NET 8 SDK or newer, a local game installation, and loader references. From the repository root:

```powershell
./material-cost-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./material-cost-mod/build.ps1 -Loader MelonLoader -GameDir 'D:\Games\Old Market Simulator'
```

The scripts run checks, download pinned loader references when needed, verify their checksums, and write packages to `outputs/`. They read local game assemblies for compilation but neither package them nor modify the game. All localization sources required by this Mod live under `material-cost-mod/localization/`.

Version 0.5.2 compiles for BepInEx, MelonLoader, and the source-retained standalone target. Automated verification passes 40 accounting checks, 27 runtime cache and rule checks, and 11 localization suites. Coverage includes recipe and case conversion, current-day prices, losses, zero-cost and unknown-rule behavior, on-demand caching and invalidation, native-term lookup, locale fallback, and placeholder parity.

Automated checks are not in-game acceptance. Layout across resolutions, controller selection, all-language fonts and wrapping, day changes, multiplayer, and runtime performance have not been fully validated. Game updates and Mods that patch the same screens may require renewed testing.

## License and citation

Original source, tests, build scripts, and documentation use the [MIT License](LICENSE), Copyright (c) 2026 Zhaohan Liu. The game and third-party components retain their own licenses.

If this work helps a Mod, article, video, or research project, linking to [martin-lzh/old-market-simulator-mods](https://github.com/martin-lzh/old-market-simulator-mods) and naming the Mod and version is appreciated. Citation is voluntary and is not an additional MIT condition.

---

# Old Market Material Cost（中文）

Old Market Material Cost 在 Old Market Simulator 的三个界面中加入成本与利润估算：

- 每日结算销售表增加“仅计原料”开关。开启后按配方原料成本估算并重算利润；关闭后恢复游戏原有文字。
- 配方详情显示本批及每件原料成本、本批产量、当天建议售价、本批及每件预计利润，以及利润与原料成本之比。
- 码头订购页显示当前商品的建议单件售价、整箱进货成本、整箱预计利润，以及利润与进货成本之比。

这些估算只改变显示文字，不会修改金币、价格、库存、配方、顾客、存档、网络消息或游戏程序集。成本由当前配方和价格数据重建，因此也可估算旧库存，不依赖历史进货记录。

## 下载

当前已公开版本见 [Material Cost 0.5.1 发布页](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.1)。待发布的 0.5.2 构建使用以下包名：

- `OldMarket.MaterialCost-0.5.2-BepInEx.zip`
- `OldMarket.MaterialCost-0.5.2-MelonLoader.zip`
- `SHA256SUMS.txt`

GitHub 自动生成的 **Source code** 压缩包是源码快照，不是可安装包。请选择一种加载器，绝不能同时运行 BepInEx 和 MelonLoader。独立启动入口只保留源码供本机调查，不公开分发，详情见[独立启动入口](STANDALONE.md)。

## 运行要求与安装

开发和自动化检查基于 Windows x64、Unity Mono 的 Old Market Simulator 2.1.6。其他游戏或加载器版本可能需要重新核查兼容性。

### BepInEx 5

1. 退出游戏。如有需要，按[官方说明](https://docs.bepinex.dev/articles/user_guide/installation/index.html)安装 BepInEx 5 x64。
2. 将 `OldMarket.MaterialCost-0.5.2-BepInEx.zip` 解压到游戏目录。插件路径应为 `BepInEx/plugins/OldMarket.MaterialCost/OldMarket.MaterialCost.dll`。
3. 启动游戏，在 BepInEx 日志中确认加载信息。

设置位于 `BepInEx/config/local.oldmarket.materialcost.cfg`。`[Report] MaterialsOnly` 记忆日报开关，默认 `false`；`[Diagnostics] Enabled` 控制可选帧采样，默认 `false`，修改后需重启。

### MelonLoader 0.7.3

1. 退出游戏，停用 BepInEx 的 `winhttp.dll` 入口，再从 [0.7.3 官方发布页](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.3)安装 Windows x64 MelonLoader。
2. 将 `OldMarket.MaterialCost-0.5.2-MelonLoader.zip` 解压到游戏目录。插件路径应为 `Mods/OldMarket.MaterialCost.dll`。
3. 启动游戏，在 `MelonLoader/Logs` 中确认加载信息。

设置位于 `UserData/OldMarket.MaterialCost.cfg`，类别是 `OldMarketMaterialCost`。`MaterialsOnly` 记忆日报开关；`Diagnostics` 控制可选诊断采样。两者默认均为 `false`，修改诊断设置后需重启。

卸载时请退出游戏，只删除相应目录中的 `OldMarket.MaterialCost.dll`。现有存档无需转换。

## 计算口径

本 Mod 估算标准生产原料成本，并非逐批历史取得成本。同种商品统一使用一条生产规则，因此不能区分买入库存和自产库存。

- 蜂蜜、果树产出、奶和蛋等可重复取得的动物产出，以及捕获鱼类，原料成本按零计算。蜂箱、树木、可重复使用的动物、捕捞设备、耐久、人工及其他经营成本不在此口径内。
- 种子作物成本为种子包价格除以包内种子数，再除以每株产量。
- 屠宰肉类计入被消耗动物的基础价格，并按游戏中的产出箱数和每箱件数分摊。
- 加工品单件成本为 `Σ（原料单件价 × 每箱件数 × 配方箱数）÷（产出箱数 × 每箱件数）`。直接原料使用当天批发价，包含季节和活动调价；不会递归展开原料自身的生产链。
- 非商品投入使用基础包价除以包内数量。种子和动物价格是资源基础价，不代表历史成交价或特定商店报价。

配方和订购页利润为“按建议价计算的收入 − 估算成本”。百分比为 `利润 ÷ 成本 × 100`，使用游戏的成本分母，并非销售收入。计算保留完整精度，显示最多两位小数。成本为零时百分比无定义，显示“—”。商品没有建议售价时，仍显示配方成本，售价和利润显示“—”。

如果商品没有可识别的生产规则，或存在成本不同的多条规则，日报会保留游戏原有成本和利润，不会假定为零，并显示回退行数。平均售价、销量、现金收支、租金、税费和维护费仍使用游戏原值。

## 语言

界面跟随游戏当前语言，支持英语、简体中文、繁体中文、法语、德语、意大利语、西班牙语、葡萄牙语、日语、韩语、俄语、土耳其语和乌克兰语。“建议”“利润”等游戏词条直接读取当前 `Translations` 表；Mod 专有文字内嵌在各 DLL 中。不支持的语言回退到英语，无需另装语言包。

详情见本 Mod 的[本地化说明](localization/README.md)。补充译文尚未全部经过对应语言母语者审校；游戏内字形、换行和实时切换仍需验证。

## 构建与验证

构建需要 Windows、.NET 8 SDK 或更新版本、本机游戏和加载器引用。在仓库根目录运行：

```powershell
./material-cost-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./material-cost-mod/build.ps1 -Loader MelonLoader -GameDir 'D:\Games\Old Market Simulator'
```

脚本会运行检查，在需要时下载固定版本的加载器引用并校验哈希，然后将安装包写入 `outputs/`。脚本只读取本机游戏程序集用于编译，不会打包或修改它们。本 Mod 所需的全部本地化源码均位于 `material-cost-mod/localization/`。

0.5.2 已为 BepInEx、MelonLoader 和保留源码的独立目标完成编译。自动化验证通过 40 项会计检查、27 项运行时缓存与规则检查，以及 11 组本地化测试，覆盖配方与箱件换算、当天价格、亏损、零成本、未知规则、按需缓存与失效、原生词条、语言回退和占位符一致性。

自动化检查不等同于游戏内验收。不同分辨率的布局、手柄选择、所有语言的字体与换行、换日、多人和运行时性能尚未完整验证。游戏更新或其他修改相同界面的 Mod 可能需要重新测试。

## 许可证与引用

本目录原创源码、测试、构建脚本和文档采用 [MIT License](LICENSE)，Copyright (c) 2026 Zhaohan Liu。游戏和第三方组件继续适用各自许可证。

如果本项目帮助了其他 Mod、文章、视频或研究，欢迎链接到 [martin-lzh/old-market-simulator-mods](https://github.com/martin-lzh/old-market-simulator-mods)，并注明 Mod 名称和版本。引用完全自愿，不是 MIT 许可证的附加条件。

## Risk notes / 风险提示

### English

- Saves: UI-only patches and local toggle configuration; no new game-save fields or cash/item-cost changes. Old-inventory estimates are not historical purchase costs.

- Multiplayer: no custom protocol or connection changes; only viewers need this Mod by design. Actual host/client tests are incomplete; loader/Mod combinations may prevent joining in either direction.

- Performance/conflicts: first-use resource discovery and UI add overhead. Local loader comparisons have shown stuttering even without plugins; no variant is proven to fix it. Use one variant/copy. Other report/recipe/order UI Mods may conflict or overlap.

- Estimates exclude equipment/other expenses; profits assume all goods sell at today's recommended price. Other Mods changing resources within a day may leave caches stale. Turn off the report toggle to compare (recipe/order UI stays enabled), or exit before uninstalling.

### 中文

- 存档：仅 UI 补丁及本地开关配置，不新增存档字段或修改现金/物品成本；旧库存估算不是历史采购成本。

- 联机：不新增协议或修改连接流程，设计上仅查看者需要安装；房主/客人实测未完成，加载器/Mod 组合可能导致双方无法加入。

- 性能/冲突：首次资源查询和 UI 增加开销，零插件加载器对照也曾卡顿，未证明任何变体已修复。只启用一种变体/一个副本，其他日报/配方/订购 UI Mod 可能冲突遮挡。

- 估算排除设备等费用，利润假设全部按当天建议价售出；其他 Mod 同日改资源可能使缓存过时。关闭日报开关可对照（配方/订购仍启用），或退出后卸载。

## SDK and automated builds / SDK 与自动构建

This Mod pins its compilation SDK in [release.json](release.json). Current baseline: game 2.1.6 / SDK r1. Run `python tools/ci.py build` from the repository root for a game-free build. See [SDK maintenance](../sdk/README.md) and [CI releases](../releases/README.md). Compilation does not replace in-game compatibility checks.

本 Mod 在 [release.json](release.json) 固定编译 SDK，当前基线为游戏 2.1.6 / SDK r1。从仓库根目录运行 `python tools/ci.py build` 可无游戏文件构建。见 [SDK 维护](../sdk/README.md)和 [CI 发布](../releases/README.md)；编译不能代替实机兼容验证。
