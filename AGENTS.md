# Agent instructions

Source, build tooling and release automation for the Old Market Simulator Mods (Unity, BepInEx 5; Profit Insights also ships a MelonLoader 0.7.3 edition). The sibling repositories `parisian-bistro-simulator-mods` and `farming-simulator-25-mods` follow the same rules; when you change a rule here, change it there too.

## Read first

- `README.md` for what players see, `DEVELOPMENT.md` for builds, tests and the game baseline, `releases/README.md` for how releases work, and `sdk/README.md` for API snapshots.
- The target Mod's `README.md`, `CHANGELOG.md`, `DEVELOPMENT.md` and `release.json`.
- `CONTRIBUTING.md` describes the fork workflow for outside contributors. Agents follow this file instead.

## Hard rules

1. **The game install is read-only.** Builds may read game assemblies, but never install Mods, edit saves, or start or stop the game. The maintainer installs builds and does the in-game testing.
2. **No game content in git.** Original DLLs/EXEs, assets, translation tables, decompiled code, saves, logs and credentials stay in the ignored `work/` folder; never `git add -f`. Packages contain only original files, chosen by explicit allowlists in `tools/ci.py`.
3. **Check game APIs against the real game.** Never invent signatures, add empty stubs or bypass a check to make code compile. SDK snapshots in `sdk/<game version>/r<n>/` never change once merged; add a new revision instead (see below).
4. **Versions change only when the maintainer says so.** Until then, record every change under `Unreleased` / `未发布` in that Mod's `CHANGELOG.md` and keep all version numbers. Identify test builds by commit and SHA256, not by a made-up version.
5. **Merging into `main` publishes.** Every new numbered CHANGELOG version that reaches `main` is released automatically, so only the maintainer merges. Open a PR to `main` only when the maintainer asks, and never merge it.
6. **Commit and push to `dev` at the end of every task.** Stage only files that belong to the task and leave other work untouched. Don't create git worktrees: the ignored `work/` references would be missing from them.
7. **Promotional material stays local.** Posters, generated art, copy and exports for Xiaohongshu, Steam or Nexus go in the ignored `outputs/` folder. Tracked Mod assets and README screenshots are not affected.

## Player-facing text

READMEs, CHANGELOG entries and release notes say what the Mod does and what the player should do. Leave out disclaimers ("not verified", "does not imply") and developer terms (RPC, Harmony, IL, SDK revisions, hashes, acceptance status); those belong in `DEVELOPMENT.md` and `releases/validation.md`. Bilingual documents put English first, then 中文, each complete. Links in packaged docs that point outside the Mod's ZIP use full `https://github.com/.../blob/main/...` URLs.

## Releasing a Mod

Do this only when the maintainer asks for a specific Mod and version:

1. The maintainer confirms in-game testing of the build being released. This applies to betas too.
2. Update the version in `<Mod>.csproj`, the plugin attribute (`BepInPlugin`, and `MelonInfo` for Profit Insights), the Mod README download link and the root README Mod table.
3. Move the `Unreleased` entries into `### X.Y.Z — YYYY-MM-DD` in both CHANGELOG languages, written for players. Leave the heading at version and date.
4. Set `"prerelease": true` in `release.json` for a closed beta for a few testers, or `false` for a stable release. To turn an existing beta into a stable release later, set it to `false` without changing the version.
5. Run the validation commands, commit, push `dev` and, when asked, open the PR. Fill in the PR template's version table.
6. Before the maintainer merges, tell them what the CI "Release plan" table says: which Mods will publish or be promoted, and which are skipped and why.

## Validation

```powershell
python tools/ci.py validate
python -m unittest discover -s tools/tests -v
python tools/ci.py build
python tools/ci.py verify-game --game-dir '<game-installation-path>'   # needs a matching local install
```

Report which of these ran and which didn't (for example, no game install). Passing builds and tests are not in-game testing.

## SDK snapshots

- Each Mod pins an SDK in `release.json`. When a Mod needs a game type or member the pinned SDK lacks, check it against the real game assemblies, commit, then export a new revision with `python tools/ci.py export-sdk` (the worktree must be clean). Follow `sdk/README.md`, including `supplemental.json` for members used only through `nameof`, reflection or Harmony strings.
- Move only the Mods that need the new revision; never repin everything to the latest.
- After a game update, unchanged signatures can still hide changed behavior: review save data, multiplayer messages, Harmony targets and IL assumptions.
- Generated reference DLLs stay in ignored build folders and are never installed or packaged.
