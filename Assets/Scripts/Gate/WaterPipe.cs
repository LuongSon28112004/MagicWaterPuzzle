using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public enum DirectionPipe
{
    Up,
    Down,
    Left,
    Right
}

public enum WaterTypeColor
{
    None,
    Red,
    Blue,
    Green,
    Yellow
}

[Serializable]
public class WaterTypeCounter
{
    public WaterTypeColor waterTypeColor;
    public int count;

    public WaterTypeCounter(WaterTypeColor color, int count)
    {
        this.waterTypeColor = color;
        this.count = count;
    }
}

public class WaterPipe : MonoBehaviour
{
    [Header("Water pipe Component")]
    [SerializeField] private DirectionPipe directionPipe;
    [SerializeField] List<WaterTypeCounter> waterTypeCounters;

    [Header("ref")]
    [SerializeField] private PipeLineCtrl pipeLineCtrl;



    // Getter and Setter
    public DirectionPipe DirectionPipe { get => directionPipe; set => directionPipe = value; }
    public List<WaterTypeCounter> WaterTypeCounters { get => waterTypeCounters; set => waterTypeCounters = value; }
    public PipeLineCtrl PipeLineCtrl { get => pipeLineCtrl; set => pipeLineCtrl = value; }

    public void InitColorPipe(List<GateColorInfo> colorOutputs)
    {
        pipeLineCtrl.InitColor(colorOutputs);
        waterTypeCounters = new List<WaterTypeCounter>(colorOutputs.Count);
        InitWaterTypeCounter(colorOutputs);
    }

    private void InitWaterTypeCounter(List<GateColorInfo> colorOutputs)
    {
        waterTypeCounters.Clear();

        for (int i = 0; i < colorOutputs.Count; i++)
        {
            WaterTypeColor waterColor = WaterTypeColor.None;
            if (colorOutputs[i].color == BlockColor.Red)
                waterColor = WaterTypeColor.Red;
            else if (colorOutputs[i].color == BlockColor.Blue)
                waterColor = WaterTypeColor.Blue;
            else if (colorOutputs[i].color == BlockColor.Green)
                waterColor = WaterTypeColor.Green;

            // Thêm phần tử mới thay vì truy cập theo index
            waterTypeCounters.Add(new WaterTypeCounter(waterColor, colorOutputs[i].capacity));
        }
    }

    public void UpdateListWaterTypeCounter()
    {
        for (int i = 0; i < waterTypeCounters.Count; i++)
        {
            if (waterTypeCounters[i].count == 0)
            {
                waterTypeCounters.RemoveAt(i);
            }
        }
    }

}
