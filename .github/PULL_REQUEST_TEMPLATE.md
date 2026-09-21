## 问题与行为变化 / Problem and behavior changes

<!-- 描述触发条件、修改前后行为，关联 Issue（如有）。普通贡献目标为 main。
Describe the trigger and behavior before/after; link related Issues if any. Normal contributions target main. -->

## 修改范围 / Scope

<!-- 哪个 Mod/工具/文档；是否修改配置、存档结构、网络或加载器依赖。
Identify the Mod, tool, or documentation; state any changes to configuration, save structure, networking, or loader dependencies. -->

## Mod 版本状态 / Mod version status

<!-- 检查所有 Mod 的 CHANGELOG 并对照 PR 差异，使用“Version status”列记录版本状态：已授权升级的填写目标版本；文档修正、不涉及发布的改动或本次未改动的 Mod 填写 Unchanged (现有版本)；只有本次计划发布新版本、但目标版本确实待定时才填写 **UNASSIGNED**。
在说明栏另行记录 Unreleased 内容及其是否属于本次改动；存在未发布条目不等于版本待指定。列出确实待指定目标版本的 Mod，没有则写“无”。不自动升级，也不把保持现有版本当作新版本发布授权。
Inspect every Mod CHANGELOG against the PR diff. Use a “Version status” column: give the target version for an authorized bump, Unchanged (current version) for documentation corrections, changes outside a release scope or untouched Mods, and **UNASSIGNED** only when a new release is planned in this PR but its target version is undecided.
Describe Unreleased content and whether it belongs to this PR separately in the scope/status column; pending entries alone do not imply a missing assignment. List Mods actually awaiting a target version, or write “none”. Do not bump automatically or treat an unchanged version as authorization for a new release. -->

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
- [ ] 已区分保持现有版本、已授权升级及计划发布但目标版本待定的 Mod；版本推进均有维护者明确指令，其余改动保留在 Unreleased。 / Version status distinguishes unchanged versions, authorized bumps and planned releases awaiting a target version; each bump has an explicit maintainer instruction and all other changes remain Unreleased.

<!-- 本地构建不等于安装授权。不要把编译通过写成联机、存档或性能验收通过。
A local build does not authorize installation. Do not describe successful compilation as multiplayer, save, or performance acceptance testing. -->
