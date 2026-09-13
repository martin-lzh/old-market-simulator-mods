# 贡献指南 / Contributing

[中文](#中文) | [English](#english)

## 中文

欢迎提交问题复现、机制核对、翻译、文档修正、测试和原创 Mod 改进。Issues and pull requests in Chinese or English are welcome. 请先阅读 [项目说明](README.md)、目标 Mod 的 README / CHANGELOG 和 [开发与发行标准](releases/README.md)。

### 从哪里开始

- 文档、翻译、纯逻辑测试通常不需要游戏文件；可从下方 Checkout All 测试命令开始。
- 修复问题时，先说明触发条件、预期结果和实际结果，尽量提供可重复的步骤。
- 较大的功能、存档格式或联机行为变更，先通过 Issue 讨论方案与兼容策略，再提交实现。
- 机制分析应同时核对代码路径和实际序列化资源，说明地图、季节、取整与假设；不要把字段默认值当成实际配置。
- 游戏旧版本兼容性只能根据对应版本证据填写，不能根据 Mod 版本号推断。

公开仓库与 [Issues](https://github.com/martin-lzh/old-market-simulator-mods/issues) 已可用；下载见 [Releases](https://github.com/martin-lzh/old-market-simulator-mods/releases)。私密报告方式见 SECURITY.md。

### 项目边界与许可

只提交你有权贡献的原创内容，并保留来源和所需第三方声明。本项目原创代码、测试、脚本、译文和文档采用根目录 MIT 许可证；各 Mod 也附有许可证，贡献应可按该许可分发；具体范围见 [MIT 许可证](LICENSE)。不要提交无法确认授权的代码或素材。

游戏安装目录作为只读构建依赖。不得提交原始游戏 DLL/EXE、资源包、翻译表、反编译快照、存档、凭据或未经脱敏的日志，也不得用反编译源码替换游戏组件。本项目接受按功能范围开发的原创运行时插件；安装和实机测试与构建分开，不在运行中替换 DLL。构建脚本不得自动修改贡献者的游戏安装。

### 获取源码与分支

Fork 仓库并克隆自己的 Fork，从上游 `main` 创建普通贡献分支。贡献者向上游 `main` 提交 PR，不直接推送到上游；不创建临时工作树。

克隆并进入目录、确认本地已有同步的 main 分支后：

```powershell
git switch main
git switch -c codex/checkout-fix
```

分支名按实际修改命名。不要在未检查工作区的情况下切换或丢弃已有修改。

### 环境准备

完整构建使用 Windows、PowerShell、Git、支持 .NET 8 的 SDK，并安装 .NET 8 运行时以执行现有 net8.0 测试。个别项目可仅用 .NET 8，详见各 Mod README。Python 工具使用 uv；本仓库没有需要安装的前端依赖。

```powershell
dotnet --list-sdks
dotnet --list-runtimes
uv --version
```

无游戏的完整编译可使用 [版本化 SDK](sdk/README.md)：运行 `python tools/ci.py build`，需要 Python 3.12 及 .NET 8/10 SDK。实机契约验证及 SDK 更新仍需自行准备合法取得的游戏安装和对应加载器引用。当前支持基线、程序集哈希和各加载器版本见总 README。仓库不提供游戏程序集；克隆后看不到根目录反编译快照或本地证据文件是正常情况。缺少依赖时不要从其他 Issue 下载不明 DLL，也不要绕过哈希或接口检查。

多数 Mod 默认从 `GameDir/BepInEx/core` 读取已有加载器引用；成本 Mod 的构建脚本可下载固定版本的官方加载器依赖到项目 `work/`，并校验哈希。构建不启动或安装游戏。

### 构建与验证

所有命令从仓库根目录运行。以下纯逻辑测试不引用游戏程序集：

```powershell
dotnet run --project checkout-all-mod/tests/Tests.csproj -c Release
```

完整构建示例（将路径换成自己的游戏安装位置；该位置需有 README 所列的只读依赖）：

```powershell
./checkout-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

其他 Mod 使用各自 `build.ps1`；成本插件的加载器变体按其 README 构建。输出在 `outputs/`，临时依赖在 `work/`，两者都不提交。

运行本次修改对应的构建/测试并检查 ZIP 文件清单。发行前按 releases/README.md 核对所有实际发布的包；Standalone 仅供本地验证，不是公开附件。文档修改只需检查链接、命令和差异，不要求新增测试。

涉及数量、价格、保存或网络时，应补充覆盖真实故障和边界的测试；写清使用的游戏版本、加载器、Mod 组合及单机/房主/客人身份。编译、纯逻辑测试、接口检查、实机 UI 和双端联机验证分别报告。没有执行的项目填写“未执行”，不要勾选为通过。

### 代码、语言与兼容记录

沿用目标目录的 C# 风格、命名空间和现有结构，避免无关重构。每个原创 Mod 放独立的 `*-mod/` 目录，本地化和测试随该 Mod 保存；不要引入对相邻 analysis 仓库的依赖。发行记录放 `releases/`。不使用 `git add -f` 上传被忽略的依赖或证据。

用户可见文本集中在 Texts/Localization 或语言资源中，跟随游戏语言，未知语言回退英语。优先复用原生词条；翻译贡献说明语言代码、原文、修改理由和是否实机检查，保留格式占位符。字体和长文本未测时如实记录。

功能变化同时更新 README 的操作、风险及卸载说明，并在独立 CHANGELOG 的“未发布”记录变化与兼容影响。不要自行声称发布日期、旧游戏支持或升级安全。只有维护者明确要求推进版本时才升级版本号；否则全部保留在 Unreleased / 未发布。开发、修复、安装及创建或合并 PR 均不自动授权版本升级。PR 检查须检查所有 Mod 的 CHANGELOG 并对照差异，明确列出尚未指定版本的 Mod，不自行补定版本。涉及额外库存记录的改动必须提供旧档加载、正常保存重载、多人版本要求和回退方案。

版本与发布 PR 由维护者管理。终端用户反馈问题或请求功能不需要创建 PR；希望贡献代码的贡献者仍可按下述流程提交 PR。

### 标签分类

按实际范围选择 `mod:` 和 `area:` 标签，保留 bug、enhancement、documentation 等类型。`status:` 表示当前待办条件，解决后移除；实机测试按具体 Mod 与场景记录，不把自动检查当作实机测试。标签列表见[仓库标签页](https://github.com/martin-lzh/old-market-simulator-mods/labels)。

### 提交 Pull Request

一次 PR 解决一个明确问题，使用 `fix:`、`feat:`、`docs:` 等提交前缀。按 PR 模板说明问题、行为变化、验证命令和结果，以及旧档/联机影响；UI 修改可附不含隐私的截图，禁止附完整游戏资源或存档。

提交前检查：

```powershell
git status --short
git diff --check
git diff
```

只暂存本次相关文件，再检查 `git diff --cached`；尤其确认没有游戏资源、构建产物、日志、备份或敏感信息。推送到自己的 Fork 并提交 PR，等待审查。维护者可能要求补充复现、测试、兼容说明或拆分无关改动；未完成实机验证应在 PR 中保留记录。

安全漏洞见 [SECURITY.md](SECURITY.md)，一般问题见 [SUPPORT.md](SUPPORT.md)。参与讨论请遵守 [行为准则](CODE_OF_CONDUCT.md)。

## English

Contributions can include reproducible bug reports, mechanics research, translations, documentation, tests, and original Mod improvements. Issues and pull requests in Chinese or English are welcome. Start with the [project README](README.md), the relevant Mod's README and CHANGELOG, and the [release standard](releases/README.md).

### Getting started

- Documentation, translations, and pure logic tests usually do not require game files. The Checkout All test below is a starting point.
- For fixes, describe the trigger, expected and actual behavior, and reproducible steps.
- Discuss substantial features, save format changes, or multiplayer changes in an Issue before implementing them.
- Verify mechanics against both code paths and actual serialized resources. State the map, season, rounding, and assumptions; field defaults are not evidence of actual configuration.
- Record compatibility with older game versions only when supported by evidence for those versions, not by the Mod version number.

The [repository](https://github.com/martin-lzh/old-market-simulator-mods), [Issues](https://github.com/martin-lzh/old-market-simulator-mods/issues), and [Releases](https://github.com/martin-lzh/old-market-simulator-mods/releases) are available. See SECURITY.md for private reporting.

### Scope and licensing

Submit only original material you have the right to contribute, and retain required attribution and third-party notices. Original code, tests, scripts, translations, and documentation use the root MIT license; each Mod also includes a license. Contributions must be distributable under that license. See the [license scope](LICENSE). Do not submit code or assets with unverified permissions.

Treat the game installation as a read-only build dependency. Do not commit original game DLLs/EXEs, asset bundles, translation tables, decompiled snapshots, saves, credentials, or unredacted logs. Do not replace game components with compiled decompiled code. This project accepts original runtime plugins within each feature's scope. Keep installation and in-game testing separate from builds, and never replace a DLL while the game is running. Build scripts must not automatically modify contributors' game installations.

### Source and branches

Fork the repository and clone your fork. Create an ordinary contribution branch from upstream `main` and submit a PR targeting upstream `main`, rather than pushing directly upstream. Do not create temporary worktrees.

After cloning, entering the directory, and ensuring your local main branch is synchronized:

```powershell
git switch main
git switch -c codex/checkout-fix
```

Name the branch for your change. Inspect the working tree before switching branches or discarding changes.

### Environment

Full builds use Windows, PowerShell, Git, an SDK supporting .NET 8, and the .NET 8 runtime for existing net8.0 tests. Some projects only need .NET 8; see their READMEs. Python tools use uv. There are no frontend dependencies to install.

```powershell
dotnet --list-sdks
dotnet --list-runtimes
uv --version
```

For game-free compilation, use the [versioned SDK](sdk/README.md) and `python tools/ci.py build` with Python 3.12 and .NET 8/10 SDKs. Real game contract validation and SDK updates still require a legally obtained game installation and appropriate loader references. The main README lists the game baseline, assembly hash, and loader versions. Game assemblies are not included: missing root-level decompiled snapshots and local evidence after cloning are expected. Do not obtain unknown DLLs from Issues or bypass hash/interface checks to resolve missing dependencies.

Most Mods read existing loader references from `GameDir/BepInEx/core`. Material Cost's build script can download pinned official loader dependencies into `work/` and verify their hashes. Builds do not launch or install the game.

### Building and validation

Run commands from the repository root. This pure logic test does not reference game assemblies:

```powershell
dotnet run --project checkout-all-mod/tests/Tests.csproj -c Release
```

For a full build, replace the example path with your installation, including the read-only dependencies listed in the Mod README:

```powershell
./checkout-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

Other Mods use their own `build.ps1`. Follow Material Cost's README for its loader variants. Outputs go into `outputs/`, and temporary dependencies into `work/`; neither belongs in commits.

Run the relevant build/tests and inspect the ZIP file list. Before release, follow releases/README.md for the packages actually being published; Standalone is for local validation, not a public asset. Documentation changes need link, command, and diff checks, not new tests.

For quantity, pricing, save, or network changes, add tests covering real failures and boundaries. Report the game version, loader, Mod combination, and single-player/host/client role. Distinguish compilation, pure logic tests, interface checks, in-game UI tests, and two-machine multiplayer verification. Mark anything not run as “not run,” not passed.

### Code, localization, and compatibility

Follow the target directory's C# style, namespace, and structure; avoid unrelated refactoring. Each original Mod belongs in its own `*-mod/` directory with its own localization and tests. Do not add dependencies on the neighboring analysis repository. Release records belong in `releases/`. Do not use `git add -f` to upload ignored dependencies or evidence.

Centralize user-facing text in Texts/Localization or language resources, follow the game locale, and fall back to English for unknown languages. Reuse native translation keys where possible. Translation contributions should identify the locale, original text, reason for the change, and in-game verification status. Preserve format placeholders and report untested fonts or long text.

For behavior changes, update README usage, risks, and uninstall instructions, plus the independent CHANGELOG's “Unreleased” section with compatibility effects. Do not invent release dates, older-game support, or upgrade safety claims. Advance a version only on an explicit maintainer request; otherwise keep all changes in Unreleased. Development, fixes, installation, and creating or merging PRs do not authorize a bump. PR review must inspect every Mod CHANGELOG against the diff and explicitly list Mods awaiting version assignment, without assigning versions automatically. Changes involving extra inventory records need an old-save loading plan, normal save/reload verification, multiplayer version requirements, and rollback instructions.

Maintainers manage version and release PRs. End users can report issues or request features without opening a PR; code contributors may still submit PRs through the contribution workflow.

### Pull requests

Keep each PR focused on one problem and use commit prefixes such as `fix:`, `feat:`, or `docs:`. Follow the PR template: explain the problem, behavior changes, validation commands/results, and save/multiplayer effects. UI screenshots must exclude private information; do not attach full game assets or saves.

Before committing:

```powershell
git status --short
git diff --check
git diff
```

Stage only related files, then inspect `git diff --cached`. Check especially for game assets, build outputs, logs, backups, and sensitive data. Push to your fork and open a PR for review. Maintainers may request reproduction details, tests, compatibility documentation, or separation of unrelated changes. Keep uncompleted in-game checks documented in the PR.

See [security reporting](SECURITY.md#english) for vulnerabilities and [support](SUPPORT.md#english) for general questions. Follow the [code of conduct](CODE_OF_CONDUCT.md#english).

## CI and SDK contributions / CI 与 SDK 贡献

CI runs on pushes to `dev`/`main`, PRs targeting `main`, and manual dispatch. The syntax job parses tracked Python, PowerShell, JSON and XML/MSBuild files without executing scripts; ignored dependencies and build outputs are excluded. GitHub Actions YAML, expressions and job dependencies are checked with [actionlint](https://github.com/rhysd/actionlint) 1.7.12 (SHA256-verified download). C# compilation remains covered by the SDK build job. Both jobs must pass before publication. No formatter is applied.

CI 在推送到 `dev`/`main`、面向 `main` 的 PR 及手动触发时运行。语法任务只解析 Git 已跟踪的 Python、PowerShell、JSON 和 XML/MSBuild 文件，不执行脚本；忽略目录中的依赖和构建产物不参与检查。GitHub Actions YAML、表达式和任务依赖使用 actionlint 1.7.12 检查，下载包校验 SHA256。C# 继续由 SDK 构建任务编译检查；发布前两项任务都必须通过，不自动格式化源码。

Local syntax checks require Python 3.12, Git and PowerShell 7 (`pwsh` on PATH). Stage new files first so they are included. Run actionlint 1.7.12 separately for workflows:

本地语法检查需要 Python 3.12、Git 和 PowerShell 7（`pwsh` 在 PATH 中）。新增文件先暂存才会纳入检查；工作流另行使用 actionlint 1.7.12：

```powershell
python tools/check_syntax.py
python -m unittest discover -s tools/tests -p test_syntax.py -v
actionlint -shellcheck= -pyflakes=
```

ShellCheck/Pyflakes integrations are disabled for consistent local/CI results; PowerShell and Python syntax use their own parsers above.

关闭 ShellCheck/Pyflakes 集成以保持本地与 CI 一致；PowerShell 和 Python 语法使用上述各自解析器检查。

For game-free builds, run `python tools/ci.py build` with Python 3.12 and .NET 8/10 SDKs. See [SDK maintenance](sdk/README.md) and [automatic releases](releases/README.md). Pin each Mod to a reviewed SDK in its own `release.json`. Preserve old SDK revisions. Use `dev` for maintainer work and PRs into protected `main`.

无游戏编译运行 `python tools/ci.py build`，需要 Python 3.12 和 .NET 8/10 SDK。见 [SDK 维护](sdk/README.md)和[自动发布](releases/README.md)。每个 Mod 在自己的 `release.json` 固定已审查 SDK，保留旧修订。维护工作使用 `dev`，通过 PR 合并受保护的 `main`。

### Label classification

Choose `mod:` and `area:` labels for the actual scope, alongside bug, enhancement or documentation. Use `status:` for outstanding conditions and remove it when resolved. Record in-game tests by Mod and scenario separately from automated checks. See the [repository labels](https://github.com/martin-lzh/old-market-simulator-mods/labels).
