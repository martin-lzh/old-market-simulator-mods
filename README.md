# Old Market Simulator Mods

Old Market Simulator 的五个原创 Mod，包含源码、测试和构建脚本。本仓库为非官方项目。

| Mod | 版本 | 功能 |
| --- | --- | --- |
| [Material Cost](material-cost-mod/README.md) | 0.5.0 | 日报原料成本、配方与订购页成本和利润 |
| [Coordinates](coordinates-mod/README.md) | 0.1.2 | 角色世界坐标，F8 切换 |
| [Checkout All](checkout-all-mod/README.md) | 0.1.3 | 长按 E 或 F9 连续结账 |
| [Stack All](stack-all-mod/README.md) | 0.2.0 | 每格最多 64 个容器、数量显示和长按丢弃/投掷 |
| [Price Probability](price-probability-mod/README.md) | 0.1.0 | 定价接受率预览、固定售价或概率及自动调价 |

## 构建

需要 Windows、.NET 8 SDK 或兼容的更新 SDK、自己安装的游戏及 BepInEx 5。游戏与加载器程序集仅作本机引用，不随源码分发。默认游戏位置为 `F:\SteamLibrary\steamapps\common\Old Market Simulator`，可通过 `-GameDir` 指定其他位置。

在仓库根目录用 PowerShell 运行：

```powershell
./material-cost-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./coordinates-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./checkout-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./stack-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
./price-probability-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

构建产物位于 `outputs/`，脚本不会自动安装。成本插件另有 MelonLoader 和[独立启动入口](material-cost-mod/STANDALONE.md)，请阅读相应说明，勿同时启用多个加载入口。

## 安装与兼容性

退出游戏后备份旧插件，再按各 Mod 说明安装生成的 DLL。源码仓库不附带游戏、加载器或依赖 DLL。测试依据为 Old Market Simulator 2.1.6；部分插件检查游戏程序集哈希，其他版本可能拒绝启用。

Stack All 0.2.0 需要房主及所有玩家安装同版本。卸载前必须按其说明清理额外容器记录并在游戏内保存。其他插件的多人限制见各自说明。

自动化测试和编译成功不等于游戏内界面、存档流程或多人联机验收完成，具体待验证项目见各 Mod 文档。

## 仓库范围

仅发布原创 Mod 源码、测试替身及构建说明。不包含反编译游戏源码、游戏资源、存档、个人配置、本机分析数据或第三方二进制文件。游戏及第三方组件的权利归各自权利人所有。
