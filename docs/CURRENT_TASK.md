# Sprint 8 — Planet Collection

Implemented a read-only Collection view in the existing Game scene. HOME layout and gameplay rules remain unchanged. Twenty existing regression entry points and the new Collection probe passed. Development APK build/install/cold launch and Collection touch/navigation checks passed on SM-S942N. Final post-build Editor validation: C# and shader warnings 0, runtime errors 0; Missing Script 0. See DEVLOG.md for detailed results and the known Android exception.

The HOME collection button opens CollectionRoot. Back and Android Back return directly to HOME without calling Retry or resetting a run. Reentry reads the current session PlanetRestoration model.

Planet 01 uses the existing five restoration sprites and current 0–400 progress. Planet 02–05 remain locked regardless of Planet 01 completion. No unlock rules, new planets or save keys were added.
