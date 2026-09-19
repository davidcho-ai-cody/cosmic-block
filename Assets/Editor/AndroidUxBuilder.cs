using System;
using CosmicBlock.Blocks;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class AndroidUxBuilder
{
    public const string ScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("COSMIC BLOCK/Android QA/Apply Touch And HUD Readability")]
    public static void Build()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene = EditorSceneManager.OpenScene(ScenePath);
        var safe = GameObject.Find("GameCanvas").transform.Find("SafeArea");
        var title = safe.Find("Title").GetComponent<Text>();
        var score = safe.Find("ScorePlaceholder").GetComponent<Text>();
        var combo = safe.Find("ComboText").GetComponent<Text>();
        var journey = safe.Find("Journey") as RectTransform;
        var journeyText = journey.Find("JourneyText").GetComponent<Text>();

        title.fontSize = 56; title.resizeTextForBestFit = true; title.resizeTextMinSize = 42; title.resizeTextMaxSize = 56;
        Set(title.rectTransform, new Vector2(.03f, .925f), new Vector2(.97f, .985f));
        score.fontSize = 44; score.fontStyle = FontStyle.Bold; score.resizeTextForBestFit = true; score.resizeTextMinSize = 30; score.resizeTextMaxSize = 44;
        Set(score.rectTransform, new Vector2(.03f, .875f), new Vector2(.97f, .922f));
        Set(combo.rectTransform, new Vector2(.08f, .765f), new Vector2(.92f, .798f));
        journeyText.fontSize = 30; journeyText.resizeTextForBestFit = true; journeyText.resizeTextMaxSize = 30;
        Set(journey, new Vector2(.05f, .800f), new Vector2(.95f, .872f));

        var area = safe.Find("BlockArea");
        foreach (Transform child in area)
        {
            var relay = child.GetComponent<SlotDragHandler>();
            if (relay == null) relay = child.gameObject.AddComponent<SlotDragHandler>();
            relay.Configure(child.GetComponentInChildren<BlockPiece>(true));
        }

        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        Validate(); Debug.Log("ANDROID_UX_SCENE_READY");
    }

    [MenuItem("COSMIC BLOCK/Android QA/Validate Touch And HUD Readability")]
    public static void Validate()
    {
        var safe = GameObject.Find("GameCanvas").transform.Find("SafeArea");
        Require(safe.Find("Title").GetComponent<Text>().fontSize == 56, "Title 56");
        Require(safe.Find("ScorePlaceholder").GetComponent<Text>().fontSize == 44, "Score 44");
        Require(safe.Find("Journey/JourneyText").GetComponent<Text>().fontSize == 30, "Journey 30");
        foreach (Transform child in safe.Find("BlockArea")) Require(child.GetComponent<SlotDragHandler>() != null, "Slot relay " + child.name);
    }

    static void Set(RectTransform rect, Vector2 min, Vector2 max)
    { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero; }
    static void Require(bool condition, string message)
    { if (!condition) throw new Exception("Android UX scene validation failed: " + message); }
}
