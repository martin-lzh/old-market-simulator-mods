# Tree Info 0.1.0

## English

Aim at a planted fruit tree within normal interaction range to see its production season and harvest readiness below the native interaction prompt. Purchased saplings and wild timber trees are excluded. Estimates use the actual tree's growth counter and harvest threshold, assuming daily watering; they do not alter growth. Ripe fruit remains marked ready outside its production season. Other states show days remaining, today's missing watering, an inactive production season, or insufficient days left this season.

Text follows all 13 game languages, with regional aliases and English fallback. Season names use the game's existing translations. No hotkey or configuration is needed.

### Install and remove

Local source version: **0.1.0**. No public download is available yet. Use a locally verified build for Old Market Simulator **2.1.6**, Windows x64 / Unity Mono, with **BepInEx 5**. Build and SDK details: [development notes](DEVELOPMENT.md). Changes: [CHANGELOG](CHANGELOG.md).

Exit the game, back up the existing plugin, and put the package's DLL in `BepInEx/plugins/OldMarket.TreeInfo/`. Keep only one active copy. Restart to load it. To roll back, exit and restore the backed-up DLL; to uninstall, remove this plugin's DLL. Keep other plugins, the loader and saves intact. Packages contain only the original DLL, README, CHANGELOG and MIT license.

### Compatibility and testing

The plugin appends local UI text; it does not change saves, watering, harvesting, growth or RPCs. Other players need not install it by design. Host/client operation and combinations with other subtitle patches still need in-game verification. Missing glyphs, long translated text and competing subtitle changes may affect display. No support for other game versions is established.

Pending in-game checks: newly planted, ripe and harvested trees; dry trees; inactive seasons and season end; looking away and opening menus; resolutions, all language fonts/wrapping and live language switching; host and client synchronized tree state. Automated checks are not in-game acceptance. Translations have not all received native-speaker review. License: [MIT](LICENSE).

## 中文

准星对准正常交互距离内已种植的果树时，在原生交互提示下显示生产季节和成熟状态；购买界面的树苗及野外木材树不在范围内。预测读取当前树的生长计数和成熟阈值，以每天浇水为前提，不改变生长。已成熟果实在非生产季节仍显示可采收；其他状态显示剩余天数、今日未浇水、本季不生产或季末时间不足。

跟随游戏全部 13 种语言，支持地区代码并回退英语，季节名称沿用游戏译文；无需快捷键或配置。

### 安装与卸载

当前本地源码版本 **0.1.0**，尚无公开下载。适用于 Old Market Simulator **2.1.6**、Windows x64 / Unity Mono、**BepInEx 5**。构建与 SDK 见[开发说明](DEVELOPMENT.md)，改动见[版本记录](CHANGELOG.md)。

退出游戏，备份旧插件，将包内 DLL 放入 `BepInEx/plugins/OldMarket.TreeInfo/`，只保留一个启用副本，重新启动游戏加载。回退时退出游戏并恢复备份 DLL；卸载仅删除本插件 DLL，保留其他插件、加载器及存档。包内只含原创 DLL、README、CHANGELOG 和 MIT 许可证。

### 兼容与测试

插件仅追加本地 UI，不修改存档、浇水、采收、生长或 RPC；设计上其他玩家无需安装。房主/客人及与其他副标题插件的组合仍待实机验证。可能出现缺字、长译文换行及提示争用，尚未确认其他游戏版本兼容性。

待测：新树、成熟树、采收后树、未浇水、非生产季节、季末不足、移开视角、打开菜单、不同分辨率、各语种字体与换行、即时切换语言，以及房主和客人的同步树木状态。自动检查不能代替实机验收，译文尚未全部经过母语审校。[MIT 许可证](LICENSE)。
