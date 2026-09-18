# Tree Harvest Helper — Changelog / 版本记录

## English

### 0.1.0 — prerelease (pending first publication)

- Rename the plugin display name to **Tree Harvest Helper** and reorganize the bilingual README around core features and controls, with insertion points for gameplay media. Plugin ID, assembly/configuration paths, gameplay and version are unchanged.

- Show planted fruit trees’ production seasons and estimated harvest readiness in the native interaction subtitle. Read actual tree thresholds and growth state; assume daily watering, identify dry trees and insufficient time before season end, and show ripe fruit as harvestable even outside its production season.
- Support all 13 game languages, regional aliases and English fallback; reuse native season translations. Include the MIT license.
- Migrate original source and tests into this public repository; integrate BepInEx builds, package allowlists, game-free tests and read-only game contracts. Pin game 2.1.6 / SDK r8 for reflected tree state and Harmony interaction fields. Do not modify gameplay, saves or RPCs.
- Start public versioning at 0.1.0, including the features from the unpublished local 0.1.1 build. This is release numbering, not a feature rollback. No previous public Tree Info tag or asset is replaced. UI/language and host/client coverage remains as recorded; retain prerelease status.

### Local development history (not public releases)

- Local 0.1.1 added 13-language support, locale aliases, English fallback and the MIT license before repository migration.
- Local 0.1.0 was installed on 2026-09-12 with fruit-tree season and harvest estimates. Those local labels are not public release history.

## 中文

### 0.1.0 — 预发布（首次发布待完成）

- 插件显示名改为 **Tree Harvest Helper**，双语 README 按核心功能和操作方式组织，并保留实机素材插入位置。插件 ID、程序集／配置路径、玩法及版本不变。

- 在原生交互副标题显示已种植果树的生产季节及成熟预测，读取真实阈值和生长状态，以每日浇水为前提；识别未浇水及季末时间不足，非生产季节的成熟果实仍显示可采收。
- 支持游戏全部 13 种语言、地区别名及英语回退，季节名复用原生翻译，附 MIT 许可证。
- 将原创源码和测试迁入公开仓库，接入 BepInEx 构建、包白名单、无游戏测试及只读游戏契约。固定游戏 2.1.6 / SDK r8，覆盖反射树木状态及 Harmony 交互字段；不修改玩法、存档或 RPC。
- 公开版本从 0.1.0 开始，包含未发布本地 0.1.1 的全部功能；仅调整发行编号，不回退功能，也不覆盖任何已公开 Tree Info 标签或附件。UI／语言及房主／客人覆盖仍按验证记录区分，保持预发布。

### 本地开发历史（非公开版本）

- 本地 0.1.1 在迁入仓库前增加 13 种语言、地区别名、英语回退及 MIT 许可证。
- 本地 0.1.0 于 2026-09-12 安装，包含果树季节及成熟预测；这些本地编号不作为公开发行历史。
