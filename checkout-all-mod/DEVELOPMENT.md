# Development notes / 开发说明

**2026-09-14 — maintainer confirmation:** Visual and other local in-game testing is complete; multiplayer testing is the only remaining test area. This records manual testing feedback; it is not an automated test result.

**2026-09-14 — 维护者确认：**视觉及其他本地实机测试已完成，目前只剩多人游戏测试。这是人工测试反馈，不是自动测试结果。

Player instructions: [README](README.md). Repository workflow: [CONTRIBUTING](../CONTRIBUTING.md), [SDK](../sdk/README.md), [releases](../releases/README.md). Earlier unchecked-scenario descriptions below are superseded by the confirmation above. Current test scope: [validation record](../releases/validation.md).

玩家用法见 [README](README.md)。本文保留构建、接口与历史测试资料；下方早期待测列表已由上述最新确认更新，当前实机范围以[验证记录](../releases/validation.md)为准。

## Processing and safeguards

The mod handles one product at a time, waits for its network despawn, and only collects payment after every product has disappeared. Each confirmation may take up to 10 seconds. A timeout cancels the session without resending; restart with `F9` or by releasing and holding `E` again. Waiting for the next customer at an empty checkout has no timeout, and the mod does not simulate ringing the bell.

On multiplayer clients, a coin pouch does not expose its checkout association. The mod matches it to the native checkout spawn point by world position with a one-centimeter tolerance and proceeds only when the match is unique. It declines automatic collection for overlapping checkouts or multiple pouches at the same position, leaving manual collection available.

## Build

Requirements: Windows, the .NET 8 SDK or a compatible newer SDK, and a local game installation with BepInEx 5. All required localization source, resources, and tests are contained in `checkout-all-mod/localization/`, so this Mod directory builds independently.

From the repository root, run:

```powershell
./checkout-all-mod/build.ps1
```

For a nondefault game location:

```powershell
./checkout-all-mod/build.ps1 -GameDir 'D:\Path\To\Old Market Simulator'
```

The plugin targets `netstandard2.1`; state tests target `net8.0`. Local game and BepInEx assemblies are read-only references. The script runs localization and checkout-state tests, builds the plugin, and creates `outputs/OldMarket.CheckoutAll-0.1.5.zip`. The package contains only the original DLL, README, CHANGELOG, and MIT license; it includes no game or loader files and installs nothing.

## Earlier validation notes

All current automated tests pass. They cover short presses, hold start and release, toggle start and stop, hold-to-toggle conversion, suppression until `E` is released, refusal to pre-enable without a target, cancellation on checkout changes, menus, focus loss and errors, indefinite empty-checkout waiting, confirmation timeout, and timer reset after confirmation. Builds and checks use Old Market Simulator 2.1.6.

Actual UI and multiplayer behavior have not yet been verified for this prerelease. In-game testing is still needed for the `F9` prompt and toggle, three or more consecutive customers, an initial direct payment followed by a continued hold, idle waits longer than 10 seconds, adjacent checkouts, simultaneous employee or player checkout, menu cancellation, disconnects, and confirmation that money and sales increase exactly once. Client-only installation does not require the host by design, but remains unverified. Other game versions may change interaction APIs.

## 构建

需要 Windows、.NET 8 SDK 或兼容的新版本 SDK，以及装有 BepInEx 5 的本机游戏。所需本地化源码、资源和测试均位于 `checkout-all-mod/localization/`，因此本 Mod 目录可以独立构建。

在仓库根目录运行：

```powershell
./checkout-all-mod/build.ps1
```

游戏不在默认位置时：

```powershell
./checkout-all-mod/build.ps1 -GameDir 'D:\Path\To\Old Market Simulator'
```

插件以 `netstandard2.1` 为目标框架，状态测试以 `net8.0` 为目标框架。本机游戏和 BepInEx 程序集仅作为只读引用。脚本运行本地化和结账状态测试，再构建插件并生成 `outputs/OldMarket.CheckoutAll-0.1.5.zip`。压缩包只包含原创 DLL、本说明、CHANGELOG 和 MIT 许可证，不包含游戏或加载器文件，也不会自动安装。

## 早期验证记录

当前所有自动化测试均已通过，覆盖短按、长按启动与松手停止、切换模式启停、长按转切换、关闭后等待 `E` 松开、未对准目标时拒绝预开启、切换结账台、菜单、失去焦点和错误取消，以及空台无限等待、交互确认超时和确认后重新计时。构建和检查以 Old Market Simulator 2.1.6 为依据。

此预发布版本尚未实机验证界面和联机行为。仍需测试 `F9` 提示与启停、连续处理至少三位顾客、首次按键直接收钱后继续长按、空台等待超过 10 秒、相邻结账台、员工或其他玩家同时结账、菜单取消、断线，以及金币和销量是否只增加一次。设计上仅客户端安装不要求房主安装，但尚未验证。其他游戏版本可能改变本插件使用的交互 API。

## SDK and automated builds / SDK 与自动构建

This Mod pins its compilation SDK in [release.json](release.json). Current baseline: game 2.1.6 / SDK r1. Run `python tools/ci.py build` from the repository root for a game-free build. See [SDK maintenance](../sdk/README.md) and [CI releases](../releases/README.md). Compilation does not replace in-game compatibility checks.

本 Mod 在 [release.json](release.json) 固定编译 SDK，当前基线为游戏 2.1.6 / SDK r1。从仓库根目录运行 `python tools/ci.py build` 可无游戏文件构建。见 [SDK 维护](../sdk/README.md)和 [CI 发布](../releases/README.md)；编译不能代替实机兼容验证。
