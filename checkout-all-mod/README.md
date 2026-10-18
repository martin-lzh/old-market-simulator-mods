# Auto Checkout

![Auto Checkout](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/checkout-all-mod/assets/cover.png?raw=1)

A mod for Old Market Simulator / Old Market Simulator 模组

[English](#english) · [中文](#中文)

## English

Let checkout handle the whole table while you hold a key—or keep it running with a toggle. Products are processed one by one, followed by the payment pouch.

[Download 0.1.6](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.6)

### Core features

- Process products one by one, then collect the payment pouch.
- Choose hold-to-run or toggle mode; wait for the next customer when the table is empty.
- Stop when you look away, leave interaction range or open a menu.

### How to use

| Action | Control |
| --- | --- |
| Process continuously while held | Hold E for 0.6 seconds |
| Turn continuous checkout on/off | Aim at the checkout and press F9 |
| Use the normal game interaction | Tap E |

![In-game checkout hints for holding E and toggling with F9; toggle mode is off](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/checkout-all-mod/assets/auto-checkout-gameplay.png?raw=1)

![In-game continuous checkout enabled, with F9 to turn it off](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/checkout-all-mod/assets/continuous-checkout-on-gameplay.png?raw=1)

Aim at the checkout and stay within reach. When the table is empty, continuous mode waits for the next customer; ringing the bell is still up to you. Tapping E still does the game's normal action.

Checkout stops when you look away, step out of reach, open a menu, switch windows or disconnect. In hold mode, releasing E also stops it. Pressing F9 during a hold switches to toggle mode, so checkout keeps going after you release E. Press F9 again to stop, and release E before starting a new hold.

The hold hint is white. The toggle status is white when on and red when off.

### Install

Requires Old Market Simulator **2.1.6 on Windows** with **BepInEx 5** installed.

1. Close the game and back up your save and any older copy of this plugin.
2. Download the Mod ZIP from the release page (not GitHub's **Source code**) and extract it into the game folder. The plugin should end up at `BepInEx/plugins/OldMarket.CheckoutAll/OldMarket.CheckoutAll.dll`.
3. Make sure that is the only copy of the DLL under `BepInEx/plugins`, then start the game.

### Settings

Close the game before editing `BepInEx/config/local.oldmarket.checkoutall.cfg`, created on first launch.

| Setting | Default | What it does |
| --- | --- | --- |
| `Checkout.HoldSeconds` | 0.6 | Hold delay in seconds, from 0.3 to 2.0 |
| `Checkout.ToggleKey` | F9 | Toggle key; None disables toggle mode and its hint |

Holding always uses keyboard E, regardless of the game's key bindings. Toggle mode starts off each time you launch.

### If checkout stops

Aim at the checkout again, then hold E or press F9. On a slow multiplayer connection, checkout stops after about 10 seconds without a response instead of retrying. If several checkouts or pouches are close together, collect the pouch by hand.

### Multiplayer

Only players who want automatic checkout need to install it; the host and other players don't. Don't have another player or an employee work the same checkout at the same time.

### Updating or removing

To update, close the game, back up the old DLL and replace it. To uninstall, delete `OldMarket.CheckoutAll.dll` from its plugin folder.

[What changed](CHANGELOG.md) · [Help and feedback](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/SUPPORT.md) · [Development notes](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/checkout-all-mod/DEVELOPMENT.md) · [Translations](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/checkout-all-mod/localization/README.md) · [License](LICENSE)

## 中文

长按一个键，就能依次结算桌上的商品并收取钱袋；也可以打开连续结账，不用一直按住。

[下载 0.1.6](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.6)

### 核心功能

- 逐件处理结账台上的商品，然后收取钱袋。
- 支持长按和开关两种模式，桌面清空后等待下一位顾客。
- 移开视角、离开交互距离或打开菜单时停止。

### 怎么操作

| 操作 | 按键 |
| --- | --- |
| 按住时持续结账 | 长按 E 约 0.6 秒 |
| 开启/关闭连续结账 | 对准结账台按 F9 |
| 保持原版单次交互 | 短按 E |

![实机画面：长按 E 连续结账及 F9 开关提示，当前连续结账关闭](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/checkout-all-mod/assets/auto-checkout-gameplay.png?raw=1)

![实机画面：连续结账已开启，按 F9 关闭](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/checkout-all-mod/assets/continuous-checkout-on-gameplay.png?raw=1)

准星对着结账台，站在可交互距离内即可。桌上没有商品时会等待下一位顾客，摇铃仍由你自己操作。短按 E 仍是游戏原本的交互。

看向别处、走出交互范围、打开菜单、切换窗口或断线时会停止结账；长按模式下松开 E 也会停止。长按时按 F9 会切换为开关模式，松开 E 后继续结账。再按一次 F9 停止，然后先松开 E 再开始下一次长按。

长按提示为白色；连续结账状态开启时为白色，关闭时为红色。

### 安装

需要 **Windows 版 Old Market Simulator 2.1.6**，并已安装 **BepInEx 5**。

1. 退出游戏，备份存档和旧版插件。
2. 从发布页下载 Mod ZIP（不是 GitHub 的 **Source code**），解压到游戏目录，插件应位于 `BepInEx/plugins/OldMarket.CheckoutAll/OldMarket.CheckoutAll.dll`。
3. 确认 `BepInEx/plugins` 下只有这一份 DLL，然后启动游戏。

### 调整设置

首次运行会生成 `BepInEx/config/local.oldmarket.checkoutall.cfg`，请退出游戏后编辑。

| 设置 | 默认值 | 用途 |
| --- | --- | --- |
| `Checkout.HoldSeconds` | 0.6 | 开始结账前需要按住的秒数，可设 0.3–2.0 |
| `Checkout.ToggleKey` | F9 | 连续结账开关键；设为 None 可关闭该功能和状态提示 |

长按键固定为键盘 E，不跟随游戏改键。每次启动时连续结账默认关闭。

### 如果结账停了

重新对准结账台，再长按 E 或按 F9 即可。联机网络较慢时，约 10 秒没有响应会自动停下，不会反复重试。附近有多个结账台或钱袋挤在一起时，请手动收取钱袋。

### 和朋友一起玩

想用自动结账的玩家自己安装即可，房主和其他玩家不需要安装。请不要让其他玩家或员工同时操作同一个结账台。

### 更新与卸载

更新时退出游戏，备份旧 DLL 后替换。卸载时删除插件文件夹中的 `OldMarket.CheckoutAll.dll`。

[版本变化](CHANGELOG.md) · [问题反馈](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/SUPPORT.md) · [开发说明](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/checkout-all-mod/DEVELOPMENT.md) · [翻译说明](https://github.com/martin-lzh/old-market-simulator-mods/blob/main/checkout-all-mod/localization/README.md) · [许可证](LICENSE)
