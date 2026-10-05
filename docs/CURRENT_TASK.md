# Sprint 8.1 — Planet Collection Visual Polish

Scope: center the Collection title on Safe Area and simplify the five-stage restoration row. Gameplay, restoration/save rules, navigation and preview selection remain unchanged.

The user-replaced collection_stage_frame.png is 2172×724 (3:1). It is reimported as a single Sprite, input alpha/transparency, bilinear/clamp, no mipmaps; Android ASTC 6×6. Audit: 737842 fully transparent pixels, 834617 partially transparent pixels, 69 opaque pixels. The source PNG is used directly.

Title X anchors change from 0.16–0.96 to 0.10–0.90. Width, aspect and Y anchors 0.845–0.985 are preserved; the Back button remains independent.

The new row contains five original planet sprites, four thin Gold UI connectors, small stage numbers and larger stage names. No collection_stage_slot.png is rendered; its file remains in the project. A thin procedural Cyan ring and the existing 1.00–1.04 pulse mark the actual current stage. Future stages retain dim thumbnails, smaller lock icons and dim labels. No new raster art is created.

Completed: all 20 existing regressions and final Collection probe PASS; all three resolutions rendered and inspected. Final compiler/shader warnings and runtime errors 0; Missing Script 0. Android Development Build warnings/errors 0, installed/cold-launched on SM-S942N. Preview, locked Planet 02, Back icon, HOME/GAME and prefs isolation verified. Known AssetPackManager exception and unconfirmed ADB system Back are detailed in DEVLOG.md.
