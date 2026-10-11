using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;
using CosmicBlock.UI;
public static class PopupPolishBuilder {
 const string Characters="게임을 종료할까요?현재 게임은 저장되어 다음에 이어서 플레이할 수 있어요.취소종료홈으로 돌아갈까요?진행 중인 게임은 저장돼요.홈에서 이어서 플레이할 수 있어요.계속하기";
 public static void Run(){var scene=EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");var medium=Font("Medium");var semi=Font("SemiBold");var safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");foreach(string path in new[]{"HomeRoot/QuitConfirmPanel/Card","ExitConfirm/Card"}){var card=safe.Find(path);var style=card.GetComponent<PopupTypography>()??card.gameObject.AddComponent<PopupTypography>();style.Configure(medium,semi);card.Find("TitleText").GetComponent<TMP_Text>().color=new Color(.94f,.85f,.66f);card.Find("DescriptionText").GetComponent<TMP_Text>().color=new Color(.80f,.85f,.93f);card.GetComponent<HomeQuitPopupLayout>()?.Apply();card.GetComponent<GameHomePopupLayout>()?.Apply();}EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("POPUP_POLISH_APPLIED");EditorApplication.Exit(0);}
 static TMP_FontAsset Font(string weight){string path="Assets/Fonts/CosmicPopup"+weight+".asset";var f=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);if(f!=null)return f;var source=AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/CosmicPopup"+weight+".ttf");if(source==null)throw new System.Exception("Missing font "+weight);f=TMP_FontAsset.CreateFontAsset(source);f.name="CosmicPopup"+weight;if(!f.TryAddCharacters(Characters))throw new System.Exception("Missing Korean glyphs "+weight);AssetDatabase.CreateAsset(f,path);foreach(var atlas in f.atlasTextures)AssetDatabase.AddObjectToAsset(atlas,f);AssetDatabase.AddObjectToAsset(f.material,f);f.atlasPopulationMode=AtlasPopulationMode.Static;return f;}
}
