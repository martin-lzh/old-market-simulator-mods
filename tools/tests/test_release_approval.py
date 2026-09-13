import importlib.util
import json
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest.mock import patch

spec = importlib.util.spec_from_file_location("approval_ci", Path(__file__).parents[1] / "ci.py")
ci = importlib.util.module_from_spec(spec)
spec.loader.exec_module(ci)


class ApprovalTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.root_patch = patch.object(ci, "ROOT", self.root)
        self.root_patch.start()
        self.git("init", "--quiet")
        self.c = dict(slug="coordinates", version="1.0.1", sdk="2.1.6/r1", prerelease=True,
                      directory=self.root / "coordinates-mod")
        self.write("coordinates-mod/CHANGELOG.md", "## English\n### Unreleased\n\n### 1.0.1 — release\nNotes\n## 中文\n### 未发布\n\n### 1.0.1 — release\n说明\n")
        for path in ("coordinates-mod/Plugin.cs", "sdk/2.1.6/r1/api.json", "tools/ci.py", ".github/workflows/mods.yml"):
            self.write(path, "original")
        self.source = self.commit()

    def tearDown(self):
        self.root_patch.stop()
        self.temp.cleanup()

    def git(self, *args):
        return subprocess.check_output(["git", *args], cwd=self.root, text=True, stderr=subprocess.PIPE).strip()

    def write(self, path, text):
        p = self.root / path
        p.parent.mkdir(parents=True, exist_ok=True)
        p.write_text(text, encoding="utf-8")

    def commit(self):
        self.git("add", ".")
        self.git("-c", "user.name=Test", "-c", "user.email=test@example.invalid", "commit", "--quiet", "-m", "Test")
        return self.git("rev-parse", "HEAD")

    def approve(self):
        with patch.object(ci, "config", return_value=self.c):
            ci.record_approval("coordinates", "Test fixture: explicitly release Coordinates 1.0.1")
        return self.commit()

    def test_no_record_is_not_authorized(self):
        self.assertFalse(ci.release_authorized(self.c, self.source))

    def test_record_commit_does_not_invalidate_source_binding(self):
        approved = self.approve()
        self.assertNotEqual(approved, self.source)
        self.assertTrue(ci.release_authorized(self.c, approved))

    def test_mod_change_invalidates_authorization(self):
        self.approve()
        self.write("coordinates-mod/Plugin.cs", "new behavior")
        with self.assertRaisesRegex(ValueError, "inputs changed"):
            ci.release_authorized(self.c, self.commit())

    def test_sdk_change_invalidates_authorization(self):
        self.approve()
        self.write("sdk/2.1.6/r1/api.json", "changed")
        with self.assertRaisesRegex(ValueError, "inputs changed"):
            ci.release_authorized(self.c, self.commit())

    def test_build_tool_change_invalidates_authorization(self):
        self.approve()
        self.write("tools/ci.py", "changed")
        with self.assertRaisesRegex(ValueError, "inputs changed"):
            ci.release_authorized(self.c, self.commit())

    def test_other_mod_does_not_invalidate_authorization(self):
        self.approve()
        self.write("navigation-mod/Plugin.cs", "other mod")
        self.assertTrue(ci.release_authorized(self.c, self.commit()))

    def test_unreleased_blocks_recording(self):
        p = self.c["directory"] / "CHANGELOG.md"
        p.write_text(p.read_text(encoding="utf-8").replace("### 未发布\n", "### 未发布\n- 待发布修复\n"), encoding="utf-8")
        self.commit()
        with patch.object(ci, "config", return_value=self.c), self.assertRaisesRegex(ValueError, "numbered release"):
            ci.record_approval("coordinates", "Explicit test authorization")

    def test_record_cannot_bypass_pending_content(self):
        self.approve()
        p = self.c["directory"] / "CHANGELOG.md"
        p.write_text(p.read_text(encoding="utf-8").replace("### Unreleased\n", "### Unreleased\n- Pending fix\n"), encoding="utf-8")
        with self.assertRaisesRegex(ValueError, "Unreleased"):
            ci.release_authorized(self.c, self.commit())

    def test_changed_version_is_not_covered(self):
        self.approve()
        changed = dict(self.c, version="2.0.0")
        self.assertFalse(ci.release_authorized(changed, self.git("rev-parse", "HEAD")))

    def test_record_refuses_dirty_source(self):
        self.write("coordinates-mod/Plugin.cs", "dirty")
        with self.assertRaisesRegex(ValueError, "Commit source"):
            ci.record_approval("coordinates", "Explicit test authorization")

    def test_record_requires_nonempty_authorization(self):
        with self.assertRaises(ValueError):
            ci.record_approval("coordinates", " ")
