using System;
using System.Collections.Generic;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Sprint73HomeBuilder
{
    public const string ScenePath="Assets/Scenes/Game.unity";
    const string Art="Assets/Art/UI/Home/";
    const string BackgroundPath="Assets/Art/Backgrounds/Home_Background_StarlightBlock.png";
    static readonly string[] AssetNames={"home_title_starlight_block.png","home_planet_hero.png","home_play_button.png","home_collection_button.png","home_best_score_frame.png","home_settings_icon.png","home_exit_icon.png"};
    static readonly string[] PlanetPaths={"Assets/Art/Planets/Planet01/Planet01_Stage01_Desolate.png","Assets/Art/Planets/Planet01/Planet01_Stage02_Awakening.png","Assets/Art/Planets/Planet01/Planet01_Stage03_Recovering.png","Assets/Art/Planets/Planet01/Planet01_Stage04_Thriving.png","Assets/Art/Planets/Planet01/Planet01_Stage05_Restored.png"};
    static readonly Color Gold=new Color(.98f,.75f,.23f,1),White=new Color(.96f,.98f,1,1),Blue=new Color(.54f,.86f,1,1),Navy=new Color(.008f,.024f,.075f,.96f);

    [MenuItem("COSMIC BLOCK/Sprint 7-3/Rebuild Release Home")]
    public static void Build()
    {
        ConfigureImports();AssetDatabase.Refresh();
        var scene=EditorSceneManager.OpenScene(ScenePath);var canvas=GameObject.Find("GameCanvas");var safe=canvas.transform.Find("SafeArea");var home=safe.Find("HomeRoot") as RectTransform;
        var flow=canvas.GetComponent<GameFlowController>();var session=UnityEngine.Object.FindAnyObjectByType<GameSession>();
        if(home==null||flow==null||session==null)throw new Exception("STOP: Sprint 7.3 HOME foundation missing.");
        int count=0;foreach(var go in SceneObjects())if(go.name=="HomeRoot")count++;if(count!=1)throw new Exception("STOP: expected exactly one HomeRoot, found "+count);
        for(int i=home.childCount-1;i>=0;i--){var c=home.GetChild(i);if(c.name!="DevButton"&&c.name!="PlanetDebugPanel")UnityEngine.Object.DestroyImmediate(c.gameObject);}
        foreach(var c in home.GetComponents<HomeViewController>())UnityEngine.Object.DestroyImmediate(c);foreach(var c in home.GetComponents<HomeAmbientMotion>())UnityEngine.Object.DestroyImmediate(c);foreach(var c in home.GetComponents<HomeFinalLayout>())UnityEngine.Object.DestroyImmediate(c);
        var rootImage=Ensure<Image>(home.gameObject);rootImage.color=Color.clear;rootImage.raycastTarget=false;

        var bg=Image("HomeBackground",home,BackgroundPath);Stretch(bg.rectTransform);bg.preserveAspect=true;bg.raycastTarget=false;var aspect=bg.gameObject.AddComponent<AspectFillBackground>();var aso=new SerializedObject(aspect);aso.FindProperty("referenceSize").vector2Value=new Vector2(940,1672);aso.FindProperty("includeUnsafeArea").boolValue=true;aso.ApplyModifiedPropertiesWithoutUndo();bg.transform.SetAsFirstSibling();

        var titleArea=UI("TitleArea",home);Set(titleArea,new Vector2(.055f,.795f),new Vector2(.945f,.965f));var title=Image("Title",titleArea,Art+AssetNames[0]);Stretch(title.rectTransform);title.preserveAspect=true;title.raycastTarget=false;

        var heroArea=UI("HeroArea",home);Set(heroArea,new Vector2(.08f,.485f),new Vector2(.92f,.79f));
        var hero=Image("HeroPlanet",heroArea,Art+"home_planet_hero.png");Stretch(hero.rectTransform);hero.preserveAspect=true;hero.raycastTarget=false;

        var actions=UI("MainActions",home);Set(actions,new Vector2(.035f,.27f),new Vector2(.965f,.525f));var play=ImageButton("GameStartButton",actions,Art+"home_play_button.png");Set(play.GetComponent<RectTransform>(),new Vector2(0,.54f),Vector2.one);var collection=ImageButton("PlanetCollectionButton",actions,Art+"home_collection_button.png");Set(collection.GetComponent<RectTransform>(),new Vector2(.04f,0),new Vector2(.96f,.45f));

        var bestArea=UI("BestScoreArea",home);Set(bestArea,new Vector2(.08f,.105f),new Vector2(.92f,.245f));var badge=Image("Frame",bestArea,Art+"home_best_score_frame.png");Stretch(badge.rectTransform);badge.preserveAspect=true;badge.raycastTarget=false;var scoreArea=UI("ScoreValueArea",badge.transform);Set(scoreArea,new Vector2(.55f,.31f),new Vector2(.86f,.59f));var best=Text("ScoreValue",scoreArea,"0",40,White,FontStyle.Bold);Stretch(best.rectTransform);best.alignment=TextAnchor.MiddleCenter;best.resizeTextMinSize=19;best.resizeTextMaxSize=40;Shadow(best,new Color(0,0,.08f,.95f),new Vector2(2,-2));

        var bottom=UI("BottomActions",home);Set(bottom,new Vector2(.08f,.015f),new Vector2(.92f,.10f));var settings=IconButton("SettingsButton",bottom,Art+AssetNames[5],"설정");Set(settings.GetComponent<RectTransform>(),Vector2.zero,new Vector2(.42f,1));var quit=IconButton("QuitButton",bottom,Art+AssetNames[6],"게임 종료");Set(quit.GetComponent<RectTransform>(),new Vector2(.58f,0),Vector2.one);

        var toast=UI("CollectionToast",home);Set(toast,new Vector2(.20f,.50f),new Vector2(.80f,.565f));var toastBg=toast.gameObject.AddComponent<Image>();toastBg.sprite=StarlightUiTheme.RoundedSprite;toastBg.type=UnityEngine.UI.Image.Type.Sliced;toastBg.color=Navy;var toastText=Text("Text",toast,"행성 도감은 준비 중입니다.",22,White,FontStyle.Bold);Stretch(toastText.rectTransform);toast.gameObject.SetActive(false);
        var settingsPanel=Modal("SettingsPanel",home,out var settingsCard);var settingsTitle=Text("Title",settingsCard,"설정",32,Gold,FontStyle.Bold);Set(settingsTitle.rectTransform,new Vector2(.08f,.72f),new Vector2(.92f,.92f));var sfx=Text("Sfx",settingsCard,"효과음                         ON",23,White,FontStyle.Normal);Set(sfx.rectTransform,new Vector2(.10f,.48f),new Vector2(.90f,.67f));var haptic=Text("Haptic",settingsCard,"진동                             ON",23,White,FontStyle.Normal);Set(haptic.rectTransform,new Vector2(.10f,.30f),new Vector2(.90f,.49f));var settingsClose=TextButton("Close",settingsCard,"닫기",23,true);Set(settingsClose.GetComponent<RectTransform>(),new Vector2(.23f,.08f),new Vector2(.77f,.25f));settingsPanel.gameObject.SetActive(false);
        var quitPanel=Modal("QuitConfirmPanel",home,out var quitCard);var quitQuestion=Text("Question",quitCard,"게임을 종료할까요?",28,White,FontStyle.Bold);Set(quitQuestion.rectTransform,new Vector2(.08f,.57f),new Vector2(.92f,.87f));var cancel=TextButton("Cancel",quitCard,"취소",23,true);Set(cancel.GetComponent<RectTransform>(),new Vector2(.08f,.14f),new Vector2(.46f,.40f));var confirm=TextButton("Confirm",quitCard,"종료",23,false);Set(confirm.GetComponent<RectTransform>(),new Vector2(.54f,.14f),new Vector2(.92f,.40f));quitPanel.gameObject.SetActive(false);

        var view=home.gameObject.AddComponent<HomeViewController>();view.Configure(flow,best,null,null,null,null,null,null,null,collection,settings,settingsClose,quit,cancel,confirm,toast.gameObject,settingsPanel.gameObject,quitPanel.gameObject);
        var layout=home.gameObject.AddComponent<HomeFinalLayout>();layout.Configure(home,actions,play.GetComponent<RectTransform>(),collection.GetComponent<RectTransform>(),bestArea,bottom,VisibleBounds(Art+"home_play_button.png").yMin,VisibleBounds(Art+"home_collection_button.png").yMax);var motion=home.gameObject.AddComponent<HomeAmbientMotion>();motion.Configure(hero.rectTransform,title.rectTransform);
        flow.Configure(home.gameObject,FilteredRoots(flow.GameRoots),session,best,null,play,safe.Find("GameOverPanel/Card/HomeButton").GetComponent<Button>());var playingHome=safe.Find("PlayingHomeButton").GetComponent<Button>();var playingConfirm=safe.Find("ExitConfirm");flow.ConfigureNavigation(playingHome,null,playingConfirm.gameObject,playingConfirm.Find("Card/Continue").GetComponent<Button>(),playingConfirm.Find("Card/GoHome").GetComponent<Button>());flow.ConfigureHomeView(view);
        var dev=home.Find("DevButton");if(dev!=null){Set(dev.GetComponent<RectTransform>(),new Vector2(.445f,.025f),new Vector2(.555f,.07f));dev.SetAsLastSibling();}var debug=home.Find("PlanetDebugPanel");if(debug!=null)debug.SetAsLastSibling();
        EditorUtility.SetDirty(flow);EditorUtility.SetDirty(view);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Validate();Debug.Log("SPRINT73_HOME_READY");
    }

    [MenuItem("COSMIC BLOCK/Sprint 7-3/Validate")]
    public static void Validate(){var home=GameObject.Find("GameCanvas").transform.Find("SafeArea/HomeRoot");Require(home.Find("TitleArea/Title").GetComponent<Image>().sprite.name=="home_title_starlight_block","title asset");Require(home.Find("HeroArea/HeroPlanet").GetComponent<Image>().sprite.name=="home_planet_hero","fixed hero asset");Require(home.Find("CurrentPlanetArea")==null,"legacy stage card removed");Require(home.Find("EnergyArea")==null,"HOME energy removed");Require(home.Find("MainActions/GameStartButton").GetComponent<Image>().sprite.name=="home_play_button","play asset");Require(home.Find("MainActions/PlanetCollectionButton").GetComponent<Image>().sprite.name=="home_collection_button","collection asset");Require(home.Find("BestScoreArea/Frame").GetComponent<Image>().sprite.name=="home_best_score_frame","best asset");Require(home.Find("BottomActions/SettingsButton/Icon").GetComponent<Image>().sprite.name=="home_settings_icon"&&home.Find("BottomActions/QuitButton/Icon").GetComponent<Image>().sprite.name=="home_exit_icon","utility assets");Require(home.Find("SettingsPanel")!=null&&home.Find("QuitConfirmPanel")!=null&&home.Find("DevButton")!=null,"existing flows");}
    static void ConfigureImports(){foreach(string name in AssetNames){string path=Art+name;var importer=AssetImporter.GetAtPath(path) as TextureImporter;if(importer==null){AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);importer=AssetImporter.GetAtPath(path) as TextureImporter;}if(importer==null)throw new Exception("STOP: missing HOME asset "+path);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=name.Contains("icon")?1024:2048;var android=importer.GetPlatformTextureSettings("Android");android.overridden=true;android.maxTextureSize=importer.maxTextureSize;android.format=TextureImporterFormat.ASTC_6x6;android.compressionQuality=100;importer.SetPlatformTextureSettings(android);importer.SaveAndReimport();}}
    static Rect VisibleBounds(string path)
    {
        var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);
        try
        {
            texture.LoadImage(System.IO.File.ReadAllBytes(path));var pixels=texture.GetPixels32();int minX=texture.width,minY=texture.height,maxX=0,maxY=0;
            for(int y=0;y<texture.height;y++)for(int x=0;x<texture.width;x++)if(pixels[y*texture.width+x].a>=77){minX=Mathf.Min(minX,x);maxX=Mathf.Max(maxX,x);minY=Mathf.Min(minY,y);maxY=Mathf.Max(maxY,y);}
            return Rect.MinMaxRect((float)minX/texture.width,(float)minY/texture.height,(float)(maxX+1)/texture.width,(float)(maxY+1)/texture.height);
        }
        finally{UnityEngine.Object.DestroyImmediate(texture);}
    }
    static GameObject[] FilteredRoots(GameObject[] roots){var list=new List<GameObject>();if(roots!=null)foreach(var r in roots)if(r!=null&&r.name!="GameOverPanel"&&r.name!="ReachedFeedback"&&r.name!="PlanetCompletion")list.Add(r);return list.ToArray();}
    static RectTransform Modal(string name,Transform parent,out RectTransform card){var panel=UI(name,parent);Stretch(panel);panel.gameObject.AddComponent<Image>().color=new Color(.005f,.01f,.04f,.78f);card=UI("Card",panel);Set(card,new Vector2(.11f,.36f),new Vector2(.89f,.64f));var image=card.gameObject.AddComponent<Image>();image.sprite=StarlightUiTheme.RoundedSprite;image.type=UnityEngine.UI.Image.Type.Sliced;image.color=Navy;return panel;}
    static Button ImageButton(string name,Transform parent,string path){var image=Image(name,parent,path);image.preserveAspect=true;var button=image.gameObject.AddComponent<Button>();button.targetGraphic=image;image.gameObject.AddComponent<ButtonPressFeedback>();return button;}
    static Button IconButton(string name,Transform parent,string path,string label){var root=UI(name,parent);var button=root.gameObject.AddComponent<Button>();var clear=root.gameObject.AddComponent<Image>();clear.color=Color.clear;button.targetGraphic=clear;root.gameObject.AddComponent<ButtonPressFeedback>();var icon=Image("Icon",root,path);Set(icon.rectTransform,new Vector2(0,.04f),new Vector2(.372f,.96f));icon.preserveAspect=true;icon.raycastTarget=false;var text=Text("Label",root,label,32,White,FontStyle.Bold);Set(text.rectTransform,new Vector2(.405f,.08f),new Vector2(1,.92f));text.alignment=TextAnchor.MiddleLeft;text.resizeTextMinSize=28;return button;}
    static Button TextButton(string name,Transform parent,string label,int size,bool primary){var root=UI(name,parent);var image=root.gameObject.AddComponent<Image>();image.sprite=StarlightUiTheme.RoundedSprite;image.type=UnityEngine.UI.Image.Type.Sliced;image.color=primary?new Color(.07f,.18f,.42f,.98f):new Color(.34f,.18f,.07f,.98f);var button=root.gameObject.AddComponent<Button>();button.targetGraphic=image;root.gameObject.AddComponent<ButtonPressFeedback>();var t=Text("Label",root,label,size,primary?White:Gold,FontStyle.Bold);Stretch(t.rectTransform);return button;}
    static Image Image(string name,Transform parent,string path){var image=UI(name,parent).gameObject.AddComponent<Image>();if(path!=null)image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);return image;}
    static Text Text(string name,Transform parent,string value,int size,Color color,FontStyle style){var t=UI(name,parent).gameObject.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=value;t.fontSize=size;t.fontStyle=style;t.alignment=TextAnchor.MiddleCenter;t.color=color;t.raycastTarget=false;t.resizeTextForBestFit=true;t.resizeTextMinSize=12;t.resizeTextMaxSize=size;return t;}
    static void Shadow(Graphic g,Color c,Vector2 d){var s=g.gameObject.AddComponent<Shadow>();s.effectColor=c;s.effectDistance=d;s.useGraphicAlpha=true;}
    static RectTransform UI(string name,Transform parent){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);return r;}static void Stretch(RectTransform r)=>Set(r,Vector2.zero,Vector2.one);static void Set(RectTransform r,Vector2 min,Vector2 max){r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;r.localScale=Vector3.one;}static T Ensure<T>(GameObject go)where T:Component{var c=go.GetComponent<T>();return c==null?go.AddComponent<T>():c;}
    static GameObject[] SceneObjects(){var list=new List<GameObject>();foreach(var root in SceneManager.GetActiveScene().GetRootGameObjects())foreach(var t in root.GetComponentsInChildren<Transform>(true))list.Add(t.gameObject);return list.ToArray();}static void Require(bool v,string m){if(!v)throw new Exception("Sprint 7.3 validation failed: "+m);}
}
