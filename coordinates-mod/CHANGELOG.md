# Coordinates 0.1.3 Change Log / 版本记录

[English](#english) · [中文](#中文)

## English

### Unreleased

- Add code-specific risks, compatibility guidance and this independent change log. Future builds include CHANGELOG.md. Gameplay code and existing public assets/tags are unchanged.

### 0.1.3 — 2026-09-12 (prerelease)

First public release in this repository: original Mod features with independent localization (Coordinates uses universal axis labels). Detailed features and changes: [release notes](../releases/coordinates-v0.1.3.md).

- Build/reference baseline: Old Market Simulator 2.1.6, Windows x64 / Unity Mono. BepInEx 5; Material Cost also offers MelonLoader 0.7.3. Standalone remains source-only for local testing.
- Game assembly SHA256: FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296.
- Automated checks passed; in-game UI, saves and real multiplayer acceptance remain incomplete.
- [Release/downloads](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.3); source commit: 01685796bc9953122c33c240e92547bedc0edb79.
- Save/rollback: Saves: reads local position and creates HUD text; no save writes, teleportation, inventory changes or migration. Back up saves before loader changes.
- Multiplayer: Multiplayer: no RPC or connection changes; others need not install by design. Two-machine tests are incomplete, so joining friends or accepting connections is not guaranteed with all loader/Mod combinations.

Older development versions mentioned in the README are not verified downloadable releases in this public repository. No older/newer supported game baseline is recorded. Match game/hash, loader and save requirements; an older Mod does not imply support for an older game.

## 中文

### 未发布

- 补充代码相关风险、兼容说明和独立版本记录；后续构建包含 CHANGELOG.md。游戏逻辑及现有公开附件/标签不变。

### 0.1.3 — 2026-09-12（预发布）

本仓库首次公开发布：原创功能与独立本地化（坐标使用通用轴标记）。详细功能与变化见[发布说明](../releases/coordinates-v0.1.3.md)。

- 构建/引用基线：Old Market Simulator 2.1.6，Windows x64 / Unity Mono，BepInEx 5；成本插件另有 MelonLoader 0.7.3，Standalone 仅源码本地测试。
- 游戏程序集 SHA256 同上；自动检查通过，实机 UI、存档及多人验收未完成。
- [发布/下载](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.3)；源码提交同上。
- 存档/回退：存档：只读本地位置并创建 HUD，不写存档、不传送、不改库存，无迁移；更换加载器前仍应备份。
- 联机：联机：不发送 RPC 或改连接流程，设计上其他人无需安装。双端未验证，不保证各种加载器/Mod 组合下能加入好友或接受连接。

README 提到的更早开发版本不是本公开仓库已确认的可下载发行版；尚无其他新旧游戏版本兼容记录。匹配游戏/哈希、加载器和存档要求；旧 Mod 不自动等于支持旧游戏。

## Published files / 已发布文件

| File / 文件 | SHA256 |
| --- | --- |
| [OldMarket.Coordinates-0.1.3.zip](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/coordinates-v0.1.3/OldMarket.Coordinates-0.1.3.zip) | 293dac15ed69b0601173a89918edddfcee34ad4bd0affcf2ebbecd9741b75484 |

Future entries must record changes, actual date/downloads, game/hash, loader, save/multiplayer/rollback requirements and test scope. Publish package hashes externally in the release checksums; preserve old releases/tags.

后续条目必须记录变化、真实日期/下载定位、游戏/哈希、加载器、存档/联机/回退要求及验证范围。包哈希放发布页外部校验文件，保留历史发布/标签。
