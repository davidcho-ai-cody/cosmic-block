# Sprint 9.1 — GAME Layout Match & Polish

Implemented against the attached GAME layout reference. Existing nine PNGs, all gameplay/save/transition/feedback rules, HOME/Collection and original slot input forwarding remain. No hint or new in-game Retry button was added.

At 1080×1920 (top-left x/y, Canvas units):

| Element | Sprint 9 | Sprint 9.1 |
|---|---|---|
| Board Rect | (194.4,777.6), 691.2² | (76,562), 928²; +34.26% |
| Slot 0 Rect | (29.6,1612.8), 326.93×268.8 | (18.8,1560), 336.8×324; +3.02% width / +20.54% height |
| Slot Sprite aspect-fit area | 281.3×268.8 | 336.8×321.84; +19.73% linear |
| Planet Status Rect | (32.4,245.65), 1015.2×391.9 | (21.6,232), 1036.8×260; height −33.66% |
| Planet Rect to real Board gap | 140.05 | 70 |
| Real Board to Slot gap | 144 | 70 |
| Current Score max font | 70 | 106; +51.43% |

An initial 20% enlargement exposed overlap with the original frame's thick internal ornaments. GameFrameMesh splits the existing image into UI regions: corners/middle ornaments preserve isotropic scaling and straight strips expand. Planet Status uses the same approach to preserve its circular ornament while becoming wider/flatter. No pixels, alpha or import settings are modified. Board is still a separate 64-child Grid and the frame remains noninteractive decoration.

Planet Stage/Energy Text components are disabled for rendering but retained and updated internally. Current stage sprite/name, overall restoration %, and existing stage-local Bar remain. The existing stage-local Bar is retained within the requested preservation scope; energy thresholds/rewards/timing are unchanged. Slot root size is increased; rest cell cap 64→78 is presentation-only. Drag geometry/scale 1.05/offset 110, validation and consumed-slot guards remain.

All 23 regression entry points PASS, including Sprint 9 and Sprint 9.1; Sprint 9.1 has 2,099 assertions. Responsive 1080×1920 / Tall Safe / Short PASS; Board sides 928 / 891.648 / 696. Tall gaps remain 67.2, Short 52.5. Raw source SHA-256 unchanged for all nine PNGs; protected HOME/Collection/feedback/transitions/modals scene comparison shows 0 changes. Missing Script 0. Final Development build warnings/errors 0; compiler warnings/errors 0; clean Editor runtime warnings/errors 0. SM-S942N Android 16 install -r and launch succeeded. Native touch/preview/drop/clear/refill/HOME cancel-confirm/new run verified. No crash/new game error; existing AssetPackManager ClassNotFoundException logged once. Installation preserved Energy 335 / Best 15930 / version 2; one natural Clear changed Energy to 345. Final clean run: Score 0, empty Board, three Pieces; permanent values 345/15930/2. Commit title: fix: align game layout with visual reference.

The TMP probe now iterates actual characterCount instead of the capacity array; old unused entries can retain previous glyph geometry after shortening text. The same glyph-bound checks and truncation checks are retained. Production TMP drawing is unchanged by this harness correction.

Evidence under Validation/sprint91_*: empty/placed/reference_1280, Tall/Short, score sizes, drag/clear/game-over captures, layout_measurements.json, regression summary and scene preservation. Original Sprint 9 renders were backed up as sprint91_before_* before regressions overwrote old capture names.