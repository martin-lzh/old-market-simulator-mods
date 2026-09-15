# Stack All 0.2.2 Change Log / 版本记录

[English](#english) · [中文](#中文)

## English

### Unreleased

- Allow ordinary collectibles such as mint leaves and flowers to rejoin matching stacks when their incidental world-day counters differ after dropping and pickup. Preserve product freshness, animal ages and fish-trap use counts; retain 64-item limits, weighted cost and overflow. Version 0.2.2 and SDK 2.1.6/r1 are unchanged; in-game and multiplayer confirmation is pending.

- Treat seeds as physical packets for stacking, drop, throw, placement, overflow, repeat actions and quantity display. Preserve each packet's remaining seeds and metadata without merging contents; planting still consumes one seed. Existing merged seed records retain their saved quantity as one packet. Save records and RPC formats are unchanged, but all peers need the same build. Seed UI, save/reload and multiplayer acceptance remain untested in game. Retain Mod 0.2.2 and SDK 2.1.6/r1.
- Show only the right-hand physical quantity for single-use, one-unit products such as whole fish. Keep both goods and container counts for reusable containers and multi-unit products, including boxed fish cuts. Inventory, saves and networking are unchanged.
- Keep one- and two-digit quantities at the native font size with auto-sizing disabled and full slot width. Only goods totals of three or more digits may shrink within the left half to avoid overlapping the container count. Game 2.1.6 uses 22-point inventory numbers. The new display regression still needs in-game visual confirmation; the preceding build's two-computer multiplayer test is complete.
- Retain version 0.2.2 and SDK 2.1.6/r1; this follow-up awaits a maintainer-assigned release version.

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

### 未发布

- 修复薄荷叶、花等普通收集品丢出后因地面天数计数不同而无法重新并入同类堆叠的问题。保留食品保鲜、动物年龄、鱼笼使用次数、64 件上限、加权成本与溢出处理。版本 0.2.2 和 SDK 2.1.6/r1 不变；实机与联机仍待确认。

- 种子按实物包处理堆叠、丢下、投掷、放置、溢出、连续操作及数量显示。各包保留剩余种子和元数据，不合并包内数量；播种仍消耗一粒。旧版已合并的种子记录按存档数量保留为一包。存档记录和 RPC 格式不变，但所有玩家须使用相同构建。种子 UI、保存重载和联机仍待实机验收；保持 Mod 0.2.2 和 SDK 2.1.6/r1。
- 整条鱼等容量为 1、用完消失的单件商品只显示右侧实际数量；可复用容器及多件装商品（包括鱼肉箱）仍显示商品量和容器数。不改变库存、存档或网络逻辑。
- 一位数和两位数使用原生字号，关闭自动缩放并使用整个格子的宽度；只有三位及以上的商品总量可在左半格缩小，避免与容器数重叠；游戏 2.1.6 的库存数字原生字号为 22。本次显示修复仍待实机视觉确认，修复前构建的双实机联机测试已完成。
- 保持版本 0.2.2 和 SDK 2.1.6/r1；本次后续修复尚待维护者指定发行版本。

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
