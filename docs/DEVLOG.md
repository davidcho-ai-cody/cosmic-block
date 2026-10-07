# Sprint 8 Development Log

## Structure and data

- GameCanvas/SafeArea/CollectionRoot holds the background, title, back/navigation buttons, name frame, dynamic hero, progress, five stage slots and lore frame.
- PlanetCollectionView reads GameSession.Restoration. PreviewStage and PlanetIndex are view state only. The view contains no PlayerPrefs writes or progression setters.
- PlanetCollectionData stores five planet display names and stage-specific lore text separately from the view.
- Existing stage mapping is preserved: 1 DESOLATE / 황폐, 2 AWAKENING / 깨어남, 3 RECOVERING / 회복, 4 THRIVING / 번성, 5 RESTORED / 완성.
- Existing model thresholds remain 0–99, 100–199, 200–299, 300–399, and 400. Collection does not introduce thresholds or progression rules.
- Completed stages remain visible and previewable; current stage has a 1.00–1.04 unscaled pulse and cyan label; future thumbnails are dimmed with a small lock.
- Preview changes the hero and message only. Reentry restores the actual current stage.
- Prev/Next stop at Planet 01/05; boundary buttons are disabled. Locked planets use the supplied lock asset, ??? name and locked stage slots, with no invented planet sprites.
- Existing HOME DEV presets remain development-only; no additional Collection DEV UI is created.

## Assets

All nine user-supplied PNG files are used unchanged. Sprite import: Single, input alpha, alpha transparency, no mipmaps, bilinear, clamp; Editor uncompressed, Android ASTC 6x6. Icons/slot max 1024, frames/title max 2048. All assets have transparent and partial-alpha pixels; full pixel counts are recorded in Validation/sprint8_asset_alpha.txt.

| PNG | Source dimensions | Use |
| --- | --- | --- |
| collection_title.png | 2172×724 | Header logo |
| collection_back_icon.png | 1263×1246 | HOME return |
| collection_planet_name_frame.png | 1983×793 | Dynamic number/name |
| collection_prev_icon.png | 1429×1101 | Previous planet |
| collection_next_icon.png | 1430×1100 | Next planet |
| collection_stage_frame.png | 1945×809 | Stage row frame |
| collection_stage_slot.png | 1282×1227 | Five repeated thumbnail frames |
| collection_lock_icon.png | 1302×1208 | Locked hero/future slots |
| collection_message_frame.png | 1947×808 | Dynamic lore |

Reference images guide composition only and are not imported as full-screen UI. Existing Home_Background_StarlightBlock.png and Planet01_Stage01_Desolate.png through Planet01_Stage05_Restored.png are reused.

## Files

- Assets/Scripts/UI/PlanetCollectionData.cs
- Assets/Scripts/UI/PlanetCollectionView.cs
- Assets/Scripts/UI/GameFlowController.cs
- Assets/Scripts/UI/HomeViewController.cs
- Assets/Editor/Sprint8CollectionBuilder.cs
- Assets/Editor/Sprint8CollectionPlayProbe.cs
- Assets/Editor/Sprint71HomePlayProbe.cs: replace obsolete placeholder-toast assertion with Collection/save-isolation assertion.
- Assets/Scenes/Game.unity and Collection PNG imports/meta files.

## Validation

Validation is running. Final regression/build/device results will be appended after completion.

Automated probe snapshots and restores Energy, migration version and Best Score preferences. It covers five stages, unlocked/current/future states, completed progress, lore, preview/save isolation, reentry, navigation boundaries, Android Back, HOME/GAME flow, Release DEV contract, text visibility, stage overlap and Safe Area bounds. Rendering covers Planet 01 and a locked planet at 1080×1920, 1080×2400 with insets, and 1080×1440.

Images: Validation/sprint8_collection_9x16.png, sprint8_collection_tall_safe.png, sprint8_collection_short.png, sprint8_collection_locked_9x16.png, sprint8_collection_locked_tall_safe.png, sprint8_collection_locked_short.png.

## Final validation — 2026-10-05

- PASS: all 20 existing entry points: Sprint 0/1/2/3/4, VisualReadability, AndroidUx, Sprint 5, Sprint 5 Journey Feedback, Sprint 5 New Run Game Over, Sprint 6/6.5/6.6, Sprint 7.1/7.2/7.3/7.4/7.5/7.6/7.6.1.
- PASS: Sprint8CollectionPlayProbe, including final post-build rerun; five-stage/current/past/future/completed states, lore, readonly preview, no Piece resupply on Collection navigation, reentry, boundary buttons, no duplicate Collection view, Release DEV visibility contract, and HOME/GAME flow.
- PASS: Planet 01 and locked planet renders at 1080×1920, 1080×2400 with 2% horizontal/4% bottom/6% top insets, and 1080×1440. No control outside Safe Area, overlapping stage slots or clipped text. All six images were visually inspected; label/frame spacing was corrected.
- HOME subtree audit: no layout/style changes. Editor automatic HomeBackground size recalculation was restored and is preserved by the Collection builder.
- Compiler C# warnings/errors: 0. Missing Script: 0. Runtime scene errors: 0. Final post-build shader warnings: 0. New runtime code contains no deprecated object-finding calls.
- Initial script-reload rendering logged 414 precision-conversion shader warnings from Unity's render-pipelines.core package (Editor D3D11 path-tracing/lightmap shaders). They did not recur in subsequent regressions or the final post-build probe; package shaders were not changed. Initial diagnostics remain in local logs.
- Android Build Report: Succeeded, warnings 0, errors 0. APK: Builds/Android/Development/CosmicBlock-dev.apk, 149,480,119 bytes.
- Samsung SM-S942N, Android 16/API 36, physical 1080×2340: normal ADB device, adb install -r Success, cold launch Success.
- Device: HOME → Collection; Planet 01 Stage 3/51% matches saved Energy 205. Touch Stage 1 and Stage 2 previews change Hero/lore; Stage 4 touch remains blocked. Planet 02 locked and Planet 05 boundary/disabled Next verified. Back icon → HOME → Collection defaults to actual Stage 3. HOME → GAME starts Score 0 with three Pieces and no Game Over panel.
- Actual device preferences before/after: Planet01Energy 205, PlanetRestorationVersion 2, BestScore 4100; all unchanged. Device progress was never reset or set to a preset.
- App PID Logcat: Crash/FATAL EXCEPTION/AndroidJavaException/NullReferenceException/MissingReferenceException/UnityException 0; Unity warnings 0. One existing E Unity ClassNotFoundException for com.google.android.play.core.assetpacks.AssetPackManager remains, explicitly allowed by the Sprint instruction; no new game-code errors.
- Extra hardware Back/relaunch screen confirmation was interrupted by the phone locking and subsequent loss of the ADB connection. Back icon navigation is verified; Android gesture/physical Back should be checked manually after unlocking. No PASS is claimed for the interrupted check or subjective touch/display/Game Feel quality.
- Local evidence: Validation/sprint8_regression_summary.txt, sprint8_collection.txt, sprint8_asset_alpha.txt, sprint8_postbuild_probe.log, sprint8_missing.log, sprint8_android_build.log, sprint8_device_logcat.txt, sprint8_device_prefs_before.txt, sprint8_device_prefs_after.txt.
- Device screenshots: sprint8_device_home.png, sprint8_device_planet01.png, sprint8_device_preview1.png, sprint8_device_preview2.png, sprint8_device_future_locked.png, sprint8_device_locked.png, sprint8_device_planet05.png, sprint8_device_reentry.png, sprint8_device_game.png, sprint8_device_home_confirm.png (all under Validation/).

Only Collection UI/navigation, its scene/assets and automated checks were added. Board/Piece/Score/Combo/line clear/energy/save/transition/SFX/haptic rules and HOME design were preserved. Build-generated pipeline/settings/performance-resource changes were removed.

# Sprint 8.1 — Planet Collection Visual Polish

## Scope and assets

- Title X anchors: 0.16–0.96 → 0.10–0.90, independently centered on Safe Area. Width and Y anchors remain unchanged.
- User-replaced collection_stage_frame.png: 2172×724, 3:1. Transparent/partial/opaque pixel counts: 737842/834617/69. SHA256 F20F4E7F14594A23616B787BBC9A8C6FE8067A819D1AD7D45B3D41A0375F7A21. Source PNG was not processed.
- Reimported Sprite/Single, input alpha, transparency, no mipmaps, bilinear/clamp; Android ASTC 6×6, max size 2048.
- Removed five SlotFrame objects from Collection rendering; collection_stage_slot.png remains in the project.
- Five identical base thumbnail areas have centers 0.14, 0.32, 0.50, 0.68, 0.86 inside the fitted frame. Existing original stage sprites are reused.
- Four thin Gold Image connectors, a procedural Cyan current-stage ring, and the existing 1.00–1.04 pulse. No new raster assets.
- Stage numbers use size 18, names size 25 (responsive best fit). Names/numbers are inside the frame with padding. Completed/current thumbnails stay alpha 1; future thumbnails alpha 0.3 with small lock overlays and text alpha 0.65.
- Current highlighting follows the actual stage when past-stage preview changes the Hero.
- HOME design, Hero/name/message/progress dimensions, navigation button hit areas and all game/restoration/save/preview/navigation rules are preserved.
- Builder now resolves the HOME background's actual serialized transform ID to preserve its existing size.

## Title center measurements

| Resolution | Previous center X | New center X | Safe Area center X | Final absolute offset |
|---|---:|---:|---:|---:|
| 1080×1920 | 604.8 | 540 | 540 | 0 px |
| 1080×2400 with Safe Area | 602.208 | 540 | 540 | 0.000031 px |
| 1080×1440 | 604.8 | 540 | 540 | 0.000031 px |

Previous positions are derived from the previous 0.56 anchor center at the same tested Safe Area. The corresponding prior offsets are 64.8/62.208/64.8 px; thus full-width previous center X is 604.8.

## Automated and visual validation

All 20 existing regression entry points passed, along with the Collection probe. Final Collection probe passed after adjusting label padding: five stages, current rings, dim future labels, number/name hierarchy, preview/Save/Best/Energy/current-stage isolation, reentry, locked planets 02–05, navigation bounds, HOME/GAME flow and Release DEV contract.

Numeric checks cover title centering, five equal-spaced stage areas, equal proportions, thumbnail/frame bounds, visible frame padding, separation of number/name/thumbnail, no old slot sprite rendering, Safe Area and text clipping.

Rendered and inspected Planet 01 at 1080×1920, 1080×2400 with Safe Area insets, and 1080×1440, plus locked layouts. Files: Validation/sprint8_1_collection_9x16.png, sprint8_1_collection_tall_safe.png, sprint8_1_collection_short.png, and corresponding locked variants.

Scene preservation comparison normalizes Unity-generated local IDs and compares serialized objects by hierarchy path/component type. Result: all non-Collection scene blocks unchanged, including HOME, Board/Piece, feedback, flow and restoration objects.

Evidence: Validation/sprint8_1_regression_summary.txt, sprint8_1_collection.txt, sprint8_1_scene_preservation.txt, sprint8_asset_alpha.txt and sprint8_1_* logs. Final build/device diagnostics follow below.

## Final diagnostics, Android and remaining manual checks

- Final cleaned-scene probe: PASS; C# compiler warnings/errors 0, Shader warnings 0, runtime errors 0. Missing Script scan: 0.
- Earlier script-reload/build render probes each logged 414 existing render-pipelines.core D3D11 path-tracing/lightmap precision warnings. Package shaders were not changed. After restoring build-generated pipeline/settings changes, the final clean probe logged none; original diagnostic logs remain available.
- Android Development Build Report: Succeeded, warnings 0, errors 0. APK 149507151 bytes, built 2026-10-05 20:21:03. SHA256 4C709BDB62DAC2D6553A71D8C8F58539B44F7060B97840B2C5F07796E42C0F17.
- SM-S942N (R3KL20DY0KF), Android 16, 1080×2340: normal device, adb install -r Success, cold launch Success.
- Native Collection displays the centered title, new stage frame, five direct planet thumbnails, thin connectors, separate labels and Stage 5 current ring/pulse.
- Existing device state is Energy 400 / actual Stage 5 / Best 15930. Stage 1 and Stage 3 taps preview the corresponding Hero and lore while Stage 5 remains highlighted. Planet 02 remains locked. Back icon → HOME passed. Reentry reads actual Stage 5.
- HOME → PLAY passed: Score 0, empty Board, three Pieces, no immediate Game Over or stale milestone overlay. Playing HOME confirm → HOME → Collection also passed.
- Actual device prefs before/after: Planet01Energy 400→400, PlanetRestorationVersion 2→2, BestScore 15930→15930. No device presets were applied.
- Logcat: FATAL EXCEPTION/crash/AndroidJavaException/NullReferenceException/MissingReferenceException/UnityException 0; Unity warnings 0. One existing E Unity ClassNotFoundException for com.google.android.play.core.assetpacks.AssetPackManager remains and is reported separately as permitted by the instructions.
- Additional ADB KEYCODE_BACK injection did not return Collection to HOME. No PASS is claimed for system Back; gesture/hardware Back needs manual confirmation. The Back icon is verified. Input/navigation code was preserved as required; the previous Sprint also left this hardware check unconfirmed.
- Subjective readability/touch/SFX/haptic/transition feel remains a user check.
- Device captures: Validation/sprint8_1_device_collection.png, preview1.png, preview3.png, locked.png, home.png, android_back.png, game.png, reentry.png (each with sprint8_1_device_ prefix). The android_back image documents the unconfirmed input check, not a successful HOME return.
- Final Scene preservation: no changes outside Collection. Build-generated settings, temporary output and import whitespace changes were removed.
- Main files: Assets/Editor/Sprint8CollectionBuilder.cs, Sprint8CollectionPlayProbe.cs; Assets/Scripts/UI/PlanetCollectionView.cs, CollectionStageRing.cs (+meta); Assets/Scenes/Game.unity; user-provided collection_stage_frame.png; docs/CURRENT_TASK.md, DEVLOG.md, NEXT_TASK.md.

# Sprint 8.2 — Planet Collection Final Visual Polish

## Implementation and preserved scope

- User-provided Previous/Next PNGs replace the previous sprites. Hit rectangles and anchors are unchanged: Previous (0.01,0.56)-(0.16,0.665); Next (0.84,0.56)-(0.99,0.665). Their clear Image targets use rectangular raycasts; icons preserve their individual source aspect ratios. Disabled icon alpha remains 0.35.
- At 1080×1920, both button hit rectangles measure 162×201.6 px; centers measured from the screen top are (91.8,744) and (988.2,744). Tall Safe Area: 155.52×226.8; Short: 162×151.2. Visible icon width is 145.8 px at 9:16. Left/right artwork has slightly different native aspect ratios, so its aspect-preserved visible heights differ by about 0.43 px.
- New collection_progress_frame.png contains fixed Unity Text 복원도 (22), dynamic Warm Gold percentage (32), and the existing Unity Image track/fill. The actual PlanetRestoration.Percent drives both text and the existing anchor-width fill. The previous plain known-planet caption is hidden; locked-planet messaging remains.
- Energy 0 / 205 / 400 renders actual restoration 0% / 51% / 100%, with corresponding rendered fill width ratios 0 / 0.51 / 1. All three states use identical frame/text/track geometry. Preview does not change the actual percentage.
- Supplied collection_stage_frame2.png was renamed to the required collection_stage_frame.png, preserving all PNG bytes and the existing Sprite GUID. No new artwork was generated or processed.
- Stage container y anchors: 0.135–0.345 → 0.125–0.345 (+4.76%). The source canvas remains 2172×724 (3:1); the supplied new artwork has taller interior space. FitInParent preserves aspect ratio. Actual fitted frame height stays 334.96 px at 9:16 and 321.56 px at Tall (width limited); Short grows 302.4→316.8 px.
- Planet thumbnail fitted diameter grows about 8.9% in 9:16/Tall and 14.1% in Short. Thumbnail Image rectangles: 135.59×91.95, 130.17×88.27 and 128.24×86.96 px respectively. Original five planet sprites, actual-stage ring/pulse, future dim/locks and thin Gold connectors remain.
- The 9:16 thumbnail-to-number gap is about 25.2 px and number-to-name gap about 5.0 px. Planet/number/name areas are separated. Number font 18 and name font 25 are preserved; five centers remain equally spaced at 0.14/0.32/0.50/0.68/0.86 inside the stage frame. No collection_stage_slot sprite is rendered.
- Message center y moves 0.0675→0.10 of Safe Area height: upward 62.4 / 70.2 / 46.8 px across 9:16/Tall/Short. Original message sprite, lore, dimensions and font 27 remain. Visible stage/message frame edges remain separated in all tested layouts.
- Title, Name and Hero geometry are unchanged. No game rules, energy/save/migration/unlock rules, HOME design, Board/Piece, Combo, fragment/transition/SFX/haptic behavior were changed.

## Asset audit and import settings

All four supplied PNGs have RGBA alpha and truly transparent exterior pixels. Read-only inspection found no baked background. Source hashes remain unchanged after integration.

| Asset | Source dimensions | Alpha 0 / partial / 255 |
|---|---|---|
| collection_prev_icon.png | 1371×1147 | 730109 / 842428 / 0 |
| collection_next_icon.png | 1374×1145 | 679088 / 894142 / 0 |
| collection_progress_frame.png | 2172×724 | 876375 / 696089 / 64 |
| collection_stage_frame.png | 2172×724 | 420903 / 1151530 / 95 |

Sprite/Single; input alpha; alpha transparency enabled; no mipmaps; Bilinear/Clamp. Editor texture compression is Uncompressed. Android uses ASTC 6×6, quality 100; max 1024 for icons and 2048 for frames. Frame imports retain their aspect ratio (2048×683 after scaling).

## Automated and rendered validation

- All 21 regression entry points passed (Sprint 0–7.6.1 plus Collection and earlier touch/readability/hotfix probes). Missing Script scan: 0.
- Final Collection probe also invokes the actual Previous/Next button events, checks boundaries, measures real Fill/track widths, checks 0%/51%/100%, frame text separation, five planets, label spacing, ring/locks/dim state, actual-stage reentry and save/run isolation.
- Preview/navigation leave Energy, migration version, Best Score, run Score, Board and Piece supply unchanged. HOME/GAME and Release DEV visibility contract passed.
- Rendered and visually inspected requested stage1/stage3/complete/tall_safe/short images and three locked layouts. Safe Area, text clipping, title centering, planet/frame/message separation passed.
- Serialized Scene comparison normalizes regenerated Unity IDs by hierarchy and component type. All non-Collection scene blocks are unchanged.
- Evidence: Validation/sprint8_2_asset_audit.txt, sprint8_2_collection.txt, sprint8_2_regression_summary.txt, sprint8_2_scene_preservation.txt and sprint8_2_* logs.
- Required renders: Validation/sprint8_2_collection_stage1.png, sprint8_2_collection_stage3.png, sprint8_2_collection_complete.png, sprint8_2_collection_tall_safe.png, sprint8_2_collection_short.png. Additional locked_9x16/locked_tall_safe/locked_short renders use the same prefix.

Android Development Build: Succeeded, Build Report warnings 0 / errors 0. APK 149495326 bytes; SHA256 297557BB8DACC41462D3C75F1EFEBBC0C85C23C808B432EC3F37C34EBBEAE06B. Final/post-build Collection probes passed. Final clean diagnostics and device results follow below.

## Final diagnostics and SM-S942N verification

- Final cleaned-scene Collection probe: PASS; C# compiler warning/error 0, shader warning 0, runtime error 0. Missing Script scan: 0. Earlier script-reload/post-build render logs contain 414 existing render-pipelines.core D3D11 precision-conversion shader warnings; package shaders were not modified. After removing build-generated pipeline/settings changes, the final clean probe logged none. Earlier logs remain available.
- SM-S942N R3KL20DY0KF, Android 16/API 36, physical 1080×2340: ADB device; adb install -r Success; cold launch Success. The phone was initially locked, then confirmed unlocked with the app in focus before UI testing.
- Native HOME → Collection shows Planet 01 Stage 1, 0%, a genuinely empty bar, the new navigation/progress/stage artwork, spaced stage labels and the closer Message frame. Source alpha is correct, with no opaque rectangular background.
- Tap (1040,890) in the Next rectangle above the visible icon successfully opens locked Planet 02, confirming that the touch target extends beyond opaque artwork. Navigation through Planet 05 stops at the disabled Next boundary; Previous returns through Planet 01. Disabled icons remain dim.
- Stage 2 touch at actual Stage 1 is blocked; current Hero/lore/ring remain Stage 1. Current-stage selection/reentry is preserved. Full past-stage preview across all five actual stages was verified automatically; no device presets were used to fabricate later stages.
- Back icon → HOME passed. HOME → PLAY shows Score 0, an empty Board and three Pieces, without Game Over or stale Journey feedback. Playing HOME confirmation → HOME → Collection returns to actual Stage 1 / 0%.
- Actual device save before→after: Planet01Energy 0→0; actual restoration Stage 1→1; PlanetRestorationVersion 2→2; BestScore 15930→15930. No save keys were reset or deleted.
- PID-scoped Logcat: Crash/FATAL EXCEPTION/AndroidJavaException/NullReferenceException/MissingReferenceException/UnityException 0; Unity warnings 0; new game-code errors 0. One existing E Unity ClassNotFoundException for com.google.android.play.core.assetpacks.AssetPackManager remains, separated as explicitly allowed by the Sprint instruction.
- Required native image: Validation/sprint8_2_device_collection.png. Other device images: sprint8_2_device_home.png, future_locked.png, locked.png, planet05.png, back_home.png, game.png, home_confirm.png and reentry.png (each with sprint8_2_device_ prefix).
- Device evidence: Validation/sprint8_2_device_status.txt, device_install.txt, device_launch.txt, device_logcat.txt, device_prefs_before.txt and device_prefs_after.txt (each with sprint8_2_ prefix).
- Subjective finger feel/display readability/SFX/haptic/transition feel remains user confirmation. System gesture/hardware Back was not retested in this visual-only Sprint; the previously unconfirmed manual check remains.
- Final PNG hashes match the initial supplied files. Scene preservation audit: zero changes outside Collection. Build-generated settings/import whitespace/resources/temp output were removed.
- Changed files: Assets/Editor/Sprint8CollectionBuilder.cs and Sprint8CollectionPlayProbe.cs; Assets/Scripts/UI/PlanetCollectionView.cs; Assets/Scenes/Game.unity; collection_prev_icon.png, collection_next_icon.png, collection_stage_frame.png, collection_progress_frame.png (+new meta); docs/CURRENT_TASK.md, DEVLOG.md and NEXT_TASK.md.

# Sprint 8.2.1 — Collection navigation button fine tuning

- Previous/Next visual RectTransforms enlarge exactly 18% in both dimensions, with both button centers unchanged. Source PNGs and preserveAspect are unchanged; matching UI rectangles preserve each sprite's slightly different native aspect ratio.
- 1080×1920 visual Rect: 145.80×181.44 → 172.04×214.10 px. Actual aspect-fitted sprite width 145.80→172.04 px. Hit rectangle: 162×201.60 → 176.04×237.89 px.
- 1080×2400 with Safe Area visual Rect: 139.97×204.12 → 165.16×240.86 px; hit 169.00×267.62 px. 1080×1440 visual Rect: 145.80×136.08 → 172.04×160.57 px; hit 176.04×178.42 px.
- Previous center remains x=0.085, Next x=0.915, both y=0.6125 of Safe Area. No X/Y center changes. Expanded hit bounds: center ±0.0815 in X, y 0.55055–0.67445. The final hit width was tightened after a conservative Tall rendered-planet disk test detected overlap in the first candidate. Visual size and centers stayed unchanged.
- Automated checks measure both visual Rects and exact scale; confirm full visual coverage by hit Rect, Safe Area bounds, unchanged centers, aspect preservation and separation from the rendered planet disk. All three responsive sizes and locked layouts pass.
- Existing Collection probe passes all five actual stages, Preview/save isolation, navigation boundaries and button events, Planet 02–05 locks, Back/HOME/GAME flow. Missing Script: 0. No navigation, save, restoration or gameplay code changed.
- Serialized Scene audit: every block outside Previous/Next and their Icon RectTransforms is unchanged. Builder uses a navigation-only helper; Back button builder is untouched. No PNG changes.
- Evidence: Validation/sprint8_2_1_collection.txt, sprint8_2_1_scene_preservation.txt and sprint8_2_1_* logs/renders. Render names include collection_stage1/stage3/complete/tall_safe/short and locked variants.

Android Development Build succeeded with Build Warning/Error 0. APK 149495326 bytes, SHA256 0CCA5FFA3DBBC242B02F5A606CF5478DD55D24393E0580756B1DCAC1F01099E1. Post-build and final clean probes pass; final compiler warning/error 0, shader warning 0, runtime error 0, Missing Script 0. Earlier post-build rendering logged the existing 414 package shader precision warnings; no package changes were made, and final clean diagnostics contain none.

SM-S942N (Android 16, 1080×2340): adb install-r Success and cold launch Success. Native Collection shows larger arrows with clear planet separation and no clipping. Expanded hit tap (1040,890) opens Planet 02; Previous/Next through Planet 05 and disabled boundary pass. Stage 1 preview changes Hero/lore while actual Stage 2 remains highlighted and restoration remains 28%. Back→HOME→PLAY starts Score 0 with three Pieces and no stale Game Over; Playing HOME confirm→Collection returns actual Stage 2. Device save before/after: Energy 110→110, actual Stage 2→2, Best 15930→15930, migration version 2→2. No presets or save edits were performed.

PID Logcat: crash/game exceptions 0 and Unity warnings 0. One existing AssetPackManager ClassNotFoundException remains, reported separately. Physical finger comfort and subjective visual balance remain user confirmation; ADB touch navigation and screenshot bounds are verified. Device images and preferences/log evidence use Validation/sprint8_2_1_device_* paths.

Changed files: navigation geometry in Game.unity and Sprint8CollectionBuilder.cs, geometry assertions/output paths in Sprint8CollectionPlayProbe.cs, and three task documents. All other Scene blocks and original PNGs are unchanged. Build-generated settings/resources/temp output were removed.

# Sprint 9 — GAME Visual Rebuild

## Scope / source assets / import

All nine final user PNGs are used separately, without pixel edits, stretching or baked dynamic numbers. Source RGBA/transparent exterior pixels were inspected and hashes recorded in Validation/sprint9_asset_audit.txt.

| Source | Dimensions | Application |
|---|---|---|
| game_home_button.png | 1341×1173 | Existing PlayingHomeButton / confirmation flow |
| game_best_score_frame.png | 2019×779 | BestFrame with dynamic TMP value |
| game_planet_status_frame.png | 2018×779 | Planet Status decoration behind actual stage sprite/data |
| game_board_frame.png | 1299×1211 | Separate BoardFrame sibling, original aspect ratio |
| game_empty_cell.png | 1254×1254 | Existing 64 Cell Images |
| game_block_blue.png | 1254×1254 | Blue Piece cells and matching placed Board cells |
| game_block_purple.png | 1254×1254 | Purple Piece cells and matching placed Board cells |
| game_block_gold.png | 1254×1254 | Gold Piece cells and matching placed Board cells |
| game_piece_slot.png | 1283×1226 | Same three existing whole-slot drag targets |

Sprite/Single and FullRect; input alpha / alpha transparency; Bilinear/Clamp; no mipmaps. Editor Uncompressed; Android ASTC 6×6 quality 100, max 1024 for tile/button/slot and 2048 for frames. PNG source bytes are unchanged.

The project previously used Legacy Text, despite the document assuming existing TMP. Dynamic score/Best use the TMP font/settings/shader resources bundled with the already-installed uGUI package, including the LiberationSans OFL license. No external package or game feature was added. Existing hidden Legacy score fields remain for compatibility with GameHud and old regression probes.

## Layout / rendering / preservation

- HOME icon remains bound to the existing confirmation flow; sufficient rectangular hit target. No Retry/save reset is called by the new artwork.
- SCORE is centered above a larger current value, without another frame. Best has its own image at the upper right. Values are formatted with invariant thousands separators and TMP auto sizing.
- Planet Status uses the existing current/next stage sprites and restoration presentation. Overall % follows PresentedEnergy, preserving fragment-arrival timing. Existing stage-local Bar, overflow and transition rules remain; only bar/label geometry and rounded rendering changed.
- Board is still the same Grid/BoardView with 64 immediate Cell children. Decorative BoardFrame is a sibling, not a source of cell coordinates. SquareBoardLayout uses min(SafeWidth×0.66, SafeHeight×0.36), center (0.5,0.415); frame height is BoardSide/0.74 with its source ratio. Tested Board sides: 691.2 / 684.288 / 518.4 at 9:16/Tall Safe/Short.
- BlockArea uses anchors (0.02,0.02)-(0.98,0.16), equal existing Slot roots, gap 20 and horizontal padding 8. Original shape sizing rules, whole-slot hit forwarding, offset 110 and drag scale 1.05 remain.
- Empty cells have restrained tint; placed cells preserve the Piece's Blue/Purple/Gold sprite. Normal extra outlines/shadows are disabled for sprite skins. Valid Warm Gold / invalid Muted Red preview and the existing Gold dragging outline remain.
- Existing ThemeFill decorations under GAME HOME/Planet Status were removed because they covered the new art. HOME/Collection ThemeFill objects were not touched.
- Game Over and HOME confirmation siblings draw above GAME HUD/frames. Existing Clear effect/particle/Score Pop/Combo pools and Planet central transition objects/data remain unchanged.
- BoardModel, Shape pool, generator, score/combo/energy/save/game-over rules, DragHandler/SlotDragHandler, clear timing/SFX/haptic and PlanetRestorationView transition code are unchanged. GameSession only forwards appearance to BoardView after valid model placement, before clearing; it does not change validation, state, rewards or supply order.
- Scene comparison by hierarchy/component with normalized local IDs: all HOME/Collection/clear feedback/planet flight/transition/confirmation/Game Over blocks unchanged. Two Collection icon anchor literals were restored after Unity rounded their floats during scene save.

## Validation (final build/device results follow)

Initial and final-presentation regression suites execute all 22 entry points (Sprint 0–8.2.1 / earlier hotfix probes / Sprint 9). Sprint 9 checks all catalog shapes via actual slot-edge drag events, matching palette cells, invalid drop isolation, three-piece refill, 64-coordinate roundtrips per resolution, TMP glyph bounds at 999 / 15,930 / 999,999 / 9,999,999, HOME/Collection/GAME, Game Over/Retry and save isolation. Final responsive and Android diagnostics are recorded below when complete.

Evidence: Validation/sprint9_game.txt, sprint9_regression_summary.txt, sprint9_scene_preservation.txt and sprint9_* logs. GAME renders: sprint9_game_9x16.png, game_tall_safe.png, game_short.png, game_placed.png and game_over.png (each prefixed sprint9_). Additional value/drag/feedback captures are being generated.

## Final validation / Android (2026-10-06)

- All 22 regression entry points PASS on final presentation, including Sprint 0–8.2.1, earlier UX/flow/Journey fixes and Sprint 9. Final Sprint 9 probe: 1,794 assertions. All catalog shapes use actual pointer/slot events; 64 coordinate roundtrips at each captured resolution; invalid drop, consumption/refill, palette placement, score/Best glyph bounds and clean flows PASS.
- Responsive 1080×1920 / 1080×2400 + Safe Area / 1080×1440 PASS: square Board sides 691.2 / 684.288 / 518.4, all 64 cells aligned, no clipping/overlap/Safe Area intrusion. Actual glyph bounds pass at 999 / 15,930 / 999,999 / 9,999,999. Clear row+column highlights align with their real cell geometry.
- Source SHA-256 verification: all nine PNG files unchanged. Normalized protected scene comparison: 0 changed blocks (HOME/Collection/Clear feedback/Planet energy flight/central transition/confirmation/Game Over). Temporary build settings/URP serialization and performance artifacts were cleaned up.
- Final Compiler Warning/Error 0; final clean Editor Shader Warning 0; Runtime Game Error 0; Missing Script 0. Initial TMP native compilation recorded 3 large-method splitting notices (TMP_TextParsingUtilities.cctor and GenerateTextMesh in TextMeshPro/TextMeshProUGUI). They describe C++ compilation optimization, not C# failures. Same-code incremental Android build: Succeeded, Warning 0, Error 0. Intermediate postbuild D3D11 import recorded 414 existing URP precision warnings; retained in logs, final clean confirm run 0. No package edits or warning filters.
- Final APK: Builds/Android/Development/CosmicBlock-dev.apk; 151796683 bytes; SHA256 207638CB6E8D6C474E7ECC2192AF5078019605B3261A77ACEA76907A4C325F2A. First native compile took about 22 minutes; final incremental build reused native outputs.
- SM-S942N / R3KL20DY0KF / Android 16 API 36 / 1080×2340: authorized device, adb install -r Success, cold launch and foreground activity PASS. Package name unchanged. No uninstall, preset/reset, migration or development gameplay hooks were used on the phone.
- Actual ADB drag QA: invalid outside-board drop returns, then 23 placements starting at slot edges. First 22 screenshot boards match predicted 64-cell state. One Row Clear produces score 260 at step 6 and Energy 290→300, Stage 3→4 with next-stage energy 0/100. Final score 690. Final remaining 2x2 has zero legal positions; Game Over overlay verified visually (overlay naturally occludes the screenshot board classifier on step 23).
- Real Retry: Score 0, empty Board, three Pieces, no stale overlay. HOME confirmation Cancel/Confirm, HOME→Collection, Next→Planet 02 Locked, Prev→Planet 01, Back→HOME→GAME clean run PASS. Device left in GAME for user review. Hardware/gesture Android Back and subjective feel are not newly certified by this visual Sprint.
- Save before/after install: Best 15930 / Planet Energy 290 / restoration version 2 unchanged. After actual one Clear: Best 15930 / Energy 300 / version 2, preserved through Game Over/Retry/HOME/Collection/new GAME. Increase is the existing +10 reward, not save alteration by visual code. Editor probe snapshots and restores original save keys.
- PID-scoped cold-launch-to-final Logcat: crash/fatal 0, managed/new game exception 0, Unity warning 0. One pre-existing AssetPackManager ClassNotFoundException at startup remains separately documented; no new exception or crash follows. Physical touch comfort, readability and SFX/haptic/transition feel remain user confirmation.
- Evidence: sprint9_regression_summary.txt, sprint9_game.txt, sprint9_scene_preservation.txt, sprint9_device_validation.txt, sprint9_device_moves.json, sprint9_device_prefs_{before,after_install,after_gameplay,after_home,final}.txt, sprint9_device_logcat.txt. Build logs: sprint9_android_build.log (initial 3 notices), sprint9_android_incremental.log (0/0). Editor final: sprint9_confirm_clean.log (0/0).
- Renders: sprint9_game_9x16.png, sprint9_game_tall_safe.png, sprint9_game_short.png, sprint9_game_placed.png, sprint9_game_over.png; best_999/15930/999999/9999999, drag_valid/drag_invalid/clear_feedback with sprint9_ prefix. Android: sprint9_device_game.png, first_placement.png, move_6.png (Stage 4), move_23.png (Game Over), retry/confirm/cancel/collection/locked/final with sprint9_device_ prefix.
- Changed runtime files: BoardView.cs, BlockPiece.cs, GameSession.cs (appearance forwarding only), SquareBoardLayout.cs, new GameVisualPresentation.cs. Scene: Game.unity. Editor: new Sprint9GameBuilder.cs and Sprint9GamePlayProbe.cs. Art/import: nine Game PNGs/metas, essential TMP resources/fonts/license. Documentation: README and docs/CURRENT_TASK, NEXT_TASK, DEVLOG. Git uses the existing origin/main with ordinary commit/push after checks.
# Sprint 9.1 — GAME Layout Match & Polish (2026-10-06)

## Reason / scope

The Sprint 9 art was installed, but large decorative insets, detailed Planet labels and vertical whitespace made the actual Board small. The supplied reference is the layout target. GAME presentation is adjusted; HOME/Collection, rules, save, restoration thresholds/overflow/cinematic, clear feedback/SFX/haptic/drag are preserved. No new PNGs, hint, advertisement, gameplay Retry or other content.

## Layout and source preservation

- Board at 1080×1920: (194.4,777.6), 691.2² → (76,562), 928², +34.26%. Initial 20% scale was insufficient against the reference and overlapped original frame insets; the final layout gives the real grid the main visual area.
- Slot 0: (29.6,1612.8), 326.933×268.8 → (18.8,1560), 336.8×324 (+3.02% W / +20.54% H). Same-sized roots with full-slot input, 16 spacing / 8 padding. Aspect-fit sprite area 281.3×268.8 → 336.8×321.84, +19.73% linear. Rest piece size limit 64→78; actual standard cell 64→77.76, retaining shape/padding/drag rules.
- Planet Status: (32.4,245.65), 1015.2×391.9 → (21.6,232), 1036.8×260, height −33.66%. Stage and Energy legacy Text rendering disabled; data and view callbacks remain. Name/current-stage planet/overall % and existing stage-local Bar remain.
- Real Board gaps: Planet→Board 140.05→70, Board→Slots 144→70. Decorative frame gaps are 16. SCORE max font 70→106 (+51.43%), Bold, warm gold, centered; label 26→32. HOME/best remain the existing actions/assets. Large values are checked through 9,999,999.
- GameFrameMesh uses source UV regions on existing Sprite images. Board: 5×5 patches keep corners/center ornaments isotropic; straight connecting strips stretch. Planet frame: horizontal patches preserve the circular left ornament and caps at the same scale. No source pixels/alpha/import settings/new bitmap are produced. Board frame stays a separate sibling behind the original Grid, noninteractive; all 64 cells and pointer conversion remain unchanged.
- Layout fits the Safe Area and vertically packs header/status/frame/slots with bounded gaps. Tall does not separate the Board and Slot group with large gaps; spare height remains below it. Requested Short stays square and unclipped. No simulated Retry/hint footer from the reference was added.

## Tests / known harness issue

All 23 regression entry points pass (Sprint 0–9.1 plus earlier hotfix/UX probes). Sprint 9.1 has 2,099 assertions: actual whole-slot events, all catalog shapes, all palette placements, invalid return/consumption/refill, 64 coordinate roundtrips per capture, all text sizes, clean HOME/Collection/GAME/GameOver/Retry and save isolation. Missing Script 0. Responsive board sides: 928 / 891.648 / 696 at 9:16 / Tall Safe / Short; gaps 70 / 67.2 / 52.5. All required images directly inspected, including reference fixture and 9,999,999.

After a shorter text replaced the display fixture, the old TMP fit helper checked unused capacity entries in characterInfo. They can retain old visible flags/geometry although TMP does not draw them. Both Sprint 9 and 9.1 helpers now check only characterCount; glyph bounds/truncation requirements are unchanged. This corrects test observation without relaxing text fit or changing production text parsing.

All nine source PNG SHA-256 hashes match Sprint 9. Protected scene blocks (HOME/Collection/clear feedback/planet flight/central transition/confirmation/Game Over) have zero changes after preserving Unity-rounded Collection anchor literals. Android/final diagnostics/save/Git results follow below.
## Final Android / diagnostics / delivery

Development APK build succeeded. Initial native compile emitted three existing TMP C++ large-method splitting notices; incremental final build emitted 0 warnings / 0 errors. Post-build first Editor shader import emitted 414 existing URP precision warnings; the subsequent clean final probe emitted 0. No package, shader or warning suppression changes were made. Compiler warning/error 0; final clean Editor runtime warning/error 0; Missing Script 0.

APK: Builds/Android/Development/CosmicBlock-dev.apk, 151807714 bytes, SHA-256 F45FF429C82CB0F469603DAA0FF673FCA41D70754CD4E1B64221FEB14D654645. Connected R3KL20DY0KF / SM-S942N / Android 16 API 36: install -r Success, cold launch Success. Native resolution 1080×2340. Source saves before/after install: Energy 335, Best 15930, migration 2. Six successful natural placements (Score 260), one Line Clear (+10 Energy), Piece refill, invalid return, single-cell drag preview and valid drop were visually confirmed. A sequential driver final drop did not place; a later identical swipe placed normally. The precise reason for the first rejected input was not established. The failed attempt is retained in Validation evidence; it is not counted as a successful placement. No game logic or timing was changed for the driver. Phone screen doze was resolved with the ordinary wake key, without changing device settings.

Native HOME confirmation Cancel, confirm HOME, then PLAY verified: Score 0, empty Board, three Pieces, no stale Game Over. Permanent save remained Energy 345 / Best 15930 / migration 2. Device Game Over/Retry is covered by automated regression rather than claimed as a newly completed native flow. Subjective touch/sound/vibration quality remains user confirmation.

PID-scoped startup/run log: existing AssetPackManager ClassNotFoundException once; FATAL EXCEPTION/AndroidJavaException/NullReferenceException/MissingReferenceException 0; new game error 0; Unity warning 0; crash 0. Validation/sprint91_device_logcat.txt retains the exception, so total device error log count is not claimed as zero.

Changed files: GameVisualPresentation, SquareBoardLayout, BlockPiece (presentation cap only), GameFrameMesh, Sprint91GameBuilder/PlayProbe and their metas, Sprint9GameBuilder helper access, Sprint9GamePlayProbe TMP observation helper, Game scene, README and docs/CURRENT_TASK/NEXT_TASK/DEVLOG. Original nine PNG hashes unchanged; protected scene block diff 0. Final screenshots include sprint91_game_empty/placed/tall_safe/short, reference_1280 and native drag_preview/retry_drop/home/new_run; before/reference/after comparison is sprint91_visual_comparison.md.

Delivery commit title: fix: align game layout with visual reference. Normal origin/main push; no remote changes, force push or history rewrite.

# Sprint 9.1.1 — Board & Piece Density Polish (2026-10-06/07)

Baseline 941f48b. The attached target/current screenshots showed wide black gaps despite the enlarged Board. Changed only Cell/Block artwork mesh size and Slot internal fitting. Board/Frame/Slot outer Rect, all Cell RectTransforms, Planet/Score/Home/Best layout, vertical group spacing, source PNGs, logical centers, pointer conversion, shape offsets, rules, HOME/Collection and saves remain.

## Before / After at 1080×1920

Source alpha >=20/255 bounds; exclude near-transparent glow. Cell center pitch 115.5→115.5, logical Cell Rect 103.5²→103.5². Visible Empty 86.91×83.36→104.29×100.03; Blue 83.03×79.23→99.64×95.08; Purple 92.19×85.26→110.63×102.31; Gold 92.19×86.17→110.63×103.40. Width/pitch: Empty 75.25%→90.30%, blocks 71.89–79.82%→86.27–95.78%. Artwork expansion is 20%, all Sprite aspects unchanged. Actual static alpha pixel gaps remain positive at every resolution.

Frame inner cyan rim to first visible Empty pixel X/Y 42.34/39.57→33.65/31.14. Frame outer Rect to pixel X/Y 70.34/71.57→61.65/63.14. Rim sampled on the before/after renders (inner boundary x=50/y=540); original Alpha bounds determine Cell pixel bounds. Frame/Grid padding and all centers remain exactly unchanged: visible growth reduces the remaining padding without moving coordinates.

Slot Rect 336.8×324 unchanged. Prior effective common 3-cell envelope 245.28²; final allocated Available Rect 276.18×265.68. Rest logical cell 77.76→84.56, existing 6 spacing. H3 visible bounds: width 229.90–236.78→262.52–271.51, height 59.53–64.74→77.68–84.48. V3: width 62.38–69.26→81.40–90.39, height 227.05–232.26→258.80–265.60. Ranges reflect the three original palette PNGs. H3 uses about 78–81% of Slot width, V3 80–82% of height. No shape-specific scale; available 82% root Rect uses min dimension fit after spacing, capped by actual maximum catalog bounds for a common cell size. Single remains proportional. The initial 90% trial caused V3/frame overlap; direct render inspection led to 82%.

## Implementation / validation

CellSpriteDensity expands UI mesh around existing centers. BoardView serializes/configures 1.20 and BlockPiece applies the same effect to Piece cells. This is vertex presentation, no bitmap edits/import changes. BlockPiece common fit uses actual Shape/Catalog Bounds. BlockDragHandler, SetGeometry/OriginWorld coordinate math, SlotDragHandler full-slot policy, consumed guards, scale 1.05, offset, Warm Gold/Muted Red preview and Gold outline remain. Builders only configure existing scene components.

All 24 regression entry points PASS (previous 23 plus Sprint911). Final alpha probe 12,371 assertions PASS, 8 shapes at standard/Tall Safe/Short, 64 coordinate roundtrips, actual Alpha pixel bounds inside Frame/Slots and positive horizontal/vertical gaps, palette/valid-invalid/consumption/refill, Clear/score/combo/restoration, HOME/Collection/GameOver/Retry and save isolation. Existing probes cover row/column/simultaneous clear and restoration timing. Directly inspected before/reference/after, H3/V3, Tall and Short renders. Protected HOME/Collection/feedback/modal scene diff 0; GAME and all Cell RectTransform diff 0; 9 PNG SHA-256 hashes unchanged. Missing Script 0.

Final incremental BuildReport: Succeeded, warnings 0/errors 0. C# compiler warning/error 0. Final clean Editor shader/runtime-game warning/error 0. First native build had 3 unchanged TMP large-method C++ splitting notices; first post-build shader import had 414 existing URP precision warnings, followed by clean zero-warning runs. No package changes or warning suppression.

## Android / deferred physical QA

SM-S942N / R3KL20DY0KF / Android 16 connected as device. Install -r Success; cold launch and HOME→GAME Success. Energy 0 / Best 15930 / migration 2 before and after install, unchanged. The existing Development APK was updated without uninstall/reset/DEV presets. APK SHA-256 40EF7E037D796BEF5A2DCC329921E95A77654B89B1DF4AC50131D4937652B9B3; actual APK 151823240 bytes (BuildReport total includes symbols).

Native QA: 12 matching drops from multiple Slot corners/blank areas over H3/H2/V2/V3/L/Reverse L; targets include Board top-left/bottom-left/bottom-right edges. Four HOME→PLAY cycles completed. An initial swipe from the extreme screen edge (31,1682) did not begin drag; (100,1720), another blank area in the same Slot, placed H3 correctly. Cause of the extreme-edge rejection was not conclusively established; it is not claimed as a full extreme-edge PASS, and input/system settings were not changed. Subsequent four interior-corner patterns all matched.

USB disconnected before Single/Square2×2 and final Logcat collection. User replied that reconnection is currently unavailable. These native checks remain deferred; they passed automated tests. No crash was observed during launch/12 drops, but final native Logcat zero-error is not claimed. Observed Android DexFile finalizer AssertionError also appears in Sprint 9 and 9.1 logs; AssetPackManager is a previously known environment exception. System exceptions are separate from Editor runtime-game zero errors. No game/system exception was hidden or worked around. Subjective touch/SFX/haptic feel remains user confirmation. Before/after install saves are verified; post-touch preferences could not be re-read after disconnection.

## Files / delivery

Modified: Assets/Scripts/Board/BoardView.cs, Assets/Scripts/Blocks/BlockPiece.cs, Assets/Scenes/Game.unity, README.md and docs/CURRENT_TASK/NEXT_TASK/DEVLOG. Added CellSpriteDensity.cs, Sprint911GameBuilder.cs, Sprint911GamePlayProbe.cs and their metas. No GameVisualPresentation/SquareBoardLayout/SlotVisual/BlockDragHandler/SlotDragHandler/core rule/save files or source art changed.

Evidence: Validation/sprint911_game.txt, regression_summary, measurements.json, sprite_bounds.json, layout_preservation.txt, protected scene preservation and all before/after/shape/responsive/native screenshots. Native edges JSON records 12 successful matches; failed first-edge screenshot retained. Comparison: Validation/sprint911_visual_comparison.md.

Commit title: fix: tighten board and piece spacing. Normal origin/main push only; no remote change, force push or history rewrite. Physical checks above remain pending due user-confirmed device unavailability.


## 2026-10-07 — Sprint 9.2 Line Clear & Combo Juice

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

Modified/added files:

- Assets/Art/UI/Game/game_block_blue.png
- Assets/Art/UI/Game/game_block_blue.png.meta
- Assets/Art/UI/Game/game_block_gold.png.meta
- Assets/Art/UI/Game/game_block_purple.png.meta
- Assets/Art/UI/Game/game_clear_starlight.png
- Assets/Art/UI/Game/game_clear_starlight.png.meta
- Assets/Art/UI/Game/game_empty_cell.png.meta
- Assets/Editor/Sprint2Builder.cs
- Assets/Editor/Sprint2PlayProbe.cs
- Assets/Editor/Sprint3PlayProbe.cs
- Assets/Editor/Sprint4Builder.cs
- Assets/Editor/Sprint4PlayProbe.cs
- Assets/Editor/Sprint65PlayProbe.cs
- Assets/Editor/Sprint66PlayProbe.cs
- Assets/Editor/Sprint92Builder.cs
- Assets/Editor/Sprint92Builder.cs.meta
- Assets/Editor/Sprint92PlayProbe.cs
- Assets/Editor/Sprint92PlayProbe.cs.meta
- Assets/Editor/Sprint92ProbeSupport.cs
- Assets/Editor/Sprint92ProbeSupport.cs.meta
- Assets/Scenes/Game.unity
- Assets/Scripts/Board/BoardModel.cs
- Assets/Scripts/Board/SequentialClearPlan.cs
- Assets/Scripts/Board/SequentialClearPlan.cs.meta
- Assets/Scripts/Core/GameSession.cs
- Assets/Scripts/Core/ScoreRules.cs
- Assets/Scripts/Effects/GameFeedbackController.cs
- README.md
- docs/CURRENT_TASK.md
- docs/DEVLOG.md
- docs/NEXT_TASK.md

Android haptic API: https://developer.android.com/reference/android/os/VibrationEffect.html — createOneShot(20ms, DEFAULT_AMPLITUDE), API26+.

Delivery commit: feat: improve sequential line clear and combo feedback. Normal origin/main push; no force push or remote change. Device QA remains pending because USB is unavailable.


## 2026-10-07 — Sprint9.2.1 Starlight Impact Polish

# Sprint 9.2.1 — Starlight Impact Polish

Baseline:3e5d498. Runtime changes are limited to GameFeedbackController star size/pop/movement/bounds. Scene, sprites/imports, Block size, all layouts, source PNGs, core gameplay/Score/Combo/Energy/save code, ClearCell animation, SFX/Haptic and CLEAR/Combo/Score text animations remain unchanged.

Before: Rect22–38 UI units (uniform random; sample mean30.02676), Scale0.7→1.1 without shrinking, overall lifetime0.45–0.75s (0.10 onset+0.35–0.65 visible), radial35–85+up35, twelve per line, pool48.

After: Base59 UI units × random0.75–1.35. Three small0.75–0.90 (44.25–53.10), five medium0.90–1.15 (53.10–67.85), four large1.15–1.35 (67.85–79.65), spread across the source Line. Seeded sample mean62.26365. Scale0.65→1.15 over0.10s, then1.15→0 until unchanged lifetime ends, with original alpha fade. Sample peak mean33.02944→71.60319,2.168x. Isolated star rendered at this representative peak size on black at1080×1920 gives RGB>=20 bounds20×24→46×53 pixels: width2.30x/height2.208x. This threshold measures displayed pixels, not raw source alpha.

Movement vector is exactly1.35x (same seeded directions verified), colors/random rotation/source-cell origins unchanged. Expanded stars remain in Board neighborhood (+0.35 CellSize allowance), Safe Area and below Planet HUD, using the full rotated Rect extent. This avoids clipping from the old fixed30-unit margin. No RectTransform/layout changes, new assets, particle count increase or runtime object creation.

Before/After fixture uses identical Row/Column and seeded particles, fixed Slot shapes/colors, and identical elapsed0.20s on the latest Line (previous Lines offset0.16). Pixel measurement uses unrotated representative peak size with all other graphics hidden. Full/cropped comparison images are validation output; source art is never processed.

Editor visual probe:409 assertions PASS. Single Row/Column, Double Row and Three Column, top/right/bottom edges and all requested resolutions captured. Large4/Medium5/Small3 verified per burst; Scale0.65 start/1.15 peak, unchanged lifetime, exact movement multiplier, pool return under1s and hierarchy stability pass. 36 active pooled stars are warmed and held while invoking the animation Update500 times:calibrated Mono heap delta0/no GC collections; sample0.1588826ms/update on this PC (Before0.12666ms). This measures the effect animation loop, not entire-frame/native GPU performance; real mobile FPS/feel is verified/reported separately.

Minimum gameplay regression:9 existing entrypoints all PASS (Sprint92/2/3/5/NewRunGameOver/65/66/911/8). Covers start/supply/Drag/Preview/drop/sequential clear/score/Combo/Best/Energy/Stage/refill/GameOver/Retry/HOME/Collection/save and all8 shapes. Missing Script0. All source Art PNG hashes unchanged; Scene/import changes0; protected ClearCell/SFX/Haptic/text blocks byte-identical.

Responsive1080×1920,1080×2400 with Safe Area,1080×1440: geometry/capture checks PASS, including star corner containment and existing Board/Slot/HUD constraints. Combo and Score text remain readable in the direct Before/After renders.

Android SM-S942N / Android16: existing latest Development APK rebuilt, install -r/launch successful. Before/after install Energy290/Best15930/Migration2 identical. Native8 shapes/34 placements and7 cleared Lines (five Singles, one Double Row) match independent Board/Score/Energy simulation. Final SCORE1530, Energy365; Stage3->4 transition/overflow completed naturally. Best15930/Migration2 unchanged. HOME confirmation cancel/confirm and clean PLAY leave Score0, empty Board, three Pieces and no stale feedback; app remains open for user testing.

SurfaceFlinger app-layer timestamps: idle median29.945FPS; six Clear samples (20 intervals each, including Double Row) median29.960–29.966FPS; maximum sampled Clear interval33.744ms versus idle34.184ms. No measured pacing degradation in these windows. These are presentation timestamps, not a full CPU/GPU trace. Native three-line/cross and subjective brightness/rhythm/haptic comfort remain pending; they are not labeled PASS. Editor three-line/36-particle pool and rendering checks pass.

GC measurement caveat: GC.GetAllocatedBytesForCurrentThread returned0 even for a1MB calibration allocation on this Unity Mono runtime, so that counter is unsupported and not treated as proof of allocation-free execution. Profiler.GetMonoUsedSizeLong correctly detected1,052,672 bytes from the control; warmed36-star loop500 calls had heap delta0 and collection count delta0. This supports no observed GC spike in the measured animation loop, not a blanket claim about all Unity/native frames.

Final Development build Warning0/Error0. Initial native build has3 existing TMP large-method splitting notices. First postbuild import has414 known URP shader precision warnings; final clean probe C# Warning0/Compiler Error0/Shader Warning0/Runtime Game Error0, Missing Script0. No warning suppression/package changes. PID-scoped native Logcat has no FATAL/AndroidJavaException/NullReferenceException/MissingReferenceException/new Unity game error/short-haptic warning. Existing AssetPackManager ClassNotFoundException and DexFile finalizer logs are separated from game exceptions; total Logcat is not claimed zero-error.

Modified: Assets/Scripts/Effects/GameFeedbackController.cs; added Assets/Editor/Sprint921PlayProbe.cs and meta; docs/CURRENT_TASK/DEVLOG/NEXT_TASK. No Scene/Art/import/other runtime changes.

Evidence: Validation/sprint921_before_after params/tests and isolated render, starlight_before_after.png, three_before_after.png, row/column/edge/responsive captures, regression_summary, missing.log, android_build[_final].log, postbuild/clean_probe.log, protection.txt, source_hashes.json, device install prefs/moves/burst/newrun screenshots, final_result.json, native_fps.json, device_logcat_final.txt and package-filtered vibrator log. Comparison renders are generated QA artifacts; original PNGs are unchanged.


Commit: fix: enlarge line clear starlight effect. Normal origin/main push only; no force push/remote change.

## Sprint 9.3 — Planet Progression & Feedback Polish (2026-10-08)

Planet01 total1500; stage requirements150/250/350/450/300, starts0/150/400/750/1200, names 황폐/싹틈/깨어남/회복/완성. Stage5 entry1200 is not completion;1500 alone completes and permanently discovers Planet02. Planet02 independent2000 data/save (200/300/450/600/450) without new art/play. Percent=floor(total/1500*100), Bar uses exact normalized total. Stage numeric uses current-stage-start/required. Existing v2 raw progress, global Best and legacy500->400 migration preserved; over1500 clamps. Clear rewards unchanged10/25/45/70. Durable award occurs immediately after validated frozen clear; presentation waits for fragments, cancellation cannot double-award.

Burst Rect59->80, RGB20 representative46x53->63x71px, original12-per-line/pool48/mixed sizes preserved. Flight Rect140/render91x104, duration.72/stagger.022, arrival pulse1.07/.25, bar.30. Central stage animation design/timing remains. Sequential interval.16->.32; measured clean.320549/.321091, native haptic request spacing~.328. One20ms request per Line preserved. Existing Y/X order, tie Row first, unique intersection removal, score100/250/450/700 and placement10/cell unchanged.

HUD BEST Frame330x110->412.5x137.5 (+25%), maxnumeric font37->53.65 (+45%), AutoSize. Up/right positioning within SafeArea and Current SCORE margin baseline8 units lower avoid crown overlap; Current SCORE Rect/font106 and total inset unchanged.24 actual RGB20 pixel-mask combinations at1080x1920/2400SafeArea/1440 have overlap0. Source Art/imports, Board/Piece/Slot dimensions untouched.

Regression27 entrypoints PASS across Sprint0~9.1.1/new93; final header follow-up93/91/911/92 allPASS. New progression212 assertions, visual351, existing92 gameplay2125.11 P1 boundaries, variable stage needs, instant save/delayed render, overflow140+25->Stage2 current15/overall11%, transition/completion once, continued Run, Home/Retry cleanup, globalBest and P2 independence covered. MissingScript0. Warm36-star500-update loop: calibrated Mono heap delta0/collections0, ~.184ms; UnityMono allocation-counter API unsupported, not used as proof.

Earlier Sprint9.3 APK nativeQA SM-S942N/Android16/1080x2340: updateinstall preservedEnergy105/Best15930/Version2.34 placements/all8 shapes/8 lines/four double-clears matched independent Board model; Energy205, Score1850, naturalStage1->2. HOME/Collection/lockedP2/Game navigation passed. Clear samples~29.96FPS, max33.886ms, existing30FPS cap retained. Native cross/three-line/full1500 completion not exercised, covered in Editor. Touch/SFX/Haptic/readability subjective confirmation remains pending.

Resume: prior header build stopped with internal Bee BuildFinishedMessage error. Rebuilt without gameplay changes. Before final install phone nowEnergy220/Best15930/Version2 (user play continued); preserve these current values. Final build/install results below.
Final: Development build Warning0/Error0, clean rendered header PASS/no compiler or shader warnings, MissingScript0. Latest install-r/launch succeeded on SM-S942N. Energy220/Best15930/Version2 unchanged before/after/final. Horizontal3 Slot blank drag/Board match PASS; HOME confirm->HOME->PLAY leaves Score0/empty64/threePieces. Final PID Logcat has no crash/new game exception, existing nonfatal AssetPackManager/Dex logs remain. Incidental settings/generated files removed. APK SHA2563FF5469B6A989780C9E0CE81CAB9FD91A65FC6987AD939F9B793A1F867F6E32E. Final evidence Validation/sprint93_resume_build_clean.log, sprint93_resume_header_clean.log, sprint93_device_resume_*.png, sprint93_resume_logcat_final.txt, prefs final_before_install/final_after_install/resume_final. Normal commit/push main only.
