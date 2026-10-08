using System;
using System.IO;
using CosmicBlock.Core;
using UnityEditor;
// Opt-in isolation survives EnterPlaymode domain reload and never touches the user's current-run file.
[InitializeOnLoad] public static class RunSaveProbeIsolation {
 static RunSaveProbeIsolation(){string p=Environment.GetEnvironmentVariable("COSMIC_RUN_SAVE_TEST_PATH");if(!string.IsNullOrEmpty(p))RunSaveStore.PathOverride=p;}
}
