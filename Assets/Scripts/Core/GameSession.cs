using System;
using System.Collections;
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
        public const float ClearLineInterval = .32f;
        private SequentialClearPlan pendingPlan;
        private Coroutine clearRoutine;
        private int nextLine, turnToken;
        private bool energyCommitted;
        private int clearEnergyBefore,clearEnergyAfter;
        public int LineComboIndex { get; private set; }
        public bool ClearSequenceActive => pendingPlan != null;
        public int PlacementCombo => Combo;


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
            ++turnToken;
            State = GameState.Resolving;
            board.ClearPreview();
            if (!Model.TryPlace(piece.Shape, x, y)) { State = GameState.Playing; return false; }
            board.PaintPlacement(piece.Shape, x, y, piece.AppearanceSprite);
            LastClear = Model.SnapshotCompletedLines();
            Combo = LastClear.LineCount > 0 ? (int)Math.Min(int.MaxValue, (long)Combo + 1) : 0;
            AddScore(piece.Shape.Cells.Count * ScoreRules.PointsPerPlacedCell);
            piece.Consume(); LineComboIndex=0;
            if (LastClear.LineCount==0) { FinishTurn(); return true; }
            pendingPlan=new SequentialClearPlan(LastClear);nextLine=0;energyCommitted=false;
            clearEnergyBefore=restoration.CurrentEnergy;CommitEnergy();clearEnergyAfter=restoration.CurrentEnergy;
            // Freeze visible progress while durable data is already saved.
            if(hud!=null&&hud.PlanetView!=null){hud.PlanetView.ResetTransient();if(clearEnergyAfter>clearEnergyBefore)hud.PlanetView.HoldEnergy(clearEnergyBefore);}
            clearRoutine=StartCoroutine(ResolveClear(turnToken));
            return true;
        }
        private IEnumerator ResolveClear(int token)
        {
            while (pendingPlan!=null && nextLine<pendingPlan.Steps.Count)
            {
                ResolveLine(true);
                if(nextLine<pendingPlan.Steps.Count)yield return new WaitForSecondsRealtime(ClearLineInterval);
            }
            yield return new WaitForSecondsRealtime(.68f);
            clearRoutine=null;
            if(token==turnToken)CompleteClearPresentation(token);
        }
        private void ResolveLine(bool present)
        {
            var step=pendingPlan.Steps[nextLine++];LineComboIndex=nextLine;
            // Copy source sprites while their original occupied view still exists.
            if(present && feedback!=null)feedback.PlayClear(step.Result,ScoreRules.LinePoints(LineComboIndex),LineComboIndex);
            Model.ClearCells(step.Cells);AddScore(ScoreRules.LinePoints(LineComboIndex));
        }
        private void AddScore(int points)
        {
            int before=Score;Score=(int)Math.Min(int.MaxValue,(long)Score+points);
            if(Score>BestScore){BestScore=Score;PlayerPrefs.SetInt(bestScoreKey,BestScore);}
            if(hud!=null){hud.Render(this);hud.NotifyJourneyCrossings(before,Score);}
        }
        private void CompleteClearPresentation(int token)
        {
            if(pendingPlan==null)return;
            int before=clearEnergyBefore,after=clearEnergyAfter;
            pendingPlan=null;PlayerPrefs.Save();
            bool allConsumed=true;foreach(var slot in slots)if(!slot.IsConsumed){allConsumed=false;break;}
            if(allConsumed)GenerateBlockSet();
            bool gameOver=!HasPlaceableRemainingBlock();
            bool stageChanges=PlanetRestoration.StageForEnergy(before)!=PlanetRestoration.StageForEnergy(after)||before<PlanetRestoration.RequiredEnergy&&after>=PlanetRestoration.RequiredEnergy;
            // The frozen clear is finished. Preserve free play during a non-boundary fragment flight.
            State=gameOver||stageChanges?GameState.Resolving:GameState.Playing;
            Action finish=()=>{if(token!=turnToken)return;State=gameOver?GameState.GameOver:GameState.Playing;if(hud!=null)hud.Render(this);};
            // Present before HUD refresh so the old stage/fill remains until fragment arrival.
            if(hud!=null && after>before){hud.PresentPlanetEnergy(LastClear,board,before,after,finish);hud.Render(this);return;}
            finish();
        }
        private void CommitEnergy(){if(energyCommitted)return;energyCommitted=true;restoration.AddEnergy(PlanetRestoration.AwardForLines(LastClear.LineCount));}
        private void FinishTurn()
        {
            bool allConsumed=true;foreach(var slot in slots)if(!slot.IsConsumed){allConsumed=false;break;}
            if(allConsumed)GenerateBlockSet();
            State=HasPlaceableRemainingBlock()?GameState.Playing:GameState.GameOver;
            PlayerPrefs.Save();if(hud!=null)hud.Render(this);
        }
        private void CancelResolution()
        {
            ++turnToken;if(clearRoutine!=null)StopCoroutine(clearRoutine);clearRoutine=null;
            if(pendingPlan!=null){while(nextLine<pendingPlan.Steps.Count)ResolveLine(false);CommitEnergy();pendingPlan=null;PlayerPrefs.Save();}
            if(State==GameState.Resolving)State=GameState.Playing;
        }
#if UNITY_EDITOR
        public void DebugCompleteTurnForProbe(){DebugCompleteLineSequence();if(hud!=null)hud.ResetPlanetFeedback();if(State==GameState.Resolving)FinishTurn();}
        // Legacy synchronous probes advance only the line timeline; real timing is tested separately.
        public void DebugCompleteLineSequence(){if(pendingPlan==null)return;if(clearRoutine!=null)StopCoroutine(clearRoutine);clearRoutine=null;while(nextLine<pendingPlan.Steps.Count)ResolveLine(true);CompleteClearPresentation(turnToken);}
#endif
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
            Score = Combo = BlockSetNumber = LineComboIndex = 0;
            LastClear = LineClearResult.Empty;
            generator = new BlockGenerator(useFixedSeed ? (int?)fixedSeed : null);
            GenerateBlockSet();
            State = GameState.Playing;
            if (slots != null && slots.Length > 0) EvaluateGameOver();
            else if (hud != null) hud.Render(this);
        }

        public void ResetTransientFeedback()
        {
            CancelResolution();
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
        [ContextMenu("Debug/Planet/Set 24%")] private void DebugPlanet24()=>DebugSetPlanetEnergy(360);
        [ContextMenu("Debug/Planet/Set 25%")] private void DebugPlanet25()=>DebugSetPlanetEnergy(375);
        [ContextMenu("Debug/Planet/Set 49%")] private void DebugPlanet49()=>DebugSetPlanetEnergy(735);
        [ContextMenu("Debug/Planet/Set 50%")] private void DebugPlanet50()=>DebugSetPlanetEnergy(750);
        [ContextMenu("Debug/Planet/Set 74%")] private void DebugPlanet74()=>DebugSetPlanetEnergy(1110);
        [ContextMenu("Debug/Planet/Set 75%")] private void DebugPlanet75()=>DebugSetPlanetEnergy(1125);
        [ContextMenu("Debug/Planet/Set 99%")] private void DebugPlanet99()=>DebugSetPlanetEnergy(1485);
        [ContextMenu("Debug/Planet/Set 100%")] private void DebugPlanet100()=>DebugSetPlanetEnergy(1500);
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
            if (!Application.isPlaying) return;
            ResetTransientFeedback();
            if (activeDrag != null) activeDrag.CancelDrag();
            board.ClearPreview(); Model.Clear(); DebugSetShapes(BlockCatalog.Shapes[0]);
            for (int x = 0; x < 8; x++) if (x != 3) Model.SetOccupied(x, 3, true);
        }
        [ContextMenu("Debug/Force Game Over")]
        public void DebugForceGameOver()
        {
            if (!Application.isPlaying) return;
            ResetTransientFeedback();
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
