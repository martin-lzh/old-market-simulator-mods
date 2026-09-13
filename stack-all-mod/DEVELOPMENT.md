# Development notes / 开发说明

**2026-09-14 — maintainer confirmation:** Visual and other local in-game testing is complete; multiplayer testing is the only remaining test area. This records manual testing feedback; it is not an automated test result.

**2026-09-14 — 维护者确认：**视觉及其他本地实机测试已完成，目前只剩多人游戏测试。这是人工测试反馈，不是自动测试结果。

Player instructions: [README](README.md). Repository workflow: [CONTRIBUTING](../CONTRIBUTING.md), [SDK](../sdk/README.md), [releases](../releases/README.md). Earlier unchecked-scenario descriptions below are superseded by the confirmation above. Current test scope: [validation record](../releases/validation.md).

玩家用法见 [README](README.md)。本文保留构建、接口与历史测试资料；下方早期待测列表已由上述最新确认更新，当前实机范围以[验证记录](../releases/validation.md)为准。

## Configuration and build

There are no user options; capacity and save representation are compatibility rules.

```powershell
./stack-all-mod/build.ps1
./stack-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

The build uses local game/BepInEx assemblies as read-only references, runs checks, and creates `outputs/OldMarket.StackAll-0.2.2.zip`; it does not install. All required localization source, resources, and tests are contained in `stack-all-mod/localization/`, so this Mod directory builds independently. Startup rejects an unreviewed game assembly or patch surface.

## Earlier validation notes

The original stack/container/input tests, game IL contract checks, and the Mod-local localization checks pass. They cover conservation, empty containers, merges, 64-container overflow, removal, save-array reload, tool durability, hold cancellation, translations, formatting, and native action lookup.

In-game UI, normal save/load, and real multiplayer have not been verified for this release. Back up important saves. Inventory Mods patching the same operations may conflict.

## 配置、构建与验证

没有用户配置项。在仓库根目录运行：

```powershell
./stack-all-mod/build.ps1
./stack-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

构建只读引用本机程序集，运行检查并生成 `outputs/OldMarket.StackAll-0.2.2.zip`，不会安装。所需本地化源码、资源和测试均位于 `stack-all-mod/localization/`，因此本 Mod 目录可以独立构建。原有功能测试、IL 契约检查和 Mod 自带本地化检查均通过；尚未验证游戏内 UI、正常保存/读取和真实多人联机。首次使用前请备份重要存档。

## SDK and automated builds / SDK 与自动构建

This Mod pins its compilation SDK in [release.json](release.json). Current baseline: game 2.1.6 / SDK r1. Run `python tools/ci.py build` from the repository root for a game-free build. See [SDK maintenance](../sdk/README.md) and [CI releases](../releases/README.md). Compilation does not replace in-game compatibility checks.

本 Mod 在 [release.json](release.json) 固定编译 SDK，当前基线为游戏 2.1.6 / SDK r1。从仓库根目录运行 `python tools/ci.py build` 可无游戏文件构建。见 [SDK 维护](../sdk/README.md)和 [CI 发布](../releases/README.md)；编译不能代替实机兼容验证。
