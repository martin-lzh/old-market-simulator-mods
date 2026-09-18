# Rome artwork prompts / 罗马插画提示词

Mode: built-in Image Gen. Nine retained assets in this directory: `rome-town-artwork.png`, `rome-town-detail.png`, `rome-caravan1-artwork.png`, `rome-caravan2-artwork.png`, `rome-mine-artwork.png`, and `rome-gate1-artwork.png` through `rome-gate4-artwork.png`.

## Region bases

Each region used its own locally rendered geometry reference, with full terrain bounds from its manifest.

Use case: style-transfer. Asset: calibrated orthographic game navigation map. Redraw this reference as original finely inked and painted Mediterranean/Roman cartography: muted olive vegetation, warm limestone paths and walls, terracotta roof silhouettes, slate rocks, subtle paper grain. Preserve EVERY geographic footprint, building location, open space, terrain extent and orientation exactly. Strict vertical overhead, no perspective, no labels, no icons, no compass, no border. Fill entire frame with map. Do not enlarge the settlement relative to surrounding terrain or invent structures, paths or water. This is a coordinate-aligned texture, not a poster.

Square-region suffix: Preserve the square aspect ratio.

Mine suffix: The reference is an underground mine. Preserve its very wide aspect, rock chambers and connecting timbered passages.

Gate 1 suffix: Preserve the tall narrow 1:2 portrait aspect, including ALL the empty southern terrain.

## Town detail

Use case: style-transfer. Asset: calibrated orthographic game navigation map. Redraw this reference as original finely inked and painted Mediterranean/Roman cartography: muted olive vegetation, warm limestone paths and walls, terracotta roof silhouettes, slate rocks, subtle paper grain. Preserve EVERY geographic footprint, building location, open space, terrain extent and orientation exactly. Strict vertical overhead, no perspective, no labels, no icons, no compass, no border. Fill entire frame with map. Do not enlarge the settlement relative to surrounding terrain or invent structures, paths or water. This is a coordinate-aligned texture, not a poster. Image 1 is the EXACT street-level geometry to redraw at high detail. Image 2 provides only the illustration palette and pen style; do not copy its zoom level. Keep all shapes from image 1 precisely aligned with all four edges, including the path clipped at the left edge. Keep the farmland rectangles empty rather than inventing extra buildings. Preserve slightly portrait 2048:2172 aspect. This will be inserted at its measured coordinates into the wider map. Subtle green surroundings should blend at its borders. No border/frame.

## Gate 4 correction

Precise-object-edit. Image 1 is the map to correct, image 2 is the exact geometry reference. Change ONLY the large elongated rectangle near the center-right: it is a flat pale limestone top surface/terrace, as shown in reference image 2, not a pitched red tiled roof. Remove its invented roof ridge and red tile pattern; restore the flat warm cream rectangle and its thin stepped edge. Keep its exact footprint, every other object and all terrain unchanged. Strict overhead map, no text.

## Gate 4 elevated lake correction — 2026-09-18

Built-in Image Gen, precise-object-edit. Input 1: existing Gate 4 artwork (edit target). Input 2: corrected local orthographic geometry with terrain-clipped water (geographic reference). The reference and raw scene evidence remain local.

Correct only the omitted lake west of the long cream terrace, matching the cyan region in the corrected geometry reference (approximately x42–55%, y38–53%). Preserve its irregular shoreline, stones, wooden square, overlapping rocks/trees, all other terrain, square framing, north-up orientation and 305 × 305 world coverage. Render blue-teal water in the existing ink/watercolor Mediterranean style, no labels. Do not add water to the southwestern circular terrain patch. Preserve the flat terrace, not a roof.

Cleanup pass: remove the newly invented horizontal tan rectangle at approximately x44.1%, y49.7%, replacing it with water; preserve the original diagonal square below/right at x45.5%, y51.5%. Keep the lake boundary and all other features unchanged.
