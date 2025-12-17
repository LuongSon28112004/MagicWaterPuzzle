using System;
using System.Collections;
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
    Yellow,
    purple,
    pink,
    Brown,
    Turquoise,
    Orange,
    Darkgreen
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
    [SerializeField] private PipeLineHeadCtrl pipeLineHeadCtrl;
    [SerializeField] private PipeIdleBubbleParticle pipeIdleBubbleParticle;



    // Getter and Setter
    public DirectionPipe DirectionPipe { get => directionPipe; set => directionPipe = value; }
    public List<WaterTypeCounter> WaterTypeCounters { get => waterTypeCounters; set => waterTypeCounters = value; }
    public PipeLineCtrl PipeLineCtrl { get => pipeLineCtrl; set => pipeLineCtrl = value; }
    public PipeLineHeadCtrl PipeLineHeadCtrl { get => pipeLineHeadCtrl; set => pipeLineHeadCtrl = value; }

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
            else if (colorOutputs[i].color == BlockColor.Yellow)
                waterColor = WaterTypeColor.Yellow;
            else if (colorOutputs[i].color == BlockColor.purple)
                waterColor = WaterTypeColor.purple;
            else if (colorOutputs[i].color == BlockColor.pink)
                waterColor = WaterTypeColor.pink;
            else if (colorOutputs[i].color == BlockColor.Brown)
                waterColor = WaterTypeColor.Brown;
            else if (colorOutputs[i].color == BlockColor.Turquoise)
                waterColor = WaterTypeColor.Turquoise;
            else if (colorOutputs[i].color == BlockColor.Orange)
                waterColor = WaterTypeColor.Orange;
            else if (colorOutputs[i].color == BlockColor.Darkgreen)
                waterColor = WaterTypeColor.Darkgreen;

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
        UpdateIce();
        UpdateColorSplash();
    }

    // FillColor
    public IEnumerator FillColorWater(float Height, int value, Action<int> action)
    {
        PipeLineHeadCtrl.PlayParticleWaterFall(Height);
        yield return StartCoroutine(PipeLineCtrl.FillColor(value, action));
        PipeLineHeadCtrl.StopParticleWaterFall();
    }


    //Splash
    public void UpdateColorSplash()
    {
        if (waterTypeCounters.Count > 0)
        {
            WaterTypeColor waterTypeColor = waterTypeCounters[0].waterTypeColor;
            Color hexColor;
            if (!ColorUtility.TryParseHtmlString(Contacts.HexColorSplash(waterTypeColor), out hexColor))
            {
                // fallback color if parsing fails
                hexColor = Color.white;
            }
            PipeLineHeadCtrl.SetColorSplash(hexColor);
        }

    }



    // ice
    public void UpdateIce()
    {
        if (waterTypeCounters.Count > 0)
        {
            WaterTypeColor waterTypeColor = waterTypeCounters[0].waterTypeColor;
            pipeLineHeadCtrl.ChangeColorIce(Contacts.GetColorMatPipe(waterTypeColor).Frame_01);
        }
        else
        {
            pipeLineHeadCtrl.ChangeColorIce(Contacts.Instance.ice);
            pipeIdleBubbleParticle.StopParticle();
        }
    }


    // idle bubble
    public void PlayParticleIdleBubble()
    {
        pipeIdleBubbleParticle.PlayParticle();
    }


    // water fall
    public void PlayParticleWaterFall(float Height)
    {
        pipeLineHeadCtrl.PlayParticleWaterFall(Height);
    }

    public void StopParticleWaterFall()
    {
        pipeLineHeadCtrl.StopParticleWaterFall();
    }



}
