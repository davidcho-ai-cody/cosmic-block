using System;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public static class Sprint8CollectionBuilder
{
    const string Art="Assets/Art/UI/Collection/";
    static readonly string[] Names={"collection_title","collection_back_icon","collection_planet_name_frame","collection_prev_icon","collection_next_icon","collection_stage_frame","collection_stage_slot","collection_lock_icon","collection_message_frame"};
    static readonly string[] Stages={"Desolate","Awakening","Recovering","Thriving","Restored"};
    public static void Build()
    {
        AuditAssets();AssetDatabase.Refresh();
        foreach(string n in Names)
        {
            string p=Art+n+".png";var t=AssetImporter.GetAtPath(p)as TextureImporter;
            if(t==null)throw new Exception("STOP: missing Collection asset "+p);
            t.textureType=TextureImporterType.Sprite;t.spriteImportMode=SpriteImportMode.Single;t.alphaSource=TextureImporterAlphaSource.FromInput;
            t.alphaIsTransparency=true;t.mipmapEnabled=false;t.filterMode=FilterMode.Bilinear;t.wrapMode=TextureWrapMode.Clamp;t.textureCompression=TextureImporterCompression.Uncompressed;t.maxTextureSize=n.Contains("icon")||n.Contains("slot")?1024:2048;
            var a=t.GetPlatformTextureSettings("Android");a.overridden=true;a.maxTextureSize=t.maxTextureSize;a.format=TextureImporterFormat.ASTC_6x6;a.compressionQuality=100;t.SetPlatformTextureSettings(a);t.SaveAndReimport();
        }
        string originalScene=System.IO.File.ReadAllText("Assets/Scenes/Game.unity");var scene=EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");var canvas=GameObject.Find("GameCanvas");var safe=canvas.transform.Find("SafeArea");var flow=canvas.GetComponent<GameFlowController>();
        if(safe==null||flow==null||safe.Find("HomeRoot")==null)throw new Exception("STOP: missing existing HOME foundation");
        var old=safe.Find("CollectionRoot");if(old!=null)UnityEngine.Object.DestroyImmediate(old.gameObject);
        var root=UI("CollectionRoot",safe,0,0,1,1);
        var bg=Image("Background",root,"Assets/Art/Backgrounds/Home_Background_StarlightBlock.png",0,0,1,1);var aspect=bg.gameObject.AddComponent<AspectFillBackground>();var so=new SerializedObject(aspect);so.FindProperty("referenceSize").vector2Value=new Vector2(940,1672);so.FindProperty("includeUnsafeArea").boolValue=true;so.ApplyModifiedPropertiesWithoutUndo();
        var shade=UI("Shade",root,0,0,1,1).gameObject.AddComponent<Image>();shade.color=new Color(0,0,.03f,.12f);shade.raycastTarget=false;
        Image("Title",root,Art+Names[0]+".png",.10f,.845f,.90f,.985f);
        var back=Button("Back",root,Names[1],.02f,.88f,.16f,.975f);
        var nameFrame=Image("NameFrame",root,Art+Names[2]+".png",.21f,.72f,.79f,.85f);Fit(nameFrame);
        var number=Text("Number",nameFrame.transform,"",28,.29f,.67f,.71f,.87f);
        var name=Text("Name",nameFrame.transform,"",46,.16f,.22f,.84f,.64f);
        var hero=Image("Hero",root,null,.14f,.405f,.86f,.725f);
        var heroLock=Image("HeroLock",root,Art+Names[7]+".png",.34f,.455f,.66f,.66f);
        var prev=Button("Previous",root,Names[3],.01f,.56f,.16f,.665f);
        var next=Button("Next",root,Names[4],.84f,.56f,.99f,.665f);
        var state=Text("Status",root,"",28,.10f,.365f,.90f,.405f);
        var track=UI("Progress",root,.26f,.348f,.74f,.357f).gameObject.AddComponent<Image>();track.color=new Color(.025f,.06f,.14f,.9f);track.raycastTarget=false;
        var fill=UI("Fill",track.transform,0,0,1,1).gameObject.AddComponent<Image>();fill.color=new Color(.3f,.85f,1,1);fill.raycastTarget=false;
        var stageArea=UI("StageArea",root,.035f,.135f,.965f,.345f);
        var frame=Image("Frame",stageArea,Art+Names[5]+".png",0,0,1,1);Fit(frame);
        var buttons=new Button[5];var thumbs=new Image[5];var locks=new GameObject[5];var labels=new Text[5];var sprites=new Sprite[5];
        for(int i=0;i<5;i++)
        {
            sprites[i]=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Planets/Planet01/Planet01_Stage0"+(i+1)+"_"+Stages[i]+".png");if(sprites[i]==null)throw new Exception("STOP: missing existing planet stage "+(i+1));
            float center=.14f+i*.18f;
            var slot=UI("Stage"+(i+1),frame.transform,center-.075f,.48f,center+.075f,.76f);
            var hit=slot.gameObject.AddComponent<Image>();hit.color=Color.clear;hit.raycastPadding=new Vector4(0,-70,0,-15);buttons[i]=slot.gameObject.AddComponent<Button>();buttons[i].targetGraphic=hit;buttons[i].transition=Selectable.Transition.None;
            thumbs[i]=Image("Thumbnail",slot,null,.05f,.04f,.95f,.94f);
            if(i<4){var line=Image("Connector"+(i+1),frame.transform,null,center+.065f,.618f,center+.115f,.626f);line.color=new Color(.8f,.78f,.35f,.65f);line.transform.SetAsFirstSibling();}
            var ring=UI("CurrentRing",slot,0,0,1,1).gameObject.AddComponent<CollectionStageRing>();ring.raycastTarget=false;ring.color=new Color(.35f,.85f,1,.8f);
            locks[i]=Image("Lock",slot,Art+Names[7]+".png",.59f,.02f,.99f,.42f).gameObject;
            Text("StageNumber"+(i+1),frame.transform,(i+1)+"단계",18,center-.083f,.39f,center+.083f,.48f);labels[i]=Text("StageLabel"+(i+1),frame.transform,"",25,center-.083f,.29f,center+.083f,.39f);
        }
        var loreFrame=Image("MessageFrame",root,Art+Names[8]+".png",.08f,.005f,.92f,.13f);Fit(loreFrame);
        var lore=Text("Message",loreFrame.transform,"",27,.14f,.25f,.86f,.65f);
        var view=root.gameObject.AddComponent<PlanetCollectionView>();view.Configure(flow,sprites,hero,fill,heroLock.gameObject,number,name,state,lore,back,prev,next,buttons,thumbs,locks,labels);flow.ConfigureCollection(view);
        root.gameObject.SetActive(false);PreserveHomeBackgroundSize(originalScene,safe);EditorUtility.SetDirty(flow);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("SPRINT8_COLLECTION_READY");
    }
            static void PreserveHomeBackgroundSize(string source,Transform safe)
    {
        var background=safe.Find("HomeRoot/HomeBackground") as RectTransform;
        if(background==null)return;
        long id=0;
        foreach(System.Text.RegularExpressions.Match candidate in System.Text.RegularExpressions.Regex.Matches(source,@"(?ms)^--- !u!224 &(\d+)\r?\n.*?(?=^---|\z)"))
        {
            var gameObject=System.Text.RegularExpressions.Regex.Match(candidate.Value,@"m_GameObject: \{fileID: (\d+)\}");
            if(!gameObject.Success)continue;
            var owner=System.Text.RegularExpressions.Regex.Match(source,@"(?ms)^--- !u!1 &"+gameObject.Groups[1].Value+@"\r?\n.*?(?=^---|\z)");
            if(owner.Value.Contains("m_Name: HomeBackground")){id=long.Parse(candidate.Groups[1].Value);break;}
        }
        var block=System.Text.RegularExpressions.Regex.Match(source,@"(?ms)^--- !u!224 &"+id+@"\r?\n.*?(?=^---|\z)");
        var size=System.Text.RegularExpressions.Regex.Match(block.Value,@"m_SizeDelta: \{x: ([^,]+), y: ([^}]+)\}");
        if(size.Success)background.sizeDelta=new Vector2(float.Parse(size.Groups[1].Value,System.Globalization.CultureInfo.InvariantCulture),float.Parse(size.Groups[2].Value,System.Globalization.CultureInfo.InvariantCulture));
    }
    public static void AuditAssets()
    {
        var output=new System.Collections.Generic.List<string>();
        foreach(string name in Names)
        {
            string path=Art+name+".png";var tex=new Texture2D(2,2,TextureFormat.RGBA32,false);
            try
            {
                tex.LoadImage(System.IO.File.ReadAllBytes(path));long clear=0,partial=0,opaque=0;
                foreach(var pixel in tex.GetPixels32()){if(pixel.a==0)clear++;else if(pixel.a==255)opaque++;else partial++;}
                if(clear==0||partial==0)throw new Exception("STOP: Collection alpha invalid "+path);
                output.Add(name+".png "+tex.width+"x"+tex.height+" transparent="+clear+" partial="+partial+" opaque="+opaque);
            }
            finally{UnityEngine.Object.DestroyImmediate(tex);}
        }
        System.IO.Directory.CreateDirectory("Validation");System.IO.File.WriteAllLines("Validation/sprint8_asset_alpha.txt",output);Debug.Log("SPRINT8_ASSET_ALPHA_PASS");
    }
    static void Fit(Image image){var r=image.rectTransform;var holder=UI(image.name+"Area",r.parent,r.anchorMin.x,r.anchorMin.y,r.anchorMax.x,r.anchorMax.y);holder.SetSiblingIndex(r.GetSiblingIndex());r.SetParent(holder,false);r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;var fit=image.gameObject.AddComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=image.sprite.rect.width/image.sprite.rect.height;}
    static RectTransform UI(string name,Transform parent,float x0,float y0,float x1,float y1){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=new Vector2(x0,y0);r.anchorMax=new Vector2(x1,y1);r.offsetMin=r.offsetMax=Vector2.zero;return r;}
    static Image Image(string name,Transform parent,string path,float x0,float y0,float x1,float y1){var i=UI(name,parent,x0,y0,x1,y1).gameObject.AddComponent<Image>();if(path!=null)i.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);i.preserveAspect=true;i.raycastTarget=false;return i;}
    static Button Button(string name,Transform parent,string asset,float x0,float y0,float x1,float y1){var r=UI(name,parent,x0,y0,x1,y1);var hit=r.gameObject.AddComponent<Image>();hit.color=Color.clear;var b=r.gameObject.AddComponent<Button>();b.targetGraphic=hit;Image("Icon",r,Art+asset+".png",.05f,.05f,.95f,.95f);return b;}
    static Text Text(string name,Transform parent,string value,int size,float x0,float y0,float x1,float y1){var t=UI(name,parent,x0,y0,x1,y1).gameObject.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=value;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.alignment=TextAnchor.MiddleCenter;t.color=new Color(.96f,.98f,1);t.raycastTarget=false;t.resizeTextForBestFit=true;t.resizeTextMinSize=15;t.resizeTextMaxSize=size;return t;}
}
