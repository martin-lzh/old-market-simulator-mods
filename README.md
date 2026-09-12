# Old Market Simulator Mods

[English](#english) · [中文](#中文)

## English

Five unofficial mods for **Old Market Simulator**, with source code, tests, and build scripts. Each mod is versioned and released independently.

### Downloads

| Mod | Release | Features |
| --- | --- | --- |
| [Material Cost](material-cost-mod/README.md) | [0.5.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.1) | Material-cost estimates in daily reports, recipes, and purchase-profit details in orders |
| [Coordinates](coordinates-mod/README.md) | [0.1.3](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.3) | World coordinates beneath the money HUD; F8 toggle |
| [Checkout All](checkout-all-mod/README.md) | [0.1.4](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.4) | Continuous checkout by holding E or toggling F9 |
| [Stack All](stack-all-mod/README.md) | [0.2.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/stack-all-v0.2.1) | Up to 64 containers per inventory slot, separate item/container counts, repeated drop/throw |
| [Price Probability](price-probability-mod/README.md) | [0.1.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/price-probability-v0.1.1) | Price-acceptance estimates and host-controlled fixed-price/probability rules |

Get installable ZIPs from the linked [GitHub Releases](https://github.com/martin-lzh/old-market-simulator-mods/releases). GitHub's **Source code** archives contain source, not ready-to-install plugins. Each release includes installation notes and `SHA256SUMS.txt`. Current versions are **prereleases**: automated checks passed, but in-game acceptance is incomplete.

### Installation and compatibility

The build/reference environment is Old Market Simulator **2.1.6**, Windows x64, Unity Mono. Most packages require an existing **BepInEx 5** installation. Material Cost also provides a **MelonLoader 0.7.3** package; choose one loader variant. The experimental standalone entry remains available as source for local testing and has no public release package.

Exit the game and back up the old plugin before installing. Follow the individual mod's README and preserve other plugins and loader settings. Plugin ZIPs include only the original plugin DLL, README, and MIT LICENSE; game files and loaders are not bundled.

**Stack All requires the host and all players to use the same version.** Its extra container records are saved through the game's normal save process. Before disabling or downgrading it, follow the [removal procedure](stack-all-mod/README.md); loading such a save without a compatible plugin is unsafe. Other multiplayer restrictions are documented per mod.

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

### License and attribution

Original source, tests, build scripts, translations, and documentation are under the [MIT License](LICENSE), **Copyright (c) 2026 Zhaohan Liu**. Each mod also includes its own LICENSE. Game and third-party components retain their own licenses. Decompiled game source, game assets, saves, credentials, and personal configuration are not published.

If this work helps your project, article, or video, please cite the project and relevant mod/version or commit:

> Zhaohan Liu. Old Market Simulator Mods — mod name, version or commit. https://github.com/martin-lzh/old-market-simulator-mods

Citation is requested, not an additional MIT condition. A project link does not replace the copyright and permission notices required by MIT when copying the software or substantial portions of it.

## 中文

本仓库提供 **Old Market Simulator** 的五个非官方 Mod，以及源码、测试和构建脚本。各 Mod 独立管理版本与发布。

### 下载

| Mod | 发布版本 | 功能 |
| --- | --- | --- |
| [Material Cost](material-cost-mod/README.md) | [0.5.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/material-cost-v0.5.1) | 日报与配方原料成本、订购页进货成本和预计利润 |
| [Coordinates](coordinates-mod/README.md) | [0.1.3](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/coordinates-v0.1.3) | 金钱栏下方显示世界坐标，F8 切换 |
| [Checkout All](checkout-all-mod/README.md) | [0.1.4](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.4) | 长按 E 或按 F9 开关连续结账 |
| [Stack All](stack-all-mod/README.md) | [0.2.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/stack-all-v0.2.1) | 每格最多 64 个容器，分别显示商品与容器数，支持连续放下/扔出 |
| [Price Probability](price-probability-mod/README.md) | [0.1.1](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/price-probability-v0.1.1) | 定价接受概率，以及房主控制的固定售价/概率规则 |

从上表对应的 [GitHub Release](https://github.com/martin-lzh/old-market-simulator-mods/releases) 下载安装 ZIP。GitHub 的 **Source code** 压缩包是源码，不是可直接安装的插件。每次发布附安装说明和 `SHA256SUMS.txt`。当前版本均为**预发布**：自动检查已通过，游戏内验收尚未完成。

### 安装与兼容性

构建和引用环境为 Old Market Simulator **2.1.6**、Windows x64、Unity Mono。多数包依赖已安装的 **BepInEx 5**。成本插件另有 **MelonLoader 0.7.3** 包，两种加载版本择一使用。实验性独立启动入口保留源码用于本机测试，目前没有公开安装包。

安装前退出游戏并备份旧插件，按各 Mod README 操作，保留其他插件和加载器设置。插件 ZIP 仅包含原创 DLL、README 和 MIT LICENSE，不附游戏组件或加载器。

**Stack All 要求房主及所有玩家安装同版本。** 额外容器记录通过游戏正常保存流程写入。停用或降级前必须按其[卸载说明](stack-all-mod/README.md)处理；不能直接在缺少兼容插件的情况下读取含额外记录的存档。其他联机限制见各 Mod 说明。

### 语言

界面跟随游戏配置的 13 种语言：英语、简体中文、繁体中文、法语、德语、意大利语、西班牙语、葡萄牙语、日语、韩语、俄语、土耳其语和乌克兰语。已有动作及标签直接读取游戏 `Translations` 表；补充文案嵌入相应 DLL，无需额外语言包或共用运行库。

坐标使用通用 X/Y/Z 标记及原生 HUD 样式。配置键名和开发日志保持稳定。未知语言的补充文案回退到英语。每个 Mod 在自己的目录内维护本地化源码、译文与测试，验证范围见各 Mod README。译文未经全部语言的母语者审校，字形、换行和实时切换效果仍需实机确认。

### 从源码构建

可以克隆仓库，也可以单独取出所需 Mod 的目录；各 Mod 独立构建，不依赖其他 Mod 或仓库级本地化工程。需要 PowerShell、支持 .NET 8 工程的 .NET SDK、本机游戏和相应加载器的引用程序集。构建只读取游戏文件，产物写入项目 `work/`、`outputs/` 和构建缓存目录，不自动安装。

在仓库根目录运行前面英文部分的五条构建命令，将示例游戏路径换为你的安装位置。ZIP 输出至 `outputs/`。有可翻译界面的 Mod 执行自己的本地化及功能测试；坐标没有可翻译文案，不依赖本地化工程。编译和测试通过不等于游戏内界面、保存或联机已验证。独立标签、发布说明和产物核验流程见[版本发布规则](releases/README.md)。

### 许可与引用

原创源码、测试、构建脚本、译文和文档使用 [MIT License](LICENSE)，版权人为 **Copyright (c) 2026 Zhaohan Liu**。各 Mod 目录也附有独立 LICENSE。游戏和第三方组件保持各自许可。仓库不公开反编译游戏源码、游戏资源、存档、凭据或个人配置。

如果本项目帮助了你的项目、文章或视频，请引用项目及具体 Mod 版本或提交号：

> Zhaohan Liu. Old Market Simulator Mods — Mod 名称，版本或提交号. https://github.com/martin-lzh/old-market-simulator-mods

这是引用请求，不是 MIT 的附加条件。复制软件或其重要部分时，项目链接不能替代 MIT 要求保留的版权和许可声明。
