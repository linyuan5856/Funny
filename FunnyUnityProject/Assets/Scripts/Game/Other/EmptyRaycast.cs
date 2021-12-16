using UnityEngine.UI;

public class EmptyRaycast : Graphic
{
   protected override void OnPopulateMesh(VertexHelper vh)
   {
      vh.Clear();
   }
}