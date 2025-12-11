using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


[Serializable]
public class InforColorPipe
{
    public Color color;
    public WaterTypeColor waterTypeColor;
}

public class PipeLineCtrl : MonoBehaviour
{
    private const int MAX_HEIGHT_FILL = 5;
    [SerializeField] private GameObject PipeLineWater;
    [SerializeField] private int currentFillColor;
    [SerializeField] private int maxColor;
    [SerializeField] InforColorPipe[] colors;
    [SerializeField] float[] fills;
    protected MaterialPropertyBlock materialPropertyBlock;

    public InforColorPipe[] Colors { get => colors; set => colors = value; }
    public float[] Fills { get => fills; set => fills = value; }

    public void HideWater()
    {
        PipeLineWater.SetActive(false);
    }

    public void ShowWater()
    {
        PipeLineWater.SetActive(true);
        StartCoroutine(FillUpAllColors());
    }

    // Show Color Water
    // Fill tất cả layer từ 0 lên đến fills[i] lần lượt
    public IEnumerator FillUpAllColors(float durationPerLayer = 0.2f)
    {
        // reset currentFillColor
        currentFillColor = 1;

        // Set tất cả layer Fill = 0 trước khi fill từng cái
        MeshRenderer mesh = PipeLineWater.GetComponent<MeshRenderer>();
        mesh.GetPropertyBlock(materialPropertyBlock);

        for (int i = 0; i < maxColor; i++)
        {
            materialPropertyBlock.SetFloat("_Fill" + (i + 1), 0f);
        }
        mesh.SetPropertyBlock(materialPropertyBlock);

        // fill từng lớp một
        for (int i = 1; i <= maxColor; i++)
        {
            yield return StartCoroutine(FillUpColorCoroutine(i, durationPerLayer));
        }
    }

    // Coroutine fill 1 layer từ 0 → fills[layerIndex-1]
    private IEnumerator FillUpColorCoroutine(int layerIndex, float duration)
    {
        MeshRenderer mesh = PipeLineWater.GetComponent<MeshRenderer>();
        mesh.GetPropertyBlock(materialPropertyBlock);

        float targetFill = fills[layerIndex - 1];
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentFill = Mathf.Lerp(0f, targetFill, t);

            materialPropertyBlock.SetFloat("_Fill" + layerIndex, currentFill / MAX_HEIGHT_FILL);
            mesh.SetPropertyBlock(materialPropertyBlock);

            yield return null;
        }

        materialPropertyBlock.SetFloat("_Fill" + layerIndex, targetFill / MAX_HEIGHT_FILL);
        mesh.SetPropertyBlock(materialPropertyBlock);
    }


    //Init Color
    public void InitColor(List<GateColorInfo> colorOutputs)
    {
        currentFillColor = 1;
        maxColor = colorOutputs.Count;
        int layerCount = colorOutputs.Count;
        colors = new InforColorPipe[layerCount];
        fills = new float[layerCount];
        for (int i = 0; i < layerCount; i++)
        {
            fills[i] = colorOutputs[i].capacity;
            // khoi tao
            colors[i] = new InforColorPipe();
            if (colorOutputs[i].color == BlockColor.Red)
            {
                colors[i].color = Color.red;
                colors[i].waterTypeColor = WaterTypeColor.Red;
            }
            else if (colorOutputs[i].color == BlockColor.Blue)
            {
                colors[i].color = Color.blue;
                colors[i].waterTypeColor = WaterTypeColor.Blue;
            }
            else if (colorOutputs[i].color == BlockColor.Green)
            {
                colors[i].color = Color.green;
                colors[i].waterTypeColor = WaterTypeColor.Green;
            }
            else if (colorOutputs[i].color == BlockColor.Yellow)
            {
                colors[i].color = Color.yellow;
                colors[i].waterTypeColor = WaterTypeColor.Yellow;
            }
            else if (colorOutputs[i].color == BlockColor.purple)
            {
                colors[i].color = new Color(173f / 255f, 3f / 255f, 252f / 255f);
                colors[i].waterTypeColor = WaterTypeColor.purple;

            }
            else if (colorOutputs[i].color == BlockColor.pink)
            {
                colors[i].color = new Color(252f / 255f, 3f / 255f, 173f / 255f);
                colors[i].waterTypeColor = WaterTypeColor.pink;
            }
            else if (colorOutputs[i].color == BlockColor.Brown)
            {
                colors[i].color = new Color(139f / 255f, 69f / 255f, 19f / 255f);
                colors[i].waterTypeColor = WaterTypeColor.Brown;

            }
            else if (colorOutputs[i].color == BlockColor.Turquoise)
            {
                colors[i].color = new Color(64f / 255f, 224f / 255f, 208f / 255f);
                colors[i].waterTypeColor = WaterTypeColor.Turquoise;
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
            materialPropertyBlock.SetColor("_Color" + (i + 1), colors[i].color);
            materialPropertyBlock.SetFloat("_Fill" + (i + 1), fills[i] / MAX_HEIGHT_FILL);
        }

        mesh.SetPropertyBlock(materialPropertyBlock);

    }


    //Fill Color
    public IEnumerator FillColor(int reduce, Action<int> action, float duration = 1f)
    {
        yield return StartCoroutine(FillColorCoroutine(reduce, currentFillColor, duration, action));
    }

    private IEnumerator FillColorCoroutine(int reduce, int layerIndex, float duration, Action<int> action)
    {
        MeshRenderer mesh = PipeLineWater.GetComponent<MeshRenderer>();
        mesh.GetPropertyBlock(materialPropertyBlock);
        while (fills[layerIndex - 1] == 0)
        {
            layerIndex++;
            currentFillColor++;
        }

        print(layerIndex);

        float startFill = fills[layerIndex - 1]; // giá trị fill hiện tại
        if (fills[layerIndex - 1] < reduce)
        {
            fills[layerIndex - 1] = 0;
            reduce -= (int)fills[layerIndex - 1];
        }
        else if (fills[layerIndex - 1] == reduce)
        {
            fills[layerIndex - 1] = 0;
            reduce = 0;
        }
        else
        {
            fills[layerIndex - 1] -= reduce;
            reduce = 0;
        }
        float endFill = fills[layerIndex - 1];
        float elapsed = 0f;
        float currentFill = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            currentFill = Mathf.Lerp(startFill, endFill, t); // giảm dần về 0
            materialPropertyBlock.SetFloat("_Fill" + layerIndex, currentFill / MAX_HEIGHT_FILL); // chia 3 như lúc trước
            mesh.SetPropertyBlock(materialPropertyBlock);
            yield return null;
        }

        // đảm bảo chắc chắn fill = endfill
        materialPropertyBlock.SetFloat("_Fill" + layerIndex, currentFill / MAX_HEIGHT_FILL);
        mesh.SetPropertyBlock(materialPropertyBlock);
        if (fills[layerIndex - 1] == 0)
        {
            currentFillColor += 1;
        }
        action.Invoke(reduce);
    }
    public IEnumerator FillColor(int indexColor, int reduce, float duration = 0.5f)
    {
        yield return StartCoroutine(FillColorCoroutineReduce(reduce, indexColor, duration));
    }

    private IEnumerator FillColorCoroutineReduce(int reduce, int layerIndex, float duration)
    {
        MeshRenderer mesh = PipeLineWater.GetComponent<MeshRenderer>();
        mesh.GetPropertyBlock(materialPropertyBlock);

        while (fills[layerIndex - 1] == 0)
        {
            layerIndex++;
            currentFillColor++;
        }

        float startFill = fills[layerIndex - 1]; // giá trị fill hiện tại
        if (fills[layerIndex - 1] < reduce)
        {
            fills[layerIndex - 1] = 0;
            reduce -= (int)fills[layerIndex - 1];
        }
        else if (fills[layerIndex - 1] == reduce)
        {
            fills[layerIndex - 1] = 0;
            reduce = 0;
        }
        else
        {
            fills[layerIndex - 1] -= reduce;
            reduce = 0;
        }
        float endFill = fills[layerIndex - 1];
        float elapsed = 0f;
        float currentFill = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            currentFill = Mathf.Lerp(startFill, endFill, t); // giảm dần về 0
            materialPropertyBlock.SetFloat("_Fill" + layerIndex, currentFill / MAX_HEIGHT_FILL); // chia 3 như lúc trước
            mesh.SetPropertyBlock(materialPropertyBlock);
            yield return null;
        }

        // đảm bảo chắc chắn fill = endfill
        materialPropertyBlock.SetFloat("_Fill" + layerIndex, currentFill / MAX_HEIGHT_FILL);
        mesh.SetPropertyBlock(materialPropertyBlock);
    }




}
