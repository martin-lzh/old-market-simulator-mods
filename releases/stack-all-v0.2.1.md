## English

Up to 64 physical product containers per inventory slot, separate goods/container counts, and repeated drop/throw.

### Changes

Localization is independently maintained inside this mod, including only its own messages, source, and tests. It follows all 13 configured game locales and reads existing terms from native tables. No sibling mod or root localization project is needed. The packaged README is now English-first and Chinese-second, with versioned GitHub download links.

### Download and install

Requires BepInEx 5 and the same version on the host and every player. Saves with extra container records require a compatible plugin. Before disabling or downgrading, remove extra containers, restore vanilla stack limits, save normally, and exit. Do not load these saves without a compatible plugin.

Exit the game and back up the previous plugin. Extract the selected ZIP according to its README, preserving unrelated plugins and loader settings. Each plugin ZIP contains only the original DLL, README, and MIT LICENSE; no loader, game assembly, game asset, or save is bundled.

- `OldMarket.StackAll-0.2.1.zip`
- `SHA256SUMS.txt`: SHA-256 checksums for the installation ZIPs above.

### Validation

Reference environment: Old Market Simulator 2.1.6, Windows x64 / Unity Mono.

89,116 stack/container/input checks, 47 game IL contract checks, local localization tests, and the build passed. Every mod was also built from a separate source directory without the other mods or a root localization project. Uploaded packages are checked against their recorded hashes.

**Prerelease: in-game acceptance is incomplete.** UI glyphs, wrapping, live language switching, save behavior, and multiplayer have not been validated solely by compilation or automated tests. Supplemental translations have not been reviewed by native speakers of every supported language.

### Source and license

Tag: `stack-all-v0.2.1`. Source commit: [0168579](https://github.com/martin-lzh/old-market-simulator-mods/commit/01685796bc9953122c33c240e92547bedc0edb79). Original work is under the MIT License, Copyright (c) 2026 Zhaohan Liu. See the packaged README and LICENSE for details.

## 中文

每格最多 64 个商品容器，左右分别显示商品总数与容器数，支持长按放下或扔出。

本次更新：界面覆盖游戏配置的 13 种语言；本地化源码、所需译文和测试在本 Mod 目录独立维护，无需其他 Mod 或仓库级本地化工程。原生名词及动作直接读取游戏译文，切换语言后同步文字和字体。

## 下载与安装

依赖 BepInEx 5。房主及所有玩家应安装同版本。含额外容器记录的存档需要兼容插件；停用或降级前必须按 Mod README 拆出额外容器、恢复原版上限并在游戏内保存。不要直接卸载后读取这类存档。

完全退出游戏并备份旧插件后，按 ZIP 中的目录结构放置 DLL。各插件包只含原创 DLL、README 和 Zhaohan Liu 的 MIT LICENSE；不包含加载器、游戏程序集、资源或存档。不要替换其他插件和加载器配置。

- `OldMarket.StackAll-0.2.1.zip`
- `SHA256SUMS.txt`：上述安装包的 SHA-256 校验值。

## 验证范围

目标游戏：Old Market Simulator 2.1.6，Windows x64 / Unity Mono。

89,116 项堆叠、容器与输入检查、47 项游戏 IL 契约检查及 BepInEx 构建通过。 该 Mod 适用的本地化检查通过，引用词条已与游戏语言表核对；ZIP 内容、许可证和该 Mod 内嵌语言资源均已检查。

**预发布：尚未完成此版本游戏内验收。** 字形、换行、语言切换、保存流程和联机体验不能仅凭编译或自动测试认定通过。补充译文未由全部语言的母语者审校。

## 源码与许可

- 标签：`stack-all-v0.2.1`
- 源码提交：[0168579](https://github.com/martin-lzh/old-market-simulator-mods/commit/01685796bc9953122c33c240e92547bedc0edb79)
- [Mod 使用说明](https://github.com/martin-lzh/old-market-simulator-mods/blob/stack-all-v0.2.1/stack-all-mod/README.md)
- [MIT License](https://github.com/martin-lzh/old-market-simulator-mods/blob/stack-all-v0.2.1/stack-all-mod/LICENSE)
