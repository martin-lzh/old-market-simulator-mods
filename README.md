# Old Market Simulator Mods

[English](#english) · [中文](#中文)

## English

Five released unofficial mods and an experimental Navigation mod for **Old Market Simulator**, with source code, tests, and build scripts. Each mod is versioned independently.

### Downloads

| Mod | Release | Features |
| --- | --- | --- |
| [Material Cost](material-cost-mod/README.md) | [0.5.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.1) | Material-cost estimates in daily reports, recipes, and purchase-profit details in orders |
| [Coordinates](coordinates-mod/README.md) | [0.1.3](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.3) | World coordinates beneath the money HUD; F8 toggle |
| [Checkout All](checkout-all-mod/README.md) | 0.1.5 pending; [0.1.4 download](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.4) | Continuous checkout by holding E or toggling F9 |
| [Stack All](stack-all-mod/README.md) | [0.2.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/stack-all-v0.2.1) | Up to 64 containers per inventory slot, separate item/container counts, repeated drop/throw |
| [Price Probability](price-probability-mod/README.md) | [0.1.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/price-probability-v0.1.1) | Price-acceptance estimates and host-controlled fixed-price/probability rules |

Get installable ZIPs from the linked [GitHub Releases](https://github.com/martin-lzh/old-market-simulator-mods/releases). GitHub's **Source code** archives contain source, not ready-to-install plugins. Each release includes installation notes and `SHA256SUMS.txt`. Current versions are **prereleases**: automated checks passed, but in-game acceptance is incomplete.

### Installation and compatibility

[Navigation 0.1.4](navigation-mod/README.md) is a local-build experiment with a default bottom-left minimap, M-key map, compass, personal target markers and native HUD reflow. It is included in SDK r7 CI builds and the automated prerelease scope, but no Navigation GitHub release has been published yet. Its original UI artwork is embedded in the DLL; scene-derived maps remain local and are not distributed here. See its README for build commands, unlock-state map layers and validation limits.

The build/reference environment is Old Market Simulator **2.1.6**, Windows x64, Unity Mono. Most packages require an existing **BepInEx 5** installation. Material Cost also provides a **MelonLoader 0.7.3** package; choose one loader variant. The experimental standalone entry remains available as source for local testing and has no public release package.

Exit the game and back up the old plugin before installing. Follow the individual mod's README and preserve other plugins and loader settings. Plugin ZIPs include only the original plugin DLL, README, and MIT LICENSE; game files and loaders are not bundled.

**Stack All requires the host and all players to use the same version.** Its extra container records are saved through the game's normal save process. Before disabling or downgrading it, follow the [removal procedure](stack-all-mod/README.md); loading such a save without a compatible plugin is unsafe. Other multiplayer restrictions are documented per mod.

### Current support and risks

As of 2026-09-13, the latest supported build/reference baseline is **Old Market Simulator 2.1.6**, Windows x64 / Unity Mono 2022.3.62f3. This is not a claim of completed in-game testing or support for subsequent game updates. Assembly-CSharp.dll SHA256: `FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296`. Current per-Mod versions and downloads are listed above.

Installing a Mod or loader may cause save incompatibility, failed connections to/from friends, desynchronization, startup failures, stuttering or crashes. Risks differ by code path and installed combination; these are possible outcomes, not confirmed faults in every Mod.

| Risk | What to check |
| --- | --- |
| Saves and rollback | Back up saves/plugins/configuration. Stack All's extra records need a compatible plugin; deleting DLLs does not undo saved inventory, prices or transactions. |
| Multiplayer | Verify both sides' game/Mod versions and host/client behavior. Stack All requires matching versions for everyone; display-only Mods do not automatically require all players to install. |
| Updates and conflicts | Game APIs, loader entry points, duplicate DLLs and other Mods affecting the same UI/logic may conflict. |
| Performance and stability | Resource queries, per-frame UI, inventory synchronization and automation add work. Loader-only stuttering has occurred in local comparisons; no fix is guaranteed. |
| Accidental actions and economy | Holds/toggles and price anchors perform real actions. Stopping or uninstalling does not reverse completed actions. |
| Display and estimates | Text may overlap or lack glyphs. Material costs, acceptance rates and profit estimates are not actual net profit or daily sales guarantees. |

Exit before installation/upgrades/removal and test backup copies first. Follow each Mod's risk and rollback instructions; do not load a Stack All extended save in vanilla just because an update disables the plugin. See the independent change logs for release downloads and compatibility:

- [Material Cost Change Log](material-cost-mod/CHANGELOG.md)
- [Coordinates Change Log](coordinates-mod/CHANGELOG.md)
- [Checkout All Change Log](checkout-all-mod/CHANGELOG.md)
- [Stack All Change Log](stack-all-mod/CHANGELOG.md)
- [Price Probability Change Log](price-probability-mod/CHANGELOG.md)

### Languages

UI follows all 13 configured game languages: English, Simplified Chinese, Traditional Chinese, French, German, Italian, Spanish, Portuguese, Japanese, Korean, Russian, Turkish, and Ukrainian. Existing action names and labels are read directly from the game's `Translations` table. Original supplemental messages are embedded in each applicable DLL; no separate language pack or shared runtime DLL is needed.

Coordinates uses universal X/Y/Z labels and native HUD styling. Config identifiers and developer logs remain stable. Supplemental messages fall back to English for unsupported locales. Each mod owns its localization code, messages, and tests inside its own directory; see that mod’s README for checks and limitations. Translations have not been reviewed by native speakers of every supported language; glyphs, wrapping, and live language changes still need in-game verification.

### Build from source

Clone the repository, or take the directory for the mod you want to build. Each mod is self-contained and requires no sibling mod or root localization project. You need PowerShell, a .NET SDK capable of building .NET 8 projects, a local game installation, and the selected loader's reference assemblies. Builds read game files and write only local `work/`, `outputs/`, and build-cache directories; they do not install mods.

Run from the repository root, replacing the example game path:

```powershell
./material-cost-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./coordinates-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./checkout-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./stack-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./price-probability-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

ZIPs appear in `outputs/`. Each mod with translatable UI runs its own localization checks and applicable feature tests. Coordinates has no translatable messages and no localization-project dependency. Passing builds and tests do not establish in-game UI, save, or multiplayer correctness. See [release management](releases/README.md) for independent tags, release notes, and artifact checks.

### Contributing

See [CONTRIBUTING](CONTRIBUTING.md#english) for setup, tests and PRs targeting main. Chinese and English contributions are welcome. Also see [Support](SUPPORT.md#english), [Security reporting](SECURITY.md#english), and the [Code of conduct](CODE_OF_CONDUCT.md#english). Bilingual Issue/PR templates are included. The existing root MIT license applies to original project contributions.

### License and attribution

Original source, tests, build scripts, translations, and documentation are under the [MIT License](LICENSE), **Copyright (c) 2026 Zhaohan Liu**. Each mod also includes its own LICENSE. Game and third-party components retain their own licenses. Decompiled game source, game assets, saves, credentials, and personal configuration are not published.

If this work helps your project, article, or video, please cite the project and relevant mod/version or commit:

> Zhaohan Liu. Old Market Simulator Mods — mod name, version or commit. https://github.com/martin-lzh/old-market-simulator-mods

Citation is requested, not an additional MIT condition. A project link does not replace the copyright and permission notices required by MIT when copying the software or substantial portions of it.

## 中文

本仓库提供 **Old Market Simulator** 的五个已发布非官方 Mod，以及实验中的 Navigation Mod，包含源码、测试和构建脚本。各 Mod 独立管理版本。

### 下载

[Navigation 0.1.4](navigation-mod/README.md) 为本地构建实验版，提供默认左下小地图、M 键大地图、罗盘、个人目标标记与原生 HUD 避让，已接入 SDK r7 的 CI 构建与自动预发布范围，但尚无 Navigation GitHub Release。原创 UI 美术资源嵌入 DLL；场景派生地图仅留本机，不在此分发。构建方式、解锁状态叠层和验证范围见其说明。

| Mod | 发布版本 | 功能 |
| --- | --- | --- |
| [Material Cost](material-cost-mod/README.md) | [0.5.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.1) | 日报与配方原料成本、订购页进货成本和预计利润 |
| [Coordinates](coordinates-mod/README.md) | [0.1.3](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.3) | 金钱栏下方显示世界坐标，F8 切换 |
| [Checkout All](checkout-all-mod/README.md) | 0.1.5 待发布；[0.1.4 下载](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.4) | 长按 E 或按 F9 开关连续结账 |
| [Stack All](stack-all-mod/README.md) | [0.2.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/stack-all-v0.2.1) | 每格最多 64 个容器，分别显示商品与容器数，支持连续放下/扔出 |
| [Price Probability](price-probability-mod/README.md) | [0.1.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/price-probability-v0.1.1) | 定价接受概率，以及房主控制的固定售价/概率规则 |

从上表对应的 [GitHub Release](https://github.com/martin-lzh/old-market-simulator-mods/releases) 下载安装 ZIP。GitHub 的 **Source code** 压缩包是源码，不是可直接安装的插件。每次发布附安装说明和 `SHA256SUMS.txt`。当前版本均为**预发布**：自动检查已通过，游戏内验收尚未完成。

### 安装与兼容性

构建和引用环境为 Old Market Simulator **2.1.6**、Windows x64、Unity Mono。多数包依赖已安装的 **BepInEx 5**。成本插件另有 **MelonLoader 0.7.3** 包，两种加载版本择一使用。实验性独立启动入口保留源码用于本机测试，目前没有公开安装包。

安装前退出游戏并备份旧插件，按各 Mod README 操作，保留其他插件和加载器设置。插件 ZIP 仅包含原创 DLL、README 和 MIT LICENSE，不附游戏组件或加载器。

**Stack All 要求房主及所有玩家安装同版本。** 额外容器记录通过游戏正常保存流程写入。停用或降级前必须按其[卸载说明](stack-all-mod/README.md)处理；不能直接在缺少兼容插件的情况下读取含额外记录的存档。其他联机限制见各 Mod 说明。

### 当前支持版本与风险

截至 2026-09-13，最新支持的构建/引用基线为 **Old Market Simulator 2.1.6**、Windows x64 / Unity Mono 2022.3.62f3，不代表已完成实机验收或支持后续游戏更新。Assembly-CSharp.dll SHA256 同上；各 Mod 当前版本和下载见上表。

安装 Mod 或加载器可能导致旧档不兼容、无法加入好友/好友无法加入、同步异常、启动失败、卡顿或崩溃。具体风险取决于代码及安装组合；这是可能后果，不表示每个 Mod 已发生这些故障。

| 风险 | 检查要点 |
| --- | --- |
| 存档与回退 | 备份存档/插件/配置。Stack All 扩展记录需要兼容插件；删除 DLL 不撤销已保存的库存、售价或交易。 |
| 联机 | 检查双方游戏/Mod 版本及房主/客人行为。Stack All 全员同版本；纯显示 Mod 不能一概要求全员安装。 |
| 更新与冲突 | 游戏接口、加载入口、重复 DLL 或修改相同 UI/逻辑的 Mod 可能冲突。 |
| 性能与稳定性 | 资源查询、逐帧 UI、库存同步和自动操作增加开销；本机零插件加载器对照也曾卡顿，不保证修复。 |
| 误操作与经济 | 长按/开关及价格锚定执行真实操作，停止或卸载不撤销已完成动作。 |
| 显示与估算 | 文字可能遮挡/缺字；原料成本、接受率和利润估算不是实际净利润或日销量保证。 |

安装/升级/卸载前退出游戏，先测试备份副本，遵循各 Mod 风险及回退说明；不要因为更新后插件失效就用原版读 Stack All 扩展存档。历史下载及兼容性见独立版本记录：

- [Material Cost Change Log](material-cost-mod/CHANGELOG.md)
- [Coordinates Change Log](coordinates-mod/CHANGELOG.md)
- [Checkout All Change Log](checkout-all-mod/CHANGELOG.md)
- [Stack All Change Log](stack-all-mod/CHANGELOG.md)
- [Price Probability Change Log](price-probability-mod/CHANGELOG.md)

### 语言

界面跟随游戏配置的 13 种语言：英语、简体中文、繁体中文、法语、德语、意大利语、西班牙语、葡萄牙语、日语、韩语、俄语、土耳其语和乌克兰语。已有动作及标签直接读取游戏 `Translations` 表；补充文案嵌入相应 DLL，无需额外语言包或共用运行库。

坐标使用通用 X/Y/Z 标记及原生 HUD 样式。配置键名和开发日志保持稳定。未知语言的补充文案回退到英语。每个 Mod 在自己的目录内维护本地化源码、译文与测试，验证范围见各 Mod README。译文未经全部语言的母语者审校，字形、换行和实时切换效果仍需实机确认。

### 从源码构建

可以克隆仓库，也可以单独取出所需 Mod 的目录；各 Mod 独立构建，不依赖其他 Mod 或仓库级本地化工程。需要 PowerShell、支持 .NET 8 工程的 .NET SDK、本机游戏和相应加载器的引用程序集。构建只读取游戏文件，产物写入项目 `work/`、`outputs/` 和构建缓存目录，不自动安装。

在仓库根目录运行前面英文部分的五条构建命令，将示例游戏路径换为你的安装位置。ZIP 输出至 `outputs/`。有可翻译界面的 Mod 执行自己的本地化及功能测试；坐标没有可翻译文案，不依赖本地化工程。编译和测试通过不等于游戏内界面、保存或联机已验证。独立标签、发布说明和产物核验流程见[版本发布规则](releases/README.md)。

### 参与贡献

环境、测试及面向 main 的 PR 流程见[贡献指南](CONTRIBUTING.md#中文)，欢迎中英文贡献。另见[使用支持](SUPPORT.md#中文)、[安全报告](SECURITY.md#中文)及[行为准则](CODE_OF_CONDUCT.md#中文)。仓库附中英双语 Issue/PR 模板；原创贡献沿用既有根目录 MIT 许可证。

### 许可与引用

原创源码、测试、构建脚本、译文和文档使用 [MIT License](LICENSE)，版权人为 **Copyright (c) 2026 Zhaohan Liu**。各 Mod 目录也附有独立 LICENSE。游戏和第三方组件保持各自许可。仓库不公开反编译游戏源码、游戏资源、存档、凭据或个人配置。

如果本项目帮助了你的项目、文章或视频，请引用项目及具体 Mod 版本或提交号：

> Zhaohan Liu. Old Market Simulator Mods — Mod 名称，版本或提交号. https://github.com/martin-lzh/old-market-simulator-mods

这是引用请求，不是 MIT 的附加条件。复制软件或其重要部分时，项目链接不能替代 MIT 要求保留的版权和许可声明。

## Compilation SDK and CI / 编译 SDK 与 CI

Current SDK pins: **Old Market Simulator 2.1.6 / SDK r1 for existing Mods, r7 for Navigation**, Windows x64 / Unity Mono 2022.3.62f3. Each Mod pins its SDK in `release.json`; previous SDKs and Mod releases remain available. GitHub CI compiles all six Mods (seven loader variants) without original game files. A push to `main` after PR merge publishes only new numbered CHANGELOG versions. See [SDK build/update instructions](sdk/README.md) and [release workflow](releases/README.md).

当前 SDK 固定版本：**Old Market Simulator 2.1.6 / 既有 Mod 使用 r1，Navigation 使用 r7**，Windows x64 / Unity Mono 2022.3.62f3。每个 Mod 在 `release.json` 中固定 SDK，保留历史 SDK 和 Mod 发布。GitHub CI 无需原始游戏文件即可编译六个 Mod（七个加载器变体）；PR 合并后推送到 `main`，只发布 CHANGELOG 中尚未发布的编号版本。见 [SDK 构建与更新](sdk/README.md)和[发布流程](releases/README.md)。
