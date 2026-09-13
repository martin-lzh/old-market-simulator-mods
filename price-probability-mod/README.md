# Old Market Price Probability

Current source version: **0.1.2**, authorized as a patch for SDK/build/documentation changes; publication is pending. Download links below still refer to the published 0.1.1 release. New local builds use 0.1.2; gameplay behavior and SDK 2.1.6/r1 remain unchanged.

当前源码版本：**0.1.2**，本次已授权将 SDK、构建及文档改动推进一个 patch，尚待发布。下方下载链接仍指向已公开的 0.1.1；本地新构建使用 0.1.2，游戏逻辑及 SDK 2.1.6/r1 不变。


[Risk notes / 风险提示](#risk-notes--风险提示) · [Change Log / 版本记录](CHANGELOG.md)

English | [中文](#中文说明)

Version 0.1.1 adds price-acceptance preview and host-owned automatic anchors to the native price panel. It targets Old Market Simulator 2.1.6 for Windows/Mono and requires BepInEx 5. Download this prerelease from the [release page](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/price-probability-v0.1.1); in-game acceptance is not yet complete.

## Features and meaning

- A 100% → 0% slider updates with entered price and converts a target back to the nearest positive integer price.
- Shows target and actual estimates because integer prices create discrete steps.
- Modes: off, fixed price, and fixed probability. Probability anchors recalculate when wholesale inputs change.
- 100% always uses the game's current recommended price.
- Uses the original confirmation button and original price-update RPC.
- Supports the game's 13 configured languages, including Simplified and Traditional Chinese.

Acceptance is the chance that a customer who already found the product does not reject it as too expensive. It is not daily sell-through. Stock, expiration, restrictions, demand, and traffic still control sales. The estimate uses current wholesale price, `recommendedProfitPercentage`, and scene `maxProfitMultiplier`, including integer half-unit boundaries. Low-priced products can have large probability steps; 0% chooses a positive integer above the accepted range.

Use the native confirmation button to submit the price and save the selected anchor. Closing the panel discards that unsaved anchor edit.

## Multiplayer

Clients may preview, move the slider, and submit ordinary prices. Only the host stores and executes anchors. An active host anchor normalizes client requests during server processing.

Updates use the original `UpdatePriceServerRpc(long, int)` and native networked price. No new NetworkBehaviour, RPC, NetworkVariable, or protocol is added, so clients are not structurally required to install the Mod. Installed clients get preview UI; vanilla clients retain the native panel. Vanilla clients cannot see anchor status, and an open panel does not live-update another player's change. Rules remain on the host machine. Mismatched resources or other automatic pricing Mods can produce conflicts.

## Install

1. Install BepInEx 5 and close the game.
2. Download [`OldMarket.PriceProbability-0.1.1.zip`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/price-probability-v0.1.1/OldMarket.PriceProbability-0.1.1.zip) and verify [`SHA256SUMS.txt`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/price-probability-v0.1.1/SHA256SUMS.txt).
3. Extract into the game directory. The DLL should be at `BepInEx/plugins/OldMarket.PriceProbability/OldMarket.PriceProbability.dll`.
4. Start the game and look for `Price Probability ready` in the BepInEx log.

To uninstall, close the game and remove that plugin directory. Current native prices remain; automatic adjustment stops.

## Configuration

Rules are stored by save slot and product ID in `BepInEx/config/local.oldmarket.priceprobability.cfg`. Generated `Rule` values are empty, `price:<positive integer>`, or `probability:<0..1>`. The UI manages them. If a save is replaced externally, remove that slot's old anchor sections first.

## Build and validation

```powershell
./price-probability-mod/build.ps1
./price-probability-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

The build requires the .NET 8 SDK and uses local assemblies as read-only references, runs checks, and produces `outputs/OldMarket.PriceProbability-0.1.2.zip`; it does not install. Original pricing/math and multiplayer-policy tests, IL contract checks, and all 11 localization suites pass. In-game UI, native save behavior, and real multiplayer have not been verified.

## License and attribution

Original work here is under the [MIT License](LICENSE); game and third-party components are excluded. Citation is voluntary, not a license condition: **Old Market Price Probability by Zhaohan Liu**, with a link to the [repository](https://github.com/martin-lzh/old-market-simulator-mods).

---

## 中文说明

0.1.1 在原生定价页面增加价格接受概率预览和房主自动锚定，适用于 Windows/Mono 版 Old Market Simulator 2.1.6，需要 BepInEx 5。此预发布版可从[发布页面](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/price-probability-v0.1.1)下载，游戏内验收尚未完成。

## 功能与概率含义

- 100% → 0% 滑条随输入售价更新，也可将目标反算为最接近的正整数售价。
- 同时显示目标和实际估算值，因为整数售价会产生离散台阶。
- 提供关闭、固定售价、固定概率；批发参数变化时重新计算概率锚定。
- 100% 始终使用游戏当天建议售价。
- 使用原确认按钮和原改价 RPC，支持游戏配置的 13 种语言。

接受概率指顾客已经找到商品后，不因价格过高而拒绝拿货的概率，不是每日售罄率。库存、过期、限制、需求和客流仍决定销量。估算使用当天批发价、`recommendedProfitPercentage` 和场景 `maxProfitMultiplier`，包含整数半单位边界。低价商品可能出现较大概率台阶；0% 使用高于接受范围的正整数。

请使用原生确认按钮提交售价并保存所选锚定；直接关闭页面会丢弃尚未保存的锚定编辑。

## 多人游戏

客人可以预览、拖动并提交普通售价；只有房主保存和执行锚定。服务器处理请求时会对已锚定商品应用房主规则。

插件只使用原 `UpdatePriceServerRpc(long, int)` 和原生网络价格，不新增 NetworkBehaviour、RPC、NetworkVariable 或协议。因此客人从网络结构上无需安装：安装者有预览 UI，未安装者保留原版页面。原版客人看不到锚定状态，已打开页面也不会实时刷新其他玩家改价。规则保留在房主本机；资源不一致或其他自动定价 Mod 可能冲突。

## 安装与配置

安装 BepInEx 5 并退出游戏。下载 [`OldMarket.PriceProbability-0.1.1.zip`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/price-probability-v0.1.1/OldMarket.PriceProbability-0.1.1.zip)，用 [`SHA256SUMS.txt`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/price-probability-v0.1.1/SHA256SUMS.txt) 校验，解压后确认 DLL 位于 `BepInEx/plugins/OldMarket.PriceProbability/OldMarket.PriceProbability.dll`。卸载时退出游戏并移除该目录；当前售价保留，自动调整停止。

规则按存档槽和商品 ID 保存在 `BepInEx/config/local.oldmarket.priceprobability.cfg`，`Rule` 为留空、`price:<正整数>` 或 `probability:<0..1>`。通常由 UI 管理；在游戏外替换存档后应先删除该槽位的旧锚定段。

## 构建与验证

```powershell
./price-probability-mod/build.ps1
./price-probability-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

构建需要 .NET 8 SDK，只读引用本机程序集，运行检查并生成 `outputs/OldMarket.PriceProbability-0.1.2.zip`，不会安装。原有定价/数学、多人权限、IL 契约检查和 11 项本地化测试均通过；尚未验证游戏内 UI、原生保存行为和真实多人联机。

## 许可与引用

原创内容采用 [MIT License](LICENSE)，不涵盖游戏及第三方组件。引用完全自愿，不是许可条件：**Old Market Price Probability，作者 Zhaohan Liu**，并链接[项目仓库](https://github.com/martin-lzh/old-market-simulator-mods)。

## Risk notes / 风险提示

### English

- Native price RPCs change real prices persisted in saves. Anchors are local slot/product configuration; externally replacing a slot may apply old rules to the wrong save. Back up both and clear mismatched anchors. Uninstalling stops automation, not saved prices.

- No custom network objects/protocol; all players need not install by design. Host anchors override client requests without vanilla UI notices; resource/date differences affect estimates. Actual connections and synchronization are unverified.

- Automatic repricing may lower income/acceptance. Acceptance is not daily sales; integer prices may miss targets. Closing a panel cancels the draft, not saved anchors. Check price/mode before confirming.

- Periodic/pre-purchase checks add processing/sync work. Other auto-pricing Mods may compete; updates may break APIs/assumptions. UI overlap may mislead input. Keep one controller, disable anchors and confirm, check actual price, then exit before rollback.

### 中文

- 原生价格 RPC 改真实售价并持久化；锚定按本地槽位/商品配置，外部替换槽位可能误用旧规则。备份两者并清除不匹配锚定；卸载停止自动跟随，不恢复售价。

- 无自定义网络对象/协议，设计上无需全员安装；房主锚定覆盖客人请求，原版无提示，资源/日期差异影响估算。实际连接和同步未验证。

- 自动改价可能降低收入/接受率，接受率不等于日销量，整数价格可能偏离目标。关面板只取消草稿，不停止已存锚定，确认前检查售价/模式。

- 定时/购买前检查增加处理/同步工作，其他自动定价 Mod 可能争价，更新可能破坏接口/前提，UI 遮挡可能误操作。保留一套控制，关闭锚定确认、核对价签并退出回退。

## SDK and automated builds / SDK 与自动构建

This Mod pins its compilation SDK in [release.json](release.json). Current baseline: game 2.1.6 / SDK r1. Run `python tools/ci.py build` from the repository root for a game-free build. See [SDK maintenance](../sdk/README.md) and [CI releases](../releases/README.md). Compilation does not replace in-game compatibility checks.

本 Mod 在 [release.json](release.json) 固定编译 SDK，当前基线为游戏 2.1.6 / SDK r1。从仓库根目录运行 `python tools/ci.py build` 可无游戏文件构建。见 [SDK 维护](../sdk/README.md)和 [CI 发布](../releases/README.md)；编译不能代替实机兼容验证。
