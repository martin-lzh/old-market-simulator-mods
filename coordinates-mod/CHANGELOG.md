# Coordinates HUD — Changelog / 版本记录

[English](#english) · [中文](#中文)

## English

### Unreleased

### 0.1.5 — 2026-10-18

- Rewrite the README in plainer language, focusing on what the Mod does and how to use it, with clearer installation, multiplayer and removal steps.
- Links and images in the README and CHANGELOG now use full GitHub addresses, so they also work when opened from the downloaded ZIP.
- Add an in-game screenshot of the coordinate display, and show the Nexus cover at the top of the README.
- The README now links to the guide for building the Mod yourself.
- The Mod is now licensed under the PolyForm Noncommercial License 1.0.0: free to use, share and modify for non-commercial purposes; commercial use is not allowed. Versions released before this stay under MIT.

### 0.1.4 — 2026-09-19 (stable release)

In-game acceptance is confirmed by the maintainer. This version is a stable release; see the [validation record](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/releases/validation.md).

- Rename the plugin display name to **Coordinates HUD** and reorganize the bilingual README around core features and controls, with insertion points for gameplay media. Plugin ID, assembly/configuration paths, gameplay and version are unchanged.

- Correct current README test status to reference the completed `e23f986` local and two-computer multiplayer report. Documentation only; runtime, version and SDK are unchanged.

- Add game-versioned SDK compilation and CHANGELOG-driven CI releases; pin this Mod to SDK 2.1.6/r1. Existing published versions are not rebuilt or replaced.

- Add code-specific risks, compatibility guidance and this independent change log. Future builds include CHANGELOG.md. Gameplay code and existing public assets/tags are unchanged.

### 0.1.3 — 2026-09-12 (prerelease)

First public release in this repository: original Mod features with independent localization (Coordinates uses universal axis labels). Detailed features and changes: [release notes](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/releases/coordinates-v0.1.3.md).

- Build/reference baseline: Old Market Simulator 2.1.6, Windows x64 / Unity Mono. BepInEx 5; Material Cost also offers MelonLoader 0.7.3. Standalone remains source-only for local testing.
- Game assembly SHA256: FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296.
- Automated checks passed; in-game UI, saves and real multiplayer acceptance remain incomplete.
- [Release/downloads](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.3); source commit: 01685796bc9953122c33c240e92547bedc0edb79.
- Save/rollback: Saves: reads local position and creates HUD text; no save writes, teleportation, inventory changes or migration. Back up saves before loader changes.
- Multiplayer: Multiplayer: no RPC or connection changes; others need not install by design. Two-machine tests are incomplete, so joining friends or accepting connections is not guaranteed with all loader/Mod combinations.

Older development versions mentioned in the README are not verified downloadable releases in this public repository. No older/newer supported game baseline is recorded. Match game/hash, loader and save requirements; an older Mod does not imply support for an older game.

## 中文

### 未发布

### 0.1.5 — 2026-10-18

- 用更直白的语言重写 README，聚焦功能和用法，并理清安装、联机与卸载步骤。
- README 和 CHANGELOG 中的链接与图片改用完整 GitHub 地址，从下载的 ZIP 中打开也能正常访问。
- README 加入坐标显示的实机截图，并以 Nexus 封面作为首图。
- README 中新增自行构建 Mod 的说明链接。
- 本 Mod 改用 PolyForm Noncommercial 1.0.0 许可：可免费用于非商业用途的使用、分享和修改，不允许商用。此前已发布的版本仍为 MIT 许可。

### 0.1.4 — 2026-09-19（正式发布）

维护者已确认实机验收完成，本版本为正式发布版，详见[验证记录](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/releases/validation.md)。

- 插件显示名改为 **Coordinates HUD**，双语 README 按核心功能和操作方式组织，并保留实机素材插入位置。插件 ID、程序集／配置路径、玩法及版本不变。

- 修正 README 当前测试状态，引用 `e23f986` 已完成的本地及双实机联机报告。仅文档改动，运行时、版本和 SDK 不变。

- 增加随游戏版本维护的 SDK 编译和 CHANGELOG 驱动的 CI 发布，本 Mod 固定 SDK 2.1.6/r1；不重建或替换既有公开版本。

- 补充代码相关风险、兼容说明和独立版本记录；后续构建包含 CHANGELOG.md。游戏逻辑及现有公开附件/标签不变。

### 0.1.3 — 2026-09-12（预发布）

本仓库首次公开发布：原创功能与独立本地化（坐标使用通用轴标记）。详细功能与变化见[发布说明](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/releases/coordinates-v0.1.3.md)。

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
