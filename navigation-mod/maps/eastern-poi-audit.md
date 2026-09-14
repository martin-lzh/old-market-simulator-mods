# Eastern Town POI review / 东方小镇地点复核

Current localization: water/calendar now have original functional labels in all 13 game languages. The icon-only findings below describe the earlier audit. See [current map naming](README.md#localized-names--本地化名称).

本地化更新：补水处和日历现已有全部 13 种游戏语言的原创功能标签；下方仅图标结论保留早期审计状态，当前行为见[地图命名说明](README.md#localized-names--本地化名称)。


2026-09-15, game 2.1.6, BazaarFarEast / map ID 1. Scene SHA256: `6f84c8de8332a5746af4143f66f35c54fecf3c5a25d421b8c1367fadc4228fae`.

The earlier NPC extraction verified a fixed list of eight actors and retained five landmark/bed entries. It was not an exhaustive interaction inventory: it omitted the Junkman and EmployeeStation actors, plus non-NPC facilities. This review scans all serialized scene components, checks Interactable inheritance against local read-only scripts and recomputes world positions through parent transforms. Original scene files and extraction inventories remain local and are not distributed.

旧分析核对了写死在脚本中的八个 NPC，并保留五个地标/床位条目，没有穷举交互设施。它遗漏了回收与雇员服务，以及非 NPC 设施。本轮扫描全部序列化组件，结合只读交互类继承关系、父子变换、激活状态和扩建开关复核；原始场景与提取清单不随公开仓库分发。

## Added fixed services / 补充固定服务

| Place / 地点 | X | Z | Category | Icon | Evidence / 依据 |
| --- | ---: | ---: | --- | --- | --- |
| Recycling / 回收服务 | 64.817823 | 126.047634 | other | recycle | Junkman NPC; same hierarchy contains Grinder and GrinderLever |
| Employees / 雇员服务 | 100.184980 | 127.189024 | other | users-three | EmployeeGuy actor |
| Water well / 水井补水 | 70.910004 | 206.740005 | other | drop | Refiller component |
| Calendar / 日历 | 72.853999 | 111.877002 | other | calendar-blank | Calendar component |

The recycling POI follows the existing NPC-anchor convention. The actual lever is X=67.698823, Z=124.849634; the grinder root is X=64.041823, Z=122.645634. They form one service, not three duplicate POIs. All four new anchors are active in the serialized scene, inside the current map bounds and outside all recorded expansion enable/disable subtrees. All four central-market manifests now carry the same 17 POIs; existing 13 entries and their positions are preserved.

回收 POI 沿用 NPC 服务位置口径：操作杆为 X=67.698823、Z=124.849634，回收机本体为 X=64.041823、Z=122.645634，三者属于同一服务，不重复放三个点。四个新锚点均处于激活状态、现有地图边界内且不在已提取的扩建启停子树中。中央市场四份地图统一为 17 个 POI，原有 13 项及坐标保留。

Native keys for the added named services are `junkman` and `employees`, verified from scene signs or interaction descriptions. Water and calendar use icon-only aliases: only action keys (`refill`, `view_calendar`) were verified, so they are not repurposed as invented place names. Icons and category colors are shared by both maps and target guidance.

新增回收、雇员服务使用经路牌或交互描述核实的原生词条。水井与日历暂仅显示图标：目前确认的是“补充”“查看日历”等动作词条，不把它们伪装成地点名称。图标与类别配色由大小地图及目标指引共用。

## Remaining scope / 其余范围

- No serialized ExpansionGuy, generic NPC, Teleporter or CableCarButton was found in this scene. Do not copy the Island architect POI into Eastern Town.
- There are 35 PurchasableExpansion signs across market, farm, workshop, cellar, barn and greenhouse upgrades. They can disappear or change with unlocks; they are not 35 permanent services. Existing farm/workshop POIs remain location markers, not availability claims.
- Barn, greenhouse and cellar groups were absent from the old landmark list. They are documented here rather than assigning unchecked group origins as entrances (the greenhouse origin is displaced from its upgrade signs). Entrance anchors and underground-floor handling still require verification.
- FishingZone volumes and inactive SchoolController groups are present. These describe areas/runtime fish-school behavior; their transform origins are not verified fishing spots. Their extents, activation rules and map coverage remain a separate audit.
- Existing market controls, merchandise and removable garbage are not additional destination services. Aquarium (X=46.897991, Z=184.060073) and OrigamiStand (X=47.057582, Z=183.079351) are museum facilities. They share the existing museum POI and are not separate map destinations.

未发现独立建筑师、通用 NPC、传送点或缆车按钮组件；不要照搬海岛建筑师。35 个扩建购买牌会随解锁变化，不能作为永久服务点。旧地标清单也未记录畜棚、温室与地下室，但其入口坐标仍需核对，尤其不能把偏离设施的温室分组原点当入口。14 个钓鱼区域和 18 个运行时鱼群控制器亦未完成地图化，区域原点不等于可站立钓点。水族馆与折纸收集属于博物馆内部设施，共用已有博物馆 POI，不另放图标。

This corrects navigation POI coverage, not an assertion that every game mechanic, expansion, fishing resource or runtime state has now been audited. SDK 2.1.6/r7 and Mod 0.2.0 remain unchanged. No game/save writes or in-game/multiplayer acceptance were performed.

本轮修正导航 POI 覆盖，不代表已完整审核所有游戏机制、扩建、钓鱼资源或运行时状态。SDK 2.1.6/r7 与 Mod 0.2.0 不变；未修改游戏或存档，未执行实机及联机验收。
