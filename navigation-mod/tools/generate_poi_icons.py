# /// script
# requires-python = ">=3.12"
# dependencies = ["resvg-py==0.2.5", "pillow==11.1.0"]
# ///
"""Rebuild vendored Lucide place PNGs: uv run navigation-mod/tools/generate_poi_icons.py.

Only --fetch downloads the exact upstream commit; normal builds run offline.
"""
from pathlib import Path
import argparse
import hashlib
import io
import json
import urllib.request
import xml.etree.ElementTree as ET

import resvg_py
from PIL import Image, ImageFilter

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "assets" / "lucide"
COMMIT = "f12b0de177fbc2a6795e99be065887e72b237123"
FEATHER_LICENSE = "https://raw.githubusercontent.com/feathericons/feather/v4.29.2/LICENSE"
ICONS = ["wheat", "landmark", "hammer", "wrench", "lamp", "axe", "rabbit",
         "sprout", "shirt", "bed", "ship", "store", "house", "map-pin"]
PALETTE = {"shop": "#ad7568", "home": "#829278", "dock": "#77929d", "other": "#978190"}
SCALE = 4


def fetch():
    SOURCE.mkdir(parents=True, exist_ok=True)
    for filename in [*(f"{name}.svg" for name in ICONS), "LICENSE"]:
        upstream = filename if filename == "LICENSE" else "icons/" + filename
        url = f"https://raw.githubusercontent.com/lucide-icons/lucide/{COMMIT}/{upstream}"
        (SOURCE / filename).write_bytes(urllib.request.urlopen(url).read())
    (SOURCE / "FEATHER-LICENSE").write_bytes(urllib.request.urlopen(FEATHER_LICENSE).read())
    manifest = {"project": "https://github.com/lucide-icons/lucide", "version": "0.468.0", "commit": COMMIT,
                "feather_license": FEATHER_LICENSE,
                "sha256": {name: hashlib.sha256((SOURCE / name).read_bytes()).hexdigest()
                           for name in [*(f"{n}.svg" for n in ICONS), "LICENSE", "FEATHER-LICENSE"]}}
    (SOURCE / "source.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")


def build():
    manifest = json.loads((SOURCE / "source.json").read_text(encoding="utf-8"))
    assert manifest["commit"] == COMMIT
    for filename, expected in manifest["sha256"].items():
        assert hashlib.sha256((SOURCE / filename).read_bytes()).hexdigest() == expected, filename
    silhouettes = set()
    count = 0
    for name in ICONS:
        svg = ET.fromstring((SOURCE / f"{name}.svg").read_text(encoding="utf-8"))
        # Inset the official 24x24 line art to leave room for the outer stroke.
        svg.set("viewBox", "-4 -4 32 32")
        svg.set("width", str(32*SCALE)); svg.set("height", str(32*SCALE))
        svg.set("stroke", "white"); svg.set("stroke-width", "2")
        rendered = resvg_py.svg_to_bytes(svg_string=ET.tostring(svg, encoding="unicode"))
        mask = Image.open(io.BytesIO(rendered)).convert("RGBA").getchannel("A")
        silhouettes.add(hashlib.sha256(mask.tobytes()).hexdigest())
        # A 2px external outline, drawn at 4x then downsampled with the color line.
        outline = mask.filter(ImageFilter.MaxFilter(2*2*SCALE+1))
        outline = outline.resize((32, 32), Image.Resampling.LANCZOS)
        mask = mask.resize((32, 32), Image.Resampling.LANCZOS).point(lambda value: 255 if value >= 230 else value)
        for category, color in PALETTE.items():
            image = Image.new("RGBA", mask.size, "#0f0c09")
            image.putalpha(outline)
            inner = Image.new("RGBA", mask.size, color); inner.putalpha(mask)
            image = Image.alpha_composite(image, inner)
            assert image.getchannel("A").getextrema() == (0, 255)
            rgba = tuple(bytes.fromhex(color[1:])) + (255,)
            assert rgba in set(image.getdata()), (name, category, "missing solid palette color")
            image.save(ROOT / "assets" / f"poi-{name}-{category}.png", optimize=False)
            count += 1
    assert len(silhouettes) == len(ICONS), "Distinct places must have distinct silhouettes"
    print(f"Generated and checked {count} icons, {len(silhouettes)} distinct silhouettes, four exact palette colors and transparent backgrounds.")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(); parser.add_argument("--fetch", action="store_true")
    args = parser.parse_args()
    if args.fetch: fetch()
    build()
