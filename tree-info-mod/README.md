# Tree Harvest Helper

![Tree Harvest Helper](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/tree-info-mod/assets/cover.png?raw=1)

A mod for Old Market Simulator / Old Market Simulator 模组

[English](#english) · [中文](#中文)

## English

See when your fruit trees are ready to harvest, directly in the interaction hint.

[Download 0.1.0](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/tree-info-v0.1.0)

### Core features

- Show production seasons and harvest readiness when you aim at a planted fruit tree.
- Estimate days remaining from the tree’s actual growth state, assuming daily watering.
- Flag missing watering, inactive seasons and insufficient time before season end.
- Follow all 13 game languages and reuse native season names.

![In-game fruit-tree hint showing all-season production and fruit ready to harvest](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/tree-info-mod/assets/tree-harvest-gameplay.png?raw=1)

### How to use

1. Enter your town and walk up to a planted fruit tree.
2. Aim at it; the extra line appears below the game's usual hint.
3. Read its season, days remaining or ready-to-harvest state. There's nothing to set up.

The info appears on fruit trees you've planted, not on saplings in the shop or wild timber trees. Ripe fruit can be picked even outside its production season. Day estimates assume you water the tree every day.

### Install

Requires Old Market Simulator **2.1.6 on Windows** with **BepInEx 5** installed.

1. Close the game and back up any older copy of this plugin.
2. Download the Mod ZIP from the release page (not GitHub's **Source code**) and extract it into the game folder. The plugin should end up at `BepInEx/plugins/OldMarket.TreeInfo/OldMarket.TreeInfo.dll`.
3. Make sure that is the only copy of the DLL under `BepInEx/plugins`, then start the game.

### Multiplayer

Only players who want to see tree info need to install it.

### Updating or removing

To update, close the game, back up the old DLL and replace it. To uninstall, delete `OldMarket.TreeInfo.dll` from its plugin folder.

[What changed](CHANGELOG.md) · [Help and feedback](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/SUPPORT.md) · [Development notes](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/tree-info-mod/DEVELOPMENT.md) · [License](LICENSE)

## 中文

准星对准果树，就能在交互提示中查看生产季节与成熟时间。

[下载 0.1.0](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/tree-info-v0.1.0)

### 核心功能

- 准星对准已种植果树时显示生产季节和成熟状态。
- 按树木实际生长状态估算剩余天数，以每日浇水为前提。
- 提示今日未浇水、本季不生产及季末时间不足。
- 跟随游戏全部 13 种语言，季节名复用原生翻译。

![实机画面：果树提示全年生产，果实已成熟可采收](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/tree-info-mod/assets/tree-harvest-gameplay.png?raw=1)

### 怎么操作

1. 进入小镇，走近一棵已经种下的果树。
2. 用准星对准它，信息会显示在游戏原有提示下方。
3. 查看生产季节、剩余天数或可采收状态，无需任何设置。

信息只显示在你种下的果树上，商店里的树苗和野外的木材树不显示。成熟的果实在非生产季节也能采摘。剩余天数按每天浇水估算。

### 安装

需要 **Windows 版 Old Market Simulator 2.1.6**，并已安装 **BepInEx 5**。

1. 退出游戏，备份旧版插件。
2. 从发布页下载 Mod ZIP（不是 GitHub 的 **Source code**），解压到游戏目录，插件应位于 `BepInEx/plugins/OldMarket.TreeInfo/OldMarket.TreeInfo.dll`。
3. 确认 `BepInEx/plugins` 下只有这一份 DLL，然后启动游戏。

### 和朋友一起玩

想看果树信息的玩家自己安装即可。

### 更新与卸载

更新时退出游戏，备份旧 DLL 后替换。卸载时删除插件文件夹中的 `OldMarket.TreeInfo.dll`。

[版本变化](CHANGELOG.md) · [问题反馈](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/SUPPORT.md) · [开发说明](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/tree-info-mod/DEVELOPMENT.md) · [许可证](LICENSE)
