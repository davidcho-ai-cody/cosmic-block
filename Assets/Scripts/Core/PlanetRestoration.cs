using UnityEngine;
namespace CosmicBlock.Core {
 public sealed class PlanetRestoration {
  public const int StageEnergyRequired=100,RequiredEnergy=400,CurrentVersion=2;
  public const string DefaultKey="CosmicBlock.Planet01Energy",VersionKey="CosmicBlock.PlanetRestorationVersion";
  static readonly string[] Names={"DESOLATE","AWAKENING","RECOVERING","THRIVING","RESTORED"};
  readonly string key,versionKey; public int CurrentEnergy{get;private set;}
  public int Stage=>StageForEnergy(CurrentEnergy); public string StageName=>Names[Stage-1];
  public int StageEnergy=>IsRestored?100:CurrentEnergy%100; public int Percent=>Mathf.RoundToInt(CurrentEnergy*100f/RequiredEnergy); public bool IsRestored=>CurrentEnergy>=RequiredEnergy;
  public PlanetRestoration(string persistenceKey=DefaultKey,string migrationVersionKey=VersionKey){key=persistenceKey;versionKey=migrationVersionKey;Load();}
  public void Load(){int stored=PlayerPrefs.GetInt(key,0);if(PlayerPrefs.GetInt(versionKey,0)<CurrentVersion){stored=Mathf.RoundToInt(Mathf.Clamp(stored,0,500)/500f*RequiredEnergy);PlayerPrefs.SetInt(versionKey,CurrentVersion);PlayerPrefs.SetInt(key,stored);PlayerPrefs.Save();}CurrentEnergy=Mathf.Clamp(stored,0,RequiredEnergy);}
  public void Save(){PlayerPrefs.SetInt(key,CurrentEnergy);PlayerPrefs.SetInt(versionKey,CurrentVersion);PlayerPrefs.Save();}
  public int AddEnergy(int amount){int before=CurrentEnergy;CurrentEnergy=Mathf.Clamp(CurrentEnergy+Mathf.Max(0,amount),0,RequiredEnergy);if(CurrentEnergy!=before)Save();return CurrentEnergy-before;}
  public void SetEnergy(int value){CurrentEnergy=Mathf.Clamp(value,0,RequiredEnergy);Save();}
  public static int AwardForLines(int lines)=>lines<=0?0:lines==1?10:lines==2?25:lines==3?45:70;
  public static int StageForEnergy(int energy){energy=Mathf.Clamp(energy,0,RequiredEnergy);return energy>=400?5:energy/100+1;}
  public static string NameForStage(int stage)=>Names[Mathf.Clamp(stage,1,5)-1];
  public static int StageEnergyForTotal(int energy){energy=Mathf.Clamp(energy,0,RequiredEnergy);return energy>=400?100:energy%100;}
  public static float StageFillForTotal(int energy)=>StageEnergyForTotal(energy)/100f;
  public static int PercentForTotal(int energy)=>Mathf.RoundToInt(Mathf.Clamp(energy,0,RequiredEnergy)*100f/RequiredEnergy);
 }
}
