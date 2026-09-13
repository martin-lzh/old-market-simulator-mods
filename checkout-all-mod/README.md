# Old Market Checkout All

[Risk notes / 风险提示](#risk-notes--风险提示) · [Change Log / 版本记录](CHANGELOG.md)

Continuously bags products and collects payment at one checkout while you hold a key or enable a toggle.

Version 0.1.5 is prepared as a prerelease for Old Market Simulator 2.1.6, Unity 2022.3, and BepInEx 5; publication is pending.

## Features

- Hold keyboard `E` for the configured delay to process checkout products in order and collect the resulting coin pouch.
- Press `F9` while aiming at a checkout to toggle continuous checkout without holding `E`.
- Waits at the same empty checkout for later customers with no idle timeout.
- Uses the game's original `CheckoutItem.Interact` and `CoinPouch.Interact` paths and server RPCs. Prices, money, saves, customer queues, and pouch creation remain under game logic.
- Sends each locally observed product or pouch interaction at most once, then waits for server confirmation before continuing.
- Shows a localized hint below the native interaction prompt in all 13 game locales. Native action terms, font, and font size come from the current game localization and UI.
- Works as a standalone BepInEx 5 plugin; Coordinates and Material Cost are not required.

Short presses retain the original game interaction. The first `E` press may therefore bag one product, ring the bell, or collect money before continuous mode begins. Reaching the hold threshold starts continuous processing. Pressing `F9` during a hold converts that session to toggle mode, so releasing `E` no longer stops it.

Both modes remain locked to their starting checkout. Looking away, leaving interaction range, opening a menu, losing application focus, changing checkout, disconnecting, or encountering an error cancels pending automation. Completed interactions are not undone. Restart with `F9`, or release and hold `E` again. Turning toggle mode off while `E` remains held blocks a new hold session until `E` is released.

## Download

0.1.5 is not published yet. The links below refer to the previous 0.1.4 release.

Download [`OldMarket.CheckoutAll-0.1.4.zip`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/checkout-all-v0.1.4/OldMarket.CheckoutAll-0.1.4.zip) from the [`checkout-all-v0.1.4` prerelease](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.4).

The release also includes [`SHA256SUMS.txt`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/checkout-all-v0.1.4/SHA256SUMS.txt) for package verification.

## Installation

1. Exit the game completely.
2. Make sure BepInEx 5 is installed and working.
3. Extract the archive into the game directory. The DLL should be at `BepInEx/plugins/OldMarket.CheckoutAll/OldMarket.CheckoutAll.dll`.
4. Start the game, aim at a checkout within normal interaction distance, and hold `E` or press `F9`.

To update, exit the game and replace only this mod's DLL. To uninstall, exit the game and remove `OldMarket.CheckoutAll.dll`; do not remove BepInEx or unrelated plugins.

## Controls and configuration

In the development build, hold instructions always use the game's default white. Only the toggle status is red when off and white when on; holding E does not enable the toggle. With `ToggleKey = None`, the toggle status is omitted. Native interaction availability no longer colors these instructions. This UI fix is not included in the published 0.1.4 download; in-game color and layout verification is pending.

The first launch creates `BepInEx/config/local.oldmarket.checkoutall.cfg`.

| Setting | Default | Description |
| --- | --- | --- |
| `Checkout.HoldSeconds` | `0.6` | Seconds keyboard `E` must remain held before continuous checkout starts. Range: `0.3`–`2.0`. |
| `Checkout.ToggleKey` | `F9` | Unity Input System keyboard key that toggles the checkout currently under the crosshair. `None` disables it. |

Edit configuration while the game is closed. Hold input is fixed to keyboard `E`; it does not follow game rebinding and does not support a controller. Toggle state is not saved between launches and cannot be enabled in advance without aiming at a checkout.

Keep the crosshair on the checkout body for reliable targeting. A product or pouch may disappear after interaction and leave the crosshair outside the checkout, which cancels the session.

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

## Validation and known limits

All current automated tests pass. They cover short presses, hold start and release, toggle start and stop, hold-to-toggle conversion, suppression until `E` is released, refusal to pre-enable without a target, cancellation on checkout changes, menus, focus loss and errors, indefinite empty-checkout waiting, confirmation timeout, and timer reset after confirmation. Builds and checks use Old Market Simulator 2.1.6.

Actual UI and multiplayer behavior have not yet been verified for this prerelease. In-game testing is still needed for the `F9` prompt and toggle, three or more consecutive customers, an initial direct payment followed by a continued hold, idle waits longer than 10 seconds, adjacent checkouts, simultaneous employee or player checkout, menu cancellation, disconnects, and confirmation that money and sales increase exactly once. Client-only installation does not require the host by design, but remains unverified. Other game versions may change interaction APIs.

## License and attribution

Copyright © 2026 Zhaohan Liu. Original source, tests, and documentation in this directory are available under the [MIT License](LICENSE). Copies or substantial portions must retain the copyright and license notice. Old Market Simulator and third-party components are not covered.

If this mod helps your project, article, or video, please credit **Old Market Checkout All** and link to the [Old Market Simulator Mods repository](https://github.com/martin-lzh/old-market-simulator-mods). This is a voluntary request, not an additional license condition.

---

# Old Market Checkout All（中文）

长按按键或开启切换模式后，在同一张结账台持续装袋商品并收取钱袋。

0.1.5 是面向 Old Market Simulator 2.1.6、Unity 2022.3 和 BepInEx 5 准备的预发布版本，尚未发布。

## 功能

- 按住键盘 `E` 达到设定时间后，依次装袋结账台上的商品并收取随后出现的钱袋。
- 准星对着结账台时按 `F9`，可在无需持续按住 `E` 的情况下开启或关闭连续结账。
- 同一张空结账台可以不限时等待后续顾客。
- 调用游戏原有的 `CheckoutItem.Interact`、`CoinPouch.Interact` 及服务器 RPC。商品价格、金币、存档、顾客队列和钱袋生成仍由游戏原逻辑处理。
- 本地观察到的每件商品或钱袋最多发送一次交互，等待服务器确认对象消失后才继续。
- 在原生交互提示下方显示支持游戏全部 13 种语言的提示。原生动作词、字体和字号来自游戏当前的本地化与界面。
- 是独立的 BepInEx 5 插件，不依赖 Coordinates 或 Material Cost Mod。

短按仍使用游戏原有操作。因此按下 `E` 的第一刻可能先装袋一件商品、响铃或收钱；达到长按阈值后才由 Mod 继续处理。长按期间按 `F9` 会把当前会话转为切换模式，之后松开 `E` 也不会停止。

两种模式都锁定在启动时对准的结账台。看向别处、走出交互距离、打开菜单、游戏失去焦点、切换结账台、断线或发生错误，都会取消尚未完成的自动操作；已经完成的交互不会撤回。取消后需要再次按 `F9`，或松开并重新长按 `E`。如果在 `E` 仍按住时关闭切换模式，必须先松开 `E` 才能开始新的长按会话。

## 下载

0.1.5 尚未发布，以下链接保留指向上一版 0.1.4。

从 [`checkout-all-v0.1.4` 预发布页面](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.4)下载 [`OldMarket.CheckoutAll-0.1.4.zip`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/checkout-all-v0.1.4/OldMarket.CheckoutAll-0.1.4.zip)。发布页同时提供 [`SHA256SUMS.txt`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/checkout-all-v0.1.4/SHA256SUMS.txt) 用于校验压缩包。

## 安装

1. 完全退出游戏。
2. 确认 BepInEx 5 已安装并能正常加载插件。
3. 将压缩包解压到游戏目录。DLL 的最终路径应为 `BepInEx/plugins/OldMarket.CheckoutAll/OldMarket.CheckoutAll.dll`。
4. 启动游戏，在原版交互距离内将准星对准结账台，然后长按 `E` 或按 `F9`。

更新时先退出游戏，只替换本 Mod 的 DLL。卸载时退出游戏并移除 `OldMarket.CheckoutAll.dll`；不要删除 BepInEx 或其他插件。

## 操作与配置

开发版本中，长按说明始终使用游戏默认白色。只有切换状态在停用时为红色、启用时为白色；长按 E 不等于开启切换模式。`ToggleKey = None` 时不显示切换状态。提示颜色不再受原生交互可用性影响。已发布的 0.1.4 下载包尚不包含此修复，颜色与排版仍待实机验证。

首次运行会生成 `BepInEx/config/local.oldmarket.checkoutall.cfg`。

| 设置 | 默认值 | 说明 |
| --- | --- | --- |
| `Checkout.HoldSeconds` | `0.6` | 键盘 `E` 需要持续按住多少秒才启动连续结账。有效范围为 `0.3`–`2.0`。 |
| `Checkout.ToggleKey` | `F9` | 在准星当前指向的结账台切换连续结账的 Unity Input System 键盘按键。设为 `None` 可禁用。 |

请在游戏退出后编辑配置。长按输入固定为键盘 `E`，不会跟随游戏按键重绑定，也不支持手柄。切换状态不会保存到下次启动，未对准结账台时也不能预先开启。

为提高目标识别的稳定性，建议让准星一直对着结账台本体。商品或钱袋在交互后会消失，如果此时准星落到结账台外，会话就会取消。

## 处理方式与保护措施

Mod 每次处理一件商品，等待它从网络中消失，并在所有商品都消失后才收取钱袋。每次交互确认最多等待 10 秒；超时会取消会话且不会重发，需要按 `F9` 或松开并重新长按 `E`。空结账台等待下一位顾客没有超时，Mod 也不会模拟响铃。

联机客户端的钱袋不公开所属结账台。Mod 会用原生钱袋生成点的世界位置匹配结账台，容差为 1 厘米，并且只在结果唯一时继续。结账台重叠或同一位置存在多个钱袋时不会自动收钱，仍可手动收取。

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

## 验证与已知限制

当前所有自动化测试均已通过，覆盖短按、长按启动与松手停止、切换模式启停、长按转切换、关闭后等待 `E` 松开、未对准目标时拒绝预开启、切换结账台、菜单、失去焦点和错误取消，以及空台无限等待、交互确认超时和确认后重新计时。构建和检查以 Old Market Simulator 2.1.6 为依据。

此预发布版本尚未实机验证界面和联机行为。仍需测试 `F9` 提示与启停、连续处理至少三位顾客、首次按键直接收钱后继续长按、空台等待超过 10 秒、相邻结账台、员工或其他玩家同时结账、菜单取消、断线，以及金币和销量是否只增加一次。设计上仅客户端安装不要求房主安装，但尚未验证。其他游戏版本可能改变本插件使用的交互 API。

## 许可与引用

Copyright © 2026 Zhaohan Liu。本目录的原创源码、测试和文档采用 [MIT License](LICENSE)。复制软件或其重要部分时必须保留版权和许可声明；Old Market Simulator 及第三方组件不受此许可证覆盖。

如果本 Mod 帮助了你的项目、文章或视频，欢迎注明 **Old Market Checkout All** 并链接到 [Old Market Simulator Mods 仓库](https://github.com/martin-lzh/old-market-simulator-mods)。这是自愿引用请求，不是附加许可条件。

## Risk notes / 风险提示

### English

- Saves: no custom format/session persistence, but completed native transactions affect normal saves. Uninstalling does not undo bagging/payments; back up and test a copy.

- Native RPCs need no new connection protocol; server installation is not required by design but real multiplayer is unverified. Latency, disconnects, staff/player races may cause timeouts or incorrect results. Local deduplication cannot guarantee no multiplayer duplicates; connections may fail with incompatible combinations.

- First press can bag/ring/collect immediately. F9 mode continues after release; toggle off or look away. Sent actions cannot be recalled; hotkeys can conflict.

- Updates or checkout Mods may break interaction/confirmation patches. Continuous checks add overhead and hints may overlap. Stop automation on errors, exit before rollback, and check income/counts/sales.

### 中文

- 存档：无自定义格式/会话持久化，但原生交易会影响正常保存；卸载不撤销装袋/收款，应备份并测试副本。

- 原生 RPC 不新增连接协议，设计上服务器无需安装但未实测。延迟、断线、员工/玩家竞争可能超时或结果异常；本地去重不保证联机绝不重复，不兼容组合可能连接失败。

- 首按可立即装袋/响铃/收款；F9 模式松手仍继续，需再次关闭或移开视线。已发送动作不可撤回，热键可能冲突。

- 更新或结账 Mod 可能破坏交互/确认补丁；连续检查增加开销，提示可能遮挡。异常时停止自动操作、退出回退并核对收入/数量/销量。

## SDK and automated builds / SDK 与自动构建

This Mod pins its compilation SDK in [release.json](release.json). Current baseline: game 2.1.6 / SDK r1. Run `python tools/ci.py build` from the repository root for a game-free build. See [SDK maintenance](../sdk/README.md) and [CI releases](../releases/README.md). Compilation does not replace in-game compatibility checks.

本 Mod 在 [release.json](release.json) 固定编译 SDK，当前基线为游戏 2.1.6 / SDK r1。从仓库根目录运行 `python tools/ci.py build` 可无游戏文件构建。见 [SDK 维护](../sdk/README.md)和 [CI 发布](../releases/README.md)；编译不能代替实机兼容验证。
