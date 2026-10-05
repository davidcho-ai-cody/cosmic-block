using UnityEngine;
using UnityEngine.UI;
namespace CosmicBlock.UI
{
    public sealed class CollectionStageRing : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Vector2 center=rectTransform.rect.center;
            float radius=Mathf.Min(rectTransform.rect.width,rectTransform.rect.height)*.47f;
            float thickness=1.5f;
            for(int i=0;i<64;i++)
            {
                float a=i*Mathf.PI*2/64,b=(i+1)*Mathf.PI*2/64;
                Vector2 u=new Vector2(Mathf.Cos(a),Mathf.Sin(a)),v=new Vector2(Mathf.Cos(b),Mathf.Sin(b));
                int n=mesh.currentVertCount;
                mesh.AddVert(center+u*radius,color,Vector2.zero);
                mesh.AddVert(center+u*(radius-thickness),color,Vector2.zero);
                mesh.AddVert(center+v*(radius-thickness),color,Vector2.zero);
                mesh.AddVert(center+v*radius,color,Vector2.zero);
                mesh.AddTriangle(n,n+1,n+2);mesh.AddTriangle(n,n+2,n+3);
            }
        }
    }
}
