import importlib.util
import json
from pathlib import Path, PurePosixPath
import subprocess
import sys
import tempfile
import unittest
from unittest.mock import patch
import zipfile

sys.path.insert(0, str(Path(__file__).parents[1]))
spec = importlib.util.spec_from_file_location("ci", Path(__file__).parents[1] / "ci.py")
ci = importlib.util.module_from_spec(spec)
spec.loader.exec_module(ci)


def log(en="1.2.3", zh="1.2.3"):
    return f"## English\n### Unreleased\nPending\n### {en} — 2026-09-13\nEnglish notes\n## 中文\n### 未发布\n待发布\n### {zh} — 2026-09-13\n中文记录\n"


class VersionTests(unittest.TestCase):
    def test_bilingual_notes_ignore_unreleased(self):
        version, notes = ci.changelog(log())
        self.assertEqual(version, "1.2.3")
        self.assertNotIn("Pending", notes)
        self.assertIn("中文记录", notes)

    def test_disagreement_fails(self):
        with self.assertRaises(ValueError):
            ci.changelog(log(zh="1.2.2"))

    def test_duplicate_history_fails(self):
        with self.assertRaises(ValueError):
            ci.changelog(log().replace("## 中文", "### 1.2.3 — duplicate\n\n## 中文"))

    def test_missing_language_fails(self):
        with self.assertRaises(ValueError):
            ci.changelog("## English\n### 1.0.0 — today\n")

    def test_numeric_order(self):
        text = log("1.10.0", "1.10.0").replace("## 中文", "### 1.9.0 — old\n\n## 中文") + "### 1.9.0 — old\n"
        self.assertEqual(ci.changelog(text)[0], "1.10.0")

    def test_repository_configs(self):
        for slug in ci.MODS:
            self.assertTrue((ci.ROOT / "sdk" / ci.config(slug)["sdk"] / "manifest.json").is_file())

    def test_navigation_in_all_ci_scopes(self):
        self.assertEqual(ci.MODS["navigation"], "Navigation")
        self.assertEqual(ci.variants("navigation"), ["BepInEx"])
        self.assertTrue((ci.ROOT / "navigation-mod/tests/Navigation.Tests.csproj").is_file())

    def test_tree_info_build_and_reflection_contracts(self):
        c = ci.config("tree-info")
        self.assertEqual(ci.variants("tree-info"), ["BepInEx"])
        self.assertEqual(c["sdk"], "2.1.6/r8")
        self.assertTrue((c["directory"] / "tests/Tests.csproj").is_file())
        self.assertTrue((c["directory"] / "tests/ContractChecks.csproj").is_file())
        self.assertEqual(ci.ET.parse(c["project"]).find(".//ManagedDir").attrib["Condition"], "'$(ManagedDir)' == ''")
        api = ci.json.loads((ci.ROOT / "sdk" / c["sdk"] / "api.json").read_text())
        types = {t["Name"]: t for t in api["Types"]}
        for name, argument in (("dayCounter", "System.Int32"), ("isWatered", "System.Boolean")):
            field = next(f for f in types["BlockTree"]["Fields"] if f["Name"] == name)
            self.assertEqual(field["Type"]["Element"]["Name"], "Unity.Netcode.NetworkVariable`1")
            self.assertEqual(field["Type"]["Arguments"][0]["Name"], argument)
        self.assertIn("InteractionRay", {m["Name"] for m in types["PlayerInteraction"]["Methods"]})
        self.assertTrue({"mainCamera", "isInteractionEnabled"} <= {f["Name"] for f in types["PlayerInteraction"]["Fields"]})

    def test_navigation_managed_directory_respects_override(self):
        node = ci.ET.parse(ci.ROOT / "navigation-mod/Navigation.csproj").find(".//ManagedDir")
        self.assertEqual(node.attrib.get("Condition"), "'$(ManagedDir)' == ''")

    def test_stack_empty_action_exports_harmony_injection(self):
        c = ci.config("stack-all")
        api = ci.json.loads((ci.ROOT / "sdk" / c["sdk"] / "api.json").read_text())
        inventory = next(t for t in api["Types"] if t["Name"] == "PlayerInventory")
        current = next(f for f in inventory["Fields"] if f["Name"] == "currentSlot")
        self.assertEqual(current["Type"]["Element"]["Name"], "Unity.Netcode.NetworkVariable`1")
        self.assertEqual(current["Type"]["Arguments"][0]["Name"], "System.Int32")
        self.assertIn("LateUpdate", {m["Name"] for m in inventory["Methods"]})

    def test_navigation_reflection_declarations_are_exported(self):
        api = ci.json.loads((ci.ROOT / "sdk/2.1.6/r2/api.json").read_text(encoding="utf-8"))
        types = {t["Name"]: t for t in api["Types"]}
        slots = {f["Name"]: f for f in types["SaveManager"]["Fields"]}
        self.assertEqual(slots["currentSlot"]["Type"]["Name"], "System.String")
        expansions = {f["Name"]: f for f in types["GameManager"]["Fields"]}
        active = expansions["activeExpansions"]["Type"]
        self.assertEqual(active["Element"]["Name"], "Unity.Netcode.NetworkList`1")
        self.assertEqual(active["Arguments"][0]["Name"], "System.Int64")
        self.assertIn("GetLocalCurrentRegionSceneName", {m["Name"] for m in types["RegionManager"]["Methods"]})

    def test_navigation_inlined_constants_match_reviewed_metadata(self):
        api = ci.json.loads((ci.ROOT / "sdk/2.1.6/r2/api.json").read_text(encoding="utf-8"))
        types = {t["Name"]: t for t in api["Types"]}
        maths = {f["Name"]: f for f in types["UnityEngine.Mathf"]["Fields"]}
        self.assertEqual(maths["Deg2Rad"]["ConstantType"], "Single")
        self.assertAlmostEqual(float(maths["Deg2Rad"]["Constant"]), 0.017453292, places=9)
        self.assertAlmostEqual(float(maths["Rad2Deg"]["Constant"]), 57.29578, places=5)
        physics = {f["Name"]: f for f in types["UnityEngine.Physics"]["Fields"]}
        self.assertEqual(physics["DefaultRaycastLayers"]["Constant"], "-5")

    def test_existing_sdk_cannot_be_edited(self):
        with patch.object(ci.subprocess, "check_output", return_value="sdk/2.1.6/r1/api.json\n"), \
                patch.object(ci.subprocess, "run") as git:
            git.return_value.returncode = 0
            with self.assertRaises(ValueError):
                ci.immutable_sdk("a" * 40)

    def test_new_sdk_revision_is_allowed(self):
        with patch.object(ci.subprocess, "check_output", return_value="sdk/2.1.6/r2/api.json\n"), \
                patch.object(ci.subprocess, "run") as git:
            git.return_value.returncode = 1
            ci.immutable_sdk("a" * 40)

    def test_export_refuses_dirty_source(self):
        with patch.object(ci.subprocess, "check_output", return_value=" M Plugin.cs\n"):
            with self.assertRaisesRegex(ValueError, "Commit source"):
                ci.export_sdk("fake-game", "2.1.6", 2, "fake-seeds")

    def test_export_refuses_existing_revision(self):
        with patch.object(ci.subprocess, "check_output", return_value=""):
            with self.assertRaisesRegex(ValueError, "already exists"):
                ci.export_sdk("fake-game", "2.1.6", 1, "fake-seeds")


class ReleaseAdapterTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.c = dict(slug="coordinates", name="Coordinates", displayName="Coordinates HUD", version="1.0.0",
                      sdk="2.1.6/r1", prerelease=True, notes="## English\n\n### 1.0.0 — 2026-10-18\nRelease\n",
                      directory=self.root / "coordinates-mod",
                      manifest=dict(gameVersion="2.1.6", gameAssemblySha256="game", apiSha256="api"))
        self.c["directory"].mkdir()
        for name in ("README.md", "LICENSE"):
            (self.c["directory"] / name).write_text(name)
        (self.c["directory"] / "CHANGELOG.md").write_text("## English\n### Unreleased\n\n### 1.0.0 — 2026-10-18\nRelease\n", encoding="utf-8")
        self.output = self.root / "outputs/ci/coordinates"
        self.output.mkdir(parents=True)
        self.package = self.output / ci.archive_name(self.c, "BepInEx")
        self.zip()
        self.patches = [patch.object(ci, "ROOT", self.root), patch.object(ci, "MODS", {"coordinates": "Coordinates"}),
                        patch.object(ci, "config", return_value=self.c),
                        patch.object(ci.subprocess, "check_output", return_value="a" * 40)]
        for p in self.patches:
            p.start()
        ci.write_evidence(self.c, self.output)

    def tearDown(self):
        for p in reversed(self.patches):
            p.stop()
        self.temp.cleanup()

    def zip(self, extra=None):
        with zipfile.ZipFile(self.package, "w") as z:
            z.writestr(ci.plugin_path(self.c, "BepInEx"), b"MZ-test-plugin")
            for name in ("README.md", "CHANGELOG.md", "LICENSE"):
                z.writestr(name, (self.c["directory"] / name).read_bytes())
            if extra:
                z.writestr(extra, "unexpected")

    def test_release_mods_describe_display_name_assets_and_state(self):
        mod = ci.release_mods("owner/repo")[0]
        self.assertEqual((mod.display_name, mod.version, mod.prerelease, mod.unreleased),
                         ("Coordinates HUD", "1.0.0", True, False))
        self.assertEqual([p.name for p in mod.assets], [self.package.name, "build-info.json"])
        self.assertIn("Old Market Simulator 2.1.6", mod.summary)

    def test_unreleased_entries_are_detected(self):
        (self.c["directory"] / "CHANGELOG.md").write_text("## English\n### Unreleased\n- Pending\n### 1.0.0 — x\n", encoding="utf-8")
        self.assertTrue(ci.release_mods("owner/repo")[0].unreleased)

    def test_build_evidence_has_no_validation_disclaimer(self):
        evidence = json.loads((self.output / "build-info.json").read_text(encoding="utf-8"))
        self.assertNotIn("validation", evidence)
        self.assertEqual(evidence["source"], "a" * 40)

    def test_verified_artifacts_pass(self):
        ci.verify_artifacts(self.c, "a" * 40)

    def test_sdk_in_package_fails_before_upload(self):
        self.zip("Assembly-CSharp.dll")
        with self.assertRaises(ValueError):
            ci.verify_artifacts(self.c, "a" * 40)

    def test_evidence_from_other_commit_fails(self):
        with self.assertRaises(ValueError):
            ci.verify_artifacts(self.c, "b" * 40)

    def test_publish_refuses_other_checkout(self):
        with self.assertRaises(ValueError):
            ci.publish("owner/repo", "b" * 40, api=object())


class ChangeTests(unittest.TestCase):
    """CI skips build and test only for files that never reach a package, validation or tests."""

    def test_examples(self):
        core = ["navigation-mod/Navigation.csproj", "navigation-mod/Plugin.cs", "navigation-mod/README.md",
                "navigation-mod/CHANGELOG.md", "navigation-mod/LICENSE", "navigation-mod/release.json",
                "navigation-mod/map-pack.json", "navigation-mod/maps/town.png", "navigation-mod/assets/parchment.png",
                "navigation-mod/assets/cover.png", "navigation-mod/assets/README.md", "navigation-mod/tests/Program.cs",
                "material-cost-mod/localization/Catalog.cs", "sdk/2.1.6/r1/api.json", "sdk/README.md",
                "tools/ci.py", "tools/Sdk/Sdk.csproj", "tools/tests/test_ci.py", ".github/workflows/mods.yml",
                ".github/ISSUE_TEMPLATE/bug.yml", "Directory.Build.props", ".gitattributes", ".gitignore", "LICENSE",
                "docs.cs", "new-mod/New.csproj"]
        non_core = ["README.md", "AGENTS.md", "DEVELOPMENT.md", "CODE_OF_CONDUCT.md", "CONTRIBUTING.md",
                    "SECURITY.md", "SUPPORT.md", "docs/game-build-monitor.md", "docs/nexus/covers/prompts.json",
                    "assets/cover.png", "releases/validation.md", "releases/approvals/navigation-v0.2.1.json",
                    "navigation-mod/DEVELOPMENT.md"]
        for path in core:
            with self.subTest(core=path):
                self.assertTrue(ci.changes.is_core(path, ci.NON_CORE))
        for path in non_core:
            with self.subTest(non_core=path):
                self.assertFalse(ci.changes.is_core(path, ci.NON_CORE))

    def test_mod_and_build_files_are_core(self):
        root = Path(__file__).resolve().parents[2]
        names = subprocess.check_output(["git", "ls-files", "-z"], cwd=root).decode("utf-8").split("\0")
        for name in filter(None, names):
            path = PurePosixPath(name)
            mod_file = path.parts[0].endswith("-mod") and path.name != "DEVELOPMENT.md"
            if mod_file or path.parts[0] in {"sdk", "tools", ".github"} or path.suffix in {".cs", ".csproj", ".props"}:
                with self.subTest(name=name):
                    self.assertTrue(ci.changes.is_core(name, ci.NON_CORE))


if __name__ == "__main__":
    unittest.main()
