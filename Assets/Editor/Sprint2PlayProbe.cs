using System;
using System.Collections.Generic;
using System.Globalization;
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

public static class Sprint2PlayProbe
{
    private const string TestKey = "CosmicBlock.Tests.Sprint2.BestScore";
    private static bool hadTestKey;
    private static int previousTestBest;
    private static double started;
    private static int frames, stage;
    private static string runtimeErrors;
    private static GameSession session;
    private static BoardView board;
    private static RectTransform layer;
    private static BlockPiece[] pieces;
    private static readonly List<string> results = new List<string>();

    // Batch-only. Saved scenes and the player's production Best Score are untouched.
    public static void Run()
    {
        Sprint2Builder.Validate();
        EditorSceneManager.OpenScene(Sprint2Builder.ScenePath);
        var sceneSession = UnityEngine.Object.FindAnyObjectByType<GameSession>();
        var serialized = new SerializedObject(sceneSession);
        serialized.FindProperty("bestScoreKey").stringValue = TestKey;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        hadTestKey = PlayerPrefs.HasKey(TestKey);
        previousTestBest = PlayerPrefs.GetInt(TestKey, 0);
        PlayerPrefs.SetInt(TestKey, 123); PlayerPrefs.Save();
        started = EditorApplication.timeSinceStartup;
        frames = stage = 0; runtimeErrors = ""; results.Clear();
        Application.logMessageReceived += Log;
        EditorApplication.update += Check;
        EditorApplication.EnterPlaymode();
    }
    private static void Log(string message, string stack, LogType type)
    { if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) runtimeErrors += message + "\n"; }
    private static void Check()
    {
        if (EditorApplication.timeSinceStartup - started > 180) { Finish(false, "Play Mode timeout"); return; }
        if (!EditorApplication.isPlaying || ++frames < 30) return;
        try
        {
            if (stage == 0)
            {
                session = UnityEngine.Object.FindAnyObjectByType<GameSession>();
                board = UnityEngine.Object.FindAnyObjectByType<BoardView>();
                layer = GameObject.Find("GameCanvas").transform.Find("DragLayer") as RectTransform;
                pieces = new BlockPiece[session.Slots.Count];
                for (int i = 0; i < pieces.Length; i++) pieces[i] = session.Slots[i];
                Require(pieces.Length == 3 && board.CellCount == 64 && board.transform.childCount == 64 &&
                        session.State == GameState.Playing && session.Score == 0 && session.BestScore == 123, "Initial state/Best read");
                Require(AllReady(), "Initial random three");
                RaycastHit(pieces[0].gameObject);
                TestLoop();
                stage = 1; frames = 0; return;
            }
            if (stage == 1)
            {
                TestRetry();
                stage = 2; frames = 0; return;
            }
            foreach (var piece in pieces)
            {
                Require(piece.transform.childCount == piece.Shape.Cells.Count, "No stale cells after deferred Destroy");
                Require(!piece.GetComponent<BlockDragHandler>().IsDragging && piece.transform.parent.name.StartsWith("Slot_"),
                    "No drag-layer ghost");
            }
            AssertBoardVisuals();
            Require(runtimeErrors.Length == 0, "Runtime errors: " + runtimeErrors);
            results.Add("PASS: deferred old visuals removed; model/view match; no drag ghost or runtime errors.");
            Finish(true, "");
        }
        catch (Exception ex) { Finish(false, ex.ToString()); }
    }
    private static void TestLoop()
    {
        Prepare(true, 0, 0, 0);
        for (int x = 0; x < 7; x++) session.Model.SetOccupied(x, 0, true);
        Drop(0, new Vector2Int(7, 0), -1);
        Require(session.LastClear.LineCount == 1 && session.LastClear.UniqueClearedCells.Count == 8 &&
                Count() == 0 && session.Score == 110 && session.Combo == 1 && session.BlockSetNumber == 1 &&
                pieces[0].IsConsumed && !pieces[1].IsConsumed && !pieces[2].IsConsumed, "Test 1/5/6 row score");
        AssertBoardVisuals();
        results.Add("PASS tests 1/5/6: Row, 8 cleared, Single + line = 110, Combo 1, only used slot consumed.");

        Prepare(true, 0, 0, 0);
        for (int y = 0; y < 7; y++) session.Model.SetOccupied(0, y, true);
        Drop(0, new Vector2Int(0, 7), 42);
        Require(session.LastClear.ClearedColumns.Count == 1 && Count() == 0 && session.Score == 110, "Test 2 column");
        results.Add("PASS test 2: column clear through touch-style uGUI pointer events.");

        session.DebugPrepareCross();
        Drop(0, new Vector2Int(3, 3), -1);
        Require(session.LastClear.LineCount == 2 && session.LastClear.UniqueClearedCells.Count == 15 &&
                session.Score == 210 && session.Combo == 1 && Count() == 0, "Test 3 cross score");
        Require(session.BestScore == 210 && PlayerPrefs.GetInt(TestKey) == 210, "Test 13 record save");
        AssertBoardVisuals();
        results.Add("PASS tests 3/13: simultaneous row+column, unique 15 cells, score 210, Best 123 -> 210 saved to isolated test key.");

        session.Model.SetOccupied(0, 0, true);
        string before = Snapshot(); int score = session.Score, combo = session.Combo;
        Drop(1, new Vector2Int(0, 0), -1);
        Require(Snapshot() == before && session.Score == score && session.Combo == combo &&
                !pieces[1].IsConsumed && !session.TryPlacePiece(pieces[1], 8, 0), "Invalid placement is inert");
        Begin(1, -1);
        ExecuteEvents.Execute(pieces[1].gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.cancelHandler);
        Require(session.Score == score && session.Combo == combo && Snapshot() == before, "Cancel is inert");
        results.Add("PASS: invalid/outside/cancel never changes score, Combo, board or slot.");

        Prepare(true, 3, 0, 0);
        for (int y = 2; y <= 3; y++) for (int x = 1; x < 8; x++) session.Model.SetOccupied(x, y, true);
        Drop(0, new Vector2Int(0, 2), -1);
        Require(session.LastClear.ClearedRows.Count == 2 && session.LastClear.UniqueClearedCells.Count == 16 &&
                Count() == 0 && session.Score == 220, "Test 4 two rows");
        results.Add("PASS test 4: Vertical 2 clears two rows, score 20 + 200 = 220.");

        Prepare(true, 5, 0, 0);
        for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
            if (((x == 2 || x == 3) || (y == 2 || y == 3)) && !((x == 2 || x == 3) && (y == 2 || y == 3)))
                session.Model.SetOccupied(x, y, true);
        Drop(0, new Vector2Int(2, 2), 42);
        Require(session.LastClear.LineCount == 4 && session.LastClear.UniqueClearedCells.Count == 28 &&
                session.Score == 440 && Count() == 0, "Four-line core loop");
        results.Add("PASS: Square clears two rows + two columns atomically, unique 28 cells, score 440.");

        session.DebugPrepareCross(); Drop(0, new Vector2Int(3, 3), -1);
        session.DebugPrepareNextRow(); Drop(0, new Vector2Int(3, 3), -1);
        Require(session.Combo == 2 && session.Score == 370 && ComboLabel().text == "COMBO 2", "Test 7 Combo 2");
        results.Add("PASS test 7: consecutive clear Combo 2, +50 bonus, cumulative 370.");

        // Temporary board state for placeholder UI review, removed by the next preparation.
        Prepare(false, 2, 6, 3);
        session.Model.TryPlace(BlockCatalog.Shapes[2], 1, 1);
        session.Model.TryPlace(BlockCatalog.Shapes[5], 4, 4);
        session.Model.TryPlace(BlockCatalog.Shapes[6], 1, 5);
        board.ShowPreview(BlockCatalog.Shapes[7], new Vector2Int(5, 6), true);
        Capture(1080, 1920, "playing", false);
        Capture(1080, 2400, "long", true);
        Capture(1080, 1440, "short", false);
        board.ClearPreview();

        session.DebugPrepareNextRow(); Drop(0, new Vector2Int(3, 3), -1);
        Require(session.Combo == 3 && session.Score == 580 && ComboLabel().text == "STAR COMBO 3", "Combo 3");
        session.DebugPrepareNextRow(); Drop(0, new Vector2Int(3, 3), 42);
        Require(session.Combo == 4 && session.Score == 840 && ComboLabel().text == "COSMIC COMBO 4", "Combo 4");
        Drop(1, new Vector2Int(0, 0), -1);
        Require(session.Combo == 0 && session.Score == 850 && ComboLabel().text == "", "Test 8 reset");
        results.Add("PASS test 8: Combo 3/4 text and bonus; valid non-clear resets to 0; cumulative score 850.");

        Prepare(true, 0, 2, 5);
        Drop(0, new Vector2Int(0, 0), -1);
        Require(session.Score == 10 && session.BlockSetNumber == 1 && pieces[1].Shape == BlockCatalog.Shapes[2], "One used, no refill");
        Drop(1, new Vector2Int(2, 2), -1);
        Require(session.Score == 40 && session.BlockSetNumber == 1 && pieces[2].Shape == BlockCatalog.Shapes[5], "Two used, no refill");
        Drop(2, new Vector2Int(5, 5), 42);
        Require(session.Score == 80 && Count() == 8 && session.BlockSetNumber == 2 && AllReady(), "Test 9 refresh");
        results.Add("PASS tests 5/9: placements +10/+30/+40, no early refill, exactly three refreshed after third.");
        for (int round = 0; round < 12; round++)
        {
            session.Model.Clear();
            int number = session.BlockSetNumber;
            Drop(0, new Vector2Int(0, 0), -1);
            Drop(1, new Vector2Int(4, 0), 42);
            Drop(2, new Vector2Int(0, 4), -1);
            Require(session.BlockSetNumber == number + 1 && AllReady(), "Repeated refresh");
            foreach (var piece in pieces)
            {
                int active = 0;
                foreach (Transform child in piece.transform) if (child.gameObject.activeSelf) active++;
                Require(active == piece.Shape.Cells.Count, "No active visual accumulation");
            }
        }
        results.Add("PASS: twelve additional block-set cycles, correct single refresh and reusable visuals.");

        Prepare(true, 2, 2, 0); FillDiagonalHoles();
        session.EvaluateGameOver();
        Require(session.State == GameState.Playing && !session.Model.CanPlaceAnywhere(pieces[0].Shape) &&
                !session.Model.CanPlaceAnywhere(pieces[1].Shape) && session.Model.CanPlaceAnywhere(pieces[2].Shape),
                "Test 10 search all remaining");
        Drop(2, new Vector2Int(7, 7), -1);
        Require(session.LastClear.LineCount == 2 && session.LastClear.UniqueClearedCells.Count == 15 &&
                session.State == GameState.Playing && session.HasPlaceableRemainingBlock() && pieces[2].IsConsumed,
                "Clear before Game Over");
        results.Add("PASS test 10: only last remaining Single fits; its clear creates space for H3, game continues.");

        Prepare(false, 2, 2, 2); session.Model.Clear(); FillDiagonalHoles();
        Begin(0, -1); session.EvaluateGameOver();
        Require(session.State == GameState.GameOver && !session.HasPlaceableRemainingBlock() &&
                !pieces[0].GetComponent<BlockDragHandler>().IsDragging && Panel().activeSelf, "Test 11 Game Over");
        before = Snapshot(); score = session.Score;
        Begin(0, 42);
        Require(!pieces[0].GetComponent<BlockDragHandler>().IsDragging &&
                !session.TryPlacePiece(pieces[0], 0, 0) && Snapshot() == before && session.Score == score, "Frozen game input");
        var ad = Card().Find("AdPlaceholderButton").GetComponent<Button>();
        Require(!ad.interactable && ad.onClick.GetPersistentEventCount() == 0, "Disabled ad placeholder");
        ad.onClick.Invoke();
        Require(Snapshot() == before && session.State == GameState.GameOver, "No ad action");
        Debug.Log("SPRINT2_GAMEOVER_ACTIVATION_DEPTH=" + Card().Find("RetryButton").GetComponent<Image>().depth);
        Capture(1080, 1920, "gameover", false);
        Capture(1080, 1440, "gameover_short", false);
        results.Add("PASS test 11: no remaining block fits, Game Over modal, drag/placement locked, ad placeholder inactive.");

    }
    private static void TestRetry()
    {
        RaycastHit(Card().Find("RetryButton").gameObject);
        int best = session.BestScore;
        var model = session.Model;
        Card().Find("RetryButton").GetComponent<Button>().onClick.Invoke();
        Require(session.State == GameState.Playing && session.Score == 0 && session.Combo == 0 && Count() == 0 &&
                session.BestScore == best && PlayerPrefs.GetInt(TestKey) == best && session.BlockSetNumber == 1 &&
                AllReady() && !Panel().activeSelf && ReferenceEquals(session.Model, model), "Test 12 Retry");
        results.Add("PASS test 12: Retry button resets board/score/Combo/set/state/UI, keeps Best, no scene/model reload.");

        Begin(0, 42); Begin(1, 43);
        Require(pieces[0].GetComponent<BlockDragHandler>().IsDragging && !pieces[1].GetComponent<BlockDragHandler>().IsDragging,
                "Multi-pointer guard");
        pieces[0].GetComponent<BlockDragHandler>().CancelDrag();
        Require(Count() == 0 && session.Score == 0 && session.Combo == 0, "Cancel after Retry");
        AssertBoardVisuals();
    }
    private static void Prepare(bool reset, params int[] indices)
    {
        if (reset) session.Retry();
        for (int i = 0; i < pieces.Length; i++) pieces[i].Initialize(BlockCatalog.Shapes[indices[i]], board, layer, session);
    }
    private static void FillDiagonalHoles()
    { for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++) if (x != y) session.Model.SetOccupied(x, y, true); }
    private static bool AllReady()
    {
        foreach (var piece in pieces) if (piece.Shape == null || piece.IsConsumed || !piece.gameObject.activeSelf) return false;
        return true;
    }
    private static int Count() => Sprint2Builder.Count(session.Model);
    private static string Snapshot() => Sprint1Builder.Snapshot(session.Model);
    private static Transform Safe() => GameObject.Find("GameCanvas").transform.Find("SafeArea");
    private static GameObject Panel() => Safe().Find("GameOverPanel").gameObject;
    private static Transform Card() => Panel().transform.Find("Card");
    private static Text ComboLabel() => Safe().Find("ComboText").GetComponent<Text>();
    private static Camera UICamera()
    {
        var canvas = layer.GetComponentInParent<Canvas>();
        return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }
    private static Vector2 Screen(Vector3 world) => RectTransformUtility.WorldToScreenPoint(UICamera(), world);
    private static void RaycastHit(GameObject target)
    {
        Canvas.ForceUpdateCanvases();
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = Screen(target.transform.position) }, hits);
        Require(hits.Exists(hit => hit.gameObject == target), "UI raycaster: " + target.name + " at " + Screen(target.transform.position) + "; depth " + target.GetComponent<Image>().depth + "; hits " + string.Join(",", hits.ConvertAll(hit => hit.gameObject.name)));
    }
    private static void Begin(int index, int pointer)
    {
        var data = new PointerEventData(EventSystem.current) { pointerId = pointer, position = Screen(pieces[index].transform.position) };
        ExecuteEvents.Execute(pieces[index].gameObject, data, ExecuteEvents.beginDragHandler);
    }
    private static void Drop(int index, Vector2Int coordinate, int pointer)
    {
        string before = Snapshot(); int score = session.Score, combo = session.Combo;
        Begin(index, pointer);
        var piece = pieces[index];
        Canvas.ForceUpdateCanvases();
        float pitch = board.CellSize + board.CellSpacing;
        Vector3 center = board.GetCellWorld(coordinate) + layer.TransformVector(
            new Vector3((piece.Shape.Width - 1) * pitch / 2, -(piece.Shape.Height - 1) * pitch / 2 - session.DragFingerOffset));
        var data = new PointerEventData(EventSystem.current) { pointerId = pointer, position = Screen(center) };
        ExecuteEvents.Execute(piece.gameObject, data, ExecuteEvents.dragHandler);
        Require(Snapshot() == before && session.Score == score && session.Combo == combo, "Preview is non-mutating");
        Require(Vector3.Distance(piece.OriginWorld, board.GetCellWorld(coordinate)) < .1f, "Preview/visual same anchor");
        ExecuteEvents.Execute(piece.gameObject, data, ExecuteEvents.endDragHandler);
    }
    private static void AssertBoardVisuals()
    {
        for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
        {
            Color expected = session.Model.IsOccupied(x, y) ? BoardView.OccupiedFill : BoardView.EmptyFill;
            Require(board.transform.GetChild(y * 8 + x).GetComponent<Image>().color == expected, "Board model/view match");
        }
    }
    private static void Capture(int width, int height, string name, bool notch)
    {
        var canvas = layer.GetComponentInParent<Canvas>(); var camera = Camera.main;
        var safe = (RectTransform)Safe();
        var mode = canvas.renderMode; var oldCamera = canvas.worldCamera; var target = camera.targetTexture;
        bool ortho = camera.orthographic; float size = camera.orthographicSize; var active = RenderTexture.active;
        var rt = new RenderTexture(width, height, 24); Texture2D image = null;
        try
        {
            rt.Create(); camera.targetTexture = rt; camera.orthographic = true; camera.orthographicSize = height / 2f;
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 10;
            safe.anchorMin = notch ? new Vector2(.02f, .04f) : Vector2.zero;
            safe.anchorMax = notch ? new Vector2(.98f, .94f) : Vector2.one;
            safe.offsetMin = safe.offsetMax = Vector2.zero;
            Canvas.ForceUpdateCanvases();
            board.GetComponent<SquareBoardLayout>().SendMessage("LateUpdate");
            board.SendMessage("LateUpdate"); Canvas.ForceUpdateCanvases();
            foreach (var piece in pieces) piece.FitSlot();
            Canvas.ForceUpdateCanvases();
            var rect = ((RectTransform)board.transform).rect;
            Require(rect.width > 0 && Mathf.Abs(rect.width - rect.height) < .1f, "Square board: " + name);
            for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
            {
                var coordinate = new Vector2Int(x, y);
                Require(board.TryScreenToCell(Screen(board.GetCellWorld(coordinate)), camera, out var mapped) &&
                        mapped == coordinate, "Cell mapping: " + name);
            }
            foreach (var piece in pieces) AssertInside((RectTransform)piece.transform.parent, safe);
            if (Panel().activeSelf) AssertInside((RectTransform)Card(), safe);
            var background = UnityEngine.Object.FindAnyObjectByType<AspectFillBackground>();
            if (background != null) background.SendMessage("LateUpdate");
            Canvas.ForceUpdateCanvases();
            camera.Render(); RenderTexture.active = rt;
            image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply();
            File.WriteAllBytes("Validation/sprint2_" + name + ".png", image.EncodeToPNG());
            results.Add("PASS UI render: " + name + " " + width + "x" + height +
                (notch ? " simulated SafeArea insets" : "") + ", 64 mappings/board square/slot and modal bounds.");
        }
        finally
        {
            canvas.renderMode = mode; canvas.worldCamera = oldCamera;
            camera.targetTexture = target; camera.orthographic = ortho; camera.orthographicSize = size; RenderTexture.active = active;
            if (image != null) UnityEngine.Object.DestroyImmediate(image);
            rt.Release(); UnityEngine.Object.DestroyImmediate(rt);
            safe.GetComponent<SafeArea>().SendMessage("Apply"); Canvas.ForceUpdateCanvases();
            board.GetComponent<SquareBoardLayout>().SendMessage("LateUpdate");
            board.SendMessage("LateUpdate"); Canvas.ForceUpdateCanvases();
            foreach (var piece in pieces) piece.FitSlot();
        }
    }
    private static void AssertInside(RectTransform rect, RectTransform safe)
    {
        var corners = new Vector3[4]; rect.GetWorldCorners(corners);
        foreach (var corner in corners)
        {
            Vector3 local = safe.InverseTransformPoint(corner);
            Require(local.x >= safe.rect.xMin - .1f && local.x <= safe.rect.xMax + .1f &&
                    local.y >= safe.rect.yMin - .1f && local.y <= safe.rect.yMax + .1f, "SafeArea bounds: " + rect.name);
        }
    }
    private static void Require(bool condition, string message) => Sprint2Builder.Require(condition, message);
    private static void Finish(bool passed, string failure)
    {
        EditorApplication.update -= Check; Application.logMessageReceived -= Log;
        if (hadTestKey) PlayerPrefs.SetInt(TestKey, previousTestBest); else PlayerPrefs.DeleteKey(TestKey);
        PlayerPrefs.Save();
        File.AppendAllText("Validation/sprint2.txt", string.Join("\n", results) + "\n" + (passed ? "SPRINT2_PLAY_PASS" : failure));
        if (passed) Debug.Log("SPRINT2_PLAY_PASS"); else Debug.LogError(failure);
        EditorApplication.Exit(passed ? 0 : 1);
    }
}
