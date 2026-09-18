import importlib.util
from pathlib import Path
import subprocess
import tempfile
import unittest

spec = importlib.util.spec_from_file_location("check_syntax", Path(__file__).parents[1] / "check_syntax.py")
syntax = importlib.util.module_from_spec(spec)
spec.loader.exec_module(syntax)


class SyntaxTests(unittest.TestCase):
    def test_parsers_reject_invalid_source(self):
        for suffix, content in [(".py", "def broken(:"), (".json", '{"a":}'),
                                (".json", '{"a": NaN}'), (".csproj", "<Project>")]:
            with self.subTest(suffix=suffix, content=content), tempfile.TemporaryDirectory() as temp:
                path = Path(temp) / f"bad{suffix}"
                path.write_text(content, encoding="utf-8")
                with self.assertRaises((SyntaxError, ValueError, syntax.ET.ParseError)):
                    syntax.check_file(path)

    def test_valid_files_and_python_is_not_executed(self):
        for suffix, content in [(".py", 'raise RuntimeError("must not execute")'),
                                (".json", '{"名称": true}'), (".props", "<Project />")]:
            with self.subTest(suffix=suffix), tempfile.TemporaryDirectory() as temp:
                path = Path(temp) / f"valid{suffix}"
                path.write_text(content, encoding="utf-8")
                self.assertTrue(syntax.check_file(path))
                self.assertFalse((Path(temp) / "__pycache__").exists())

    def test_git_selection_handles_spaces_unicode_and_ignored_files(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            subprocess.run(["git", "init", "--quiet", temp], check=True)
            (root / ".gitignore").write_text("work/\n", encoding="utf-8")
            (root / "名称 with spaces.py").write_text("pass\n", encoding="utf-8")
            (root / "work").mkdir()
            (root / "work/bad.py").write_text("broken(", encoding="utf-8")
            subprocess.run(["git", "add", "."], cwd=root, check=True)
            self.assertEqual(set(syntax.tracked_files(root)),
                             {Path(".gitignore"), Path("名称 with spaces.py")})

    def test_powershell_parser_rejects_errors_without_executing_scripts(self):
        with tempfile.TemporaryDirectory() as temp:
            path = Path(temp) / "名称 with spaces.ps1"
            path.write_text("throw 'must not execute'\n", encoding="utf-8-sig")
            self.assertEqual(syntax.check_powershell([path]), 0)
            path.write_text("function Broken {\n", encoding="utf-8-sig")
            self.assertNotEqual(syntax.check_powershell([path]), 0)


if __name__ == "__main__":
    unittest.main()
