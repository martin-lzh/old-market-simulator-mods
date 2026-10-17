# Smart Pricing

![Smart Pricing](assets/cover.png)

A mod for Old Market Simulator / Old Market Simulator 模组

[English](#english) · [中文](#中文)

## English

See how likely customers are to accept a price, then choose whether to keep a fixed price or a fixed acceptance chance.

[Download 0.1.2](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/price-probability-v0.1.2)

### Core features

- Preview the chance that a customer accepts a price in the normal pricing panel.
- Adjust the price or target acceptance chance and see the corresponding estimate.
- Let the host keep a fixed price or automatically maintain a target acceptance chance as wholesale prices change.

![In-game pricing panel with price acceptance estimate, target slider and pricing-rule control](assets/smart-pricing-gameplay.png)

### How to use

1. Open the game's price panel. Changing the price updates the chance; dragging the chance slider fills in a matching price.
2. Choose a rule: **Off**, **Fixed price** or **Fixed probability**. Fixed probability re-prices the product whenever wholesale prices change. A 100% target uses the game's suggested price for the day.
3. Press the game's **Confirm** button to save. Closing the panel without confirming discards the changes.

![In-game fixed-probability pricing: an 86.2% target corresponds to a selling price of 73 C](assets/fixed-probability-gameplay.png)

The percentage is the chance that a customer who has found the product accepts the price. How much you sell still depends on demand, customer traffic, freshness and stock. Prices are whole numbers, so the actual chance can land slightly off the target, most noticeably on cheap products.

### Install

Requires Old Market Simulator **2.1.6 on Windows** with **BepInEx 5** installed.

1. Close the game and back up your save and any older copy of this plugin.
2. Download the Mod ZIP from the release page (not GitHub's **Source code**) and extract it into the game folder. The plugin should end up at `BepInEx/plugins/OldMarket.PriceProbability/OldMarket.PriceProbability.dll`.
3. Make sure that is the only copy of the DLL under `BepInEx/plugins`, then start the game.

### Multiplayer

Pricing rules are saved and applied by the host. Guests with the Mod can preview chances; guests without it use the normal pricing screen. The host's rules win over a guest's price change. Reopen a price panel to see changes made by someone else.

Don't run another automatic-pricing Mod at the same time.

### Managing rules

Rules are set in the price panel and saved per save slot in `BepInEx/config/local.oldmarket.priceprobability.cfg`.

- **Clear one product's rule:** select **Off** in its price panel and press **Confirm**.
- **Reuse a save slot for a different save:** clear the old rules first so they don't carry over.
- **Reset all rules:** close the game and move the configuration file out of `BepInEx/config` (keep it as a backup). A fresh one is created when needed.
- **Move a world to another host or computer:** set the rules up again there, or copy this configuration file along with the save.

### Updating or removing

To update, close the game, back up the old DLL and settings, then replace the plugin. To uninstall, delete `OldMarket.PriceProbability.dll`. Automatic re-pricing stops, and products keep their current prices.

[What changed](CHANGELOG.md) · [Help and feedback](../SUPPORT.md) · [Development notes](DEVELOPMENT.md) · [Translations](localization/README.md) · [License](LICENSE)

## 中文

定价时直接看看顾客愿不愿意买，还可以选择固定售价或固定接受概率，让价格随行情调整。

[下载 0.1.2](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/price-probability-v0.1.2)

### 核心功能

- 在原生定价面板预览顾客接受该价格的概率。
- 调整价格或目标接受率，查看对应估算。
- 房主可设置固定价格，或随批发价变化自动维持目标接受率。

![实机画面：定价面板中的价格接受概率、目标滑条与定价规则控件](assets/smart-pricing-gameplay.png)

### 怎么操作

1. 打开游戏的定价页。修改售价会更新概率，拖动概率滑条会自动填入对应售价。
2. 选择规则：“关闭”“固定售价”或“固定概率”。固定概率会在批发价变化时自动重新定价；目标为 100% 时使用游戏当天的建议售价。
3. 点击游戏原有的**确认**按钮保存。不确认直接关闭会放弃本次修改。

![实机画面：固定概率模式下，86.2% 目标接受率对应 73 C 售价](assets/fixed-probability-gameplay.png)

这里的概率是顾客找到商品后愿意接受这个价格的可能性。实际卖出多少，还取决于需求、客流、保鲜和库存。售价只能是整数，所以实际概率会与目标略有出入，低价商品更明显。

### 安装

需要 **Windows 版 Old Market Simulator 2.1.6**，并已安装 **BepInEx 5**。

1. 退出游戏，备份存档和旧版插件。
2. 从发布页下载 Mod ZIP（不是 GitHub 的 **Source code**），解压到游戏目录，插件应位于 `BepInEx/plugins/OldMarket.PriceProbability/OldMarket.PriceProbability.dll`。
3. 确认 `BepInEx/plugins` 下只有这一份 DLL，然后启动游戏。

### 和朋友一起玩

定价规则由房主保存和执行。安装了 Mod 的客人可以查看概率，没安装的客人照常使用游戏定价页。房主的规则优先于客人的改价。别人改价后，重新打开定价页即可看到。

请不要同时使用其他自动定价 Mod。

### 管理规则

规则在定价页设置，按存档槽保存在 `BepInEx/config/local.oldmarket.priceprobability.cfg`。

- **清除单个商品的规则：**在其定价页选择**关闭**并点击**确认**。
- **同一存档槽换成另一份存档：**先清除旧规则，避免沿用。
- **重置全部规则：**退出游戏，把配置文件移出 `BepInEx/config`（留作备份），需要时会自动生成新文件。
- **把世界转给另一位房主或另一台电脑：**在新位置重新设置规则，或连同存档一起复制这个配置文件。

### 更新与卸载

更新时退出游戏，备份旧 DLL 和设置后替换插件。卸载时删除 `OldMarket.PriceProbability.dll`；自动调价随之停止，商品保持当前售价。

[版本变化](CHANGELOG.md) · [问题反馈](../SUPPORT.md) · [开发说明](DEVELOPMENT.md) · [翻译说明](localization/README.md) · [许可证](LICENSE)
