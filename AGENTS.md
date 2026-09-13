# Mod 仓库 Agent 规则 / Agent instructions for the Mod repository

## 中文

本目录是公开 Mod 源码、SDK 和发行工具的维护仓库。开始修改 Mod、构建或发布流程前，必须阅读 [贡献指南](CONTRIBUTING.md)、[SDK 维护说明](sdk/README.md)、[发行规则](releases/README.md)，以及目标 Mod 的 README、CHANGELOG 和 `release.json`。公开 Mod 改动应留在本仓库，不写到相邻的 analysis 仓库。

### Mod 版本推进与 PR 检查

1. 只有维护者明确要求“推进版本”或明确指定本次版本升级时，才推进对应 Mod 的版本。开发、修复、构建、安装、提交、推送或创建/合并 PR 本身均不代表版本推进授权。
2. 未获明确指令的改动，全部暂记入对应 Mod CHANGELOG 的 `Unreleased` / `未发布` 中英文区段；保持现有程序集、插件和包版本号，不擅自新增编号版本条目，也不把新改动补写进已有编号的历史记录。本地构建用提交号和 SHA256 区分，不能因安装测试需要自行加版本。
3. 维护者明确要求推进后，按其指定范围统一落实版本号、插件声明、包名、README 与中英文 CHANGELOG，将相应 Unreleased 内容移入编号版本。必要的不可变 SDK 接口快照修订按下文维护，但不自动推进 Mod 版本。
4. 创建、更新或审查 PR 时，必须检查所有 Mod 的 CHANGELOG，并对照 PR 差异，列出有改动但尚未指定版本的 Mod（包括仍有 Unreleased 内容者）。PR 中逐项记录“已明确指定的目标版本”或“Unreleased，待维护者指定版本”；没有则明确写“无”。不得为了让检查通过自行分配版本，也不得仅凭已有旧版本号认定本轮改动已指定版本。

5. 发布还须检查对应 `<mod>-mod/` 目录相对该 Mod 最近已发布版本标签的净差异；目录未变默认跳过。共享 SDK、CI 或根目录文档改动可以触发验证，但不能单独触发未改动 Mod 的 Release。首次发布须有已跟踪的 Mod 文件；目录变化也不能替代维护者的版本推进授权。

### SDK 维护是 Mod 开发的一部分

发布授权必须按 [发布记录流程](releases/approvals/README.md)绑定到确定的源码输入。只有维护者明确推进对应版本后才记录授权；已获授权无需重复询问。没有记录的版本默认不发布；有记录但构建输入变化或仍有 Unreleased 内容时停止发布，不自动更新记录绕过检查。记录通过受保护 PR 审查，不能将它宣称为对话授权的自动证明。

1. 每次新增或修改 Mod，都检查其 `release.json` 所固定的 SDK 是否覆盖实际使用的游戏、Unity 和网络接口。新增类型/成员调用、修改方法签名或泛型用法、增加 `nameof`/反射/Harmony 目标、适配游戏更新时，必须重新评估 SDK 和本地游戏契约检查。纯文档、翻译或不改变接口依赖的逻辑修改无需机械增加 SDK 修订。
2. 新增 Mod 时，将它及需要公开发行的加载器变体接入 `tools/ci.py` 的构建、SDK 导出、验证和发布范围，并提供独立 `release.json`。检查脚本当前枚举哪些 Mod，不得假定新目录会被自动发现；新 Mod 必须实际通过 SDK 编译。
3. SDK 必须跟随已确认的游戏版本维护。`sdk/<游戏版本>/r<修订号>/` 一旦合并就不可修改、删除或覆盖。同一游戏版本扩展接口时新增修订；游戏升级时新增游戏版本目录。保留历史 SDK、Mod 标签和发布附件，以支持旧版用户。
4. 缺少接口时，先用合法取得的真实游戏程序集只读核对并构建，再用 `python tools/ci.py export-sdk` 导出新快照。遵循 SDK 文档的参数与流程；导出要求先提交对应源码且工作区干净。审查 `supplemental.json` 中仅被 `nameof` 使用的方法，以及未体现在编译引用中的反射/Harmony 依赖。不得编造签名、随意增加空接口或绕过校验来消除编译错误。
5. 核对新 SDK 的程序集身份、接口声明、泛型约束、枚举、类型转发，以及 `manifest.json` 的游戏版本、源码提交和哈希。只迁移需要适配的 Mod 的 SDK pin，不将所有 Mod 自动切到“最新”。接口不变也可能发生游戏行为变化，仍须审查 RPC、存档、Harmony 目标和 IL 假设。
6. SDK 迁移或 Mod 发行时，同步维护对应 `release.json`、程序集/加载器版本、双语 CHANGELOG、README 的兼容与风险说明，以及根 README 和 SDK 文档的支持版本记录。按实际发布范围更新；不要把尚未验证的游戏版本声明为已支持。
7. 修改后运行与改动相关的版本验证、CI 工具测试及 SDK 构建。接口或游戏基线变化还必须在匹配的游戏安装上运行真实引用对比与游戏契约检查。不同 Mod 使用不同游戏基线时，按 SDK 文档分别验证。缺少游戏或未完成实机验证时明确报告限制，不跳过哈希失败，不把编译通过等同于存档、UI、联机或性能验收；实机验收未完成保持预发布。
8. SDK 仅包含编译所需声明，不包含游戏实现或资源。原始游戏 DLL/EXE、资源、反编译快照、存档、凭据不得提交或上传。生成的 SDK 引用 DLL 只留在被忽略的构建目录，不能安装到游戏或放进 Mod 发布包；保留发布脚本的显式文件白名单。游戏安装只读，构建不授予安装、修改游戏或存档的权限。

### 分支与交付

PR 标题和正文使用英语，以维护者的口吻面向其他开发者撰写。说明最终问题、行为变化、验证和发布状态；不写 Agent 向用户汇报的语气，不叙述对话、用户指令或执行过程。版本推进由维护者决定，终端用户不需要创建 PR。

在当前 `dev` 分支维护，通过 PR 合并受保护的 `main`；不创建临时工作树，不直接推送 main 或绕过保护。按用户约定提交并推送本次相关改动。先检查工作区，保留其他任务的修改，不将其混入提交。报告是否更新 SDK、所用基线和实际验证结果；文档修改只需检查内容、链接与差异。

## English

This is the public repository for original Mod source, the compilation SDK and release tooling. Before changing a Mod, build or release workflow, read [CONTRIBUTING](CONTRIBUTING.md), [SDK maintenance](sdk/README.md), [release rules](releases/README.md), and the target Mod's README, CHANGELOG and `release.json`. Keep public Mod changes here, not in the adjacent analysis repository.

### Mod version advancement and PR review

1. Advance a Mod version only when the maintainer explicitly requests version advancement or explicitly specifies the version upgrade. Development, fixes, builds, installation, commits, pushes, and creating or merging a PR do not themselves authorize a version bump.
2. Otherwise, record all pending changes in that Mod's bilingual `Unreleased` / `未发布` CHANGELOG sections. Preserve existing assembly, plugin and package versions; do not create numbered entries or append new changes to numbered historical entries. Identify local builds by commit and SHA256 instead of inventing a version for installation tests.
3. After an explicit request, synchronize the authorized version scope across assembly/plugin declarations, package names, README and both CHANGELOG languages, moving the relevant Unreleased entries into the numbered release. Maintain required immutable SDK API revisions under the rules below; this does not automatically advance any Mod version.
4. When creating, updating or reviewing a PR, inspect every Mod CHANGELOG and compare the PR diff. List Mods with changes that have not yet been assigned a version, including those with pending Unreleased entries. Record each as an explicitly assigned target version or “Unreleased, awaiting a maintainer-assigned version”; state “none” if there are none. Never assign versions just to pass this check or treat an existing old version number as an assignment for the new changes.

5. Publishing additionally requires a net change in the corresponding `<mod>-mod/` directory since that Mod's latest published version tag. Skip unchanged directories. Shared SDK, CI and root documentation changes may trigger validation, but cannot alone trigger a Release for an unchanged Mod. A first release requires tracked Mod files; directory changes do not replace explicit version-advancement authorization.

### SDK maintenance is part of Mod development

Bind release authorization to exact source inputs using the [release record workflow](releases/approvals/README.md). Record it only after explicit maintainer version advancement; do not ask again for an already-granted instruction. Versions without records do not publish. Changed build inputs or remaining Unreleased content invalidate readiness; never auto-refresh records to bypass checks. Records are reviewed attestations in protected PRs, not automatic proof of conversation consent.

1. For every new or modified Mod, check whether its pinned SDK covers the game, Unity and networking APIs it actually uses. New types/members, signature or generic changes, new `nameof`/reflection/Harmony targets and game updates require reassessing the SDK and local game contracts. Documentation, translations and logic changes without new interface dependencies do not automatically require a new SDK revision.
2. Register each new Mod and its public loader variants in the build, SDK export, validation and release scope of `tools/ci.py`, and provide its own `release.json`. Inspect the current Mod enumeration; do not assume directory discovery. The new Mod must actually compile against the SDK.
3. Maintain SDKs against verified game versions. Once merged, `sdk/<game-version>/r<revision>/` snapshots must not be edited, deleted or overwritten. Extend an existing game baseline with a new revision; use a new game-version directory after a game update. Preserve historical SDKs, Mod tags and release assets.
4. When an interface is missing, first verify and compile against legally obtained game assemblies read-only, then export a new snapshot with `python tools/ci.py export-sdk`. Follow the SDK documentation; export requires committed source and a clean worktree. Review supplemental methods used only in `nameof`, and reflection/Harmony dependencies absent from compiled member references. Never invent signatures, add arbitrary empty interfaces or bypass checks to silence compilation errors.
5. Review assembly identities, API declarations, generic constraints, enums, type forwarding, and the manifest's game version, source commit and hashes. Migrate only the Mods needing the new SDK; never select latest implicitly. Unchanged signatures do not imply unchanged behavior: review RPC, save, Harmony and IL assumptions too.
6. When migrating an SDK or releasing a Mod, update the relevant `release.json`, assembly/loader versions, bilingual CHANGELOG, README compatibility/risk guidance, and supported-baseline records in the root README and SDK documentation. Match the actual release scope; do not claim support for an unverified game version.
7. Run relevant version validation, CI tooling tests and SDK builds. Interface or game-baseline changes also require real-reference comparisons and game contracts against a matching installation. Follow the SDK guide for mixed game baselines. Report unavailable game files and unperformed in-game checks explicitly. Never bypass hash failures or equate compilation with save, UI, multiplayer or performance acceptance; retain prerelease status until acceptance is complete.
8. SDK snapshots contain compilation declarations, not game implementations or resources. Never commit or upload original game DLLs/EXEs, resources, decompiled snapshots, saves or credentials. Generated SDK DLLs remain in ignored build directories and must never be installed or packaged with Mods. Preserve explicit package allowlists. Game installations are read-only; building does not authorize game installation changes or save modifications.

### Branches and delivery

Write PR titles and bodies in English, in the maintainer's voice for other developers. Explain the final problem, behavior changes, validation and release status. Do not frame the PR as an agent's report to a user or recount conversation history, user instructions or execution steps. Maintainers decide version advancement; end users are not required to open PRs.

Work on the current `dev` branch and merge through a PR into protected `main`. Do not create temporary worktrees, push directly to main or bypass protection. Commit and push task-related changes as requested by the user. Inspect the worktree first and preserve unrelated work without including it in the commit. Report whether the SDK changed, the selected baseline and actual validation results. Documentation-only changes need content, link and diff checks.

## 验证命令 / Validation commands

Run from the repository root; the last command requires a matching local game installation. / 从仓库根目录运行；最后一项需要匹配的本机游戏安装。

```powershell
python tools/ci.py validate
python -m unittest discover -s tools/tests -v
python tools/ci.py build
python tools/ci.py verify-game --game-dir '<game-installation-path>'
```
