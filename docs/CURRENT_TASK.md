# Sprint 9.1.1 — Board & Piece Density Polish

Baseline: 941f48b (Sprint 9.1). Only Sprite mesh density and Slot internal Piece fitting are changed. Board/Frame/Slot outer Rects, Planet/Score/Home/Best positions, vertical layout, Grid centers, pointer conversion, shape offsets, rules and saves remain.

At full Safe Area 1080×1920, alpha >=20/255 source bounds (the original PNG is read only):

| Metric | Before | After |
|---|---:|---:|
| Cell center pitch | 115.5 | 115.5 |
| Logical Cell Rect | 103.5² | 103.5² |
| Empty visible pixels | 86.91×83.36 | 104.29×100.03 |
| Blue visible pixels | 83.03×79.23 | 99.64×95.08 |
| Purple visible pixels | 92.19×85.26 | 110.63×102.31 |
| Gold visible pixels | 92.19×86.17 | 110.63×103.40 |
| Empty width / center pitch | 75.25% | 90.30% |
| Block width / center pitch | 71.89–79.82% | 86.27–95.78% |
| Frame inner cyan rim to first visible Empty pixel X/Y | 42.34 / 39.57 | 33.65 / 31.14 |
| Frame outer Rect to first visible Empty pixel X/Y | 70.34 / 71.57 | 61.65 / 63.14 |
| Slot Rect | 336.8×324 | 336.8×324 |
| Piece Available Rect | effective common 3-cell envelope 245.28² | 276.18×265.68 |
| Logical Piece cell at rest | 77.76 | 84.56 |
| H3 visible bounds by palette | 229.90–236.78 × 59.53–64.74 | 262.52–271.51 × 77.68–84.48 |
| V3 visible bounds by palette | 62.38–69.26 × 227.05–232.26 | 81.40–90.39 × 258.80–265.60 |

CellSpriteDensity expands artwork 1.20 around each existing cell center, including its Preview/Outline mesh. Images remain noninteractive where they previously were. BoardView configures density; BlockPiece uses the same mesh for resting/dragged artwork. BlockDragHandler and logical SetGeometry/OriginWorld calculations are unchanged; drag scale 1.05, offset and Warm Gold/Muted Red/Gold Outline remain.

Slot fit: 82% of the existing root Rect is available, with existing 6 spacing. min(width/shape columns, height/shape rows), subtracting spacing, is capped by the widest/tallest catalog shape using the same rule. All shapes share one cell scale, so Single cannot fill the Slot. The initial 90% trial overlapped the V3 frame; direct renders led to the final common 82% rule. All 8 shapes are captured at all 3 resolutions.

Frame inner rim was sampled on the saved renders (cyan inner edge x=50 / y=540), then compared with alpha bounds. Near-transparent glow is excluded by the stated alpha threshold. Board frame inset and grid padding values are deliberately unchanged: growing visual pixels reduces visible frame padding while preserving the required outer Rect and exact Cell centers. No source pixels, texture import settings or Save keys were modified.

All 24 regression entry points pass. Final alpha bounds probe: 12,371 assertions PASS, all 8 shapes at all 3 resolutions; 64 coordinate roundtrips, pixel separation, frame containment and Slot containment. Missing Script 0. Final incremental Development build warnings/errors 0; final clean Editor shader/C# warnings and runtime errors 0. First native build had the three existing TMP splitting notices; initial post-build shader import had 414 existing precision warnings, final clean run 0. Protected HOME/Collection/feedback/modal scene diff 0; preserved GAME and all Cell RectTransforms diff 0; all 9 original PNG hashes unchanged. SM-S942N Android 16 install -r and launch succeeded; save before/after install 0/15930/2. Native QA has 12 matching edge drops over H3/H2/V2/V3/L/Reverse L and four HOME/PLAY flows. USB disconnected before Single/Square and final Logcat checks; user cannot reconnect now, so remaining physical checks are deferred. Delivery uses completed required automated checks and the available native evidence.
