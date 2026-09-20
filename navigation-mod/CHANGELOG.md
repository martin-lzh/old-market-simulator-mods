# Map & Compass — Changelog / 版本记录

## English

### Unreleased

- Reduce Rome's main-map coverage from 1005 × 1005 to 180 × 175 world units around the scene-reviewed castle walls, retaining all 59 unlock areas, 46 conditional POIs and seven separate travel maps. Crop base/overlay UVs against optional texture bounds so the existing artwork and detail patch keep their world alignment in both maps.
- Make one wheel notch and one + / − click use the same adaptive zoom factor (1.25 at the base range), retaining fractional scrolling, pointer anchoring, reversal and zoom limits. Keep Mod 0.2.0 and game 2.1.6 / SDK r7; these changes await in-game acceptance.

- Add in-game compass, minimap, Rome map, marker editor and target-tracking screenshots to both README languages. Exclude README cover/screenshots from embedded plugin resources; gameplay, version and SDK are unchanged.

- Move the Nexus cover into assets/cover.png and display it at the top of the README. Documentation only; runtime, version and SDK are unchanged.

### 0.2.0 — 2026-09-18 (stable release)

In-game acceptance is confirmed by the maintainer. This version is a stable release; see the [validation record](../releases/validation.md).

- Rename the plugin display name to **Map & Compass** and reorganize the bilingual README around core features and controls, with insertion points for gameplay media. Plugin ID, assembly/configuration paths, gameplay and version are unchanged.

- Regenerate Gate 4 artwork with its missing elevated lake, using a local geometry reference that clips the actual water mesh against terrain heights instead of assuming sea level. Preserve map bounds, return POI, version 0.2.0 and SDK 2.1.6/r7; refresh the packaged texture hash. Local in-game testing of corrected build `921d14b` is confirmed; this does not extend the multiplayer test scope.

- Complete localization of built-in map titles and functional POI labels across all 13 game languages. Add optional NameTextKey metadata, preserve custom names, update labels immediately on language changes, and support Macau Traditional Chinese aliases. Keep native place translations, version 0.2.0 and SDK 2.1.6/r7.

- Refresh bilingual map coverage, package and icon inventories, Rome unlock behavior and source-specific validation documentation. Make the unbundled Eastern Town outer terrain explicit; preserve historical test evidence and version/SDK pins.

- Add Rome and all seven travel-region maps, a calibrated town detail layer, and 61 independent unlock-area overlays (59 in town, two in the mine). Filter POIs by enable/disable ancestry, distinguish locked gates from open portals, and retain region-scoped markers and return entrances. Preserve SDK 2.1.6/r7 and version 0.2.0.

- Audit Eastern Town services beyond the former fixed NPC list. Add recycling, employees, water refill and calendar to all four market manifests (17 POIs); keep aquarium and origami collection grouped under the existing museum POI. Add water/calendar icons without inventing native place labels. Record expansion-sign and fishing-area audit limits; no version or SDK change.

- Add the first Island map (BazaarIsland, map ID 0), a full-terrain Image Gen illustration and 16 scene-verified POIs. Add filled employee, expansion and recycling icons with native labels and category colors. Ship the map through the hashed allowlist. This is a static surface overview, not all expansion/underground states. Keep 0.2.0 and SDK 2.1.6/r7 unchanged.

- Scale large-map wheel/button zoom steps and the maximum zoom by world coverage and viewport aspect, preserving close-up detail and approximately the same input travel for larger maps. Minimap steps remain world-distance based. SDK 2.1.6/r7 and Mod version are unchanged.

- Include the Eastern Town base map, four unlock overlays and four map/POI manifests in the Navigation release ZIP. Maps are tracked Mod resources and install with the DLL; no separate map download is needed.
- Share the explicit map allowlist between CI and local packaging. Verify hashes, PNG integrity, texture references and metadata/POIs, and include map hashes in build evidence. Reject missing, modified or unexpected ZIP entries; continue excluding SDK/game/loader binaries and personal files.
- Consolidate the complete map, UI and localization scope into the assigned 0.2.0 first public release. Retain game 2.1.6 / SDK r7; a new source-bound record replaces the obsolete development candidate for publication.

- Reuse the same colored, outlined POI/personal sprite on the compass and in world guidance; keep the entire 25 px compass icon within its strip.

- Remove the separate top target name/distance/bearing panel; retain the compass target marker and projected world guidance.

- Reuse the selected POI or personal marker sprite for 3D target guidance, preserving its semantic shape, color and outline instead of a generic diamond.

- Start the minimap at a 75-unit radius, one zoom-in step from the previous 100-unit default.

- Hide world and bearing target guidance while native modal/loading panels are open, using the same native panel gate as map input. Closing the panel restores guidance without clearing the selected target.

- Increase source icon outlines to 3 px and minimap POI, personal marker and player canvases to 25 px; adjust circular bounds and label collision spacing. Preserve the approved palette and 25 px large-map icons.

- Add a filled identification-card symbol for local POIs using the native licenses key. Shop companion coordinates can be anchored to verified NPC transforms; scene-derived coordinates remain outside the public package.

- Replace personal marker font symbols with centered sprites in both maps. Keep Set target as text, place color/icon/delete actions in one icon row, and expand explicit color or shape slots below it. Clicking a slot saves immediately and highlights the selection; existing marker IDs and save format remain compatible.

- Give built-in POIs distinct filled Phosphor symbols while retaining category colors. Left/right-click a POI icon or visible label to set it as the navigation target; large-map gestures continue through POI nodes. Include upstream icon notices in the packaged license.

- Apply the selected muted POI palette: shop #ad7568, home #829278, orders #77929d, other #978190 and player #c1ccd0. Use 3 px source outlines and 25 px player/POI canvases in both maps.

- Remove the player position nameplate and use a narrow, outlined direction arrow in both maps. Color POI sprites by category with dark outlines and transparent backgrounds, including the large map. Resolve built-in POI labels from native game text instead of Mod translations.

- Add the missing minimap POI layer with upright semantic icons and collision-aware labels in both rotation modes; resolve POI targets for compass guidance. Share icons with the large map. Verify nested POI metadata with the same .NET reader used by the plugin and log loaded/rendered counts for diagnosis. Local in-game testing is confirmed; see [validation scope](../releases/validation.md).

- Right-click a personal marker icon or label to remove it; left-click to edit. Remove the icon backdrop and overview list; place current target above the marker editor. Pair minimap action labels individually with their minus/equals keycaps.

- Pin game 2.1.6 SDK r7 for the actual UI scroll multiplier, runtime input range and platform APIs. Version 0.2.0 passes 1225 navigation checks, 19 native-name checks, 635 metadata/IL contracts, 48 CI tooling tests and real-reference/SDK builds; local in-game testing is confirmed, with scenario limits recorded in [validation scope](../releases/validation.md).

- Normalize wheel input by the active UI module scale, preserve fractional movement and cancel pending opposite zoom; +/- button behavior is unchanged.
- Read optional validated local POIs and render semantic shop/home/dock icons. Labels expand with zoom when space permits, avoid overlaps and prioritize the selected target; POIs can be navigation targets without entering personal marker files.

- Keep the map north label above its arrow in a taller, non-ellipsized text box for native CJK font metrics.

### 0.1.4 — 2026-09-13

- Pin game 2.1.6 SDK r6 for verified map control APIs; pass 894 navigation checks, 489 installed-assembly contracts and all seven SDK/real-reference comparisons. Isolate the two test projects' restore, intermediate and output directories to prevent executable apphost cache collisions.

- Rework the map window around a dark brown and gold frame, centered title, map controls and a separate marker list and target sidebar.
- Cover the map viewport and clamp panning to map edges; smooth wheel/button zoom around the pointer where boundaries allow.
- Preserve local map geometry, unlock layers and personal marker storage. In-game visual and interaction acceptance remains pending.

### 0.1.3 — 2026-09-13

- Pin game 2.1.6 SDK r5 for verified text focus and UI layout APIs; pass 606 navigation checks, 440 installed-assembly contracts and all seven SDK/real-reference comparisons.

- Replace the compass's missing-font arrow with a generated triangle, add scrolling 5/15/45-degree ticks, and separate direction labels from the heading readout.
- Fix narrow inherited prefab layout turning minimap zoom descriptions into vertical text; use an explicitly sized horizontal native-style keycap row.
- Skip null entries in keyboard.allKeys: the installed 0.1.2 DLL and Unity log identify a per-frame null reference before the M-key handler. Check actual input-field focus and log blocked map presses, preserving native modal/loading protections and Escape restoration.
- Follow up on user-provided 0.1.2 screenshots; the repaired build still requires in-game visual and M-key acceptance.

### 0.1.2 — 2026-09-13

- Add main-row minus/equals minimap zoom shortcuts with bounded range steps; ignore typing, native menus, the large map and hidden minimaps. Numpad add/remove bindings remain separate.
- Clone native keycap/description prefabs above the minimap, localize the hint in all 13 languages and include it in notification avoidance bounds.
- Keep shortcut zoom for the current session; a changed MinimapRange in layout JSON resets it, while unrelated layout changes preserve it. In-game validation remains outstanding.

### 0.1.1 — 2026-09-13

- Default the minimap to the lower-left corner; retain optional upper-right placement through layout JSON.
- Reflow native notifications above the minimap, move conflicting task cards nearby, and place top hint banners below the compass and its visible coordinate/target rows.
- Measure visible UI in screen space across canvas scales; preserve native text and tweened positions by translating anchors, then restore owned anchor changes when navigation is hidden or disabled.
- Add the default-enabled `Display.ReflowNativeHud` option and respond to layout hot reload, screen size and native content changes.
- Pin game 2.1.6 compilation SDK r3 for verified HUD APIs. Build/contract checks do not replace in-game layout and notification-animation acceptance; keep prerelease status.

### 0.1.0 — 2026-09-13

Experimental initial build; not installed or verified in game.

- Pin game 2.1.6 compilation SDK r2 and register the BepInEx variant in CI, including metadata-only reflection declarations and read-only game contracts. SDK verification does not replace in-game acceptance.

- Add an independent compass, north-up/camera-up minimap and M-key map window.
- Add zoom, pan, personal marker editing, target bearing/distance and optional XYZ.
- Add actual-camera target projection only after a bounded, once-per-second raycast hits a loaded collision surface; keep bearing-only fallback, with no occlusion or walkability guarantee.
- Add save/map/region marker isolation, session-only remote markers and explicit remote profiles.
- Select local map variants using replicated expansion state, without changing marker scope.
- Add embedded UI decorations, layout JSON hot reload and 13-language UI text.
- Guard player/cursor ownership and defer restoration of native Escape actions after closing.
- Keep game-derived map companions out of the public DLL and archive. No map data is bundled.
- Pass SDK/real-reference symbolic IL/resource equivalence, 511 pure checks and 318 read-only game contracts on game 2.1.6; 22 CI tooling tests pass.
- Supply coordinate, persistence and localization checks. In-game appearance, input, performance and multiplayer testing remain outstanding.

## 中文

### 未发布

- 将罗马主地图从 1005 × 1005 世界单位收紧至场景核对的城墙周边 180 × 175，保留全部 59 个解锁区、46 条条件 POI 和七张独立传送区域地图。通过可选贴图边界裁剪底图/覆盖层 UV，让大小地图中的原有插画及细节图保持世界坐标对齐。
- 滚轮一格与 + / − 按钮一次点击使用相同的自适应缩放倍率（基础范围放大 1.25 倍），保留小数滚动、鼠标锚点、反向取消和缩放边界。保持 Mod 0.2.0 与游戏 2.1.6 / SDK r7，本次改动仍待实机验收。

- 在双语 README 中加入罗盘轴、小地图、罗马大地图、标记编辑及目标跟踪实机截图；将 README 封面及截图排除出插件嵌入资源，玩法、版本及 SDK 不变。

- 将 Nexus 封面移入 assets/cover.png，并作为 README 首图展示。仅文档改动，运行时、版本和 SDK 不变。

### 0.2.0 — 2026-09-18（正式发布）

维护者已确认实机验收完成，本版本为正式发布版，详见[验证记录](../releases/validation.md)。

- 插件显示名改为 **Map & Compass**，双语 README 按核心功能和操作方式组织，并保留实机素材插入位置。插件 ID、程序集／配置路径、玩法及版本不变。

- 重新生成大门4底图，补回遗漏的高处湖泊；本地参考图按真实水面网格与地形高度裁剪，不再假定海平面。地图边界、返回点、版本 0.2.0 和 SDK 2.1.6/r7 不变，更新打包贴图哈希。修正构建 `921d14b` 已获本地实机测试确认，不据此扩展联机测试范围。

- 补齐游戏全部 13 种语言的内置地图标题及功能 POI 标签。增加可选 NameTextKey 元数据，保留自定义名称，切换语言即时更新，补充澳门繁中别名。保留原生地点译文、0.2.0 版本与 SDK 2.1.6/r7。

- 更新双语地图覆盖、地图包与图标清单、罗马解锁行为及按源码区分的验证说明。明确东方小镇外围地形尚未打包，保留历史测试证据及版本/SDK 固定值。

- 加入罗马主城与七个独立传送区域、主城细节层及 61 个独立解锁区块（主城 59、矿洞 2）。POI 按启停条件更新，区分锁定大门与开放传送入口，保留按区域隔离的个人标记及返回入口。SDK 2.1.6/r7 与版本 0.2.0 不变。

- 将东方小镇复核范围从原先固定 NPC 名单扩展到交互组件，四份市场地图补充回收、雇员、补水、日历，共 17 个 POI；水族馆与折纸收集共用原有博物馆点位。新增水滴/日历图标，不编造原生地点名；记录扩建购买牌与钓鱼区域的审核边界。版本与 SDK 不变。

- 补入第一个海岛地图（BazaarIsland，地图 ID 0）、完整地形范围的 Image Gen 插画及 16 个经场景核对的 POI。新增雇员、扩建、回收实心图标，复用原生名称及类别配色；地图纳入哈希白名单打包。这是静态地表概览，不覆盖全部扩建与地下状态。保持 0.2.0 与 SDK 2.1.6/r7 不变。

- 大地图滚轮、按钮缩放步长与放大上限随世界覆盖范围和视口比例调整，让扩大后的地图保持近景细节与近似操作次数。小地图仍按世界距离分档。SDK 2.1.6/r7 与 Mod 版本不变。

- 将东方小镇底图、四种解锁覆盖层及四份地图/POI 元数据纳入 Navigation 发行 ZIP。地图作为 Mod 资源跟踪，与 DLL 一起安装，无需另行下载地图。
- CI 与本地构建共用明确的地图文件白名单，检查哈希、PNG 完整性、贴图引用和地图/POI 元数据，并在构建记录中保存地图哈希。拒绝遗漏、修改或额外 ZIP 条目，继续排除 SDK、游戏、加载器程序集及个人文件。
- 将完整地图、UI 及本地化范围整理到已指定的首次公开版本 0.2.0，保持游戏 2.1.6 / SDK r7；使用新的源码绑定记录发布，旧开发候选记录继续归档。

- 罗盘目标标记与 3D 指引复用同一 POI/个人标记图标、颜色和描边，25 px 图标完整限制在罗盘条内。

- 移除顶部独立的目标名称、距离与方位提示栏，保留罗盘目标标记和 3D 场景指引。

- 3D 目标指引复用所选 POI 或个人标记的图标、颜色和描边，不再统一显示菱形。

- 小地图默认显示半径改为 75 世界单位，相当于原先 100 默认值按一次放大。

- 原生弹窗及加载面板打开时隐藏世界目标与方位目标提示，复用地图输入的原生面板判定；关闭面板后恢复指引，不清除所选目标。

- 图标源纹理描边加至 3 px，小地图 POI、个人标记与玩家图标统一为 25 px，并调整圆形边界和名称避让间距。保持已选配色及大地图 25 px 图标。

- 原生 licenses 词条对应的本地 POI 增加实心证件图标；店铺配套坐标可依据经核实的 NPC Transform 定位，场景派生坐标不进入公开安装包。

- 个人标记在两张地图中改用居中精灵图标，避免字体符号被裁切或缺失。保留“设为目标”文字按钮，颜色、图标、删除合并为一排图标操作，下方展开颜色或形状格子；点选立即保存并突出选中项，保持现有标记 ID 与存储格式兼容。

- 内置 POI 使用不同 Phosphor 实心图形，同类别保持同颜色。左右键点击 POI 图标或可见名称均可设为导航目标，大地图图标节点继续转发拖动与滚轮；发行包许可包含上游图标声明。

- 应用选定 POI 配色：商店 #ad7568、房屋 #829278、订购 #77929d、其他 #978190、玩家 #c1ccd0。源纹理使用 3 px 描边，两张地图中的玩家与 POI 画布尺寸统一为 25 px。

- 移除玩家位置文字，两张地图改用细长描边方向箭头。POI 图标按类别着色并加深色描边，大地图也去除图标底板。内置地点标签改从游戏原生文本读取，不再使用 Mod 自译名称。

- 补齐小地图 POI 图层，两种朝向模式下语义图标和名称保持正立并按空间避让；罗盘指引支持 POI 目标。大小地图共用图标，通过插件实际使用的 .NET 读取器验证嵌套 POI 数据，并记录读取与绘制数量用于排查。本地实机测试已确认，范围见[验证记录](../releases/validation.md)。

- 右键个人标记图标或名称即可删除，左键选中编辑；去掉图标底色和标记总览，将当前目标放在编辑区上方。小地图缩放提示改为“缩小 [-]　放大 [=]”两组动作与键帽。

- 固定游戏 2.1.6 SDK r7，覆盖实际 UI 滚轮倍率、运行时输入范围与平台接口。0.2.0 通过 1225 项导航检查、19 项原生名称检查、635 项元数据/IL 契约、48 项 CI 工具测试及真实引用/SDK 构建；本地实机测试已确认，场景覆盖限制见[验证记录](../releases/validation.md)。

- 按当前 UI 模块系数还原滚轮输入，保留小数滚动并取消反方向的剩余缩放；加减按钮行为不变。
- 读取经校验的本地 POI，绘制商店、房屋、码头等语义图标；放大且空间足够时展开名称，避让重叠并优先显示目标。POI 可设为导航目标，不写入个人标记文件。

- 调整大地图北向标识的文字区域高度，禁用省略裁切，在箭头上方显示“北”等本地化方位文字，兼容原生中文字体行高。

### 0.1.4 — 2026-09-13

- 固定游戏 2.1.6 SDK r6，覆盖已核对的地图控件接口；通过 894 项导航检查、489 项原生程序集契约和全部七个 SDK/真实引用构建对比。将两个测试项目的还原、中间及输出目录隔离，修复 apphost 缓存相互覆盖。

- 重排大地图弹窗：深棕金边外框、居中标题、地图内操作按钮，以及独立的标记列表与当前目标侧栏。
- 地图铺满视口，拖动限制在边缘；滚轮及按钮平滑缩放，边界允许时保持鼠标指向的地图位置。
- 保留本地地形、解锁叠层和个人标记存储，游戏内视觉与交互仍待验收。

### 0.1.3 — 2026-09-13

- 固定游戏 2.1.6 SDK r5，覆盖已核对的文字焦点及布局接口；通过 606 项导航检查、440 项原生程序集契约和全部七个 SDK/真实引用构建对比。

- 罗盘缺字箭头改为程序三角图形，新增随视角移动的 5/15/45 度刻度，将方位文字与角度读数分行排列。
- 修复继承原生预制体窄布局导致缩放说明竖排的问题，改为明确尺寸的横向原生风格键帽行。
- 跳过 keyboard.allKeys 中的空项：已安装 0.1.2 DLL 与 Unity 日志对应确认，每帧空引用导致 M 键处理前中断。改用输入框实际焦点判断，记录按键拦截原因，保留原生窗口、加载保护与 Esc 恢复逻辑。
- 根据用户提供的 0.1.2 实机截图修复；新版仍需验证游戏内布局及 M 键操作。

### 0.1.2 — 2026-09-13

- 增加主键盘减号/等号小地图缩放，按有限范围分档切换；输入文字、原生菜单、大地图及隐藏小地图时不触发，不复用数字小键盘加减。
- 在小地图上方复用原生键帽与说明预制体，提供 13 种语言提示，并将其纳入通知避让范围。
- 快捷键缩放保留于本次会话；修改布局 JSON 的 MinimapRange 会重置范围，其他布局修改保留当前缩放。尚待实机验收。

### 0.1.1 — 2026-09-13

- 小地图默认放在左下角；布局 JSON 仍可切回右上角。
- 将原生通知列表放到小地图上方，将冲突的任务卡安排在附近，顶部提示条移到罗盘及可见坐标/目标行下方。
- 按屏幕空间测量原生 UI，兼容不同画布缩放；通过平移锚点保留原生文字及位置动画，导航隐藏或停用后恢复本插件修改的锚点。
- 新增默认开启的 `Display.ReflowNativeHud`，响应布局热更新、屏幕尺寸与原生内容变化。
- 为已核对的 HUD 接口固定游戏 2.1.6 编译 SDK r3；构建与契约检查不能替代游戏内布局及通知动画验收，仍保持预发布。

### 0.1.0 — 2026-09-13

实验性首版；尚未安装或完成实机验证。

- 固定游戏 2.1.6 编译 SDK r2，将 BepInEx 变体接入 CI，包含仅声明的反射依赖及只读游戏契约验证；SDK 检查不替代实机验收。

- 新增独立罗盘、固定正北/随视角小地图与 M 键大地图。
- 支持缩放、拖动、个人标记编辑、目标方位/距离及可选 XYZ。
- 目标每秒最多一次向下射线命中已加载碰撞体后，使用实际摄像机投影；否则回退方位栏，不保证遮挡判断或地面可行走。
- 按存档/地图/区域隔离标记，远程默认仅本次连接，可显式指定持久化身份。
- 按同步的扩建解锁状态选取本地地图变体，不改变标记范围。
- 提供内嵌 UI 装饰、布局 JSON 热更新及 13 种语言。
- 限定角色/鼠标控制权恢复，关图后延迟恢复原生 Esc 操作。
- 游戏派生底图配套文件不进入公开 DLL 或发行包；包内不含地图数据。
- 游戏 2.1.6 上通过 SDK/真实引用符号 IL 与资源等价、511 项纯检查、318 项只读原生契约及 22 项 CI 工具测试。
- 提供坐标、存储及本地化检查；视觉、输入、性能和联机仍待实机验收。
