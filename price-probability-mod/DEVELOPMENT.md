# Development notes / 开发说明

**2026-09-14 — maintainer confirmation:** Visual and other local in-game testing is complete; multiplayer testing is the only remaining test area. This records manual testing feedback; it is not an automated test result.

**2026-09-14 — 维护者确认：**视觉及其他本地实机测试已完成，目前只剩多人游戏测试。这是人工测试反馈，不是自动测试结果。

Player instructions: [README](README.md). Repository workflow: [CONTRIBUTING](../CONTRIBUTING.md), [SDK](../sdk/README.md), [releases](../releases/README.md). Earlier unchecked-scenario descriptions below are superseded by the confirmation above. Current test scope: [validation record](../releases/validation.md).

玩家用法见 [README](README.md)。本文保留构建、接口与历史测试资料；下方早期待测列表已由上述最新确认更新，当前实机范围以[验证记录](../releases/validation.md)为准。

## Build and validation

```powershell
./price-probability-mod/build.ps1
./price-probability-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

The build requires the .NET 8 SDK and uses local assemblies as read-only references, runs checks, and produces `outputs/OldMarket.PriceProbability-0.1.2.zip`; it does not install. Original pricing/math and multiplayer-policy tests, IL contract checks, and all 11 localization suites pass. In-game UI, native save behavior, and real multiplayer have not been verified.

## 构建与验证

```powershell
./price-probability-mod/build.ps1
./price-probability-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

构建需要 .NET 8 SDK，只读引用本机程序集，运行检查并生成 `outputs/OldMarket.PriceProbability-0.1.2.zip`，不会安装。原有定价/数学、多人权限、IL 契约检查和 11 项本地化测试均通过；尚未验证游戏内 UI、原生保存行为和真实多人联机。

## SDK and automated builds / SDK 与自动构建

This Mod pins its compilation SDK in [release.json](release.json). Current baseline: game 2.1.6 / SDK r1. Run `python tools/ci.py build` from the repository root for a game-free build. See [SDK maintenance](../sdk/README.md) and [CI releases](../releases/README.md). Compilation does not replace in-game compatibility checks.

本 Mod 在 [release.json](release.json) 固定编译 SDK，当前基线为游戏 2.1.6 / SDK r1。从仓库根目录运行 `python tools/ci.py build` 可无游戏文件构建。见 [SDK 维护](../sdk/README.md)和 [CI 发布](../releases/README.md)；编译不能代替实机兼容验证。
