# 菜市场模拟器：7 款实用 Mod 合集与安装指南

我是这七款 Mod 的作者。这篇指南把下载入口、安装方法和常用操作放在一起：从连续结账、整理背包，到查成本、定价格，再到看地图和果树状态，可以按自己的需要挑选安装。

本文对应 **Windows x64 / Unity Mono 版 Old Market Simulator 2.1.6**。下列版本均已正式发布；不代表已经适配其他游戏版本、主机或移动端。界面跟随游戏语言，支持中文，无需另装汉化包；坐标使用通用的 X/Y/Z 标签。

## 先选你需要的功能

| 想解决的问题 | Mod | 本文版本 |
| --- | --- | --- |
| 反复按键结账太累 | Auto Checkout／自动结账 | 0.1.5 |
| 篮子和箱子占满背包 | Better Stacking／物品叠放 | 0.3.0 |
| 想知道制作和进货划不划算 | Profit Insights／成本与利润信息 | 0.5.2 |
| 想按顾客接受率调整售价 | Smart Pricing／智能定价 | 0.1.2 |
| 找不到店铺、入口或自己记下的位置 | Map & Compass／地图与罗盘 | 0.2.0 |
| 想和朋友交流准确位置 | Coordinates HUD／坐标显示 | 0.1.4 |
| 想知道果树何时成熟 | Tree Harvest Helper／果树信息 | 0.1.0 |

这些是七个独立下载，不是必须一起安装的整合包。地图与罗盘也不依赖坐标显示。

## 安装准备：BepInEx 5

本指南统一使用 **BepInEx 5 的 Windows x64 版本**，这样可以按需组合全部七款 Mod。加载器不包含在 Mod 安装包中。

1. 退出游戏，备份存档及已有的插件、配置。
2. 打开 [BepInEx 5 官方发布页](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2)，选择 Windows x64 安装包。不要选 x86、Linux、macOS、BepInEx 6 或 IL2CPP 包。如果已有正常工作的 BepInEx 5，无需重复覆盖加载器。
3. 在 Steam 库中打开本游戏的本地文件目录，将加载器压缩包的内容解压到游戏程序所在目录；不要额外套一层压缩包同名文件夹。
4. 启动一次游戏，然后退出。确认生成了 `BepInEx/config` 和 `BepInEx/LogOutput.log`。加载器的详细步骤见 [BepInEx 官方安装说明](https://docs.bepinex.dev/articles/user_guide/installation/index.html)。
5. 从下方各 Mod 的 Nexus 或 GitHub 页面下载对应版本的 **Mod ZIP**，解压到游戏目录，保留包内目录结构。GitHub 的 **Source code** 是源码，不是可安装插件。
6. 检查每个插件只保留一个启用副本，再启动游戏。

以智能定价为例，插件应位于：

`BepInEx/plugins/OldMarket.PriceProbability/OldMarket.PriceProbability.dll`

其他 Mod 也在 `BepInEx/plugins/` 下使用各自的 `OldMarket.*` 文件夹。**地图 Mod 要保留随包的 maps 文件夹，不能只复制 DLL。**

Profit Insights 另有 MelonLoader 0.7.3 版本，供已经使用该加载器的玩家单独选择。两个版本二选一，不要同时启用 BepInEx 和 MelonLoader；其余六款按本指南使用 BepInEx。

## Auto Checkout：自动结账

逐件处理结账台上的商品，再收取钱袋，减少重复按键。

- **长按 E 约 0.6 秒：**按住期间连续结账，松开停止。
- **对准结账台按 F9：**开启／关闭连续结账，开启后不用一直按住。
- **短按 E：**仍执行原版单次交互。

![连续结账开启，按 F9 可关闭](../../checkout-all-mod/assets/continuous-checkout-on-gameplay.png)

准星需要对着结账台本体，并保持在交互距离内。桌面清空后会等待下一位顾客，不会自动摇铃。看向别处、走远、打开菜单、切换窗口或断线都会停止；重新对准后再长按 E 或按 F9 即可。

长按键固定为键盘 E，暂不跟随游戏改键，也不支持手柄。已完成的销售和收款会照常保存，停止或卸载不会撤销交易。

[Nexus 下载](https://www.nexusmods.com/oldmarketsimulator/mods/46) · [GitHub 0.1.5](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.5)

## Better Stacking：物品叠放与丢空盒

每格最多容纳 **64 个容器或种子包**，各自保留内容、成本和保鲜信息；工具仍单独放置。

格子左下角显示商品总数，右下角显示实际容器数。例如下图是 **2640 件商品、46 个容器**，空盒也计入容器数。

![商品总量与实际容器数分别显示](../../stack-all-mod/assets/stack-counts-gameplay.png)

- **Q：**丢下一个物品或一个完整容器。
- **F：**投掷一个物品或一个完整容器。
- **G：**只丢出当前格的一个空篮／空盒，保留满盒和半满盒。
- **长按 Q、F 或 G：**约 0.6 秒后连续操作，直到松开或没有符合条件的物品。

![原生样式的操作提示与长按说明](../../stack-all-mod/assets/stack-controls-gameplay.png)

没有空盒时，G 的提示会变暗。G 只处理可复用容器，不处理种子包或一次性包装。Q/F 跟随游戏设置，G 是固定键。切换格子、打开菜单或切换窗口等操作会停止连续动作。

**联机时房主和所有玩家都要安装相同版本。卸载前必须整理库存，具体步骤见下方“联机、更新与卸载”。** 尽量不要与其他修改库存堆叠的 Mod 混用。

[Nexus 下载](https://www.nexusmods.com/oldmarketsimulator/mods/47) · [GitHub 0.3.0](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/stack-all-v0.3.0)

## Profit Insights：成本与利润信息

在原有界面补充经营数据，方便比较商品。

- **每日销售报告：**打开“仅计原料”后按原料估算成本和利润；关闭恢复游戏原有统计，下次会记住选择。
- **配方详情：**查看整批与单件的原料成本、产量，以及按当天建议售价计算的预计利润。
- **码头订购页：**查看整包进货成本、单件建议售价与预计利润。

![配方页的原料成本和预计利润](../../material-cost-mod/assets/recipe-cost-profit-gameplay.png)

![订单页的进货成本和预计利润](../../material-cost-mod/assets/order-cost-profit-gameplay.png)

这里是**材料成本参考，不是店铺净利润或历史采购账本**：原料按当天价格估算，不计人工、租金、工具等经营支出；加工品按直接原料估算，不逐层展开配方。水果、蜂蜜、牛奶、鸡蛋和钓到的鱼等可重复获取产物，在该口径下的原料成本可能为零。

显示的百分比是**利润 ÷ 成本**，不是利润 ÷ 销售收入。成本为零时无法计算的百分比显示“—”。Mod 只增加显示，不改变金币、库存或售价。

[Nexus 下载](https://www.nexusmods.com/oldmarketsimulator/mods/45) · [GitHub 0.5.2](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.2)

## Smart Pricing：智能定价

打开商品原有的定价面板，即可查看顾客接受当前售价的估算概率。

1. 输入售价查看概率，或拖动概率滑条，让 Mod 计算对应售价并写入输入框。
2. 点击 Anchor／定价规则，在“关闭”“固定售价”“固定概率”之间选择。
3. **点击游戏原有的确认按钮保存。**直接关闭窗口会丢弃尚未确认的规则修改。

![拖动接受率后同步调整售价，并固定目标概率](../../price-probability-mod/assets/fixed-probability-gameplay.png)

“固定概率”会在批发价变化后调整售价，尽量维持目标接受率；100% 目标使用当天建议售价。售价取整数，实际接受率可能与目标略有偏差。

**接受率表示顾客找到商品后，不会因价格过高而放弃的可能性，不是当天卖光库存的概率。**需求、客流、保鲜和库存依然影响销量。

联机时只有房主能保存和执行自动定价规则；客人安装后可以查看概率。房主启用的规则会优先于客人的改价，未安装 Mod 的客人看不到规则状态。规则按存档槽保存，更换同一槽里的存档前应清除旧规则。

[Nexus 下载](https://www.nexusmods.com/oldmarketsimulator/mods/48) · [GitHub 0.1.2](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/price-probability-v0.1.2)

## Map & Compass：地图、罗盘与个人标记

加入小地图、可缩放大地图、罗盘与场景内目标指引。安装包包含东方小镇、海岛和罗马地图，罗马支持七个独立传送区域。

- **M：**打开／关闭大地图；Esc 也能关闭。
- **滚轮或 + / − 按钮：**缩放大地图；左键拖动平移。
- **主键盘 − / =：**缩小／放大小地图。
- **右键地图空白处：**添加个人标记。
- **左键个人标记：**编辑名称、颜色和图标，再点“设为目标”。
- **右键个人标记：**删除标记；也可用编辑区的删除按钮。
- **点击地点图标或名称：**设置店铺等地点为目标。

![罗马地图与地点标签](../../navigation-mod/assets/rome-map-gameplay.png)

![场景内显示目标方向和距离](../../navigation-mod/assets/target-tracking-gameplay.png)

大地图中的朝向按钮切换小地图的“固定正北／随视角转动”，大地图本身始终正北朝上。**打开地图不会暂停游戏。**目标提供方向和距离，不是自动寻路路线。

东方小镇目前包含镇区与中央市场解锁状态，外围地形未打包；海岛是静态地表概览。地图不会自动重画玩家摆放的物品。

个人标记按存档、区域分别保存，不同步给其他玩家。加入好友房间时默认只保留本次连接的标记；要跨次保留，可在配置中为该好友存档设置独立的 `Markers.RemoteProfile`。

[Nexus 下载](https://www.nexusmods.com/oldmarketsimulator/mods/49) · [GitHub 0.2.0](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/navigation-v0.2.0)

## Coordinates HUD：坐标显示

在金钱栏下方显示实时 XYZ 坐标，字体与游戏界面一致，步行和乘船时都会更新。

**按 F8 显示／隐藏。**每次启动游戏默认显示；坐标保留一位小数，表示实际位置。

![金钱栏下方的 XYZ 坐标](../../coordinates-mod/assets/coordinates-hud-gameplay.png)

该 Mod 只显示信息，不移动角色，也不改变库存或存档，不要求其他玩家安装。它与 Map & Compass 可以独立使用；一起用时建议保持地图 Mod 自带的坐标显示关闭，避免重复。

[Nexus 下载](https://www.nexusmods.com/oldmarketsimulator/mods/44) · [GitHub 0.1.4](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.4)

## Tree Harvest Helper：果树信息

走近已经种植的果树，用准星对准它，原生交互提示下方会显示生产季节、成熟状态或预计剩余天数，无需快捷键。

![果树的生产季节与成熟采收状态](../../tree-info-mod/assets/tree-harvest-gameplay.png)

还会提示当天未浇水、本季不生产，或剩余季节时间不足。预测以每日浇水为前提，**不会加快生长或自动浇水**。已成熟果实在非生产季节仍可采收。

购买界面的树苗和野外木材树不在显示范围内。信息仅在本机显示，不要求其他玩家安装。

[Nexus 下载](https://www.nexusmods.com/oldmarketsimulator/mods/50) · [GitHub 0.1.0](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/tree-info-v0.1.0)

## 常用按键速查

| 按键／操作 | 功能 |
| --- | --- |
| F8 | 显示／隐藏坐标 |
| 长按 E | 自动结账，松开停止 |
| F9 | 对准结账台时开启／关闭连续结账 |
| Q / F | 丢下／投掷一个，长按连续操作 |
| G | 丢当前格的空容器，长按连续丢 |
| M | 打开／关闭大地图 |
| 主键盘 − / = | 缩小／放大小地图 |
| 大地图滚轮／左键拖动 | 缩放／平移地图 |
| 右键地图空白处 | 添加个人标记 |

以上为默认操作。Q/F 跟随游戏改键；G 和自动结账的长按 E 为固定键。F8、F9 和 M 可在各自 Mod 的配置中调整。

## 联机、更新与卸载

**Better Stacking 的联机和卸载要求最需要留意：**

1. 房主和所有玩家使用同一版本，加入房间前自行核对，Mod 不会自动检查版本。
2. 想卸载时，先保持 Mod 安装，把每格额外容器取出，整理到每格最多一个；种子包也整理到每格最多一包。
3. 将其他堆叠拆分到原版上限。旧构建留下的超容量种子包，应先用完再退回原版；其他旧版存档迁移情形请查看该 Mod 的完整说明。
4. 正常保存、退出游戏，备份整理后的存档，再移除插件。**不要在还有额外容器时直接删 DLL 后读取存档。**

其余 Mod 更新时，同样先退出游戏、备份旧插件和设置再替换，避免同一 DLL 留下多个副本。地图更新要连同随包地图一起替换，保留 `BepInEx/config/OldMarket.Navigation/` 可留下个人标记。

卸载自动结账不会撤销交易；卸载智能定价会停止自动调价，但已设置的售价仍保留。只移除目标 Mod 的文件，保留其他插件和加载器。

## 常见问题与反馈

**安装后没有效果？**

确认游戏版本和 BepInEx 5 Windows x64 对应；插件应在 `BepInEx/plugins/`，而不是多套一层文件夹，也不要把源码 ZIP 当插件。先检查 `BepInEx/LogOutput.log` 是否生成、是否加载了对应插件。

**地图没有底图？**

检查 `BepInEx/plugins/OldMarket.Navigation/maps/` 是否完整，重新解压该 Mod 的整个安装包后重启。

**连续结账突然停了？**

先重新对准结账台并站近，松开后再长按 E 或按 F9。移开视角、打开菜单和切换窗口都会停止；联机响应过慢或钱袋归属不明确时也可能停止，可先手动完成当前交互。

**想反馈问题或建议？**

欢迎在对应 Nexus 页面留言，或到 [GitHub Issues](https://github.com/martin-lzh/old-market-simulator-mods/issues) 用中文反馈。请说明游戏版本、Mod 版本、单机／房主／客人身份、复现步骤，并附必要的错误片段或截图；发布前检查日志中的用户名、路径、房间码等隐私。

作者：LZHSimulators。感谢 Old Market Simulator 开发团队，以及 BepInEx、Harmony 和 MelonLoader 的维护者。地图图标使用 Phosphor（MIT）。这些是非官方 Mod，与游戏制作方没有隶属关系。

[项目源码与完整说明](https://github.com/martin-lzh/old-market-simulator-mods) · [各版本 GitHub 下载](https://github.com/martin-lzh/old-market-simulator-mods/releases)
