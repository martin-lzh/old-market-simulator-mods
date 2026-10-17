# Coordinates HUD

![Coordinates HUD](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/coordinates-mod/assets/cover.png?raw=1)

A mod for Old Market Simulator / Old Market Simulator 模组

[English](#english) · [中文](#中文)

## English

See your XYZ position just below the money display, in the same style as the game. The readout updates as you move, including when you are aboard a ship.

[Download 0.1.4](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.4)

### Core features

- Display live X/Y/Z coordinates beneath the money bar using the game’s HUD style.
- Keep the readout updated while walking or travelling aboard a ship.
- Toggle the display with F8; hide it automatically when your character is unavailable.

![XYZ coordinates displayed beneath the money bar in game](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/coordinates-mod/assets/coordinates-hud-gameplay.png?raw=1)

### How to use

- The display is on each time you launch the game.
- Press **F8** to hide or show it.
- Coordinates show one decimal place.
- The display hides itself while you have no character, such as before spawning or after disconnecting.

### Install

Requires Old Market Simulator **2.1.6 on Windows** with **BepInEx 5** installed.

1. Close the game and back up your save and any older copy of this plugin.
2. Download the Mod ZIP from the release page (not GitHub's **Source code**) and extract it into the game folder. The plugin should end up at `BepInEx/plugins/OldMarket.Coordinates/OldMarket.Coordinates.dll`.
3. Start the game.

### Settings

After the first launch, close the game and open `BepInEx/config/local.oldmarket.coordinates.cfg`.

| Setting | Default | What it does |
| --- | --- | --- |
| `Display.ToggleKey` | F8 | Show/hide key; None disables the shortcut |
| `Display.Visible` | true | Resets to visible when you launch the game |

If F8 clashes with another Mod, pick a different keyboard key. The display always follows the money bar, so the old `Left`, `Top` and `FontSize` settings can be deleted.

### Multiplayer

Only players who want to see their coordinates need to install it.

### Updating or removing

To update, close the game, back up the old DLL and replace it. To uninstall, delete `OldMarket.Coordinates.dll` from its plugin folder. Keep the configuration file if you want your shortcut back after reinstalling.

[What changed](CHANGELOG.md) · [Help and feedback](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/SUPPORT.md) · [Development notes](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/coordinates-mod/DEVELOPMENT.md) · [License](LICENSE)

## 中文

在金钱栏下方随时查看自己的 XYZ 坐标，字体和样式与游戏保持一致。坐标会跟随移动更新，在船上也能查看。

[下载 0.1.4](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.4)

### 核心功能

- 在金钱栏下方以原生界面样式显示实时 X/Y/Z 坐标。
- 步行或乘船时持续更新位置。
- 按 F8 显示／隐藏；角色尚未生成或断线后自动隐藏。

![实机画面：金钱栏下方显示 XYZ 坐标](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/coordinates-mod/assets/coordinates-hud-gameplay.png?raw=1)

### 怎么操作

- 每次启动游戏时默认显示。
- 按 **F8** 隐藏或重新显示。
- 坐标保留一位小数。
- 角色尚未出现或断线时，显示会自动隐藏。

### 安装

需要 **Windows 版 Old Market Simulator 2.1.6**，并已安装 **BepInEx 5**。

1. 退出游戏，备份存档和旧版插件。
2. 从发布页下载 Mod ZIP（不是 GitHub 的 **Source code**），解压到游戏目录，插件应位于 `BepInEx/plugins/OldMarket.Coordinates/OldMarket.Coordinates.dll`。
3. 启动游戏。

### 调整设置

首次运行后，退出游戏并打开 `BepInEx/config/local.oldmarket.coordinates.cfg`。

| 设置 | 默认值 | 用途 |
| --- | --- | --- |
| `Display.ToggleKey` | F8 | 显示/隐藏快捷键；None 表示关闭快捷键 |
| `Display.Visible` | true | 每次启动恢复为显示 |

如果 F8 与其他 Mod 冲突，换一个键盘按键即可。显示位置始终跟随金钱栏，旧版的 `Left`、`Top`、`FontSize` 设置可以删掉。

### 和朋友一起玩

想看坐标的玩家自己安装即可。

### 更新与卸载

更新时退出游戏，备份旧 DLL 后替换。卸载时删除插件文件夹中的 `OldMarket.Coordinates.dll`；保留配置文件，重新安装后会沿用原来的快捷键。

[版本变化](CHANGELOG.md) · [问题反馈](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/SUPPORT.md) · [开发说明](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/coordinates-mod/DEVELOPMENT.md) · [许可证](LICENSE)
