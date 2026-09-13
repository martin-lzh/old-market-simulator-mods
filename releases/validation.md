# Validation record / 验证记录

## English

### Current status — 2026-09-14

The maintainer confirmed that visual testing and all other local in-game testing for the current PR's Mods are complete. **Only multiplayer testing remains.** This supersedes the narrower initial confirmation below. It records maintainer-reported manual testing; no new automated checks or measurements are implied. The supported game baseline remains 2.1.6, and prerelease status is retained while multiplayer testing is pending.

| Mod | Version | Visual and local in-game tests | Multiplayer |
| --- | --- | --- | --- |
| Navigation | 0.2.0 | Complete | Pending |
| Coordinates | 0.1.4 | Complete | Pending |
| Checkout All | 0.1.5 | Complete | Pending |
| Material Cost | 0.5.2 | Complete | Pending |
| Stack All | 0.2.2 | Complete | Pending |
| Price Probability | 0.1.2 | Complete | Pending |

The version column identifies the PR's target versions; this confirmation does not create new installation or binary-hash records. Multiplayer follow-up should cover host/client installation combinations, synchronized actions, reconnects and concurrent interactions. Existing same-version requirements and save rollback instructions remain in effect.

### Earlier evidence

The following initial report is retained for provenance; its open non-multiplayer items are superseded by the current confirmation.

### Local in-game confirmation — 2026-09-14

The maintainer confirmed actual in-game testing of the Navigation work discussed in PR #1. The latest installed build was source `f571d75`, version 0.1.4, on Old Market Simulator 2.1.6 / Windows x64 / Unity Mono with BepInEx 5.4.23.4. Its functionality is carried into 0.2.0; the version-number change itself was built but has not been recorded as installed. This is a user-reported local test, not an automated Unity run.

The confirmation does not provide a per-scenario matrix for every locale, resolution, unlock transition, remote host/client combination, save migration or measured frame time. Keep these limits explicit and retain prerelease status. Earlier dated SDK/CHANGELOG evidence describes checks at that time and is not retroactively rewritten.

| Mod | Current source | Recorded validation scope |
| --- | --- | --- |
| Navigation | 0.2.0 | Local in-game testing confirmed; 1225 navigation checks, 19 native-name checks, 635 metadata/IL contracts and real/SDK builds passed during version preparation. |
| Checkout All | 0.1.5 | Existing localization, state and SDK/real-reference checks; this confirmation does not establish separate checkout UI or multiplayer acceptance. |
| Coordinates | 0.1.4 | Existing build/reference checks; no new Mod-specific test report in this confirmation. |
| Material Cost | 0.5.2 | Existing build/reference and feature/localization checks; no new Mod-specific test report. |
| Stack All | 0.2.2 | Existing inventory/save-array/input contracts and build checks; no new normal save/load or host/client report. |
| Price Probability | 0.1.2 | Existing pricing/localization/authority contracts and build checks; no new native-save or host/client report. |

The CI tooling suite passed 48 tests during Navigation 0.2.0 preparation. See each Mod README and CHANGELOG for commands, historical results and rollback limits. A repository-wide test label must not imply that every Mod has the same remaining coverage.

### Documentation review

Reviewed all six Mod READMEs, CHANGELOG/version metadata and build/package declarations, plus repository setup, support, security, conduct, templates, SDK and release documentation. Corrected Navigation's obsolete test status and source-version wording, current SDK indexing, root build/CHANGELOG links and package contents, and the Navigation release-tag catalog. Existing published download links still resolve to the five previously published prereleases; 0.2.0 is not presented as an already published Navigation download. Other Mod safety and rollback notes remain applicable.

## 中文

### 当前状态 — 2026-09-14

维护者确认当前 PR 各 Mod 的视觉测试及其他本地实机测试均已完成，**目前只剩多人游戏测试**。此确认更新了下方最初较窄的记录；它属于维护者反馈的人工测试，不表示本次新增了自动检查或性能测量。支持的游戏基线仍为 2.1.6，多人测试完成前保留预发布状态。

| Mod | 版本 | 视觉及本地实机测试 | 多人游戏 |
| --- | --- | --- | --- |
| Navigation | 0.2.0 | 已完成 | 待完成 |
| Coordinates | 0.1.4 | 已完成 | 待完成 |
| Checkout All | 0.1.5 | 已完成 | 待完成 |
| Material Cost | 0.5.2 | 已完成 | 待完成 |
| Stack All | 0.2.2 | 已完成 | 待完成 |
| Price Probability | 0.1.2 | 已完成 | 待完成 |

版本栏表示本 PR 的目标版本，本次确认不新增安装或二进制哈希记录。后续多人测试重点为房主/客人安装组合、动作同步、断线重连与同时交互；既有同版本要求及存档回退步骤继续适用。

### 早期证据

下方保留最初报告以便追溯，其中未完成的非多人项目已由上述最新确认更新。

### 本地实机确认 — 2026-09-14

维护者确认已对 PR #1 中讨论的 Navigation 改动进行实际游戏测试。最近安装的是提交 `f571d75`、版本 0.1.4，环境为 Old Market Simulator 2.1.6 / Windows x64 / Unity Mono、BepInEx 5.4.23.4。该功能已纳入 0.2.0；版本号调整已构建，但尚无安装 0.2.0 编号包的记录。此处记录维护者反馈，不将其描述为自动执行 Unity 测试。

反馈未提供所有语言、分辨率、解锁状态、房主/客户端组合、存档迁移或量化帧耗时的逐项测试矩阵，保持这些范围的区别及预发布状态。早期 SDK/CHANGELOG 的带日期记录保留当时结果，不倒改历史。

| Mod | 当前源码 | 已记录验证范围 |
| --- | --- | --- |
| Navigation | 0.2.0 | 已确认本地实机测试；版本准备时通过 1225 项导航、19 项原生名称、635 项元数据/IL 契约及真实引用/SDK 构建。 |
| Checkout All | 0.1.5 | 已有本地化、状态及 SDK/真实引用检查；此次反馈不单独证明结账 UI 或联机验收。 |
| Coordinates | 0.1.4 | 已有构建/引用检查；此次无独立的新测试报告。 |
| Material Cost | 0.5.2 | 已有构建/引用及功能/本地化检查；此次无独立的新测试报告。 |
| Stack All | 0.2.2 | 已有库存、保存数组、输入契约及构建检查；此次无正常保存重载或双端联机新记录。 |
| Price Probability | 0.1.2 | 已有定价、本地化、权限契约及构建检查；此次无原生保存或双端联机新记录。 |

Navigation 0.2.0 准备时通过 48 项 CI 工具测试。具体命令、历史结果及回退限制见各 Mod README/CHANGELOG；仓库级测试标签不代表所有 Mod 的待测范围相同。

### 文档核对

核对了六个 Mod 的 README、CHANGELOG/版本元数据、构建和包声明，以及仓库环境、支持、安全、行为准则、模板、SDK 和发布说明。修正 Navigation 过时的验证状态与源码版本表述、当前 SDK 索引、首页构建/CHANGELOG 入口与包内容，以及 Navigation 标签命名表。公开下载仍指向已有五个预发布版本，不将 Navigation 0.2.0 写成已经公开下载。其余 Mod 的风险及回退说明继续适用。
