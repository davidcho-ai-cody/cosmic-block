using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Android.Types;

public static class AndroidBuildAutomation
{
    public const string DevelopmentPath = "Builds/Android/Development/CosmicBlock-dev.apk";
    public const string ReleasePath = "Builds/Android/Release/CosmicBlock.apk";

    [MenuItem("COSMIC BLOCK/Build/Android Development APK")]
    public static void BuildDevelopment() => Build(DevelopmentPath, BuildOptions.Development | BuildOptions.AllowDebugging);

    [MenuItem("COSMIC BLOCK/Build/Android Release APK")]
    public static void BuildRelease() => Build(ReleasePath, BuildOptions.None);

    [MenuItem("COSMIC BLOCK/Build/Report Android Configuration")]
    public static void ReportConfiguration()
    {
        var target = UnityEditor.Build.NamedBuildTarget.Android;
        string packageName = PlayerSettings.GetApplicationIdentifier(target);
        string backend = PlayerSettings.GetScriptingBackend(target).ToString();
        string architectures = PlayerSettings.Android.targetArchitectures.ToString();
        string graphics = string.Join(", ", PlayerSettings.GetGraphicsAPIs(BuildTarget.Android).Select(x => x.ToString()));
        string report =
            "ANDROID_CONFIGURATION\n" +
            "Platform=Android\n" +
            "Orientation=" + PlayerSettings.defaultInterfaceOrientation + "\n" +
            "MinSdk=" + (int)PlayerSettings.Android.minSdkVersion + "\n" +
            "TargetSdk=" + (int)PlayerSettings.Android.targetSdkVersion + "\n" +
            "ScriptingBackend=" + backend + "\n" +
            "TargetArchitectures=" + architectures + "\n" +
            "PackageName=" + packageName + "\n" +
            "Version=" + PlayerSettings.bundleVersion + "\n" +
            "VersionCode=" + PlayerSettings.Android.bundleVersionCode + "\n" +
            "GraphicsAPI=" + graphics + "\n" +
            "InputSystem=New Input System (activeInputHandler 1)\n";
        Directory.CreateDirectory("Validation");
        File.WriteAllText("Validation/android_configuration.txt", report);
        Debug.Log(report);
    }

    static void Build(string outputPath, BuildOptions options)
    {
        ReportConfiguration();
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android &&
            !EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            throw new Exception("Unable to switch active build target to Android.");

        EditorUserBuildSettings.buildAppBundle = false;
        var scenes = EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray();
        if (scenes.Length == 0) throw new Exception("No enabled scenes in Build Settings.");
        var build = new BuildPlayerOptions { scenes = scenes, locationPathName = outputPath, target = BuildTarget.Android, options = options };
        var previousSymbols = UnityEditor.Android.UserBuildSettings.DebugSymbols.level;
        BuildReport report;
        try
        {
            if ((options & BuildOptions.Development) != 0)
                UnityEditor.Android.UserBuildSettings.DebugSymbols.level = DebugSymbolLevel.SymbolTable;
            report = BuildPipeline.BuildPlayer(build);
        }
        finally
        {
            UnityEditor.Android.UserBuildSettings.DebugSymbols.level = previousSymbols;
        }
        var summary = report.summary;
        Debug.Log("ANDROID_BUILD_RESULT result=" + summary.result + " errors=" + summary.totalErrors +
                  " warnings=" + summary.totalWarnings + " size=" + summary.totalSize + " output=" + outputPath);
        if (summary.result != BuildResult.Succeeded || summary.totalErrors != 0)
            throw new Exception("Android build failed: " + summary.result + ", errors=" + summary.totalErrors);
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }
}
