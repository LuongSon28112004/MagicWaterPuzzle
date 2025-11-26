using UnityEngine;

public class PipeLineHeadCtrl : MonoBehaviour
{
    [SerializeField] private MeshRenderer IceMeshRenderer;
    //protected MaterialPropertyBlock materialPropertyBlock;
    [Header("ref")]
    [SerializeField] WaterFall waterFall;

    public void ChangeColorIce(Material material)
    {
        // if (materialPropertyBlock == null)
        // {
        //     materialPropertyBlock = new MaterialPropertyBlock();
        // }
        // IceMeshRenderer.GetPropertyBlock(materialPropertyBlock);
        IceMeshRenderer.material = material;
    }

    public void SetDirectionWaterFall(DirectionWaterSplash directionWaterSplash)
    {
        waterFall.DirectionWaterSplash = directionWaterSplash;
    }

    public void PlayParticleWaterFall(float Height)
    {
        waterFall.PlayParticle();
        waterFall.SetHeight(Height);
    }

    public void StopParticleWaterFall()
    {
        waterFall.StopParticle();
    }

    public void SetColorSplash(Color color)
    {
        waterFall.SetColorSplash(color);
    }

}
