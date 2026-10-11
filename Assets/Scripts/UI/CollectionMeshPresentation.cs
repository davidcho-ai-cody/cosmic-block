using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI {
 public sealed class CollectionMeshPresentation:MonoBehaviour {
  [Serializable] public struct Caption {public Text source;public TextMeshProUGUI target;public float size;public bool copyColor;}
  [SerializeField] Caption[] captions;
  public void Configure(Caption[] values){captions=values;Sync();}
  void LateUpdate()=>Sync();
  public void Sync(){float q=((RectTransform)transform).rect.width/1080f;if(captions!=null)foreach(var c in captions){c.target.text=c.source.text;c.target.gameObject.SetActive(c.source.gameObject.activeSelf);c.target.fontSizeMax=c.size*q;c.target.fontSizeMin=Mathf.Min(c.size*.65f,18)*q;c.target.fontSize=c.size*q;if(c.copyColor)c.target.color=c.source.color;}
   foreach(string name in new[]{"Back","Previous","Next"}){var node=transform.Find(name);var r=(RectTransform)node;float side=Mathf.Min(((RectTransform)transform).rect.width*.105f,((RectTransform)transform).rect.height*.075f);r.sizeDelta=Vector2.one*side;var g=node.Find("MeshSurface").GetComponent<HomeQuitMeshGraphic>();g.Geometry(side*.5f,1.5f*q);var icon=node.Find("MeshIcon").GetComponent<Graphic>();icon.color=new Color(.50f,.86f,1,node.GetComponent<Button>().interactable?1:.32f);}
  }
 }
}
