using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using DG.Tweening;
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
    [Header("BaseBlock Components")]
    [SerializeField] protected BlockType blockType;
    [SerializeField] protected Direction blockDirection;
    [SerializeField] protected int blockID;
    [SerializeField] protected string blockName;
    [SerializeField] protected int blockColorID;
    [SerializeField] protected int maxCapacity;
    [SerializeField] protected int currentCapacity;

    // component
    [SerializeField] protected Rigidbody2D rb;
    //Move
    [SerializeField] protected Vector3 offset;
    [SerializeField] protected float zCoord;

    // ref
    [SerializeField] protected BlockVisual blockVisual;
    [SerializeField] protected BlockColor blockColorVisual;


    // check kéo
    [SerializeField] protected bool isGragging = false;
    [SerializeField] protected bool IsMove = true;
    [SerializeField] protected bool isFill = false;

    // Hướng bị chặn
    protected Vector2 blockNormal = Vector2.zero;

    //Block particle
    [SerializeField] protected List<BlockParticle> blockParticles;


    public Direction BlockDirection { get => blockDirection; set => blockDirection = value; }

    // 
    public virtual void AddVisualColor(BlockColor color)
    {
        blockColorVisual = color;
        blockVisual.blockTypeVariant.AddVisual(color);
    }

    protected Vector2 SnapToPipe(Vector2 pipePos)
    {
        // vẫn dùng logic odd/even theo hướng
        if (blockDirection == Direction.HORIZONTAL)
        {
            float yFix = SnapEven(pipePos.y);
            float xFix = SnapOdd(pipePos.x);
            return new Vector2(xFix, yFix);
        }
        else // VERTICAL
        {
            float xFix = SnapEven(pipePos.x);
            float yFix = SnapOdd(pipePos.y);
            return new Vector2(xFix, yFix);
        }
    }



    //add visual water
    public virtual void AddVisualWater(BlockColor blockColor)
    {
        // for override
    }

    public virtual Vector3 DirectionWater(Direction direction)
    {
        return Vector3.zero;
        // for override
    }


    // OnMouse Click
    void OnMouseDown()
    {
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseUp()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 1;
        isGragging = false;
        transform.position = SnapToGrid(transform.position);
    }

    //Làm tròn về lẻ gần nhất
    protected float SnapOdd(float v)
    {
        float rounded = Mathf.Round((v - 1f) / 2f) * 2f + 1f;
        return rounded;
    }


    // Làm tròn về chẵn gần nhất
    protected float SnapEven(float v)
    {
        return Mathf.Round(v / 2f) * 2f;
    }

    protected virtual Vector2 SnapToGrid(Vector2 position)
    {
        return Vector2.zero;
    }

    void OnMouseDrag()
    {
        if (!IsMove) return;
        isGragging = true;
        Vector3 target = GetMouseWorldPos() + offset;

        if (IsBlockedDirection(target))
        {
            return;
        }

        rb.MovePosition(target);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.linearDamping = 0;
        rb.angularDamping = 0;
    }

    private bool IsBlockedDirection(Vector3 targetPos)
    {
        if (blockNormal == Vector2.left && targetPos.x > transform.position.x) return true;
        if (blockNormal == Vector2.right && targetPos.x < transform.position.x) return true;
        if (blockNormal == Vector2.down && targetPos.y > transform.position.y) return true;
        if (blockNormal == Vector2.up && targetPos.y < transform.position.y) return true;
        return false;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    // Trigger xử lý va chạm
    void OnTriggerEnter2D(Collider2D other)
    {
        ProcessTriggerEnter2D(other);
    }

    private void ProcessTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("BoxCamera"))
        {
            Debug.Log("Enter BoxCamera");
            return;
        }
        ProcessTriggerMove(other);
        StartCoroutine(ProcessTriggerWaterPipe(other));
    }

    void ProcessTriggerMove(Collider2D other)
    {
        if ((other.GetComponentInParent<BlockTwo>() != null ||
             other.GetComponentInParent<BaseBlock>() != null) && isGragging)
        {
            Vector3 pos = transform.position;
            pos.z = -1;
            transform.position = pos;
        }
        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol == null) return;

        ColliderDistance2D dist = other.Distance(myCol);
        Vector2 normal = dist.normal;

        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
            blockNormal = new Vector2(Mathf.Sign(normal.x), 0);
        else
            blockNormal = new Vector2(0, Mathf.Sign(normal.y));
    }

    protected IEnumerator ProcessTriggerWaterPipe(Collider2D other)
    {
        if (isFill) yield break;
        WaterPipe waterPipe = other.GetComponentInParent<WaterPipe>();
        if (waterPipe == null) yield break;
        if (waterPipe.WaterTypeCounters.Count == 0) yield break;
        if (CheckSameColor(waterPipe.WaterTypeCounters[0].waterTypeColor, blockColorVisual))
        {
            isFill = true;
            Debug.Log("Enter WaterPipe Color");
            // chặn không cho di chuyển nữa
            IsMove = false;
            Vector3 pos = transform.position;
            transform.position = SnapToGrid(pos);
            yield return StartCoroutine(FillPipeAndBlock(waterPipe));
            // thả di chuyển ra khi đã fill song
            IsMove = true;
            isFill = false;
        }
    }

    private IEnumerator FillPipeAndBlock(WaterPipe waterPipe)
    {
        //play sound
        AudioManager.Instance.PlayOneShot("WaterPOURvar1S1", 1);
        StartCoroutine(waterPipe.PipeLineCtrl.FillColor());
        yield return StartCoroutine(ProcessFillWaterBlock(waterPipe));
    }

    public IEnumerator ProcessFillWaterBlock(WaterPipe waterPipe)
    {
        int value = waterPipe.WaterTypeCounters[0].count;
        int remainingCapacity = maxCapacity - currentCapacity;
        if (value <= remainingCapacity)
        {
            int addCapacity = currentCapacity + value;
            currentCapacity += value;
            waterPipe.WaterTypeCounters[0].count -= value;
            PlayParticleBlock();
            yield return StartCoroutine(blockVisual.blockTypeVariant.FillWater(addCapacity * 1.0f / maxCapacity));
        }
        else
        {
            int addCapacity = currentCapacity + remainingCapacity;
            currentCapacity += remainingCapacity;
            waterPipe.WaterTypeCounters[0].count -= remainingCapacity;
            PlayParticleBlock();
            yield return StartCoroutine(blockVisual.blockTypeVariant.FillWater(addCapacity * 1.0f / maxCapacity));
        }

        waterPipe.UpdateListWaterTypeCounter();
    }

    public IEnumerator ProcessFillWaterPipe(WaterPipe waterPipe)
    {
        yield return StartCoroutine(waterPipe.PipeLineCtrl.FillColor());
    }

    protected virtual void PlayParticleBlock()
    {

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("BoxCamera"))
        {
            Debug.Log("Enter BoxCamera");
            return;
        }
        if ((other.GetComponentInParent<BlockTwo>() != null ||
             other.GetComponentInParent<BaseBlock>() != null) && isGragging)
        {
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }

        blockNormal = Vector2.zero;
    }

    protected bool CheckSameColor(WaterTypeColor waterTypeColor, BlockColor blockColorVisual)
    {
        string a = waterTypeColor.ToString();
        string b = blockColorVisual.ToString();
        return a.Contains(b);
    }
}
