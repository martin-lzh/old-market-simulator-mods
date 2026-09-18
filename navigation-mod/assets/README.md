# Navigation artwork / 导航美术资源

These three original UI textures were generated with the built-in Image Gen tool on 2026-09-13 from text prompts, without game screenshots or game assets as image inputs. They are embedded in the DLL and covered by this project's MIT license. Labels, coordinates, markers, directions and controls are rendered by code with the native game font, not painted into these textures.

这三张原创 UI 纹理由内置 Image Gen 工具于 2026-09-13 按文字提示生成，没有输入游戏截图或游戏资源。纹理嵌入 DLL，适用本项目 MIT 许可。文字、坐标、标记、方向与控件由程序使用游戏字体绘制，不烘焙进图片。

| File / 文件 | Purpose / 用途 | Format / 格式 |
| --- | --- | --- |
| parchment.png | Map panel paper / 地图面板纸张 | 1254×1254 RGB |
| minimap-ring.png | Minimap rim / 小地图边框 | 1254×1254 RGBA |
| map-frame.png | Map window frame / 大地图边框 | 1536×1024 RGBA |

Reproduction prompt specifications / 重制提示规格:

- `parchment.png`: A seamless square warm tan parchment texture for an ancient Chinese town game UI. Fine paper fibers, subtle mottling, evenly lit. Edge-to-edge material only. No border, folds, symbols, map geometry, text, objects or watermark.
- `minimap-ring.png`: A thin circular antique gold minimap frame with small etched tick marks and restrained Chinese-inspired detailing. Centered, symmetric, flat front view. Actual transparent canvas outside and inside the ring; no opaque center, map, compass letters, labels, icons or watermark.
- `map-frame.png`: A rectangular antique gold map-window frame with understated curled corners, matching a warm parchment and Chinese market game UI. Flat front view; actual transparent center and exterior. No map, paper fill, lettering, icons, shadows across the center or watermark.

Exact pixel reproduction is not guaranteed by a generative model. The checked-in PNGs are the reproducible build inputs; no image-generation request runs at build or game startup. The two frame centers were checked for transparent alpha.

生成模型不保证逐像素重现。仓库中的 PNG 是可复现构建输入；构建及游戏启动均不调用图像生成服务。两张边框已检查中心 alpha 透明。

The [map pack](../maps/README.md) ships generated illustrations for Eastern Town, Island and Rome. Eastern Town uses market obstacle textures, Island is a static surface overview, and Rome uses runtime hatched unlock areas plus conditional POIs. [Phosphor POI icons](phosphor/README.md) have separate upstream sources and MIT notices.

[地图包](../maps/README.md)随发行 ZIP 提供东方小镇、海岛与罗马的生成插画。东方小镇使用市场障碍贴图，海岛为静态地表概览，罗马由运行时绘制解锁斜线区块并筛选条件 POI。[Phosphor POI 图标](phosphor/README.md)单独记录上游来源及 MIT 声明。
