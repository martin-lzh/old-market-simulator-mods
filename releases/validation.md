# Validation record / 验证记录

## English

### Stack All feedback and empty-container follow-up — 2026-09-18

Local testing of the preceding Stack All changes was reported without major issues. No detailed save/multiplayer scenario matrix was supplied, and no broader Navigation acceptance is inferred. The new G empty-container action is subsequent work and remains Unreleased, target **UNASSIGNED**, with in-game and multiplayer acceptance pending. Source version stays 0.2.2; only Stack All moves to SDK 2.1.6/r10.

The G action supports one empty per tap and paced holding (0.6-second delay, 0.12-second interval). Its native caption/keycap row joins the existing right-hand control list and dims when no empty remains; native Q/F rows are retained. Version validation, 67 CI tooling tests, all eight SDK builds, 89,760 Stack All checks, 13-language checks and 84 read-only Stack All game contracts passed. Repository-wide SDK/real-reference IL/resource comparison passed, including the r10 dependency hashes. No game installation was changed and no Unity rendering test was performed for this follow-up.

### Tree Info migration — 2026-09-18

Tree Info 0.1.1 joins the public source repository from the local analysis project. Its existing version is retained, migration changes are Unreleased, the target release version is **UNASSIGNED**, and no release authorization was created. Game baseline remains 2.1.6; only Tree Info pins the new SDK 2.1.6/r8. The earlier six-Mod bundle confirmation does not cover Tree Info.

Version/map validation, 66 CI tooling tests, all eight SDK loader builds and repository-wide real-reference symbolic IL/resource comparisons passed. Tree Info passed nine harvest-estimate cases, 13 language checks, nine locale alias/fallback checks and 21 read-only game/plugin contracts. SDK r8 verifies 15 dependency hashes; compiler-only references exclude implementation/resources. Script syntax and actionlint 1.7.12 passed. No Unity execution or in-game UI, language switching, save or multiplayer acceptance was performed. Installation is a separate local operation and does not establish runtime acceptance.

### Navigation localization follow-up — 2026-09-15

Complete the 13-language interface with localized titles for all 13 packaged map manifests and original functional labels for rest, market, water, calendar and return POIs. The locale list was checked against game 2.1.6 locale assets; no game translation tables are bundled. Version 0.2.0 / SDK 2.1.6/r7 remain unchanged, with target version **UNASSIGNED** and changes under Unreleased.

Version and map-hash validation, all SDK builds, 64 CI tooling tests, 1776 Navigation checks and 22 POI name checks passed. The Navigation-only standard real-reference verifier passed 15 dependency hashes, symbolic IL/resource comparison and 663 installed-assembly contracts. No new game interfaces or SDK revision were needed. Font coverage, long translated labels and in-game language switching remain unverified; no game installation or save was modified.

### Navigation follow-up — source `db03d32`, 2026-09-15

Subsequent Navigation changes add adaptive large-map zoom, Island POIs, expanded Eastern Town service POIs and Rome's eight region maps with conditional POIs and 61 unlock areas. They remain Unreleased with target version **UNASSIGNED**; the source version remains 0.2.0 and the SDK remains 2.1.6/r7. The earlier manual confirmation applies to `e23f986`, not these later changes. New in-game alignment, zoom interaction, region travel, unlock sequencing and multiplayer checks remain pending. No release approval was refreshed.

For `db03d32`, version validation, all SDK builds, 64 CI tooling tests, 1291 Navigation checks, 22 native-name checks and 659 Navigation installed-assembly contracts passed. Navigation's real-reference and SDK builds matched after verification of 15 dependency hashes. The repository-wide real-reference run stopped at Stack All differences; Navigation was then checked independently. No Unity execution, installation or save changes were performed. These are recorded implementation results, not tests rerun for the documentation update. See [Rome evidence](../navigation-mod/maps/rome-audit.md) and [map coverage limits](../navigation-mod/maps/README.md).

### Tested bundle — two-computer multiplayer completed, 2026-09-14

Two-computer in-game multiplayer testing is complete for all six Mods in the supplied `e23f986` bundle: Material Cost 0.5.2, Coordinates 0.1.4, Checkout All 0.1.5, Stack All 0.2.2, Price Probability 0.1.2 and Navigation 0.2.0, on game 2.1.6. This is maintainer-reported manual testing, following the already completed visual and local in-game tests. It supersedes the multiplayer-pending status below without claiming a detailed scenario matrix or performance measurements.

A subsequent Stack All display fix removes the duplicate left number for whole fish and preserves native-size quantity labels. That follow-up is Unreleased and still needs in-game visual confirmation; the completed multiplayer report does not claim testing of later code. Versions, SDK pins and prerelease flags are unchanged. Stack All's follow-up has no assigned target release version. Same-version multiplayer requirements and save rollback instructions remain applicable.

### Previous status — 2026-09-14

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

The following initial report is retained for provenance; its open non-multiplayer items were superseded by the later confirmation for that bundle.

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

### Stack All 反馈与空盒后续操作 — 2026-09-18

此前 Stack All 改动的本地测试反馈为未发现明显问题，未提供完整存档/联机场景矩阵，也不据此扩大 Navigation 的验收范围。新 G 空盒操作属于后续工作，仍记入未发布，目标 **UNASSIGNED**，实机和联机验收待完成。源码版本保持 0.2.2，仅 Stack All 改用 SDK 2.1.6/r10。

G 支持短按一个及长按连扔（延迟 0.6 秒，间隔 0.12 秒）；提示以原生说明文字和键帽加入右侧按钮列表，没有空盒时整行变暗，保留原生 Q/F 行。版本校验、67 项 CI 工具测试、八种 SDK 构建、89,760 项 Stack All 检查、13 语种检查及 84 项 Stack All 只读游戏契约通过。全仓 SDK/真实引用 IL 和资源对比通过，包括 r10 依赖哈希。本次后续改动未修改游戏安装，未执行 Unity 渲染验收。

### Tree Info 迁移 — 2026-09-18

Tree Info 0.1.1 从本地分析项目纳入公开源码仓库，保留既有版本，迁移改动记入未发布，目标发行版本 **UNASSIGNED**，未新增发行授权。游戏基线仍为 2.1.6，仅 Tree Info 固定新增 SDK 2.1.6/r8。此前六 Mod 合集的实机确认不覆盖 Tree Info。

版本/地图校验、66 项 CI 工具测试、全部八种 SDK 加载器构建及全仓真实引用符号 IL/资源对比通过。Tree Info 通过 9 项成熟时间用例、13 语种检查、9 项地区代码/回退检查及 21 项只读游戏/插件契约。r8 核验 15 个依赖哈希，编译引用排除实现和资源。脚本语法及 actionlint 1.7.12 通过。未执行 Unity 或实机 UI、语言切换、存档及联机验收；安装另行在本机进行，不等同于运行验收。

### Navigation 本地化后续改动 — 2026-09-15

补齐 13 语种界面中的地图标题与功能标签：13 份随包地图均提供本地化标题，休息处、市场、补水处、日历及返回入口使用原创翻译。已按游戏 2.1.6 的语言资源核对语言列表，不打包游戏翻译表。版本保持 0.2.0、SDK 保持 2.1.6/r7，目标版本 **UNASSIGNED**，改动记入未发布。

版本及地图哈希验证、全部 SDK 构建、64 项 CI 工具测试、1776 项导航检查及 22 项 POI 名称检查通过。仅针对 Navigation 运行标准真实引用验证，15 个依赖哈希、符号 IL/资源对比及 663 项原生契约检查通过。无需新增游戏接口或 SDK 修订。字体覆盖、长译文和游戏内语言切换仍待实机验收；未修改游戏安装或存档。

### Navigation 后续改动 — 源码 `db03d32`，2026-09-15

后续 Navigation 改动包含大地图自适应缩放、海岛 POI、东方小镇服务 POI 补全，以及罗马八个区域地图、条件 POI 和 61 个解锁区块。这些改动记入未发布，目标版本为 **UNASSIGNED**；源码版本保持 0.2.0，SDK 保持 2.1.6/r7。此前人工确认仅覆盖 `e23f986`，不涵盖后续改动。新地图实机对齐、缩放操作、区域往返、解锁顺序与多人测试仍待完成；未刷新发行授权。

`db03d32` 已通过版本验证、全部 SDK 构建、64 项 CI 工具测试、1291 项导航检查、22 项原生名称检查及 659 项 Navigation 原生契约检查。核对 15 个依赖哈希后，Navigation 的真实引用与 SDK 构建一致。全仓真实引用检查先在 Stack All 差异处停止，随后独立验证 Navigation。未执行 Unity，未修改游戏安装或存档。这是实现阶段的已记录结果，不是本次文档更新重新运行的检查。详见[罗马证据](../navigation-mod/maps/rome-audit.md)和[地图覆盖边界](../navigation-mod/maps/README.md)。

### 已测试合集 — 双实机联机测试已完成，2026-09-14

维护者确认已完成所提供 `e23f986` 合集内全部六个 Mod 的双实机联机测试：Material Cost 0.5.2、Coordinates 0.1.4、Checkout All 0.1.5、Stack All 0.2.2、Price Probability 0.1.2、Navigation 0.2.0，游戏基线为 2.1.6。此前视觉及本地实机测试也已完成。这是维护者反馈的人工测试，更新下方“多人待完成”的状态，不据此声称提供了完整场景矩阵或性能测量。

随后新增的 Stack All 显示修复去除整条鱼左侧重复数字，并保持数量标签的原生字号。该后续修复记入未发布，仍待实机视觉确认；已完成的联机报告不涵盖后续代码。版本、SDK 固定修订及预发布标记不变，Stack All 后续修复尚未指定目标发行版本。既有联机同版本要求及存档回退说明继续适用。

### 先前状态 — 2026-09-14

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

下方保留最初报告以便追溯，其中未完成的非多人项目已由随后针对该合集的确认更新。

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

## Stack All hold guidance — 2026-09-18

Source `2a7ae08` adds a localized caption above native controls, reusing the game font, material and layout and hiding its keycap. It appears only when the selected stack has multiple units. Stack All alone moves to game 2.1.6 / SDK r11 (Transform.SetAsFirstSibling); Mod version remains 0.2.2 and the target release remains **UNASSIGNED**. In-game layout, font fallback and host/client acceptance remain pending.

源码 `2a7ae08` 在原生按钮上方增加本地化长按说明，复用游戏字体、材质和布局并隐藏键帽，仅当前堆叠剩余多个物品时显示。仅 Stack All 切换游戏 2.1.6 / SDK r11（Transform.SetAsFirstSibling），Mod 保持 0.2.2，目标发行版本仍为 **UNASSIGNED**。实机布局、字体回退及房主/客人验收仍待完成。

Validation: version/immutable-SDK validation, all eight SDK builds and real-reference comparisons, 67 CI tooling tests, 89,760 Stack All logic checks, 13-language localization and 84 read-only game contracts passed. These checks do not execute Unity.

验证：版本及 SDK 不可变性、八种 SDK 构建与真实引用比对、67 项 CI 工具测试、89,760 项 Stack All 逻辑检查、13 种语言及 84 项只读游戏契约全部通过；未执行 Unity 实机显示。

## Local follow-up — 2026-09-18

Stack All build `d4e033d`: local testing of the latest hold guidance/native font change was reported complete. This confirms that local UI follow-up only; it does not establish a new save/reload or host/client scenario matrix.

Stack All 构建 `d4e033d`：最新长按说明及原生字体改动获本地测试完成反馈。本次确认限于该 UI 后续改动，不扩展为新的存档重载或房主/客人场景验收。

Navigation Gate 4: corrected the omitted elevated lake after reviewing the real water mesh and terrain elevation. The regenerated illustrated texture preserves metadata/bounds; in-game shoreline confirmation remains pending. Navigation remains 0.2.0 / SDK 2.1.6/r7, target version **UNASSIGNED**.

Navigation 大门4：复核真实水面网格与地形高度后补回高处湖泊，重新生成插画贴图；元数据和边界不变，实机湖岸确认待完成。保持 0.2.0 / SDK 2.1.6/r7，目标版本 **UNASSIGNED**。

Gate 4 validation: version/immutable-SDK checks, 67 CI tooling tests and all eight SDK builds/package allowlists passed. The new PNG was visually compared with the water-corrected reference; its blue water pixels stay within the reviewed lake region. No runtime code/API changed, so Navigation retains SDK r7. No additional game execution or multiplayer test was performed.

大门4验证：版本及 SDK 不可变性、67 项 CI 工具测试、八种 SDK 构建与打包白名单检查通过。新 PNG 已对照修正参考图检查，蓝色水域仅位于已复核湖区。无运行时代码/API 改动，Navigation 保持 SDK r7；未新增实机或联机测试。
