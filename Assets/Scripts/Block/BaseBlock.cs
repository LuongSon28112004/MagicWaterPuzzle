using UnityEngine;

public enum BlockType
{
    L,
    ONE,
    PLUS,
    REACTANGLE,
    REVERSE_L,
    REVERSE_Z,
    SHORT_L,
    SHORT_T,
    THREE,
    THREE_TUT,
    THREE_SQUARE,
    TWO,
    TWO_SQUARE,
    U,
    Z,
}

public enum Direction
{
    NORMAL,
    VERTICAL,
    HORIZONTAL,
}


public abstract class BaseBlock : MonoBehaviour
{
    [SerializeField] protected BlockType blockType;
    [SerializeField] protected Direction blockDirection;
    [SerializeField] protected int blockID;
    [SerializeField] protected string blockName;
    [SerializeField] protected int blockColorID;
    [SerializeField] protected int maxCapacity;
    [SerializeField] protected int currentCapacity;

    public Direction BlockDirection { get => blockDirection; set => blockDirection = value; }
}
