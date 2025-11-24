using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PipeLineCtrl : MonoBehaviour
{
    [SerializeField] private GameObject PipeLineWater;
    [SerializeField] private int currentFillColor;
    [SerializeField] private int maxColor;
    Color[] colors;
    float[] fills;
    protected MaterialPropertyBlock materialPropertyBlock;

    public void InitColor(List<GateColorInfo> colorOutputs)
    {
        currentFillColor = 1;
        maxColor = colorOutputs.Count;
        int layerCount = colorOutputs.Count;
        colors = new Color[layerCount];
        fills = new float[layerCount];
        for (int i = 0; i < layerCount; i++)
        {
            fills[i] = colorOutputs[i].capacity;
            if (colorOutputs[i].color == BlockColor.Red)
            {
                colors[i] = Color.red;
            }
            else if (colorOutputs[i].color == BlockColor.Blue)
            {
                colors[i] = Color.blue;
            }
            else if (colorOutputs[i].color == BlockColor.Green)
            {
                colors[i] = Color.green;
            }
        }
        if (materialPropertyBlock == null)
        {
            materialPropertyBlock ??= new MaterialPropertyBlock();
        }
        MeshRenderer mesh = PipeLineWater.GetComponent<MeshRenderer>();
        mesh.GetPropertyBlock(materialPropertyBlock);
        // Material material = PipeLineWater.GetComponent<MeshRenderer>().material;
        materialPropertyBlock.SetFloat("_LayerCount", layerCount);
        for (int i = 0; i < layerCount; i++)
        {
            materialPropertyBlock.SetColor("_Color" + (i + 1), colors[i]);
            materialPropertyBlock.SetFloat("_Fill" + (i + 1), fills[i] / 3);
        }

        mesh.SetPropertyBlock(materialPropertyBlock);

    }


    public IEnumerator FillColor(float duration = 1.5f)
    {
        yield return StartCoroutine(FillColorCoroutine(currentFillColor, duration));
    }

    private IEnumerator FillColorCoroutine(int layerIndex, float duration)
    {
        MeshRenderer mesh = PipeLineWater.GetComponent<MeshRenderer>();
        mesh.GetPropertyBlock(materialPropertyBlock);

        float startFill = fills[layerIndex - 1]; // giá trị fill hiện tại
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentFill = Mathf.Lerp(startFill, 0f, t); // giảm dần về 0
            materialPropertyBlock.SetFloat("_Fill" + layerIndex, currentFill / 3); // chia 3 như lúc trước
            mesh.SetPropertyBlock(materialPropertyBlock);
            yield return null;
        }

        // đảm bảo chắc chắn fill = 0
        materialPropertyBlock.SetFloat("_Fill" + layerIndex, 0f);
        mesh.SetPropertyBlock(materialPropertyBlock);
        currentFillColor += 1;
    }

}
