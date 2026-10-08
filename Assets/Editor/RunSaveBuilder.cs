using System;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class RunSaveBuilder {
 public static void Build(){
  AssetDatabase.Refresh();const string p="Assets/Art/UI/Home/home_continue_button.png";var importer=(TextureImporter)AssetImporter.GetAtPath(p);var existing=(TextureImporter)AssetImporter.GetAtPath("Assets/Art/UI/Home/home_play_button.png");EditorUtility.CopySerialized(existing,importer);importer.SaveAndReimport();
  var scene=EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");var canvas=GameObject.Find("GameCanvas");var flow=canvas.GetComponent<GameFlowController>();var home=canvas.transform.Find("SafeArea/HomeRoot");var primary=home.Find("MainActions/GameStartButton").GetComponent<Image>();
  foreach(string n in new[]{"NewGameButton","NewGameConfirmPanel"})if(home.Find(n)!=null)UnityEngine.Object.DestroyImmediate(home.Find(n).gameObject);
  var template=home.Find("QuitConfirmPanel");var panel=UnityEngine.Object.Instantiate(template.gameObject,home);panel.name="NewGameConfirmPanel";var card=panel.transform.Find("Card");var question=card.Find("Question").GetComponent<Text>();question.text="진행 중인 게임이 있습니다.\n새 게임을 시작하면 현재 게임 기록이 사라집니다.\n새 게임을 시작할까요?";question.resizeTextMinSize=18;question.resizeTextMaxSize=26;var qr=question.rectTransform;qr.anchorMin=new Vector2(.06f,.48f);qr.anchorMax=new Vector2(.94f,.94f);qr.offsetMin=qr.offsetMax=Vector2.zero;var cr=(RectTransform)card;cr.anchorMin=new Vector2(.08f,.31f);cr.anchorMax=new Vector2(.92f,.69f);cr.offsetMin=cr.offsetMax=Vector2.zero;
  var no=card.Find("Cancel").GetComponent<Button>();var yes=card.Find("Confirm").GetComponent<Button>();yes.GetComponentInChildren<Text>().text="새 게임 시작";panel.SetActive(false);
  var go=UnityEngine.Object.Instantiate(yes.gameObject,home);go.name="NewGameButton";var r=(RectTransform)go.transform;r.anchorMin=new Vector2(.64f,.025f);r.anchorMax=new Vector2(.94f,.065f);r.offsetMin=r.offsetMax=Vector2.zero;go.GetComponent<Image>().color=new Color(.02f,.055f,.14f,.80f);var t=go.GetComponentInChildren<Text>();t.fontSize=t.resizeTextMaxSize=23;t.resizeTextMinSize=16;t.text="새 게임\n시작";
  var controls=home.GetComponent<HomeRunControls>();if(controls==null)controls=home.gameObject.AddComponent<HomeRunControls>();controls.Configure(flow,primary,AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Home/home_play_button.png"),AssetDatabase.LoadAssetAtPath<Sprite>(p),go.GetComponent<Button>(),panel,no,yes);
  canvas.transform.Find("SafeArea/ExitConfirm/Card/Question").GetComponent<Text>().text="게임을 저장하고 홈으로 이동할까요?";
  EditorUtility.SetDirty(controls);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("RUN_SAVE_SCENE_READY");EditorApplication.Exit(0);
 }
}
