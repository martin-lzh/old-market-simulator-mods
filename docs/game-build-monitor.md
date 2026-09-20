# Steam game build monitoring / Steam 游戏构建监测

## 中文

[Steam game build monitor](../.github/workflows/game-build-monitor.yml) 使用 Valve SteamCMD 匿名查询 Old Market Simulator（App ID `2878420`）的 `depots.branches.public.buildid`。不需要 Steam API Key、账号密码或购买凭据，也不下载、更新、启动游戏。

工作流合并到默认分支 `main` 后，每天 UTC 00:23、06:23、12:23、18:23 运行，可从 Actions 页面手动运行。相关文件推送至 `dev` 时也执行真实查询，但只读、不创建 Issue。分支检查与通知共用串行并发组，避免同时读取旧状态后重复通知。SteamCMD 安装包来自 Valve HTTPS 地址，执行前验证 Valve 的 Authenticode 签名；SteamCMD 自行更新后再查询元数据。没有使用第三方查询服务。

### 检测和通知

- 初始构建记录在 [game-build-baseline.json](../tools/game-build-baseline.json)：2026-09-21 通过 SteamCMD 直接查询到 `25102733`，分支更新时间为 2026-09-03 13:30:20 UTC。本机程序集哈希与仓库游戏 2.1.6 SDK 基线一致；Build ID 不等于游戏版本号，也不单独证明兼容性。
- 与最近一次检测记录的 Build ID 不同就创建双语兼容性检查 Issue，包括新旧构建、时间、官方公告入口和检查清单。较小的 Build ID 也视为变化，因此可以识别观察到的回滚。仅时间或其他分支元数据变化不提醒。
- 只在 `main` 上允许通知。使用 Actions 自动提供的 `GITHUB_TOKEN`，权限为 `contents: read`、`issues: write`，无需额外配置 Secrets。
- 最近创建、带监测元数据且由 `github-actions[bot]` 创建的 Issue 是持久状态；查询包括已关闭 Issue，不依赖短期缓存，不回写 Git。请保留这些 Issue 及其隐藏元数据；删除会丢失部分检测历史。首次没有历史时使用已提交的初始构建，而不是静默接受部署时的新构建。
- 构建不变时不创建或修改 Issue。超时、缺字段、错误游戏、无效数据或 GitHub 读取失败会使工作流失败，不把错误当作“没有更新”，也不推进状态。写入响应丢失后，下次从已有 Issue 恢复，避免再次创建。

GitHub Issue 通知和失败运行通知取决于个人 GitHub 通知设置。若需要站外提醒，可为该仓库启用 Issues 通知及 Actions 失败通知。

### 本地检查

先从 [Valve SteamCMD 文档](https://developer.valvesoftware.com/wiki/SteamCMD)安装并初始化 SteamCMD，放在被忽略的 `work/` 下。查询命令：

```powershell
./work/steamcmd-monitor/steamcmd.exe +login anonymous +app_info_update 1 +app_info_print 2878420 +quit
python tools/game_build_monitor.py --steamcmd work/steamcmd-monitor/steamcmd.exe
```

第二条命令仅与已提交初始构建比较并输出 JSON。提供 `--repository martin-lzh/old-market-simulator-mods` 和环境变量 `GH_TOKEN` 后，还会读取 GitHub 上的检测历史；默认仍不写入。`--notify` 是显式写入开关，应交给 `main` 的工作流使用，以保持 bot 作者身份和串行执行。不要在命令行或仓库文件中填写 Token。

### 更新后的维护与限制

提醒只表示构建变化。按 [SDK 维护流程](../sdk/README.md#游戏更新后的维护)确认游戏版本、哈希、接口和行为后再决定是否适配。此工作流不修改 SDK、Mod 版本、发布授权或游戏安装；不会自动发布 Mod。

每六小时轮询可能遗漏两次检查之间发生且已恢复的短暂切换；SteamCMD 也不是历史构建档案。GitHub 定时任务可能延迟，公开仓库连续 60 天没有活动时会自动禁用定时任务，届时需在 Actions 页面重新启用。见 [GitHub schedule 规则](https://docs.github.com/en/actions/reference/workflows-and-actions/events-that-trigger-workflows#schedule)。

## English

The [Steam game build monitor](../.github/workflows/game-build-monitor.yml) queries Valve directly using anonymous SteamCMD for Old Market Simulator (`2878420`), selecting `depots.branches.public.buildid`. It needs no Steam API key or account credentials and never downloads, updates or launches the game. The Valve HTTPS bootstrap is checked for a valid Valve Authenticode signature before execution, then SteamCMD updates itself. No third-party metadata service is used.

After merging into default branch `main`, checks run at 00:23, 06:23, 12:23 and 18:23 UTC, with manual dispatch available in Actions. Relevant pushes on `dev` perform a live read-only check. One concurrency group serializes checks. Only `main` can create notifications, using the automatically supplied `GITHUB_TOKEN` with `contents: read` and `issues: write`; no extra Secrets are needed.

The [initial baseline](../tools/game-build-baseline.json) was queried directly on 2026-09-21: build `25102733`, branch updated 2026-09-03 13:30:20 UTC. The local game assembly hash matched the repository's game 2.1.6 SDK baseline. Build IDs are not game version numbers or proof of compatibility.

A changed Build ID creates a bilingual compatibility issue with old/new builds, timestamps and verification tasks. Any different ID counts, including rollbacks and later returns to previously observed builds. Timestamp-only changes and other branches do not notify. The most recently created monitor issue authored by `github-actions[bot]`, including closed issues, is durable state. Preserve these issues and their hidden metadata; deleting them loses history. With no history, the committed initial baseline is used. No cache or Git writeback is involved. Unchanged builds produce no issue writes.

Local usage is shown above. Without `--repository`, the script only queries Steam and compares the committed baseline. With the repository and `GH_TOKEN` environment variable, it also reads issue history; it remains read-only unless `--notify` is supplied. Leave notification writes to the serialized `main` workflow so author identity remains consistent. Never place tokens in command arguments or repository files.

Steam query failures, malformed/missing data and GitHub read failures fail the run without advancing state. Issue creation is not blindly retried; subsequent runs recover a successful but uncertain creation from history. GitHub delivery depends on the user's Issues and Actions notification settings.

Follow [SDK maintenance](../sdk/README.md) before claiming compatibility. Monitoring changes no SDK pins, Mod versions, release authorizations or local installations, and cannot publish a Mod. Polling can miss changes that revert between checks. GitHub schedules may be delayed, and public repositories with 60 days of inactivity have schedules disabled; re-enable the workflow in Actions when needed. See [GitHub schedule documentation](https://docs.github.com/en/actions/reference/workflows-and-actions/events-that-trigger-workflows#schedule).
