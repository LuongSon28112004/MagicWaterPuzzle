using UnityEngine;

public class WaterSplash : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    protected MaterialPropertyBlock materialPropertyBlock;

    public void SetColor(Color color)
    {
        if (materialPropertyBlock == null)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        meshRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor("_Tint", color);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }


}
