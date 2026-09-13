# 安全问题报告 / Security Reporting

[中文](#中文) | [English](#english)

## 中文

安全问题包括插件或构建脚本导致的非预期文件写入、任意代码执行、凭据泄露及可被利用的网络输入处理问题。一般卡顿、显示错误或功能失效请按 [支持说明](SUPPORT.md) 报告；若存在可利用的漏洞则按本文件处理。

### 支持范围

优先调查 [总说明](README.md) 所列当前 Mod 和游戏基线。旧版本可报告，但没有承诺长期维护或为每个旧版提供补丁；修复可能要求升级，涉及存档时会说明迁移与回退限制。

### 私下报告

目前未公布专用安全邮箱；私密报告入口是否可用以仓库当前设置为准。如果仓库启用了 GitHub 私密漏洞报告，可使用 Security 页的 Report a vulnerability；该入口不存在时，先提交不含利用细节的一般 Issue，请维护者提供私密联系方式。不要把漏洞利用步骤、恶意文件或秘密信息直接贴在公开 Issue/PR 中，也不要猜测维护者邮箱。

私密渠道确认后，提供受影响 Mod/游戏/加载器版本、最小复现、影响范围和可能的修复建议。只提供必要且脱敏的代码片段，不上传游戏程序集、存档、凭据或完整日志。不在他人服务器或未获许可的环境验证漏洞。

尚无固定响应或修复时限。公开细节前与维护者协调；修复发布时应在 CHANGELOG 中标明受影响及修复版本和必要的回退限制。此流程不代表仓库已启用任何平台安全功能。

## English

Security issues include unintended file writes, arbitrary code execution, credential exposure, or exploitable network input handling in plugins or build scripts. Report ordinary stuttering, display errors, or broken features through [support](SUPPORT.md#english); use this process if an exploitable vulnerability is involved.

### Supported scope

Investigation prioritizes the current Mods and game baseline listed in the [main README](README.md). Reports about older versions are welcome, but there is no promise of long-term maintenance or patches for every old release. Fixes may require upgrading; save-related changes will include migration and rollback limitations.

### Private reporting

No dedicated security email is published. Availability of private reporting depends on the current repository settings. Use “Report a vulnerability” on the repository's Security page if GitHub private vulnerability reporting is enabled. If that option is absent, open a general Issue without exploit details and ask maintainers for a private contact method. Do not post exploit steps, malicious files, or secrets in public Issues/PRs, and do not guess maintainer email addresses.

Once a private channel is confirmed, provide affected Mod/game/loader versions, a minimal reproduction, impact, and possible fixes. Include only necessary, redacted code excerpts; do not upload game assemblies, saves, credentials, or complete logs. Do not test vulnerabilities on other people's servers or in environments without permission.

There is no fixed response or resolution time. Coordinate with maintainers before public disclosure. When fixes are released, the CHANGELOG should identify affected and fixed versions and any rollback limitations. This process does not mean any platform security feature has already been enabled.
