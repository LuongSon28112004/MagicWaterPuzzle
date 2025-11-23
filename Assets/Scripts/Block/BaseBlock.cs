using System;
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

    // Hướng bị chặn
    protected Vector2 blockNormal = Vector2.zero;

    public Direction BlockDirection { get => blockDirection; set => blockDirection = value; }

    // 
    public void AddVisualColor(BlockColor color)
    {
        blockColorVisual = color;
        ColorMaterialCongig colorMat = Contacts.GetColorMat(color);
        if (colorMat == null) return;
        MeshRenderer[] meshRenderers = blockVisual.BlockVaritant.GetComponentsInChildren<MeshRenderer>();
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
        ProcessTriggerWaterPipe(other);
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

    protected virtual void ProcessTriggerWaterPipe(Collider2D other)
    {
        WaterPipe waterPipe = other.GetComponentInParent<WaterPipe>();
        if (waterPipe != null)
        {
            Debug.Log("Enter WaterPipe Color");
            // xử lý water pipe change color

            ////
            // chặn không cho di chuyển nữa
            IsMove = false;
            transform.position = SnapToGrid(transform.position);

        }
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
}
