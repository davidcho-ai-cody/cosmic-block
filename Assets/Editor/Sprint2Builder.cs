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
using UnityEngine.UI;

public static class Sprint2Builder
{
    public const string ScenePath = "Assets/Scenes/Game.unity";
    [MenuItem("COSMIC BLOCK/Sprint 2/Upgrade Core Loop UI")]
    public static void Build()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene = EditorSceneManager.OpenScene(ScenePath);
        var canvas = GameObject.Find("GameCanvas");
        var safe = canvas.transform.Find("SafeArea");
        var session = UnityEngine.Object.FindAnyObjectByType<GameSession>();
        var board = UnityEngine.Object.FindAnyObjectByType<BoardView>();
        if (safe == null || session == null || board == null || board.transform.childCount != 64)
            throw new Exception("Sprint 1 foundation missing.");
        var score = safe.Find("ScorePlaceholder").GetComponent<Text>();
        var scoreRect = (RectTransform)score.transform;
        scoreRect.anchorMin = scoreRect.anchorMax = new Vector2(.5f, .86f);
        score.resizeTextForBestFit = true; score.resizeTextMinSize = 18; score.resizeTextMaxSize = 36;
        score.text = "BEST  0     SCORE  0";
        var comboRect = safe.Find("ComboText") as RectTransform;
        if (comboRect == null) comboRect = UI("ComboText", safe);
        SetAnchors(comboRect, new Vector2(.05f, .795f), new Vector2(.95f, .835f));
        var combo = TextOn(comboRect, "", 32);

        var panel = safe.Find("GameOverPanel") as RectTransform;
        if (panel == null)
        {
            panel = UI("GameOverPanel", safe);
            SetAnchors(panel, Vector2.zero, Vector2.one);
            var overlay = panel.gameObject.AddComponent<Image>();
            overlay.color = new Color(.025f, .03f, .09f, .88f);
            var card = UI("Card", panel);
            SetAnchors(card, new Vector2(.07f, .12f), new Vector2(.93f, .88f));
            card.gameObject.AddComponent<Image>().color = new Color(.105f, .125f, .245f);
            var title = UI("Title", card);
            SetAnchors(title, new Vector2(.06f, .79f), new Vector2(.94f, .91f));
            TextOn(title, "GAME OVER", 56);
            var final = UI("FinalScore", card);
            SetAnchors(final, new Vector2(.08f, .39f), new Vector2(.92f, .73f));
            TextOn(final, "SCORE\n0\n\nBEST\n0", 46);
            ButtonOn("RetryButton", card, new Vector2(.1f, .2f), new Vector2(.9f, .31f), "RETRY",
                new Color(.35f, .4f, .72f));
            var ad = ButtonOn("AdPlaceholderButton", card, new Vector2(.1f, .055f), new Vector2(.9f, .16f),
                "WATCH AD TO CONTINUE\nCOMING LATER", new Color(.2f, .23f, .36f));
            ad.interactable = false;
        }
        panel.SetAsLastSibling();
        var hud = canvas.GetComponent<GameHud>();
        if (hud == null) hud = canvas.AddComponent<GameHud>();
        var cardTransform = panel.Find("Card");
        hud.Configure(score, combo, panel.gameObject, cardTransform.Find("FinalScore").GetComponent<Text>(),
            cardTransform.Find("RetryButton").GetComponent<Button>(),
            cardTransform.Find("AdPlaceholderButton").GetComponent<Button>());
        session.ConfigureHud(hud);
        panel.gameObject.SetActive(false);
        EditorUtility.SetDirty(hud); EditorUtility.SetDirty(session);
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Validate();
        Debug.Log("SPRINT2_SCENE_READY");
    }
    private static RectTransform UI(string name, Transform parent)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false); return rect;
    }
    private static void SetAnchors(RectTransform rect, Vector2 min, Vector2 max)
    { rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero; }
    private static Text TextOn(RectTransform rect, string value, int size)
    {
        var text = rect.GetComponent<Text>();
        if (text == null) text = rect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value; text.fontSize = size; text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(.9f, .82f, .62f); text.raycastTarget = false;
        text.resizeTextForBestFit = true; text.resizeTextMinSize = 18; text.resizeTextMaxSize = size;
        return text;
    }
    private static Button ButtonOn(string name, Transform parent, Vector2 min, Vector2 max, string value, Color color)
    {
        var rect = UI(name, parent); SetAnchors(rect, min, max);
        var image = rect.gameObject.AddComponent<Image>(); image.color = color;
        var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = image;
        var navigation = button.navigation; navigation.mode = Navigation.Mode.None; button.navigation = navigation;
        var label = UI("Label", rect); SetAnchors(label, new Vector2(.04f, .07f), new Vector2(.96f, .93f));
        TextOn(label, value, name == "RetryButton" ? 42 : 28);
        return button;
    }
    [MenuItem("COSMIC BLOCK/Sprint 2/Validate Line And Score Rules")]
    public static void Validate()
    {
        var single = BlockCatalog.Shapes[0];
        var board = new BoardModel();
        for (int x = 0; x < 7; x++) board.SetOccupied(x, 0, true);
        Require(board.TryPlace(single, 7, 0), "Row setup");
        var result = board.ClearCompletedLines();
        Require(result.ClearedRows.Count == 1 && result.ClearedColumns.Count == 0 &&
                result.UniqueClearedCells.Count == 8 && Count(board) == 0, "Test 1 row clear");

        for (int y = 0; y < 7; y++) board.SetOccupied(0, y, true);
        Require(board.TryPlace(single, 0, 7), "Column setup");
        result = board.ClearCompletedLines();
        Require(result.ClearedColumns.Count == 1 && result.ClearedRows.Count == 0 &&
                result.UniqueClearedCells.Count == 8 && Count(board) == 0, "Test 2 column clear");

        for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
            if ((x == 3 || y == 3) && !(x == 3 && y == 3)) board.SetOccupied(x, y, true);
        Require(board.TryPlace(single, 3, 3), "Cross setup");
        int clears = 0;
        board.CellChanged += (_, _, occupied) => { if (!occupied) clears++; };
        result = board.ClearCompletedLines();
        Require(result.LineCount == 2 && result.ClearedRows[0] == 3 && result.ClearedColumns[0] == 3 &&
                result.UniqueClearedCells.Count == 15 && clears == 15 && Count(board) == 0, "Test 3 unique intersection");

        for (int y = 2; y <= 3; y++) for (int x = 1; x < 8; x++) board.SetOccupied(x, y, true);
        Require(board.TryPlace(BlockCatalog.Shapes[3], 0, 2), "Two-row setup");
        result = board.ClearCompletedLines();
        Require(result.ClearedRows.Count == 2 && result.UniqueClearedCells.Count == 16 && Count(board) == 0,
            "Test 4 multiple rows");

        for (int x = 2; x <= 3; x++) for (int y = 1; y < 8; y++) board.SetOccupied(x, y, true);
        Require(board.TryPlace(BlockCatalog.Shapes[1], 2, 0), "Two-column setup");
        result = board.ClearCompletedLines();
        Require(result.ClearedColumns.Count == 2 && result.UniqueClearedCells.Count == 16 && Count(board) == 0,
            "Multiple columns");

        for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
            if (((y == 2 || y == 3) || (x == 2 || x == 3)) && !((y == 2 || y == 3) && (x == 2 || x == 3)))
                board.SetOccupied(x, y, true);
        Require(board.TryPlace(BlockCatalog.Shapes[5], 2, 2), "Four-line setup");
        result = board.ClearCompletedLines();
        Require(result.LineCount == 4 && result.ClearedRows.Count == 2 && result.ClearedColumns.Count == 2 &&
                result.UniqueClearedCells.Count == 28 && new HashSet<Vector2Int>(result.UniqueClearedCells).Count == 28 &&
                Count(board) == 0, "Multiple rows and columns simultaneous");

        board.SetOccupied(1, 1, true);
        result = board.ClearCompletedLines();
        Require(result.LineCount == 0 && Count(board) == 1, "No lines leaves board unchanged");
        board.Clear();
        for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++) board.SetOccupied(x, y, true);
        result = board.ClearCompletedLines();
        Require(result.LineCount == 16 && result.UniqueClearedCells.Count == 64 && Count(board) == 0, "All lines");

        Require(ScoreRules.AddPlacement(0, 1, 0, 0) == 10 && ScoreRules.AddPlacement(0, 3, 0, 0) == 30 &&
                ScoreRules.AddPlacement(0, 4, 0, 0) == 40 && ScoreRules.AddPlacement(0, 1, 2, 1) == 210 &&
                ScoreRules.AddPlacement(0, 3, 2, 4) == 380, "Test 5 score formula");
        Require(ScoreRules.AddPlacement(int.MaxValue - 5, 1, 0, 0) == int.MaxValue, "Score integer cap");
        foreach (var shape in BlockCatalog.Shapes) Require(board.CanPlaceAnywhere(shape), "Empty board placeability");
        for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++) if (x != y) board.SetOccupied(x, y, true);
        Require(board.CanPlaceAnywhere(single) && !board.CanPlaceAnywhere(BlockCatalog.Shapes[2]), "Isolated hole search");
        Directory.CreateDirectory("Validation");
        File.WriteAllText("Validation/sprint2.txt",
            "PASS domain tests 1-5: row/column, unique cross (15 cells), multi-row/multi-column/four-line (28 cells), all lines (64), score rules, placeability.\n");
        Debug.Log("SPRINT2_DOMAIN_PASS");
    }
    public static int Count(BoardModel board) => Sprint1Builder.Count(board);
    public static void Require(bool condition, string message)
    { if (!condition) throw new Exception("Sprint 2 test failed: " + message); }
}
