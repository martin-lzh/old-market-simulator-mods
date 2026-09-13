# Release management / 版本发布

## English

Each mod has its own version and tag. Releases share this repository; they do not share one global version.

| Mod | Tag pattern |
| --- | --- |
| Material Cost | `material-cost-vMAJOR.MINOR.PATCH` |
| Coordinates | `coordinates-vMAJOR.MINOR.PATCH` |
| Checkout All | `checkout-all-vMAJOR.MINOR.PATCH` |
| Stack All | `stack-all-vMAJOR.MINOR.PATCH` |
| Price Probability | `price-probability-vMAJOR.MINOR.PATCH` |

1. Update the affected mod's assembly/plugin version, package name, and English/Chinese README. Localization belongs to each mod. Version and test the mod whose own code or messages changed; no shared localization project is required.
2. Run its build script and tests against the supported local game installation. Game reference assemblies must not be uploaded to Git or CI.
3. Verify the ZIP's explicit file list, version, README, license, and embedded messages. Future public plugin ZIPs must include the original DLL, README, CHANGELOG.md, and LICENSE; existing releases retain their original three-file contents. Create `SHA256SUMS.txt` for the exact bytes to upload.
4. Commit and push the source. Tag the exact commit used for the release; do not tag an unrelated later implementation. Save release notes as `<tag>.md` here and record the source commit and asset hashes in `catalog.json`.
5. Create the GitHub Release as a draft, upload the named ZIPs and checksum file, then verify the uploaded assets before publishing. Use a prerelease while in-game acceptance is incomplete. Material Cost may have separate BepInEx and MelonLoader assets under the same mod version.
6. Link the release from the root and mod READMEs. Use per-mod release links: a repository-wide “latest release” cannot represent the latest version of all five mods.

Never replace published binaries or move a published tag to different source. Publish a new version for fixes. Preserve earlier releases for reproducibility. The current standalone loading bundle is for local validation and is not a public release asset.

`catalog.json` is the repository's record of published source commits and ZIP hashes; the GitHub Release pages are the distribution channel. Binary assets remain in Releases, not Git history. The first releases point to the shared source snapshot, and later tags advance independently.

## 中文

每个 Mod 使用独立版本号和标签；它们共用仓库，不共用一个全局版本号。标签前缀见上表。

1. 更新对应 Mod 的程序集/插件版本、包名及英文/中文 README。本地化在各 Mod 内独立维护，修改哪个 Mod 的源码或译文，就验证并升级该 Mod，无需仓库级本地化工程。
2. 对受支持的本机游戏运行构建及测试，不将游戏引用程序集上传到 Git 或 CI。
3. 检查 ZIP 文件清单、版本、README、许可证和内嵌语言资源。后续公开插件 ZIP 须含原创 DLL、README、CHANGELOG.md 和 LICENSE，既有发布保持原三文件内容；为将上传的准确文件生成 `SHA256SUMS.txt`。
4. 提交并推送源码，标签指向实际发布所用的提交，不指向后来无关的实现。发布说明保存在此目录 `<tag>.md`，源码提交与安装包哈希记入 `catalog.json`。
5. 先创建 GitHub Release 草稿，上传明确指定的 ZIP 和校验文件，验证远程附件后公开。实机验收未完成时标记预发布。成本插件的 BepInEx 与 MelonLoader 包可放在同一 Mod 版本下。
6. 更新仓库首页及对应 Mod 的发布链接。使用各 Mod 的链接，仓库级“最新发布”不能代表五个 Mod 各自的最新版本。

已公开的二进制文件不覆盖，标签不改指向；修复通过新版本发布，历史版本保留以便追溯。当前独立启动包仅用于本机验证，不作为公开发布附件。

`catalog.json` 记录发布源码与 ZIP 哈希，GitHub Release 页面负责下载分发。二进制附件存于 Releases，不加入 Git 历史。首批版本来自同一源码快照，后续各标签独立推进。

## Documentation requirements / 文档要求

Each Mod must keep English/Chinese risk notes and an independent CHANGELOG.md linked from its README and the root index. Record real release dates/downloads, game version/hash, loader, save and multiplayer constraints, rollback and test scope. Do not infer old-game support from an old Mod version. Keep previous release records. Update the root supported baseline when it changes. Stage unreleased documentation without overwriting public binaries or checksum records.

每个 Mod 必须保留中英风险提示和独立 CHANGELOG.md，由本 README 及总索引链接。记录真实日期/下载、游戏版本/哈希、加载器、存档和多人限制、回退及验证范围；不按 Mod 版本推断旧游戏支持，保留历史发布。基线变更时更新总说明；未发布文档更新不得覆盖公开附件或校验记录。
