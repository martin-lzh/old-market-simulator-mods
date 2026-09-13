# Development notes / 开发说明

**2026-09-14 — maintainer confirmation:** Visual and other local in-game testing is complete; multiplayer testing is the only remaining test area. This records manual testing feedback; it is not an automated test result.

**2026-09-14 — 维护者确认：**视觉及其他本地实机测试已完成，目前只剩多人游戏测试。这是人工测试反馈，不是自动测试结果。

Player instructions: [README](README.md). Repository workflow: [CONTRIBUTING](../CONTRIBUTING.md), [SDK](../sdk/README.md), [releases](../releases/README.md). Earlier unchecked-scenario descriptions below are superseded by the confirmation above. Current test scope: [validation record](../releases/validation.md).

玩家用法见 [README](README.md)。本文保留构建、接口与历史测试资料；下方早期待测列表已由上述最新确认更新，当前实机范围以[验证记录](../releases/validation.md)为准。

### Layout hot reload

The minimap defaults to the lower-left corner. Native HUD reflow uses its rendered bounds, including the compass and its optional coordinate row. Notifications (including player-join messages) move above the minimap and align to the same side; the upper-right task card stays in place unless it conflicts. Top hints appear below the compass. The Mod retains native fonts, content and animation-controlled positions. Disabling the corresponding navigation area, disabling reflow or leaving gameplay restores owned anchor changes. Full-screen menus and external Steam/performance overlays are not moved. In-game animation, long-message and unusual-resolution acceptance remains outstanding.

Edit `BepInEx/config/OldMarket.Navigation/layout.json` while playing. Changes are checked every 0.5 seconds on the main thread. Malformed, oversized or out-of-range files retain the previous valid layout. UI position/size changes do not require a DLL rebuild. This does not hot-reload C# logic, map files or game data.

```json
{
  "MinimapSize": 240,
  "MinimapBottomLeft": true,
  "MinimapLeft": 24,
  "MinimapBottom": 48,
  "MinimapRight": 24,
  "MinimapTop": 150,
  "MinimapRange": 75,
  "CompassWidth": 520,
  "CompassTop": 22,
  "WorldMarkerSize": 28,
  "MapWidth": 1080,
  "MapHeight": 720,
  "Opacity": 0.92
}
```

Layout units use a 1920×1080 reference canvas. `MinimapRange` is the radius in world units. The historical field name `WorldMarkerSize` controls projected-marker icon and text size. Very small layouts and long translated text still require visual testing.

`MinimapBottomLeft=true` uses `MinimapLeft`/`MinimapBottom`; the default bottom margin leaves room for the mode label. Set it to `false` to use the legacy `MinimapRight`/`MinimapTop` upper-right placement. Existing layout files without the new fields use the new lower-left defaults. HUD content is remeasured at most every 0.1 seconds, with immediate reflow for navigation-bound or screen-size changes. If a message burst cannot fit anywhere without overlap, its original position is retained rather than hiding or shrinking it.

## Map data and expansion states

Local companion metadata matches map ID, scene, region and expansion IDs; each image includes explicit X/Z bounds and `North: "+Z"`. Unlock variants can share one painted base image and supply exact obstacle overlays; they are not four independently invented terrain layouts. Ambiguous matching metadata is rejected instead of choosing an arbitrary map. Map geometry is never inferred from decorative artwork alone. Game-derived map textures, geometry and metadata stay outside the public source/release. The optional local companion currently has evidence only for Eastern Town and the central market's four unlock states; other areas or changes in game content must be independently checked.

The runtime reads the replicated unlock set and selects matching variants after unlock or scene events, with a short delay for game activation. This supports removal of market clutter when the corresponding companion variant exists. It does not redraw arbitrary player buildings, items, vegetation or every expansion automatically. Map files load at plugin startup; restart after changing them. Expansion changes do not change marker storage scope.

## Build and validation

For a game-free CI build, run `python tools/ci.py validate`, `python -m unittest discover -s tools/tests -v`, and `python tools/ci.py build`. This runs Navigation tests and packages only the original DLL and documents. To compare with a matching installed game and execute read-only contracts, run `python tools/ci.py verify-game --game-dir <path>`. These checks do not launch Unity or constitute in-game acceptance.

Requirements for the local build below: Windows, .NET SDK (tests target .NET 8), local game assemblies (including the read-only `UnityEngine.PhysicsModule.dll` reference) and BepInEx 5. From the repository root:

```powershell
./navigation-mod/build.ps1
./navigation-mod/build.ps1 -GameDir 'D:\Path\To\Old Market Simulator'
dotnet run --project navigation-mod/tests/Navigation.Tests.csproj
```

Build output: `outputs/OldMarket.Navigation-0.2.0.zip`. The script runs the Release checks before building, prints the package SHA256 and installs nothing. The archive allowlist is exactly the plugin DLL, this README, CHANGELOG and LICENSE. It excludes game assemblies, loader files, map companions, saves, logs and backups.

Verified on the 2.1.6 baseline: SDK r7 and real-reference Release builds match in symbolic IL, assembly references and embedded resources; The 0.2.0 build passes 1225 navigation checks, 19 native-name checks, 635 read-only installed-assembly contracts and 48 CI tooling tests. No Unity game code was executed by these checks.

Automated coverage exercises coordinate conversion/rotation boundaries, marker serialization/isolation/error handling and locale fallback. Static review checked camera-up rotation, expansion event invalidation and Esc action handling. The maintainer has confirmed local in-game testing. A per-scenario record for all locales/resolutions, disconnect/region travel, each unlock transition, measured frame time and host/client combinations was not supplied; do not infer complete coverage from that confirmation. See [validation scope](../releases/validation.md).

### 地图状态与存储

本地地图按地图 ID、场景、区域和解锁集合匹配，显式保存 X/Z 边界及北向。解锁变体可共用同一绘画底图，并叠加准确障碍层，并非四幅各自编造地形的地图；匹配条件有歧义时拒绝选图。不能仅凭装饰画推断几何。地图图像及其游戏派生几何/元数据不进入公开仓库或发行包。目前本地证据范围为东方小镇及中央市场四个解锁状态，其余区域尚未验证。

读取原生同步的解锁集合，在解锁或场景事件后切换匹配底图；有对应变体时可反映市场垃圾消失。不会自动重绘任意玩家建筑、物品、植被或全部扩建。解锁变化不改变个人标记的存储范围。

标记写入 Mod 自己的 XML，按本机存档、地图、区域隔离，不读取序列化存档内容或写回游戏存档。本机身份包含槽位及目录创建时间，复制或重建存档目录可能改变身份。目标选择不持久化。

远程客户端的 `Markers.RemoteProfile` 默认留空，标记只保留本次连接；同次连接往返区域仍保留，断开或重启后清除。填写独立名称可启用远程持久化；不同主机存档不要使用同一个名称，否则会混用标记。标记不会共享给其他玩家。文件读取失败会禁止本连接该范围的写入，避免覆盖原文件。

### 语言、风险与验证

跟随游戏选定语言及金钱栏 TMP 字体，提供 13 种原创翻译：简中、繁中、英语、德语、法语、意大利语、日语、韩语、葡萄牙语、俄语、西班牙语、土耳其语、乌克兰语；未知语言回退英语。尚未经母语审校和所有字体实机检查。个人标记名与地图文件地名不会自动翻译。

固定游戏 **2.1.6**、Unity Mono **2022.3.62f3** 与编译 SDK **2.1.6/r7**，既有 Mod 保留 r1。CI 使用官方固定哈希的 BepInEx 5.4.23.5 引用；反射使用的存档槽、区域与扩建成员纳入 SDK 和真实程序集契约检查。无游戏环境可运行上文 `ci.py validate`、CI 工具测试和 `ci.py build`；匹配的本机安装可用 `ci.py verify-game --game-dir <path>` 核对真实引用符号 IL/资源与契约，不启动游戏。

构建命令见上文，测试需要 .NET 8 SDK，并只读引用游戏的 UnityEngine.PhysicsModule.dll。构建脚本先运行 Release 检查，通过后才打包。包输出为 `outputs/OldMarket.Navigation-0.2.0.zip`，打印 SHA256，不自动安装，严格只含 DLL、README、CHANGELOG、LICENSE。自动检查覆盖坐标转换/旋转边界、标记读写与隔离/异常、语言回退。编译成功不代表游戏内效果或无卡顿。

2.1.6 基线验证：SDK r7 与真实引用 Release 构建的符号 IL、程序集引用及内嵌资源一致；0.2.0 通过 1225 项导航检查、19 项原生名称检查、635 项只读原生契约和 48 项 CI 工具测试，检查不执行 Unity 游戏代码。

维护者已确认本地实机测试。尚未提供所有语言/分辨率、断线与区域旅行、各解锁阶段、量化帧耗时及房主/客户端组合的逐项记录，因此不将本次确认扩大为全部场景覆盖。详见[验证记录](../releases/validation.md)。

原创代码、文档与原创 UI 装饰素材采用 [MIT](LICENSE)，版本记录见 [CHANGELOG](CHANGELOG.md)。许可不覆盖游戏组件、原始资源、派生本地地图、商标或其他 Mod。未打包游戏或第三方运行库。

Phosphor POI icons: upstream MIT notices are included in [LICENSE](LICENSE); sources and regeneration instructions are included alongside the icon assets. No game artwork is included in these icon sources.

Phosphor POI 图标的上游 MIT 声明见 [LICENSE](LICENSE)，源文件与重建说明随图标素材保存，不包含游戏美术资源。

World and bearing target guidance hides while native modal or loading panels are open, and resumes when they close without clearing the selected target.

原生弹窗或加载面板打开时，世界目标与方位目标提示隐藏，关闭后自动恢复，所选目标保留。

Compass and 3D target guidance reuse the selected POI icon or personal marker shape and color, including its outline.

罗盘和 3D 目标指引复用所选 POI 图标或个人标记的形状、颜色和描边。
