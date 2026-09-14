import io
import json
from pathlib import Path
import shutil
import tempfile
import unittest
from unittest.mock import patch
import zipfile

from test_ci import ci, FakeGitHub


class MapPackageTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.c = ci.config("navigation")
        original = self.c["directory"]
        self.c["directory"] = self.root / "navigation-mod"
        self.c["directory"].mkdir()
        for name in ("README.md", "CHANGELOG.md", "LICENSE", "map-pack.json"):
            shutil.copyfile(original / name, self.c["directory"] / name)
        shutil.copytree(original / "maps", self.c["directory"] / "maps")
        self.dll = self.root / "OldMarket.Navigation.dll"
        self.dll.write_bytes(b"MZ-test-plugin")
        self.output = self.root / "outputs/ci/navigation"
        self.package = self.output / ci.archive_name(self.c, "BepInEx")
        self.manifest = self.c["directory"] / "map-pack.json"

    def build(self):
        ci.write_package(self.c, "BepInEx", self.dll, self.package)
        return self.package.read_bytes()

    def update_manifest(self, change):
        manifest = json.loads(self.manifest.read_text())
        change(manifest)
        self.manifest.write_text(json.dumps(manifest))

    def update_map(self, change):
        path = self.c["directory"] / "maps/market0.json"
        data = json.loads(path.read_text())
        change(data)
        path.write_text(json.dumps(data))
        self.update_manifest(lambda m: next(e for e in m["files"] if e["name"] == path.name)
                             .update(sha256=ci.sha(path.read_bytes())))

    def test_package_contains_all_reviewed_maps_at_install_path(self):
        with zipfile.ZipFile(io.BytesIO(self.build())) as z:
            count = len(json.loads(self.manifest.read_text())["files"])
            self.assertEqual(len(z.namelist()), 4 + count)
            self.assertEqual(sum("/maps/" in n for n in z.namelist()), count)
            self.assertIsNone(z.testzip())
            for name, data in ci.map_resources(self.c).items():
                self.assertEqual(z.read(name), data)

    def test_missing_map_fails_build(self):
        (self.c["directory"] / "maps/market3-obstacles.png").unlink()
        with self.assertRaises(FileNotFoundError):
            self.build()

    def test_changed_source_map_fails_hash(self):
        (self.c["directory"] / "maps/market0.json").write_text("{}")
        with self.assertRaisesRegex(ValueError, "hash mismatch"):
            self.build()

    def test_wrong_game_baseline_fails(self):
        self.update_manifest(lambda m: m.update(gameVersion="9.9.9"))
        with self.assertRaisesRegex(ValueError, "baseline"):
            self.build()

    def test_duplicate_source_name_fails(self):
        self.update_manifest(lambda m: m["files"].append(m["files"][0]))
        with self.assertRaisesRegex(ValueError, "Duplicate"):
            self.build()

    def test_unsafe_or_non_map_source_name_fails(self):
        for name in ("../config.json", "C:/save.json", "Assembly-CSharp.dll"):
            with self.subTest(name=name):
                self.update_manifest(lambda m: m["files"][0].update(name=name))
                with self.assertRaisesRegex(ValueError, "file entry"):
                    self.build()

    def test_texture_reference_must_be_packaged(self):
        self.update_map(lambda m: m.update(Texture="unlisted.png"))
        with self.assertRaisesRegex(ValueError, "texture not in package"):
            self.build()

    def test_invalid_bounds_fail(self):
        self.update_map(lambda m: m.update(MaxX=m["MinX"]))
        with self.assertRaisesRegex(ValueError, "metadata"):
            self.build()

    def test_png_corruption_fails_even_with_updated_hash(self):
        path = self.c["directory"] / "maps/market0-obstacles.png"
        data = bytearray(path.read_bytes())
        data[50] ^= 1
        path.write_bytes(data)
        self.update_manifest(lambda m: next(e for e in m["files"] if e["name"] == path.name)
                             .update(sha256=ci.sha(data)))
        with self.assertRaisesRegex(ValueError, "CRC"):
            self.build()

    def test_unlisted_local_files_never_enter_archive(self):
        (self.c["directory"] / "maps/private-config.json").write_text("private")
        (self.c["directory"] / "maps/Assembly-CSharp.dll").write_bytes(b"MZ")
        with zipfile.ZipFile(io.BytesIO(self.build())) as z:
            self.assertEqual(len(z.namelist()), 4 + len(json.loads(self.manifest.read_text())["files"]))

    def test_island_identity_pois_and_texture_are_packaged(self):
        with zipfile.ZipFile(io.BytesIO(self.build())) as z:
            prefix = "BepInEx/plugins/OldMarket.Navigation/maps/"
            island = json.loads(z.read(prefix + "island.json"))
            self.assertEqual((island["MapId"], island["SceneName"]), (0, "BazaarIsland"))
            self.assertEqual(len(island["Pois"]), 16)
            self.assertEqual(len({p["Id"] for p in island["Pois"]}), 16)
            self.assertIn(prefix + island["Texture"], z.namelist())
            points = {p["Id"]: p for p in island["Pois"]}
            self.assertEqual(points["island-junkman"]["Category"], "other")
            self.assertAlmostEqual(points["island-employees"]["X"], 33.278999, places=5)
            self.assertAlmostEqual(points["island-orders"]["Z"], 50.721001, places=5)
            for point in island["Pois"]:
                self.assertTrue(island["MinX"] <= point["X"] <= island["MaxX"])
                self.assertTrue(island["MinZ"] <= point["Z"] <= island["MaxZ"])

    def test_package_rejects_missing_altered_moved_or_extra_resources(self):
        with zipfile.ZipFile(io.BytesIO(self.build())) as z:
            original = {name: z.read(name) for name in z.namelist()}
        name = "BepInEx/plugins/OldMarket.Navigation/maps/market0.json"
        for mutation in ("missing", "altered", "moved", "sdk", "duplicate"):
            with self.subTest(mutation=mutation):
                entries = dict(original)
                if mutation == "missing": entries.pop(name)
                if mutation == "altered": entries[name] = b"{}"
                if mutation == "moved": entries["maps/market0.json"] = entries.pop(name)
                if mutation == "sdk": entries["Assembly-CSharp.dll"] = b"MZ"
                buffer = io.BytesIO()
                with zipfile.ZipFile(buffer, "w") as z:
                    for n, data in entries.items(): z.writestr(n, data)
                    if mutation == "duplicate":
                        with self.assertWarns(UserWarning): z.writestr(name, entries[name])
                with self.assertRaises(ValueError):
                    ci.validate_package(self.c, "BepInEx", buffer.getvalue())

    def test_release_upload_contains_maps_and_evidence(self):
        data = self.build()
        with patch.object(ci, "ROOT", self.root), patch.object(ci, "MODS", {"navigation": "Navigation"}), \
                patch.object(ci, "config", return_value=self.c), \
                patch.object(ci.subprocess, "check_output", return_value="a" * 40), \
                patch.object(ci, "release_authorized", return_value=True):
            ci.write_evidence(self.c, self.output)
            evidence = json.loads((self.output / "build-info.json").read_text(encoding="utf-8"))
            self.assertEqual(len(evidence["mapFiles"]), len(json.loads(self.manifest.read_text())["files"]))
            api = FakeGitHub()
            ci.publish("owner/repo", "a" * 40, api)
            self.assertIn(data, api.assets.values())
            self.assertEqual(api.calls[-1], ("/releases/1", "PATCH"))


if __name__ == "__main__":
    unittest.main()
