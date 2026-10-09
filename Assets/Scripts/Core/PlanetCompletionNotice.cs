using UnityEngine;
namespace CosmicBlock.Core {
 // 0: not completed, 1: durable pending notification, 2: user acknowledged.
 public static class PlanetCompletionNotice {
  public static string Key(int id)=>"CosmicBlock.Planet"+id.ToString("00")+"CompletionNotice";
  public static bool Pending(int id)=>PlayerPrefs.GetInt(Key(id),0)==1;
  public static void InitializeLegacy(int id,int energy){if(PlayerPrefs.HasKey(Key(id)))return;PlayerPrefs.SetInt(Key(id),energy>=PlanetDefinitions.Get(id).Total?2:0);PlayerPrefs.Save();}
  public static void Queue(int id){if(PlayerPrefs.GetInt(Key(id),0)!=2)PlayerPrefs.SetInt(Key(id),1);}
  public static void Acknowledge(int id){PlayerPrefs.SetInt(Key(id),2);PlayerPrefs.Save();}
 }
}
