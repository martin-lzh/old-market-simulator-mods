# Profit Insights

![Profit Insights](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/material-cost-mod/assets/cover.png?raw=1)

A mod for Old Market Simulator / Old Market Simulator 模组

[English](#english) · [中文](#中文)

## English

Get a clearer view of material costs and possible profit while making, ordering and selling goods.

[Download 0.5.3](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.3)

### Core features

- Compare ingredient costs and estimated profit in daily reports, recipe details and dock orders.
- Switch the daily report between material estimates and the game’s usual figures.
- See per-item and whole-batch costs, output quantities and suggested-price profit.

![In-game daily report showing material costs, average prices, estimated profits and the Materials only switch](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/material-cost-mod/assets/material-cost-report-gameplay.png?raw=1)

![In-game recipe details showing batch and unit material costs, recommended price and estimated profit](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/material-cost-mod/assets/recipe-cost-profit-gameplay.png?raw=1)

![In-game dock order showing purchase cost per pack, recommended unit price and estimated profit](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/material-cost-mod/assets/order-cost-profit-gameplay.png?raw=1)

### How to use

- **Daily sales report:** turn on **Materials only** to estimate costs from ingredients. Turn it off to return to the game's usual figures; your choice is remembered.
- **Recipe details:** see material cost, output quantity, today's suggested price and estimated profit for each item and the whole batch.
- **Dock orders:** see the suggested selling price, purchase cost and estimated profit for the selected case.

The Mod only adds these numbers to the screen; your money, stock and prices stay as they are. Stock you already own is included.

### How the numbers are worked out

Costs use today's prices and production rules, so bought and crafted stock are valued the same way.

- Crops include their seeds; meat includes the animal it came from.
- Honey, fruit, milk, eggs and caught fish count as free materials. Trees, beehives, animals you keep, equipment, labor and running costs are left out.
- Crafted goods use today's wholesale price of their direct ingredients.
- Estimated profit = suggested-price revenue − estimated cost. The percentage is **profit ÷ cost**.

**—** appears where a value can't be worked out, such as a zero cost or a missing suggested price. Products whose cost can't be determined keep the game's original figures in the report.

### Install

Requires Old Market Simulator **2.1.6 on Windows**. Pick the edition that matches your loader—**BepInEx 5** or **MelonLoader 0.7.3**—and use only one loader.

1. Close the game and back up any older copy of this plugin.
2. Download the matching Mod ZIP from the release page (not GitHub's **Source code**) and extract it into the game folder.
3. Check the plugin is in the right place, then start the game.

| Edition | Where the plugin goes |
| --- | --- |
| BepInEx | `BepInEx/plugins/OldMarket.MaterialCost/OldMarket.MaterialCost.dll` |
| MelonLoader | `Mods/OldMarket.MaterialCost.dll` |

### Settings

The report's switch covers everyday use. To edit settings, close the game after the first launch and open `BepInEx/config/local.oldmarket.materialcost.cfg` (BepInEx) or `UserData/OldMarket.MaterialCost.cfg` (MelonLoader).

| Setting | BepInEx section/key | MelonLoader category/key | Default |
| --- | --- | --- | --- |
| Estimate report material costs | `Report.MaterialsOnly` | `OldMarketMaterialCost.MaterialsOnly` | false |
| Diagnostic logging | `Diagnostics.Enabled` | `OldMarketMaterialCost.Diagnostics` | false |

Turn on diagnostic logging only when reporting a problem, then restart the game.

### Languages and multiplayer

Text follows the game's 13 languages and reuses the game's own words for terms such as profit and recommended price. Only players who want these numbers need to install the Mod.

### Updating or removing

To update, close the game, back up the old DLL and replace it. To uninstall, delete `OldMarket.MaterialCost.dll` from the location above; your saves keep working as they are. Keep the configuration file if you want your settings back after reinstalling.

[What changed](CHANGELOG.md) · [Help and feedback](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/SUPPORT.md) · [Development notes](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/material-cost-mod/DEVELOPMENT.md) · [License](LICENSE)

## 中文

制作、订货和查看日报时，多一份原料成本与预计利润参考，定价心里更有数。

[下载 0.5.3](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.3)

### 核心功能

- 在日报、配方详情和码头订单中查看原料成本与预计利润。
- 日报可切换原料估算与游戏原有统计，并记住选择。
- 同时查看单件与整批成本、产出数量及按建议售价计算的利润。

![实机画面：日报中的原料成本、平均售价、预计利润及仅计原料开关](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/material-cost-mod/assets/material-cost-report-gameplay.png?raw=1)

![实机画面：配方页显示整批及单件原料成本、建议售价与预计利润](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/material-cost-mod/assets/recipe-cost-profit-gameplay.png?raw=1)

![实机画面：订单页显示整包进货成本、单件建议售价与预计利润](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/material-cost-mod/assets/order-cost-profit-gameplay.png?raw=1)

### 怎么操作

- **每日销售报告：**打开“仅计原料”，按原料估算成本和利润；关闭后恢复游戏原有数字，下次会记住选择。
- **配方详情：**查看每件和整批的原料成本、产量、当天建议售价与预计利润。
- **码头订购页：**查看当前商品的建议售价、整箱进货成本和预计利润。

Mod 只在界面上增加这些数字，金币、库存和售价都保持原样。已有库存也会一并计算。

### 数字怎么算

成本按当天价格和生产规则估算，买来的和自己做的商品用同一套算法。

- 种植作物计入种子，肉类计入对应的动物。
- 蜂蜜、水果、牛奶、鸡蛋和钓到的鱼，原料成本记为零；树木、蜂箱、饲养的动物、工具、人工和日常经营费用不计入。
- 加工品按直接原料的当天批发价估算。
- 预计利润 = 按建议售价计算的收入 − 估算成本；百分比为**利润 ÷ 成本**。

成本为零或没有建议售价等无法计算的情况显示 **—**。无法确定成本的商品，日报中保留游戏原有数字。

### 安装

需要 **Windows 版 Old Market Simulator 2.1.6**。按已有加载器选择 **BepInEx 5** 或 **MelonLoader 0.7.3** 版本，两个加载器只用一个。

1. 退出游戏，备份旧版插件。
2. 从发布页下载对应的 Mod ZIP（不是 GitHub 的 **Source code**），解压到游戏目录。
3. 确认插件位置正确，然后启动游戏。

| 安装包 | 插件位置 |
| --- | --- |
| BepInEx | `BepInEx/plugins/OldMarket.MaterialCost/OldMarket.MaterialCost.dll` |
| MelonLoader | `Mods/OldMarket.MaterialCost.dll` |

### 调整设置

平时用日报里的开关就够了。需要修改配置时，首次运行后退出游戏，打开 `BepInEx/config/local.oldmarket.materialcost.cfg`（BepInEx）或 `UserData/OldMarket.MaterialCost.cfg`（MelonLoader）。

| 设置 | BepInEx 分组/键名 | MelonLoader 分类/键名 | 默认值 |
| --- | --- | --- | --- |
| 日报按原料估算成本 | `Report.MaterialsOnly` | `OldMarketMaterialCost.MaterialsOnly` | false |
| 诊断日志 | `Diagnostics.Enabled` | `OldMarketMaterialCost.Diagnostics` | false |

只在反馈问题时打开诊断日志，修改后需重启游戏。

### 语言与联机

文字跟随游戏的 13 种语言，利润、建议售价等词直接使用游戏译文。想看这些数字的玩家自己安装即可。

### 更新与卸载

更新时退出游戏，备份旧 DLL 后替换。卸载时删除上表位置的 `OldMarket.MaterialCost.dll`，存档照常可用。保留配置文件，重新安装后会沿用原来的设置。

[版本变化](CHANGELOG.md) · [问题反馈](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/SUPPORT.md) · [开发说明](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/material-cost-mod/DEVELOPMENT.md) · [许可证](LICENSE)
