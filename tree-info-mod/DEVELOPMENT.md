# Tree Info development / 开发说明

**Current acceptance — 2026-09-19:** Tree Harvest Helper 0.1.0 is a stable release with maintainer-confirmed in-game acceptance. See the [validation record](../releases/validation.md) for the dated migration checks and later acceptance; this documentation update performs no new game run or scenario measurements.

**当前验收 — 2026-09-19：**Tree Harvest Helper 0.1.0 已获维护者实机验收确认，为正式发布版。迁移检查及后续验收范围见[验证记录](../releases/validation.md)；本次文档更新未重新运行游戏或测量场景。

Source version 0.1.1 was originally migrated from the local analysis project without a version bump. Public versioning then started at 0.1.0 with all those features retained. All public maintenance belongs here. Only original source, tests and documentation were migrated; game assets, decompiled snapshots and raw resource evidence remain outside this repository.

源码最初以本地 0.1.1 从分析项目迁入，迁移时未升版本；随后公开版本从 0.1.0 开始，保留全部原有功能。后续公开维护在此进行。仅迁移原创源码、测试和文档，不迁入游戏资源、反编译快照或原始取证数据。

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

Local package: `outputs/OldMarket.TreeInfo-0.1.0.zip`; CI package: `outputs/ci/tree-info/`. Package allowlists exclude game, loader and SDK assemblies. Tests cover harvest boundaries and all 13 locales. Read-only Cecil contracts check the actual game hash, reflected `BlockTree.dayCounter` / `isWatered`, Harmony `PlayerInteraction.InteractionRay` and injected fields, native ray selection, and absence of game-state writes in the plugin. SDK export includes these reflection-only members in supplemental metadata.

本地包和 CI 包位置同上，白名单排除游戏、加载器及 SDK DLL。测试覆盖成熟边界及 13 语种；Cecil 只读检查真实游戏哈希、树木反射字段、Harmony 目标和注入字段、原生射线选择及插件不写游戏状态。SDK supplemental 明确纳入只通过反射引用的成员。

Historical 2026-09-12 analysis checked 25 TreeSO resources against this game baseline. Runtime reads each tree's actual thresholds and season, rather than copying a static asset table. Migration did not rerun asset extraction or bundle those resources. UI, language and multiplayer acceptance was pending at migration; the later stable acceptance above supersedes that status without supplying an exhaustive per-scenario matrix. Player instructions: [README](README.md).

2026-09-12 的历史分析已核对此基线的 25 个 TreeSO 资源；运行时读取实际阈值和季节，不复制静态资源表。迁移时未重新提取或打包这些资源，当时实机 UI、语言和联机验收尚待完成；上方后续正式发布验收已取代该状态，但未提供逐场景完整测试矩阵。玩家用法见 [README](README.md)。

Current public version: **0.1.0**; game/SDK pin is unchanged. The initial public numbering did not alter runtime behavior or extend the recorded test scope.

当前公开版本：**0.1.0**，游戏／SDK 固定修订不变；首次公开编号本身不改变运行时行为，也不扩展已记录测试范围。

## Local media archive / 本地素材归档

The local index at `outputs/media/README.md` lists reusable Xiaohongshu, Steam, Nexus and gameplay media with source records. This ignored archive is absent from clones and release packages; archived media alone does not establish in-game acceptance.

本地 `outputs/media/README.md` 索引可复用的小红书、Steam、Nexus 与实机素材及来源记录。归档被 Git 忽略，不随克隆或发行包提供；素材本身不代表已通过实机验收。
