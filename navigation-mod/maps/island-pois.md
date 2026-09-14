# Island POIs / 海岛地点

Game 2.1.6; Map ID `0`, scene `BazaarIsland` (`level9`). Fixed anchors were checked against serialized actor transforms, store hierarchies, merchandise and interaction components. Animals for sale and inactive UI actors are excluded. All 16 anchors are active and outside expansion enable/disable subtrees. Coordinates are X/Z world positions, not guaranteed entrances or walking routes.

游戏 2.1.6；地图 ID `0`，场景 `BazaarIsland`。通过实际 NPC、店铺层级、商品与交互组件核对坐标，排除售卖动物和停用的 UI 替身。16 个固定锚点均处于激活状态且不在扩建启停子树中；坐标不代表门口或可行走路线。

| Place / 地点 | X | Z | Category / 类别 | Icon | Anchor / 锚点 |
| --- | ---: | ---: | --- | --- | --- |
| Engineer | 30.197999 | 18.887000 | shop | wrench | NPC |
| Decoration Store | 24.407000 | -47.569000 | shop | armchair | NPC |
| Lumberjack | -0.474216 | -45.254703 | shop | axe | NPC |
| Animal Market | -7.231000 | -37.340000 | shop | cow | NPC |
| Gardener | -8.867162 | 2.014615 | shop | plant | NPC |
| Clothing Store | 39.799999 | -6.207000 | shop | t-shirt | NPC |
| Junkman | 37.919728 | -18.629119 | other | recycle | NPC |
| Orders | -5.679000 | 50.721001 | dock | boat | NPC |
| Licenses | 37.116001 | -40.936001 | other | identification-card | NPC |
| Employees | 33.278999 | -28.929001 | other | users-three | NPC |
| Expansions | 49.896870 | -45.855766 | other | ruler | NPC |
| Rest | 3.367357 | -15.197052 | home | bed | Bed interaction |
| Market | 1.170000 | -13.430000 | shop | storefront | Area origin |
| Farm | -25.786367 | -7.696741 | home | barn | Area origin |
| Museum | 28.300001 | 15.096001 | other | bank | Area origin |
| Workshop | 4.613001 | -20.672001 | home | hammer | Area origin |

Names use native localization; the unnamed market and rest point remain icon-only. Employees and expansions use their interaction description keys. Junkman is a recycling service, verified through its Grinder/GrinderLever subtree, not a merchandise shop.

名称使用游戏本地化；没有原生地点名的市场与休息点仅显示图标。雇员与扩建使用交互组件的原生词条。废品商通过 Grinder/GrinderLever 子树确认是回收服务，归为 other。

The complete terrain is 1000 by 1000 units; bounds [-501,501] on both axes include a margin for the serialized transform. Geometry audit: 507904 triangles, no decoding errors. Scene SHA256: `9fe0a187c9ea33a3a266ecd8e08198a1946699a1f8a7c675fe47c4160be6e95b`. Raw extraction evidence and geometry remain local under ignored outputs. The public texture is an Image Gen illustration; local shoreline/building details can deviate from the projection.

This is a static surface map, selected independently of expansion state. It does not represent all ten market upgrades, cellar floors, farm/greenhouse/barn changes or player placements. Workshop/farm POIs identify their locations, not whether an upgrade is purchased. In-game alignment and multiplayer validation for this map are pending; retain prerelease status.

这是静态地表图，不按扩建状态切换，不代表市场十阶扩建、地下室、农场/温室/畜棚的全部状态或玩家摆放物；工坊与农场 POI 只表示地点。新图的实机对齐及联机验证待完成，保持预发布。
