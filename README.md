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

## 许可证

仓库中的原创源码、测试、构建脚本及文档采用 [MIT License](LICENSE)，版权声明为 `Copyright (c) 2026 Zhaohan Liu`。各 Mod 目录均附有独立许可证，便于单独使用和分发：

| Mod | 许可证 |
| --- | --- |
| Material Cost | [MIT](material-cost-mod/LICENSE) |
| Coordinates | [MIT](coordinates-mod/LICENSE) |
| Checkout All | [MIT](checkout-all-mod/LICENSE) |
| Stack All | [MIT](stack-all-mod/LICENSE) |
| Price Probability | [MIT](price-probability-mod/LICENSE) |

MIT 允许使用、修改和再分发（包括商业使用），但复制软件或其重要部分时必须保留版权声明和许可声明。构建包附带对应 Mod 的 LICENSE。原始游戏、加载器和第三方依赖不属于本项目的 MIT 授权范围，继续适用各自许可证。

## 引用项目

如果你参考了本项目的实现、将代码用于其他 Mod，或在文章、视频和研究中介绍相关功能，请注明来源并链接本仓库。建议同时注明具体 Mod、版本或提交号，方便读者追溯。

可使用以下格式，将版本或提交号替换为实际使用的值：

> Zhaohan Liu. Old Market Simulator Mods — Mod 名称，版本或提交号. https://github.com/martin-lzh/old-market-simulator-mods

在其他项目的 README 或致谢中也可写：

```markdown
本项目参考了 [martin-lzh/old-market-simulator-mods](https://github.com/martin-lzh/old-market-simulator-mods) 的相关实现。
```

此处的项目引用是一项请求，不是 MIT 的附加许可条件。仅添加项目链接不能替代 MIT 要求保留的版权声明与许可声明；仅参考思路而未复制代码，也不会因此被本项目额外要求承担引用义务。
