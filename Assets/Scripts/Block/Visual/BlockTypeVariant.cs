using System;
using System.Collections;
using UnityEngine;

public class BlockTypeVariant : MonoBehaviour
{
    public GameObject BlockVaritant;
    public GameObject Water;
    public MeshRenderer waterMeshRenderer;
    protected MaterialPropertyBlock materialPropertyBlock;


    public void AddVisual(BlockColor color)
    {
        ColorMaterialCongig colorMat = Contacts.GetColorMat(color);
        if (colorMat == null) return;
        MeshRenderer[] meshRenderers = BlockVaritant.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            if (meshRenderer.gameObject.name.Contains("Middle"))
            {
                meshRenderer.materials = new Material[] { colorMat.Glass_01 };
                continue;
            }
            meshRenderer.materials = new Material[] { colorMat.Glass_01, colorMat.Frame_01 };
        }
    }

    public void InitWater(BlockColor color, Vector3 direction)
    {
        if (materialPropertyBlock == null)
        {
            materialPropertyBlock ??= new MaterialPropertyBlock();
        }

        waterMeshRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_FillAmount", 0);
        materialPropertyBlock.SetVector("_FillDir", direction);

        //xet color water
        string hex = Contacts.HexColor(color);
        Color waterColor;
        ColorUtility.TryParseHtmlString(hex, out waterColor);

        materialPropertyBlock.SetColor("_WaterColor", waterColor);


        waterMeshRenderer.SetPropertyBlock(materialPropertyBlock);
    }


    public IEnumerator FillWater(float valueFill, float duration = 1f)
    {
        yield return StartCoroutine(FillWaterCoroutine(valueFill, duration));
    }

    private IEnumerator FillWaterCoroutine(float targetFill, float duration)
    {
        if (materialPropertyBlock == null)
            materialPropertyBlock = new MaterialPropertyBlock();

        // Lấy block hiện tại
        waterMeshRenderer.GetPropertyBlock(materialPropertyBlock);

        // Fill ban đầu
        float startFill = materialPropertyBlock.GetFloat("_FillAmount");
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // Lerp từ startFill → targetFill
            float currentFill = Mathf.Lerp(startFill, targetFill, t);

            materialPropertyBlock.SetFloat("_FillAmount", currentFill);
            waterMeshRenderer.SetPropertyBlock(materialPropertyBlock);

            yield return null;
        }

        // Đảm bảo kết thúc chính xác target
        materialPropertyBlock.SetFloat("_FillAmount", targetFill);
        waterMeshRenderer.SetPropertyBlock(materialPropertyBlock);
    }

}
