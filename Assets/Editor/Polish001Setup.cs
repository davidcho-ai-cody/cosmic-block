using CosmicBlock.Effects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Polish001Setup {
 public static void Run(){EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");var feedback=Object.FindAnyObjectByType<GameFeedbackController>();var score=feedback.transform.Find("ScorePop").GetComponent<Text>();score.fontSize=38;score.color=new Color(1,.76f,.28f,.96f);score.rectTransform.sizeDelta=new Vector2(420,60);score.text="+100 SCORE";var title=feedback.transform.Find("ComboPop").GetComponent<Text>();title.color=new Color(.7f,.94f,1,.96f);
 var old=feedback.transform.Find("EnergyPop");if(old!=null)Object.DestroyImmediate(old.gameObject);var energy=Object.Instantiate(score,feedback.transform);energy.name="EnergyPop";energy.fontSize=30;energy.color=new Color(.55f,.88f,1,.96f);energy.rectTransform.sizeDelta=new Vector2(360,44);energy.text="";energy.gameObject.SetActive(false);feedback.ConfigureEnergyPopup(energy);
 EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());Debug.Log("POLISH001_SETUP_PASS");EditorApplication.Exit(0);}
}
