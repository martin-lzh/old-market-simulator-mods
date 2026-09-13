# Mod compilation SDK / Mod 编译 SDK

## 中文

本 SDK 让 GitHub 托管 Runner 在没有安装游戏的情况下编译 Mod。使用标准 .NET 编译器；`api.json` 保存 Mod 用到的类型、字段、方法签名、泛型约束、枚举值及必要的编译元数据。没有原始游戏 DLL、游戏方法实现、资源、存档或反编译源码。

`tools/Sdk` 用 Mono.Cecil 将声明重建为带 `ReferenceAssemblyAttribute` 的编译引用，所有非抽象方法都是工具生成的 `throw null` 占位体。SDK 不是游戏运行库，不能运行游戏，也不能安装到游戏或放进 Mod 下载包。发布脚本只打包原创 Mod DLL、README、CHANGELOG 和 LICENSE。加载器从官方固定版本下载并校验 SHA256。

### 版本与支持范围

| 游戏版本 | SDK | 平台 | 说明 |
| --- | --- | --- | --- |
| 2.1.6 | [2.1.6/r1](2.1.6/r1/manifest.json) | Windows x64，Unity Mono 2022.3.62f3 | 原有五个 Mod 的六种发行构建；实机 UI、存档及双端联机验收未完成 |
| 2.1.6 | [2.1.6/r2](2.1.6/r2/manifest.json) | Windows x64，Unity Mono 2022.3.62f3 | Navigation 0.1.0 固定此修订；增加地图 UI、物理投影、输入、区域与反射依赖，实机验收未完成 |

每个 Mod 在自己的 `release.json` 中固定 SDK，例如 `"sdk": "2.1.6/r1"`。不自动选择“最新”SDK。目录 `sdk/<游戏版本>/r<修订号>/` 一旦合并就保留原样；同一游戏版本补充接口时新建 `r2`，游戏升级时新建对应游戏版本目录。旧 Mod 发布、SDK 和 CHANGELOG 都保留，下载前仍须确认存档、加载器与联机要求。

`manifest.json` 记录游戏版本、源码提交、游戏程序集 SHA256 和接口快照 SHA256；`api.json` 另记录各修订实际涉及的依赖程序集身份与本机文件哈希。SDK 没有重新实现游戏；类型和成员标识用于兼容。原创工具遵循根目录 MIT 许可证，游戏、Unity 和加载器的名称及 API 标识仍属于各自权利人，本项目不声称授予其实现或资源的再分发许可。

### 无游戏文件的构建

需要 Python 3.12、.NET 8 和 .NET 10 SDK；自动化环境使用 Windows。从仓库根目录运行：

```powershell
python tools/ci.py validate
python -m unittest discover -s tools/tests -v
python tools/ci.py build
```

产物在 `outputs/ci/<mod>/`；引用在被忽略的 `work/ci-sdk/`。构建显式指定所有引用目录，不退回本机游戏目录。新增 API 调用若超出当前 SDK，会编译失败；先使用真实游戏引用核对，再增加 SDK 修订，不用随意填空的接口绕过错误。

### 游戏更新后的维护

1. 在本机合法取得新版游戏，只读检查版本和接口。用真实游戏引用调整 Mod；记录未验证的玩法、UI、存档及网络变化。先提交源码，SDK 导出要求工作区干净，以记录准确源码提交。
2. 准备 `supplemental.json`：列出只出现在 `nameof` 中、不会生成成员引用的游戏方法。可在 `work/` 下复制旧文件再调整。反射方法同样列出；反射字段使用显式 `field:字段名`，如 `field:currentSlot` 和 `field:activeExpansions`。所有名称必须来自真实程序集。导出工具自动从已注册六个 Mod 的七个发行构建读取其他依赖声明，不读取游戏方法体或资源。
3. 用实际确认的游戏版本导出新目录。例如同一 2.1.6 基线新增接口时：

```powershell
python tools/ci.py export-sdk --game-dir 'F:\SteamLibrary\steamapps\common\Old Market Simulator' --game-version 2.1.6 --revision 2 --supplemental sdk/2.1.6/r1/supplemental.json
```

4. 审查声明差异和哈希，将需要迁移的 Mod 的 `release.json` 指向新 SDK。更新 Mod 版本、双语 CHANGELOG（包含游戏/SDK 版本）、README 兼容与风险说明、总 README 支持版本和本表。未迁移的 Mod 保留自己的旧 SDK。
5. 重新执行 SDK 构建，再在持有匹配游戏版本的电脑执行：

```powershell
python tools/ci.py verify-game --game-dir 'F:\SteamLibrary\steamapps\common\Old Market Simulator'
```

该命令核对原始依赖哈希、以真实引用编译并对比 Mod 的符号化 IL 和嵌入资源，随后运行现有堆叠/定价游戏方法契约检查。不执行游戏、不安装 DLL、不写存档。当前命令验证全部 Mod；如果它们选择不同游戏基线，需分别保留相应游戏安装并按各 Mod 构建/契约检查命令验证，不能把哈希失败跳过。

6. 游戏更新可能保持接口不变却改变行为。重新审查 Harmony 目标、RPC、存档及 IL 检查；完成与风险匹配的实机测试。CI 编译通过不等于通过这些检查。未完成实机验收时保留 `"prerelease": true`。提交到 `dev`，通过 PR 合并 `main`。

SDK 随维护者确认的游戏版本更新，不会监测到一次游戏更新就自动覆盖已验证的接口快照。

## English

This SDK lets GitHub-hosted runners compile Mods without a game installation. The standard .NET compiler consumes references reconstructed from `api.json`: the types, fields, method signatures, generic constraints, enum values and compiler metadata used by these Mods. No original game DLLs, game method implementations, resources, saves or decompiled source are included.

`tools/Sdk` uses Mono.Cecil to produce assemblies marked `ReferenceAssemblyAttribute`; every concrete method contains a tool-authored `throw null` placeholder. These are compiler references, never runtime libraries. Do not install or distribute them with a Mod. Packages contain only the original Mod DLL, README, CHANGELOG and LICENSE. Official loader downloads are version-pinned and SHA256-verified.

### Versioning and compatibility

The table records game 2.1.6 on Windows x64 / Unity Mono 2022.3.62f3. The original five Mods retain SDK r1; Navigation 0.1.0 pins r2, adding map UI, physics projection, input, region and reflection declarations. CI builds six Mods and seven loader variants. In-game UI, save and multiplayer acceptance remains incomplete.

Each Mod pins an SDK in its own `release.json`, for example `"sdk": "2.1.6/r1"`. There is no implicit latest SDK. Merged `sdk/<game-version>/r<revision>/` snapshots are immutable. Add a new revision to extend the same game baseline, or a new game-version directory after a game update. Preserve old SDKs, Mod releases and CHANGELOG records; users must still check save, loader and multiplayer requirements.

The manifest records game version, source commit, game assembly hash and API snapshot hash. API metadata records the identities and local hashes of each revision’s required dependency assemblies. Original tools use the root MIT license. Game, Unity and loader names/API identifiers belong to their respective owners; this project does not grant redistribution rights to their implementations or resources.

### Building and updating

Install Python 3.12 plus .NET 8 and 10 SDKs. Run the three validation/build commands above from the repository root; CI uses Windows. Packages go to `outputs/ci/<mod>/`; generated references stay in ignored `work/ci-sdk/`. Reference paths are explicit and never fall back to an installed game. An API absent from the selected SDK must be checked against the real game before adding a revision.

After a game update:

1. Read the legally obtained game installation, adapt the original Mod source and commit it. Export requires a clean worktree to record the exact source commit.
2. Review supplemental method names used only in `nameof` expressions. Copy the previous `supplemental.json` into ignored `work/` if it needs editing. Reflection method names are also listed; reflection fields use the explicit `field:name` syntax (for example `field:currentSlot` and `field:activeExpansions`). All declarations must resolve from real assemblies. Other declarations are discovered from the seven registered loader builds compiled against actual game references; game method bodies/resources are not exported.
3. Run `export-sdk` as shown above, using the actual verified game version and a new revision. Existing snapshots cannot be overwritten.
4. Review metadata/hash changes, migrate selected `release.json` pins, update Mod versions, bilingual CHANGELOG entries with game/SDK versions, compatibility/risk notes, root supported baseline and the table above. Mods not migrated keep their previous SDK.
5. Run the SDK build followed by `verify-game` above. This verifies dependency hashes, compares symbolic Mod IL and embedded resources against a real-reference build, and runs the existing Stack All/Price Probability game contracts. It never executes or modifies the game. The command currently verifies every Mod; when pins span multiple game baselines, retain matching installations and verify each Mod using its build/contract commands. Do not bypass hash failures.
6. Review Harmony targets, RPC/save behavior and IL assumptions even if API signatures did not change. Perform in-game checks appropriate to the risks. Keep `"prerelease": true` until acceptance is complete. Commit on `dev` and merge through a PR into `main`.

SDK updates follow maintainer-verified game versions; they do not automatically overwrite a reviewed snapshot whenever the game updates.
