# Phosphor place icons / Phosphor 地点图标

Official **fill-weight** Phosphor SVGs are preserved unchanged from
`phosphor-icons/core`, commit `2b75f3ad12b420c9504ef05df8d2564a28f8500e`.
These are upstream filled silhouettes, not outline icons with their paths filled
afterward. `source.json` records each SHA256. The full MIT license is in `LICENSE`
and in the Mod's packaged LICENSE.

官方 **fill 实心款式** SVG 来自固定提交，保留原样；不是对线框图标强行填充。
`source.json` 记录 SHA256，完整 MIT 许可证同时包含于本目录及 Mod 安装包。

Rebuild from the repository root / 在仓库根目录重新生成：

```powershell
uv run navigation-mod/tools/generate_poi_icons.py
```

The generator verifies vendored source hashes and works offline after its pinned
resvg-py 0.2.5 and Pillow 11.1.0 dependencies are cached. `--fetch` explicitly
downloads the pinned sources again. It renders the official filled paths,
applies four category colors, and adds a 3px dark outer outline at 32px output
size. All 72 PNGs are embedded in the Mod, without a runtime SVG dependency.
Generation checks transparent backgrounds, four exact colors and 18 distinct
silhouettes. The player's existing original arrow remains unchanged.

生成器校验来源哈希，固定版本依赖缓存后可离线执行；`--fetch` 才重新下载来源。
官方实心路径按四种类别颜色着色，在 32px 输出上增加 3px 深色外描边。
72 张 PNG 嵌入 Mod，运行时无需 SVG 依赖；检查透明背景、四种精确颜色和
18 个不同轮廓。玩家原创箭头保持不变。

| Place / 地点 | Phosphor fill icon |
| --- | --- |
| Farm / 农场 | barn |
| Museum / 博物馆 | bank |
| Workshop / 工坊 | hammer |
| Engineer / 工程师 | wrench |
| Decorations / 装饰品 | armchair |
| Carpenter / 木匠 | axe |
| Animals / 动物 | cow |
| Garden / 园艺 | plant |
| Clothing / 服装 | t-shirt |
| Rest / 休息 | bed |
| Orders / 订购 | boat |
| Licenses / 许可证 | identification-card |
| Employees / 雇员 | users-three |
| Expansions / 扩建 | ruler |
| Recycling / 回收 | recycle |
| Market / 市场 | storefront |
| Generic home / 通用住所 | house |
| Generic place / 通用地点 | map-pin |

These descriptions document semantic mappings; game labels still come from the
native translation table. 上表解释图标语义，游戏内标签仍使用原生翻译表。
