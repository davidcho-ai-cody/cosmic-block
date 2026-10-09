using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class Sprint101Builder {
 public static void Build(){var scene=EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");var canvas=GameObject.Find("GameCanvas");var popup=canvas.GetComponent<PlanetCompletionPopup>();if(popup==null)popup=canvas.AddComponent<PlanetCompletionPopup>();popup.Configure(canvas.GetComponent<GameFlowController>(),Object.FindAnyObjectByType<GameSession>());EditorUtility.SetDirty(popup);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();EditorApplication.Exit(0);}
}
