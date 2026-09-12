# Old Market Coordinates

## 0.1.3 本地化

坐标 X/Y/Z 为通用轴标识，无需翻译。数字使用与游戏一致的当前系统数字格式；字体及材质继续跟随原生金钱栏。

构建需保留仓库根目录的 `localization/`，构建前自动执行语言与占位符检查。详情见[本地化说明](../localization/README.md)。配置键名、插件 ID、日志及开发文档不随游戏语言改名。此版本只更新显示与本地化，不自动安装；游戏内布局、字形及实时切换仍需验证。


进入地图后在金钱显示栏下方显示本地角色的世界坐标，每次启动默认开启，F8 仅切换本次运行的显示状态。X、Z 是水平位置，Y 是高度，保留一位小数，每帧 LateUpdate 读取角色实际移动对象的位置；不是地图格子编号，也未将轴向标成未经验证的东南西北。

这是独立 DLL 插件，通过当前安装的 BepInEx 5 加载，在游戏画面内显示；不是另开的桌面窗口。无需成本插件、CraftSupport 或 Harmony 补丁。只读取本地玩家的位置，不调用传送、网络 RPC 或存档写入。菜单、断线或尚未生成角色时不显示上次的坐标。显示框不接收鼠标点击、不更改光标，也不扫描场景对象。

## 安装与使用

1. 完全退出游戏。
2. 将压缩包中的 `BepInEx/plugins/OldMarket.Coordinates/OldMarket.Coordinates.dll` 按目录放到游戏根目录。依赖已经启用的 BepInEx 5；无需更换加载器。若已有本插件，先备份旧 DLL。
3. 启动并进入地图，查看金钱栏下方的坐标。按 F8 切换显示。

首次运行生成 `BepInEx/config/local.oldmarket.coordinates.cfg`，仅保存插件设置。退出游戏后可编辑 ToggleKey（例如 F9）；Visible 在启动时重置为 true。0.1.1 跟随原生金钱栏定位，在其下方留出 6 个 UI 单位；直接复用 textCoins 的字体资源、字体材质、字号、字重、颜色和字间距，随 HUD 缩放。旧配置中的 Left、Top、FontSize 不再使用。

卸载：退出游戏，将本插件 DLL 移出 `BepInEx/plugins` 即可；不要删除整个 BepInEx 或其他插件。

## 构建与验证

运行 `./coordinates-mod/build.ps1`。使用本机游戏安装和 BepInEx 核心作为只读引用，输出 `outputs/OldMarket.Coordinates-0.1.3.zip`，仅包含原创 DLL 和本说明，不打包游戏组件，不自动安装。

本机依据：Old Market Simulator 的 `ExampleCharacterSetup.OnNetworkSpawn` 按 IsLocalPlayer 初始化本地角色，游戏 UI 通过 `NetworkManager.Singleton.LocalClient.PlayerObject` 访问自己的玩家对象。本插件使用同一路径，取 `transform.position` 世界坐标，在船上也不改用相对船体的位置。输入使用游戏现有 Unity Input System。

目标环境：本机 Old Market Simulator 2.1.6、Unity 2022.3、BepInEx 5.4.23.4。编译和打包检查不能代替实机测试；安装后仍需验证移动、跳跃、F8、暂停菜单、退出重进和联机客户端的显示是否正常，以及是否遮挡现有 HUD。

0.1.1：用户确认 0.1.0 被界面遮挡。改为金钱栏下方的原生 TMP 文本，使用独立 Canvas 排序避免被其他 HUD 面板遮住，保留 F8 开关；只在实际收到切换时记日志。已编译通过，实际位置和显示仍待更新后验证。

0.1.2：移除 0.1 秒刷新间隔，优先读取本地 ExampleCharacterSetup 的 customCharacterController 世界位置，每帧在 LateUpdate 更新；显示保留一位小数，数值未变时不重写文字。忽略旧版保存的隐藏状态，每次启动默认打开。已编译通过，移动显示仍需游戏内确认。

## 许可证与引用

本目录的原创源码、测试及文档采用 [MIT License](LICENSE)。复制软件或其重要部分时须保留版权声明和许可声明；游戏及第三方组件不受此授权覆盖。

如果本 Mod 帮助了你的项目、文章或视频，请注明 Mod 名称并链接到 [Old Market Simulator Mods](https://github.com/martin-lzh/old-market-simulator-mods)。引用格式见[仓库首页](../README.md#引用项目)。这是一项引用请求，不是 MIT 的附加许可条件。
