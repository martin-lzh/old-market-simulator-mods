# Old Market Material Cost 独立加载版

启动入口 0.1.0，成本 UI 0.5.1。适用于本机 Old Market Simulator 2.1.6 / Unity 2022.3.62f3 Windows x64 Mono。

本项目编写 managed 启动入口，借助未修改的 Unity Doorstop 4.5.0 进入游戏原生 Mono。独立包不运行 BepInEx 或 MelonLoader；底层 Doorstop 也是 BepInEx 使用的引导工具，因此不是完全独立重写的原生加载器，不能凭此保证修复覆盖层卡顿。

启动入口只解析本包四个补丁依赖和成本模块，不扫描所有插件，不修改 CultureInfo、数字小数符号、Mono 调试器、网络、EOS、Steam 覆盖层、画质、帧率或 GC 配置。原始游戏文件与存档不修改。场景加载后建立隐藏的 UI 管理对象，成本模块仍只注册四个 UI Postfix（含订购页商品详情）；配方按选中项懒加载，日报按开关计算。没有逐帧诊断、周期扫描或日志控制台。

## 安装与回退

游戏完全退出后备份已添加的加载器文件。先确保 MelonLoader 的 version.dll 与 BepInEx 的 winhttp.dll 已停用，不能混合运行。仅将包内 winhttp.dll、doorstop_config.ini 和 OldMarketMod 目录放入游戏根目录；遇到同名原始游戏文件不得覆盖。

启动后检查 OldMarketMod/loader.log：应有 Entry culture、Scene callback registered、Standalone UI ready。系统为 de-DE 时小数符号应为逗号；本加载器只记录、不强制设置文化。原版自身跨系统区域格式的不兼容不属于此加载器修复范围。

- OldMarketMod/enabled.txt：true 加载成本模块；false 仅执行启动入口并记录一次日志，不加载 Harmony、成本模块或注册 Unity 回调。修改后重启。
- OldMarketMod/materials-only.txt：日报开关记忆，仅存本 Mod 显示设置，不是游戏存档。
- 日志每次启动保留上一份为 loader.log.previous；没有定时日志。
- 回退原版：退出游戏，将此包 winhttp.dll 改名为 winhttp.dll.standalone-disabled。保留备份，勿启用旧加载器，勿删除存档。

需要实际验证配方成本、当日建议价利润、日报开关、旧档障碍物状态、卡顿和房间码联机。编译通过不代表上述运行期项目已经通过。

## 第三方组件

原生入口与独立补丁库取自已验证的官方 BepInEx 5.4.23.5 压缩包，只复用以下第三方组件，不加载其 BepInEx 框架：

- [Unity Doorstop 4.5.0](https://github.com/NeighTools/UnityDoorstop)（LGPL-2.1），原样 winhttp.dll。
- [HarmonyX 2.9.0](https://github.com/BepInEx/HarmonyX/tree/v2.9.0)（MIT），0Harmony.dll。
- [MonoMod 22.1.29.1](https://github.com/MonoMod/MonoMod)（MIT），RuntimeDetour 与 Utils。
- [Mono.Cecil 0.10.4](https://github.com/jbevain/cecil/tree/0.10.4)（MIT）。

此包为本机验证产物，不包含游戏程序集、资源、存档或反编译代码。构建：运行 build-standalone.ps1，仅写项目 work/outputs，不自动安装。
