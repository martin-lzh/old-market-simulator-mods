# Development notes / 开发说明

**Current acceptance — 2026-09-19:** Coordinates HUD 0.1.4 is a stable release with maintainer-confirmed in-game acceptance. The earlier `e23f986` bundle also has reported local and two-computer multiplayer acceptance. These are manual reports, not new automated test runs or exhaustive scenario measurements.

**当前验收 — 2026-09-19：**Coordinates HUD 0.1.4 已获维护者实机验收确认，为正式发布版；此前 `e23f986` 合集也已有本地及双实机联机确认。这是人工反馈，不代表新运行了自动检查或完成所有场景测量。

Player instructions: [README](README.md). Repository workflow: [CONTRIBUTING](../CONTRIBUTING.md), [SDK](../sdk/README.md), [releases](../releases/README.md). Earlier unchecked-scenario descriptions below are superseded by the confirmation above. Current test scope: [validation record](../releases/validation.md).

玩家用法见 [README](README.md)。本文保留构建、接口与历史测试资料；下方早期待测列表已由上述最新确认更新，当前实机范围以[验证记录](../releases/validation.md)为准。

## Build

Requirements: Windows, the .NET 8 SDK or a compatible newer SDK, a local game installation with BepInEx 5.

From the repository root, run:

```powershell
./coordinates-mod/build.ps1
```

For a nondefault game location:

```powershell
./coordinates-mod/build.ps1 -GameDir 'D:\Path\To\Old Market Simulator'
```

The project targets `netstandard2.1` and uses local game and BepInEx assemblies as read-only references. The script builds this mod independently and creates `outputs/OldMarket.Coordinates-0.1.4.zip`. The package contains only the original DLL, README, CHANGELOG, and MIT license; it includes no game or loader files and installs nothing.

## Earlier validation notes

The earlier prerelease passed build and packaging checks against Old Market Simulator 2.1.6 with in-game UI placement, glyph rendering, live culture changes, movement and jumping updates, pause and relaunch behavior, and multiplayer-client display still pending. The current stable acceptance above supersedes that pending status; an exhaustive per-scenario matrix was not supplied.

Other game versions may change the player or HUD APIs. Coordinate orientation has no compass labels. The hotkey is keyboard-only.

## 构建

需要 Windows、.NET 8 SDK 或兼容的新版本 SDK、本机游戏安装及 BepInEx 5。

在仓库根目录运行：

```powershell
./coordinates-mod/build.ps1
```

游戏不在默认位置时：

```powershell
./coordinates-mod/build.ps1 -GameDir 'D:\Path\To\Old Market Simulator'
```

项目以 `netstandard2.1` 为目标框架，将本机游戏和 BepInEx 程序集作为只读引用。脚本独立构建插件并生成 `outputs/OldMarket.Coordinates-0.1.4.zip`。压缩包只包含原创 DLL、本说明、CHANGELOG 和 MIT 许可证，不包含游戏或加载器文件，也不会自动安装。

## 早期验证记录

早期预发布版先通过了面向 Old Market Simulator 2.1.6 的构建和打包检查，当时游戏内布局、字形、运行中区域格式变化、移动和跳跃刷新、暂停与退出重进，以及联机客户端显示尚未实机确认。上方当前正式发布验收已取代该待测状态；未提供逐场景完整测试矩阵。

其他游戏版本可能改变本插件依赖的玩家或 HUD API。坐标轴未标注罗盘方向。快捷键仅支持键盘。

## SDK and automated builds / SDK 与自动构建

This Mod pins its compilation SDK in [release.json](release.json). Current baseline: game 2.1.6 / SDK r1. Run `python tools/ci.py build` from the repository root for a game-free build. See [SDK maintenance](../sdk/README.md) and [CI releases](../releases/README.md). Compilation does not replace in-game compatibility checks.

本 Mod 在 [release.json](release.json) 固定编译 SDK，当前基线为游戏 2.1.6 / SDK r1。从仓库根目录运行 `python tools/ci.py build` 可无游戏文件构建。见 [SDK 维护](../sdk/README.md)和 [CI 发布](../releases/README.md)；编译不能代替实机兼容验证。

## Local media archive / 本地素材归档

The local index at `outputs/media/README.md` lists reusable Xiaohongshu, Steam, Nexus and gameplay media with source records. This ignored archive is absent from clones and release packages; archived media alone does not establish in-game acceptance.

本地 `outputs/media/README.md` 索引可复用的小红书、Steam、Nexus 与实机素材及来源记录。归档被 Git 忽略，不随克隆或发行包提供；素材本身不代表已通过实机验收。
