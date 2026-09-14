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
