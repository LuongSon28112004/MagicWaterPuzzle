using System.Collections.Generic;
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
    [SerializeField] private DirectionPipe directionPipe;
    [SerializeField] List<WaterTypeCounter> waterTypeCounters = new List<WaterTypeCounter>();


    public DirectionPipe DirectionPipe { get => directionPipe; set => directionPipe = value; }
}
