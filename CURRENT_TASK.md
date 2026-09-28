# CURRENT TASK

Sprint 6 Planet Restoration is implemented and under final validation.

- STAR JOURNEY user-facing progression replaced by persistent Planet 01 restoration.
- 500 Energy total; line clear awards 10/25/45/70.
- Five visual stages with cross-fade, travel sparks, pulse, and one-time completion celebration.
- Home and Game Over show Planet restoration progress.

## Sprint 6.5 — Planet Restoration Game Feel
- Planet 01 progression uses total Energy 0–400 and four 100-Energy healing transitions.
- HUD shows current Stage energy (0–100); Stage 5 shows RESTORED.
- Line clears award 10/25/45/70 Energy for 1/2/3/4+ lines.
- Pooled fragments fly from cleared Board cells to the Planet before the visible bar advances.
- Stage boundaries use a central 1.5–2.0 second healing transition with Resolving input lock.
- Legacy 0–500 saves migrate once using `round(old / 500 * 400)` and a version key.

- Sprint 6.5 QA Hotfix: development-only Planet reset/presets and Stage 5 HUD duplicate removal.
