using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CosmicBlock.UI;
public static class HomeExitDesignBuilder {
 public static void Run(){var scene=EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");var flow=Object.FindAnyObjectByType<GameFlowController>();var card=(RectTransform)flow.HomeRoot.transform.Find("QuitConfirmPanel/Card");
  if(card.Find("InteriorBacking")!=null)Object.DestroyImmediate(card.Find("InteriorBacking").gameObject);
  var frame=card.Find("CosmicFrame");Mesh(frame,new Color(.063f,.102f,.18f),new Color(.81f,.70f,.43f,.5f),18,1).raycastTarget=false;
  if(card.Find("OrbitMark")==null){var orbit=new GameObject("OrbitMark",typeof(RectTransform),typeof(HomeQuitOrbitGraphic));orbit.transform.SetParent(card,false);}var mark=(RectTransform)card.Find("OrbitMark");if(mark.GetComponent<CanvasRenderer>()==null)mark.gameObject.AddComponent<CanvasRenderer>();Place(mark,new Vector2(.47f,.78f),new Vector2(.53f,.874f));mark.GetComponent<HomeQuitOrbitGraphic>().color=new Color(.475f,.741f,.816f);mark.GetComponent<HomeQuitOrbitGraphic>().raycastTarget=false;
  Place((RectTransform)card.Find("TitleText"),new Vector2(.10f,.63f),new Vector2(.90f,.75f));var title=card.Find("TitleText").GetComponent<TextMeshProUGUI>();title.color=new Color(.918f,.831f,.635f);title.fontSizeMax=52;
  Place((RectTransform)card.Find("DescriptionText"),new Vector2(.08f,.39f),new Vector2(.92f,.59f));card.Find("DescriptionText").GetComponent<TextMeshProUGUI>().color=new Color(.745f,.788f,.863f);
  foreach(string name in new[]{"Cancel","Confirm"}){var r=(RectTransform)card.Find(name);bool cancel=name=="Cancel";var graphic=Mesh(r,cancel?new Color(.09f,.192f,.267f):new Color(.788f,.698f,.482f),cancel?new Color(.235f,.427f,.514f):new Color(.788f,.698f,.482f),10,1);graphic.raycastTarget=true;var button=r.GetComponent<Button>();button.targetGraphic=graphic;var colors=button.colors;colors.normalColor=Color.white;colors.highlightedColor=new Color(1,1,1,1);colors.pressedColor=new Color(.90f,.90f,.90f);button.colors=colors;Place(r,new Vector2(cancel?.08f:.515f,.10f),new Vector2(cancel?.485f:.92f,.275f));var text=r.GetComponentInChildren<TextMeshProUGUI>();text.color=cancel?new Color(.824f,.929f,.953f):new Color(.09f,.125f,.20f);text.fontSizeMax=36;text.alignment=TextAlignmentOptions.Center;}
  card.GetComponent<HomeQuitPopupLayout>().Apply();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);Debug.Log("HOME_EXIT_MESH_APPLIED");EditorApplication.Exit(0);
 }
 static HomeQuitMeshGraphic Mesh(Transform target,Color fill,Color edge,float radius,float stroke){if(target.GetComponent<Image>()!=null)Object.DestroyImmediate(target.GetComponent<Image>());var mesh=target.GetComponent<HomeQuitMeshGraphic>()??target.gameObject.AddComponent<HomeQuitMeshGraphic>();mesh.color=Color.white;mesh.Style(fill,edge,radius,stroke);return mesh;}
 static void Place(RectTransform r,Vector2 min,Vector2 max){r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;}
}
