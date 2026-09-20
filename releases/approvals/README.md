# Release authorization / 发布授权记录

## 中文

只有维护者明确推进对应 Mod 版本后，Agent 才能记录该次授权。记录是受保护 PR 中可审查的维护者声明，不是对话授权的自动识别，也不是新增一次审批要求；已经取得的明确指令可直接用于生成记录。不得因为维护者要求开发、构建、合并或完善 CI 就自行生成授权。

1. 按维护者指定范围更新版本，整理双语编号 CHANGELOG，并清空对应 `Unreleased` / `未发布` 内容（允许空区段或 HTML 注释）。如果仍需保留后续未发布功能，不发布当前全部源码，先分离发行范围。
2. 完成 SDK、实际接口及必要实机验证，提交源码。工作区必须干净。
3. 运行以下命令，填写实际 Mod 标识和维护者指令的准确摘要；不要包含隐私信息。命令不推进版本、不上传附件、不赋予授权。

```powershell
python tools/ci.py record-approval --mod <mod-id> --authorization '<explicit maintainer version instruction>'
```

4. 审查并提交生成的 `<mod>-v<version>.json`，在 PR 中列出授权版本、来源提交和验证结果。合并后 main 的发布任务仍须通过已有版本、目录变化和附件校验规则。

记录绑定 Mod 版本、SDK、预发布状态及 Git 中的构建输入摘要：对应 Mod 目录、选定 SDK、`tools/`、工作流、根构建配置和 `.gitattributes`。记录位于输入之外，因此提交记录、merge 或 squash 不会因提交号改变而使它失效。修改其他 Mod 或根说明也不会单独使该记录失效。构建输入发生变化后，重新核对原指令是否仍覆盖变更；删除旧记录并提交，再按已获授权的准确范围重新生成，不自动扩大授权。

缺少记录的新版本会跳过，允许开发 PR 正常合并而不发布。已有记录失效或仍有未发布内容会阻止该版本发布；已公开版本始终跳过，不覆盖。保留历史记录。首个真实 Release 仍需在获授权后验证 main 工作流及其远程附件；模拟测试不等于真实发布已完成。

main 的合并要求 GitHub Actions 的 `SDK build and tests` 和 `Script, data and workflow syntax` 检查通过，且分支必须包含最新 main。不得关闭检查或添加绕过者来完成发布。

SDK 检查还会运行只读 `check-releases`，结合实际已公开 Release 检查授权是否过期；它不读取安装包或写入 GitHub。没有授权记录不会阻止开发 PR，失效的候选发布记录会使检查失败。

## English

An Agent may record authorization only after the maintainer explicitly advances the relevant Mod version. The record is a reviewable maintainer attestation in a protected PR, not automatic proof of conversation consent or a new approval round. Reuse an already-granted explicit instruction; development, builds, merging or CI maintenance alone do not authorize a record.

1. Apply the maintainer-assigned scope/version and bilingual numbered CHANGELOG entries. Empty both Unreleased sections; empty sections and HTML comments are allowed. Separate later pending functionality before releasing rather than shipping it with old numbered notes.
2. Complete SDK, actual-interface and appropriate in-game validation, then commit source. Recording requires a clean worktree.
3. Run the command above with the actual Mod ID and an accurate, non-sensitive summary of the explicit maintainer instruction. The command neither bumps a version, uploads assets nor grants authorization.
4. Review and commit the generated `<mod>-v<version>.json`. Record authorized versions, source and validation in the PR. Existing-version, changed-directory and asset verification gates still apply on main.

The record binds Mod version, SDK, prerelease status and a Git digest of build inputs: the Mod directory, selected SDK, tools, workflows, root build configuration and `.gitattributes`. Records are outside those inputs, so committing a record or merging/squashing does not invalidate it solely because the commit ID changes. Other Mods and root documentation alone do not invalidate it. After input changes, verify that the existing maintainer instruction still covers the scope, remove/commit the stale record and regenerate within the granted scope. Never expand authorization automatically.

New versions without records are skipped, so development PRs can merge without publishing. Stale records or pending Unreleased content prevent publication of that version. Public versions are always skipped without replacement; preserve historical records. The first real Release must still verify the authorized main workflow and remote assets; mocked tests are not evidence of a completed real publication.

Merging into main requires successful GitHub Actions checks named `SDK build and tests` and `Script, data and workflow syntax`, with the branch up to date with main. Never disable these checks or add bypass actors to complete a release.

The SDK check also runs read-only `check-releases` against actual published Releases to detect stale authorization. It neither reads packages nor writes to GitHub. Missing records do not block development PRs; stale candidate records fail the check.

## Retiring an obsolete candidate / 归档失效候选授权

When later Unreleased work supersedes an unpublished release scope, move its active `<mod>-v<version>.json` into `history/` with the bound source revision in the filename, preserving its bytes. Record why it is obsolete. Only files at the exact active approval path authorize a candidate; archived records remain evidence and cannot authorize any release. This permits ordinary development CI to pass while the Mod is skipped for publication. Do not update the old input hash, remove Unreleased notes or assign a version merely to make CI green. A future release still needs a maintainer-assigned scope/version and a fresh source-bound record under the existing rules.

后续未发布改动超出尚未发行的授权范围时，将活动 `<mod>-v<version>.json` 移入 `history/`，在文件名中注明绑定源码，保留原始字节并记录归档原因。只有精确活动路径中的记录可授权候选发布；历史记录只供追溯，不授予发布权限。此时普通开发 CI 可以通过，该 Mod 因缺少活动授权而跳过发布。不得为让 CI 通过而改写旧输入哈希、移除未发布说明或擅自指定版本。未来发布仍须维护者指定范围/版本，并按既有规则生成新的源码绑定记录。

On 2026-09-15, [Navigation's previous 0.2.0 record](history/navigation-v0.2.0-c3eeff1.json) was archived unchanged. It covered source `c3eeff1`; map packaging starting at `b6f1972` and later zoom, POI, Rome and localization changes are Unreleased and outside that scope. Repeated PR failures occurred in release preflight after successful builds/tests. Version 0.2.0 and SDK r7 remain unchanged; no replacement authorization was created.

2026-09-15，[Navigation 旧 0.2.0 记录](history/navigation-v0.2.0-c3eeff1.json)原样归档。该记录绑定 `c3eeff1`；从 `b6f1972` 开始的地图打包及后续缩放、POI、罗马和本地化改动属于未发布，超出原范围。连续 PR 失败发生在编译/测试成功之后的发布预检。版本 0.2.0 与 SDK r7 保持不变，未生成替代授权。

Navigation's [initial 0.2.1 record](history/navigation-v0.2.1-538024f.json) is preserved unchanged after local feedback exposed incomplete Windows wheel normalization. Correcting the same wheel/button consistency issue remains within the assigned 0.2.1 patch scope. Retire the stale record before binding the corrected, verified source; version 0.2.1 and SDK r7 stay unchanged.

本地反馈发现 Windows 滚轮单位换算仍不完整后，[Navigation 初始 0.2.1 记录](history/navigation-v0.2.1-538024f.json)原样归档。修正相同的滚轮／按钮一致性问题仍属已指定的 0.2.1 补丁范围；先移除失效记录，再绑定修正并验证后的源码，版本 0.2.1 与 SDK r7 不变。

After local in-game acceptance of source `0c5b952`, preserve its [0.2.1 prerelease record](history/navigation-v0.2.1-0c5b952.json) unchanged and bind the stable release metadata under the same assigned patch scope. Only acceptance documentation and prerelease status change; runtime code and SDK remain as tested.

源码 `0c5b952` 通过本地实机验收后，其 [0.2.1 预发布记录](history/navigation-v0.2.1-0c5b952.json)原样归档，按同一已指定补丁范围绑定正式发行元数据。仅更新验收文档和预发布状态，运行时代码与 SDK 保持实测版本。
