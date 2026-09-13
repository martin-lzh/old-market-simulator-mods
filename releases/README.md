# Release management / 版本发布

## Automatic CI releases / CI 自动发布

New numbered versions additionally require a [source-bound authorization record](approvals/README.md). Without a record they are skipped; a stale record or pending Unreleased content blocks publication. A version number alone is not release authorization.

新编号版本还必须有[绑定源码的授权记录](approvals/README.md)。没有记录则跳过；记录失效或仍有未发布内容时阻止发布。单独存在版本号不代表获准发布。

See [SDK maintenance](../sdk/README.md) for game-free builds and game-version updates. The workflow below replaces the manual upload steps in the historical procedure later in this document.

无游戏构建和游戏版本更新见 [SDK 维护](../sdk/README.md)。下述工作流接管本文后方历史流程中的手动上传步骤。

### English

1. Work on `dev` and open a PR into protected `main`. Only after the maintainer explicitly requests version advancement, update the affected Mod’s assembly/plugin version and move its pending changes into matching numbered entries in both CHANGELOG languages. Otherwise retain the existing version and record changes under Unreleased. During PR review, inspect every Mod CHANGELOG against the diff and list all Mods still awaiting version assignment; do not assign versions automatically. Record game/SDK versions, loader, risks, save/multiplayer constraints and actual validation. Unreleased entries are ignored.
2. Each Mod pins an SDK and prerelease status in its own `release.json`. Keep `prerelease: true` until in-game acceptance is complete. Existing SDK revisions remain immutable.
3. PR CI compiles all six Mods (seven loader variants), runs pure logic/localization tests and verifies package allowlists on a GitHub-hosted Windows runner. It uses compiler-only API declarations; original game files and self-hosted runners are unnecessary.
4. A push to `main` after merge automatically publishes new CHANGELOG versions at that exact commit. Already-public versions are skipped without replacing assets or moving tags. Documentation/SDK changes without a new Mod version never republish old binaries.
5. The publishing job creates a draft, uploads ZIPs, SHA256SUMS.txt and build-info.json, downloads and verifies every asset byte, then publishes. Evidence records source commit, game version/hash, SDK revision and API snapshot hash. Release notes contain the current numbered English and Chinese entries. No workflow writes back to protected main.

Release selection also compares the committed `<mod>-mod/` directory with the tag of that Mod's highest already-published semantic version (including published prereleases, excluding drafts and other Mods). An unchanged directory is skipped before reading artifacts or writing to GitHub. The comparison covers all commits since that release, not just the latest push; fully reverted changes do not count. Files within the Mod directory, including its documentation, count as changes. Shared SDK, CI and root documentation alone do not. First releases require tracked files in the Mod directory. Missing baseline tags or Git comparison errors stop publication rather than guessing. Release checkout fetches complete history and tags. This gate supplements explicit maintainer version authorization and the new numbered CHANGELOG requirement; it does not replace either.

Packages allow exactly the original Mod DLL under its installation path, README.md, CHANGELOG.md and LICENSE. Never package loaders, SDK references or game files. Existing three-file releases remain untouched.

For interrupted uploads, rerun the failed jobs of the original main-push workflow. A draft can resume only for the same source and notes; existing asset bytes must match. Conflicts leave the draft for inspection. Conflicting tags, another commit’s draft and backwards new versions fail closed. Manual workflow_dispatch runs build diagnostics only and never publish.

The existing catalog.json and tag markdown files remain the historical manual release record. CI does not rewrite them; future provenance is recorded in each Release’s build-info.json and SHA256SUMS.txt and the tagged CHANGELOG. Index updates can follow in a PR.

### 中文

1. 在 `dev` 开发，通过 PR 合并受保护的 `main`。只有维护者明确要求推进版本后，才更新对应 Mod 的程序集/插件版本，并将待发布改动移入 CHANGELOG 中英文区段对应的新编号条目；否则保持现有版本，改动暂归 Unreleased / 未发布。PR 检查须对照差异检查所有 Mod 的 CHANGELOG，列出尚未指定版本的 Mod，不自行补定版本。注明游戏/SDK 版本、加载器、风险、存档/联机约束和实际验证。“未发布”不参与版本选择。
2. 各 Mod 在自己的 `release.json` 固定 SDK 并选择预发布状态。未完成实机验收时保留 `prerelease: true`；已有 SDK 修订保持不变。
3. PR CI 在 GitHub 托管 Windows Runner 上编译六个 Mod（七个加载器变体），运行纯逻辑/本地化测试并检查包清单。引用来自仅含接口声明的 SDK，无需原始游戏文件或自托管 Runner。
4. PR 合并后 main 的 push 自动按 CHANGELOG 新版本发布，标签指向此次准确提交。已公开版本直接跳过，不覆盖附件、不移动标签；仅修改文档/SDK 而不增加 Mod 编号，不会重新发布旧包。
5. 发布任务创建草稿，上传 ZIP、SHA256SUMS.txt、build-info.json，下载逐字节校验所有附件后公开。记录源码提交、游戏版本/哈希、SDK 修订和接口快照哈希；说明提取当前编号的中英文条目。工作流不回写受保护的 main。

发布筛选还会将已提交的 `<mod>-mod/` 目录与该 Mod 最高已公开语义版本的标签比较（包括已公开预发布，排除草稿及其他 Mod）。目录无净变化时，在读取附件和写入 GitHub 前跳过。比较覆盖上次发布以来的所有提交，不只检查最近一次 push；完全还原的改动不计。目录内文档也属于改动，共享 SDK、CI 和根目录文档单独变化则不计。首次发布须有已跟踪的 Mod 文件。缺少基准标签或 Git 比较失败时停止，不猜测结果；发布任务获取完整历史与标签。此条件与维护者明确授权推进版本、新编号 CHANGELOG 同时适用，不能相互替代。

包内只允许原创 Mod DLL（保留安装目录）、README.md、CHANGELOG.md、LICENSE。加载器、SDK 引用、原始游戏文件不得入包；已有三文件发布保持原样。

上传中断时，重跑原 main push 工作流的失败任务。仅同源码、同说明的草稿能续传，已有附件必须逐字节匹配；冲突时保留草稿供检查，不覆盖。已有冲突标签、其他提交的草稿或新版本倒退均拒绝发布。手动 workflow_dispatch 只构建诊断，不发布。

现有 catalog.json 和标签 markdown 保留为历史人工发布记录，CI 不改写它们；后续追溯记录位于各 Release 的 build-info.json、SHA256SUMS.txt 和对应标签的 CHANGELOG。索引可在后续 PR 补充。

### Validation / 验证

```powershell
python tools/ci.py validate
python -m unittest discover -s tools/tests -v
python tools/ci.py build
python tools/ci.py verify-game --game-dir "F:\SteamLibrary\steamapps\common\Old Market Simulator"
```

The first three commands need no game. The last command checks local dependency hashes, compares real-reference and SDK Mod instructions/resources, and runs existing game IL contracts. Cloud CI does not perform this check or in-game UI/save/multiplayer acceptance. Record unperformed checks explicitly.

前三项不需要游戏；最后一项核对本机依赖哈希、对比真实引用与 SDK 的 Mod 指令/资源并执行现有游戏 IL 契约检查。云 CI 不执行此项及实机 UI/存档/联机验收，未执行的检查须明确记录。

## Historical manual procedure / 历史手动流程

### English

Each mod has its own version and tag. Releases share this repository; they do not share one global version.

| Mod | Tag pattern |
| --- | --- |
| Material Cost | `material-cost-vMAJOR.MINOR.PATCH` |
| Coordinates | `coordinates-vMAJOR.MINOR.PATCH` |
| Checkout All | `checkout-all-vMAJOR.MINOR.PATCH` |
| Stack All | `stack-all-vMAJOR.MINOR.PATCH` |
| Price Probability | `price-probability-vMAJOR.MINOR.PATCH` |

1. After the maintainer explicitly authorizes version advancement, update the affected mod's assembly/plugin version, package name, and English/Chinese README. Until then, keep changes in Unreleased. Localization belongs to each mod. Test the mod whose own code or messages changed, and advance its version only with that explicit authorization; no shared localization project is required.
2. Run its build script and tests against the supported local game installation. Original game assemblies must not be uploaded to Git or CI; the reviewed API-only SDK is maintained separately.
3. Verify the ZIP's explicit file list, version, README, license, and embedded messages. Future public plugin ZIPs must include the original DLL, README, CHANGELOG.md, and LICENSE; existing releases retain their original three-file contents. Create `SHA256SUMS.txt` for the exact bytes to upload.
4. Commit and push the source. Tag the exact commit used for the release; do not tag an unrelated later implementation. Save release notes as `<tag>.md` here and record the source commit and asset hashes in `catalog.json`.
5. Create the GitHub Release as a draft, upload the named ZIPs and checksum file, then verify the uploaded assets before publishing. Use a prerelease while in-game acceptance is incomplete. Material Cost may have separate BepInEx and MelonLoader assets under the same mod version.
6. Link the release from the root and mod READMEs. Use per-mod release links: a repository-wide “latest release” cannot represent the latest version of all five mods.

Never replace published binaries or move a published tag to different source. Publish a new version for fixes. Preserve earlier releases for reproducibility. The current standalone loading bundle is for local validation and is not a public release asset.

`catalog.json` is the repository's record of published source commits and ZIP hashes; the GitHub Release pages are the distribution channel. Binary assets remain in Releases, not Git history. The first releases point to the shared source snapshot, and later tags advance independently.

### 中文

每个 Mod 使用独立版本号和标签；它们共用仓库，不共用一个全局版本号。标签前缀见上表。

1. 维护者明确要求推进版本后，才更新对应 Mod 的程序集/插件版本、包名及英文/中文 README；此前改动保留在 Unreleased / 未发布。本地化在各 Mod 内独立维护，修改哪个 Mod 的源码或译文，就验证该 Mod，版本升级仍须维护者明确指令，无需仓库级本地化工程。
2. 对受支持的本机游戏运行构建及测试，不将原始游戏程序集上传到 Git 或 CI；经审查的纯接口 SDK 独立维护。
3. 检查 ZIP 文件清单、版本、README、许可证和内嵌语言资源。后续公开插件 ZIP 须含原创 DLL、README、CHANGELOG.md 和 LICENSE，既有发布保持原三文件内容；为将上传的准确文件生成 `SHA256SUMS.txt`。
4. 提交并推送源码，标签指向实际发布所用的提交，不指向后来无关的实现。发布说明保存在此目录 `<tag>.md`，源码提交与安装包哈希记入 `catalog.json`。
5. 先创建 GitHub Release 草稿，上传明确指定的 ZIP 和校验文件，验证远程附件后公开。实机验收未完成时标记预发布。成本插件的 BepInEx 与 MelonLoader 包可放在同一 Mod 版本下。
6. 更新仓库首页及对应 Mod 的发布链接。使用各 Mod 的链接，仓库级“最新发布”不能代表五个 Mod 各自的最新版本。

已公开的二进制文件不覆盖，标签不改指向；修复通过新版本发布，历史版本保留以便追溯。当前独立启动包仅用于本机验证，不作为公开发布附件。

`catalog.json` 记录发布源码与 ZIP 哈希，GitHub Release 页面负责下载分发。二进制附件存于 Releases，不加入 Git 历史。首批版本来自同一源码快照，后续各标签独立推进。

## Documentation requirements / 文档要求

Each Mod must keep English/Chinese risk notes and an independent CHANGELOG.md linked from its README and the root index. Record real release dates/downloads, game version/hash, loader, save and multiplayer constraints, rollback and test scope. Do not infer old-game support from an old Mod version. Keep previous release records. Update the root supported baseline when it changes. Stage unreleased documentation without overwriting public binaries or checksum records.

每个 Mod 必须保留中英风险提示和独立 CHANGELOG.md，由本 README 及总索引链接。记录真实日期/下载、游戏版本/哈希、加载器、存档和多人限制、回退及验证范围；不按 Mod 版本推断旧游戏支持，保留历史发布。基线变更时更新总说明；未发布文档更新不得覆盖公开附件或校验记录。
