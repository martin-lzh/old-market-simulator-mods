import importlib.util
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch
import zipfile

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

    def test_navigation_managed_directory_respects_override(self):
        node = ci.ET.parse(ci.ROOT / "navigation-mod/Navigation.csproj").find(".//ManagedDir")
        self.assertEqual(node.attrib.get("Condition"), "'$(ManagedDir)' == ''")

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


class FakeGitHub:
    def __init__(self, existing=None, corrupt=False):
        self.existing = existing or []
        self.calls = []
        self.assets = {}
        self.corrupt = corrupt
        self.draft = None

    def releases(self):
        return self.existing

    def request(self, path, method="GET", payload=None, raw=False):
        self.calls.append((path, method))
        if path.startswith("/git/ref/"):
            return None
        if path == "/releases" and method == "POST":
            self.draft = dict(payload, id=1, assets=[], upload_url="https://uploads.github.com/fake{?name}")
            return self.draft
        if path.startswith("https://uploads"):
            number = len(self.assets) + 1
            self.assets[number] = payload
            return {"id": number}
        if raw:
            return b"wrong" if self.corrupt else self.assets[int(path.split('/')[-1])]
        return {}


class PublishTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.c = dict(slug="coordinates", name="Coordinates", version="1.0.0", sdk="2.1.6/r1",
                      prerelease=True, notes="## English\nRelease\n## 中文\n发布\n", directory=self.root / "coordinates-mod",
                      manifest=dict(gameVersion="2.1.6", gameAssemblySha256="game", apiSha256="api"))
        self.c["directory"].mkdir()
        for name in ("README.md", "CHANGELOG.md", "LICENSE"):
            (self.c["directory"] / name).write_text(name)
        self.output = self.root / "outputs/ci/coordinates"
        self.output.mkdir(parents=True)
        self.package = self.output / ci.archive_name(self.c, "BepInEx")
        self.zip()
        self.patches = [patch.object(ci, "ROOT", self.root), patch.object(ci, "MODS", {"coordinates": "Coordinates"}),
                        patch.object(ci, "config", return_value=self.c), patch.object(ci.subprocess, "check_output", return_value="a" * 40)]
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
                z.writestr(name, name)
            if extra:
                z.writestr(extra, "unexpected")

    def test_existing_public_release_is_never_modified(self):
        api = FakeGitHub([dict(tag_name="coordinates-v1.0.0", draft=False)])
        ci.publish("owner/repo", "a" * 40, api)
        self.assertEqual(api.calls, [])

    def test_upload_verify_then_publish(self):
        api = FakeGitHub()
        ci.publish("owner/repo", "a" * 40, api)
        self.assertEqual(len(api.assets), 3)
        self.assertEqual(api.calls[-1], ("/releases/1", "PATCH"))
        self.assertEqual(api.draft["target_commitish"], "a" * 40)
        self.assertTrue(api.draft["prerelease"])

    def test_remote_corruption_leaves_draft(self):
        api = FakeGitHub(corrupt=True)
        with self.assertRaises(ValueError):
            ci.publish("owner/repo", "a" * 40, api)
        self.assertNotIn(("/releases/1", "PATCH"), api.calls)

    def test_rejects_sdk_in_package(self):
        self.zip("Assembly-CSharp.dll")
        ci.write_evidence(self.c, self.output)
        api = FakeGitHub()
        with self.assertRaises(ValueError):
            ci.publish("owner/repo", "a" * 40, api)
        self.assertEqual(api.calls, [])

    def test_bad_checksum_fails_before_network_write(self):
        (self.output / "SHA256SUMS.txt").write_text("wrong")
        api = FakeGitHub()
        with self.assertRaises(ValueError):
            ci.publish("owner/repo", "a" * 40, api)
        self.assertEqual(api.calls, [])

    def test_other_commit_draft_is_not_reused(self):
        api = FakeGitHub([dict(tag_name="coordinates-v1.0.0", draft=True, target_commitish="b" * 40)])
        with self.assertRaises(ValueError):
            ci.publish("owner/repo", "a" * 40, api)
        self.assertEqual(api.calls, [])

    def test_refuses_backwards_new_release(self):
        api = FakeGitHub([dict(tag_name="coordinates-v2.0.0", draft=False)])
        with self.assertRaises(ValueError):
            ci.publish("owner/repo", "a" * 40, api)
        self.assertEqual(api.calls, [])

    def test_mismatched_source_fails(self):
        with self.assertRaises(ValueError):
            ci.publish("owner/repo", "b" * 40, FakeGitHub())


if __name__ == "__main__":
    unittest.main()
