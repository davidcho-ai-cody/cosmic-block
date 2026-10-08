using System.Collections.Generic;
using CosmicBlock.Blocks;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.Board {
 [RequireComponent(typeof(RectTransform),typeof(GridLayoutGroup))]
 public sealed class BoardView:MonoBehaviour {
  public static readonly Color EmptyFill=new Color(.22f,.36f,.55f,.92f);
  public static readonly Color EmptyBorder=new Color(.30f,.72f,.88f,.58f);
  public static readonly Color OccupiedFill=new Color(.43f,.56f,.94f);
  public static readonly Color OccupiedBorder=new Color(.62f,.77f,1,.72f);
  public static readonly Color ValidFill=new Color(.96f,.73f,.30f,.24f);
  public static readonly Color ValidBorder=new Color(1,.78f,.31f,1);
  public static readonly Color InvalidFill=new Color(.67f,.25f,.32f,.25f);
  public static readonly Color InvalidBorder=new Color(.82f,.32f,.38f,.96f);
  [SerializeField] Image[] cells;
  [SerializeField] Sprite emptySprite;[SerializeField] Sprite[] blockSprites;
  [SerializeField] float spriteVisualScale=1;
  public float SpriteVisualScale=>spriteVisualScale;
  public void ConfigureDensity(float scale){spriteVisualScale=scale;foreach(var cell in cells){var effect=cell.GetComponent<CosmicBlock.UI.CellSpriteDensity>();if(effect==null)effect=cell.gameObject.AddComponent<CosmicBlock.UI.CellSpriteDensity>();effect.Configure(scale);}}
  readonly Sprite[] placedSprites=new Sprite[64];
  public bool HasVisualSkin=>emptySprite!=null;
  public Sprite BlockSprite(int index)=>blockSprites==null||blockSprites.Length==0?null:blockSprites[index%blockSprites.Length];
  public int PaletteOf(Sprite sprite){if(blockSprites!=null)for(int i=0;i<blockSprites.Length;i++)if(blockSprites[i]==sprite)return i;return 0;}
  public int[] CaptureCells(){var result=new int[64];for(int i=0;i<64;i++)result[i]=Model.IsOccupied(i%8,i/8)?PaletteOf(placedSprites[i]??BlockSprite(0)):-1;return result;}
  public void RestoreCells(int[] values){ClearPreview();Model.Clear();for(int i=0;i<64;i++)if(values[i]>=0){Model.SetOccupied(i%8,i/8,true);placedSprites[i]=BlockSprite(values[i]);Refresh(i%8,i/8,true);}}
  public void ConfigureSkin(Sprite empty,Sprite[] blocks){emptySprite=empty;blockSprites=blocks;}
  public void PaintPlacement(BlockShape shape,int x,int y,Sprite sprite){if(sprite==null)return;foreach(var o in shape.Cells){int index=(y+o.y)*8+x+o.x;if(index<0||index>=64||!Model.IsOccupied(x+o.x,y+o.y))continue;placedSprites[index]=sprite;Refresh(x+o.x,y+o.y,true);}}
  readonly HashSet<int> preview=new HashSet<int>();bool previewValid;
  public BoardModel Model{get;private set;} public int CellCount=>cells==null?0:cells.Length;
  public int PreviewCount=>preview.Count; public bool PreviewValid=>previewValid;
  public void Configure(Image[] images)=>cells=images;
  public void Bind(BoardModel model){if(Model!=null)Model.CellChanged-=Refresh;Model=model;Model.CellChanged+=Refresh;for(int y=0;y<8;y++)for(int x=0;x<8;x++)Refresh(x,y,Model.IsOccupied(x,y));}
  void Refresh(int x,int y,bool occupied){int index=y*8+x;bool isPreview=preview.Contains(index);var image=cells[index];if(!occupied)placedSprites[index]=null;if(emptySprite!=null){image.sprite=occupied?(placedSprites[index]??BlockSprite(0)):emptySprite;image.preserveAspect=true;}image.color=isPreview?(previewValid?ValidFill:InvalidFill):(occupied?(emptySprite!=null?Color.white:OccupiedFill):EmptyFill);var outline=image.GetComponent<Outline>();if(outline!=null){outline.enabled=isPreview||emptySprite==null;outline.effectColor=isPreview?(previewValid?ValidBorder:InvalidBorder):(occupied?OccupiedBorder:EmptyBorder);outline.effectDistance=isPreview?new Vector2(3,-3):(emptySprite!=null?Vector2.zero:new Vector2(1.5f,-1.5f));outline.useGraphicAlpha=false;}}
  public float CellSize=>GetComponent<GridLayoutGroup>().cellSize.x;public float CellSpacing=>GetComponent<GridLayoutGroup>().spacing.x;
  public bool TryScreenToCell(Vector2 screen,Camera camera,out Vector2Int coordinate){coordinate=default;var rect=(RectTransform)transform;if(cells==null||cells.Length!=64||!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,screen,camera,out var local))return false;Vector3 first=cells[0].transform.localPosition;var grid=GetComponent<GridLayoutGroup>();float px=grid.cellSize.x+grid.spacing.x,py=grid.cellSize.y+grid.spacing.y;if(px<=0||py<=0)return false;coordinate=new Vector2Int(Mathf.RoundToInt((local.x-first.x)/px),Mathf.RoundToInt((first.y-local.y)/py));return coordinate.x>=0&&coordinate.y>=0&&coordinate.x<8&&coordinate.y<8;}
  public Vector3 GetCellWorld(Vector2Int c){var g=GetComponent<GridLayoutGroup>();return transform.TransformPoint(cells[0].transform.localPosition+new Vector3(c.x*(g.cellSize.x+g.spacing.x),-c.y*(g.cellSize.y+g.spacing.y),0));}
  public void ShowPreview(BlockShape shape,Vector2Int anchor,bool valid){ClearPreview();previewValid=valid;foreach(var o in shape.Cells){int x=anchor.x+o.x,y=anchor.y+o.y;if(x<0||y<0||x>=8||y>=8)continue;preview.Add(y*8+x);}foreach(int index in preview){int x=index%8,y=index/8;Refresh(x,y,Model.IsOccupied(x,y));}}
  public void ClearPreview(){preview.Clear();if(Model==null)return;for(int y=0;y<8;y++)for(int x=0;x<8;x++)Refresh(x,y,Model.IsOccupied(x,y));}
  void LateUpdate(){var g=GetComponent<GridLayoutGroup>();float size=Mathf.Max(0,(((RectTransform)transform).rect.width-g.padding.horizontal-g.spacing.x*7)/8);g.cellSize=new Vector2(size,size);}
  void OnDestroy(){if(Model!=null)Model.CellChanged-=Refresh;}
 }
}