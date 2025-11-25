using UnityEngine;

public class PipeLineHeadCtrl : MonoBehaviour
{
    [SerializeField] private MeshRenderer IceMeshRenderer;
    //protected MaterialPropertyBlock materialPropertyBlock;

    public void ChangeColorIce(Material material)
    {
        // if (materialPropertyBlock == null)
        // {
        //     materialPropertyBlock = new MaterialPropertyBlock();
        // }
        // IceMeshRenderer.GetPropertyBlock(materialPropertyBlock);
        IceMeshRenderer.material = material;
    }

}
