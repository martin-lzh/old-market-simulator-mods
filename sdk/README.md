# Mod compilation SDK / Mod 编译 SDK

## 中文

本 SDK 让 GitHub 托管 Runner 在没有安装游戏的情况下编译 Mod。使用标准 .NET 编译器；`api.json` 保存 Mod 用到的类型、字段、方法签名、泛型约束、枚举值及必要的编译元数据。没有原始游戏 DLL、游戏方法实现、资源、存档或反编译源码。

`tools/Sdk` 用 Mono.Cecil 将声明重建为带 `ReferenceAssemblyAttribute` 的编译引用，所有非抽象方法都是工具生成的 `throw null` 占位体。SDK 不是游戏运行库，不能运行游戏，也不能安装到游戏或放进 Mod 下载包。发布脚本只打包原创 Mod DLL、README、CHANGELOG 和 LICENSE。加载器从官方固定版本下载并校验 SHA256。

### 版本与支持范围

| 游戏版本 | SDK | 平台 | 说明 |
| --- | --- | --- | --- |
| 2.1.6 | [2.1.6/r1](2.1.6/r1/manifest.json) | Windows x64，Unity Mono 2022.3.62f3 | 原有五个 Mod 的六种发行构建；实机 UI、存档及双端联机验收未完成 |
| 2.1.6 | [2.1.6/r2](2.1.6/r2/manifest.json) | Windows x64，Unity Mono 2022.3.62f3 | Navigation 0.1.0 固定此修订；增加地图 UI、物理投影、输入、区域与反射依赖，实机验收未完成 |
| 2.1.6 | [2.1.6/r3](2.1.6/r3/manifest.json) | Windows x64，Unity Mono 2022.3.62f3 | Navigation 0.1.1 固定此修订；增加原生 HUD 容器、补间端点与跨画布屏幕坐标接口，实机验收未完成 |
| 2.1.6 | [2.1.6/r4](2.1.6/r4/manifest.json) | Windows x64，Unity Mono 2022.3.62f3 | Navigation 0.1.2 固定此修订；增加原生按键提示预制体与布局接口，实机验收未完成 |
| 2.1.6 | [2.1.6/r5](2.1.6/r5/manifest.json) | Windows x64，Unity Mono 2022.3.62f3 | Navigation 0.1.3 固定此修订；增加输入框焦点、原生提示子控件与独立布局接口，实机验收未完成 |

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
2. 准备 `supplemental.json`：列出只出现在 `nameof` 中、不会生成成员引用的游戏方法。可在 `work/` 下复制旧文件再调整。反射方法同样列出；反射字段使用显式 `field:字段名`，如 `field:currentSlot` 和 `field:activeExpansions`。跨程序集类型使用 `程序集简单名::类型全名`；编译内联的数值/布尔常量使用 `constant:字段名`，显式审核真实值，拒绝字符串或非字面量。所有名称必须来自真实程序集。导出工具自动从已注册六个 Mod 的七个发行构建读取其他依赖声明，不读取游戏方法体或资源。
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

2026-09-13：r2 从源码提交 `135eb89` 导出，共 15 个程序集、255 个类型、641 个方法、469 个字段，仅含声明。原 r1 全部类型/方法/字段保留，原 13 个程序集身份与哈希不变；r1 文件及旧 Mod pin 未修改。六个 Mod 的七个构建均通过 SDK 编译与真实引用符号 IL/嵌入资源比对；Navigation 511 项逻辑/本地化检查、318 项只读原生契约、22 项 CI 工具测试通过。实机验收未执行，Navigation 保持预发布。

2026-09-13：r3 从源码提交 `b038bf0` 导出，共 15 个程序集、257 个类型、652 个方法、476 个字段，仅含声明。全部 r2 声明及 15 个程序集身份/哈希保留；新增 `Screen` 的 UnityEngine 类型转发与真实组件一致。仅 Navigation 0.1.1 改用 r3，旧快照与其他 Mod pin 不变。左下默认布局使用相同 API；最终版本通过七个构建的 SDK/真实引用符号 IL 和资源比对、564 项导航检查、400 项只读原生契约及 22 项 CI 工具测试。实机 UI 与动画验收未执行。

## English

This SDK lets GitHub-hosted runners compile Mods without a game installation. The standard .NET compiler consumes references reconstructed from `api.json`: the types, fields, method signatures, generic constraints, enum values and compiler metadata used by these Mods. No original game DLLs, game method implementations, resources, saves or decompiled source are included.

`tools/Sdk` uses Mono.Cecil to produce assemblies marked `ReferenceAssemblyAttribute`; every concrete method contains a tool-authored `throw null` placeholder. These are compiler references, never runtime libraries. Do not install or distribute them with a Mod. Packages contain only the original Mod DLL, README, CHANGELOG and LICENSE. Official loader downloads are version-pinned and SHA256-verified.

### Versioning and compatibility

The table records game 2.1.6 on Windows x64 / Unity Mono 2022.3.62f3. The original five Mods retain SDK r1; Navigation 0.1.0 pins r2, adding map UI, physics projection, input, region and reflection declarations. Navigation 0.1.1 pins r3, adding native HUD containers, tween endpoints and screen-coordinate conversion across canvases. CI builds six Mods and seven loader variants. In-game UI, save and multiplayer acceptance remains incomplete.

Each Mod pins an SDK in its own `release.json`, for example `"sdk": "2.1.6/r1"`. There is no implicit latest SDK. Merged `sdk/<game-version>/r<revision>/` snapshots are immutable. Add a new revision to extend the same game baseline, or a new game-version directory after a game update. Preserve old SDKs, Mod releases and CHANGELOG records; users must still check save, loader and multiplayer requirements.

The manifest records game version, source commit, game assembly hash and API snapshot hash. API metadata records the identities and local hashes of each revision’s required dependency assemblies. Original tools use the root MIT license. Game, Unity and loader names/API identifiers belong to their respective owners; this project does not grant redistribution rights to their implementations or resources.

### Building and updating

Install Python 3.12 plus .NET 8 and 10 SDKs. Run the three validation/build commands above from the repository root; CI uses Windows. Packages go to `outputs/ci/<mod>/`; generated references stay in ignored `work/ci-sdk/`. Reference paths are explicit and never fall back to an installed game. An API absent from the selected SDK must be checked against the real game before adding a revision.

After a game update:

1. Read the legally obtained game installation, adapt the original Mod source and commit it. Export requires a clean worktree to record the exact source commit.
2. Review supplemental method names used only in `nameof` expressions. Copy the previous `supplemental.json` into ignored `work/` if it needs editing. Reflection method names are also listed; reflection fields use the explicit `field:name` syntax (for example `field:currentSlot` and `field:activeExpansions`). Use `AssemblySimpleName::Type.FullName` for non-game assembly types. Explicit `constant:name` seeds cover reviewed, compiler-inlined numeric/boolean literals; string and nonliteral seeds are rejected. All declarations must resolve from real assemblies. Other declarations are discovered from the seven registered loader builds compiled against actual game references; game method bodies/resources are not exported.
3. Run `export-sdk` as shown above, using the actual verified game version and a new revision. Existing snapshots cannot be overwritten.
4. Review metadata/hash changes, migrate selected `release.json` pins, update Mod versions, bilingual CHANGELOG entries with game/SDK versions, compatibility/risk notes, root supported baseline and the table above. Mods not migrated keep their previous SDK.
5. Run the SDK build followed by `verify-game` above. This verifies dependency hashes, compares symbolic Mod IL and embedded resources against a real-reference build, and runs the existing Stack All/Price Probability game contracts. It never executes or modifies the game. The command currently verifies every Mod; when pins span multiple game baselines, retain matching installations and verify each Mod using its build/contract commands. Do not bypass hash failures.
6. Review Harmony targets, RPC/save behavior and IL assumptions even if API signatures did not change. Perform in-game checks appropriate to the risks. Keep `"prerelease": true` until acceptance is complete. Commit on `dev` and merge through a PR into `main`.

SDK updates follow maintainer-verified game versions; they do not automatically overwrite a reviewed snapshot whenever the game updates.

Validation on 2026-09-13: r2 was exported from source `135eb89` and contains 15 assemblies, 255 types, 641 methods and 469 fields, declarations only. All r1 declarations are retained; the original 13 identities/hashes and old SDK pins are unchanged. All seven builds passed SDK compilation and real-reference symbolic IL/resource comparison. Navigation passed 511 logic/localization checks and 318 read-only game contracts; the CI tools passed 22 tests. In-game acceptance is not performed; Navigation remains a prerelease.

Validation on 2026-09-13: r3 was exported from source `b038bf0` and contains 15 assemblies, 257 types, 652 methods and 476 fields, declarations only. All r2 declarations and all 15 assembly identities/hashes remain intact; the additional UnityEngine `Screen` forwarder matches the real assemblies. Only Navigation 0.1.1 moves to r3; old snapshots and other Mod pins are unchanged. The bottom-left layout uses the same API. The final version passed all seven SDK/real-reference symbolic IL and resource comparisons, 564 navigation checks, 400 read-only game contracts and 22 CI tooling tests. In-game UI and animation acceptance was not performed.

2026-09-13：r4 从源码提交 `b242768` 导出，共 15 个程序集、258 个类型、655 个方法、481 个字段，仅含声明。Navigation 0.1.2 使用原生按键提示预制体与布局接口；其他 Mod 固定版本不变。七个构建通过 SDK/真实引用符号 IL 与资源比对，606 项导航检查、424 项只读原生契约和 22 项 CI 工具测试通过。实机显示验收未执行。

Validation on 2026-09-13: r4 was exported from source `b242768`, with 15 assemblies, 258 types, 655 methods and 481 fields, declarations only. Navigation 0.1.2 uses native key-hint prefab and layout APIs; other Mod pins are unchanged. All seven builds passed SDK/real-reference symbolic IL and resource comparison, with 606 navigation checks, 424 read-only game contracts and 22 CI tooling tests passing. In-game visual acceptance remains outstanding.

2026-09-13：r5 从源码提交 `907b196` 导出，共 15 个程序集、259 个类型、656 个方法、484 个字段，仅含声明。新增 TMP/UGUI 输入框焦点、包含隐藏子控件的查找、ContentSizeFitter 与颜色字段；不再引用 LayoutRebuilder 和无参子控件查找。15 个程序集身份与哈希同 r4，历史快照保留原样，仅 Navigation 0.1.3 改用 r5。七个构建通过 SDK/真实引用符号 IL 与资源比对，606 项导航检查、440 项只读原生契约及 22 项 CI 工具测试通过。空按键槽回归契约对已安装 0.1.2 正确失败，对修复版通过；不执行游戏代码，实机显示及输入验收仍待完成。

Validation on 2026-09-13: r5 was exported from source `907b196` with 15 assemblies, 259 types, 656 methods and 484 fields, declarations only. It adds TMP/UGUI editing focus, inactive-child lookup, ContentSizeFitter and color fields; unused LayoutRebuilder and parameterless child lookup are no longer referenced. All 15 assembly identities/hashes match r4. Historical snapshots remain unchanged, and only Navigation 0.1.3 moves to r5. All seven builds passed SDK/real-reference symbolic IL and resource comparison; 606 navigation checks, 440 read-only game contracts and 22 CI tooling tests passed. The null-key-slot regression correctly rejects the installed 0.1.2 DLL and passes the repaired build. No game code executes in these checks; in-game layout and input acceptance remain outstanding.
