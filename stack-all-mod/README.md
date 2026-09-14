# Old Market Stack All

[English](#english) · [中文](#中文)

## English

Carry more without losing track of your baskets and boxes. Each product slot can hold up to 64 containers; tools stay separate.

[Download 0.2.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/stack-all-v0.2.1) · 0.2.2 is being prepared and is not on the release page yet.

## Install

For Old Market Simulator **2.1.6 on Windows**, with **BepInEx 5** installed.

1. Close the game and back up your save and any older plugin.
2. Extract the Mod ZIP into the game folder. The plugin belongs at `BepInEx/plugins/OldMarket.StackAll/OldMarket.StackAll.dll`.
3. Start the game and enter your town.

Choose the Mod ZIP on the release page, not GitHub's **Source code** download. The loader is not included.

## How stacks work

The bottom-left number is the amount of goods; the bottom-right is the number of containers. Two baskets with 24 goods each show **48 goods / 2 containers**. Empty baskets still count as containers. Whole fish and other single-use, one-unit products show only their quantity on the right. One- and two-digit quantities keep the native font size; larger goods totals fit in the left half.

Each container keeps its contents, cost and freshness. Compatible goods use the game's usual merging and expiry rules. Other non-tool items stack up to 64. A 65th container goes into another available slot; if there is no room, it stays near you.

## Controls

- Tap Q to drop one item or one complete container; tap F to throw one. Mouse placement also handles one at a time.
- Hold Q or F to continue through the selected slot, starting after about 0.6 seconds.
- These actions follow your game key bindings. Switching slots/items, opening a menu, pausing, switching windows or pressing both actions stops the sequence.

There are no settings to adjust before playing.

## Multiplayer

**Everyone in the room, including the host, needs the same Stack All version.** Please compare versions before joining; the Mod does not check this for you. Avoid combining it with other Mods that change inventory stacking. Visual, local in-game and two-computer multiplayer testing of the preceding build is complete. The subsequent fish-quantity and font-size fix still needs in-game visual confirmation.

## Backups and removal

**Back up your save before installing. Saves containing extra containers need Stack All 0.2.0 or a later compatible version. Do not simply remove the Mod while those containers remain.**

To remove it or return to an incompatible older version:

1. With Stack All still installed, take out extra containers until each product slot has at most one.
2. Split other stacks to the game's normal limits.
3. Save normally, then exit the game.
4. Back up that save and remove the plugin DLL. Keep the previous backup until you have checked the result.

Old 0.1.0 overfilled baskets still count as one basket. Empty them or use the game's V transfer to bring them back within normal capacity before removing the Mod. Removing a DLL does not undo inventory changes already saved.

For a compatible update, close the game and back up your save and old DLL before replacing the plugin. Leave your loader and other plugins in place.


[What changed](CHANGELOG.md) · [Help and feedback](../SUPPORT.md) · [License](LICENSE)

## 中文

让一个格子装下更多篮子和箱子，又能清楚看见实际数量。每个商品格最多容纳 64 个容器，工具仍然单独放置。

[下载 0.2.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/stack-all-v0.2.1) · 0.2.2 正在准备，发布页暂时仍是 0.2.1。

## 安装

适用于 **Windows 版 Old Market Simulator 2.1.6**，需要先安装 **BepInEx 5**。

1. 退出游戏，备份存档和已有的旧插件。
2. 将 Mod ZIP 解压到游戏目录，确认插件位于 `BepInEx/plugins/OldMarket.StackAll/OldMarket.StackAll.dll`。
3. 启动游戏并进入小镇。

请选发布页里的 Mod ZIP，不要下载 **Source code** 当作插件安装。安装包不含加载器。

## 数量怎么看

格子左下角是商品总数，右下角是容器数。例如两个各装 24 件商品的篮子，会显示 **48 件商品 / 2 个容器**。空篮子也会计入容器数。整条鱼等容量为 1、用完消失的单件商品只显示右侧数量；一位数和两位数保持原生字号，更大的商品总量会在左半格内缩放。

每个容器保留自己的内容、成本和保鲜信息，兼容商品按游戏原有的合并与过期规则处理。其他非工具物品最多堆叠 64 个。第 65 个容器会放到其他空格；没有空间时会留在玩家附近。

## 怎么操作

- 短按 Q 丢下一个物品或一个完整容器，短按 F 投掷一个；鼠标放置也是一次一个。
- 长按 Q 或 F，约 0.6 秒后会继续逐个处理当前格。
- 按键跟随游戏设置。切换格子或物品、打开菜单、暂停、切换窗口或同时按两个动作都会停止。

不用额外调整设置，安装后即可使用。

## 和朋友一起玩

**房主和所有玩家都需要安装相同版本的 Stack All。**加入房间前请互相确认，Mod 不会自动检查版本。尽量不要同时使用其他修改库存堆叠的 Mod。修复前构建的视觉、本地实机及双实机联机测试已完成；随后新增的鱼类数量和字号修复仍待实机视觉确认。

## 备份与卸载

**安装前请备份存档。含额外容器的存档需要 Stack All 0.2.0 或更新的兼容版本，不能在额外容器还留着时直接卸载。**

如果想卸载，或退回不兼容的旧版：

1. 先保持 Stack All 安装着，把每个商品格的额外容器取出，直到每格最多一个。
2. 将其他堆叠拆分到游戏原有上限以内。
3. 正常保存并退出游戏。
4. 备份整理后的存档，再移除插件 DLL；确认结果前保留原来的备份。

旧版 0.1.0 的超量篮子仍算一个篮子，卸载前请先用完内容，或用游戏的 V 转移功能恢复正常容量。删除 DLL 不会撤销已保存的库存变化。

更新到兼容版本时，也请退出游戏，备份存档和旧 DLL 后再替换插件，保留加载器及其他插件。


[版本变化](CHANGELOG.md) · [问题反馈](../SUPPORT.md) · [许可证](LICENSE)
