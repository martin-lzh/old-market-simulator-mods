# Navigation maps / 导航地图包

These files are Navigation Mod resources for Old Market Simulator 2.1.6. The release ZIP installs them under `BepInEx/plugins/OldMarket.Navigation/maps/` alongside the plugin. [map-pack.json](../map-pack.json) lists the 28 runtime files (13 JSON and 15 PNG) and their SHA256 hashes; this maintenance note is not a runtime map.

| Map / 地图 | JSON | PNG | Runtime files / 运行时文件 |
| --- | --- | --- | --- |
| Eastern Town / 东方小镇 | 4 | 5 | 9 |
| Island / 海岛 | 1 | 1 | 2 |
| Rome / 罗马 | 8 | 9 | 17 |

Eastern Town resources / 东方小镇资源：

- `oriental-town-artwork.png`: illustrated base generated with Image Gen from a calibrated map diagram on 2026-09-13. It is a map illustration, not an extracted game texture.
- `market0-obstacles.png` through `market3-obstacles.png`: programmatically drawn transparent obstacle overlays for four central-market unlock states. Their footprints were measured from the supported game scene; the fully unlocked overlay is transparent.
- `market0.json` through `market3.json`: map identity, world bounds, expansion matching, texture references and POI coordinates. POI display names are resolved through native localization by the plugin.

Eastern Town base artwork and market overlays use normalized UVs and the same world bounds; their pixel resolutions may differ. Rome’s town detail patch uses its own world rectangle. Eastern Town PNG files retain the bytes used in the tested `e23f986` bundle, but later POI metadata changes are not covered by that manual report. Island is a static surface overview; Rome has eight region maps with conditional areas and POIs. Moving arbitrary buildings or objects does not regenerate the artwork.

The Mod's original illustration, overlay styling and metadata contributions are provided under [LICENSE](../LICENSE). Game names and underlying game content remain their owners' property. Original game binaries, textures, meshes, extraction snapshots and personal markers are not included.

To update a map, replace only the intended runtime files, update their hashes in `map-pack.json`, and run version validation, CI tooling tests, SDK builds and the map-reader checks. Both `build.ps1` and CI package the listed files from this repository; neither reads maps from an installed game or the analysis repository. Map changes are release inputs and require the usual Unreleased/version/approval review.

这些文件属于 Navigation Mod，适用于游戏 2.1.6。发行 ZIP 将其放在 `BepInEx/plugins/OldMarket.Navigation/maps/`，与插件一同安装。[map-pack.json](../map-pack.json) 明确列出 28 个运行时文件（13 份 JSON、15 张 PNG）及 SHA256；本维护说明不作为运行时地图打包。

东方小镇底图在 2026-09-13 基于校准地图示意图由 Image Gen 生成，并非直接提取的游戏贴图。四张透明障碍层按已核对场景的占地范围程序绘制，对应中央市场四种解锁状态；完全解锁层为透明。四份 JSON 保存地图标识、世界边界、解锁条件、贴图引用和 POI 坐标，显示名称由插件查询游戏本地化。

东方小镇底图与市场覆盖层通过相同的归一化 UV 和世界边界对齐，像素分辨率可以不同；罗马主城细节图使用独立世界矩形。东方小镇 PNG 与已测试的 `e23f986` 合集保持逐字节一致，但后续 POI 元数据改动不在该人工测试范围内。另含海岛静态地表图及罗马八个区域地图，罗马区块与 POI 随解锁变化；移动任意建筑或物体不会自动重绘插画。

地图中的原创插画、覆盖层样式及元数据贡献随 Mod 采用 [LICENSE](../LICENSE)；游戏名称和底层游戏内容仍属于各自权利人。不包含原始游戏程序集、贴图、网格、提取快照或个人标记。

修改地图时同步更新清单哈希，并运行版本验证、CI 工具测试、SDK 构建和地图读取器检查。本地 `build.ps1` 与 CI 均从本仓库读取清单文件，不依赖游戏安装里的地图或 analysis 仓库。地图属于发行输入，变动遵循未发布记录、版本指定和源码授权规则。

## Island / 海岛

`island.json` selects map ID 0 / BazaarIsland and records 16 fixed POIs; `island-artwork.png` is a new Image Gen illustration made on 2026-09-15 from a full-terrain orthographic diagram, with a second pass correcting town/northern-island placement. Bounds are X/Z [-501,501]. It is independent of Eastern Town unlock conditions and intentionally has no expansion overlay. See [coordinates, categories, icon assignments and validation limits](island-pois.md).

`island.json` 对应第一张海岛地图，记录 16 个固定 POI；`island-artwork.png` 于 2026-09-15 由完整地形投影经 Image Gen 绘制并修正镇区和北岛位置。X/Z 边界均为 [-501,501]，不复用东方小镇的解锁条件，也不包含扩建覆盖层。[坐标、类别、图标及验证范围](island-pois.md)。新图仍待实机验收。

## Eastern Town service coverage / 东方小镇服务覆盖

The four Eastern Town manifests share 17 fixed POIs and cover X [-120,220], Z [40,300]. The expanded full-terrain render remains a local artifact and is not part of the runtime map pack. The [coverage review](eastern-poi-audit.md) adds recycling, employees, water refill and calendar; aquarium and origami remain grouped under the museum POI. Water/calendar now use original functional labels in all 13 game languages; verified native names still use the game table.

东方小镇四份地图共用 17 个固定 POI，覆盖 X [-120,220]、Z [40,300]。扩大后的完整地形渲染仍为本地产物，尚未纳入运行时地图包。[覆盖复核](eastern-poi-audit.md)补充回收、雇员、补水和日历；水族馆与折纸收集合并在博物馆 POI 中。补水处和日历现使用覆盖游戏全部 13 种语言的原创功能标签；已确认的地点名继续使用游戏翻译表。

## Rome / 罗马

Eight manifests select map ID 2 / BazaarRome and the exact local region name. The town has 59 unlock areas and a higher-detail image patch; the mine has two additional areas whose source objects reside in the main scene. These 61 independent unlock IDs control hatched rectangles over measured affected-object bounds, not intermediate building reconstructions or walkability. The town has 46 conditional POI records, with 15 initially visible and 39 fully unlocked; each of the seven travel regions has a return portal. See [region links, POIs, conditions and artwork provenance](rome-audit.md).

八份元数据按地图 ID 2、BazaarRome 和本地玩家区域名称匹配。主城有 59 个解锁区块及细节底图；矿洞有两个额外区块，其源对象属于主城场景。61 个独立解锁 ID 控制按受影响对象边界测得的斜线矩形，不表示中间建筑模型或通行性。主城有 46 条条件 POI 记录，初始显示 15 个，全部解锁后显示 39 个；七个传送区域各有一个返回入口。[区域关联、POI、条件及插画来源](rome-audit.md)。

## Localized names / 本地化名称

All 13 manifests set `NameTextKey` for a title from the Mod's 13-language text catalog, updated whenever the game language changes. `Name` remains a fallback for custom maps without a recognized key. Rest, market, water, calendar and return POIs have original translated functional labels; other verified POI keys resolve through native localization. Metadata and texture bounds are independent of language, and personal marker names are never rewritten.

13 份地图元数据均设置 NameTextKey，标题从 Mod 的 13 语种词条读取并随游戏语言更新；没有可识别键的自定义地图仍使用 Name。休息处、市场、补水处、日历与返回入口使用原创功能标签，其他已确认的 POI 继续查询原生本地化。语言不会改变地图坐标或贴图边界，也不会重写个人标记名称。
