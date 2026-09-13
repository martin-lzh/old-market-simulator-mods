# 使用问题与反馈 / Support

[中文](#中文) | [English](#english)

## 中文

先查 [总说明](README.md) 中的当前支持版本，以及对应 Mod 的 README 风险提示、安装和回退步骤、CHANGELOG 兼容记录。当前没有承诺适配所有旧版游戏或所有 Mod 组合。

普通问题使用 Bug report 模板，功能建议使用 Feature request 模板；中英文均可。问题追踪：[GitHub Issues](https://github.com/martin-lzh/old-market-simulator-mods/issues)。安全漏洞不要使用公开 Bug 模板，见 [SECURITY.md](SECURITY.md)。

报告问题时提供：

- Mod 名称和版本、游戏版本及可获取的程序集 SHA256、加载器和版本、系统环境。
- 单机/房主/客人身份；联机时双方的版本与安装组合。
- 从哪个版本升级、是否使用旧存档、是否改过插件配置。
- 可重复的操作步骤、预期和实际结果，以及相关的脱敏错误片段。
- 是否做过对照，以及哪些验证没有执行。

日志可能包含用户名、绝对路径、房间码或其他隐私，粘贴前先检查。不要上传完整存档、游戏 DLL、原始资源或反编译源码。

测试时先备份，退出游戏后只调整需要对照的插件。Stack All 含额外容器记录的存档不能直接停用插件后读取；使用备份副本或遵循其拆分回退步骤。不要通过删除整个加载器或其他插件来代替单项排查。

维护者可能需要更多信息，也可能无法复现。提供了构建或测试结果不代表已经完成游戏内验收，当前没有固定支持时限。

## English

First check the supported versions in the [main README](README.md), and the relevant Mod's README risks, installation/rollback steps, and CHANGELOG compatibility records. Compatibility with every older game version or Mod combination is not promised.

Use the Bug report template for problems and Feature request for suggestions. Chinese and English are welcome. Use [GitHub Issues](https://github.com/martin-lzh/old-market-simulator-mods/issues). Do not use public bug reports for vulnerabilities; see [security reporting](SECURITY.md#english).

Include:

- Mod name/version, game version and assembly SHA256 if available, loader/version, and operating environment.
- Single-player/host/client role; both sides' versions and installed Mod combinations for multiplayer.
- The version you upgraded from, whether you use an existing save, and any plugin configuration changes.
- Reproducible steps, expected and actual results, and relevant redacted error excerpts.
- Any comparison tests performed and checks not run.

Inspect logs before posting: they may include usernames, absolute paths, room codes, or other private information. Do not upload complete saves, game DLLs, original assets, or decompiled source.

Back up before testing, exit the game, and adjust only the plugin needed for the comparison. Do not load a Stack All save containing extra container records with the plugin simply disabled; use a backup copy or follow its unpacking/rollback steps. Do not delete an entire loader or other plugins as a substitute for isolating one issue.

Maintainers may need more information or may be unable to reproduce the issue. Build/test results do not constitute in-game acceptance testing. There is no fixed support response time.
