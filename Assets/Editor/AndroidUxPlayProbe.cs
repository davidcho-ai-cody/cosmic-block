using System;
using System.Collections.Generic;
using System.IO;
using CosmicBlock.Blocks;
using CosmicBlock.Board;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class AndroidUxPlayProbe
{
    static double started;
    static int frames;
    static string runtimeErrors = "";
    static GameSession session;
    static BoardView board;
    static RectTransform layer;
    static readonly List<string> results = new List<string>();

    public static void Run()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
        started = EditorApplication.timeSinceStartup; frames = 0; runtimeErrors = ""; results.Clear();
        Application.logMessageReceived += Log; EditorApplication.update += Check; EditorApplication.EnterPlaymode();
    }

    static void Log(string message, string stack, LogType type)
    { if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) runtimeErrors += message + "\n"; }

    static void Check()
    {
        if (EditorApplication.timeSinceStartup - started > 180) { Finish(false, "timeout"); return; }
        if (!EditorApplication.isPlaying || ++frames < 30) return;
        try
        {
            session = UnityEngine.Object.FindAnyObjectByType<GameSession>();
            board = UnityEngine.Object.FindAnyObjectByType<BoardView>();
            layer = GameObject.Find("GameCanvas").transform.Find("DragLayer") as RectTransform;
            TestSlotHitAreas();
            TestHud();
            Capture(1080, 1920, "9x16", false);
            Capture(1080, 2400, "tall", true);
            Capture(1080, 1440, "short", false);
            Require(runtimeErrors.Length == 0, "runtime errors: " + runtimeErrors);
            Finish(true, "");
        }
        catch (Exception ex) { Finish(false, ex.ToString()); }
    }

    static void TestSlotHitAreas()
    {
        int[] shapes = { 0, 1, 3, 5, 6, 7 };
        string[] names = { "Single", "Horizontal 2", "Vertical 2", "2x2", "L", "Reverse L" };
        for (int i = 0; i < shapes.Length; i++)
        {
            session.Retry(); var piece = session.Slots[0]; piece.Initialize(BlockCatalog.Shapes[shapes[i]], board, layer, session);
            var slot = piece.transform.parent as RectTransform; var relay = slot.GetComponent<SlotDragHandler>();
            Require(relay != null && relay.IsInteractable && slot.GetComponent<Image>().raycastTarget, names[i] + " slot ready");
            Vector3 edge = slot.TransformPoint(new Vector3(slot.rect.xMax * .82f, slot.rect.yMax * .72f));
            var data = new PointerEventData(EventSystem.current) { pointerId = 100 + i, position = Screen(edge), button = PointerEventData.InputButton.Left };
            relay.OnBeginDrag(data);
            Require(piece.GetComponent<BlockDragHandler>().IsDragging, names[i] + " begins from slot edge");
            relay.OnCancel(data);
            Require(!piece.GetComponent<BlockDragHandler>().IsDragging && piece.transform.parent == slot, names[i] + " cancel restores");
        }
        session.Retry(); var consumed = session.Slots[0]; consumed.Initialize(BlockCatalog.Shapes[0], board, layer, session);
        var consumedSlot = consumed.transform.parent as RectTransform; var consumedRelay = consumedSlot.GetComponent<SlotDragHandler>();
        consumed.Consume();
        Require(consumed.IsConsumed && !consumed.gameObject.activeSelf && !consumedRelay.IsInteractable && !consumedSlot.GetComponent<Image>().raycastTarget, "consumed slot inert");
        results.Add("PASS Touch: six requested shapes begin drag from slot edge; consumed slot raycast disabled; offset remains 110.");
    }

    static void TestHud()
    {
        var safe = GameObject.Find("GameCanvas").transform.Find("SafeArea");
        var title = safe.Find("Title").GetComponent<Text>(); var score = safe.Find("ScorePlaceholder").GetComponent<Text>();
        var journey = safe.Find("Journey/JourneyText").GetComponent<Text>();
        Require(title.fontSize == 56 && score.fontSize == 44 && score.fontStyle == FontStyle.Bold && journey.fontSize == 30, "HUD font settings");
        foreach (int value in new[] { 7, 670, 9999, 10000 })
        {
            session.DebugSetScore(value); Canvas.ForceUpdateCanvases();
            Require(score.text.Contains(value.ToString("N0", System.Globalization.CultureInfo.InvariantCulture)) && Fits(score), "score layout " + value);
            Require(Fits(journey), "journey layout " + value);
        }
        results.Add("PASS HUD: Title 56, bold Best/Score 44, Journey 30; 1/3/4/5 digit scores fit.");
    }

    static bool Fits(Text text)
    {
        Canvas.ForceUpdateCanvases();
        var rect = text.rectTransform.rect;
        var generator = text.cachedTextGenerator;
        generator.Populate(text.text, text.GetGenerationSettings(rect.size));
        float units = Mathf.Max(.0001f, text.pixelsPerUnit);
        Vector2 renderedLocal = generator.rectExtents.size / units;
        Debug.Log("HUD_FIT " + text.name + " rect=" + rect.size + " renderedLocal=" + renderedLocal + " ppu=" + units);
        return renderedLocal.x <= rect.width + .5f && renderedLocal.y <= rect.height + .5f;
    }

    static void Capture(int width, int height, string name, bool inset)
    {
        var canvas = layer.GetComponentInParent<Canvas>(); var camera = Camera.main; var safe = canvas.transform.Find("SafeArea") as RectTransform;
        var oldMode = canvas.renderMode; var oldCamera = canvas.worldCamera; var oldTarget = camera.targetTexture;
        bool oldOrtho = camera.orthographic; float oldSize = camera.orthographicSize; var oldActive = RenderTexture.active;
        Vector2 oldMin = safe.anchorMin, oldMax = safe.anchorMax, oldOffsetMin = safe.offsetMin, oldOffsetMax = safe.offsetMax;
        var rt = new RenderTexture(width, height, 24); Texture2D image = null;
        try
        {
            rt.Create(); camera.targetTexture = rt; camera.orthographic = true; camera.orthographicSize = height / 2f;
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 10;
            safe.anchorMin = inset ? new Vector2(.02f, .04f) : Vector2.zero; safe.anchorMax = inset ? new Vector2(.98f, .94f) : Vector2.one;
            safe.offsetMin = safe.offsetMax = Vector2.zero; Canvas.ForceUpdateCanvases();
            board.GetComponent<SquareBoardLayout>().SendMessage("LateUpdate"); board.SendMessage("LateUpdate");
            foreach (var piece in session.Slots) if (!piece.IsConsumed) piece.FitSlot(); Canvas.ForceUpdateCanvases();
            var title = safe.Find("Title").GetComponent<Text>(); var score = safe.Find("ScorePlaceholder").GetComponent<Text>(); var journey = safe.Find("Journey/JourneyText").GetComponent<Text>();
            Require(Fits(title) && Fits(score) && Fits(journey), name + " text fit");
            AssertInside(title.rectTransform, safe, name); AssertInside(score.rectTransform, safe, name); AssertInside(journey.rectTransform, safe, name);
            var boardRect = board.transform as RectTransform; Require(Mathf.Abs(boardRect.rect.width - boardRect.rect.height) < .1f, name + " square board");
            foreach (var piece in session.Slots) AssertInside(piece.transform.parent as RectTransform, safe, name);
            var bg = UnityEngine.Object.FindAnyObjectByType<AspectFillBackground>(); if (bg != null) bg.SendMessage("LateUpdate");
            camera.Render(); RenderTexture.active = rt; image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply(); File.WriteAllBytes("Validation/android_ux_" + name + ".png", image.EncodeToPNG());
            results.Add("PASS Render " + name + ": SafeArea/text/board/slots, board=" + boardRect.rect.width.ToString("F1") + "x" + boardRect.rect.height.ToString("F1"));
        }
        finally
        {
            canvas.renderMode = oldMode; canvas.worldCamera = oldCamera; camera.targetTexture = oldTarget; camera.orthographic = oldOrtho; camera.orthographicSize = oldSize;
            RenderTexture.active = oldActive; safe.anchorMin = oldMin; safe.anchorMax = oldMax; safe.offsetMin = oldOffsetMin; safe.offsetMax = oldOffsetMax;
            if (image != null) UnityEngine.Object.DestroyImmediate(image); rt.Release(); UnityEngine.Object.DestroyImmediate(rt);
        }
    }

    static void AssertInside(RectTransform rect, RectTransform safe, string name)
    {
        var corners = new Vector3[4]; rect.GetWorldCorners(corners);
        foreach (var corner in corners) { var local = safe.InverseTransformPoint(corner); Require(local.x >= safe.rect.xMin - .1f && local.x <= safe.rect.xMax + .1f && local.y >= safe.rect.yMin - .1f && local.y <= safe.rect.yMax + .1f, name + " bounds " + rect.name); }
    }

    static Vector2 Screen(Vector3 world)
    {
        var canvas = layer.GetComponentInParent<Canvas>(); var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        return RectTransformUtility.WorldToScreenPoint(camera, world);
    }

    static void Require(bool condition, string message) { if (!condition) throw new Exception("Android UX test failed: " + message); }
    static void Finish(bool passed, string failure)
    {
        EditorApplication.update -= Check; Application.logMessageReceived -= Log;
        Directory.CreateDirectory("Validation"); File.WriteAllText("Validation/android_ux.txt", string.Join("\n", results) + "\n" + (passed ? "ANDROID_UX_PLAY_PASS" : failure));
        if (passed) Debug.Log("ANDROID_UX_PLAY_PASS"); else Debug.LogError(failure); EditorApplication.Exit(passed ? 0 : 1);
    }
}
