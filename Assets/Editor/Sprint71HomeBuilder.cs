using System;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Sprint71HomeBuilder
{
    public const string ScenePath="Assets/Scenes/Game.unity";
    const string BackgroundPath="Assets/Art/Backgrounds/Home_Background_StarlightBlock.png";
    static readonly string[] PlanetPaths={
        "Assets/Art/Planets/Planet01/Planet01_Stage01_Desolate.png",
        "Assets/Art/Planets/Planet01/Planet01_Stage02_Awakening.png",
        "Assets/Art/Planets/Planet01/Planet01_Stage03_Recovering.png",
        "Assets/Art/Planets/Planet01/Planet01_Stage04_Thriving.png",
        "Assets/Art/Planets/Planet01/Planet01_Stage05_Restored.png"};
    static readonly Color Gold=new Color(.98f,.76f,.32f,1),White=new Color(.96f,.97f,1,1),Blue=new Color(.72f,.84f,1,1),Navy=new Color(.018f,.035f,.10f,.82f);

    [MenuItem("COSMIC BLOCK/Sprint 7-1/Build Starlight Home")]
    public static void Build()
    {
        ConfigureBackground();AssetDatabase.Refresh();
        var scene=EditorSceneManager.OpenScene(ScenePath);var canvas=GameObject.Find("GameCanvas");var safe=canvas.transform.Find("SafeArea");
        var home=safe.Find("HomeRoot") as RectTransform;var flow=canvas.GetComponent<GameFlowController>();var session=UnityEngine.Object.FindAnyObjectByType<GameSession>();
        if(home==null||flow==null||session==null)throw new Exception("Sprint 7-1 foundation missing.");
        int homeCount=0;foreach(var go in SceneObjects())if(go.name=="HomeRoot")homeCount++;if(homeCount!=1)throw new Exception("STOP: expected exactly one HomeRoot, found "+homeCount);

        for(int i=home.childCount-1;i>=0;i--){var child=home.GetChild(i);if(child.name!="DevButton"&&child.name!="PlanetDebugPanel")UnityEngine.Object.DestroyImmediate(child.gameObject);}
        var oldView=home.GetComponent<HomeViewController>();if(oldView!=null)UnityEngine.Object.DestroyImmediate(oldView);
        var rootImage=Ensure<Image>(home.gameObject);rootImage.color=Color.clear;rootImage.raycastTarget=false;

        var background=UI("HomeBackground",home);Stretch(background);var backgroundImage=background.gameObject.AddComponent<Image>();backgroundImage.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);backgroundImage.preserveAspect=true;backgroundImage.raycastTarget=false;
        var aspect=background.gameObject.AddComponent<AspectFillBackground>();var aspectSo=new SerializedObject(aspect);aspectSo.FindProperty("referenceSize").vector2Value=new Vector2(940,1672);aspectSo.FindProperty("includeUnsafeArea").boolValue=true;aspectSo.ApplyModifiedPropertiesWithoutUndo();background.SetAsFirstSibling();

        var titleArea=UI("TitleArea",home);Set(titleArea,new Vector2(.06f,.79f),new Vector2(.94f,.965f));
        var title=Text("Title",titleArea,"별빛 블록",68,Gold,FontStyle.Bold);Set(title.rectTransform,new Vector2(0,.35f),Vector2.one);Shadow(title,new Color(.01f,.02f,.08f,.95f),new Vector2(2,-3));
        var subtitle=Text("Subtitle",titleArea,"상상을 플레이하다",25,White,FontStyle.Normal);Set(subtitle.rectTransform,new Vector2(.05f,0),new Vector2(.95f,.36f));Shadow(subtitle,new Color(0,0,.08f,.9f),new Vector2(1,-2));

        var actions=UI("MainActions",home);Set(actions,new Vector2(.14f,.57f),new Vector2(.86f,.76f));
        var play=Button("GameStartButton",actions,"게임 시작",38,Navy,Gold,true);Set(play.GetComponent<RectTransform>(),new Vector2(0,.56f),Vector2.one);
        var collection=Button("PlanetCollectionButton",actions,"행성 도감",29,new Color(.025f,.05f,.14f,.74f),new Color(.57f,.70f,.92f,.82f),false);Set(collection.GetComponent<RectTransform>(),Vector2.zero,new Vector2(1,.42f));

        var best=Text("BestScore",home,"최고 점수  0",31,White,FontStyle.Bold);Set(best.rectTransform,new Vector2(.16f,.505f),new Vector2(.84f,.56f));Shadow(best,new Color(0,0,.05f,.95f),new Vector2(1,-2));

        var planetArea=UI("CurrentPlanetArea",home);Set(planetArea,new Vector2(.09f,.19f),new Vector2(.91f,.49f));var planetBg=planetArea.gameObject.AddComponent<Image>();planetBg.color=new Color(.015f,.035f,.11f,.47f);planetBg.raycastTarget=false;var planetOutline=planetArea.gameObject.AddComponent<Outline>();planetOutline.effectColor=new Color(.35f,.58f,.92f,.28f);planetOutline.effectDistance=new Vector2(1,-1);
        var heading=Text("Heading",planetArea,"현재 복원 중인 행성",22,Blue,FontStyle.Normal);Set(heading.rectTransform,new Vector2(.05f,.82f),new Vector2(.95f,.98f));
        var planetImage=MakeImage("Planet",planetArea);Set(planetImage.rectTransform,new Vector2(.06f,.09f),new Vector2(.48f,.80f));planetImage.preserveAspect=true;planetImage.raycastTarget=false;
        var planetName=Text("PlanetName",planetArea,"푸른 별",31,Gold,FontStyle.Bold);Set(planetName.rectTransform,new Vector2(.50f,.59f),new Vector2(.95f,.79f));
        var stage=Text("Stage",planetArea,"복원 단계 1 / 5",23,White,FontStyle.Bold);Set(stage.rectTransform,new Vector2(.50f,.41f),new Vector2(.95f,.60f));
        var track=UI("ProgressBar",planetArea);Set(track,new Vector2(.55f,.27f),new Vector2(.91f,.34f));var trackImage=track.gameObject.AddComponent<Image>();trackImage.color=new Color(.02f,.04f,.10f,.94f);trackImage.raycastTarget=false;
        var fill=MakeImage("Fill",track);Set(fill.rectTransform,Vector2.zero,new Vector2(0,1));fill.color=Gold;fill.type=Image.Type.Simple;fill.fillAmount=0;fill.raycastTarget=false;
        var energy=Text("Energy",planetArea,"0%",19,Blue,FontStyle.Bold);Set(energy.rectTransform,new Vector2(.52f,.08f),new Vector2(.94f,.24f));

        var bottom=UI("BottomActions",home);Set(bottom,new Vector2(.08f,.075f),new Vector2(.92f,.145f));
        var settings=Button("SettingsButton",bottom,"⚙ 설정",21,new Color(.02f,.04f,.10f,.58f),new Color(.66f,.76f,.92f,.50f),false);Set(settings.GetComponent<RectTransform>(),new Vector2(0,0),new Vector2(.42f,1));
        var quit=Button("QuitButton",bottom,"게임 종료",21,new Color(.02f,.04f,.10f,.58f),new Color(.66f,.76f,.92f,.50f),false);Set(quit.GetComponent<RectTransform>(),new Vector2(.58f,0),Vector2.one);

        var toast=UI("CollectionToast",home);Set(toast,new Vector2(.20f,.50f),new Vector2(.80f,.56f));var toastBg=toast.gameObject.AddComponent<Image>();toastBg.color=new Color(.02f,.04f,.12f,.96f);var toastText=Text("Text",toast,"행성 도감은 준비 중입니다.",22,White,FontStyle.Bold);Stretch(toastText.rectTransform);toast.gameObject.SetActive(false);

        var settingsPanel=Modal("SettingsPanel",home,out var settingsCard);var settingsTitle=Text("Title",settingsCard,"설정",32,Gold,FontStyle.Bold);Set(settingsTitle.rectTransform,new Vector2(.08f,.72f),new Vector2(.92f,.92f));
        var sfx=Text("Sfx",settingsCard,"효과음                         ON",23,White,FontStyle.Normal);Set(sfx.rectTransform,new Vector2(.10f,.48f),new Vector2(.90f,.67f));
        var haptic=Text("Haptic",settingsCard,"진동                             ON",23,White,FontStyle.Normal);Set(haptic.rectTransform,new Vector2(.10f,.30f),new Vector2(.90f,.49f));
        var settingsClose=Button("Close",settingsCard,"닫기",23,new Color(.10f,.15f,.27f,.95f),new Color(.55f,.70f,.95f,.7f),false);Set(settingsClose.GetComponent<RectTransform>(),new Vector2(.23f,.08f),new Vector2(.77f,.25f));settingsPanel.gameObject.SetActive(false);

        var quitPanel=Modal("QuitConfirmPanel",home,out var quitCard);var quitQuestion=Text("Question",quitCard,"게임을 종료할까요?",28,White,FontStyle.Bold);Set(quitQuestion.rectTransform,new Vector2(.08f,.57f),new Vector2(.92f,.87f));
        var cancel=Button("Cancel",quitCard,"취소",23,new Color(.10f,.15f,.27f,.95f),new Color(.55f,.70f,.95f,.7f),false);Set(cancel.GetComponent<RectTransform>(),new Vector2(.08f,.14f),new Vector2(.46f,.40f));
        var confirm=Button("Confirm",quitCard,"종료",23,new Color(.46f,.27f,.10f,.96f),Gold,false);Set(confirm.GetComponent<RectTransform>(),new Vector2(.54f,.14f),new Vector2(.92f,.40f));quitPanel.gameObject.SetActive(false);

        var sprites=new Sprite[5];for(int i=0;i<sprites.Length;i++)sprites[i]=AssetDatabase.LoadAssetAtPath<Sprite>(PlanetPaths[i]);
        var view=home.gameObject.AddComponent<HomeViewController>();view.Configure(flow,best,planetImage,planetName,stage,energy,fill,sprites,collection,settings,settingsClose,quit,cancel,confirm,toast.gameObject,settingsPanel.gameObject,quitPanel.gameObject);
        flow.Configure(home.gameObject,FilteredRoots(flow.GameRoots),session,best,stage,play,safe.Find("GameOverPanel/Card/HomeButton").GetComponent<Button>());
        var playingHome=safe.Find("PlayingHomeButton").GetComponent<Button>();var playingConfirm=safe.Find("ExitConfirm");flow.ConfigureNavigation(playingHome,null,playingConfirm.gameObject,playingConfirm.Find("Card/Continue").GetComponent<Button>(),playingConfirm.Find("Card/GoHome").GetComponent<Button>());flow.ConfigureHomeView(view);
        var dev=home.Find("DevButton");if(dev!=null)dev.SetAsLastSibling();var debug=home.Find("PlanetDebugPanel");if(debug!=null)debug.SetAsLastSibling();
        EditorUtility.SetDirty(flow);EditorUtility.SetDirty(view);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Validate();Debug.Log("SPRINT71_HOME_READY");
    }

    [MenuItem("COSMIC BLOCK/Sprint 7-1/Validate")]
    public static void Validate()
    {
        var safe=GameObject.Find("GameCanvas").transform.Find("SafeArea");var home=safe.Find("HomeRoot");var flow=GameObject.Find("GameCanvas").GetComponent<GameFlowController>();
        int homes=0;foreach(var go in SceneObjects())if(go.name=="HomeRoot")homes++;
        Require(homes==1&&home!=null&&flow.HomeRoot==home.gameObject,"one HomeRoot");
        Require(home.Find("HomeBackground")!=null&&home.Find("TitleArea/Title").GetComponent<Text>().text=="별빛 블록","background/title");
        Require(home.Find("MainActions/GameStartButton")!=null&&home.Find("MainActions/PlanetCollectionButton")!=null,"main actions");
        Require(home.Find("CurrentPlanetArea/Planet")!=null&&home.Find("CurrentPlanetArea/ProgressBar/Fill")!=null,"planet presentation");
        Require(home.Find("SettingsPanel")!=null&&home.Find("QuitConfirmPanel")!=null&&home.Find("DevButton")!=null,"bottom/modal/dev");
        Require(Array.IndexOf(flow.GameRoots,safe.Find("GameOverPanel").gameObject)<0,"GameOver independent");
        Require(Array.IndexOf(flow.GameRoots,safe.Find("PlanetCompletion").gameObject)<0,"Planet transition independent");
        Require(Array.IndexOf(flow.GameRoots,home.Find("SettingsPanel").gameObject)<0&&Array.IndexOf(flow.GameRoots,home.Find("QuitConfirmPanel").gameObject)<0,"Home modals independent");
        var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);Require(sprite!=null&&sprite.texture.width==940&&sprite.texture.height==1672,"background import");
    }

    static GameObject[] FilteredRoots(GameObject[] roots){var result=new System.Collections.Generic.List<GameObject>();if(roots!=null)foreach(var root in roots)if(root!=null&&root.name!="GameOverPanel"&&root.name!="ReachedFeedback"&&root.name!="PlanetCompletion")result.Add(root);return result.ToArray();}
    static void ConfigureBackground(){var importer=AssetImporter.GetAtPath(BackgroundPath) as TextureImporter;if(importer==null){AssetDatabase.ImportAsset(BackgroundPath,ImportAssetOptions.ForceSynchronousImport);importer=AssetImporter.GetAtPath(BackgroundPath) as TextureImporter;}if(importer==null)throw new Exception("STOP: Home background import failed.");importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaSource=TextureImporterAlphaSource.None;importer.mipmapEnabled=false;importer.filterMode=FilterMode.Bilinear;importer.textureCompression=TextureImporterCompression.CompressedHQ;importer.maxTextureSize=2048;importer.SaveAndReimport();}
    static RectTransform Modal(string name,Transform parent,out RectTransform card){var panel=UI(name,parent);Stretch(panel);panel.gameObject.AddComponent<Image>().color=new Color(.005f,.01f,.04f,.76f);card=UI("Card",panel);Set(card,new Vector2(.11f,.36f),new Vector2(.89f,.64f));card.gameObject.AddComponent<Image>().color=new Color(.018f,.035f,.10f,.98f);return panel;}
    static RectTransform UI(string name,Transform parent){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);return r;}
    static Image MakeImage(string name,Transform parent)=>UI(name,parent).gameObject.AddComponent<Image>();
    static Text Text(string name,Transform parent,string value,int size,Color color,FontStyle style){var t=UI(name,parent).gameObject.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=value;t.fontSize=size;t.fontStyle=style;t.alignment=TextAnchor.MiddleCenter;t.color=color;t.raycastTarget=false;t.resizeTextForBestFit=true;t.resizeTextMinSize=14;t.resizeTextMaxSize=size;return t;}
    static Button Button(string name,Transform parent,string label,int size,Color background,Color accent,bool strong){var r=UI(name,parent);var image=r.gameObject.AddComponent<Image>();image.color=background;var outline=r.gameObject.AddComponent<Outline>();outline.effectColor=accent;outline.effectDistance=strong?new Vector2(3,-3):new Vector2(1.5f,-1.5f);outline.useGraphicAlpha=false;var button=r.gameObject.AddComponent<Button>();button.targetGraphic=image;r.gameObject.AddComponent<ButtonPressFeedback>();var text=Text("Label",r,label,size,strong?Gold:White,FontStyle.Bold);Stretch(text.rectTransform);return button;}
    static void Shadow(Graphic graphic,Color color,Vector2 distance){var shadow=graphic.gameObject.AddComponent<Shadow>();shadow.effectColor=color;shadow.effectDistance=distance;shadow.useGraphicAlpha=true;}
    static void Stretch(RectTransform r)=>Set(r,Vector2.zero,Vector2.one);static void Set(RectTransform r,Vector2 min,Vector2 max){r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;r.localScale=Vector3.one;}
    static T Ensure<T>(GameObject go)where T:Component{var c=go.GetComponent<T>();return c==null?go.AddComponent<T>():c;}
    static GameObject[] SceneObjects(){var scene=SceneManager.GetActiveScene();var list=new System.Collections.Generic.List<GameObject>();foreach(var root in scene.GetRootGameObjects())foreach(var t in root.GetComponentsInChildren<Transform>(true))list.Add(t.gameObject);return list.ToArray();}
    static void Require(bool value,string message){if(!value)throw new Exception("Sprint 7-1 validation failed: "+message);}
}


