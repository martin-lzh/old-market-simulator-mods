# Development notes / 开发说明

**2026-09-14 — maintainer confirmation:** Visual, local in-game and two-computer multiplayer testing of the preceding build is complete. The subsequent quantity-display fix awaits in-game visual confirmation. This records manual testing feedback; it is not an automated test result.

**2026-09-14 — 维护者确认：**修复前构建的视觉、本地实机及双实机联机测试已完成；后续数量显示修复仍待实机视觉确认。这是人工测试反馈，不是自动测试结果。

Player instructions: [README](README.md). Repository workflow: [CONTRIBUTING](../CONTRIBUTING.md), [SDK](../sdk/README.md), [releases](../releases/README.md). Earlier unchecked-scenario descriptions below are superseded by the confirmation above. Current test scope: [validation record](../releases/validation.md).

玩家用法见 [README](README.md)。本文保留构建、接口与历史测试资料；下方早期待测列表已由上述最新确认更新，当前实机范围以[验证记录](../releases/validation.md)为准。

## Configuration and build

There are no user options; capacity and save representation are compatibility rules.

```powershell
./stack-all-mod/build.ps1
./stack-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

The build uses local game/BepInEx assemblies as read-only references, runs checks, and creates `outputs/OldMarket.StackAll-0.3.0.zip`; it does not install. All required localization source, resources, and tests are contained in `stack-all-mod/localization/`, so this Mod directory builds independently. Startup rejects an unreviewed game assembly or patch surface.

## Earlier validation notes

The original stack/container/input tests, game IL contract checks, and the Mod-local localization checks pass. They cover conservation, empty containers, merges, 64-container overflow, removal, save-array reload, tool durability, hold cancellation, translations, formatting, and native action lookup.

In-game UI, normal save/load, and real multiplayer have not been verified for this release. Back up important saves. Inventory Mods patching the same operations may conflict.

## 配置、构建与验证

没有用户配置项。在仓库根目录运行：

```powershell
./stack-all-mod/build.ps1
./stack-all-mod/build.ps1 -GameDir 'D:\Games\Old Market Simulator'
```

构建只读引用本机程序集，运行检查并生成 `outputs/OldMarket.StackAll-0.3.0.zip`，不会安装。所需本地化源码、资源和测试均位于 `stack-all-mod/localization/`，因此本 Mod 目录可以独立构建。原有功能测试、IL 契约检查和 Mod 自带本地化检查均通过；尚未验证游戏内 UI、正常保存/读取和真实多人联机。首次使用前请备份重要存档。

## SDK and automated builds / SDK 与自动构建

This Mod pins its compilation SDK in [release.json](release.json). Current baseline: game 2.1.6 / SDK r11. Run `python tools/ci.py build` from the repository root for a game-free build. See [SDK maintenance](../sdk/README.md) and [CI releases](../releases/README.md). Compilation does not replace in-game compatibility checks.

本 Mod 在 [release.json](release.json) 固定编译 SDK，当前基线为游戏 2.1.6 / SDK r11。从仓库根目录运行 `python tools/ci.py build` 可无游戏文件构建。见 [SDK 维护](../sdk/README.md)和 [CI 发布](../releases/README.md)；编译不能代替实机兼容验证。

## Collectible pickup compatibility / 收集品重新拾取兼容性

Game 2.1.6 gives harvested resources a zero day counter, while Item.OnDayChanged increments that counter for ordinary world items as well as products. Requiring equality for every non-product therefore splits otherwise identical collectibles after time on the ground. StackCompatibility ignores that incidental counter for ordinary items, preserves strict equality for AnimalSO and the native fish-trap item, and retains product freshness checks. Drop/pickup RPCs and save fields are unchanged. Existing split stacks consolidate on a normal inventory reload or by dropping and repicking them, subject to capacity. No game, Unity or network API was added beyond types/members already declared in SDK r1; new read-only IL checks verify the native counter behavior. The regression first failed under the old policy and now covers repickup, repeated cycles, full hotbar capacity, overflow, weighted cost and meaningful state exclusions. In-game acceptance remains pending.

游戏 2.1.6 采集资源时传入天数 0，但 Item.OnDayChanged 会给普通地面物品和商品增加天数。原先对所有非商品都要求天数相等，导致同类收集品因在地面经过的时间不同而分堆。StackCompatibility 对普通物品忽略这一无实际状态含义的计数；AnimalSO 与原生鱼笼仍严格比较状态，商品保鲜规则不变。丢出/拾取 RPC 和存档字段不变，已有分堆可在正常重载背包或重新丢出拾取时按容量合并。使用的类型与成员均已被 SDK r1 覆盖；新增只读 IL 检查核对原生计数行为。回归测试在旧逻辑下先复现失败，再验证重复丢捡、满背包、溢出、成本和状态排除；仍待实机验收。

Validation for this collectible fix: 89,299 stack/container/input/display checks, 65 CI tooling tests, all SDK builds and 57 read-only game IL/patch contracts passed. The 13 r1 dependency hashes and Stack All SDK/real-reference symbolic IL and resources matched on game 2.1.6. No Unity runtime or multiplayer test was performed.

本次收集品修复通过 89,299 项堆叠/容器/输入/显示检查、65 项 CI 工具测试、全部 SDK 构建与 57 项只读游戏 IL/补丁契约检查；游戏 2.1.6 的 r1 依赖哈希及 Stack All SDK/真实引用符号 IL 和资源比对通过。未执行 Unity 实机与联机测试。


## Empty-container action / 空盒操作

G uses the existing RepeatGate: one immediate drop, 0.6-second hold delay, then one every 0.12 seconds. The batch tracks only empty reusable containers in the selected group, not total goods/containers. Removing a backing record preserves its item/cost/age metadata for the existing spawn RPC and leaves all filled records intact. Held input cannot resume after cancellation or silently extend to newly acquired empties. Native gameplay-action enablement, menus, focus, cursor lock, network state and competing inventory actions gate disposal. The action runs after native inventory LateUpdate, and its hint immediately reflects the remaining empties.

G 复用 RepeatGate：立即丢一个，长按 0.6 秒后每 0.12 秒一个，只计当前格空的可复用容器。移除空盒记录后使用现有生成 RPC，保留物品、成本与天数元数据，不改变有货容器。取消后须松开重按，新增空盒不能延长原批次。原生操作启用状态、菜单、焦点、鼠标锁定、网络状态及冲突库存操作均参与门控；操作在原生 LateUpdate 后执行，提示即时更新。

SDK r10 adds the required native input getters and explicitly records PlayerInventory.currentSlot/LateUpdate for Harmony injection. Only Stack All changes its pin. The game remains 2.1.6, Mod version remains 0.2.2, and existing save/RPC formats are unchanged. In-game UI, hold timing and host/client acceptance for G remain pending.

SDK r10 补充实际输入访问器及 Harmony 注入的 PlayerInventory.currentSlot/LateUpdate，仅变更 Stack All 固定修订；游戏 2.1.6、Mod 0.2.2 和存档/RPC 格式不变。G 的 UI、长按手感和房主/客人验收仍待完成。


The G hint clones an existing native one-key row inside controlHintPanel, keeping the native layout group, caption and keycap sizing. It recreates itself after native CheckHint rebuilds, refreshes fonts on language changes, and dims every Graphic when unavailable. Native Q/F rows are unchanged; their duplicate plain-text hold overlay is removed. SDK r10 adds the Color alpha field needed to preserve the original row transparency while dimming.

G 提示复制 controlHintPanel 内现有的单键原生行，保持布局组、说明文字及键帽尺寸。原生 CheckHint 重建后自动恢复，随语言更新字体；无空盒时所有 Graphic 一起变暗。原生 Q/F 行不变，移除重复的纯文字长按浮层。SDK r10 补充 Color 的透明度字段，用于变暗时保留原有透明度。

The hold caption clones a native row without its keycap and uses Transform.SetAsFirstSibling so the native layout places it above the actions. It shares the native font, font material and size and appears only while more than one unit remains. SDK r11 adds this verified Transform method; other pins are unchanged.

长按说明复制原生行并隐藏键帽，通过 Transform.SetAsFirstSibling 排在操作上方；字体、字体材质和字号跟随游戏，仅剩余多个物品时显示。SDK r11 增加经真实引用核对的 Transform 方法，其他 Mod 固定修订不变。

Current release target: **0.3.0**; game/SDK pin is unchanged. This version assignment does not alter runtime behavior or extend the recorded test scope.

当前发行目标：**0.3.0**，游戏／SDK 固定修订不变；版本指定不改变运行时行为，也不扩展已记录测试范围。
