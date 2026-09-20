# Rome map audit / 罗马地图复核

Baseline: Old Market Simulator 2.1.6, map ID 2 (`BazaarRome`), SDK 2.1.6/r7. Reviewed 2026-09-15. Original serialized scenes were read locally; raw geometry, inventories and game resources are not distributed.

基于本地只读场景、完整父子变换、`RegionSO` 往返配置及扩建启停列表复核。地图元数据保存地点和区块范围，公开底图为 Image Gen 插画；原始几何、场景与提取清单不公开。

## Region coverage / 区域范围

| Manifest | Local region | X bounds | Z bounds | Unlock areas |
| --- | --- | --- | --- | ---: |
| `rome-caravan1.json` | `RomeCaravan1` | -2 … 302 | -2 … 302 | 0 |
| `rome-caravan2.json` | `RomeCaravan2` | -203 … 202 | -2002 … -1597 | 0 |
| `rome-gate1.json` | `RomeGate1` | 1998 … 2152 | 1848 … 2152 | 0 |
| `rome-gate2.json` | `RomeGate2` | -2002 … -1797 | -2002 … -1797 | 0 |
| `rome-gate3.json` | `RomeGate3` | -1318 … -913 | 1793 … 2198 | 0 |
| `rome-gate4.json` | `RomeGate4` | 1619 … 1924 | -1316 … -1011 | 0 |
| `rome-mine.json` | `RomeEngineerMine` | 1023 … 1178 | 1192 … 1247 | 2 |
| `rome-town.json` | `(main)` | -75 … 105 | -85 … 90 | 59 |

Travel-region bounds include every terrain tile with a small margin; both Gate 1 tiles are included. The main map focuses on the town walls with a margin, sampling the original terrain artwork through `TextureBounds` X [-472,533], Z [-365,640]. The calibrated detail image remains at X [-60,105], Z [-85,90]. Local player region identity, rather than the active Unity scene or another player’s region, selects the map. Return travel uses the empty region identifier. Existing marker scopes already include this region identity.

传送区域边界包含各自全部地形块；一号大门区域包含南北两块地形。主城范围收紧至城墙及周边余量，通过 TextureBounds X [-472,533]、Z [-365,640] 采样原有地形插画，街区细节仍位于 X [-60,105]、Z [-85,90]。按本地玩家所在区域选图，不根据 Unity 当前活动场景或其他玩家区域猜测；回到主城时区域标识为空，个人标记按区域隔离。

## Travel links / 传送关联

| Destination | Main entrance X / Z | Unlock ID | Return entrance X / Z |
| --- | ---: | --- | ---: |
| `RomeGate2` | 90.757998 / 68.199997 | `1762112275696` | -1816.361882 / -1910.329880 |
| `RomeGate1` | -35.400002 / 16.537997 | `1762112272642` | 2004.022908 / 2038.810001 |
| `RomeGate3` | 26.299995 / -23.814003 | `1762112277788` | -1299.248584 / 1869.124832 |
| `RomeCaravan1` | 76.750000 / 72.110001 | `1762112210506` | 128.142003 / 150.100006 |
| `RomeCaravan2` | 81.375999 / 70.944000 | `1762112212734` | -1.391800 / -1785.358485 |
| `RomeGate4` | -30.200005 / -69.948007 | `1762112279681` | 1890.268954 / -1158.603195 |
| `RomeEngineerMine` | -26.589994 / -1.642002 | `1762112242569` | 1171.226347 / 1213.280516 |

All seven regional portals serialize `Source`; their main-town counterparts serialize `Destination`. Gate 1–4 teleport components are initially active, while a separate expansion-controlled obstacle blocks access. They therefore have mutually exclusive lock/open-door markers. Caravan travel also requires the game’s coin payment; an open-door marker indicates the entrance is unlocked, not that the player can afford a trip. Map interactions only select navigation targets; they do not invoke teleportation or unlock RPCs.

七处区域返回门均配置为 Source，主城入口为 Destination。四座大门的传送组件初始已激活，实际通行受独立障碍阻挡，故用互斥的锁/开放入口图标。商队仍由游戏检查旅行费用；开放图标只表示入口已解锁，不表示当前金币足够。地图点击只设导航目标，不触发传送或解锁。

## Unlock areas / 解锁区块

The 62 serialized Expansion components reference 61 distinct IDs; `farmland_2` is duplicated and is represented once. Fifty-nine areas belong to town. `engineer_6` and `engineer_7` are defined in the main scene but their child geometry is at mine coordinates; their masks belong to `RomeEngineerMine`. Independent ID predicates avoid enumerating combinations or treating the highest market level as the whole save state.

62 个 Expansion 组件实际对应 61 个不同 ID，重复的 farmland_2 合并为一项。主城 59 个区块；engineer_6、engineer_7 虽挂在主城场景，其子对象却位于矿洞坐标，故绘制于矿洞地图。各项独立判断，不以最高市场等级推断整个存档。

Hatching covers the measured XZ bounds of geometry affected by each expansion. It clears when the synchronized expansion list contains that ID; overlapping pending upgrades remain shaded. These are schematic upgrade extents, not navigation polygons or a claim that the whole existing building is inaccessible. Completed artwork is revealed rather than rerendering intermediate models. Underground cellar floors, runtime resource depletion and player placements are not reconstructed.

斜线覆盖扩建影响对象的 XZ 范围，列表同步解锁后移除；重叠的未完成升级仍保留阴影。这是扩建范围示意，不是可行走多边形，也不表示范围内原有建筑完全无法进入。解锁后显露完成态插画，不逐一重绘中间阶段模型；地下室楼层、运行时资源消耗和玩家摆放物不重建。

## POIs / 地点

Town contains 46 conditional records: 15 visible initially and 39 with all expansions unlocked. NPC positions use actual actor transforms and enable/disable ancestry. Building/farm symbols use measured footprint centers, not entrance claims. The curator, aquarium and origami collection share one museum marker. Each external region has one always-available return marker once that region is selected. Resource nodes and purchasable merchandise are not permanent service POIs.

主城有 46 条含条件记录，初始显示 15 条、全解锁显示 39 条。NPC 使用实际角色位置和祖先启停条件；建筑、农场图标使用测量范围中心，不冒充入口。馆长、水族馆、折纸收集合为一个博物馆点。外部区域选中后始终保留返回点；资源节点与商品不作为永久服务 POI。

| Map | POI | X | Z | Category | Native key / icon | Required IDs | Excluded IDs |
| --- | --- | ---: | ---: | --- | --- | --- | --- |
| rome-caravan1 | `rome-caravan1-return` | 128.142003 | 150.100006 | other | `poi_return` / return | — | — |
| rome-caravan2 | `rome-caravan2-return` | -1.3918 | -1785.358485 | other | `poi_return` / return | — | — |
| rome-gate1 | `rome-gate1-return` | 2004.022908 | 2038.810001 | other | `poi_return` / return | — | — |
| rome-gate2 | `rome-gate2-return` | -1816.361882 | -1910.32988 | other | `poi_return` / return | — | — |
| rome-gate3 | `rome-gate3-return` | -1299.248584 | 1869.124832 | other | `poi_return` / return | — | — |
| rome-gate4 | `rome-gate4-return` | 1890.268954 | -1158.603195 | other | `poi_return` / return | — | — |
| rome-mine | `rome-mine-return` | 1171.226347 | 1213.280516 | other | `poi_return` / return | — | — |
| rome-town | `rome-397` | 12.762087 | 21.493506 | shop | `lumberjack` / semantic | 1762112294224 | — |
| rome-town | `rome-1666` | -16.848195 | -4.501701 | shop | `engineer` / semantic | — | 1762112234865 |
| rome-town | `rome-1744` | 28.017004 | 34.015999 | shop | `breeder` / semantic | 1762112177650 | — |
| rome-town | `rome-2337` | 30.303993 | -5.156998 | other | `recruiter` / semantic | — | 1762112233170 |
| rome-town | `rome-2474` | 78.329001 | 70.171997 | other | `plautia` / semantic | 1762112210506 | — |
| rome-town | `rome-3251` | 23.267003 | -7.337012 | shop | `gardener` / semantic | 1762112264976 | — |
| rome-town | `rome-4124` | -10.990272 | -6.122931 | other | `museum` / semantic | — | — |
| rome-town | `rome-4228` | 21.013001 | -6.935004 | shop | `gardener` / semantic | — | 1762112264976 |
| rome-town | `rome-6192` | -18.403594 | -4.5105 | shop | `engineer` / semantic | 1762112234865 | — |
| rome-town | `rome-11057` | 20.546753 | -17.692535 | other | `postbox` / mail | — | — |
| rome-town | `rome-8007` | -9.846149 | 8.977359 | shop | `decorator` / semantic | — | — |
| rome-town | `rome-1519` | 29.736003 | -18.894027 | home | `poi_rest` / semantic | — | — |
| rome-town | `rome-2005` | 30.303993 | -5.156998 | other | `employees` / semantic | 1762112233170 | — |
| rome-town | `rome-7534` | 78.826752 | 23.667465 | other | `poi_water` / semantic | — | — |
| rome-town | `rome-10486` | 69.279999 | 48.210007 | other | `poi_water` / semantic | — | — |
| rome-town | `rome-13189` | 47.330002 | 13.419998 | other | `poi_water` / semantic | — | — |
| rome-town | `rome-3199` | 27.167999 | 0.147995 | other | `junkman` / semantic | 1762112291698 | — |
| rome-town | `rome-7501` | 9.552185 | -27.390228 | other | `poi_market` / semantic | — | — |
| rome-town | `rome-gate_2-open` | 90.757998 | 68.199997 | other | `gate_2` / portal | 1762112275696 | — |
| rome-town | `rome-gate_2-locked` | 90.757998 | 68.199997 | other | `gate_2` / locked | — | 1762112275696 |
| rome-town | `rome-gate_1-open` | -35.400002 | 16.537997 | other | `gate_1` / portal | 1762112272642 | — |
| rome-town | `rome-gate_1-locked` | -35.400002 | 16.537997 | other | `gate_1` / locked | — | 1762112272642 |
| rome-town | `rome-gate_3-open` | 26.299995 | -23.814003 | other | `gate_3` / portal | 1762112277788 | — |
| rome-town | `rome-gate_3-locked` | 26.299995 | -23.814003 | other | `gate_3` / locked | — | 1762112277788 |
| rome-town | `rome-caravan_1-open` | 76.75 | 72.110001 | other | `caravan_1` / portal | 1762112210506 | — |
| rome-town | `rome-caravan_2-open` | 81.375999 | 70.944 | other | `caravan_2` / portal | 1762112212734 | — |
| rome-town | `rome-gate_4-open` | -30.200005 | -69.948007 | other | `gate_4` / portal | 1762112279681 | — |
| rome-town | `rome-gate_4-locked` | -30.200005 | -69.948007 | other | `gate_4` / locked | — | 1762112279681 |
| rome-town | `rome-engineer_mine-open` | -26.589994 | -1.642002 | other | `engineer_mine` / portal | 1762112242569 | — |
| rome-town | `rome-facility-barn_1` | 36.655 | 30.56 | other | `barn_1` / semantic | 1762112200305 | — |
| rome-town | `rome-facility-barn_2` | 66.89 | 8.63 | other | `barn_2` / semantic | 1762112203057 | — |
| rome-town | `rome-facility-barn_4` | 54.13 | 70.225 | other | `barn_4` / semantic | 1762112208026 | — |
| rome-town | `rome-facility-barn_3` | 24.95 | 41.99 | other | `barn_3` / semantic | 1762112205490 | — |
| rome-town | `rome-facility-clothing_store_1` | 6.015 | -9.93 | shop | `clothing_store_1` / semantic | 1762112222977 | — |
| rome-town | `rome-facility-farm_1` | 20.825 | 16.775 | other | `farm_1` / semantic | 1762112248741 | — |
| rome-town | `rome-facility-farm_6` | 57.88 | 42.92 | other | `farm_6` / semantic | 1762112258729 | — |
| rome-town | `rome-facility-farm_5` | 79.09 | 56.78 | other | `farm_5` / semantic | 1762112257233 | — |
| rome-town | `rome-facility-farm_4` | 78.66 | 30.08 | other | `farm_4` / semantic | 1762112255681 | — |
| rome-town | `rome-facility-farm_3` | 54.055 | 12.055 | other | `farm_3` / semantic | 1762112253353 | — |
| rome-town | `rome-facility-farm_2` | 20.705 | 23.105 | other | `farm_2` / semantic | 1762112250785 | — |
| rome-town | `rome-facility-greenhouse_2` | 37.77 | 70.24 | other | `greenhouse_2` / semantic | 1762112285796 | — |
| rome-town | `rome-facility-greenhouse_1` | 27.27 | 70.24 | other | `greenhouse_1` / semantic | 1762112283728 | — |
| rome-town | `rome-facility-workshop_2` | 80.59 | 11.085 | other | `workshop_2` / semantic | 1762112318271 | — |
| rome-town | `rome-facility-workshop_1` | -43.965 | 10.25 | other | `workshop_1` / semantic | 1762112316424 | — |
| rome-town | `rome-facility-workshop_4` | 6.94 | -17.16 | other | `workshop_4` / semantic | 1762112321865 | — |
| rome-town | `rome-facility-workshop_3` | 36.46 | 45.0 | other | `workshop_3` / semantic | 1762112320143 | — |

## Artwork / 插画来源

Nine final PNGs were made with the built-in Image Gen tool: eight region bases and one town detail. The Gate 4 large terrace received a correction pass to remove an invented tiled roof. [Prompt set](rome-image-prompts.md). Regions are illustrated overhead maps; rock ceilings and building roofs can obscure lower geometry, so they do not establish traversable routes.

使用内置 Image Gen 生成八张区域底图及一张主城细节图；四号大门区域另修正了被误画成瓦屋顶的平坦露台。[提示词记录](rome-image-prompts.md)。插画采用俯视表现，岩顶与屋顶可能遮挡下层结构，不代表已经验证可行走路线。

## Validation limits / 验证边界

No game installation or save was modified. In-game travel, unlock sequencing, artwork-to-world alignment, reload and two-player acceptance remain pending. Version 0.2.0 and the SDK pin remain unchanged.

未修改游戏安装或存档；区域往返、解锁顺序、插画实机对齐、重载和双端联机尚待验收。版本与 SDK 不推进。

Validation for source `db03d32` (2026-09-15): version validation and the full SDK build passed; 64 CI tooling tests, 1291 navigation checks and 659 Navigation contract checks passed. The Navigation real-reference build matched the SDK build in symbolic IL and embedded resources after verifying 15 dependency hashes. The repository-wide real-reference command stopped earlier at Stack All's SDK/real-build differences; Navigation was then verified independently without bypassing any hashes.

源码 `db03d32`（2026-09-15）的版本验证与全仓 SDK 构建通过，64 项 CI 工具测试、1291 项导航检查和 659 项 Navigation 契约检查通过。Navigation 的 15 个真实依赖哈希已核对，真实引用与 SDK 构建的符号 IL/内嵌资源一致。全仓真实引用命令先在 Stack All 的 SDK/真实构建差异处停止，随后独立验证 Navigation，未绕过任何哈希检查。

## Gate 4 water correction — 2026-09-18

The previous local reference skipped water-material meshes and classified water only below a fixed sea-level threshold. Gate 4 contains an active elevated lake at Y ≈ 69.66, so that reference incorrectly showed dry terrain. The replacement reference rasterizes its actual 50 × 50 water mesh footprint, clips it against terrain elevations, and preserves overlying rocks, trees and structures. The visible lake is west of the long central terrace; the southwestern circular terrain patch is not this lake. The published asset is a new Image Gen illustration, not the raw render.

大门4旧参考图跳过水材质网格，仅以固定海平面识别水域，漏掉了 Y 约 69.66 的高处湖面。新参考图对真实 50 × 50 水面网格按地形高度裁剪，保留上方岩石、树木与结构。可见湖泊位于中央长平台西侧，西南圆形地块并非该湖。公开贴图为重新生成的 Image Gen 插画，原始渲染不公开。

World bounds remain X [1619,1924], Z [-1316,-1011], north +Z. Return POI and region metadata are unchanged. The new artwork has been visually compared with the corrected reference; local in-game testing of build `921d14b` was subsequently reported complete on 2026-09-18.

世界边界仍为 X [1619,1924]、Z [-1316,-1011]，北向 +Z；返回点和区域元数据不变。新图已与修正参考图作视觉比对，随后于 2026-09-18 获构建 `921d14b` 本地实机测试完成反馈。

## Main-town coverage correction — 2026-09-20

The local `level11` scene hash still matches the evidence used for the original map. Its large terrain tile spans 1000 × 1000 units; that is scenery coverage, not a measured player-accessible boundary. The active castle-wall geometry spans approximately X [-63.665,93.797], Z [-71.004,80.290]. The former 1005 × 1005 map rectangle was about 32 times the area of the replacement 180 × 175 rectangle, X [-75,105], Z [-85,90]. The replacement includes the wall footprint with at least 9 units of margin, all 46 conditional POIs and all 59 town expansion rectangles. Travel destinations remain on their seven separate maps.

The west wall extends beyond the detail patch's X = -60 edge, so the detail image alone cannot replace the base map. Preserve both PNGs byte-for-byte and crop the base UVs using its original world bounds. Both large map and minimap use this calibration; detail/expansion layers and personal markers continue to use world coordinates. This is a framing correction based on scene geometry, not a NavMesh or exhaustive walkability claim. In-game alignment, town-edge movement and wheel/button feel remain untested for this change.

本地 level11 场景哈希与原地图证据一致。1000 × 1000 的大地形块表示背景覆盖，不是实测可行走边界；当前激活城墙的几何范围约为 X [-63.665,93.797]、Z [-71.004,80.290]。原 1005 × 1005 显示矩形面积约为新范围的 32 倍；新范围为 X [-75,105]、Z [-85,90]，包含城墙及至少 9 单位余量、全部 46 条条件 POI 和 59 个主城扩建矩形，七个旅行目的地继续单独显示。

西侧城墙超出细节图 X = -60 的边界，不能直接把细节图当成完整底图。保留两张 PNG 的原始字节，按原世界范围裁剪底图 UV；大小地图共用此标定，细节层、扩建层与个人标记继续使用世界坐标。本次依据场景几何修正显示范围，不代表 NavMesh 或逐处通行验收；实机对齐、城墙边缘行走及滚轮/按钮手感仍待验证。

Validation: version and syntax checks, 69 CI tooling tests, 1820 navigation checks and all eight SDK builds passed. All eight real-reference builds matched SDK symbolic IL/resources after dependency-hash verification; 674 Navigation metadata/IL contracts passed. SDK 2.1.6/r7 and Mod 0.2.0 are unchanged. No game installation or save was modified.

验证：版本与语法检查、69 项 CI 工具测试、1820 项导航检查及八种 SDK 构建通过；依赖哈希核对后，八种真实引用构建的符号 IL/资源均与 SDK 构建一致，674 项 Navigation 元数据/IL 契约通过。SDK 2.1.6/r7 与 Mod 0.2.0 不变，未修改游戏安装或存档。
