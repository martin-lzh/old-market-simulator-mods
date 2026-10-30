"""Repository boundaries preserve runtime maps and SDK facts, not local game data."""

import contextlib
import importlib.util
import io
from pathlib import Path
import subprocess
import unittest
from unittest.mock import patch


spec = importlib.util.spec_from_file_location(
    "check_repository", Path(__file__).parents[1] / "check_repository.py"
)
checker = importlib.util.module_from_spec(spec)
spec.loader.exec_module(checker)


class RepositoryBoundaryTests(unittest.TestCase):
    def check_paths(self, paths):
        result = subprocess.CompletedProcess([], 0, "\0".join(paths).encode() + b"\0")
        with patch.object(checker.subprocess, "run", return_value=result):
            with contextlib.redirect_stdout(io.StringIO()), contextlib.redirect_stderr(io.StringIO()):
                return checker.main()

    def test_allows_original_runtime_maps_and_sdk_facts(self):
        self.assertEqual(0, self.check_paths([
            "navigation-mod/maps/island.png", "sdk/snapshots/r11/schema.json",
            "tools/check_repository.py", "material-cost-mod/src/Plugin.cs",
        ]))

    def test_rejects_game_binaries_local_references_and_secrets(self):
        for path in [
            "sdk/Assembly-CSharp.dll", "data/sharedassets0.assets", "data/level.bundle",
            "data/level.unity3d", "data/sharedassets0.resS", "work/report.json",
            "mod/outputs/release.zip", "references/game.cs", "decompiled/Game.cs",
            ".env", "settings.local.json", "Logs/startup.log", "save/slot1.sav",
        ]:
            with self.subTest(path=path):
                self.assertEqual(1, self.check_paths([path]))


if __name__ == "__main__":
    unittest.main()
