using UnityEngine;
namespace CosmicBlock.Core {
 public sealed class PlanetDefinition {
  public readonly int Id, Total, Predecessor; public readonly string Name,EnglishName; public readonly bool ContentReady,CompletionSpriteOnly; public readonly int[] Amounts;
  public PlanetDefinition(int id,string name,int total,bool ready,string englishName=""){Id=id;Name=name;EnglishName=englishName;Total=total;Predecessor=id-1;ContentReady=ready;// Stage 5 artwork is a phase image; only Total determines restoration completion.
   CompletionSpriteOnly=false;Amounts=new[]{150,250,350,450,300};for(int i=0;i<5;i++)Amounts[i]*=total/PlanetDefinitions.Planet01Total;}
  public string StageLabel(int stage,int energy)=>CompletionSpriteOnly&&stage==5&&energy<Total?"최종 복원":PlanetRestoration.NameForStage(stage);
  public string[] StagePaths {get {var paths=new string[5];string[] suffix={"barren","sprout","awakening","restoration","complete"};string[] legacy={"Desolate","Awakening","Recovering","Thriving","Restored"};for(int i=0;i<5;i++)paths[i]=Id==1?"Assets/Art/Planets/Planet01/Planet01_Stage0"+(i+1)+"_"+legacy[i]+".png":"Assets/Art/Planets/Planet"+Id.ToString("00")+"/planet"+Id.ToString("00")+"_stage0"+(i+1)+"_"+suffix[i]+".png";return paths;}}
  public string EnergyKey=>"CosmicBlock.Planet"+Id.ToString("00")+"Energy";
  public string UnlockKey=>"CosmicBlock.Planet"+Id.ToString("00")+"Unlocked";
  public int SpriteForStage(int stage,bool completed)=>CompletionSpriteOnly&&stage==5&&!completed?4:stage;
  public int SpriteStage(int energy)=>SpriteForStage(PlanetRestoration.StageForEnergy(energy,Id),energy>=Total);
 }
 public static class PlanetDefinitions {
  public const int Planet01Total=1500,Planet02Total=3000;
  public static readonly PlanetDefinition[] All={new PlanetDefinition(1,"푸른 별",Planet01Total,true,"BLUE STAR"),new PlanetDefinition(2,"크리스탈리아",Planet02Total,true,"CRYSTALIA"),new PlanetDefinition(3,"이그니스",6000,true,"IGNIS"),new PlanetDefinition(4,"글라시아",12000,true,"GLACIA"),new PlanetDefinition(5,"루미나",24000,true,"LUMINA")};
  public static PlanetDefinition Get(int id)=>id>=1&&id<=All.Length?All[id-1]:All[0];
  public static bool IsUnlocked(int id){if(id<1||id>5)return false;if(id==1)return true;var p=Get(id-1);return PlayerPrefs.GetInt(Get(id).UnlockKey,0)==1||PlayerPrefs.GetInt(p.EnergyKey,0)>=p.Total;}
 }
 public static class PlanetSelection {
  public const string Key="CosmicBlock.SelectedPlanet";
  public static int Current {get {int id=PlayerPrefs.GetInt(Key,1);return id>=1&&id<=5&&PlanetDefinitions.Get(id).ContentReady&&PlanetDefinitions.IsUnlocked(id)?id:1;}}
  public static bool Select(int id){if(id<1||id>5||!PlanetDefinitions.Get(id).ContentReady||!PlanetDefinitions.IsUnlocked(id))return false;if(PlayerPrefs.GetInt(Key,1)!=id){PlayerPrefs.SetInt(Key,id);PlayerPrefs.Save();}return true;}
 }
}
