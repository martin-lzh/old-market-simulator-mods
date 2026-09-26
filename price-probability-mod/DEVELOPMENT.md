# Development notes / 开发说明

**2026-09-19 — current acceptance:** The maintainer confirms in-game acceptance of Smart Pricing 0.1.2; this is a stable release on game 2.1.6. The earlier `e23f986` bundle also has a two-computer multiplayer report. These are reported manual results, not a new automated game run or a complete scenario matrix; see the [validation record](../releases/validation.md).

**2026-09-19 — 当前验收：**维护者已确认 Smart Pricing 0.1.2 的实机验收，当前为游戏 2.1.6 的正式发布版；此前 `e23f986` 合集另有双实机联机报告。这是人工测试反馈，不代表新执行了自动化游戏测试或补齐完整场景矩阵，详见[验证记录](../releases/validation.md)。

Player instructions: [README](README.md). Repository workflow: [CONTRIBUTING](../CONTRIBUTING.md), [SDK](../sdk/README.md), [releases](../releases/README.md). Earlier unchecked-scenario descriptions below are superseded by the confirmation above. Current test scope: [validation record](../releases/validation.md).

玩家用法见 [README](README.md)。本文保留构建、接口与历史测试资料；下方早期验证状态已由上述确认更新，当前实机范围以[验证记录](../releases/validation.md)为准。

## Build and validation

```powershell
./price-probability-mod/build.ps1
./price-probability-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

The build requires the .NET 8 SDK and uses local assemblies as read-only references, runs checks, and produces `outputs/OldMarket.PriceProbability-0.1.2.zip`; it does not install. Earlier automated evidence records passing pricing/math and multiplayer-policy tests, IL contract checks, and 11 localization test groups covering 13 languages. At that stage, in-game UI, native save behavior and real multiplayer were not yet verified; current acceptance is summarized above.

## 构建与验证

```powershell
./price-probability-mod/build.ps1
./price-probability-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

构建需要 .NET 8 SDK，只读引用本机程序集，运行检查并生成 `outputs/OldMarket.PriceProbability-0.1.2.zip`，不会安装。早期自动化记录中的定价/数学、多人权限、IL 契约检查和覆盖 13 种语言的 11 组本地化测试均通过；当时尚未验证游戏内 UI、原生保存行为和真实多人联机，当前验收状态以上方摘要为准。

## SDK and automated builds / SDK 与自动构建

This Mod pins its compilation SDK in [release.json](release.json). Current baseline: game 2.1.6 / SDK r1. Run `python tools/ci.py build` from the repository root for a game-free build. See [SDK maintenance](../sdk/README.md) and [CI releases](../releases/README.md). Compilation does not replace in-game compatibility checks.

本 Mod 在 [release.json](release.json) 固定编译 SDK，当前基线为游戏 2.1.6 / SDK r1。从仓库根目录运行 `python tools/ci.py build` 可无游戏文件构建。见 [SDK 维护](../sdk/README.md)和 [CI 发布](../releases/README.md)；编译不能代替实机兼容验证。

## Local media archive / 本地素材归档

The local reuse index is `outputs/media/README.md`, with provenance and hashes in `outputs/media/manifest.json`. This ignored archive is absent from clones and release packages; promotional material does not establish in-game acceptance.

本地复用入口为 `outputs/media/README.md`，来源与哈希记录在 `outputs/media/manifest.json`。此归档被 Git 忽略，不随克隆或发行包分发；宣发素材不构成实机验收证据。
