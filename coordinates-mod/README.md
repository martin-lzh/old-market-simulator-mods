# Old Market Coordinates

[Risk notes / 风险提示](#risk-notes--风险提示) · [Change Log / 版本记录](CHANGELOG.md)

Displays the local player's world coordinates below the money HUD in Old Market Simulator.

Version 0.1.3 is a prerelease for Old Market Simulator 2.1.6, Unity 2022.3, and BepInEx 5.

## Features

- Shows `X`, `Y`, and `Z` world coordinates with one decimal place.
- Reads the local player's active movement transform every `LateUpdate`, including aboard a ship.
- Formats numbers with the current game/system culture.
- Reuses the native money HUD's TextMesh Pro font, material, size, weight, color, spacing, position, and scale.
- Appears below the money display and does not receive pointer input or change the cursor.
- Starts visible on every launch. Press `F8` to hide or show it for the current session.
- Hides when the local player is unavailable, including in menus, during a disconnect, or before spawning.
- Only reads position data. It does not teleport, call network RPCs, or write to saves.

These are world-space positions, not map grid numbers. The axes remain `X`, `Y`, and `Z` because their compass directions have not been verified.

## Download

Download [`OldMarket.Coordinates-0.1.3.zip`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/coordinates-v0.1.3/OldMarket.Coordinates-0.1.3.zip) from the [`coordinates-v0.1.3` prerelease](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.3).

The release also includes [`SHA256SUMS.txt`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/coordinates-v0.1.3/SHA256SUMS.txt) for package verification.

## Installation

1. Exit the game completely.
2. Make sure BepInEx 5 is installed and working.
3. Extract the archive into the game directory. The DLL should be at `BepInEx/plugins/OldMarket.Coordinates/OldMarket.Coordinates.dll`.
4. Start the game and enter a map. Coordinates should appear below the money display.

To update, exit the game and replace only this mod's DLL. To uninstall, exit the game and remove `OldMarket.Coordinates.dll`; do not remove BepInEx or unrelated plugins.

## Configuration

The first launch creates `BepInEx/config/local.oldmarket.coordinates.cfg`.

| Setting | Default | Description |
| --- | --- | --- |
| `Display.ToggleKey` | `F8` | Unity Input System keyboard key used to toggle the display. `None` disables the hotkey. |
| `Display.Visible` | `true` | Legacy compatibility setting. Version 0.1.3 resets it to `true` at launch, so visibility is not preserved between sessions. |

Edit configuration while the game is closed. Legacy `Left`, `Top`, and `FontSize` settings are no longer used because the display follows the native money HUD.

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

The project targets `netstandard2.1` and uses local game and BepInEx assemblies as read-only references. The script builds this mod independently and creates `outputs/OldMarket.Coordinates-0.1.3.zip`. The package contains only the original DLL, README, and MIT license; it includes no game or loader files and installs nothing.

## Validation and known limits

The source builds and packaging checks pass against Old Market Simulator 2.1.6. Actual in-game UI placement, glyph rendering, live culture changes, movement and jumping updates, pause and relaunch behavior, and multiplayer-client display have not yet been verified for this prerelease.

Other game versions may change the player or HUD APIs. Coordinate orientation has no compass labels. The hotkey is keyboard-only.

## License and attribution

Copyright © 2026 Zhaohan Liu. Original source, tests, and documentation in this directory are available under the [MIT License](LICENSE). Copies or substantial portions must retain the copyright and license notice. Old Market Simulator and third-party components are not covered.

If this mod helps your project, article, or video, please credit **Old Market Coordinates** and link to the [Old Market Simulator Mods repository](https://github.com/martin-lzh/old-market-simulator-mods). This is a voluntary request, not an additional license condition.

---

# Old Market Coordinates（中文）

在 Old Market Simulator 的金钱栏下方显示本地玩家的世界坐标。

0.1.3 是面向 Old Market Simulator 2.1.6、Unity 2022.3 和 BepInEx 5 的预发布版本。

## 功能

- 显示保留一位小数的 `X`、`Y`、`Z` 世界坐标。
- 每帧在 `LateUpdate` 读取本地玩家实际移动对象的位置，在船上也使用世界坐标。
- 数字格式跟随游戏当前使用的系统区域格式。
- 复用原生金钱栏的 TextMesh Pro 字体、材质、字号、字重、颜色、字间距、位置和缩放。
- 显示在金钱栏下方，不接收鼠标输入，也不改变光标。
- 每次启动默认显示。按 `F8` 可在本次运行中隐藏或重新显示。
- 菜单中、断线时或本地角色尚未生成时自动隐藏。
- 只读取位置，不传送角色、不调用网络 RPC，也不写入存档。

这里显示的是世界空间位置，不是地图格子编号。由于尚未确认各轴对应的罗盘方向，因此只标记为 `X`、`Y`、`Z`。

## 下载

从 [`coordinates-v0.1.3` 预发布页面](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.3)下载 [`OldMarket.Coordinates-0.1.3.zip`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/coordinates-v0.1.3/OldMarket.Coordinates-0.1.3.zip)。发布页同时提供 [`SHA256SUMS.txt`](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/coordinates-v0.1.3/SHA256SUMS.txt) 用于校验压缩包。

## 安装

1. 完全退出游戏。
2. 确认 BepInEx 5 已安装并能正常加载插件。
3. 将压缩包解压到游戏目录。DLL 的最终路径应为 `BepInEx/plugins/OldMarket.Coordinates/OldMarket.Coordinates.dll`。
4. 启动游戏并进入地图，坐标应显示在金钱栏下方。

更新时先退出游戏，只替换本 Mod 的 DLL。卸载时退出游戏并移除 `OldMarket.Coordinates.dll`；不要删除 BepInEx 或其他插件。

## 配置

首次运行会生成 `BepInEx/config/local.oldmarket.coordinates.cfg`。

| 设置 | 默认值 | 说明 |
| --- | --- | --- |
| `Display.ToggleKey` | `F8` | 用于切换显示的 Unity Input System 键盘按键。设为 `None` 可禁用快捷键。 |
| `Display.Visible` | `true` | 兼容旧版本的设置。0.1.3 每次启动都会重置为 `true`，因此不会跨运行保存隐藏状态。 |

请在游戏退出后编辑配置。旧版的 `Left`、`Top` 和 `FontSize` 已不再使用，因为显示位置和样式现在跟随原生金钱栏。

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

项目以 `netstandard2.1` 为目标框架，将本机游戏和 BepInEx 程序集作为只读引用。脚本独立构建插件并生成 `outputs/OldMarket.Coordinates-0.1.3.zip`。压缩包只包含原创 DLL、本说明和 MIT 许可证，不包含游戏或加载器文件，也不会自动安装。

## 验证与已知限制

当前源码可以构建，面向 Old Market Simulator 2.1.6 的构建和打包检查均已通过。此预发布版本尚未实机验证游戏内布局、字形、运行中区域格式变化、移动和跳跃刷新、暂停与退出重进，以及联机客户端显示。

其他游戏版本可能改变本插件依赖的玩家或 HUD API。坐标轴未标注罗盘方向。快捷键仅支持键盘。

## 许可证与引用

Copyright © 2026 Zhaohan Liu。本目录的原创源码、测试和文档采用 [MIT License](LICENSE)。复制软件或其重要部分时必须保留版权和许可声明；Old Market Simulator 及第三方组件不受此许可证覆盖。

如果本 Mod 帮助了你的项目、文章或视频，欢迎注明 **Old Market Coordinates** 并链接到 [Old Market Simulator Mods 仓库](https://github.com/martin-lzh/old-market-simulator-mods)。这是自愿引用请求，不是附加许可条件。

## Risk notes / 风险提示

### English

- Saves: reads local position and creates HUD text; no save writes, teleportation, inventory changes or migration. Back up saves before loader changes.

- Multiplayer: no RPC or connection changes; others need not install by design. Two-machine tests are incomplete, so joining friends or accepting connections is not guaranteed with all loader/Mod combinations.

- Game updates/HUD Mods may break anchors or overlap text. F8 may conflict; change ToggleKey or use None. Rounded world coordinates are not map-grid coordinates.

- Per-frame formatting/UI adds overhead without a performance guarantee. Exit before removing this DLL for comparison.

### 中文

- 存档：只读本地位置并创建 HUD，不写存档、不传送、不改库存，无迁移；更换加载器前仍应备份。

- 联机：不发送 RPC 或改连接流程，设计上其他人无需安装。双端未验证，不保证各种加载器/Mod 组合下能加入好友或接受连接。

- 游戏更新/HUD Mod 可能破坏锚点或遮挡文字；F8 可能冲突，可改 ToggleKey 或设 None。取整世界坐标不是地图格号。

- 每帧格式化/UI 增加开销，无性能保证；退出后移除本 DLL 做对照。
