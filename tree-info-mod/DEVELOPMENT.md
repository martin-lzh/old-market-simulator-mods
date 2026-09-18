# Tree Info development / 开发说明

Source version 0.1.1 was migrated from the original local analysis project; no version was advanced for migration. All public maintenance now belongs here. Only original source, tests and documentation are migrated; game assets, decompiled snapshots and raw resource evidence remain outside this repository.

源码版本 0.1.1 从原本地分析项目迁入，本次不推进版本，后续公开维护在此进行。仅迁移原创源码、测试和文档，不迁入游戏资源、反编译快照或原始取证数据。

## Build / 构建

Game baseline: 2.1.6, Windows x64 / Unity Mono 2022.3.62f3; SDK 2.1.6/r8. Assembly-CSharp SHA256: `FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296`. The installed loader is BepInEx 5.4.23.4; CI uses pinned official 5.4.23.5 references. SDKs contain compilation declarations only.

游戏基线及哈希同上；SDK 仅用于编译，不能安装。构建只读游戏引用，不自动安装。根目录执行：

```powershell
dotnet run --project tree-info-mod/tests/Tests.csproj -c Release
./tree-info-mod/build.ps1 -GameDir 'F:\SteamLibrary\steamapps\common\Old Market Simulator'
python tools/ci.py validate
python tools/ci.py build
python tools/ci.py verify-game --game-dir 'F:\SteamLibrary\steamapps\common\Old Market Simulator'
```

Local package: `outputs/OldMarket.TreeInfo-0.1.1.zip`; CI package: `outputs/ci/tree-info/`. Package allowlists exclude game, loader and SDK assemblies. Tests cover harvest boundaries and all 13 locales. Read-only Cecil contracts check the actual game hash, reflected `BlockTree.dayCounter` / `isWatered`, Harmony `PlayerInteraction.InteractionRay` and injected fields, native ray selection, and absence of game-state writes in the plugin. SDK export includes these reflection-only members in supplemental metadata.

本地包和 CI 包位置同上，白名单排除游戏、加载器及 SDK DLL。测试覆盖成熟边界及 13 语种；Cecil 只读检查真实游戏哈希、树木反射字段、Harmony 目标和注入字段、原生射线选择及插件不写游戏状态。SDK supplemental 明确纳入只通过反射引用的成员。

Historical 2026-09-12 analysis checked 25 TreeSO resources against this game baseline. Runtime reads each tree's actual thresholds and season, rather than copying a static asset table. Migration does not rerun asset extraction or bundle those resources. In-game UI, language and multiplayer acceptance remains pending; see [README](README.md).

2026-09-12 的历史分析已核对此基线的 25 个 TreeSO 资源；运行时读取实际阈值和季节，不复制静态资源表。本次迁移不重新提取或打包这些资源，实机 UI、语言和联机验收仍待完成，见 [README](README.md)。
