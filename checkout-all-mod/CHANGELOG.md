# Checkout All 0.1.4 Change Log / 版本记录

[English](#english) · [中文](#中文)

## English

### Unreleased

- Add code-specific risks, compatibility guidance and this independent change log. Future builds include CHANGELOG.md. Gameplay code and existing public assets/tags are unchanged.

### 0.1.4 — 2026-09-12 (prerelease)

First public release in this repository: original Mod features with independent localization (Coordinates uses universal axis labels). Detailed features and changes: [release notes](../releases/checkout-all-v0.1.4.md).

- Build/reference baseline: Old Market Simulator 2.1.6, Windows x64 / Unity Mono. BepInEx 5; Material Cost also offers MelonLoader 0.7.3. Standalone remains source-only for local testing.
- Game assembly SHA256: FA6CE6B89AEBDF50DD46FF9C857650DB0E9CC618B1CE939F58501E0DC59C6296.
- Automated checks passed; in-game UI, saves and real multiplayer acceptance remain incomplete.
- [Release/downloads](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.4); source commit: 01685796bc9953122c33c240e92547bedc0edb79.
- Save/rollback: Saves: no custom format/session persistence, but completed native transactions affect normal saves. Uninstalling does not undo bagging/payments; back up and test a copy.
- Multiplayer: Native RPCs need no new connection protocol; server installation is not required by design but real multiplayer is unverified. Latency, disconnects, staff/player races may cause timeouts or incorrect results. Local deduplication cannot guarantee no multiplayer duplicates; connections may fail with incompatible combinations.

Older development versions mentioned in the README are not verified downloadable releases in this public repository. No older/newer supported game baseline is recorded. Match game/hash, loader and save requirements; an older Mod does not imply support for an older game.

## 中文

### 未发布

- 补充代码相关风险、兼容说明和独立版本记录；后续构建包含 CHANGELOG.md。游戏逻辑及现有公开附件/标签不变。

### 0.1.4 — 2026-09-12（预发布）

本仓库首次公开发布：原创功能与独立本地化（坐标使用通用轴标记）。详细功能与变化见[发布说明](../releases/checkout-all-v0.1.4.md)。

- 构建/引用基线：Old Market Simulator 2.1.6，Windows x64 / Unity Mono，BepInEx 5；成本插件另有 MelonLoader 0.7.3，Standalone 仅源码本地测试。
- 游戏程序集 SHA256 同上；自动检查通过，实机 UI、存档及多人验收未完成。
- [发布/下载](https://github.com/martin-lzh/old-market-simulator-mods/releases/tag/checkout-all-v0.1.4)；源码提交同上。
- 存档/回退：存档：无自定义格式/会话持久化，但原生交易会影响正常保存；卸载不撤销装袋/收款，应备份并测试副本。
- 联机：原生 RPC 不新增连接协议，设计上服务器无需安装但未实测。延迟、断线、员工/玩家竞争可能超时或结果异常；本地去重不保证联机绝不重复，不兼容组合可能连接失败。

README 提到的更早开发版本不是本公开仓库已确认的可下载发行版；尚无其他新旧游戏版本兼容记录。匹配游戏/哈希、加载器和存档要求；旧 Mod 不自动等于支持旧游戏。

## Published files / 已发布文件

| File / 文件 | SHA256 |
| --- | --- |
| [OldMarket.CheckoutAll-0.1.4.zip](https://github.com/martin-lzh/old-market-simulator-mods/releases/download/checkout-all-v0.1.4/OldMarket.CheckoutAll-0.1.4.zip) | 4e1db143c08467d53fa10801da86aeb5d21eba378ab45b912845f677cb11525f |

Future entries must record changes, actual date/downloads, game/hash, loader, save/multiplayer/rollback requirements and test scope. Publish package hashes externally in the release checksums; preserve old releases/tags.

后续条目必须记录变化、真实日期/下载定位、游戏/哈希、加载器、存档/联机/回退要求及验证范围。包哈希放发布页外部校验文件，保留历史发布/标签。
