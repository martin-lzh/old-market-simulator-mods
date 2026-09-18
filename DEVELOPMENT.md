# Development / 开发

Player information: [README](README.md). Contribution setup and branch rules: [CONTRIBUTING](CONTRIBUTING.md). Build/reference details: [SDK](sdk/README.md). Versioning and publication: [release management](releases/README.md). Test scope: [validation record](releases/validation.md).

玩家介绍见 [README](README.md)，开发环境和分支规则见 [CONTRIBUTING](CONTRIBUTING.md)，引用与编译见 [SDK](sdk/README.md)，版本与发行见 [发布规则](releases/README.md)，测试范围见 [验证记录](releases/validation.md)。

| Mod | Development notes / 开发说明 |
| --- | --- |
| Navigation | [DEVELOPMENT](navigation-mod/DEVELOPMENT.md) |
| Coordinates | [DEVELOPMENT](coordinates-mod/DEVELOPMENT.md) |
| Checkout All | [DEVELOPMENT](checkout-all-mod/DEVELOPMENT.md) |
| Material Cost | [DEVELOPMENT](material-cost-mod/DEVELOPMENT.md) · [Standalone](material-cost-mod/STANDALONE.md) |
| Tree Info | [DEVELOPMENT](tree-info-mod/DEVELOPMENT.md) |
| Stack All | [DEVELOPMENT](stack-all-mod/DEVELOPMENT.md) |
| Price Probability | [DEVELOPMENT](price-probability-mod/DEVELOPMENT.md) |

### Current support and risks

As of 2026-09-13, the latest supported build/reference baseline is **Old Market Simulator 2.1.6**, Windows x64 / Unity Mono 2022.3.62f3. This is not a claim of completed in-game testing or support for subsequent game updates. Assembly-CSharp.dll SHA256: `FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296`. Current per-Mod versions and downloads are listed in [README](README.md).

Installing a Mod or loader may cause save incompatibility, failed connections to/from friends, desynchronization, startup failures, stuttering or crashes. Risks differ by code path and installed combination; these are possible outcomes, not confirmed faults in every Mod.

| Risk | What to check |
| --- | --- |
| Saves and rollback | Back up saves/plugins/configuration. Stack All's extra records need a compatible plugin; deleting DLLs does not undo saved inventory, prices or transactions. |
| Multiplayer | Verify both sides' game/Mod versions and host/client behavior. Stack All requires matching versions for everyone; display-only Mods do not automatically require all players to install. |
| Updates and conflicts | Game APIs, loader entry points, duplicate DLLs and other Mods affecting the same UI/logic may conflict. |
| Performance and stability | Resource queries, per-frame UI, inventory synchronization and automation add work. Loader-only stuttering has occurred in local comparisons; no fix is guaranteed. |
| Accidental actions and economy | Holds/toggles and price anchors perform real actions. Stopping or uninstalling does not reverse completed actions. |
| Display and estimates | Text may overlap or lack glyphs. Material costs, acceptance rates and profit estimates are not actual net profit or daily sales guarantees. |

Exit before installation/upgrades/removal and test backup copies first. Follow each Mod's risk and rollback instructions; do not load a Stack All extended save in vanilla just because an update disables the plugin. See the independent change logs for release downloads and compatibility:

- [Material Cost Change Log](material-cost-mod/CHANGELOG.md)
- [Coordinates Change Log](coordinates-mod/CHANGELOG.md)
- [Checkout All Change Log](checkout-all-mod/CHANGELOG.md)
- [Stack All Change Log](stack-all-mod/CHANGELOG.md)
- [Price Probability Change Log](price-probability-mod/CHANGELOG.md)
- [Navigation Change Log](navigation-mod/CHANGELOG.md)

### Build from source

Clone the repository, or take the directory for the mod you want to build. Each mod is self-contained and requires no sibling mod or root localization project. You need PowerShell, a .NET SDK capable of building .NET 8 projects, a local game installation, and the selected loader's reference assemblies. Builds read game files and write only local `work/`, `outputs/`, and build-cache directories; they do not install mods.

Run from the repository root, replacing the example game path:

```powershell
./material-cost-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./coordinates-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./checkout-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./stack-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./price-probability-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./navigation-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

ZIPs appear in `outputs/`. Each mod with translatable UI runs its own localization checks and applicable feature tests. Coordinates has no translatable messages and no localization-project dependency. Passing builds and tests do not establish in-game UI, save, or multiplayer correctness. See [release management](releases/README.md) for independent tags, release notes, and artifact checks.

### 当前支持版本与风险

截至 2026-09-13，最新支持的构建/引用基线为 **Old Market Simulator 2.1.6**、Windows x64 / Unity Mono 2022.3.62f3，不代表已完成实机验收或支持后续游戏更新。Assembly-CSharp.dll SHA256 同上；各 Mod 当前版本和下载见 [README](README.md)。

安装 Mod 或加载器可能导致旧档不兼容、无法加入好友/好友无法加入、同步异常、启动失败、卡顿或崩溃。具体风险取决于代码及安装组合；这是可能后果，不表示每个 Mod 已发生这些故障。

| 风险 | 检查要点 |
| --- | --- |
| 存档与回退 | 备份存档/插件/配置。Stack All 扩展记录需要兼容插件；删除 DLL 不撤销已保存的库存、售价或交易。 |
| 联机 | 检查双方游戏/Mod 版本及房主/客人行为。Stack All 全员同版本；纯显示 Mod 不能一概要求全员安装。 |
| 更新与冲突 | 游戏接口、加载入口、重复 DLL 或修改相同 UI/逻辑的 Mod 可能冲突。 |
| 性能与稳定性 | 资源查询、逐帧 UI、库存同步和自动操作增加开销；本机零插件加载器对照也曾卡顿，不保证修复。 |
| 误操作与经济 | 长按/开关及价格锚定执行真实操作，停止或卸载不撤销已完成动作。 |
| 显示与估算 | 文字可能遮挡/缺字；原料成本、接受率和利润估算不是实际净利润或日销量保证。 |

安装/升级/卸载前退出游戏，先测试备份副本，遵循各 Mod 风险及回退说明；不要因为更新后插件失效就用原版读 Stack All 扩展存档。历史下载及兼容性见独立版本记录：

- [Material Cost Change Log](material-cost-mod/CHANGELOG.md)
- [Coordinates Change Log](coordinates-mod/CHANGELOG.md)
- [Checkout All Change Log](checkout-all-mod/CHANGELOG.md)
- [Stack All Change Log](stack-all-mod/CHANGELOG.md)
- [Price Probability Change Log](price-probability-mod/CHANGELOG.md)
- [Navigation Change Log](navigation-mod/CHANGELOG.md)

### 从源码构建

可以克隆仓库，也可以单独取出所需 Mod 的目录；各 Mod 独立构建，不依赖其他 Mod 或仓库级本地化工程。需要 PowerShell、支持 .NET 8 工程的 .NET SDK、本机游戏和相应加载器的引用程序集。构建只读取游戏文件，产物写入项目 `work/`、`outputs/` 和构建缓存目录，不自动安装。

在仓库根目录运行前面英文部分的五条构建命令，将示例游戏路径换为你的安装位置。ZIP 输出至 `outputs/`。有可翻译界面的 Mod 执行自己的本地化及功能测试；坐标没有可翻译文案，不依赖本地化工程。编译和测试通过不等于游戏内界面、保存或联机已验证。独立标签、发布说明和产物核验流程见[版本发布规则](releases/README.md)。
