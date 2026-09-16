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

public static class Sprint1PlayProbe
{
    private static double started;
    private static int frames, stage;
    private static string runtimeErrors;
    private static BoardView board;
    private static GameSession session;
    private static RectTransform layer;
    private static BlockPiece[] initial;
    private static readonly List<BlockPiece> fixtures = new List<BlockPiece>();
    private static BlockPiece deferredPiece;
    private static Transform deferredSlot;
    private static readonly List<string> results = new List<string>();

    // Batch-only: exits the Editor when finished, never changes the saved scene.
    public static void Run()
    {
        Sprint1Builder.Validate();
        EditorSceneManager.OpenScene(Sprint1Builder.ScenePath);
        started = EditorApplication.timeSinceStartup;
        frames = stage = 0; runtimeErrors = ""; results.Clear();
        Application.logMessageReceived += Log;
        EditorApplication.update += Check;
        EditorApplication.EnterPlaymode();
    }
    private static void Log(string message, string stack, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            runtimeErrors += message + "\n";
    }
    private static void Check()
    {
        if (EditorApplication.timeSinceStartup - started > 120) { Finish(false, "Play Mode timeout"); return; }
        if (!EditorApplication.isPlaying || ++frames < 30) return;
        try
        {
            if (stage == 0)
            {
                board = UnityEngine.Object.FindAnyObjectByType<BoardView>();
                session = UnityEngine.Object.FindAnyObjectByType<GameSession>();
                layer = GameObject.Find("GameCanvas").transform.Find("DragLayer") as RectTransform;
                initial = UnityEngine.Object.FindObjectsByType<BlockPiece>(FindObjectsInactive.Exclude);
                Array.Sort(initial, (a, b) => string.CompareOrdinal(a.transform.parent.name, b.transform.parent.name));
                Require(initial.Length == 3 && board.Model != null && layer != null, "Initial 3 pieces and binding");
                foreach (var piece in initial) Require(piece.Shape != null && !piece.IsConsumed, "Generated shape");
                Canvas.ForceUpdateCanvases();
                var ray = new PointerEventData(EventSystem.current) { position = Screen(initial[0].transform.position) };
                var hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(ray, hits);
                Require(hits.Exists(hit => hit.gameObject == initial[0].gameObject), "Piece is reachable by UI raycaster");
                results.Add("PASS: initial random 3 slots and uGUI raycast target.");

                // Temporary gameplay state for visual review; saved scene remains untouched.
                board.Model.TryPlace(BlockCatalog.Shapes[2], 1, 1);
                board.Model.TryPlace(BlockCatalog.Shapes[5], 4, 3);
                board.Model.TryPlace(BlockCatalog.Shapes[6], 1, 5);
                board.ShowPreview(BlockCatalog.Shapes[7], new Vector2Int(5, 5), true);
                Capture(1080, 1920, "9x16", 12, false);
                Capture(1080, 1920, "spacing8", 8, false);
                Capture(1080, 2400, "long", 12, true);
                Capture(1080, 1440, "short", 12, false);
                board.GetComponent<GridLayoutGroup>().spacing = new Vector2(12, 12);
                board.ClearPreview(); board.Model.Clear();
                TestDragScenarios();
                stage = 1; frames = 0;
                return;
            }
            Require(deferredPiece != null && deferredPiece.transform.parent == deferredSlot && !deferredPiece.IsConsumed, "Deferred disable return");
            Destroy(deferredPiece);
            Require(Array.TrueForAll(initial, p => p.IsConsumed && !p.gameObject.activeSelf), "No block-set regeneration");
            Require(runtimeErrors.Length == 0, "Runtime errors: " + runtimeErrors);
            results.Add("PASS: all 3 original slots remain empty after additional frames; no runtime errors.");
            Finish(true, "");
        }
        catch (Exception ex) { Finish(false, ex.ToString()); }
    }
    private static void TestDragScenarios()
    {
        var first = initial[0];
        var handler = first.GetComponent<BlockDragHandler>();
        Drop(handler, new Vector2Int(0, 0), -1);
        Require(first.IsConsumed && !first.gameObject.activeSelf &&
                initial[1].gameObject.activeSelf && initial[2].gameObject.activeSelf &&
                Sprint1Builder.Count(board.Model) == first.Shape.Cells.Count, "Only used slot removed");
        results.Add("PASS test 8: mouse drop occupies exact offsets; only used slot emptied.");

        board.Model.Clear();
        var parent = initial[0].transform.parent;
        var single = Fixture(parent, BlockCatalog.Shapes[0]);
        var h3 = Fixture(parent, BlockCatalog.Shapes[2]);
        var l = Fixture(parent, BlockCatalog.Shapes[6]);
        Drop(single.GetComponent<BlockDragHandler>(), new Vector2Int(7, 0), -1);
        Require(board.Model.IsOccupied(7, 0) && Sprint1Builder.Count(board.Model) == 1, "Single placement");
        Drop(h3.GetComponent<BlockDragHandler>(), new Vector2Int(2, 3), -1);
        Require(board.Model.IsOccupied(2, 3) && board.Model.IsOccupied(3, 3) &&
                board.Model.IsOccupied(4, 3) && Sprint1Builder.Count(board.Model) == 4, "H3 placement");
        Drop(l.GetComponent<BlockDragHandler>(), new Vector2Int(0, 5), 42);
        Require(board.Model.IsOccupied(0, 5) && board.Model.IsOccupied(0, 6) &&
                board.Model.IsOccupied(1, 6) && !board.Model.IsOccupied(1, 5), "Touch-id L placement");
        results.Add("PASS tests 1/2/3: Single, H3, L exact cells through synthetic uGUI mouse/touch-id events.");
        Destroy(single); Destroy(h3); Destroy(l);

        var invalid = Fixture(parent, BlockCatalog.Shapes[2]);
        var drag = invalid.GetComponent<BlockDragHandler>();
        string before = Sprint1Builder.Snapshot(board.Model);
        Begin(drag, -1);
        var partial = EventAt(invalid, new Vector2Int(7, 4), -1);
        ExecuteEvents.Execute(drag.gameObject, partial, ExecuteEvents.dragHandler);
        Require(Sprint1Builder.Snapshot(board.Model) == before, "Preview never mutates model");
        var color = board.transform.GetChild(4 * 8 + 7).GetComponent<Image>().color;
        Require(color.r > color.b, "Invalid red preview");
        ExecuteEvents.Execute(drag.gameObject, partial, ExecuteEvents.endDragHandler);
        Require(!invalid.IsConsumed && invalid.transform.parent == parent &&
                invalid.Rect.anchoredPosition == Vector2.zero && Sprint1Builder.Snapshot(board.Model) == before,
                "Partially outside returns atomically");
        results.Add("PASS test 4: partial out-of-board drop returns to slot; red preview; model unchanged.");

        var collision = EventAt(invalid, new Vector2Int(2, 3), 42);
        Begin(drag, 42);
        ExecuteEvents.Execute(drag.gameObject, collision, ExecuteEvents.dragHandler);
        ExecuteEvents.Execute(drag.gameObject, collision, ExecuteEvents.endDragHandler);
        Require(!invalid.IsConsumed && invalid.transform.parent == parent &&
                Sprint1Builder.Snapshot(board.Model) == before, "Occupied collision return");
        results.Add("PASS test 5: occupied collision leaves model/slot unchanged.");

        Begin(drag, -1);
        var outside = new PointerEventData(EventSystem.current) { pointerId = -1, position = new Vector2(-500, -500) };
        ExecuteEvents.Execute(drag.gameObject, outside, ExecuteEvents.endDragHandler);
        Require(!invalid.IsConsumed && invalid.transform.parent == parent &&
                Sprint1Builder.Snapshot(board.Model) == before, "Far outside drop");
        Begin(drag, -1);
        ExecuteEvents.Execute(drag.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.cancelHandler);
        Require(!drag.IsDragging && invalid.transform.parent == parent &&
                Sprint1Builder.Snapshot(board.Model) == before, "Cancel");

        var second = Fixture(parent, BlockCatalog.Shapes[0]);
        var secondDrag = second.GetComponent<BlockDragHandler>();
        Begin(drag, 42); Vector3 position = invalid.transform.position;
        Begin(secondDrag, 43);
        Require(drag.IsDragging && !secondDrag.IsDragging, "Single active drag lock");
        var wrongPointer = EventAt(invalid, new Vector2Int(0, 0), 43);
        ExecuteEvents.Execute(drag.gameObject, wrongPointer, ExecuteEvents.dragHandler);
        ExecuteEvents.Execute(drag.gameObject, wrongPointer, ExecuteEvents.endDragHandler);
        Require(drag.IsDragging && invalid.transform.position == position, "Other pointer cannot move/drop");
        drag.gameObject.SendMessage("OnApplicationFocus", false);
        Require(!drag.IsDragging && invalid.transform.parent == parent, "Focus loss cancellation");
        Begin(secondDrag, 43); secondDrag.CancelDrag();
        Require(!secondDrag.IsDragging, "Lock released after cancellation");
        Begin(secondDrag, -1); secondDrag.gameObject.SetActive(false); secondDrag.gameObject.SetActive(true);
        Require(!secondDrag.IsDragging && Sprint1Builder.Snapshot(board.Model) == before, "Disable cancellation");
        deferredPiece = second; deferredSlot = parent;
        results.Add("PASS test 7: outside, cancel, focus loss, disable and simultaneous pointer protection.");
        // Keep this fixture until the next frame to verify deferred slot restoration.

        Drop(drag, new Vector2Int(5, 7), 42);
        Require(invalid.IsConsumed && board.Model.IsOccupied(5, 7) &&
                board.Model.IsOccupied(6, 7) && board.Model.IsOccupied(7, 7), "Valid edge");
        results.Add("PASS test 6: H3 at bottom-right edge succeeds.");
        Destroy(invalid);

        board.Model.Clear();
        Drop(initial[1].GetComponent<BlockDragHandler>(), new Vector2Int(3, 0), -1);
        Drop(initial[2].GetComponent<BlockDragHandler>(), new Vector2Int(0, 4), 42);
        Require(initial[1].IsConsumed && initial[2].IsConsumed, "Remaining originals consumed");
    }
    private static BlockPiece Fixture(Transform parent, BlockShape shape)
    {
        var obj = new GameObject("Test_" + shape.Id, typeof(RectTransform));
        var rect = obj.GetComponent<RectTransform>(); rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        var piece = obj.AddComponent<BlockPiece>();
        piece.Initialize(shape, board, layer, session);
        fixtures.Add(piece);
        RegisterFixtures();
        return piece;
    }
    private static void Destroy(BlockPiece piece)
    {
        fixtures.Remove(piece);
        UnityEngine.Object.DestroyImmediate(piece.gameObject);
        RegisterFixtures();
    }
    private static void RegisterFixtures()
    {
        var registered = new List<BlockPiece>(initial);
        registered.AddRange(fixtures);
        session.ConfigureBlocks(registered.ToArray(), layer);
    }
    private static Camera UICamera()
    {
        var canvas = layer.GetComponentInParent<Canvas>();
        return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }
    private static Vector2 Screen(Vector3 world) => RectTransformUtility.WorldToScreenPoint(UICamera(), world);
    private static PointerEventData EventAt(BlockPiece piece, Vector2Int coordinate, int pointer)
    {
        Canvas.ForceUpdateCanvases();
        float pitch = board.CellSize + board.CellSpacing;
        Vector3 center = board.GetCellWorld(coordinate) + layer.TransformVector(
            new Vector3((piece.Shape.Width - 1) * pitch / 2, -(piece.Shape.Height - 1) * pitch / 2 - session.DragFingerOffset));
        return new PointerEventData(EventSystem.current) { pointerId = pointer, position = Screen(center) };
    }
    private static void Begin(BlockDragHandler handler, int pointer)
    {
        var data = new PointerEventData(EventSystem.current) { pointerId = pointer, position = Screen(handler.transform.position) };
        ExecuteEvents.Execute(handler.gameObject, data, ExecuteEvents.beginDragHandler);
    }
    private static void Drop(BlockDragHandler handler, Vector2Int coordinate, int pointer)
    {
        var piece = handler.GetComponent<BlockPiece>();
        string before = Sprint1Builder.Snapshot(board.Model);
        Begin(handler, pointer);
        var data = EventAt(piece, coordinate, pointer);
        ExecuteEvents.Execute(handler.gameObject, data, ExecuteEvents.dragHandler);
        Require(Sprint1Builder.Snapshot(board.Model) == before, "Valid preview leaves model untouched");
        Require(Vector3.Distance(piece.OriginWorld, board.GetCellWorld(coordinate)) < .1f, "Drag visual/preview origin match");
        ExecuteEvents.Execute(handler.gameObject, data, ExecuteEvents.endDragHandler);
    }
    private static void Capture(int width, int height, string name, int spacing, bool notch)
    {
        var canvas = layer.GetComponentInParent<Canvas>();
        var camera = Camera.main;
        var safe = canvas.transform.Find("SafeArea").GetComponent<RectTransform>();
        var mode = canvas.renderMode; var oldCamera = canvas.worldCamera;
        var oldTarget = camera.targetTexture; bool oldOrtho = camera.orthographic;
        float oldSize = camera.orthographicSize;
        var rt = new RenderTexture(width, height, 24);
        var oldActive = RenderTexture.active;
        Texture2D image = null;
        try
        {
            rt.Create(); camera.targetTexture = rt;
            camera.orthographic = true; camera.orthographicSize = height / 2f;
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 10;
            safe.anchorMin = notch ? new Vector2(.02f, .04f) : Vector2.zero;
            safe.anchorMax = notch ? new Vector2(.98f, .94f) : Vector2.one;
            safe.offsetMin = safe.offsetMax = Vector2.zero;
            board.GetComponent<GridLayoutGroup>().spacing = new Vector2(spacing, spacing);
            Canvas.ForceUpdateCanvases();
            board.GetComponent<SquareBoardLayout>().SendMessage("LateUpdate");
            board.SendMessage("LateUpdate"); Canvas.ForceUpdateCanvases();
            foreach (var piece in initial) piece.FitSlot();
            Canvas.ForceUpdateCanvases();
            var rect = ((RectTransform)board.transform).rect;
            Require(rect.width > 0 && Mathf.Abs(rect.width - rect.height) < .1f, "Square at " + name);
            for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
            {
                var expected = new Vector2Int(x, y);
                Vector2 screen = RectTransformUtility.WorldToScreenPoint(camera, board.GetCellWorld(expected));
                Require(board.TryScreenToCell(screen, camera, out var mapped) && mapped == expected, "Mapping at " + name);
            }
            foreach (var piece in initial)
            {
                var corners = new Vector3[4];
                ((RectTransform)piece.transform.parent).GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    Vector3 local = safe.InverseTransformPoint(corner);
                    Require(local.x >= safe.rect.xMin - .1f && local.x <= safe.rect.xMax + .1f &&
                            local.y >= safe.rect.yMin - .1f && local.y <= safe.rect.yMax + .1f, "Slot inside SafeArea");
                }
            }
            camera.Render();
            RenderTexture.active = rt;
            image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply();
            Directory.CreateDirectory("Validation");
            File.WriteAllBytes("Validation/sprint1_" + name + ".png", image.EncodeToPNG());
            results.Add("PASS: " + width + "x" + height + (notch ? " simulated safe insets" : "") +
                        " square board, 64 mappings, 3 slots inside SafeArea; spacing " + spacing + ".");
        }
        finally
        {
            canvas.renderMode = mode; canvas.worldCamera = oldCamera;
            camera.targetTexture = oldTarget; camera.orthographic = oldOrtho; camera.orthographicSize = oldSize;
            RenderTexture.active = oldActive;
            if (image != null) UnityEngine.Object.DestroyImmediate(image);
            rt.Release(); UnityEngine.Object.DestroyImmediate(rt);
            safe.GetComponent<SafeArea>().SendMessage("Apply");
            Canvas.ForceUpdateCanvases();
            board.GetComponent<SquareBoardLayout>().SendMessage("LateUpdate");
            board.SendMessage("LateUpdate"); Canvas.ForceUpdateCanvases();
            foreach (var piece in initial) piece.FitSlot();
        }
    }
    private static void Require(bool condition, string message) => Sprint1Builder.Require(condition, message);
    private static void Finish(bool pass, string failure)
    {
        EditorApplication.update -= Check;
        Application.logMessageReceived -= Log;
        Directory.CreateDirectory("Validation");
        File.AppendAllText("Validation/sprint1.txt", string.Join("\n", results) + "\n" + (pass ? "SPRINT1_PLAY_PASS" : failure));
        if (pass) Debug.Log("SPRINT1_PLAY_PASS"); else Debug.LogError(failure);
        EditorApplication.Exit(pass ? 0 : 1);
    }
}
