# Development notes / 开发说明

**Validation scope, 2026-09-15:** The maintainer confirmed visual, local and two-computer multiplayer testing for bundle `e23f986`. Later zoom, Island, Eastern Town POI and Rome changes are Unreleased and still require in-game acceptance. Automated results below identify their source; retaining version 0.2.0 does not extend the earlier manual test coverage.

**验证范围，2026-09-15：**维护者已确认 `e23f986` 合集的视觉、本地实机和双实机联机测试。后续缩放、海岛、东方小镇 POI 与罗马改动记入未发布，仍待实机验收。下方自动验证结果注明对应源码；保留 0.2.0 版本号不代表后续改动已被此前人工测试覆盖。

Player instructions: [README](README.md). Repository workflow: [CONTRIBUTING](../CONTRIBUTING.md), [SDK](../sdk/README.md), [releases](../releases/README.md). Historical confirmations apply only to the tested source. Current test scope: [validation record](../releases/validation.md).

玩家用法见 [README](README.md)。本文保留构建、接口与历史测试资料；历史确认仅适用于当时源码，当前实机范围以[验证记录](../releases/validation.md)为准。

### Layout hot reload

The minimap defaults to the lower-left corner. Native HUD reflow uses its rendered bounds, including the compass and its optional coordinate row. Notifications (including player-join messages) move above the minimap and align to the same side; the upper-right task card stays in place unless it conflicts. Top hints appear below the compass. The Mod retains native fonts, content and animation-controlled positions. Disabling the corresponding navigation area, disabling reflow or leaving gameplay restores owned anchor changes. Full-screen menus and external Steam/performance overlays are not moved. The manual report does not provide an exhaustive animation, long-message or resolution matrix.

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

Local companion metadata matches map ID, scene, region and expansion IDs; each image includes explicit X/Z bounds and `North: "+Z"`. Unlock variants can share one painted base image and supply exact obstacle overlays; they are not four independently invented terrain layouts. Ambiguous matching metadata is rejected instead of choosing an arbitrary map. Map geometry is never inferred from decorative artwork alone. The reviewed map pack is tracked under `maps/` and ships in the public ZIP; `map-pack.json` pins every allowed JSON/PNG and SHA256. Original game assemblies, raw assets and extraction snapshots remain excluded. The pack contains Eastern Town with four central-market states, the Island static surface overview, and Rome with eight region maps. See the [map inventory and coverage limits](maps/README.md). Eastern Town still uses X [-120,220], Z [40,300]; the expanded outer-terrain render is not bundled. Image Gen alignment remains approximate until checked in-game.

The runtime reads the replicated unlock set and selects matching variants after unlock or scene events, with a short delay for game activation. Eastern Town switches market obstacle layers. Rome selects the exact local-player region, filters conditional POIs and removes hatched unlock-area rectangles as their IDs unlock; overlapping pending areas remain shaded. Its town detail patch has its own world bounds. Invalidated POI targets are cleared, while expansion changes preserve personal-marker storage scope. These overlays indicate affected surface extents, not intermediate building models, walking routes or arbitrary player placements. Map files load at plugin startup; restart after changing them.

## Build and validation

For a game-free CI build, run `python tools/ci.py validate`, `python -m unittest discover -s tools/tests -v`, and `python tools/ci.py build`. This runs Navigation tests and packages the original DLL, documents and reviewed map pack. To compare with a matching installed game and execute read-only contracts, run `python tools/ci.py verify-game --game-dir <path>`. These checks do not launch Unity or constitute in-game acceptance.

Requirements for the local build below: Windows, Python 3.12, .NET SDK (tests target .NET 8), local game assemblies (including the read-only `UnityEngine.PhysicsModule.dll` reference) and BepInEx 5. From the repository root:

```powershell
./navigation-mod/build.ps1
./navigation-mod/build.ps1 -GameDir 'D:\Path\To\Old Market Simulator'
dotnet run --project navigation-mod/tests/Navigation.Tests.csproj
```

Build output: `outputs/OldMarket.Navigation-0.2.0.zip`. The script runs the Release checks before building, prints the package SHA256 and installs nothing. The archive allowlist contains the plugin DLL, README, CHANGELOG, LICENSE and the 28 map files (13 JSON and 15 PNG) named in `map-pack.json`. Local and CI builds use `tools/ci.py package` / `write_package` for identical packaging. Validation checks resource hashes, PNG integrity, map bounds and texture references; the plugin's own metadata reader validates each packaged map and its POIs. Game assemblies, loaders, saves, logs and backups remain excluded.

Verified for source `db03d32` on game 2.1.6 / SDK r7: version validation, all SDK builds, 64 CI tooling tests, 1291 navigation checks, 22 native-name checks and 659 Navigation installed-assembly contracts passed. Navigation real-reference and SDK builds matched after all 15 dependency hashes were verified. The repository-wide real-reference run stopped at Stack All differences; Navigation was verified independently. See the [Rome audit](maps/rome-audit.md). These are recorded results, not new checks run by this documentation update; no Unity game code was executed.

Automated coverage exercises coordinate conversion/rotation boundaries, marker serialization/isolation/error handling and locale fallback. Static review checked camera-up rotation, expansion event invalidation and Esc action handling. The maintainer confirmed local and two-computer multiplayer testing of `e23f986`, before the later map and zoom changes. A per-scenario record for all locales/resolutions, disconnect/region travel, each unlock transition, measured frame time and host/client combinations was not supplied; do not infer complete coverage from that confirmation. See [validation scope](../releases/validation.md).

### 地图状态与存储

本地地图按地图 ID、场景、区域和解锁集合匹配，显式保存 X/Z 边界及北向。解锁变体可共用同一绘画底图，并叠加准确障碍层，并非四幅各自编造地形的地图；匹配条件有歧义时拒绝选图。不能仅凭装饰画推断几何。经核对的地图包纳入 `maps/` 并随公开 ZIP 发行；`map-pack.json` 逐项固定 JSON/PNG 文件及 SHA256。原始游戏程序集、资源和提取快照仍不入包。当前地图包包含东方小镇中央市场四态、海岛静态地表概览和罗马八个区域地图，详见[地图清单与覆盖边界](maps/README.md)。东方小镇仍使用 X [-120,220]、Z [40,300]，外围完整地形渲染尚未打包；Image Gen 对齐仍待实机验证。

读取原生同步的解锁集合，在解锁或场景事件后稍作延迟，等待游戏激活状态更新，再切换匹配地图。东方小镇切换市场障碍层；罗马按本地玩家的精确区域名选图，筛选条件 POI，并移除已解锁 ID 的斜线矩形，重叠的未解锁范围仍保留阴影。主城细节图使用独立世界边界。失效 POI 目标会清除，解锁变化不改变个人标记的存储范围。区块仅提示受影响地表范围，不重建中间阶段建筑、可行走路线或玩家摆放物。地图文件在插件启动时加载，修改后需重启。

标记写入 Mod 自己的 XML，按本机存档、地图、区域隔离，不读取序列化存档内容或写回游戏存档。本机身份包含槽位及目录创建时间，复制或重建存档目录可能改变身份。目标选择不持久化。

远程客户端的 `Markers.RemoteProfile` 默认留空，标记只保留本次连接；同次连接往返区域仍保留，断开或重启后清除。填写独立名称可启用远程持久化；不同主机存档不要使用同一个名称，否则会混用标记。标记不会共享给其他玩家。文件读取失败会禁止本连接该范围的写入，避免覆盖原文件。

### 语言、风险与验证

跟随游戏选定语言及金钱栏 TMP 字体，提供 13 种原创翻译：简中、繁中、英语、德语、法语、意大利语、日语、韩语、葡萄牙语、俄语、西班牙语、土耳其语、乌克兰语；未知语言回退英语。尚未经母语审校和所有字体实机检查。已确认的 POI NameKey 使用游戏原生本地化；休息处、市场、补水处、日历和返回入口使用 Mod 原创功能标签。地图元数据可通过 NameTextKey 使用 Mod 的 13 语种标题，语言切换即时更新；未设置或无法识别的键保留自定义 Name，空名称使用本地化的“地图”。个人标记和自定义字面名称保留原文。

固定游戏 **2.1.6**、Unity Mono **2022.3.62f3** 与编译 SDK **2.1.6/r7**，既有 Mod 保留 r1。CI 使用官方固定哈希的 BepInEx 5.4.23.5 引用；反射使用的存档槽、区域与扩建成员纳入 SDK 和真实程序集契约检查。无游戏环境可运行上文 `ci.py validate`、CI 工具测试和 `ci.py build`；匹配的本机安装可用 `ci.py verify-game --game-dir <path>` 核对真实引用符号 IL/资源与契约，不启动游戏。

构建命令见上文，测试需要 .NET 8 SDK，并只读引用游戏的 UnityEngine.PhysicsModule.dll。构建脚本先运行 Release 检查，通过后才打包。包输出为 `outputs/OldMarket.Navigation-0.2.0.zip`，打印 SHA256，不自动安装，包含 DLL、README、CHANGELOG、LICENSE 及 `map-pack.json` 列出的 28 个地图文件（13 份 JSON、15 张 PNG）。本地脚本和 CI 共用 Python 打包器；校验哈希、PNG 完整性、地图边界和贴图引用，并使用插件自身的读取器检查地图及 POI。自动检查覆盖坐标转换/旋转边界、标记读写与隔离/异常、语言回退。编译成功不代表游戏内效果或无卡顿。

源码 `db03d32` 在游戏 2.1.6 / SDK r7 基线上通过版本验证、全部 SDK 构建、64 项 CI 工具测试、1291 项导航检查、22 项原生名称检查和 659 项 Navigation 原生契约检查。核对 15 个依赖哈希后，Navigation 的真实引用与 SDK 构建一致。全仓真实引用检查先在 Stack All 差异处停止，随后独立验证 Navigation，详见[罗马审计](maps/rome-audit.md)。这是已记录结果，不是本次文档修改重新运行的检查；不执行 Unity 游戏代码。

维护者已确认 `e23f986` 的本地实机和双实机联机测试，但不涵盖后续地图及缩放改动。尚未提供所有语言/分辨率、断线与区域旅行、各解锁阶段、量化帧耗时及房主/客户端组合的逐项记录，因此不将本次确认扩大为全部场景覆盖。详见[验证记录](../releases/validation.md)。

原创代码、文档、UI 装饰素材及随包地图的原创部分采用 [MIT](LICENSE)，版本记录见 [CHANGELOG](CHANGELOG.md)。许可不覆盖游戏组件、原始资源、商标或其他 Mod。未打包游戏或第三方运行库。

Phosphor POI icons: upstream MIT notices are included in [LICENSE](LICENSE); sources and regeneration instructions are included alongside the icon assets. No game artwork is included in these icon sources.

Phosphor POI 图标的上游 MIT 声明见 [LICENSE](LICENSE)，源文件与重建说明随图标素材保存，不包含游戏美术资源。

World and bearing target guidance hides while native modal or loading panels are open, and resumes when they close without clearing the selected target.

原生弹窗或加载面板打开时，世界目标与方位目标提示隐藏，关闭后自动恢复，所选目标保留。

Compass and 3D target guidance reuse the selected POI icon or personal marker shape and color, including its outline.

罗盘和 3D 目标指引复用所选 POI 图标或个人标记的形状、颜色和描边。

## Island map preview / 海岛地图预览

The first map is BazaarIsland, ID 0. See [POI coordinates and evidence](maps/island-pois.md) for its 16 fixed anchors, native keys, categories and icons. The full-terrain illustration is a static surface overview; it is not an expansion-state or underground-floor map. Game 2.1.6 / SDK r7 stays unchanged: no new game, Unity or network interfaces are called. Image Gen registration is approximate and requires in-game acceptance.

第一张地图为 BazaarIsland、ID 0；[POI 坐标记录](maps/island-pois.md)列出 16 个固定锚点及类别图标。新增的是完整地形范围的静态地表图，不是全部扩建或地下楼层图。沿用游戏 2.1.6 / SDK r7，没有新增游戏、Unity 或网络接口调用；生成图对齐仍需实机验收。

Historical Island introduction (`5d328aa`, 2026-09-15): version/map-hash validation, 61 CI tooling tests, all SDK builds and 1263 Navigation checks passed (including five packaged manifests). Icon generation verified 72 category variants and 18 distinct silhouettes. No new SDK snapshot, game installation changes, in-game tests or multiplayer tests were performed.

海岛首次纳入时的历史结果（`5d328aa`，2026-09-15）：版本/地图哈希校验、61 项 CI 工具测试、全部 SDK 构建及 1263 项导航检查通过（包含五份打包地图）。图标生成检查 72 个类别变体和 18 个独立轮廓。未新增 SDK 快照，未修改游戏安装，未执行实机与联机测试。

## Rome maps / 罗马地图

Rome uses a town map plus two caravan regions, an engineer mine and four gate regions. The town contains 46 conditional POI records (15 initially visible, 39 fully unlocked); each travel region has a return portal. There are 61 independent unlock IDs across 59 town areas and two mine areas. Gate state distinguishes blocked and open entrances; caravan availability does not waive the native fare. Selecting a target does not teleport or unlock an area. [Region, POI and expansion evidence](maps/rome-audit.md).

罗马包含主城、两个商队区域、工程师矿洞及四个大门区域。主城有 46 条条件 POI 记录（初始显示 15 个，全部解锁后显示 39 个），每个传送区域都有返回入口。59 个主城区块与两个矿洞区块对应 61 个独立解锁 ID。大门区分锁定与开放状态，商队可用图标不免除原生车费；选择目标不会传送或解锁区域。[区域、POI 与扩建证据](maps/rome-audit.md)。

## Localization / 本地化

Game 2.1.6 has 13 locale assets: `en`, `zh`, `zh-Hant`, `de`, `fr`, `it`, `ja`, `ko`, `pt`, `ru`, `es`, `tr`, `uk`. Navigation supplies every UI and built-in map-title key in each locale. `NameTextKey` in map metadata selects an original Mod translation; absent or unknown keys preserve a custom `Name`, with a localized generic map title when empty. Functional POI aliases for rest, market, water, calendar and return use original translations; verified native POI keys continue through the asynchronous game table. Personal names are preserved. Language changes require no map reload. Unknown Mod locales fall back to English; native text remains icon-only while unavailable. Traditional Chinese regional aliases include Taiwan, Hong Kong and Macau. No new game, Unity or networking API is required; SDK 2.1.6/r7 and version 0.2.0 remain unchanged. In-game font and overflow acceptance remains pending.

已按游戏 2.1.6 的语言资源核对全部 13 种语言。界面和内置地图标题均提供完整译文；功能地点使用原创标签，已确认的地点名继续异步查询游戏翻译表，原生表未就绪时保留图标。切换语言无需重载地图；未知语言的 Mod 文字回退英语，繁中地区别名包括台湾、香港和澳门。无新增游戏、Unity 或网络 API，继续使用 SDK 2.1.6/r7 和 0.2.0，字体与长文本仍待实机验收。
