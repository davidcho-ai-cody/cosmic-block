# Sprint 9.2 — Line Clear & Combo Juice

Baseline: f11ee3e (Sprint 9.1.1). HOME/Collection, GAME layout and all Board/Slot coordinate geometry remain. User-provided Blue and Starlight PNGs are used without pixel editing.

## Assets

Alpha bounds use alpha >=20/255 and half-open coordinates. Blue before: (124,147)-(1130,1107), 1006×960. New Blue: (65,94)-(1189,1135), 1124×1041. Purple 1117×1033; Gold 1117×1044. New Blue width differs by +0.627%; height +0.774% vs Purple / -0.287% vs Gold. All three are RGBA 1254² with transparent corners and alpha 0..255. Shared existing mesh scale 1.20 and logical Cell Rect 103.5 are retained; no palette-specific scaling. Standard visible Blue becomes 111.32×103.10 versus previous 99.64×95.08.

Shared Sprite Single / FullRect / centered pivot / PPU 100 / alpha FromInput / alphaIsTransparency / no mipmaps / Bilinear / Clamp. Default max2048 uncompressed; existing identical Android max1024 ASTC6x6 overrides on the three Blocks remain. Starlight is RGBA1287×1222, transparent corners, alpha0..254; source bytes untouched.

## Runtime

BoardModel snapshots every completed Row/Column before any mutation. SequentialClearPlan sorts representative (0,y) for Rows / (x,0) for Columns by Y, X, then Row before Column at the (0,0) tie. Board Y increases downward. A HashSet is used only to deduplicate cells, never for ordering. All lines remain in the frozen plan even after intersections disappear. Each unique cell is popped/removed once; line counting remains per line.

GameSession starts lines with 0.16-second realtime waits. It remains Resolving throughout the clear sequence; drop/drag, refill and Game Over cannot observe an intermediate board. The existing 0.68-second final feedback hold is retained. Once the full sequence completes, supply/placeability is evaluated; non-boundary Planet fragment flight preserves existing free-play behavior. Token checks reject stale callbacks. HOME/Retry settle pending accepted-turn score/energy once, stop the coroutine and reset feedback/Planet presentation before a clean run.

The old solid-square effect had no original Block Sprite after immediate model deletion. Each new step copies the original occupied Board Image sprite/color before clearing that step. ClearCell meshes use the same shared density as Blocks: 0.10-second soft flash, 0.18-second pop/shrink/fade. Original colors/sprites remain visible.

Starlight pool: 48 prebuilt Images, 12 per nonempty line, no runtime Instantiate/Destroy. Existing 24 Text star objects are disabled. Visible lifetime .35–.65s after .10s onset (overall .45–.75s), size22–38 UI units, randomized direction35–85 plus upward35, spin ±120deg/s, white/cyan/soft-gold tint, fade. The existing 12 Planet flight Fragment Images use the same star glyph; their .46-second Bezier flight, .022 stagger, counts, callbacks and Stage cinematic are unchanged.

Combo popup: CLEAR! then 2 COMBO / 3 COMBO etc, 52 bold, dark Outline/cyan shadow, .7→1.15→1 scale and .68s hold/fade. Each line restarts one pooled text pair and displays its own +score. Same-turn line points are100,150,200,250: sums1/2/3=100/250/450. Placed cells still10 each. Placement streak is retained separately and no longer adds a second score bonus.

Per-line SFX pitches1/1.05/1.10/1.15 (cap), original clip and volume.58. Android vibrator uses20ms one-shot/default amplitude (API26+), old vibrate overload below26; fallback retained. Sound/Haptics gates are tested OFF. Current Settings UI is a placeholder; persisted Settings integration remains pending, no new general Settings feature/save keys.

Planet restoration code/thresholds/save keys are unchanged. Snapshot total awards1/2/3/4+=10/25/45/70 exactly once after visual clear; boundary/overflow and cancellation preserve total. Migration and Best Score are not reset.

## Validation

Final checks are completed below; physical QA is pending device connection. Evidence under Validation/sprint92_* includes alpha hashes, mixed-palette flash/pop renders, actual coroutine timing, intersections, numeric score, input locks, supply, callbacks, Home/Retry and responsive Safe Area assertions.

Legacy synchronous rule probes now explicitly advance the new line timeline through UNITY_EDITOR-only helpers. The production coroutine remains the Android path. The old Sprint4 simultaneous-clear/placement-streak UI expectations are superseded by the real Sprint92 timeline probe; core pointer/game/save/Planet regression remains intact.

Final validation (2026-10-07): all 25 regression entrypoints PASS, including Sprint 0–9.1.1 and the real Sprint92 coroutine probe. Final probe: 1,477 assertions PASS; uncaptured intervals 0.1612485 / 0.1606168 seconds; actual AudioSource pitch per line verified. Mixed-color Row, Column, Double Row/Column, cross, three-line and four-line cases pass. One/two/three-line point sums100/250/450 and 3-cell three-line total480 pass. Energy45 once for three lines, Energy25 for cross at0/90/190/290/390, clamp400, migration/Retry/Home preservation pass.

Responsive1080×1920,1080×2400 (Safe Area2% side,4% bottom,6% top),1080×1440: all requested captures and geometry assertions PASS. Board/Slot/Planet/Score RectTransform differences0; HOME/Collection/completion/confirmation/GameOver canonical scene differences0. All source Game PNG hashes match the user asset preflight; no source processing. Full-stage Planet code/assets are unchanged.

Android Development IL2CPP APK succeeded. First native build: Error0, three existing TMP large-method splitting notices. Final incremental build: Error0/Warning0. First postbuild Editor shader import:414 known URP precision warnings; final clean Editor run: C# Warning0/Compiler Error0/Shader Warning0/Runtime Game Error0. Missing Script0. No warning suppression/package modification. Manifest includes VIBRATE.

Final ADB devices -l has no device. SM-S942N install/launch/Logcat and actual Single/H3/V3/Square, Row/Column/cross/three-line touch QA are PENDING. No native zero-error or physical feel PASS is claimed. Previous AssetPackManager/Dex finalizer environment logs are historical, not freshly observed in this APK. Preserve physical save data when installing with -r; do not uninstall/reset to simplify QA.

Evidence: Validation/sprint92_assets.json, sprint92_tests.txt, sprint92_regression_summary.txt, sprint92_missing.log, sprint92_android_build[_final].log, sprint92_postbuild_probe.log, sprint92_clean_probe.log, sprint92_scene_preservation.txt, sprint92_layout_preservation.txt, sprint92_adb_devices.txt and mixed-palette/responsive PNGs.

APK bytes=151879934; SHA256=f904adf503fe4384584948cf8fdb76d86e1ac3796a9d3e7469038e89cb217f58
