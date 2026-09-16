using System;
using System.IO;
using CosmicBlock.Board;
using CosmicBlock.Core;
using CosmicBlock.Effects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Sprint4Builder
{
    public const string ScenePath = "Assets/Scenes/Game.unity";
    public const string AudioPath = "Assets/Audio/SFX/clear.wav";

    [MenuItem("COSMIC BLOCK/Sprint 4/Build Juice And Feedback")]
    public static void Build()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene = EditorSceneManager.OpenScene(ScenePath);
        var canvas = GameObject.Find("GameCanvas");
        var safe = canvas.transform.Find("SafeArea");
        var board = UnityEngine.Object.FindAnyObjectByType<BoardView>();
        var session = UnityEngine.Object.FindAnyObjectByType<GameSession>();
        if (safe == null || board == null || session == null) throw new Exception("Sprint 3 foundation missing.");

        CreateClearWav();
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioPath);
        if (clip == null) throw new Exception("Clear SFX import failed.");

        var old = safe.Find("FeedbackLayer");
        if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);
        var root = UI("FeedbackLayer", safe);
        Stretch(root);
        root.SetSiblingIndex(board.transform.GetSiblingIndex() + 1);

        var cells = new Image[64];
        for (int i = 0; i < cells.Length; i++)
        {
            var rect = UI("ClearCell_" + i, root);
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            cells[i] = rect.gameObject.AddComponent<Image>();
            cells[i].raycastTarget = false;
            rect.gameObject.SetActive(false);
        }

        var stars = new Text[24];
        for (int i = 0; i < stars.Length; i++)
        {
            var rect = UI("Star_" + i, root);
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(42, 42);
            stars[i] = TextOn(rect, "*", 32, Color.white);
            rect.gameObject.SetActive(false);
        }

        var score = TextOn(UI("ScorePop", root), "+100", 48, new Color(1, .86f, .48f));
        SetupPop(score.rectTransform, new Vector2(300, 80));
        var combo = TextOn(UI("ComboPop", root), "CLEAR!", 38, new Color(1, .96f, .78f));
        SetupPop(combo.rectTransform, new Vector2(520, 74));
        score.gameObject.SetActive(false); combo.gameObject.SetActive(false);

        var source = root.gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false; source.loop = false; source.spatialBlend = 0; source.volume = .58f;
        var camera = Camera.main; if (camera != null && camera.GetComponent<AudioListener>() == null) camera.gameObject.AddComponent<AudioListener>();
        var controller = root.gameObject.AddComponent<GameFeedbackController>();
        controller.Configure(board, root, cells, stars, score, combo, source, clip);
        session.ConfigureFeedback(controller);
        EditorUtility.SetDirty(session); EditorUtility.SetDirty(controller);
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        Validate();
        Debug.Log("SPRINT4_SCENE_READY");
    }

    [MenuItem("COSMIC BLOCK/Sprint 4/Validate Feedback Scene")]
    public static void Validate()
    {
        var session = UnityEngine.Object.FindAnyObjectByType<GameSession>();
        var feedback = UnityEngine.Object.FindAnyObjectByType<GameFeedbackController>();
        Require(session != null && feedback != null && session.Feedback == feedback, "Session feedback binding");
        Require(feedback.CellPoolCapacity == 64 && feedback.StarPoolCapacity == 24, "Reusable pools");
        Require(GameObject.Find("GameCanvas").transform.Find("SafeArea/FeedbackLayer/ScorePop") != null, "Score Pop");
        Require(AssetDatabase.LoadAssetAtPath<AudioClip>(AudioPath) != null, "Clear SFX");
        Directory.CreateDirectory("Validation");
        File.WriteAllText("Validation/sprint4.txt", "PASS scene: FeedbackLayer, 64 clear cells, 24 pooled stars, score/combo pop, AudioSource and controller.\n");
    }

    static void CreateClearWav()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(AudioPath));
        const int rate = 44100;
        const float duration = .28f;
        int samples = Mathf.RoundToInt(rate * duration);
        using (var stream = File.Create(AudioPath))
        using (var writer = new BinaryWriter(stream))
        {
            int dataBytes = samples * 2;
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF")); writer.Write(36 + dataBytes);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt ")); writer.Write(16); writer.Write((short)1);
            writer.Write((short)1); writer.Write(rate); writer.Write(rate * 2); writer.Write((short)2); writer.Write((short)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data")); writer.Write(dataBytes);
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)rate;
                float envelope = Mathf.Pow(1f - i / (float)samples, 2.4f);
                float sparkle = Mathf.Sin(2 * Mathf.PI * (720 + 900 * t) * t) * .55f +
                                Mathf.Sin(2 * Mathf.PI * (1080 + 1250 * t) * t) * .28f +
                                Mathf.Sin(2 * Mathf.PI * 1680 * t) * .12f;
                writer.Write((short)Mathf.Clamp(sparkle * envelope * 15000, short.MinValue, short.MaxValue));
            }
        }
        AssetDatabase.ImportAsset(AudioPath, ImportAssetOptions.ForceSynchronousImport);
    }

    static RectTransform UI(string name, Transform parent)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false); return rect;
    }
    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
    }
    static void SetupPop(RectTransform rect, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f); rect.sizeDelta = size;
    }
    static Text TextOn(RectTransform rect, string value, int size, Color color)
    {
        var text = rect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.text = value; text.fontSize = size;
        text.alignment = TextAnchor.MiddleCenter; text.color = color; text.raycastTarget = false;
        text.resizeTextForBestFit = true; text.resizeTextMinSize = 16; text.resizeTextMaxSize = size;
        return text;
    }
    static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception("Sprint 4 test failed: " + message);
    }
}

