# Old Market Stack All

English | [中文](#中文说明)

Version 0.2.1 expands the inventory while preserving physical product containers. It targets Old Market Simulator 2.1.6 for Windows/Mono and requires BepInEx 5.

## Features

- Slot lower-left: total goods. Lower-right: physical containers.
- Up to 64 containers per product slot; total goods may exceed 64. Empty containers count as containers with zero goods.
- Each container retains its product ID, contents, cost, and freshness. Compatible contents use the game's weighted integer merge and expiration rules.
- Non-product, non-tool items stack to 64. Tools remain separate with their own durability.
- Short Q/F and mouse placement handle one item or one complete container. Holding Q/F processes the current slot one unit at a time after the hold delay. Hints follow bindings, language, and native fonts.
- 13 game-configured languages, including Simplified and Traditional Chinese. Mod text is embedded; native terms come from the game's `Translations` table.

| Contents | Goods | Containers |
| --- | ---: | ---: |
| Two baskets of 24 | 48 | 2 |
| Three empty baskets | 0 | 3 |
| 64 containers of 85 | 5440 | 64 |

A 65th container uses another unlocked empty slot. With no space, the complete container remains near the player rather than replacing inventory data.

## Install

1. Install BepInEx 5, then close the game.
2. Download [`OldMarket.StackAll-0.2.1.zip`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/stack-all-v0.2.1/OldMarket.StackAll-0.2.1.zip) and verify [`SHA256SUMS.txt`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/stack-all-v0.2.1/SHA256SUMS.txt).
3. Extract into the game directory. The DLL should be at `BepInEx/plugins/OldMarket.StackAll/OldMarket.StackAll.dll`.
4. Start the game and look for `Stack All 0.2.1 ready` in the BepInEx log.

Every multiplayer participant must run 0.2.1. There is no automatic peer-version check; coordinate versions yourself. Do not mix it with vanilla, 0.1.0, or incompatible inventory Mods.

## Controls

Defaults follow the game's rebindable actions. Short Q drops one unit; short F throws one; placement handles one unit. A product unit is one whole container, so a basket of 24 is one action. Holding Q/F acts immediately, then about every 0.12 seconds after an approximately 0.6-second delay. Switching slots/items, opening UI, pausing, losing focus, or pressing both actions cancels the sequence.

## Saves, upgrades, and removal

Extra containers are stored in additional inventory records written by the game's normal save flow. Saves containing them require Stack All 0.2.0 or a later compatible plugin.

Before removing or downgrading: take out extra containers until each product slot contains at most one; split other stacks to vanilla limits; then save normally and exit. The compatible save hook trims unused extra records. Do not disable or hot-unload while extra containers remain.

A legacy 0.1.0 overfull bag remains one bag because no historical container count exists. For example, 48 goods in one old basket display as `48 / 1`. Drain it or use V transfer to restore normal capacity before removal.

## Configuration and build

There are no user options; capacity and save representation are compatibility rules.

```powershell
./stack-all-mod/build.ps1
./stack-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

The build uses local game/BepInEx assemblies as read-only references, runs checks, and creates `outputs/OldMarket.StackAll-0.2.1.zip`; it does not install. All required localization source, resources, and tests are contained in `stack-all-mod/localization/`, so this Mod directory builds independently. Startup rejects an unreviewed game assembly or patch surface.

## Validation and limitations

The original stack/container/input tests, game IL contract checks, and the Mod-local localization checks pass. They cover conservation, empty containers, merges, 64-container overflow, removal, save-array reload, tool durability, hold cancellation, translations, formatting, and native action lookup.

In-game UI, normal save/load, and real multiplayer have not been verified for this release. Back up important saves. Inventory Mods patching the same operations may conflict.

## License and attribution

Original work here is under the [MIT License](LICENSE); game and third-party components are excluded. Citation is voluntary, not a license condition: **Old Market Stack All by Zhaohan Liu**, with a link to the [repository](https://github.com/martin-lzh/old-market-simulator-mods).

---

## 中文说明

0.2.1 在保留实体商品容器的前提下扩展物品栏，适用于 Windows/Mono 版 Old Market Simulator 2.1.6，需要 BepInEx 5。

## 功能

- 格子左下显示商品总数，右下显示实体容器数。
- 每个商品格最多 64 个容器；商品总数可超过 64。空容器按 0 件商品、1 个容器记录。
- 分别保留商品 ID、内容、成本和保鲜数据；兼容内容沿用游戏的加权整数合并及过期规则。
- 非商品、非工具每格最多 64 件；工具独立保留耐久。
- 短按 Q/F 和鼠标摆放每次处理一件或一个完整容器；长按逐个处理。提示跟随键位、语言和原生字体。
- 支持游戏配置的 13 种语言，包括简繁中文。

第 65 个容器会进入另一个已解锁空格；没有空间时，完整容器留在玩家附近，不覆盖物品栏数据。

## 安装

1. 安装 BepInEx 5 并退出游戏。
2. 下载 [`OldMarket.StackAll-0.2.1.zip`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/stack-all-v0.2.1/OldMarket.StackAll-0.2.1.zip)，用 [`SHA256SUMS.txt`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/stack-all-v0.2.1/SHA256SUMS.txt) 校验。
3. 解压到游戏目录，确认 DLL 位于 `BepInEx/plugins/OldMarket.StackAll/OldMarket.StackAll.dll`。
4. 启动后在 BepInEx 日志确认 `Stack All 0.2.1 ready`。

联机所有玩家必须使用 0.2.1。Mod 不自动检查对方版本，请自行确认一致；不能与原版、0.1.0 或不兼容物品栏 Mod 混用。

## 操作与存档

默认键位读取游戏可重绑定动作。短按 Q 丢下、F 投掷、鼠标摆放均处理一个单位；一篮 24 件商品是一个完整容器单位。长按立即处理首个单位，约 0.6 秒后约每 0.12 秒处理一个。切格、物品变化、打开界面、暂停、失焦或同时按 Q/F 会取消。

额外容器记录由游戏正常保存。含这些记录的存档需要 Stack All 0.2.0 或后续兼容插件。停用或降级前，逐个取出额外容器至每个商品格最多一个，将其他堆叠拆回原版上限，然后正常保存并退出。额外容器仍存在时不要停用或热卸载。

旧 0.1.0 超容量篮子没有历史容器数，升级后仍算一个，例如 `48 / 1`。卸载前请消耗或用 V 转移恢复正常容量。

## 配置、构建与验证

没有用户配置项。在仓库根目录运行：

```powershell
./stack-all-mod/build.ps1
./stack-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

构建只读引用本机程序集，运行检查并生成 `outputs/OldMarket.StackAll-0.2.1.zip`，不会安装。所需本地化源码、资源和测试均位于 `stack-all-mod/localization/`，因此本 Mod 目录可以独立构建。原有功能测试、IL 契约检查和 Mod 自带本地化检查均通过；尚未验证游戏内 UI、正常保存/读取和真实多人联机。首次使用前请备份重要存档。

## 许可与引用

原创内容采用 [MIT License](LICENSE)，不涵盖游戏及第三方组件。引用完全自愿，不是许可条件：**Old Market Stack All，作者 Zhaohan Liu**，并链接[项目仓库](https://github.com/martin-lzh/old-market-simulator-mods)。
