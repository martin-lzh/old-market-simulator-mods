"""Build Mods against versioned API references; release them through tools/modrelease.py."""
from __future__ import annotations

import argparse
import hashlib
import json
import math
from pathlib import Path
import re
import subprocess
import struct
import urllib.request
import xml.etree.ElementTree as ET
import zipfile
import zlib

import modrelease

ROOT = Path(__file__).resolve().parents[1]
MODS = {
    "material-cost": "MaterialCost", "coordinates": "Coordinates",
    "checkout-all": "CheckoutAll", "stack-all": "StackAll",
    "price-probability": "PriceProbability", "navigation": "Navigation",
    "tree-info": "TreeInfo",
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
    settings = json.loads((directory / "release.json").read_text(encoding="utf-8"))
    if (set(settings) != {"displayName", "sdk", "prerelease"} or type(settings["prerelease"]) is not bool
            or not isinstance(settings["displayName"], str) or not settings["displayName"].strip()):
        raise ValueError(f"{slug}: release.json needs displayName, sdk and prerelease")
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


def validate_png(data):
    if not data.startswith(b"\x89PNG\r\n\x1a\n") or len(data) > 32 * 1024 * 1024:
        raise ValueError("Invalid map PNG signature or size")
    offset, kinds = 8, []
    while offset + 12 <= len(data):
        size = struct.unpack_from(">I", data, offset)[0]
        end = offset + 12 + size
        if end > len(data):
            raise ValueError("Truncated map PNG")
        chunk = data[offset + 4:offset + 8 + size]
        if zlib.crc32(chunk) & 0xffffffff != struct.unpack_from(">I", data, offset + 8 + size)[0]:
            raise ValueError("Map PNG CRC mismatch")
        kind = chunk[:4]
        if not kinds:
            if kind != b"IHDR" or size != 13:
                raise ValueError("Invalid map PNG header")
            dimensions = struct.unpack_from(">II", chunk, 4)
            if not all(0 < n <= 8192 for n in dimensions):
                raise ValueError("Invalid map PNG dimensions")
        kinds.append(kind)
        offset = end
        if kind == b"IEND":
            break
    if offset != len(data) or not kinds or kinds[-1] != b"IEND" or b"IDAT" not in kinds:
        raise ValueError("Incomplete map PNG")
    return dimensions


def map_resources(c):
    if c["slug"] != "navigation":
        return {}
    manifest = json.loads((c["directory"] / "map-pack.json").read_text(encoding="utf-8"))
    if (set(manifest) != {"schema", "gameVersion", "files"} or manifest["schema"] != 1
            or manifest["gameVersion"] != c["manifest"]["gameVersion"]
            or not isinstance(manifest["files"], list) or not manifest["files"]):
        raise ValueError("Invalid map pack manifest or game baseline")
    directory = c["directory"] / "maps"
    files, dimensions = {}, {}
    for entry in manifest["files"]:
        if (not isinstance(entry, dict) or set(entry) != {"name", "sha256"}
                or not re.fullmatch(r"[A-Za-z0-9][A-Za-z0-9._-]*\.(json|png)", entry["name"])
                or not re.fullmatch(r"[0-9a-f]{64}", entry["sha256"])):
            raise ValueError("Invalid map pack file entry")
        name = entry["name"]
        if name.lower() in {n.lower() for n in files}:
            raise ValueError("Duplicate map pack file")
        path = directory / name
        if path.resolve().parent != directory.resolve():
            raise ValueError("Map resource escapes source directory")
        data = path.read_bytes()
        if sha(data) != entry["sha256"]:
            raise ValueError(f"Map resource hash mismatch: {name}")
        if name.endswith(".png"):
            dimensions[name] = validate_png(data)
        elif len(data) > 32768:
            raise ValueError("Map metadata too large")
        files[name] = data
    ids, textures = set(), set()
    for name, data in files.items():
        if not name.endswith(".json"):
            continue
        m = json.loads(data)
        bounds = [m.get(k) for k in ("MinX", "MaxX", "MinZ", "MaxZ")]
        if (not all(type(v) in (int, float) and math.isfinite(v) for v in bounds)
                or bounds[0] >= bounds[1] or bounds[2] >= bounds[3]
                or not isinstance(m.get("Id"), str) or not m["Id"] or m["Id"] in ids
                or m.get("North") != "+Z" or type(m.get("MapId")) is not int
                or not isinstance(m.get("SceneName"), str) or not m["SceneName"]):
            raise ValueError("Invalid or duplicate map metadata")
        ids.add(m["Id"])
        if "TextureBounds" in m and m["TextureBounds"] is not None:
            texture_bounds = m["TextureBounds"]
            keys = ("MinX", "MaxX", "MinZ", "MaxZ")
            if (not isinstance(texture_bounds, dict) or not all(k in texture_bounds for k in keys)
                    or not all(type(texture_bounds[k]) in (int, float) and math.isfinite(texture_bounds[k]) for k in keys)
                    or not texture_bounds["MinX"] <= bounds[0] < bounds[1] <= texture_bounds["MaxX"]
                    or not texture_bounds["MinZ"] <= bounds[2] < bounds[3] <= texture_bounds["MaxZ"]):
                raise ValueError("Map texture bounds invalid")
        for key in ("Texture", "OverlayTexture", "DetailTexture"):
            texture = m.get(key)
            if key != "Texture" and not texture:
                continue
            if texture not in dimensions:
                raise ValueError(f"Map texture not in package: {texture}")
            textures.add(texture)
        # Base artwork and overlays may use different pixel resolutions: the runtime
        # aligns them through the same normalized UVs and explicit world bounds.
    if not ids or textures != set(dimensions):
        raise ValueError("Map pack must contain maps and exactly their referenced textures")
    return {f"BepInEx/plugins/OldMarket.Navigation/maps/{name}": data for name, data in files.items()}


def package_resources(c):
    return {**{doc: (c["directory"] / doc).read_bytes() for doc in ("README.md", "CHANGELOG.md", "LICENSE")},
            **map_resources(c)}


def write_package(c, variant, dll, output):
    if variant not in variants(c["slug"]):
        raise ValueError("Unsupported loader variant")
    entries = {plugin_path(c, variant): Path(dll).read_bytes(), **package_resources(c)}
    output = Path(output)
    output.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(output, "w", zipfile.ZIP_DEFLATED) as z:
        for entry, data in entries.items():
            info = zipfile.ZipInfo(entry, (2026, 1, 1, 0, 0, 0))
            info.compress_type = zipfile.ZIP_DEFLATED
            z.writestr(info, data)
    validate_package(c, variant, output.read_bytes())


def validate_package(c, variant, data):
    import io
    resources = package_resources(c)
    with zipfile.ZipFile(io.BytesIO(data)) as z:
        expected = {plugin_path(c, variant), *resources}
        if len(z.namelist()) != len(expected) or set(z.namelist()) != expected:
            raise ValueError("Unexpected package content (SDK/loader/game files must never ship)")
        for doc, expected_data in resources.items():
            if z.read(doc) != expected_data:
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
                maps = [c["directory"] / "maps" / Path(name).name for name in map_resources(c) if name.endswith(".json")]
                run("dotnet", "run", "--project", test, "-c", "Release", *(["--", *maps] if maps else []))
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
            write_package(c, variant, dll, package)
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
                    apiSha256=c["manifest"]["apiSha256"])
    if c["slug"] == "navigation":
        evidence["mapFiles"] = {name: sha(data) for name, data in map_resources(c).items()}
    (directory / "build-info.json").write_text(json.dumps(evidence, ensure_ascii=False, indent=2) + "\n", encoding="utf-8", newline="\n")


def pending_changes(c):
    text = (c["directory"] / "CHANGELOG.md").read_text(encoding="utf-8")
    for heading in ("Unreleased", "未发布"):
        for match in re.finditer(rf"^### {heading}\s*\n(.*?)(?=^## |^### |\Z)", text, re.M | re.S):
            if re.sub(r"<!--.*?-->", "", match[1], flags=re.S).strip():
                return True
    return False


def release_assets(c):
    directory = ROOT / "outputs/ci" / c["slug"]
    return tuple(directory / name for name in [archive_name(c, v) for v in variants(c["slug"])] + ["build-info.json"])


def verify_artifacts(c, commit):
    """Check one Mod's CI packages before anything is uploaded."""
    files = {path.name: path.read_bytes() for path in release_assets(c)}
    for variant in variants(c["slug"]):
        validate_package(c, variant, files[archive_name(c, variant)])
    evidence = json.loads(files["build-info.json"])
    if (evidence["source"] != commit or evidence["sdk"] != c["sdk"] or evidence["version"] != c["version"]
            or evidence["mod"] != c["slug"] or evidence["apiSha256"] != c["manifest"]["apiSha256"]):
        raise ValueError(f'{c["slug"]}: build evidence does not match the release source')


def release_mods(repository):
    mods = []
    for slug in MODS:
        c = config(slug)
        loaders = "BepInEx 5 or MelonLoader 0.7.3" if len(variants(slug)) > 1 else "BepInEx 5"
        mods.append(modrelease.ModRelease(
            slug=slug, display_name=c["displayName"], version=c["version"], prerelease=c["prerelease"],
            notes=c["notes"], unreleased=pending_changes(c), assets=release_assets(c),
            summary=f'Old Market Simulator {c["manifest"]["gameVersion"]} (Windows) · {loaders}',
            readme_url=f"https://github.com/{repository}/blob/main/{slug}-mod/README.md"))
    return mods


def publish(repository, commit, api=None):
    actual = subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip()
    if actual != commit:
        raise ValueError("Checkout differs from release commit")
    for slug in MODS:
        verify_artifacts(config(slug), commit)
    modrelease.publish(api or modrelease.GitHub(repository), release_mods(repository), commit)


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("command", choices=["validate", "build", "package", "release-plan", "publish", "verify-game", "export-sdk"])
    parser.add_argument("--dll")
    parser.add_argument("--output")
    parser.add_argument("--variant", default="BepInEx")
    parser.add_argument("--base")
    parser.add_argument("--repository")
    parser.add_argument("--commit")
    parser.add_argument("--game-dir")
    parser.add_argument("--game-version")
    parser.add_argument("--revision", type=int)
    parser.add_argument("--supplemental")
    parser.add_argument("--mod")
    args = parser.parse_args()
    if args.command == "validate":
        for slug in MODS:
            c = config(slug)
            map_resources(c)
            print(f'{slug} {c["version"]}: SDK {c["sdk"]}')
        if args.base:
            immutable_sdk(args.base)
    elif args.command == "build":
        build()
    elif args.command == "package":
        if not args.mod or not args.dll or not args.output:
            parser.error("package requires --mod, --dll and --output")
        write_package(config(args.mod), args.variant, args.dll, args.output)
    elif args.command == "verify-game":
        if not args.game_dir:
            parser.error("verify-game requires --game-dir")
        local_verify(args.game_dir)
    elif args.command == "export-sdk":
        export_sdk(args.game_dir, args.game_version, args.revision, args.supplemental)
    elif args.command == "release-plan":
        if not args.repository:
            parser.error("release-plan requires --repository")
        modrelease.check(modrelease.GitHub(args.repository), release_mods(args.repository))
    else:
        if not args.repository or not args.commit:
            parser.error("publish requires --repository and --commit")
        publish(args.repository, args.commit)
