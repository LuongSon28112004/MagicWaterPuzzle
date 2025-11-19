using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SlotHolderData
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;

    public SlotHolderData(string name, Vector3 position , Vector3 rotation)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
    }
}

public enum BlockColor
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple,
    Orange,
}

[Serializable]
public class BlockData
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public BlockColor color;

    public BlockData(string name, Vector3 position, BlockColor color, Vector3 rotation)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
        this.color = color;
    }
}

[Serializable]
public class GateColorInfo
{
    public BlockColor color;
    public int capacity;

    public GateColorInfo(BlockColor color, int capacity)
    {
        this.color = color;
        this.capacity = capacity;
    }
}

[Serializable]
public class GateData
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;

    // List các màu mà gate sẽ đổ ra
    public List<GateColorInfo> colorOutputs = new List<GateColorInfo>();

    public GateData(string name, Vector3 position, Vector3 rotation)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
    }
}


[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public List<SlotHolderData> slotHolders = new List<SlotHolderData>();
    public List<BlockData> blocks = new List<BlockData>();
    public List<GateData> gates = new List<GateData>();
}
