using System.IO;
using CosmicBlock.Board;
using CosmicBlock.Blocks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class Sprint911GameBuilder {
 public static void Build(){const string path="Assets/Scenes/Game.unity";string before=File.ReadAllText(path);var scene=EditorSceneManager.OpenScene(path);var safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");safe.Find("Board").GetComponent<BoardView>().ConfigureDensity(1.20f);foreach(var piece in safe.Find("BlockArea").GetComponentsInChildren<BlockPiece>(true))piece.ConfigureDensitySlotFit();Canvas.ForceUpdateCanvases();Sprint9GameBuilder.PreserveHome(before,safe);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("SPRINT911_DENSITY_READY");}
}
