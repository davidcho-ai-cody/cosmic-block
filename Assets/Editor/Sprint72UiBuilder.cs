using System;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Sprint72UiBuilder
{
    const string ScenePath="Assets/Scenes/Game.unity";
    [MenuItem("COSMIC BLOCK/Sprint 7-2/Polish And Koreanize UI")]
    public static void Build()
    {
        StarlightUiTheme.EnsureAssets();Sprint71HomeBuilder.Build();
        var scene=EditorSceneManager.OpenScene(ScenePath);var canvas=GameObject.Find("GameCanvas");var safe=canvas.transform.Find("SafeArea");var home=safe.Find("HomeRoot");
        if(canvas==null||safe==null||home==null)throw new Exception("STOP: Sprint 7.1 scene foundation missing.");
        PolishHome(home);PolishGame(safe);PolishPopups(safe,home);
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Validate();Debug.Log("SPRINT72_UI_READY");
    }
    static void PolishHome(Transform home)
    {
        var title=At<Text>(home,"TitleArea/Title");title.fontSize=72;StarlightUiTheme.Text(title,StarlightUiTheme.Gold,true);
        var area=home.Find("TitleArea");var sparkle=area.Find("Sparkle");if(sparkle==null){var go=new GameObject("Sparkle",typeof(RectTransform),typeof(Text));sparkle=go.transform;sparkle.SetParent(area,false);var r=(RectTransform)sparkle;r.anchorMin=new Vector2(.43f,.88f);r.anchorMax=new Vector2(.57f,1.08f);r.offsetMin=r.offsetMax=Vector2.zero;var t=go.GetComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text="✦";t.fontSize=25;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;StarlightUiTheme.Text(t,StarlightUiTheme.Gold,true);}
        StarlightUiTheme.Text(At<Text>(home,"TitleArea/Subtitle"),new Color(.78f,.88f,1,1));
        StyleButton(home,"MainActions/GameStartButton",true);StyleButton(home,"MainActions/PlanetCollectionButton",false);
        var best=At<Text>(home,"BestScore");best.fontSize=32;StarlightUiTheme.Text(best,Color.white,true);
        var card=At<Image>(home,"CurrentPlanetArea");StarlightUiTheme.Panel(card,new Color(.015f,.045f,.12f,.70f));StarlightUiTheme.Text(At<Text>(home,"CurrentPlanetArea/PlanetName"),StarlightUiTheme.Gold,true);StarlightUiTheme.Text(At<Text>(home,"CurrentPlanetArea/Heading"),StarlightUiTheme.Blue);
        var track=At<Image>(home,"CurrentPlanetArea/ProgressBar");StarlightUiTheme.Panel(track,new Color(.005f,.015f,.05f,.92f),false);At<Image>(home,"CurrentPlanetArea/ProgressBar/Fill").color=StarlightUiTheme.Gold;
        StyleButton(home,"BottomActions/SettingsButton",false);StyleButton(home,"BottomActions/QuitButton",false);
        var toast=At<Image>(home,"CollectionToast");StarlightUiTheme.Panel(toast,StarlightUiTheme.GlassStrong);
    }
    static void PolishGame(Transform safe)
    {
        var title=At<Text>(safe,"Title");title.text="별빛 블록";StarlightUiTheme.Text(title,StarlightUiTheme.Gold,true);
        var score=At<Text>(safe,"ScorePlaceholder");score.text="최고  0     점수  0";StarlightUiTheme.Text(score,Color.white);
        var planet=safe.Find("PlanetRestoration");var panel=planet.GetComponent<Image>();StarlightUiTheme.Panel(panel,new Color(.012f,.035f,.10f,.86f));
        At<Text>(planet,"Title").text="행성 01 · "+PlanetRestoration.Planet01DisplayName;At<Text>(planet,"Stage").text="복원 단계 1 / 5";At<Text>(planet,"Energy").text="에너지 0 / 100";
        StarlightUiTheme.Text(At<Text>(planet,"Title"),StarlightUiTheme.Gold,true);StarlightUiTheme.Text(At<Text>(planet,"Stage"),Color.white);StarlightUiTheme.Text(At<Text>(planet,"Energy"),StarlightUiTheme.Blue);
        var bar=At<Image>(planet,"ProgressBar");StarlightUiTheme.Panel(bar,new Color(.005f,.015f,.05f,.94f),false);At<Image>(planet,"ProgressBar/Fill").color=StarlightUiTheme.Gold;
        var home=At<Button>(safe,"PlayingHomeButton");StyleButton(home,false);At<Text>(safe,"PlayingHomeButton/Text").text="홈";
        var completion=At<Image>(safe,"PlanetCompletion");completion.color=StarlightUiTheme.Dim;StarlightUiTheme.Text(At<Text>(safe,"PlanetCompletion/Text"),StarlightUiTheme.Gold,true);
    }
    static void PolishPopups(Transform safe,Transform home)
    {
        Modal(safe,"ExitConfirm");StyleButton(safe,"ExitConfirm/Card/Continue",true);StyleButton(safe,"ExitConfirm/Card/GoHome",false);
        Modal(home,"SettingsPanel");StyleButton(home,"SettingsPanel/Card/Close",false);var sfx=At<Text>(home,"SettingsPanel/Card/Sfx");sfx.text="효과음                         ON";var haptic=At<Text>(home,"SettingsPanel/Card/Haptic");haptic.text="진동                             ON";StarlightUiTheme.Text(sfx,Color.white);StarlightUiTheme.Text(haptic,Color.white);
        Modal(home,"QuitConfirmPanel");StyleButton(home,"QuitConfirmPanel/Card/Cancel",true);StyleButton(home,"QuitConfirmPanel/Card/Confirm",false);
        Modal(safe,"GameOverPanel");var gameOver=safe.Find("GameOverPanel");At<Text>(gameOver,"Card/Title").text="게임 오버";At<Text>(gameOver,"Card/Title").color=StarlightUiTheme.Gold;At<Text>(gameOver,"Card/RetryButton/Label").text="다시 하기";StyleButton(gameOver,"Card/RetryButton",true);var homeButton=gameOver.Find("Card/HomeButton");if(homeButton!=null){At<Text>(gameOver,"Card/HomeButton/Label").text="홈으로";StyleButton(gameOver,"Card/HomeButton",false);}
    }
    static void Modal(Transform root,string path){var panel=root.Find(path) as RectTransform;var card=panel.Find("Card") as RectTransform;StarlightUiTheme.Modal(panel,card);}
    static void StyleButton(Transform root,string path,bool primary)=>StyleButton(At<Button>(root,path),primary);
    static void StyleButton(Button button,bool primary){StarlightUiTheme.Button(button,primary);var label=button.GetComponentInChildren<Text>(true);if(label!=null)StarlightUiTheme.Text(label,primary?StarlightUiTheme.Gold:Color.white,primary);}
    static T At<T>(Transform root,string path)where T:Component{var child=root.Find(path);if(child==null)throw new Exception("STOP: missing UI path "+path);var value=child.GetComponent<T>();if(value==null)throw new Exception("STOP: missing "+typeof(T).Name+" at "+path);return value;}
    [MenuItem("COSMIC BLOCK/Sprint 7-2/Validate")]
    public static void Validate(){var safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");var home=safe.Find("HomeRoot");Require(At<Text>(home,"TitleArea/Title").text=="별빛 블록","HOME title");Require(At<Text>(safe,"Title").text=="별빛 블록","GAME title");Require(At<Text>(safe,"PlayingHomeButton/Text").text=="홈","GAME home");Require(At<Text>(safe,"PlanetRestoration/Title").text.Contains(PlanetRestoration.Planet01DisplayName),"planet name");Require(At<Text>(safe,"PlanetRestoration/Stage").text.StartsWith("복원 단계"),"planet stage");Require(At<Text>(safe,"PlanetRestoration/Energy").text.StartsWith("에너지"),"planet energy");Require(At<Image>(home,"CurrentPlanetArea").type==Image.Type.Sliced,"glass card");Require(GameObject.Find("ComboText")==null,"no legacy combo HUD");}
    static void Require(bool value,string message){if(!value)throw new Exception("Sprint 7.2 validation failed: "+message);}
}
