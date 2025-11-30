using System;
using System.Collections;
using UnityEngine;

public class BlockTypeVariant : MonoBehaviour
{
    [Header("BlockType Variant Component")]
    public GameObject BlockVaritant;
    public GameObject Water;
    public MeshRenderer waterMeshRenderer;
    public MeshFilter waterMeshFilter;
    protected MaterialPropertyBlock materialPropertyBlock;

    // Dir
    public BlockMoveDir blockMoveDir;


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
        AutoComputeMinMax(direction);

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

    public void AutoComputeMinMax(Vector3 fillDir)
    {
        fillDir.Normalize();

        Mesh mesh = waterMeshFilter.sharedMesh;
        Vector3[] verts = mesh.vertices;

        float minV = float.MaxValue;
        float maxV = float.MinValue;

        for (int i = 0; i < verts.Length; i++)
        {
            float proj = Vector3.Dot(verts[i], fillDir);

            if (proj < minV) minV = proj;
            if (proj > maxV) maxV = proj;
        }

        Debug.Log(transform.root.name + "  " + minV + " " + maxV);

        materialPropertyBlock.SetFloat("_MinValue", minV);
        materialPropertyBlock.SetFloat("_MaxValue", maxV);
    }


    // add Dir Move
    public void SetDirMove(MoveDir moveDir, Direction blockDirection)
    {
        blockMoveDir.InitMoveDirection(moveDir, blockDirection);
    }


}
