# Auto Checkout

![Auto Checkout](assets/cover.png)

A mod for Old Market Simulator / Old Market Simulator 模组

[English](#english) · [中文](#中文)

## English

In-game acceptance is confirmed by the maintainer. This version is a stable release; see the [validation record](../releases/validation.md).

Let checkout handle the whole table while you hold a key—or keep it running with a toggle. Products are processed one by one, followed by the payment pouch.

[Download 0.1.5](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.5)

### Core features

- Process products one by one, then collect the payment pouch.
- Choose hold-to-run or toggle mode; wait for the next customer when the table is empty.
- Stop when you look away, leave interaction range or open a menu.

<!-- Add gameplay screenshots or GIFs here when available. -->

### How to use

| Action | Control |
| --- | --- |
| Process continuously while held | Hold E for 0.6 seconds |
| Turn continuous checkout on/off | Aim at the checkout and press F9 |
| Use the normal game interaction | Tap E |

Keep looking at the checkout body and stay within reach. When the table is empty, continuous mode waits for the next customer. It does not ring the bell for you. A short E press still does the game's normal action before the hold starts.

Looking away, leaving reach, opening a menu, switching windows or disconnecting stops checkout. In hold mode, releasing E also stops it. If you press F9 during a hold, it switches to toggle mode and keeps going after you release E. Press F9 again to stop; release E before starting another hold.

The 0.1.5 hint uses white for hold instructions; toggle status is white when on and red when off. This hint change is not in the older 0.1.4 download.

### Install

For Old Market Simulator **2.1.6 on Windows**, with **BepInEx 5** installed.

1. Close the game and back up your save and any older plugin.
2. Extract the Mod ZIP into the game folder. The plugin belongs at `BepInEx/plugins/OldMarket.CheckoutAll/OldMarket.CheckoutAll.dll`.
3. Start the game and enter your town.

Choose the Mod ZIP on the release page, not GitHub's **Source code** download. The loader is not included.

### Settings

Close the game before editing `BepInEx/config/local.oldmarket.checkoutall.cfg`, created on first launch.

| Setting | Default | What it does |
| --- | --- | --- |
| `Checkout.HoldSeconds` | 0.6 | Hold delay in seconds, from 0.3 to 2.0 |
| `Checkout.ToggleKey` | F9 | Toggle key; None disables toggle mode and its hint |

Holding is fixed to keyboard E and does not follow game rebinding. Controllers are not supported. Toggle mode starts off each time you launch.

### If checkout stops

Aim at the checkout again, then release and hold E or press F9. Slow multiplayer responses can stop the sequence after about 10 seconds; it will not repeatedly retry the same action. If nearby checkouts or payment pouches cannot be distinguished safely, collect the pouch manually.

These are real sales: completed transactions and collected money stay completed when you stop or remove the Mod. Prices and customer behavior remain as in the game. Tested multiplayer coverage is recorded in the [validation record](../releases/validation.md).

### Updating or removing

Close the game, back up the old DLL and replace it to update. Delete `OldMarket.CheckoutAll.dll` from its plugin folder to uninstall, leaving other plugins and the loader in place. Returning to an older version does not undo sales.

[What changed](CHANGELOG.md) · [Help and feedback](../SUPPORT.md) · [License](LICENSE)

## 中文

维护者已确认实机验收完成，本版本为正式发布版，详见[验证记录](../releases/validation.md)。

长按一个键，就能依次结算桌上的商品并收取钱袋；也可以打开连续结账，不用一直按住。

[下载 0.1.5](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.5)

### 核心功能

- 逐件处理结账台上的商品，然后收取钱袋。
- 支持长按和开关两种模式，桌面清空后等待下一位顾客。
- 移开视角、离开交互距离或打开菜单时停止。

<!-- 后续在此加入实机截图或 GIF。 -->

### 怎么操作

| 操作 | 按键 |
| --- | --- |
| 按住时持续结账 | 长按 E 约 0.6 秒 |
| 开启/关闭连续结账 | 对准结账台按 F9 |
| 保持原版单次交互 | 短按 E |

请让准星对着结账台本体，并保持在可交互距离内。桌上没有商品时会等待下一位顾客，不会自动摇铃。长按开始前，第一次按下 E 仍可能执行原版交互，例如处理一件商品或摇铃。

看向别处、离开交互范围、打开菜单、切换窗口或断线都会停止结账。长按模式下松开 E 也会停止；长按时按 F9 则转为开关模式，松开 E 后继续结账。再次按 F9 停止后，请先松开 E，再开始下一次长按。

0.1.5 的长按提示为白色，连续结账状态开启时为白色、关闭时为红色；旧的 0.1.4 下载包尚不包含这项提示调整。

### 安装

适用于 **Windows 版 Old Market Simulator 2.1.6**，需要先安装 **BepInEx 5**。

1. 退出游戏，备份存档和已有的旧插件。
2. 将 Mod ZIP 解压到游戏目录，确认插件位于 `BepInEx/plugins/OldMarket.CheckoutAll/OldMarket.CheckoutAll.dll`。
3. 启动游戏并进入小镇。

请选发布页里的 Mod ZIP，不要下载 **Source code** 当作插件安装。安装包不含加载器。

### 调整设置

首次运行会生成 `BepInEx/config/local.oldmarket.checkoutall.cfg`，请退出游戏后编辑。

| 设置 | 默认值 | 用途 |
| --- | --- | --- |
| `Checkout.HoldSeconds` | 0.6 | 开始结账前需要按住的秒数，可设 0.3–2.0 |
| `Checkout.ToggleKey` | F9 | 连续结账开关键；None 表示关闭该功能和状态提示 |

长按键固定为键盘 E，不跟随游戏改键，也暂不支持手柄。每次启动时连续结账默认关闭。

### 如果结账停了

重新对准结账台，松开后再次长按 E，或按 F9 即可。联机响应过慢时，等待约 10 秒后可能停止，不会反复重试同一件商品。附近结账台或钱袋挤在一起、无法确认归属时，请手动收取钱袋。

自动结账完成的交易和金币收入会照常保留，停止或卸载不会撤销。商品价格和顾客行为保持游戏原有规则；已测试联机组合见[验证记录](../releases/validation.md)。

### 更新与卸载

更新前退出游戏、备份并替换旧 DLL。卸载时删除插件文件夹中的 `OldMarket.CheckoutAll.dll`，保留加载器和其他插件。退回旧版也不会撤销已经完成的交易。

[版本变化](CHANGELOG.md) · [问题反馈](../SUPPORT.md) · [许可证](LICENSE)
