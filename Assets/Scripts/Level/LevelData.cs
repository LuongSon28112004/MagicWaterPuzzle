using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class SlotHolderData
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;

    public SlotHolderData(string name, Vector3 position, Vector3 rotation)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
    }
}


[Serializable]
public enum BlockColor
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

public enum MoveDir
{
    NORMAL,
    HORIZONTAL,
    VERTICAL
}

[Serializable]
public class IceInfor
{
    public bool IsIce;
    public int CountBreak;
}

[Serializable]
public class KeyInfor
{
    public bool IsKey;
    public BlockColor ColorKey;
}

[Serializable]
public class BlockerInfor
{
    public bool isBlocker;
    public MoveDir moveDir;

}

[Serializable]
public class BlockData
{
    public string name;
    // infor ice
    public IceInfor iceInfor;
    //infor key
    public KeyInfor keyInfor;
    //infor Blocker
    public BlockerInfor blockerInfor;
    public Vector3 position;
    public Vector3 rotation;
    public BlockColor color;
    public MoveDir moveDir;

    public BlockData(string name, Vector3 position, BlockColor color, Vector3 rotation, MoveDir moveDir)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
        this.color = color;
        this.moveDir = moveDir;
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
    public KeyInfor keyInfor;

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
    public int durationTime;
    public bool IsEvenX;
    public bool IsEvenY;
    public Vector2 BoundCam;
}
