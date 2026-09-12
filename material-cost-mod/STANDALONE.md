# Material Cost standalone bootstrap

This directory retains the source for an experimental managed bootstrap for Old Market Material Cost. It was created to compare loader behavior while investigating runtime stalls. It is not included in the public GitHub prerelease, and no standalone binary bundle is distributed.

The bootstrap targets Old Market Simulator 2.1.6 on Windows x64 with Unity Mono. It enters the game's Mono runtime through an unmodified Unity Doorstop 4.5.0 binary and loads only the listed patch dependencies and Material Cost. It does not load the BepInEx or MelonLoader frameworks or scan their plugin directories. Doorstop is also part of the BepInEx bootstrap path, so this design does not establish that it resolves stalls or overlay waits.

The feature set comes from the same version 0.5.1 source: the end-of-day materials-only switch, recipe cost and recommended-price profit estimates, dock-order purchase-cost and profit estimates, and the shared 13-locale UI. The bootstrap does not intentionally alter number culture, Mono debugging, networking, EOS, Steam, overlays, graphics, frame rate, garbage collection, game assemblies, or saves.

## Source build

This path is for local development and investigation only. It requires Windows, .NET 8 SDK or newer, a local Old Market Simulator 2.1.6 installation, and a checksum-verified BepInEx 5.4.23.5 archive prepared under the ignored `work/bepinex` directory. The archive supplies unmodified third-party bootstrap and patch libraries; they are not committed to this repository.

From the repository root:

```powershell
./material-cost-mod/build-standalone.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

The script runs localization, accounting, and bootstrap checks and writes a local package under `outputs/`. It reads game assemblies for compilation, does not package them, and does not install or launch the Mod. The generated archive is intentionally excluded from public Releases.

## Local configuration

- `OldMarketMod/enabled.txt`: `true` loads the patch libraries and Material Cost; `false` leaves only the bootstrap log path active. Restart after changing it.
- `OldMarketMod/materials-only.txt`: remembers the report switch. It is a Mod display preference, not save data.
- `OldMarketMod/loader.log`: records bootstrap and initialization messages; the preceding log is retained as `loader.log.previous`.

Only one bootstrap entry point may be active. BepInEx `winhttp.dll`, MelonLoader `version.dll`, and standalone `winhttp.dll` must not be enabled together. Local installation or removal must happen while the game is closed, after identifying and backing up existing loader files. Never overwrite a game-owned file with a same-named bootstrap file.

## Third-party components

The local build allowlist copies these components from the verified BepInEx 5.4.23.5 archive without loading the BepInEx framework:

- [Unity Doorstop 4.5.0](https://github.com/NeighTools/UnityDoorstop), LGPL-2.1
- [HarmonyX 2.9.0](https://github.com/BepInEx/HarmonyX/tree/v2.9.0), MIT
- [MonoMod 22.1.29.1](https://github.com/MonoMod/MonoMod), MIT
- [Mono.Cecil 0.10.4](https://github.com/jbevain/cecil/tree/0.10.4), MIT

The package allowlist excludes game assemblies, assets, saves, decompiled code, BepInEx framework files, and MelonLoader files. Third-party components retain their licenses. Original project code and documentation use the [MIT License](LICENSE), Copyright (c) 2026 Zhaohan Liu.

## Validation status

The standalone target compiles, and its dependency allowlist and bootstrap structure pass automated checks. Shared logic also passes 40 accounting checks, 27 runtime cache and rule checks, and 11 localization suites. These results do not verify behavior inside the game.

Runtime initialization, UI layout, locale switching, old-save behavior, multiplayer room codes, stalls, and overlay interaction remain unvalidated for this bootstrap. No claim is made that it fixes a performance problem.

---

# Material Cost 独立启动入口（中文）

本目录保留 Old Market Material Cost 实验性 managed 启动入口的源码。它最初用于排查运行时卡顿时对比加载方式，不包含在 GitHub 公开预发布中，也不分发独立版二进制安装包。

该入口面向 Windows x64、Unity Mono 的 Old Market Simulator 2.1.6，通过未经修改的 Unity Doorstop 4.5.0 进入游戏 Mono 运行时，只加载清单中的补丁依赖和 Material Cost。它不加载 BepInEx 或 MelonLoader 框架，也不扫描这些加载器的插件目录。Doorstop 同样属于 BepInEx 启动路径，因此此设计不能证明已经解决卡顿或覆盖层等待。

功能来自 0.5.1 的同一份源码，包括日报“仅计原料”开关、配方成本与建议售价利润估算、码头订购页进货成本与利润估算，以及共用的 13 种语言界面。启动入口不会主动修改数字区域格式、Mono 调试、网络、EOS、Steam、覆盖层、画质、帧率、垃圾回收、游戏程序集或存档。

## 源码构建

此路径仅供本机开发与调查。它需要 Windows、.NET 8 SDK 或更新版本、本机 Old Market Simulator 2.1.6，以及在忽略目录 `work/bepinex` 中准备并校验过哈希的 BepInEx 5.4.23.5 官方压缩包。脚本从中取得未经修改的第三方启动和补丁库；这些文件不会提交到仓库。

在仓库根目录运行：

```powershell
./material-cost-mod/build-standalone.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

脚本运行本地化、会计和启动结构检查，并在 `outputs/` 生成本机安装包。它读取游戏程序集用于编译，不会将其打包，也不会安装或启动 Mod。生成的压缩包明确不纳入公开 Releases。

## 本机配置

- `OldMarketMod/enabled.txt`：`true` 加载补丁库和 Material Cost；`false` 只保留启动日志路径。修改后需重启。
- `OldMarketMod/materials-only.txt`：记忆日报开关，仅属于 Mod 显示设置，不是存档数据。
- `OldMarketMod/loader.log`：记录启动和初始化信息；上一份日志保留为 `loader.log.previous`。

同一时间只能启用一个启动入口。BepInEx 的 `winhttp.dll`、MelonLoader 的 `version.dll` 和独立入口的 `winhttp.dll` 不得同时启用。本机安装或移除必须在游戏退出后进行，并先识别和备份已有加载器文件。若同名文件属于游戏本身，绝不能用启动文件覆盖。

## 第三方组件

本机构建白名单从已校验的 BepInEx 5.4.23.5 官方压缩包复制以下组件，但不加载 BepInEx 框架：

- [Unity Doorstop 4.5.0](https://github.com/NeighTools/UnityDoorstop)，LGPL-2.1
- [HarmonyX 2.9.0](https://github.com/BepInEx/HarmonyX/tree/v2.9.0)，MIT
- [MonoMod 22.1.29.1](https://github.com/MonoMod/MonoMod)，MIT
- [Mono.Cecil 0.10.4](https://github.com/jbevain/cecil/tree/0.10.4)，MIT

安装包白名单排除游戏程序集、资源、存档、反编译代码、BepInEx 框架文件和 MelonLoader 文件。各第三方组件继续适用各自许可证。项目原创代码和文档采用 [MIT License](LICENSE)，Copyright (c) 2026 Zhaohan Liu。

## 验证状态

独立目标已完成编译，其依赖白名单和启动结构通过自动化检查。共用逻辑还通过 40 项会计检查、27 项运行时缓存与规则检查，以及 11 组本地化测试。这些结果不等同于游戏内验证。

此入口的运行时初始化、界面布局、语言切换、旧存档表现、多人房间码、卡顿和覆盖层交互仍未验证。本项目不宣称它能够修复性能问题。
