# Development notes / 开发说明

Player instructions: [README](README.md). Repository workflow: [CONTRIBUTING](../CONTRIBUTING.md), [SDK](../sdk/README.md), [releases](../releases/README.md). Current test scope: [validation record](../releases/validation.md).

玩家用法见 [README](README.md)。本文保留构建、接口与测试资料；当前实机范围以[验证记录](../releases/validation.md)为准。

## Calculation scope

This is a standard production-material estimate, not historical acquisition accounting. One production rule applies to every unit of a product, so purchased and crafted stock cannot be distinguished.

- Honey, orchard output, repeatable animal output such as milk and eggs, and caught fish have zero material cost. Beehives, trees, reusable animals, fishing equipment, durability, labor, and other operating costs are outside this scope.
- Seed-crop cost is seed-pack price divided by seeds per pack and yield per plant.
- Meat includes the consumed animal's base price, allocated across the game's output cases and units.
- Processed-product unit cost is `sum(ingredient unit price × units per case × recipe cases) ÷ (output cases × units per case)`. Direct ingredients use the current day's wholesale price, including seasonal and event adjustments. Ingredient costs are not recursively expanded into their own production chains.
- Non-product inputs use base pack price divided by pack size. Seed and animal prices are resource base prices, not historical transactions or shop-specific quotes.

Recipe and order profit is `recommended-price revenue − estimated cost`. The percentage is `profit ÷ cost × 100`, using the game's cost denominator rather than revenue. Calculations retain full precision and display up to two decimals. Zero cost makes the percentage undefined, so it displays `—`. If a product has no recommended price, recipe cost remains visible while price and profit display `—`.

An unrecognized product, or one with multiple different-cost production rules, keeps the game's original cost and profit in the report instead of assuming zero. The report shows the number of fallback rows. Average selling price, quantity, cash transactions, rent, taxes, and maintenance remain native values.

## Build and verification

Building requires Windows, .NET 8 SDK or newer, a local game installation, and loader references. From the repository root:

```powershell
./material-cost-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./material-cost-mod/build.ps1 -Loader MelonLoader -GameDir 'D:\Games\Old Market Simulator'
```

The scripts run checks, download pinned loader references when needed, verify their checksums, and write packages to `outputs/`. They read local game assemblies for compilation but neither package them nor modify the game. All localization sources required by this Mod live under `material-cost-mod/localization/`.

Version 0.5.2 compiles for BepInEx, MelonLoader, and the source-retained standalone target. Automated verification passes 40 accounting checks, 27 runtime cache and rule checks, and 11 localization suites. Coverage includes recipe and case conversion, current-day prices, losses, zero-cost and unknown-rule behavior, on-demand caching and invalidation, native-term lookup, locale fallback, and placeholder parity.

Automated checks are not in-game acceptance. Layout across resolutions, controller selection, all-language fonts and wrapping, day changes, multiplayer, and runtime performance have not been fully validated. Game updates and Mods that patch the same screens may require renewed testing.

## 计算口径

本 Mod 估算标准生产原料成本，并非逐批历史取得成本。同种商品统一使用一条生产规则，因此不能区分买入库存和自产库存。

- 蜂蜜、果树产出、奶和蛋等可重复取得的动物产出，以及捕获鱼类，原料成本按零计算。蜂箱、树木、可重复使用的动物、捕捞设备、耐久、人工及其他经营成本不在此口径内。
- 种子作物成本为种子包价格除以包内种子数，再除以每株产量。
- 屠宰肉类计入被消耗动物的基础价格，并按游戏中的产出箱数和每箱件数分摊。
- 加工品单件成本为 `Σ（原料单件价 × 每箱件数 × 配方箱数）÷（产出箱数 × 每箱件数）`。直接原料使用当天批发价，包含季节和活动调价；不会递归展开原料自身的生产链。
- 非商品投入使用基础包价除以包内数量。种子和动物价格是资源基础价，不代表历史成交价或特定商店报价。

配方和订购页利润为“按建议价计算的收入 − 估算成本”。百分比为 `利润 ÷ 成本 × 100`，使用游戏的成本分母，并非销售收入。计算保留完整精度，显示最多两位小数。成本为零时百分比无定义，显示“—”。商品没有建议售价时，仍显示配方成本，售价和利润显示“—”。

如果商品没有可识别的生产规则，或存在成本不同的多条规则，日报会保留游戏原有成本和利润，不会假定为零，并显示回退行数。平均售价、销量、现金收支、租金、税费和维护费仍使用游戏原值。

## 构建与验证

构建需要 Windows、.NET 8 SDK 或更新版本、本机游戏和加载器引用。在仓库根目录运行：

```powershell
./material-cost-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./material-cost-mod/build.ps1 -Loader MelonLoader -GameDir 'D:\Games\Old Market Simulator'
```

脚本会运行检查，在需要时下载固定版本的加载器引用并校验哈希，然后将安装包写入 `outputs/`。脚本只读取本机游戏程序集用于编译，不会打包或修改它们。本 Mod 所需的全部本地化源码均位于 `material-cost-mod/localization/`。

0.5.2 已为 BepInEx、MelonLoader 和保留源码的独立目标完成编译。自动化验证通过 40 项会计检查、27 项运行时缓存与规则检查，以及 11 组本地化测试，覆盖配方与箱件换算、当天价格、亏损、零成本、未知规则、按需缓存与失效、原生词条、语言回退和占位符一致性。

自动化检查不等同于游戏内验收。不同分辨率的布局、手柄选择、所有语言的字体与换行、换日、多人和运行时性能尚未完整验证。游戏更新或其他修改相同界面的 Mod 可能需要重新测试。

## SDK and automated builds / SDK 与自动构建

This Mod pins its compilation SDK in [release.json](release.json). Current baseline: game 2.1.6 / SDK r1. Run `python tools/ci.py build` from the repository root for a game-free build. See [SDK maintenance](../sdk/README.md) and [CI releases](../releases/README.md). Compilation does not replace in-game compatibility checks.

本 Mod 在 [release.json](release.json) 固定编译 SDK，当前基线为游戏 2.1.6 / SDK r1。从仓库根目录运行 `python tools/ci.py build` 可无游戏文件构建。见 [SDK 维护](../sdk/README.md)和 [CI 发布](../releases/README.md)；编译不能代替实机兼容验证。
