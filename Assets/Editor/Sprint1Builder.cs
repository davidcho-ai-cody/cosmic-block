using System;
using System.IO;
using CosmicBlock.Blocks;
using CosmicBlock.Board;
using CosmicBlock.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Sprint1Builder
{
    public const string ScenePath = "Assets/Scenes/Game.unity";
    [MenuItem("COSMIC BLOCK/Sprint 1/Upgrade Game Scene")]
    public static void Build()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene = EditorSceneManager.OpenScene(ScenePath);
        var canvas = GameObject.Find("GameCanvas");
        var safe = canvas.transform.Find("SafeArea");
        var board = UnityEngine.Object.FindAnyObjectByType<BoardView>();
        var session = UnityEngine.Object.FindAnyObjectByType<GameSession>();
        if (safe == null || board == null || session == null) throw new Exception("Sprint 0 foundation missing.");
        board.GetComponent<GridLayoutGroup>().spacing = new Vector2(12, 12);
        var placeholder = safe.Find("TrayPlaceholder");
        if (placeholder != null) placeholder.gameObject.SetActive(false);
        var area = safe.Find("BlockArea") as RectTransform;
        if (area == null)
        {
            area = UI("BlockArea", safe);
            area.anchorMin = new Vector2(.04f, .025f);
            area.anchorMax = new Vector2(.96f, .19f);
            area.offsetMin = area.offsetMax = Vector2.zero;
            var layout = area.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 24; layout.padding = new RectOffset(12, 12, 12, 12);
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = layout.childForceExpandHeight = true;
        }
        var pieces = new BlockPiece[3];
        for (int i = 0; i < 3; i++)
        {
            var slot = area.Find("Slot_" + i) as RectTransform;
            if (slot == null)
            {
                slot = UI("Slot_" + i, area);
                var bg = slot.gameObject.AddComponent<Image>();
                bg.color = new Color(.16f, .19f, .33f, .45f); bg.raycastTarget = false;
                slot.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
                var piece = UI("BlockPiece", slot);
                piece.anchorMin = piece.anchorMax = new Vector2(.5f, .5f);
                piece.sizeDelta = new Vector2(180, 180);
                piece.gameObject.AddComponent<BlockPiece>();
            }
            pieces[i] = slot.GetComponentInChildren<BlockPiece>(true);
            if (pieces[i] == null) throw new Exception("Slot piece missing.");
        }
        var layer = canvas.transform.Find("DragLayer") as RectTransform;
        if (layer == null)
        {
            layer = UI("DragLayer", canvas.transform);
            layer.anchorMin = Vector2.zero; layer.anchorMax = Vector2.one;
            layer.offsetMin = layer.offsetMax = Vector2.zero;
        }
        session.ConfigureBlocks(pieces, layer);
        EditorUtility.SetDirty(session);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Validate();
        Debug.Log("SPRINT1_SCENE_READY");
    }
    private static RectTransform UI(string name, Transform parent)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false); return rect;
    }
    [MenuItem("COSMIC BLOCK/Sprint 1/Validate Placement Logic")]
    public static void Validate()
    {
        var shapes = BlockCatalog.Shapes;
        Require(shapes.Count == 8, "Shape pool");
        var model = new BoardModel();
        Require(model.TryPlace(shapes[0], 0, 0) && Count(model) == 1, "Single");
        model.Clear();
        Require(model.TryPlace(shapes[2], 2, 3) && Count(model) == 3 &&
                model.IsOccupied(2, 3) && model.IsOccupied(3, 3) && model.IsOccupied(4, 3), "Horizontal 3");
        model.Clear();
        Require(model.TryPlace(shapes[6], 2, 2) && Count(model) == 3 && !model.IsOccupied(3, 2) &&
                model.IsOccupied(2, 2) && model.IsOccupied(2, 3) && model.IsOccupied(3, 3), "L offsets");
        model.Clear();
        Require(model.TryPlace(shapes[7], 2, 2) && !model.IsOccupied(2, 2) &&
                model.IsOccupied(3, 2) && model.IsOccupied(2, 3) && model.IsOccupied(3, 3), "Reverse L offsets");
        model.Clear();
        string before = Snapshot(model);
        Require(!model.TryPlace(shapes[2], 7, 4) && Snapshot(model) == before, "Outside atomic");
        Require(!model.TryPlace(shapes[6], -1, 0) && Snapshot(model) == before, "Negative anchor");
        Require(!model.TryPlace(shapes[2], int.MaxValue, 0) && Snapshot(model) == before, "Overflow safety");
        model.SetOccupied(3, 3, true); before = Snapshot(model);
        Require(!model.TryPlace(shapes[2], 2, 3) && Snapshot(model) == before, "Collision atomic");
        model.Clear();
        Require(model.TryPlace(shapes[2], 5, 7) && Count(model) == 3, "Valid edge");
        foreach (var shape in shapes)
        {
            model.Clear();
            Require(model.TryPlace(shape, 8 - shape.Width, 8 - shape.Height) &&
                    Count(model) == shape.Cells.Count, "All shapes at bottom-right");
        }
        model.Clear();
        for (int x = 0; x < 8; x++) Require(model.TryPlace(shapes[0], x, 0), "Full row placement");
        Require(Count(model) == 8, "No line clearing in Sprint 1");
        var a = new BlockGenerator(123); var b = new BlockGenerator(123);
        var seen = new System.Collections.Generic.HashSet<string>();
        for (int i = 0; i < 256; i++)
        {
            var shape = a.Next(); seen.Add(shape.Id);
            Require(shape.Id == b.Next().Id, "Deterministic seed");
        }
        Require(seen.Count == 8, "Entire random pool reachable");
        Directory.CreateDirectory("Validation");
        File.WriteAllText("Validation/sprint1.txt",
            "PASS domain: Single, H3, L, Reverse L, outside/negative/overflow, collision atomicity, all-shape edges, no line clear, seeded random pool.\n");
        Debug.Log("SPRINT1_DOMAIN_PASS");
    }
    public static int Count(BoardModel model)
    {
        int result = 0;
        for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++) if (model.IsOccupied(x, y)) result++;
        return result;
    }
    public static string Snapshot(BoardModel model)
    {
        string result = "";
        for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++) result += model.IsOccupied(x, y) ? "1" : "0";
        return result;
    }
    public static void Require(bool condition, string message)
    { if (!condition) throw new Exception("Sprint 1 test failed: " + message); }
}
