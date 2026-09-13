"""Build with versioned API references and publish independent immutable releases."""
from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import urllib.error
import urllib.parse
import urllib.request
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[1]
MODS = {
    "material-cost": "MaterialCost", "coordinates": "Coordinates",
    "checkout-all": "CheckoutAll", "stack-all": "StackAll",
    "price-probability": "PriceProbability", "navigation": "Navigation",
}
VERSION = r"(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)"
LOADERS = {
    "BepInEx": ("https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.5/BepInEx_win_x64_5.4.23.5.zip",
                 "82f9878551030f54657792c0740d9d51a09500eeae1fba21106b0c441e6732c4", "BepInEx/core"),
    "MelonLoader": ("https://github.com/LavaGang/MelonLoader/releases/download/v0.7.3/MelonLoader.x64.zip",
                    "5b2b2f3d1cd42b59ec886c5bdc2663edae87a0097a4f4a8f58c0965a99dda416", "MelonLoader/net35"),
}


def run(*args):
    subprocess.run([str(a) for a in args], cwd=ROOT, check=True)


def sha(data):
    return hashlib.sha256(data).hexdigest()


def changelog(text):
    """Both language sections must expose identical, descending release histories."""
    histories, notes = [], []
    for heading in ("English", "中文"):
        section = re.search(rf"^## {heading}\s*\n(.*?)(?=^## |\Z)", text, re.M | re.S)
        if not section:
            raise ValueError(f"Missing changelog language: {heading}")
        entries = list(re.finditer(rf"^### ({VERSION})\s+[^\n]+\n(.*?)(?=^### |\Z)", section[1], re.M | re.S))
        history = [e[1] for e in entries]
        if not history or len(history) != len(set(history)):
            raise ValueError("Missing/duplicate numbered CHANGELOG release")
        if history != sorted(history, key=lambda v: tuple(map(int, v.split('.'))), reverse=True):
            raise ValueError("CHANGELOG versions must be descending")
        histories.append(history)
        notes.append(f"## {heading}\n\n{entries[0][0].strip()}\n")
    if histories[0] != histories[1]:
        raise ValueError("English/Chinese CHANGELOG versions disagree")
    return histories[0][0], "\n".join(notes)


def config(slug):
    directory = ROOT / f"{slug}-mod"
    name = MODS[slug]
    version, notes = changelog((directory / "CHANGELOG.md").read_text(encoding="utf-8"))
    project = directory / f"{name}.csproj"
    if ET.parse(project).findtext(".//Version") != version:
        raise ValueError(f"{slug}: CHANGELOG and project version disagree")
    settings = json.loads((directory / "release.json").read_text())
    if set(settings) != {"sdk", "prerelease"} or type(settings["prerelease"]) is not bool:
        raise ValueError("Invalid release.json")
    if not re.fullmatch(VERSION + r"/r[1-9][0-9]*", settings["sdk"]):
        raise ValueError("Invalid SDK identifier")
    sdk = ROOT / "sdk" / settings["sdk"]
    manifest = json.loads((sdk / "manifest.json").read_text())
    if (manifest["schema"] != 1 or settings["sdk"] != f'{manifest["gameVersion"]}/r{manifest["revision"]}'
            or sha((sdk / "api.json").read_bytes()) != manifest["apiSha256"]):
        raise ValueError("SDK manifest/hash mismatch")
    api = json.loads((sdk / "api.json").read_text())
    game = next(a for a in api["Assemblies"] if a["Name"].startswith("Assembly-CSharp,"))
    if game["Sha256"] != manifest["gameAssemblySha256"]:
        raise ValueError("SDK game baseline mismatch")
    return dict(slug=slug, name=name, directory=directory, project=project,
                version=version, notes=notes, manifest=manifest, **settings)


def immutable_sdk(base):
    changed = subprocess.check_output(["git", "diff", "--name-only", base, "HEAD", "--", "sdk"], cwd=ROOT, text=True).splitlines()
    for path in changed:
        if re.match(r"sdk/[^/]+/r[0-9]+/", path):
            exists = subprocess.run(["git", "cat-file", "-e", f"{base}:{path}"], cwd=ROOT, capture_output=True).returncode == 0
            if exists:
                raise ValueError(f"Published SDK snapshots are immutable; create a new revision: {path}")


def loader(name):
    url, expected, relative = LOADERS[name]
    destination = ROOT / "work" / "ci-loaders" / name
    archive = destination.parent / f"{name}.zip"
    destination.mkdir(parents=True, exist_ok=True)
    if not archive.exists():
        urllib.request.urlretrieve(url, archive)
    if sha(archive.read_bytes()) != expected:
        raise ValueError(f"{name} archive checksum mismatch")
    # Extract only after hash validation, with an explicit path containment check.
    with zipfile.ZipFile(archive) as z:
        for entry in z.infolist():
            if not (destination / entry.filename).resolve().is_relative_to(destination.resolve()):
                raise ValueError("Unsafe loader archive path")
        z.extractall(destination)
    return destination / relative


def variants(slug):
    return ["BepInEx", "MelonLoader"] if slug == "material-cost" else ["BepInEx"]


def archive_name(c, variant):
    suffix = f"-{variant}" if c["slug"] == "material-cost" else ""
    return f'OldMarket.{c["name"]}-{c["version"]}{suffix}.zip'


def plugin_path(c, variant):
    name = f'OldMarket.{c["name"]}'
    return f'Mods/{name}.dll' if variant == "MelonLoader" else f'BepInEx/plugins/{name}/{name}.dll'


def validate_package(c, variant, data):
    import io
    with zipfile.ZipFile(io.BytesIO(data)) as z:
        expected = {plugin_path(c, variant), "README.md", "CHANGELOG.md", "LICENSE"}
        if len(z.namelist()) != len(expected) or set(z.namelist()) != expected:
            raise ValueError("Unexpected package content (SDK/loader/game files must never ship)")
        for doc in ("README.md", "CHANGELOG.md", "LICENSE"):
            if z.read(doc) != (c["directory"] / doc).read_bytes():
                raise ValueError(f"Packaged {doc} differs from release source")
        dll = z.read(plugin_path(c, variant))
        if not dll.startswith(b"MZ") or b"ReferenceAssemblyAttribute" in dll:
            raise ValueError("Not an executable Mod assembly")


def build():
    configs = [config(s) for s in MODS]
    run("dotnet", "restore", "tools/Sdk", "--locked-mode")
    run("dotnet", "build", "tools/Sdk", "-c", "Release", "--no-restore")
    tool = ROOT / "tools/Sdk/bin/Release/net8.0/Sdk.dll"
    loaders = {name: loader(name) for name in LOADERS}
    built = {}
    for c in configs:
        sdk_id = c["sdk"]
        refs = ROOT / "work" / "ci-sdk" / sdk_id
        if sdk_id not in built:
            run("dotnet", tool, "build", ROOT / "sdk" / sdk_id / "api.json", refs)
            built[sdk_id] = refs
        tests = [c["directory"] / "localization/tests/Localization.Tests.csproj",
                 c["directory"] / ("tests/CostTests.csproj" if c["slug"] == "material-cost" else "tests/Navigation.Tests.csproj" if c["slug"] == "navigation" else "tests/Tests.csproj")]
        for test in tests:
            if test.exists():
                run("dotnet", "run", "--project", test, "-c", "Release")
        for variant in variants(c["slug"]):
            output = ROOT / "work" / "ci-build" / c["slug"] / variant
            run("dotnet", "build", c["project"], "-c", "Release", "--nologo",
                f'-p:ManagedDir={refs}', f'-p:GameDir={ROOT / "work/NO_GAME"}',
                f'-p:BepInExDir={loaders["BepInEx"]}', f'-p:MelonLoaderDir={loaders["MelonLoader"]}',
                f'-p:Loader={variant}', f'-p:BaseIntermediateOutputPath=obj/CI/{variant}/',
                f'-p:OutputPath={output}/', '-p:AppendTargetFrameworkToOutputPath=false')
            dll = output / f'OldMarket.{c["name"]}.dll'
            run("dotnet", tool, "verify-plugin", dll, f'OldMarket.{c["name"]}', c["version"])
            target = ROOT / "outputs/ci" / c["slug"]
            target.mkdir(parents=True, exist_ok=True)
            package = target / archive_name(c, variant)
            with zipfile.ZipFile(package, "w", zipfile.ZIP_DEFLATED) as z:
                for entry, data in [(plugin_path(c, variant), dll.read_bytes())] + [
                        (doc, (c["directory"] / doc).read_bytes()) for doc in ("README.md", "CHANGELOG.md", "LICENSE")]:
                    info = zipfile.ZipInfo(entry, (2026, 1, 1, 0, 0, 0))
                    info.compress_type = zipfile.ZIP_DEFLATED
                    z.writestr(info, data)
            validate_package(c, variant, package.read_bytes())
        write_evidence(c, target)


def local_verify(game_dir):
    """Read-only local validation; requires a preceding SDK build."""
    game = Path(game_dir).resolve()
    managed = game / "Old Market Simulator_Data/Managed"
    tool = ROOT / "tools/Sdk/bin/Release/net8.0/Sdk.dll"
    loaders = {name: loader(name) for name in LOADERS}
    checked = set()
    for slug in MODS:
        c = config(slug)
        if c["sdk"] not in checked:
            run("dotnet", tool, "verify-game", ROOT / "sdk" / c["sdk"] / "api.json", managed)
            checked.add(c["sdk"])
        for variant in variants(slug):
            output = ROOT / "work/game-build" / slug / variant
            run("dotnet", "build", c["project"], "-c", "Release", f'-p:GameDir={game}',
                f'-p:ManagedDir={managed}', f'-p:BepInExDir={loaders["BepInEx"]}',
                f'-p:MelonLoaderDir={loaders["MelonLoader"]}', f'-p:Loader={variant}',
                f'-p:BaseIntermediateOutputPath=obj/GameVerify/{variant}/', f'-p:OutputPath={output}/',
                '-p:AppendTargetFrameworkToOutputPath=false')
            run("dotnet", tool, "compare", output / f'OldMarket.{c["name"]}.dll',
                ROOT / "work/ci-build" / slug / variant / f'OldMarket.{c["name"]}.dll')
        contract = c["directory"] / "tests/ContractChecks.csproj"
        if contract.exists():
            run("dotnet", "run", "--project", contract, "-c", "Release", f'-p:GameDir={game}',
                '-p:BaseIntermediateOutputPath=obj/Contracts/', "--", game,
                ROOT / "work/ci-build" / slug / "BepInEx" / f'OldMarket.{c["name"]}.dll')


def export_sdk(game_dir, game_version, revision, supplemental):
    """Prepare a new local API snapshot; never overwrite an existing baseline."""
    if not game_dir or not game_version or not re.fullmatch(VERSION, game_version) or not revision or revision < 1:
        raise ValueError("Provide --game-dir, --game-version MAJOR.MINOR.PATCH and --revision >= 1")
    if subprocess.check_output(["git", "status", "--porcelain"], cwd=ROOT, text=True).strip():
        raise ValueError("Commit source changes before exporting so the SDK records an exact source commit")
    target = ROOT / "sdk" / game_version / f"r{revision}"
    if target.exists():
        raise ValueError("SDK revision already exists; choose a new version/revision")
    if not supplemental:
        raise ValueError("Provide the reviewed --supplemental JSON for nameof-only members")
    seeds = Path(supplemental).resolve()
    json.loads(seeds.read_text())
    game = Path(game_dir).resolve()
    managed = game / "Old Market Simulator_Data/Managed"
    run("dotnet", "restore", "tools/Sdk", "--locked-mode")
    run("dotnet", "build", "tools/Sdk", "-c", "Release", "--no-restore")
    tool = ROOT / "tools/Sdk/bin/Release/net8.0/Sdk.dll"
    loaders = {name: loader(name) for name in LOADERS}
    plugins = []
    for slug, name in MODS.items():
        for variant in variants(slug):
            output = ROOT / "work/sdk-export-build" / slug / variant
            run("dotnet", "build", ROOT / f"{slug}-mod/{name}.csproj", "-c", "Release", f'-p:GameDir={game}',
                f'-p:ManagedDir={managed}', f'-p:BepInExDir={loaders["BepInEx"]}',
                f'-p:MelonLoaderDir={loaders["MelonLoader"]}', f'-p:Loader={variant}',
                f'-p:BaseIntermediateOutputPath=obj/SdkExport/{variant}/', f'-p:OutputPath={output}/',
                '-p:AppendTargetFrameworkToOutputPath=false')
            plugins.append(output / f'OldMarket.{name}.dll')
    run("dotnet", tool, "export", managed, ";".join(str(p) for p in loaders.values()), seeds, target / "api.json", *plugins)
    (target / "supplemental.json").write_bytes(seeds.read_bytes())
    manifest = dict(schema=1, gameVersion=game_version, revision=revision,
                    platform="Windows x64 / Unity Mono (verify Unity version locally)",
                    gameAssemblySha256=sha((managed / "Assembly-CSharp.dll").read_bytes()).upper(),
                    apiSha256=sha((target / "api.json").read_bytes()),
                    sourceCommit=subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip(),
                    verification="API export only; compile equivalence, game contracts and in-game acceptance still required.")
    (target / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8", newline="\n")
    print(f"Prepared {target.relative_to(ROOT)}. Review it, update selected release.json files, then build and verify-game.")


def write_evidence(c, directory):
    commit = subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip()
    evidence = dict(source=commit, mod=c["slug"], version=c["version"], sdk=c["sdk"],
                    gameVersion=c["manifest"]["gameVersion"], gameAssemblySha256=c["manifest"]["gameAssemblySha256"],
                    apiSha256=c["manifest"]["apiSha256"],
                    validation="SDK compilation and pure logic/localization tests; in-game validation not performed / SDK 编译与纯逻辑、本地化测试，未执行实机验证")
    (directory / "build-info.json").write_text(json.dumps(evidence, ensure_ascii=False, indent=2) + "\n", encoding="utf-8", newline="\n")
    files = [archive_name(c, v) for v in variants(c["slug"])] + ["build-info.json"]
    (directory / "SHA256SUMS.txt").write_text("".join(f'{sha((directory / name).read_bytes())}  {name}\n' for name in files), encoding="utf-8", newline="\n")


class GitHub:
    def __init__(self, repository):
        if not re.fullmatch(r"[\w.-]+/[\w.-]+", repository):
            raise ValueError("Invalid repository")
        self.base = f"https://api.github.com/repos/{repository}"
        self.token = os.environ["GH_TOKEN"]
        class AssetRedirect(urllib.request.HTTPRedirectHandler):
            def redirect_request(self, req, fp, code, msg, headers, newurl):
                result = super().redirect_request(req, fp, code, msg, headers, newurl)
                if result is not None and urllib.parse.urlparse(req.full_url).hostname != urllib.parse.urlparse(newurl).hostname:
                    result.remove_header("Authorization")
                return result
        self.opener = urllib.request.build_opener(AssetRedirect())

    def request(self, path, method="GET", payload=None, raw=False):
        url = path if path.startswith("https://") else self.base + path
        if urllib.parse.urlparse(url).hostname not in ("api.github.com", "uploads.github.com"):
            raise ValueError("Unexpected GitHub endpoint")
        data = payload if isinstance(payload, bytes) else None if payload is None else json.dumps(payload).encode()
        headers = {"Authorization": f"Bearer {self.token}", "Accept": "application/octet-stream" if raw else "application/vnd.github+json", "X-GitHub-Api-Version": "2022-11-28"}
        if data is not None:
            headers["Content-Type"] = "application/octet-stream" if isinstance(payload, bytes) else "application/json"
        try:
            with self.opener.open(urllib.request.Request(url, data, headers, method=method), timeout=120) as r:
                content = r.read()
                return content if raw else json.loads(content) if content else None
        except urllib.error.HTTPError as error:
            if error.code == 404 and method == "GET":
                return None
            raise

    def releases(self):
        releases = []
        for page in range(1, 100):
            batch = self.request(f"/releases?per_page=100&page={page}")
            releases.extend(batch)
            if len(batch) < 100:
                return releases
        raise ValueError("Release pagination limit exceeded")


def mod_changed_since_release(slug, commit, releases):
    """Compare committed directory contents with this Mod's highest published version."""
    if slug not in MODS or not re.fullmatch(r"[0-9a-f]{40}", commit):
        raise ValueError("Directory comparison requires a known Mod and exact commit")
    prefix = slug + "-v"
    tags = [r["tag_name"] for r in releases if not r["draft"]
            and r["tag_name"].startswith(prefix)
            and re.fullmatch(VERSION, r["tag_name"][len(prefix):])]
    directory = f"{slug}-mod/"
    if not tags:
        # First release: require tracked files at the exact release commit.
        return bool(subprocess.check_output(
            ["git", "ls-tree", "-r", "--name-only", commit, "--", directory], cwd=ROOT, text=True).strip())
    tag = max(tags, key=lambda t: tuple(map(int, t[len(prefix):].split('.'))))
    try:
        # Peel annotated tags; target_commitish may be a branch name and is not evidence.
        baseline = subprocess.check_output(
            ["git", "rev-parse", "--verify", f"refs/tags/{tag}^{{commit}}"], cwd=ROOT, text=True).strip()
    except subprocess.CalledProcessError as error:
        raise ValueError(f"Cannot resolve published tag {tag}; fetch complete history and tags") from error
    if not re.fullmatch(r"[0-9a-f]{40}", baseline):
        raise ValueError(f"Invalid published tag commit: {tag}")
    result = subprocess.run(["git", "diff", "--quiet", "--no-ext-diff", "--no-textconv",
                             baseline, commit, "--", directory], cwd=ROOT)
    if result.returncode not in (0, 1):
        raise ValueError(f"Cannot compare {directory} with published tag {tag}")
    return result.returncode == 1


def publish(repository, commit, api=None):
    if not re.fullmatch(r"[0-9a-f]{40}", commit):
        raise ValueError("Publish requires an exact commit SHA")
    actual = subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip()
    if actual != commit:
        raise ValueError("Checkout differs from release commit")
    api = api or GitHub(repository)
    published = {r["tag_name"]: r for r in api.releases()}
    for slug in MODS:
        c = config(slug)
        tag = f'{slug}-v{c["version"]}'
        existing = published.get(tag)
        if existing and not existing["draft"]:
            print(f"Skip published {tag}; assets and tag remain unchanged.")
            continue
        newer = [r for t, r in published.items() if t.startswith(slug + "-v") and not r["draft"]
                 and re.fullmatch(VERSION, t[len(slug) + 2:])
                 and tuple(map(int, t[len(slug) + 2:].split('.'))) > tuple(map(int, c["version"].split('.')))]
        if newer:
            raise ValueError(f"Refusing a new release older than an existing version: {tag}")
        if not mod_changed_since_release(slug, commit, published.values()):
            print(f"Skip {tag}; {slug}-mod/ is unchanged since its last published version (or has no tracked files).")
            continue
        directory = ROOT / "outputs/ci" / slug
        names = [archive_name(c, v) for v in variants(slug)] + ["build-info.json", "SHA256SUMS.txt"]
        files = {name: (directory / name).read_bytes() for name in names}
        for variant in variants(slug):
            validate_package(c, variant, files[archive_name(c, variant)])
        evidence = json.loads(files["build-info.json"])
        if (evidence["source"] != commit or evidence["sdk"] != c["sdk"] or evidence["version"] != c["version"]
                or evidence["mod"] != slug or evidence["apiSha256"] != c["manifest"]["apiSha256"]):
            raise ValueError("Build evidence mismatch")
        expected_sums = "".join(f'{sha(files[name])}  {name}\n' for name in names[:-1]).encode()
        if files["SHA256SUMS.txt"] != expected_sums:
            raise ValueError("Artifact checksum mismatch")
        body = (f'Game / 游戏: {c["manifest"]["gameVersion"]}; SDK: `{c["sdk"]}`\n\n'
                f'Source / 源码: `{commit}`\n\n'
                'Built with API references; in-game validation is not implied.\n'
                '使用接口 SDK 编译；不代表已通过实机验证。\n\n' + c["notes"])
        # A draft can be resumed only for the same commit. Never overwrite an existing asset.
        if existing:
            if existing["target_commitish"] != commit or existing["body"] != body or existing["prerelease"] != c["prerelease"]:
                raise ValueError(f"Draft belongs to different source/metadata: {tag}")
        else:
            if api.request(f"/git/ref/tags/{tag}") is not None:
                raise ValueError(f"Tag already exists without a published release: {tag}")
            existing = api.request("/releases", "POST", dict(tag_name=tag, target_commitish=commit,
                name=f'{c["name"]} {c["version"]} (game {c["manifest"]["gameVersion"]})', body=body,
                draft=True, prerelease=c["prerelease"], make_latest="false"))
        assets = {a["name"]: a for a in existing["assets"]}
        if set(assets) - set(files):
            raise ValueError("Unexpected assets in release draft")
        for name, data in files.items():
            asset = assets.get(name)
            if asset is None:
                url = existing["upload_url"].split("{")[0] + "?name=" + urllib.parse.quote(name)
                asset = api.request(url, "POST", data)
            remote = api.request(f'/releases/assets/{asset["id"]}', raw=True)
            if remote != data:
                raise ValueError(f"Uploaded asset differs: {name}. Draft preserved for inspection.")
        api.request(f'/releases/{existing["id"]}', "PATCH", dict(draft=False, make_latest="false"))
        print(f"Published {tag} at {commit}.")


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("command", choices=["validate", "build", "publish", "verify-game", "export-sdk"])
    parser.add_argument("--base")
    parser.add_argument("--repository")
    parser.add_argument("--commit")
    parser.add_argument("--game-dir")
    parser.add_argument("--game-version")
    parser.add_argument("--revision", type=int)
    parser.add_argument("--supplemental")
    args = parser.parse_args()
    if args.command == "validate":
        for slug in MODS:
            c = config(slug)
            print(f'{slug} {c["version"]}: SDK {c["sdk"]}')
        if args.base:
            immutable_sdk(args.base)
    elif args.command == "build":
        build()
    elif args.command == "verify-game":
        if not args.game_dir:
            parser.error("verify-game requires --game-dir")
        local_verify(args.game_dir)
    elif args.command == "export-sdk":
        export_sdk(args.game_dir, args.game_version, args.revision, args.supplemental)
    else:
        publish(args.repository, args.commit)
