using System;
using System.Collections.Generic;
using CosmicBlock.Blocks;
using CosmicBlock.Board;
using CosmicBlock.UI;
using CosmicBlock.Effects;
using UnityEngine;

namespace CosmicBlock.Core
{
    public enum GameState { Playing, Resolving, GameOver }
    public sealed class GameSession : MonoBehaviour
    {
        public const string DefaultBestScoreKey = "CosmicBlock.BestScore";
        [SerializeField] private BoardView board;
        [SerializeField] private BlockPiece[] slots;
        [SerializeField] private RectTransform dragLayer;
        [SerializeField, Min(0)] private float dragFingerOffset = 110;
        [SerializeField] private bool useFixedSeed;
        [SerializeField] private int fixedSeed = 1;
        [SerializeField] private GameHud hud;
        [SerializeField] private GameFeedbackController feedback;
        [SerializeField, HideInInspector] private string bestScoreKey = DefaultBestScoreKey;
        private BlockDragHandler activeDrag;
        private BlockGenerator generator;
        private PlanetRestoration restoration;

        public float DragFingerOffset => dragFingerOffset;
        public GameState State { get; private set; } = GameState.Playing;
        public BoardModel Model { get; private set; }
        public int Score { get; private set; }
        public int BestScore { get; private set; }
        public int Combo { get; private set; }
        public int BlockSetNumber { get; private set; }
        public LineClearResult LastClear { get; private set; } = LineClearResult.Empty;
        public JourneyStatus Journey => JourneyProgress.At(Score);
        public JourneyStatus BestJourney => JourneyProgress.At(BestScore);
        public IReadOnlyList<BlockPiece> Slots => slots;
        public PlanetRestoration Restoration => restoration;

        public void Configure(BoardView view) => board = view;
        public void ConfigureBlocks(BlockPiece[] pieces, RectTransform layer) { slots = pieces; dragLayer = layer; }
        public void ConfigureHud(GameHud view) => hud = view;
        public void ConfigureFeedback(GameFeedbackController controller) => feedback = controller;
        public GameFeedbackController Feedback => feedback;
        private void Awake()
        {
            Model = new BoardModel(); board.Bind(Model);
            BestScore = Mathf.Max(0, PlayerPrefs.GetInt(bestScoreKey, 0));
            restoration = new PlanetRestoration();
        }
        private void Start() { if (hud != null) hud.Connect(this); Retry(); }
        public bool TryBeginDrag(BlockDragHandler handler)
        {
            if (State != GameState.Playing || activeDrag != null) return false;
            activeDrag = handler; return true;
        }
        public void ReleaseDrag(BlockDragHandler handler) { if (activeDrag == handler) activeDrag = null; }

        // This is the only gameplay placement entry point. The handler finishes its drag first.
        public bool TryPlacePiece(BlockPiece piece, int x, int y)
        {
            if (State != GameState.Playing || activeDrag != null || piece == null || piece.IsConsumed ||
                slots == null || Array.IndexOf(slots, piece) < 0 || !Model.CanPlace(piece.Shape, x, y)) return false;
            State = GameState.Resolving;
            board.ClearPreview();
            if (!Model.TryPlace(piece.Shape, x, y)) { State = GameState.Playing; return false; }
            board.PaintPlacement(piece.Shape, x, y, piece.AppearanceSprite);
            LastClear = Model.ClearCompletedLines();
            Combo = LastClear.LineCount > 0 ? (int)Math.Min(int.MaxValue, (long)Combo + 1) : 0;
            int previousScore = Score;
            Score = ScoreRules.AddPlacement(Score, piece.Shape.Cells.Count, LastClear.LineCount, Combo);
            int previousPlanetEnergy = restoration.CurrentEnergy;
            int energyAward = restoration.AddEnergy(PlanetRestoration.AwardForLines(LastClear.LineCount));
            if (Score > BestScore)
            {
                BestScore = Score;
                PlayerPrefs.SetInt(bestScoreKey, BestScore);
                PlayerPrefs.Save();
            }
            piece.Consume();
            bool allConsumed = true;
            foreach (var slot in slots) if (!slot.IsConsumed) { allConsumed = false; break; }
            if (allConsumed) GenerateBlockSet();
            // Keep Resolving until the clear-to-planet presentation finishes.
            bool gameOverAfterResolution = !HasPlaceableRemainingBlock();
            bool lockForPlanetTransition = energyAward > 0 && PlanetRestoration.StageForEnergy(previousPlanetEnergy) != restoration.Stage;
            if (feedback != null && LastClear.LineCount > 0) feedback.PlayClear(LastClear, Score - previousScore, Combo);
            Action finishResolution = () =>
            {
                State = gameOverAfterResolution ? GameState.GameOver : GameState.Playing;
                if (hud != null) hud.Render(this);
            };
            if (!lockForPlanetTransition && !gameOverAfterResolution) State = GameState.Playing;
            if (hud != null)
            {
                hud.Render(this); hud.NotifyJourneyCrossings(previousScore, Score);
                if (energyAward > 0 && hud.PresentPlanetEnergy(LastClear, board, previousPlanetEnergy, restoration.CurrentEnergy, finishResolution)) return true;
            }
            finishResolution();
            return true;
        }
        private void GenerateBlockSet()
        {
            if (slots == null || slots.Length == 0) return;
            foreach (var piece in slots) piece.Initialize(generator.Next(), board, dragLayer, this);
            BlockSetNumber++;
        }
        public bool HasPlaceableRemainingBlock()
        {
            if (slots == null) return false;
            foreach (var piece in slots)
                if (piece != null && !piece.IsConsumed && Model.CanPlaceAnywhere(piece.Shape)) return true;
            return false;
        }
        public void EvaluateGameOver()
        {
            if (State != GameState.Playing) return;
            if (!HasPlaceableRemainingBlock())
            {
                if (activeDrag != null) activeDrag.CancelDrag();
                board.ClearPreview();
                State = GameState.GameOver;
            }
            if (hud != null) hud.Render(this);
        }
        public void Retry()
        {
            ResetTransientFeedback();
            if (State == GameState.Resolving) return;
            if (activeDrag != null) activeDrag.CancelDrag();
            State = GameState.Resolving;
            board.ClearPreview(); Model.Clear();
            Score = Combo = BlockSetNumber = 0;
            LastClear = LineClearResult.Empty;
            generator = new BlockGenerator(useFixedSeed ? (int?)fixedSeed : null);
            GenerateBlockSet();
            State = GameState.Playing;
            if (slots != null && slots.Length > 0) EvaluateGameOver();
            else if (hud != null) hud.Render(this);
        }

        public void ResetTransientFeedback()
        {
            if (hud != null) hud.ResetJourneyFeedback();
            if (feedback != null) feedback.ResetFeedback();
            if (hud != null) hud.ResetPlanetFeedback();
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public void DebugSetScore(int value)
        {
            if (!Application.isPlaying || State == GameState.Resolving) return;
            int previous = Score; Score = Mathf.Max(0, value);
            if (hud != null) { hud.Render(this); hud.NotifyJourneyCrossings(previous, Score); }
        }
        public void DebugSetPlanetEnergy(int value){if(!Application.isPlaying)return;ResetTransientFeedback();restoration.SetEnergy(value);if(hud!=null)hud.Render(this);}
        [ContextMenu("Debug/Planet/Set 0%")] private void DebugPlanet0()=>DebugSetPlanetEnergy(0);
        [ContextMenu("Debug/Planet/Set 24%")] private void DebugPlanet24()=>DebugSetPlanetEnergy(99);
        [ContextMenu("Debug/Planet/Set 25%")] private void DebugPlanet25()=>DebugSetPlanetEnergy(100);
        [ContextMenu("Debug/Planet/Set 49%")] private void DebugPlanet49()=>DebugSetPlanetEnergy(199);
        [ContextMenu("Debug/Planet/Set 50%")] private void DebugPlanet50()=>DebugSetPlanetEnergy(200);
        [ContextMenu("Debug/Planet/Set 74%")] private void DebugPlanet74()=>DebugSetPlanetEnergy(299);
        [ContextMenu("Debug/Planet/Set 75%")] private void DebugPlanet75()=>DebugSetPlanetEnergy(300);
        [ContextMenu("Debug/Planet/Set 99%")] private void DebugPlanet99()=>DebugSetPlanetEnergy(399);
        [ContextMenu("Debug/Planet/Set 100%")] private void DebugPlanet100()=>DebugSetPlanetEnergy(400);
        [ContextMenu("Debug/Planet/Add 10 Energy")] private void DebugPlanetAdd10(){restoration.AddEnergy(10);if(hud!=null)hud.Render(this);}
        [ContextMenu("Debug/Planet/Reset Planet 01")] private void DebugPlanetReset()=>DebugSetPlanetEnergy(0);
        [ContextMenu("Debug/Journey/Set Score 950")] private void DebugScore950() => DebugSetScore(950);
        [ContextMenu("Debug/Journey/Set Score 2950")] private void DebugScore2950() => DebugSetScore(2950);
        [ContextMenu("Debug/Journey/Set Score 5950")] private void DebugScore5950() => DebugSetScore(5950);
        [ContextMenu("Debug/Journey/Set Score 9950")] private void DebugScore9950() => DebugSetScore(9950);
        [ContextMenu("Debug/Prepare Row And Column Clear")]
        public void DebugPrepareCross()
        {
            if (!Application.isPlaying) return;
            Retry(); DebugSetShapes(BlockCatalog.Shapes[0]);
            for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
                if ((x == 3 || y == 3) && !(x == 3 && y == 3)) Model.SetOccupied(x, y, true);
        }
        [ContextMenu("Debug/Prepare Next Row Clear")]
        public void DebugPrepareNextRow()
        {
            if (!Application.isPlaying || State != GameState.Playing) return;
            if (activeDrag != null) activeDrag.CancelDrag();
            board.ClearPreview(); Model.Clear(); DebugSetShapes(BlockCatalog.Shapes[0]);
            for (int x = 0; x < 8; x++) if (x != 3) Model.SetOccupied(x, 3, true);
        }
        [ContextMenu("Debug/Force Game Over")]
        public void DebugForceGameOver()
        {
            if (!Application.isPlaying || State == GameState.Resolving) return;
            if (activeDrag != null) activeDrag.CancelDrag();
            board.ClearPreview(); Model.Clear(); DebugSetShapes(BlockCatalog.Shapes[2]);
            for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
                if (x != y) Model.SetOccupied(x, y, true);
            State = GameState.Playing; EvaluateGameOver();
        }
        private void DebugSetShapes(BlockShape shape)
        {
            foreach (var piece in slots) piece.Initialize(shape, board, dragLayer, this);
        }
#endif
    }
}
