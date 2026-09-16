using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MissingScriptDiagnostics
{
    const string ScenePath = "Assets/Scenes/Game.unity";
    [MenuItem("COSMIC BLOCK/Diagnostics/Find Missing Scripts")]
    public static void FindMissingScripts()
    {
        var entries = ScanOpenScene();
        foreach (var entry in entries) Debug.LogWarning("MISSING_SCRIPT: " + entry);
        Debug.Log("MISSING_SCRIPT_COUNT=" + entries.Count);
    }
    public static void ScanGameSceneBatch(){EditorSceneManager.OpenScene(ScenePath);FindMissingScripts();EditorApplication.Exit(0);}
    public static void CleanGameSceneBatch()
    {
        var scene=EditorSceneManager.OpenScene(ScenePath);int removed=RemoveMissingScriptsInOpenScene();
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        int remaining=ScanOpenScene().Count;Debug.Log("MISSING_SCRIPT_CLEAN removed="+removed+" remaining="+remaining);
        EditorApplication.Exit(remaining==0?0:1);
    }
    public static int RemoveMissingScriptsInOpenScene()
    {
        int removed=0;foreach(var go in AllGameObjects()){int count=GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);if(count==0)continue;Debug.Log("REMOVING_MISSING_SCRIPT: "+PathOf(go.transform)+" count="+count);removed+=GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);}return removed;
    }
    static List<string> ScanOpenScene(){var found=new List<string>();foreach(var go in AllGameObjects()){int count=GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);if(count>0)found.Add(PathOf(go.transform)+" count="+count);}return found;}
    static IEnumerable<GameObject> AllGameObjects(){Scene scene=SceneManager.GetActiveScene();foreach(var root in scene.GetRootGameObjects())foreach(var transform in root.GetComponentsInChildren<Transform>(true))yield return transform.gameObject;}
    static string PathOf(Transform transform){string path=transform.name;while(transform.parent!=null){transform=transform.parent;path=transform.name+"/"+path;}return path;}
}
