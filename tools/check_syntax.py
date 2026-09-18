"""Parse tracked source/configuration files without executing project scripts."""
from __future__ import annotations

import json
from pathlib import Path
import subprocess
import sys
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
XML_SUFFIXES = {".xml", ".csproj", ".props", ".targets", ".resx", ".config"}
PS_SUFFIXES = {".ps1", ".psm1", ".psd1"}
PS_PARSER = r"""
$ErrorActionPreference = 'Stop'
$paths = ConvertFrom-Json ([Console]::In.ReadToEnd())
$failed = $false
foreach ($path in $paths) {
    $tokens = $null
    $parseErrors = $null
    $null = [System.Management.Automation.Language.Parser]::ParseFile(
        $path, [ref]$tokens, [ref]$parseErrors)
    foreach ($parseError in $parseErrors) {
        [Console]::Error.WriteLine(('{0}:{1}:{2}: {3}' -f $path,
            $parseError.Extent.StartLineNumber, $parseError.Extent.StartColumnNumber,
            $parseError.Message))
        $failed = $true
    }
}
if ($failed) { exit 1 }
"""


def tracked_files(root):
    """Use NUL delimiters so spaces and Unicode filenames remain intact."""
    output = subprocess.check_output(["git", "ls-files", "-z"], cwd=root)
    return [Path(name.decode("utf-8")) for name in output.split(b"\0") if name]


def reject_constant(value):
    raise ValueError(f"Invalid JSON constant: {value}")


def check_file(path):
    suffix = path.suffix.lower()
    if suffix == ".py":
        # Compile bytes to honor Python encoding declarations; never execute/write pyc.
        compile(path.read_bytes(), str(path), "exec")
    elif suffix == ".json":
        json.loads(path.read_text(encoding="utf-8-sig"), parse_constant=reject_constant)
    elif suffix in XML_SUFFIXES:
        ET.parse(path)
    else:
        return False
    return True


def check_powershell(paths):
    if not paths:
        return 0
    return subprocess.run(
        ["pwsh", "-NoLogo", "-NoProfile", "-NonInteractive", "-Command", PS_PARSER],
        input=json.dumps([str(path.resolve()) for path in paths]),
        text=True, check=False,
    ).returncode


def main():
    failed = False
    checked = 0
    powershell = []
    for relative in tracked_files(ROOT):
        path = ROOT / relative
        if path.suffix.lower() in PS_SUFFIXES:
            powershell.append(path)
            continue
        try:
            checked += check_file(path)
        except (SyntaxError, ValueError, OSError, ET.ParseError) as error:
            print(f"{relative}: {error}", file=sys.stderr)
            failed = True
    try:
        failed = bool(check_powershell(powershell)) or failed
    except OSError as error:
        print(f"PowerShell parser unavailable: {error}", file=sys.stderr)
        failed = True
    print(f"Parsed {checked} Python/JSON/XML files and {len(powershell)} PowerShell files.")
    return int(failed)


if __name__ == "__main__":
    sys.exit(main())
