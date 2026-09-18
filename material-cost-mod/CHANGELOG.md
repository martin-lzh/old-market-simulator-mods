# Profit Insights — Changelog / 版本记录

[English](#english) · [中文](#中文)

## English

### Unreleased

- Move the Nexus cover into assets/cover.png and display it at the top of the README. Documentation only; runtime, version and SDK are unchanged.

### 0.5.2 — 2026-09-19 (stable release)

In-game acceptance is confirmed by the maintainer. This version is a stable release; see the [validation record](../releases/validation.md).

- Rename the plugin display name to **Profit Insights** and reorganize the bilingual README around core features and controls, with insertion points for gameplay media. Plugin ID, assembly/configuration paths, gameplay and version are unchanged.

- Correct current README test status to reference the completed `e23f986` local and two-computer multiplayer report. Documentation only; runtime, version and SDK are unchanged.

- Add game-versioned SDK compilation and CHANGELOG-driven CI releases; pin this Mod to SDK 2.1.6/r1. Existing published versions are not rebuilt or replaced.

- Add code-specific risks, compatibility guidance and this independent change log. Future builds include CHANGELOG.md. Gameplay code and existing public assets/tags are unchanged.

### 0.5.1 — 2026-09-12 (prerelease)

First public release in this repository: original Mod features with independent localization (Coordinates uses universal axis labels). Detailed features and changes: [release notes](../releases/material-cost-v0.5.1.md).

- Build/reference baseline: Old Market Simulator 2.1.6, Windows x64 / Unity Mono. BepInEx 5; Material Cost also offers MelonLoader 0.7.3. Standalone remains source-only for local testing.
- Game assembly SHA256: FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296.
- Automated checks passed; in-game UI, saves and real multiplayer acceptance remain incomplete.
- [Release/downloads](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.1); source commit: 01685796bc9953122c33c240e92547bedc0edb79.
- Save/rollback: Saves: UI-only patches and local toggle configuration; no new game-save fields or cash/item-cost changes. Old-inventory estimates are not historical purchase costs.
- Multiplayer: Multiplayer: no custom protocol or connection changes; only viewers need this Mod by design. Actual host/client tests are incomplete; loader/Mod combinations may prevent joining in either direction.

Older development versions mentioned in the README are not verified downloadable releases in this public repository. No older/newer supported game baseline is recorded. Match game/hash, loader and save requirements; an older Mod does not imply support for an older game.

## 中文

### 未发布

- 将 Nexus 封面移入 assets/cover.png，并作为 README 首图展示。仅文档改动，运行时、版本和 SDK 不变。

### 0.5.2 — 2026-09-19（正式发布）

维护者已确认实机验收完成，本版本为正式发布版，详见[验证记录](../releases/validation.md)。

- 插件显示名改为 **Profit Insights**，双语 README 按核心功能和操作方式组织，并保留实机素材插入位置。插件 ID、程序集／配置路径、玩法及版本不变。

- 修正 README 当前测试状态，引用 `e23f986` 已完成的本地及双实机联机报告。仅文档改动，运行时、版本和 SDK 不变。

- 增加随游戏版本维护的 SDK 编译和 CHANGELOG 驱动的 CI 发布，本 Mod 固定 SDK 2.1.6/r1；不重建或替换既有公开版本。

- 补充代码相关风险、兼容说明和独立版本记录；后续构建包含 CHANGELOG.md。游戏逻辑及现有公开附件/标签不变。

### 0.5.1 — 2026-09-12（预发布）

本仓库首次公开发布：原创功能与独立本地化（坐标使用通用轴标记）。详细功能与变化见[发布说明](../releases/material-cost-v0.5.1.md)。

- 构建/引用基线：Old Market Simulator 2.1.6，Windows x64 / Unity Mono，BepInEx 5；成本插件另有 MelonLoader 0.7.3，Standalone 仅源码本地测试。
- 游戏程序集 SHA256 同上；自动检查通过，实机 UI、存档及多人验收未完成。
- [发布/下载](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.1)；源码提交同上。
- 存档/回退：存档：仅 UI 补丁及本地开关配置，不新增存档字段或修改现金/物品成本；旧库存估算不是历史采购成本。
- 联机：联机：不新增协议或修改连接流程，设计上仅查看者需要安装；房主/客人实测未完成，加载器/Mod 组合可能导致双方无法加入。

README 提到的更早开发版本不是本公开仓库已确认的可下载发行版；尚无其他新旧游戏版本兼容记录。匹配游戏/哈希、加载器和存档要求；旧 Mod 不自动等于支持旧游戏。

## Published files / 已发布文件

| File / 文件 | SHA256 |
| --- | --- |
| [OldMarket.MaterialCost-0.5.1-BepInEx.zip](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/material-cost-v0.5.1/OldMarket.MaterialCost-0.5.1-BepInEx.zip) | 76fe454cfe1c448491fff62dcb15801814c87656545fc01d8c23061d01257bc7 |
| [OldMarket.MaterialCost-0.5.1-MelonLoader.zip](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/material-cost-v0.5.1/OldMarket.MaterialCost-0.5.1-MelonLoader.zip) | 98bc4131c68fd3a5705f5769bbae9df87e66ed6d60023cb04028d6073d40c39d |

Future entries must record changes, actual date/downloads, game/hash, loader, save/multiplayer/rollback requirements and test scope. Publish package hashes externally in the release checksums; preserve old releases/tags.

后续条目必须记录变化、真实日期/下载定位、游戏/哈希、加载器、存档/联机/回退要求及验证范围。包哈希放发布页外部校验文件，保留历史发布/标签。
