## 问题与行为变化 / Problem and behavior changes

<!-- 描述触发条件、修改前后行为，关联 Issue（如有）。普通贡献目标为 main。
Describe the trigger and behavior before/after; link related Issues if any. Normal contributions target main. -->

## 修改范围 / Scope

<!-- 哪个 Mod/工具/文档；是否修改配置、存档结构、网络或加载器依赖。
Identify the Mod, tool, or documentation; state any changes to configuration, save structure, networking, or loader dependencies. -->

## Mod 版本归属 / Mod version assignment

<!-- 检查所有 Mod 的 CHANGELOG 并对照 PR 差异，逐项列出涉及的 Mod：用户明确指定的目标版本，或 Unreleased（待用户指定版本）。
同时列出其他仍有 Unreleased 内容、尚未规定版本的 Mod；没有则写“无”。已有旧版本号不代表本轮改动已指定版本，勿自动升级。
Inspect every Mod CHANGELOG against the PR diff. For each affected Mod, record the user-authorized target version or Unreleased (awaiting assignment).
Also list other Mods with pending Unreleased changes and no assigned version; write “none” when empty. An existing old version does not assign new changes. Do not bump automatically. -->

## 验证 / Validation

<!-- 写实际命令和结果；游戏版本、程序集哈希、加载器及 Mod 组合。
区分纯逻辑测试、构建、包检查、实机 UI、单机/房主/客人验证；未执行请注明。
List commands and results, game version, assembly hash, loader, and Mod combination.
Distinguish pure logic tests, builds, package checks, in-game UI, and single-player/host/client verification; identify checks not run. -->

## 兼容与回退 / Compatibility and rollback

<!-- 旧档、新档、旧配置、多人版本要求、已完成操作的持久影响；不适用请说明。
Cover old/new saves, old configuration, multiplayer version requirements, and persistent effects of completed actions. Explain when not applicable. -->

## 文档与提交检查 / Documentation and submission checks

- [ ] 贡献是原创或有明确授权，必要的来源和许可已记录。 / The contribution is original or authorized, with required attribution and licensing recorded.
- [ ] 未提交游戏组件/资源、反编译快照、存档、凭据、日志或构建产物。 / No game components/assets, decompiled snapshots, saves, credentials, logs, or build outputs are included.
- [ ] 功能变化已更新 README 风险/用法及 CHANGELOG 未发布条目，或说明不适用。 / Behavior changes are covered in README risks/usage and the CHANGELOG Unreleased section, or marked not applicable.
- [ ] 新文案已处理本地化、占位符和回退，或说明不适用。 / New text handles localization, placeholders, and fallback, or is marked not applicable.
- [ ] 已检查差异和相关验证；未执行的实机检查已明确记录。 / Diffs and relevant validation have been reviewed; in-game checks not run are documented.
- [ ] 已检查并列出尚未指定版本的 Mod；版本推进均有用户明确指令，其余改动保留在 Unreleased。 / Mods awaiting version assignment are listed; each version bump has an explicit user instruction and all other changes remain Unreleased.

<!-- 本地构建不等于安装授权。不要把编译通过写成联机、存档或性能验收通过。
A local build does not authorize installation. Do not describe successful compilation as multiplayer, save, or performance acceptance testing. -->
