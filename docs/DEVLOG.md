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
