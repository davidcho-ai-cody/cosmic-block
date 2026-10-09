using System;
using System.IO;
using System.Collections.Generic;
using CosmicBlock.Core;
using CosmicBlock.UI;
using UnityEditor;
using UnityEngine;
public static class Sprint10Builder {
 // Alpha-mask largest inscribed body disks (alpha > 100, interior holes filled),
 // normalized to a .86 body diameter. PNG pixels and existing 01/02 calibration stay intact.
 static readonly float[][] scales={new[]{1.195f,1.238f,1.220f,1.203f,1.222f},new[]{1.207f,1.206f,1.179f,1.168f,1.281f},new[]{1.012f,1.109f,1.098f,1.014f,1.071f}};
 static readonly Vector2[][] centers={new[]{new Vector2(.515f,.530f),new Vector2(.508f,.523f),new Vector2(.521f,.515f),new Vector2(.525f,.538f),new Vector2(.526f,.547f)},new[]{new Vector2(.511f,.482f),new Vector2(.514f,.509f),new Vector2(.522f,.506f),new Vector2(.526f,.511f),new Vector2(.532f,.536f)},new[]{new Vector2(.506f,.501f),new Vector2(.518f,.506f),new Vector2(.515f,.499f),new Vector2(.515f,.529f),new Vector2(.514f,.498f)}};
 public static void Build(){
  AssetDatabase.Refresh();var art=AssetDatabase.LoadAssetAtPath<PlanetArtCatalog>("Assets/Resources/PlanetArtCatalog.asset");if(art==null)throw new Exception("Missing existing PlanetArtCatalog");
  foreach(var d in PlanetDefinitions.All)foreach(var path in d.StagePaths)if(!File.Exists(path))throw new Exception("Missing planet PNG: "+path);
  var entries=new List<PlanetArtCatalog.Art>();
  foreach(var d in PlanetDefinitions.All){PlanetArtCatalog.Art entry=null;foreach(var old in art.planets)if(old.planetId==d.Id)entry=old;
   if(entry==null)entry=new PlanetArtCatalog.Art{planetId=d.Id,stages=new Sprite[5],scales=scales[d.Id-3],bodyCenters=centers[d.Id-3]};
   for(int i=0;i<5;i++)entry.stages[i]=Import(d.StagePaths[i]);entries.Add(entry);
  }
  art.planets=entries.ToArray();art.lockedPlanet=Import("Assets/Art/Planets/Common/planet_locked.png");art.smallLock=Import("Assets/Art/UI/Common/planet_lock_simple.png");
  EditorUtility.SetDirty(art);AssetDatabase.SaveAssets();Debug.Log("SPRINT10_ART_READY planets=5 sprites=25");if(Application.isBatchMode)EditorApplication.Exit(0);
 }
 static Sprite Import(string path){var t=AssetImporter.GetAtPath(path)as TextureImporter;if(t==null)throw new Exception("Missing texture importer: "+path);
  t.textureType=TextureImporterType.Sprite;t.spriteImportMode=SpriteImportMode.Single;t.alphaSource=TextureImporterAlphaSource.FromInput;t.alphaIsTransparency=true;t.mipmapEnabled=false;t.isReadable=false;t.filterMode=FilterMode.Bilinear;t.wrapMode=TextureWrapMode.Clamp;t.textureCompression=TextureImporterCompression.Uncompressed;t.maxTextureSize=2048;
  var android=t.GetPlatformTextureSettings("Android");android.overridden=true;android.maxTextureSize=2048;android.format=TextureImporterFormat.ASTC_6x6;android.compressionQuality=100;t.SetPlatformTextureSettings(android);t.SaveAndReimport();
  var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);if(sprite==null)throw new Exception("Sprite import failed: "+path);return sprite;
 }
}
