using System;
using System.IO;
using System.Text;
using CosmicBlock.Blocks;
using UnityEngine;
namespace CosmicBlock.Core {
 [Serializable] public sealed class RunSnapshot {
  public int version=1,planetId,score,combo,blockSetNumber,planetEnergy;
  public bool active=true;
  // -1 is empty; 0/1/2 identify the existing palette, never UI text or Sprite names.
  public int[] cells,shapes,appearances;public bool[] consumed;
 }
 public static class RunSaveStore {
  public static string PathOverride;
  public static string SavePath=>PathOverride??Path.Combine(Application.persistentDataPath,"current-run.json");
  public static bool Valid(RunSnapshot s){
   if(s==null||s.version!=1||!s.active||s.planetId<1||s.planetId>5||!PlanetDefinitions.Get(s.planetId).ContentReady||!PlanetDefinitions.IsUnlocked(s.planetId)||s.score<0||s.combo<0||s.blockSetNumber<1||s.planetEnergy<0||s.planetEnergy>PlanetDefinitions.Get(s.planetId).Total||s.cells==null||s.cells.Length!=64||s.shapes==null||s.shapes.Length!=3||s.appearances==null||s.appearances.Length!=3||s.consumed==null||s.consumed.Length!=3)return false;
   var board=new CosmicBlock.Board.BoardModel();for(int i=0;i<64;i++){if(s.cells[i]<-1||s.cells[i]>2)return false;if(s.cells[i]>=0)board.SetOccupied(i%8,i/8,true);}if(board.SnapshotCompletedLines().LineCount!=0)return false;
   bool placeable=false;for(int i=0;i<3;i++){if(s.shapes[i]<0||s.shapes[i]>=BlockCatalog.Shapes.Count||s.appearances[i]<0||s.appearances[i]>2)return false;if(!s.consumed[i]&&board.CanPlaceAnywhere(BlockCatalog.Shapes[s.shapes[i]]))placeable=true;}return placeable;
  }
  public static RunSnapshot Load(){try{if(!File.Exists(SavePath)||new FileInfo(SavePath).Length>16384)return null;var s=JsonUtility.FromJson<RunSnapshot>(File.ReadAllText(SavePath,Encoding.UTF8));return Valid(s)?s:null;}catch(Exception e)when(e is IOException||e is UnauthorizedAccessException||e is ArgumentException){return null;}}
  public static bool Write(RunSnapshot s){try{if(!Valid(s)){Delete();return false;}Directory.CreateDirectory(Path.GetDirectoryName(SavePath));string temp=SavePath+".tmp";using(var stream=new FileStream(temp,FileMode.Create,FileAccess.Write,FileShare.None)){byte[] b=Encoding.UTF8.GetBytes(JsonUtility.ToJson(s));stream.Write(b,0,b.Length);stream.Flush(true);}if(File.Exists(SavePath))File.Replace(temp,SavePath,null);else File.Move(temp,SavePath);return true;}catch(Exception e)when(e is IOException||e is UnauthorizedAccessException){Debug.LogWarning("Run save could not be written: "+e.Message);return false;}}
  public static void Delete(){try{if(File.Exists(SavePath))File.Delete(SavePath);if(File.Exists(SavePath+".tmp"))File.Delete(SavePath+".tmp");}catch(Exception e)when(e is IOException||e is UnauthorizedAccessException){Debug.LogWarning("Run save could not be removed: "+e.Message);}}
 }
}
