using UnityEngine;
namespace CosmicBlock.Core {
 public sealed class PlanetRestoration {
  public const int RequiredEnergy=500; public const string DefaultKey="CosmicBlock.Planet01Energy"; readonly string key;
  public int CurrentEnergy{get;private set;} public int Percent=>Mathf.FloorToInt(CurrentEnergy*100f/RequiredEnergy); public int Stage=>StageForEnergy(CurrentEnergy); public bool IsRestored=>CurrentEnergy>=RequiredEnergy;
  public PlanetRestoration(string persistenceKey=DefaultKey){key=persistenceKey;Load();}
  public void Load()=>CurrentEnergy=Mathf.Clamp(PlayerPrefs.GetInt(key,0),0,RequiredEnergy);
  public void Save(){PlayerPrefs.SetInt(key,CurrentEnergy);PlayerPrefs.Save();}
  public int AddEnergy(int amount){int before=CurrentEnergy;CurrentEnergy=Mathf.Clamp(CurrentEnergy+Mathf.Max(0,amount),0,RequiredEnergy);if(CurrentEnergy!=before)Save();return CurrentEnergy-before;}
  public void SetEnergy(int value){CurrentEnergy=Mathf.Clamp(value,0,RequiredEnergy);Save();}
  public static int AwardForLines(int lines)=>lines<=0?0:lines==1?10:lines==2?25:lines==3?45:70;
  public static int StageForEnergy(int energy){int p=Mathf.FloorToInt(Mathf.Clamp(energy,0,RequiredEnergy)*100f/RequiredEnergy);return p>=100?5:p>=75?4:p>=50?3:p>=25?2:1;}
 }
}