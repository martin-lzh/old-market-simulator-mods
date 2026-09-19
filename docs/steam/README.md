# Steam 中文指南

- [Steam 已发布指南](https://steamcommunity.com/sharedfiles/filedetails/?id=3804079040)：2026-09-19 发布，12 个章节、10 张实机图。
- [指南正文](guide.zh-CN.md)：面向玩家的内容，按二级标题拆分为 Steam 章节。
- [合集封面原图](assets/cover.png)与 [Steam 上传版](assets/cover.jpg)：中英双语方形宣传插画；正文使用各 Mod assets 内的实机截图。
- [封面生成记录](assets/cover-prompt.txt)：使用内置 image_gen 生成，不是实机画面。

发布时将 Markdown 转为 Steam 支持的格式，上传正文引用的实机截图，再插入 Steam 返回的图片地址。平台操作说明不进入指南正文。

Steam 单张图片不能超过 2 MB。封面使用同尺寸 JPEG 上传版；罗马地图原始 PNG 保留在 `navigation-mod/assets/rome-map-gameplay.png`，上传时转为同尺寸、质量 94 的 JPEG（约 461 KiB），不覆盖原图。其余九张实机图直接上传原始 PNG。

本次发布时，Steam 将 `OldMarket` 及 Nexus 游戏路径 `oldmarketsimulator` 中的 `dMarket` 字符串误过滤为爱心，连链接目标也被改变；编辑器中保存的原文仍然完整。具体过滤词库规则未得到官方确认。Steam 版本因此使用以下平台适配，仓库正文保留完整路径和 Mod 直达链接：

- Nexus 入口使用 [LZHSimulators 的 Mod 列表](https://www.nexusmods.com/profile/LZHSimulators/mods)，并说明按对应英文名称选择；各 Mod 的 GitHub Release 仍为直达链接。
- 安装说明要求保留 ZIP 原始文件夹及 DLL 名称；完整路径转由 GitHub 的 Mod 说明提供。地图配置目录以 `BepInEx/config/` 下名称以 `.Navigation` 结尾的目录描述。
- 不修改实际插件文件名、不插入隐藏字符、不要求读者关闭过滤设置。每次更新都重新预览正文和链接目标，避免过滤规则变化造成坏链接。

发布后已检查 Steam 章节目录、封面与地图排版，确认 10 张实机图引用、7 个 Nexus 作者列表入口、7 个 Mod Release 链接和 BepInEx Release 链接均存在，正文及链接中没有被替换的爱心。

维护指南时核对每款 Mod 的 README、当前 GitHub Release 和 Nexus 文件版本；版本表描述已发布安装包，不为文档修改分配新 Mod 版本。游戏兼容基线为 Windows x64 / Unity Mono 2.1.6，加载器方案为 BepInEx 5。SDK、源码和安装包均不随本指南改变。
