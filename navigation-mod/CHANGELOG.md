# Changelog / 版本记录

## English

### Unreleased

- Keep the map north label above its arrow in a taller, non-ellipsized text box for native CJK font metrics. Existing Mod version and SDK r6 are unchanged.

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

- 调整大地图北向标识的文字区域高度，禁用省略裁切，在箭头上方显示“北”等本地化方位文字，兼容原生中文字体行高。保持现有 Mod 版本和 SDK r6。

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
