# Stack All 0.3.0 Change Log / 版本记录

[English](#english) · [中文](#中文)

## English

### 0.3.0 — prerelease (pending publication)

- Add G to drop one empty reusable container from the selected slot; hold for 0.6 seconds, then repeat every 0.12 seconds. Preserve filled/partial containers, individual metadata and other slots. Exclude seed packets and disposable packaging; cancel on menus, focus loss, competing actions and slot changes.
- Add G to the native right-hand caption/keycap controls and dim the whole row when no empty remains. Place localized hold guidance above the controls using the game font/material, without a keycap, while multiple units remain. Preserve native Q/F behavior.
- Treat seeds as physical packets for stacking, drop, throw, placement and overflow, retaining contents and cost without merging packets. Planting consumes one seed. Previously merged seed records remain one packet; original packet counts cannot be recovered.
- Allow ordinary collectibles such as mint and flowers to rejoin matching stacks despite incidental world-day differences. Preserve food freshness, animal age, fish-trap state, costs, overflow and the 64-item limit.
- Show only physical quantity for disposable single-unit goods such as whole fish. Preserve native font size for one-/two-digit counts; shrink only larger goods totals within the left half.
- Include game-free SDK builds and source-bound CI publication. Pin game 2.1.6 / SDK r11; package only the original plugin and documentation. Save/RPC record formats are unchanged; all peers require the same build. Follow README unpacking and backup instructions before removing or downgrading.
- Local hold/font testing of `d4e033d` is confirmed. Automated stack, localization and game-contract checks passed; later save/reload and host/client scenarios retain their recorded limitations. Version 0.3.0 consolidates the previously unpublished follow-ups.

### 0.2.2 — prerelease (pending publication)

- Add game-versioned SDK compilation and CHANGELOG-driven CI releases; pin this Mod to SDK 2.1.6/r1. Existing published versions are not rebuilt or replaced.

- Add code-specific risks, compatibility guidance and this independent change log. Future builds include CHANGELOG.md. Gameplay code and existing public assets/tags are unchanged.

### 0.2.1 — 2026-09-12 (prerelease)

First public release in this repository: original Mod features with independent localization (Coordinates uses universal axis labels). Detailed features and changes: [release notes](../releases/stack-all-v0.2.1.md).

- Build/reference baseline: Old Market Simulator 2.1.6, Windows x64 / Unity Mono. BepInEx 5; Material Cost also offers MelonLoader 0.7.3. Standalone remains source-only for local testing.
- Game assembly SHA256: FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296.
- Automated checks passed; in-game UI, saves and real multiplayer acceptance remain incomplete.
- [Release/downloads](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/stack-all-v0.2.1); source commit: 01685796bc9953122c33c240e92547bedc0edb79.
- Save/rollback: Extra container records persist through normal saves and need 0.2.0 or a compatible later plugin. Direct disabling/downgrading may fail loading or lose/miscount items. Unpack to vanilla limits and save first; deleting DLLs does not convert saves. Old 0.1.0 merged containers cannot be reconstructed.
- Multiplayer: NetworkList/RPC handling changes: host and every player need the same version; no automatic check exists. Mixed versions may fail joining either way or desynchronize inventory. Matching versions still needs real multiplayer tests.

Older development versions mentioned in the README are not verified downloadable releases in this public repository. No older/newer supported game baseline is recorded. Match game/hash, loader and save requirements; an older Mod does not imply support for an older game.

## 中文

### 0.3.0 — 预发布（待发布）

- 新增 G 丢出当前格的一个空可复用容器；长按 0.6 秒后每 0.12 秒重复。保留满盒、半满盒、独立元数据及其他格；排除种子包和一次性包装，菜单、失焦、其他动作及切换格子会取消连扔。
- G 提示加入原生右侧文字／键帽控件，没有空盒时整行变暗。剩余多个物品时，在按钮上方用游戏字体及材质显示本地化长按说明，不加键帽；保留原生 Q/F 行为。
- 种子按实物包处理堆叠、丢下、投掷、放置与溢出，保留各包内容和成本，不合并种子包；播种消耗一粒。旧版已合并的记录仍保留为一包，无法还原原包数。
- 薄荷、花等普通收集品不再因地面天数差异而无法重新堆叠；保留食品保鲜、动物年龄、鱼笼状态、成本、溢出和 64 件上限。
- 整条鱼等一次性单件商品仅显示实际数量；一至两位数保留原生字号，仅更大的商品量在左半格缩小。
- 接入无游戏 SDK 构建及源码绑定的 CI 发布，固定游戏 2.1.6 / SDK r11，包内仅含原创插件与文档。存档／RPC 记录格式不变，全员需使用同一构建；卸载或降级前遵循 README 拆包及备份说明。
- `d4e033d` 长按说明及字体已有本地测试确认；自动堆叠、本地化和游戏契约检查通过，后续存档重载及房主／客人场景仍保留已记录限制。0.3.0 汇总此前未发布的后续改动。

### 0.2.2 — 预发布（待发布）

- 增加随游戏版本维护的 SDK 编译和 CHANGELOG 驱动的 CI 发布，本 Mod 固定 SDK 2.1.6/r1；不重建或替换既有公开版本。

- 补充代码相关风险、兼容说明和独立版本记录；后续构建包含 CHANGELOG.md。游戏逻辑及现有公开附件/标签不变。

### 0.2.1 — 2026-09-12（预发布）

本仓库首次公开发布：原创功能与独立本地化（坐标使用通用轴标记）。详细功能与变化见[发布说明](../releases/stack-all-v0.2.1.md)。

- 构建/引用基线：Old Market Simulator 2.1.6，Windows x64 / Unity Mono，BepInEx 5；成本插件另有 MelonLoader 0.7.3，Standalone 仅源码本地测试。
- 游戏程序集 SHA256 同上；自动检查通过，实机 UI、存档及多人验收未完成。
- [发布/下载](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/stack-all-v0.2.1)；源码提交同上。
- 存档/回退：额外容器记录由正常存档保留，需要 0.2.0 或后续兼容插件；直接停用/降级可能读档失败、丢物或计数错误。先拆回原版上限并保存，删 DLL 不会转换存档；0.1.0 已合并容器无法重建。
- 联机：修改 NetworkList/RPC 处理，房主及全员须同版本且无自动检查；混版可能双方无法加入或库存不同步。同版本仍需实测。

README 提到的更早开发版本不是本公开仓库已确认的可下载发行版；尚无其他新旧游戏版本兼容记录。匹配游戏/哈希、加载器和存档要求；旧 Mod 不自动等于支持旧游戏。

## Published files / 已发布文件

| File / 文件 | SHA256 |
| --- | --- |
| [OldMarket.StackAll-0.2.1.zip](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/stack-all-v0.2.1/OldMarket.StackAll-0.2.1.zip) | c9a9f18c884efe03316a79672a9142348d3f788a11fa3a773194e53af7d8fe23 |

Future entries must record changes, actual date/downloads, game/hash, loader, save/multiplayer/rollback requirements and test scope. Publish package hashes externally in the release checksums; preserve old releases/tags.

后续条目必须记录变化、真实日期/下载定位、游戏/哈希、加载器、存档/联机/回退要求及验证范围。包哈希放发布页外部校验文件，保留历史发布/标签。
