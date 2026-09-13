# Old Market Navigation

Version **0.1.4 — experimental**. An independent navigation HUD for Old Market Simulator: compass, minimap, map window, personal markers and target bearings. It does not require Coordinates or replace that plugin.

**Build and automated checks are available. Version 0.1.2 was installed locally; user screenshots exposed layout defects and M-key failure. This repair still requires in-game appearance, input, performance and multiplayer acceptance.** The public package does not include game-derived map images or geometry. Without a local map companion, the compass and optional coordinates work, the map reports that no map is available, and map marker creation is disabled.

## Features and controls

Pending local build (Unreleased, version unchanged): the wheel compensates for the active UI module scroll multiplier while keeping fractional input. Optional `Pois` arrays in local map manifests contain `Id`, `Name`, `NameKey`, `Category` (shop/home/dock/other), `X` and `Z`. Up to 256 validated points per map are supported. POI icons stay visible; names open above 1.5x zoom and close below 1.35x or when blocked. Left- or right-click a POI icon or visible label to target it. Personal labels also collapse when space is limited. POIs are companion data, not automatically discovered moving player buildings. Accuracy depends on the local manifest: prefer verified NPC transforms for shops; regional origins are not verified entrances. The native licenses key uses an identification-card icon; a home-shaped rest icon does not imply player ownership. Old maps without Pois still work. Built-in POIs use distinct Phosphor fill symbols, with color determined by category. Both map views render POIs; minimap icons and labels stay upright in either rotation mode, with labels expanding below about 67 m range where space permits. Select a POI on the large map to use compass guidance. Map-load logs include POI counts; opening the large map logs data, node and visible-node counts for diagnosis. The metadata reader is tested against nested arrays, but in-game visual acceptance is still pending.

POI categories use shop #ad7568, home/rest #829278, dock/order #77929d and other #978190 icons, all with 3 px outlines on the 32 px source texture and transparent backgrounds. The player uses a narrow #c1ccd0 direction arrow at the same canvas size as POIs (25 px minimap, 25 px large map) without a nameplate. Built-in place labels come from the native game localization table; unverified or unavailable labels remain hidden.

The map window uses a dark brown frame with gold borders, a map viewport and a separate marker sidebar. The sidebar shows the current target at the top and the selected personal marker editor below; there is no marker overview list. Wheel zoom and the on-map +/- buttons transition smoothly; wheel zoom follows the pointer where map boundaries allow. The map covers its viewport and panning stops at its edges. Map geography still comes from the same local companion files.

Press main-row **-** to zoom the minimap out and **=** to zoom in. The map frame stays the same size. Its native-style hint pairs each action with its keycap: Zoom out [-], Zoom in [=]; it sits above the map and participates in HUD avoidance. Typing, native menus, the large map and hidden minimaps suppress these shortcuts; numpad +/- are unchanged. Range steps are 20–1000 world units. Shortcut zoom lasts for this session; editing MinimapRange resets it, while moving/resizing the HUD preserves it.

- Compass follows camera yaw with 5° short, 15° medium and 45° long ticks, a fixed graphical pointer and a separate degree readout. This mod defines **+Z as north and +X as east**; this is a navigation convention, not a verified native geographic definition.
- Circular minimap defaults to north up. Switch to camera up in the map window or configuration; the choice persists. Big map always stays north up.
- Press **M** to open/close the map, or **Esc** / the Close button to close it. Keyboard input inside a marker name does not trigger M. The default matches the game's `UI.Map` default binding, but this version uses its own configurable keyboard key and does not automatically follow game rebinding.
- Scroll to zoom, drag with the left mouse button to pan, right-click empty mapped terrain to add a marker. Right-click a personal marker icon or its visible name to remove it; left-click either to edit its text, color and icon below the current target in the sidebar. Color, shape and delete actions share one icon row below Set target. Color and shape buttons expand a slot grid below; select a slot to apply and save. Personal marker sprites stay visible without native font symbols. Fixed POIs remain read-only. Up to 512 markers per scope, with names up to 80 characters.
- The target panel below the compass displays left/right bearing, horizontal distance and the absolute angle from your camera direction. Distance uses X/Z world units displayed as metres. This bearing panel is not an elevation measurement, route or obstacle-aware path.
- When a loaded non-trigger collision surface is found below the target, a separate diamond and name/distance label use the actual camera projection. The probe runs at most once per second while a target exists and the map is closed; target/scene changes invalidate its cache. No hit, a point behind the camera or a label near screen edges leaves only the bearing panel. There is no line-of-sight/occlusion check: a hit may be a roof or object rather than walkable ground, and the cached height may lag moving surfaces. No unloaded terrain is generated.
- Opening the map releases the cursor and suppresses character controls. It temporarily suppresses the game's Settings/Close actions to prevent Esc from also opening Pause. It does not pause the world or other players. Native windows/loading take priority.
- UI decorations are embedded in the DLL. The map reuses static textures; it does not create a scene-rendering camera, load extra game regions or send gameplay RPCs.

## Installation, update and removal

1. Exit the game normally. Back up an older Navigation DLL and any Navigation configuration/markers before updating.
2. Use an existing working **BepInEx 5** installation; the local development baseline is 5.4.23.4 with the Mono game assemblies. Loader compatibility beyond the local build is unverified.
3. Extract the package into the game directory. The plugin path is `BepInEx/plugins/OldMarket.Navigation/OldMarket.Navigation.dll`.
4. If you have a locally generated map companion, place its JSON/PNG files in `BepInEx/plugins/OldMarket.Navigation/maps/`, beside the DLL. The public package intentionally contains no such companion.
5. Start the game and enter a map. Test M/Esc and native menus before regular play.

Updating replaces only this plugin's files. To uninstall, exit and remove `OldMarket.Navigation.dll`; remove its `maps/` directory only if no longer wanted. Optional settings and personal markers are under `BepInEx/config/OldMarket.Navigation/` and `BepInEx/config/local.oldmarket.navigation.cfg`. Keep those files to preserve markers. To roll back, restore the backed-up DLL and matching configuration. Do not remove unrelated plugins or the loader. No original game component or game save is rewritten by this plugin.

## Configuration

The first load creates `BepInEx/config/local.oldmarket.navigation.cfg`. Edit this file with the game closed; BepInEx configuration descriptions are English because they cannot follow live game locale changes.

| Setting | Default | Meaning |
| --- | --- | --- |
| `Display.Minimap` | `true` | Show minimap. |
| `Display.Compass` | `true` | Show compass. |
| `Display.TargetGuidance` | `true` | Show target bearing panel. |
| `Display.Coordinates` | `false` | Show XYZ below compass; leave off when using Coordinates to avoid duplication. |
| `Display.CameraUp` | `false` | Rotate minimap with camera; the map-window button saves this choice. |
| `Display.ReflowNativeHud` | `true` | Move native notification/task/hint HUD around navigation; restore original anchors when disabled. |
| `Input.MapKey` | `M` | Unity Input System `Key` value; `None` disables the keyboard toggle. Avoid keys used by other actions. |
| `Markers.RemoteProfile` | empty | Explicit unique identity for a remote host's save. Empty means session-only remote markers. |

### Layout hot reload

The minimap defaults to the lower-left corner. Native HUD reflow uses its rendered bounds, including the compass's visible coordinate and target rows. Notifications (including player-join messages) move above the minimap and align to the same side; the upper-right task card stays in place unless it conflicts. Top hints appear below the compass. The Mod retains native fonts, content and animation-controlled positions. Disabling the corresponding navigation area, disabling reflow or leaving gameplay restores owned anchor changes. Full-screen menus and external Steam/performance overlays are not moved. In-game animation, long-message and unusual-resolution acceptance remains outstanding.

Edit `BepInEx/config/OldMarket.Navigation/layout.json` while playing. Changes are checked every 0.5 seconds on the main thread. Malformed, oversized or out-of-range files retain the previous valid layout. UI position/size changes do not require a DLL rebuild. This does not hot-reload C# logic, map files or game data.

```json
{
  "MinimapSize": 240,
  "MinimapBottomLeft": true,
  "MinimapLeft": 24,
  "MinimapBottom": 48,
  "MinimapRight": 24,
  "MinimapTop": 150,
  "MinimapRange": 100,
  "CompassWidth": 520,
  "CompassTop": 22,
  "WorldMarkerSize": 28,
  "MapWidth": 1080,
  "MapHeight": 720,
  "Opacity": 0.92
}
```

Layout units use a 1920×1080 reference canvas. `MinimapRange` is the radius in world units. The historical field name `WorldMarkerSize` controls target-panel and projected-marker text size. Very small layouts and long translated text still require visual testing.

`MinimapBottomLeft=true` uses `MinimapLeft`/`MinimapBottom`; the default bottom margin leaves room for the mode label. Set it to `false` to use the legacy `MinimapRight`/`MinimapTop` upper-right placement. Existing layout files without the new fields use the new lower-left defaults. HUD content is remeasured at most every 0.1 seconds, with immediate reflow for navigation-bound or screen-size changes. If a message burst cannot fit anywhere without overlap, its original position is retained rather than hiding or shrinking it.

## Map data and expansion states

Local companion metadata matches map ID, scene, region and expansion IDs; each image includes explicit X/Z bounds and `North: "+Z"`. Unlock variants can share one painted base image and supply exact obstacle overlays; they are not four independently invented terrain layouts. Ambiguous matching metadata is rejected instead of choosing an arbitrary map. Map geometry is never inferred from decorative artwork alone. Game-derived map textures, geometry and metadata stay outside the public source/release. The optional local companion currently has evidence only for Eastern Town and the central market's four unlock states; other areas or changes in game content must be independently checked.

The runtime reads the replicated unlock set and selects matching variants after unlock or scene events, with a short delay for game activation. This supports removal of market clutter when the corresponding companion variant exists. It does not redraw arbitrary player buildings, items, vegetation or every expansion automatically. Map files load at plugin startup; restart after changing them. Expansion changes do not change marker storage scope.

## Marker storage and multiplayer

Markers are stored in the mod's own XML files, separated by local save identity, map and region. The host identity includes the slot and its directory creation timestamp; recreating/copying a save directory can change that identity. No game save is deserialized or written.

For a remote client, leave `RemoteProfile` empty for connection-only markers. They survive region round trips within that connection but disappear after disconnect/restart. Setting a unique `RemoteProfile` opts into persistent remote markers; reusing it for different host saves mixes those markers, so choose distinct values. Unknown/corrupt marker files are not silently replaced; failed loading disables writes for that scope during the connection. Targets are session selections and are not persisted. Markers are personal, with no network sharing.

## Languages and compatibility

UI text follows the game's selected locale and native money-HUD TextMesh Pro font. Original translations cover `zh`, `zh-Hant`, `en`, `de`, `fr`, `it`, `ja`, `ko`, `pt`, `ru`, `es`, `tr`, `uk`; unknown locales fall back to English. These translations have not been reviewed by native speakers or verified for all font glyphs/long labels in game. Marker names and companion-provided place names are not translated automatically.

Targets Old Market Simulator **2.1.6**, Unity Mono **2022.3.62f3**, and compiler SDK **2.1.6/r7** pinned in `release.json`; the earlier Mods keep r1. CI uses the official SHA256-pinned BepInEx 5.4.23.5 reference, while in-game acceptance remains outstanding. Reflection-only save/region/expansion members are included in the metadata SDK and checked against real assemblies. Future game updates can change player, input, locale, save-identity or expansion APIs. Menu/HUD mods and bindings may conflict. A successful build does not establish compatibility for single-player, host or remote clients, nor a frame-rate guarantee.

## Build and validation

For a game-free CI build, run `python tools/ci.py validate`, `python -m unittest discover -s tools/tests -v`, and `python tools/ci.py build`. This runs Navigation tests and packages only the original DLL and documents. To compare with a matching installed game and execute read-only contracts, run `python tools/ci.py verify-game --game-dir <path>`. These checks do not launch Unity or constitute in-game acceptance.

Requirements for the local build below: Windows, .NET SDK (tests target .NET 8), local game assemblies (including the read-only `UnityEngine.PhysicsModule.dll` reference) and BepInEx 5. From the repository root:

```powershell
./navigation-mod/build.ps1
./navigation-mod/build.ps1 -GameDir 'D:\Path\To\Old Market Simulator'
dotnet run --project navigation-mod/tests/Navigation.Tests.csproj
```

Build output: `outputs/OldMarket.Navigation-0.1.4.zip`. The script runs the Release checks before building, prints the package SHA256 and installs nothing. The archive allowlist is exactly the plugin DLL, this README, CHANGELOG and LICENSE. It excludes game assemblies, loader files, map companions, saves, logs and backups.

Verified on the 2.1.6 baseline: SDK r7 and real-reference Release builds match in symbolic IL, assembly references and embedded resources; 1397 pure checks, 553 read-only installed-assembly contracts and 31 CI tooling tests pass. No Unity game code was executed by these checks.

Automated coverage exercises coordinate conversion/rotation boundaries, marker serialization/isolation/error handling and locale fallback. Static review checked camera-up rotation, expansion event invalidation and Esc action handling. **Still required in game:** native font/overlap, both map modes, dragging/zoom/marker editing, Esc without Pause leakage, disconnect/region travel, each market unlock transition, frame time and host/client behavior. Check those in a backed-up test setup before relying on the experimental build.

## License and provenance

Original code, documentation and original UI decorations are provided under [MIT](LICENSE); see [CHANGELOG](CHANGELOG.md). UI decorations were generated with image generation tools and embedded as original decorative assets. Game assemblies are read-only build references, not redistributed. The MIT grant does not cover game resources, derived local map companions, trademarks or unrelated mods. No third-party runtime dependency is bundled.

---

## 中文说明

**0.1.4 是实验版**：独立导航 Mod，包含顶部罗盘、圆形小地图、M 键大地图、个人标记和目标方位栏，不依赖也不替换 Coordinates。

已提供构建及自动检查。0.1.2 已在本机安装，用户截图暴露了布局缺陷和 M 键失效；**本次修复仍需游戏内视觉、输入恢复、性能及联机验收**。公开包不含游戏派生地图。没有本地地图配套文件时仍可显示罗盘和可选 XYZ，地图提示无数据，不能在地图上新增标记。

### 功能与操作

本地待发布构建（版本号不变）：滚轮按当前 UI 模块的实际倍率归一化，保留小数输入。本地地图 manifest 可选 `Pois` 数组，每项含 `Id`、`Name`、`NameKey`、`Category`（shop/home/dock/other）、`X`、`Z`，每幅图最多 256 个经校验地点。POI 保留语义图标，缩放达到 1.5 倍且空间足够时展开名称，低于 1.35 倍或发生遮挡时收起；左键或右键点击 POI 图标或可见名称可设为导航目标。个人标记的名称也会按空间收起。地点来自本地配套数据，不会自动追踪玩家移动的建筑；定位精度取决于本地清单，店铺宜采用经核实的 NPC Transform；区域原点不保证是入口。原生 licenses 词条使用证件图标，房屋形休息图标不代表玩家所有权。旧地图没有 Pois 时仍正常使用。内置 POI 使用不同 Phosphor 实心图形，同类别同颜色。大小地图均绘制 POI；小地图两种朝向下图标与名称保持正立，范围缩小到约 67 米以下且空间足够时展开名称。在大地图选择 POI 可使用罗盘指引。加载日志记录 POI 数量，打开大地图时记录数据、节点及视野内节点数量，便于定位显示问题。元数据读取已验证嵌套数组，实机视觉效果仍待验收。

POI 按类别使用商店 #ad7568、房屋/休息点 #829278、码头/订购点 #77929d 及其他地点 #978190，均有 32 px 源纹理上的 3 px 描边和透明背景。玩家位置使用细长 #c1ccd0 方向箭头，画布尺寸与 POI 一致（小地图 25 px、大地图 25 px），不带名称底板。内置地点标签读取游戏原生本地化表，未核实或暂不可用的文字隐藏。

地图窗口采用深棕金边外框、地图主区与独立标记侧栏。右侧顶部显示当前目标，下方编辑选中的个人标记，不再显示标记总览；滚轮与地图内加减按钮平滑缩放，边界允许时以鼠标位置为中心。地图铺满视口，拖动限制在地图边缘。地图地形继续使用原有本地配套文件。

主键盘 **-** 缩小小地图、**=** 放大，地图框大小不变。地图上方按“缩小 [-]　放大 [=]”分别配对动作与原生键帽，并纳入 HUD 避让。输入文字、原生菜单、大地图打开或小地图隐藏时不响应，不占用数字小键盘加减。显示半径在 20–1000 世界单位间分档切换，快捷键缩放保留本次会话；修改 MinimapRange 会重置范围，挪动/调整 HUD 大小则保留缩放。

- 约定 **+Z 为北、+X 为东**，并非已确认的原生地理北向。罗盘跟随镜头方向，显示 5° 短刻度、15° 中刻度和 45° 长刻度，使用固定图形指针与独立角度读数。
- 小地图默认固定正北，可在大地图按钮或配置切换随视角转动并保存选择；大地图始终固定正北。
- **M** 开关地图，**Esc** 或关闭按钮关图。输入标记名时 M 不触发开关。默认键与游戏 `UI.Map` 默认相同，但当前版本使用独立配置键，不自动跟随游戏改键。
- 滚轮缩放、左键拖动；空白地图处右键添加个人标记，再次右键其图标或可见名称即可删除。左键其图标或名称后在右栏编辑文字、颜色、图标和目标。“设为目标”下方的颜色、形状、删除使用一排图标；颜色与形状按钮展开下方格子，点击即应用保存。个人标记使用精灵图标，不依赖原生字体符号。固定 POI 不受右键删除影响。每个存储范围最多 512 个标记，名称最多 80 字符。
- 罗盘下方目标栏显示左右方向、水平距离及相对镜头的角差。距离按 X/Z 世界单位显示为米；**方位栏本身不提供高度、寻路或绕障路线**。
- 若从目标上方向下射线命中已加载的非触发碰撞体，另显示实际摄像机投影的菱形、名称和距离。仅有目标且大地图关闭时每秒最多采样一次；目标或场景变化清除缓存。未命中、位于镜头后方或靠近屏幕边缘时，仅保留方位栏。没有视线遮挡判断，命中可能落在屋顶或物体表面，并不保证是可行走地面；移动表面的缓存高度可能有延迟。不生成未加载地形。
- 开图释放鼠标并抑制角色操作；暂时停用原生 Settings/Close 操作以避免 Esc 同时打开暂停菜单，不暂停整个世界。原生窗口及加载画面优先。
- 只使用静态底图，不新建场景渲染相机、不额外加载区域、不发送游戏操作 RPC。

### 安装、升级、回退

正常退出游戏，备份旧 Navigation DLL、配置与标记。使用现有可工作的 BepInEx 5；本地开发基线为 5.4.23.4。解压后 DLL 应位于 `BepInEx/plugins/OldMarket.Navigation/OldMarket.Navigation.dll`。如持有本地生成的地图配套文件，把 JSON/PNG 放在同级 `maps/` 文件夹。公开包不提供这些派生文件。

升级仅替换本插件文件。卸载时退出游戏并移除 DLL，可按需删除该插件地图文件夹；保留 `BepInEx/config/OldMarket.Navigation/` 可保留个人标记，配置主文件为 `BepInEx/config/local.oldmarket.navigation.cfg`。回退恢复备份 DLL 和对应配置，不移除其他插件或加载器。插件不改写原始游戏组件或游戏存档。

### 配置与热更新

小地图默认位于左下角。原生 HUD 避让使用导航组件的实际屏幕范围，也计算可见的坐标和目标行。玩家加入等通知移到小地图上方并左对齐；右上任务卡无冲突时留在原位，顶部提示放在罗盘下方。保留游戏原有字体、内容与位置动画。关闭对应导航组件、关闭 `ReflowNativeHud` 或离开游戏场景后，恢复本插件修改的锚点。不会移动全屏菜单或 Steam、性能监控等外部覆盖层；长消息、原生动画及特殊分辨率仍需实机验收。

`MinimapBottomLeft=true` 使用 `MinimapLeft`/`MinimapBottom`，底部默认留出模式标签的空间；改成 `false` 可用旧的 `MinimapRight`/`MinimapTop` 切回右上角。旧布局文件没有这些新增字段时，也采用新的左下默认位置。每 0.1 秒最多检查一次原生内容，导航范围或屏幕尺寸改变时立即重排；极多通知无法找到完整空位时保留原位，不隐藏或缩小消息。

上方配置表列出了所有项目。小地图、罗盘、目标指引默认开启；导航内 XYZ 默认关闭，避免与独立坐标 Mod 重复。`CameraUp=false` 为固定正北，`MapKey=None` 禁用键盘开关。主 CFG 建议退出游戏后编辑，其说明为英语。

运行时可以编辑 `BepInEx/config/OldMarket.Navigation/layout.json`，上方 JSON 为默认值。每 0.5 秒检查一次；无效配置保留上一版布局。数值以 1920×1080 参考画布计算；`MinimapRange` 为世界单位半径，`WorldMarkerSize` 是沿用的字段名，实际控制目标栏及投影标记的文字大小。布局热更新不包含 C#、底图文件或游戏数据，替换底图后需重启。

### 地图状态与存储

本地地图按地图 ID、场景、区域和解锁集合匹配，显式保存 X/Z 边界及北向。解锁变体可共用同一绘画底图，并叠加准确障碍层，并非四幅各自编造地形的地图；匹配条件有歧义时拒绝选图。不能仅凭装饰画推断几何。地图图像及其游戏派生几何/元数据不进入公开仓库或发行包。目前本地证据范围为东方小镇及中央市场四个解锁状态，其余区域尚未验证。

读取原生同步的解锁集合，在解锁或场景事件后切换匹配底图；有对应变体时可反映市场垃圾消失。不会自动重绘任意玩家建筑、物品、植被或全部扩建。解锁变化不改变个人标记的存储范围。

标记写入 Mod 自己的 XML，按本机存档、地图、区域隔离，不读取序列化存档内容或写回游戏存档。本机身份包含槽位及目录创建时间，复制或重建存档目录可能改变身份。目标选择不持久化。

远程客户端的 `Markers.RemoteProfile` 默认留空，标记只保留本次连接；同次连接往返区域仍保留，断开或重启后清除。填写独立名称可启用远程持久化；不同主机存档不要使用同一个名称，否则会混用标记。标记不会共享给其他玩家。文件读取失败会禁止本连接该范围的写入，避免覆盖原文件。

### 语言、风险与验证

跟随游戏选定语言及金钱栏 TMP 字体，提供 13 种原创翻译：简中、繁中、英语、德语、法语、意大利语、日语、韩语、葡萄牙语、俄语、西班牙语、土耳其语、乌克兰语；未知语言回退英语。尚未经母语审校和所有字体实机检查。个人标记名与地图文件地名不会自动翻译。

固定游戏 **2.1.6**、Unity Mono **2022.3.62f3** 与编译 SDK **2.1.6/r7**，既有 Mod 保留 r1。CI 使用官方固定哈希的 BepInEx 5.4.23.5 引用；反射使用的存档槽、区域与扩建成员纳入 SDK 和真实程序集契约检查。无游戏环境可运行上文 `ci.py validate`、CI 工具测试和 `ci.py build`；匹配的本机安装可用 `ci.py verify-game --game-dir <path>` 核对真实引用符号 IL/资源与契约，不启动游戏。

构建命令见上文，测试需要 .NET 8 SDK，并只读引用游戏的 UnityEngine.PhysicsModule.dll。构建脚本先运行 Release 检查，通过后才打包。包输出为 `outputs/OldMarket.Navigation-0.1.4.zip`，打印 SHA256，不自动安装，严格只含 DLL、README、CHANGELOG、LICENSE。自动检查覆盖坐标转换/旋转边界、标记读写与隔离/异常、语言回退。编译成功不代表游戏内效果或无卡顿。

2.1.6 基线验证：SDK r7 与真实引用 Release 构建的符号 IL、程序集引用及内嵌资源一致；1397 项纯逻辑/本地化检查、553 项只读原生契约和 31 项 CI 工具测试通过，检查不执行 Unity 游戏代码。

实机待检查：字体遮挡、两种旋转模式、缩放拖动和标记编辑、Esc 不穿透暂停菜单、断线与区域旅行、市场各解锁阶段、帧耗时、单机/主机/客户端。游戏更新及其他 HUD/按键插件可能产生冲突，应在备份后的测试环境验收。

原创代码、文档与原创 UI 装饰素材采用 [MIT](LICENSE)，版本记录见 [CHANGELOG](CHANGELOG.md)。许可不覆盖游戏组件、原始资源、派生本地地图、商标或其他 Mod。未打包游戏或第三方运行库。

Phosphor POI icons: upstream MIT notices are included in [LICENSE](LICENSE); sources and regeneration instructions are included alongside the icon assets. No game artwork is included in these icon sources.

Phosphor POI 图标的上游 MIT 声明见 [LICENSE](LICENSE)，源文件与重建说明随图标素材保存，不包含游戏美术资源。

World and bearing target guidance hides while native modal or loading panels are open, and resumes when they close without clearing the selected target.

原生弹窗或加载面板打开时，世界目标与方位目标提示隐藏，关闭后自动恢复，所选目标保留。
