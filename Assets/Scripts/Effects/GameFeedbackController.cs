using System.Collections;
using CosmicBlock.Board;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicBlock.Effects
{
    public sealed class GameFeedbackController : MonoBehaviour
    {
        static readonly Color Gold = new Color(1f, .76f, .28f, .96f);
        static readonly Color SoftWhite = new Color(1f, .96f, .82f, .94f);
        static readonly Color CosmicBlue = new Color(.38f, .72f, 1f, .92f);
        [SerializeField] BoardView board;
        [SerializeField] RectTransform effectRoot;
        [SerializeField] Image[] clearCells;
        [SerializeField] Text[] stars;
        [SerializeField] Text scorePop;
        [SerializeField] Text comboPop;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip clearClip;
        Coroutine routine;
        readonly Vector2[] starOrigins = new Vector2[24];
        readonly Vector2[] starDirections = new Vector2[24];
        public int PlayCount { get; private set; }
        public int LastCellCount { get; private set; }
        public int LastParticleCount { get; private set; }
        public int LastScoreDelta { get; private set; }
        public int LastCombo { get; private set; }
        public string LastComboLabel { get; private set; } = "";
        public float LastPitch { get; private set; }
        public int HapticRequestCount { get; private set; }
        public bool IsPlaying => routine != null;
        public int ActiveCellCount => CountActive(clearCells);
        public int ActiveStarCount => CountActive(stars);
        public int CellPoolCapacity => clearCells == null ? 0 : clearCells.Length;
        public int StarPoolCapacity => stars == null ? 0 : stars.Length;
        public bool HasAudioClip => clearClip != null;
        public void Configure(BoardView boardView, RectTransform root, Image[] cells, Text[] starPool, Text scoreText, Text comboText, AudioSource source, AudioClip clip)
        {
            board=boardView;effectRoot=root;clearCells=cells;stars=starPool;scorePop=scoreText;comboPop=comboText;audioSource=source;clearClip=clip;ResetFeedback();
        }
        public void PlayClear(LineClearResult result,int scoreDelta,int combo)
        {
            if(result==null||result.LineCount==0||result.UniqueClearedCells.Count==0)return;
            ResetVisuals();PlayCount++;LastCellCount=Mathf.Min(result.UniqueClearedCells.Count,clearCells.Length);
            LastParticleCount=Mathf.Min(stars.Length,Mathf.Clamp(8+(result.LineCount-1)*4,8,24));
            LastScoreDelta=scoreDelta;LastCombo=combo;LastComboLabel=combo<=1?"CLEAR!":combo==2?"STAR COMBO":"COSMIC COMBO x"+combo;
            LastPitch=combo<=1?1f:combo==2?1.05f:1.1f;Vector2 center=Vector2.zero;
            for(int i=0;i<LastCellCount;i++){var cell=clearCells[i];var rect=(RectTransform)cell.transform;rect.anchoredPosition=effectRoot.InverseTransformPoint(board.GetCellWorld(result.UniqueClearedCells[i]));rect.sizeDelta=Vector2.one*board.CellSize;rect.localScale=Vector3.one;cell.color=Gold;cell.gameObject.SetActive(true);center+=rect.anchoredPosition;}
            center/=LastCellCount;
            for(int i=0;i<LastParticleCount;i++){float angle=i*2.39996323f;float radius=8f+(i%4)*9f;starOrigins[i]=center+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius;starDirections[i]=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*(55f+(i%5)*13f);var star=stars[i];star.rectTransform.anchoredPosition=starOrigins[i];star.rectTransform.localScale=Vector3.one*(result.LineCount>1?1.12f:1f);star.color=i%3==0?CosmicBlue:i%3==1?SoftWhite:Gold;star.gameObject.SetActive(true);}
            scorePop.text="+"+scoreDelta;comboPop.text=LastComboLabel;scorePop.rectTransform.anchoredPosition=center+new Vector2(0,65);comboPop.rectTransform.anchoredPosition=center+new Vector2(0,115);scorePop.gameObject.SetActive(true);comboPop.gameObject.SetActive(true);
            if(audioSource!=null&&clearClip!=null){audioSource.Stop();audioSource.clip=clearClip;audioSource.pitch=LastPitch;audioSource.volume=.58f;audioSource.Play();}
            RequestHaptic();routine=StartCoroutine(Animate(result.LineCount));
        }
        IEnumerator Animate(int lineCount)
        {
            const float flash=.10f,pop=.18f,total=.68f;float elapsed=0f;
            while(elapsed<total){elapsed+=Time.unscaledDeltaTime;if(elapsed<=flash){float pulse=1f+Mathf.Sin(elapsed/flash*Mathf.PI)*.12f;SetCellScale(pulse);}else if(elapsed<=flash+pop){float p=(elapsed-flash)/pop;float scale=p<.35f?Mathf.Lerp(1f,1.16f,p/.35f):Mathf.Lerp(1.16f,0f,(p-.35f)/.65f);SetCellScale(scale);SetCellAlpha(1f-Mathf.Clamp01((p-.3f)/.7f));}else DeactivateCells();
                float starP=Mathf.Clamp01(elapsed/.42f);for(int i=0;i<LastParticleCount;i++){stars[i].rectTransform.anchoredPosition=starOrigins[i]+starDirections[i]*starP;SetAlpha(stars[i],1f-starP);}
                float textP=Mathf.Clamp01(elapsed/total);scorePop.rectTransform.anchoredPosition+=Vector2.up*Time.unscaledDeltaTime*38f;comboPop.rectTransform.localScale=Vector3.one*Mathf.Lerp(lineCount>1?1.18f:1.06f,1f,textP);SetAlpha(scorePop,textP<.18f?textP/.18f:1f-Mathf.Clamp01((textP-.56f)/.44f));SetAlpha(comboPop,textP<.12f?textP/.12f:1f-Mathf.Clamp01((textP-.62f)/.38f));yield return null;}
            ResetVisuals();routine=null;
        }
        public void ResetFeedback(){if(routine!=null)StopCoroutine(routine);routine=null;ResetVisuals();if(audioSource!=null)audioSource.Stop();}
        void ResetVisuals(){DeactivateCells();if(stars!=null)foreach(var star in stars)if(star!=null)star.gameObject.SetActive(false);if(scorePop!=null)scorePop.gameObject.SetActive(false);if(comboPop!=null)comboPop.gameObject.SetActive(false);}
        void SetCellScale(float value){for(int i=0;i<LastCellCount;i++)clearCells[i].rectTransform.localScale=Vector3.one*value;}
        void SetCellAlpha(float alpha){for(int i=0;i<LastCellCount;i++)SetAlpha(clearCells[i],alpha);}
        void DeactivateCells(){if(clearCells==null)return;foreach(var cell in clearCells)if(cell!=null)cell.gameObject.SetActive(false);}
        static void SetAlpha(Graphic graphic,float alpha){var color=graphic.color;color.a=Mathf.Clamp01(alpha);graphic.color=color;}
        static int CountActive<T>(T[] graphics)where T:Graphic{int count=0;if(graphics==null)return 0;foreach(var graphic in graphics)if(graphic!=null&&graphic.gameObject.activeSelf)count++;return count;}
        void RequestHaptic(){HapticRequestCount++;
#if UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }
        void OnDisable()=>ResetFeedback();
    }
}

