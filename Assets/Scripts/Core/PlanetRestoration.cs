using UnityEngine;
namespace CosmicBlock.Core {
 public sealed class PlanetRestoration {
  public const int RequiredEnergy=PlanetDefinitions.Planet01Total,Planet02RequiredEnergy=PlanetDefinitions.Planet02Total,CurrentVersion=2;
  public const string DefaultKey="CosmicBlock.Planet01Energy",Planet02Key="CosmicBlock.Planet02Energy",Planet02UnlockKey="CosmicBlock.Planet02Unlocked",VersionKey="CosmicBlock.PlanetRestorationVersion",Planet01DisplayName="푸른 별";
  
  static readonly string[] Names={"황폐","싹틈","깨어남","회복","완성"};
  readonly string key,versionKey;readonly int planetNumber;
  public int LastUnlockedPlanetId{get;private set;}
  public int PlanetId=>planetNumber;
  public PlanetDefinition Definition=>PlanetDefinitions.Get(planetNumber);
  public int Total=>Definition.Total;
  public int SpriteStage=>Definition.SpriteStage(CurrentEnergy);
  public int CurrentEnergy{get;private set;}
  public int Stage=>StageForEnergy(CurrentEnergy,planetNumber);
  public string StageName=>Definition.StageLabel(Stage,CurrentEnergy);
  public int StageRequired=>RequiredForStage(Stage,planetNumber);
  public int StageEnergy=>StageEnergyForTotal(CurrentEnergy,planetNumber);
  public int Percent=>PercentForTotal(CurrentEnergy,planetNumber);
  public float OverallProgress=>OverallProgressForTotal(CurrentEnergy,planetNumber);
  public bool IsRestored=>CurrentEnergy>=TotalRequired(planetNumber);
  public bool IsPlanet02Unlocked=>PlanetDefinitions.IsUnlocked(2);
  public PlanetRestoration(string persistenceKey=DefaultKey,string migrationVersionKey=VersionKey,int planet=1){planetNumber=PlanetDefinitions.Get(planet).Id;key=persistenceKey==DefaultKey&&planetNumber!=1?Definition.EnergyKey:persistenceKey;versionKey=migrationVersionKey;Load();}
  public void Load(){
   int stored=PlayerPrefs.GetInt(key,0);
   // Legacy migration remains 500 -> 400, never rescaled to the new 1500 target.
   if(planetNumber==1&&PlayerPrefs.GetInt(versionKey,0)<CurrentVersion){stored=stored>500?Mathf.Clamp(stored,0,RequiredEnergy):Mathf.RoundToInt(Mathf.Clamp(stored,0,500)/500f*400);PlayerPrefs.SetInt(versionKey,CurrentVersion);PlayerPrefs.SetInt(key,stored);PlayerPrefs.Save();}
   CurrentEnergy=Mathf.Clamp(stored,0,TotalRequired(planetNumber));
   if(key==Definition.EnergyKey)PlanetCompletionNotice.InitializeLegacy(planetNumber,CurrentEnergy);
   if(CurrentEnergy!=stored)Save();else if(key==Definition.EnergyKey&&IsRestored&&planetNumber<5&&PlayerPrefs.GetInt(PlanetDefinitions.Get(planetNumber+1).UnlockKey,0)!=1)Save();
  }
  public void Save(){PlayerPrefs.SetInt(key,CurrentEnergy);if(planetNumber==1)PlayerPrefs.SetInt(versionKey,CurrentVersion);if(key==Definition.EnergyKey&&IsRestored&&planetNumber<5)PlayerPrefs.SetInt(PlanetDefinitions.Get(planetNumber+1).UnlockKey,1);PlayerPrefs.Save();}
  public int AddEnergy(int amount){int before=CurrentEnergy;LastUnlockedPlanetId=0;bool nextUnlocked=planetNumber<5&&PlanetDefinitions.IsUnlocked(planetNumber+1);CurrentEnergy=(int)System.Math.Min(TotalRequired(planetNumber),(long)CurrentEnergy+Mathf.Max(0,amount));if(CurrentEnergy!=before){if(key==Definition.EnergyKey&&before<Total&&IsRestored)PlanetCompletionNotice.Queue(planetNumber);Save();if(key==Definition.EnergyKey&&before<Total&&IsRestored&&planetNumber<5&&!nextUnlocked)LastUnlockedPlanetId=planetNumber+1;}return CurrentEnergy-before;}
  public void RestoreCheckpointEnergy(int value){if(CurrentEnergy<Total&&value>=Total&&key==Definition.EnergyKey)PlanetCompletionNotice.Queue(planetNumber);SetEnergy(value);}
  public void SetEnergy(int value){LastUnlockedPlanetId=0;CurrentEnergy=Mathf.Clamp(value,0,TotalRequired(planetNumber));Save();}
  public static int AwardForLines(int lines)=>lines<=0?0:lines==1?10:lines==2?25:lines==3?45:70;
  public static int TotalRequired(int planet=1)=>PlanetDefinitions.Get(planet).Total;
  public static int RequiredForStage(int stage,int planet=1)=>PlanetDefinitions.Get(planet).Amounts[Mathf.Clamp(stage,1,5)-1];
  public static int StartForStage(int stage,int planet=1){int start=0;for(int i=1;i<Mathf.Clamp(stage,1,5);i++)start+=RequiredForStage(i,planet);return start;}
  public static int StageForEnergy(int energy,int planet=1){energy=Mathf.Clamp(energy,0,TotalRequired(planet));for(int stage=1;stage<5;stage++)if(energy<StartForStage(stage+1,planet))return stage;return 5;}
  public static string NameForStage(int stage)=>Names[Mathf.Clamp(stage,1,5)-1];
  public static int StageEnergyForTotal(int energy,int planet=1){energy=Mathf.Clamp(energy,0,TotalRequired(planet));return energy-StartForStage(StageForEnergy(energy,planet),planet);}
  public static float StageFillForTotal(int energy,int planet=1)=>StageEnergyForTotal(energy,planet)/(float)RequiredForStage(StageForEnergy(energy,planet),planet);
  public static float OverallProgressForTotal(int energy,int planet=1)=>Mathf.Clamp01(energy/(float)TotalRequired(planet));
  public static int PercentForTotal(int energy,int planet=1)=>Mathf.FloorToInt(OverallProgressForTotal(energy,planet)*100f);
 }
}
