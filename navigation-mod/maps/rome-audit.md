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
| `rome-town.json` | `(main)` | -472 … 533 | -365 … 640 | 59 |

Bounds include every terrain tile in each region, with a small margin. The two Gate 1 tiles are both included. The main town retains its surrounding terrain and uses an additional calibrated detail image at X [-60,105], Z [-85,90]. Local player region identity, rather than the active Unity scene or another player’s region, selects the map. Return travel uses the empty region identifier. Existing marker scopes already include this region identity.

边界包含各区域全部地形块；一号大门区域包含南北两块地形。主城保留外围地形，另在指定世界坐标加载街区细节。按本地玩家所在区域选图，不根据 Unity 当前活动场景或其他玩家区域猜测；回到主城时区域标识为空，个人标记按区域隔离。

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

World bounds remain X [1619,1924], Z [-1316,-1011], north +Z. Return POI and region metadata are unchanged. The new artwork has been visually compared with the corrected reference; in-game shoreline alignment remains pending.

世界边界仍为 X [1619,1924]、Z [-1316,-1011]，北向 +Z；返回点和区域元数据不变。新图已与修正参考图作视觉比对，实机湖岸对齐待确认。
