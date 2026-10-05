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
