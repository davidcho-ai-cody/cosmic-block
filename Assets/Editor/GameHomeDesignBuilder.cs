using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CosmicBlock.UI;
public static class GameHomeDesignBuilder {
 public static void Run(){var scene=EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");var flow=Object.FindAnyObjectByType<GameFlowController>();var card=(RectTransform)GameObject.Find("GameCanvas").transform.Find("SafeArea/ExitConfirm/Card");
  foreach(var image in card.GetComponentsInChildren<Image>(true))if(image.name=="ThemeFill")Object.DestroyImmediate(image.gameObject);
  if(card.GetComponent<Image>()!=null)Object.DestroyImmediate(card.GetComponent<Image>());
  foreach(var effect in card.GetComponents<Shadow>())Object.DestroyImmediate(effect);
  var area=(RectTransform)card.parent;
  var dim=area.Find("DimOverlay");if(dim==null){var go=new GameObject("DimOverlay",typeof(RectTransform),typeof(Image));go.transform.SetParent(area,false);go.transform.SetAsFirstSibling();go.GetComponent<Image>().color=new Color(0,0,0,.70f);}
  if(area.GetComponent<Image>()!=null)Object.DestroyImmediate(area.GetComponent<Image>());
  if(card.Find("CosmicFrame")==null){var go=new GameObject("CosmicFrame",typeof(RectTransform),typeof(CanvasRenderer));go.transform.SetParent(card,false);go.transform.SetAsFirstSibling();Place((RectTransform)go.transform,Vector2.zero,Vector2.one);}
  var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/GameHomeKorean.asset");
  if(font==null){font=TMP_FontAsset.CreateFontAsset(AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/NotoSansKR-VF.ttf"));font.name="GameHomeKorean";font.TryAddCharacters("홈으로 돌아갈까요?진행 중인 게임은 저장돼요.홈에서 이어서 플레이할 수 있어요.계속하기");AssetDatabase.CreateAsset(font,"Assets/Fonts/GameHomeKorean.asset");foreach(var atlas in font.atlasTextures)AssetDatabase.AddObjectToAsset(atlas,font);AssetDatabase.AddObjectToAsset(font.material,font);font.atlasPopulationMode=AtlasPopulationMode.Static;}
  var titleNode=card.Find("Question")??card.Find("TitleText");ConvertText(titleNode,font,"홈으로 돌아갈까요?");titleNode.name="TitleText";
  if(card.Find("DescriptionText")==null){var go=new GameObject("DescriptionText",typeof(RectTransform));go.transform.SetParent(card,false);}
  ConvertText(card.Find("DescriptionText"),font,"진행 중인 게임은 저장돼요.\n홈에서 이어서 플레이할 수 있어요.");
  foreach(var name in new[]{"Continue","GoHome"})ConvertText(card.Find(name+"/Text"),font,name=="Continue"?"계속하기":"홈으로");
  if(card.GetComponent<GameHomePopupLayout>()==null)card.gameObject.AddComponent<GameHomePopupLayout>();
  var frame=card.Find("CosmicFrame");Mesh(frame,new Color(.063f,.102f,.18f),new Color(.81f,.70f,.43f,.5f),18,1).raycastTarget=false;
  if(card.Find("OrbitMark")==null){var orbit=new GameObject("OrbitMark",typeof(RectTransform),typeof(GameHomeIconGraphic));orbit.transform.SetParent(card,false);}var mark=(RectTransform)card.Find("OrbitMark");if(mark.GetComponent<CanvasRenderer>()==null)mark.gameObject.AddComponent<CanvasRenderer>();Place(mark,new Vector2(.47f,.78f),new Vector2(.53f,.874f));mark.GetComponent<GameHomeIconGraphic>().color=new Color(.475f,.741f,.816f);mark.GetComponent<GameHomeIconGraphic>().raycastTarget=false;
  Place((RectTransform)card.Find("TitleText"),new Vector2(.10f,.63f),new Vector2(.90f,.75f));var title=card.Find("TitleText").GetComponent<TextMeshProUGUI>();title.color=new Color(.918f,.831f,.635f);title.fontSizeMax=52;title.fontStyle=FontStyles.Bold;
  Place((RectTransform)card.Find("DescriptionText"),new Vector2(.08f,.39f),new Vector2(.92f,.59f));card.Find("DescriptionText").GetComponent<TextMeshProUGUI>().color=new Color(.745f,.788f,.863f);
  foreach(string name in new[]{"Continue","GoHome"}){var r=(RectTransform)card.Find(name);bool cancel=name=="Continue";var graphic=Mesh(r,cancel?new Color(.09f,.192f,.267f):new Color(.788f,.698f,.482f),cancel?new Color(.235f,.427f,.514f):new Color(.788f,.698f,.482f),10,1);graphic.raycastTarget=true;var button=r.GetComponent<Button>();button.targetGraphic=graphic;var colors=button.colors;colors.normalColor=Color.white;colors.highlightedColor=new Color(1,1,1,1);colors.pressedColor=new Color(.90f,.90f,.90f);button.colors=colors;Place(r,new Vector2(cancel?.08f:.515f,.10f),new Vector2(cancel?.485f:.92f,.275f));var text=r.GetComponentInChildren<TextMeshProUGUI>();text.color=cancel?new Color(.824f,.929f,.953f):new Color(.09f,.125f,.20f);text.fontSizeMax=36;text.fontStyle=FontStyles.Bold;text.alignment=TextAlignmentOptions.Center;}
  card.GetComponent<GameHomePopupLayout>().Apply();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);Debug.Log("GAME_HOME_MESH_APPLIED");EditorApplication.Exit(0);
 }
 static void ConvertText(Transform target,TMP_FontAsset font,string value){if(target.GetComponent<Text>()!=null)Object.DestroyImmediate(target.GetComponent<Text>());var t=target.GetComponent<TextMeshProUGUI>()??target.gameObject.AddComponent<TextMeshProUGUI>();t.font=font;t.text=value;t.enableAutoSizing=true;t.fontSizeMin=16;t.fontSizeMax=32;t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;}
 static HomeQuitMeshGraphic Mesh(Transform target,Color fill,Color edge,float radius,float stroke){if(target.GetComponent<Image>()!=null)Object.DestroyImmediate(target.GetComponent<Image>());var mesh=target.GetComponent<HomeQuitMeshGraphic>()??target.gameObject.AddComponent<HomeQuitMeshGraphic>();mesh.color=Color.white;mesh.Style(fill,edge,radius,stroke);return mesh;}
 static void Place(RectTransform r,Vector2 min,Vector2 max){r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;}
}
