# Lucide place icons / Lucide 地点图标

Official Lucide SVGs are vendored unchanged from version 0.468.0, commit
`f12b0de177fbc2a6795e99be065887e72b237123`. `source.json` records each SHA256.
Lucide's complete ISC notice is in `LICENSE`; its inherited Feather MIT notice is
in `FEATHER-LICENSE`. Both notices are included in the Mod's packaged LICENSE.

官方 Lucide SVG 保留原样，来自 0.468.0 的固定提交。`source.json` 记录各文件 SHA256；
ISC 与继承的 Feather MIT 许可全文同时收录于 Mod 安装包的 LICENSE。

From the repository root / 在仓库根目录执行：

```powershell
uv run navigation-mod/tools/generate_poi_icons.py
```

Normal generation verifies the vendored hashes and needs no network after Python
dependencies are cached. `--fetch` explicitly refreshes sources from the pinned
commit. The script uses pinned resvg-py 0.2.5 and Pillow 11.1.0, paints the official
line art with the selected category palette, adds a two-pixel dark outer outline
at 32px output size, and writes 56 PNGs to the parent assets directory. It checks
transparent backgrounds, all four exact colors and 14 distinct silhouettes.
These PNGs are embedded in the Mod; there is no runtime SVG renderer dependency.

正常生成校验本地来源哈希，Python 依赖缓存后无需联网。`--fetch` 才从固定提交下载。
生成器使用固定版本 resvg-py 与 Pillow，按类别着色，在 32px 图标上增加 2px 深色外描边，
输出 56 张 PNG，并验证透明背景、四种颜色和 14 个独立轮廓。PNG 嵌入 Mod，运行时不依赖 SVG 渲染器。

| Place / 地点 | Lucide icon |
| --- | --- |
| Farm / 农场 | wheat |
| Museum / 博物馆 | landmark |
| Workshop / 工坊 | hammer |
| Engineer / 工程师 | wrench |
| Decorations / 装饰品 | lamp |
| Carpenter / 木匠 | axe |
| Animals / 动物 | rabbit |
| Garden / 园艺 | sprout |
| Clothing / 服装 | shirt |
| Rest / 休息 | bed |
| Orders / 订购 | ship |
| Market / 市场 | store |
| Generic home / 通用住所 | house |
| Generic place / 通用地点 | map-pin |

These descriptions document semantic mappings; displayed game names still come
from the game's native translation table. The existing player arrow remains
original project code and is not a Lucide asset.

上述名称仅解释图标语义，游戏内地点名称仍使用原生翻译表。玩家箭头仍为项目原创绘制。
