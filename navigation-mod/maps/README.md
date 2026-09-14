# Eastern Town map pack / 东方小镇地图包

These files are Navigation Mod resources for Old Market Simulator 2.1.6. The release ZIP installs them under `BepInEx/plugins/OldMarket.Navigation/maps/` alongside the plugin. [map-pack.json](../map-pack.json) lists the nine runtime files and their SHA256 hashes; this maintenance note is not a runtime map.

- `oriental-town-artwork.png`: illustrated base generated with Image Gen from a calibrated map diagram on 2026-09-13. It is a map illustration, not an extracted game texture.
- `market0-obstacles.png` through `market3-obstacles.png`: programmatically drawn transparent obstacle overlays for four central-market unlock states. Their footprints were measured from the supported game scene; the fully unlocked overlay is transparent.
- `market0.json` through `market3.json`: map identity, world bounds, expansion matching, texture references and POI coordinates. POI display names are resolved through native localization by the plugin.

The base artwork and overlays use normalized UVs and the same world bounds; their pixel resolutions may differ. PNG files retain the exact bytes used in the local bundle tested on two computers; JSON line endings are normalized to LF without changing map data. Moving arbitrary buildings or objects does not regenerate it. Other towns are not included.

The Mod's original illustration, overlay styling and metadata contributions are provided under [LICENSE](../LICENSE). Game names and underlying game content remain their owners' property. Original game binaries, textures, meshes, extraction snapshots and personal markers are not included.

To update a map, replace only the intended runtime files, update their hashes in `map-pack.json`, and run version validation, CI tooling tests, SDK builds and the map-reader checks. Both `build.ps1` and CI package the listed files from this repository; neither reads maps from an installed game or the analysis repository. Map changes are release inputs and require the usual Unreleased/version/approval review.

这些文件属于 Navigation Mod，适用于游戏 2.1.6。发行 ZIP 将其放在 `BepInEx/plugins/OldMarket.Navigation/maps/`，与插件一同安装。[map-pack.json](../map-pack.json) 明确列出九个运行时文件及 SHA256；本维护说明不作为运行时地图打包。

底图在 2026-09-13 基于校准地图示意图由 Image Gen 生成，并非直接提取的游戏贴图。四张透明障碍层按已核对场景的占地范围程序绘制，对应中央市场四种解锁状态；完全解锁层为透明。四份 JSON 保存地图标识、世界边界、解锁条件、贴图引用和 POI 坐标，显示名称由插件查询游戏本地化。

底图和覆盖层通过相同的归一化 UV 与世界边界对齐，像素分辨率可以不同。PNG 与已完成双实机测试的本地合集保持逐字节一致，JSON 仅统一为 LF 换行、地图数据不变；移动任意建筑或物体不会自动重绘地图，目前不包含其他城镇。

地图中的原创插画、覆盖层样式及元数据贡献随 Mod 采用 [LICENSE](../LICENSE)；游戏名称和底层游戏内容仍属于各自权利人。不包含原始游戏程序集、贴图、网格、提取快照或个人标记。

修改地图时同步更新清单哈希，并运行版本验证、CI 工具测试、SDK 构建和地图读取器检查。本地 `build.ps1` 与 CI 均从本仓库读取清单文件，不依赖游戏安装里的地图或 analysis 仓库。地图属于发行输入，变动遵循未发布记录、版本指定和源码授权规则。
