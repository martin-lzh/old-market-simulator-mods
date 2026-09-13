# Changelog / 版本记录

## English

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
