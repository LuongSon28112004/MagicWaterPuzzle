using System.Collections.Generic;
using UnityEngine;

public class BlockL : BaseBlock
{
    [Header("BlockL Components")]
    //Move
    [SerializeField] private Vector3 offset;
    [SerializeField] private float zCoord;
    [SerializeField] private bool isGragging = false;
    [SerializeField] private Rigidbody2D rb;
    // ref
    [SerializeField] private BlockVisual blockVisual;
    [SerializeField] private BlockColor blockColorVisual;

    // Hướng bị chặn
    private Vector2 blockNormal = Vector2.zero;

    private void Awake()
    {
        blockType = BlockType.TWO;
        maxCapacity = 2;
        blockID = 12;
        currentCapacity = maxCapacity;
        rb = GetComponent<Rigidbody2D>();
    }

    public void AddVisualColor(BlockColor color)
    {
        blockColorVisual = color;
        ColorMaterialCongig colorMat = Contacts.GetColorMat(color);
        if (colorMat == null) return;
        MeshRenderer[] meshRenderers = blockVisual.BlockVaritant.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.materials = new Material[] { colorMat.Glass_01, colorMat.Frame_01 };
        }
    }

    void OnMouseDown()
    {
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
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

    public Vector2 SnapToGrid(Vector2 pos)
    {
        float cellSize = 1f;
        Vector2 origin = Vector2.zero;

        // --- Tạo danh sách các điểm snap có thể ---
        List<Vector2> candidates = new List<Vector2>();

        // Snap thô theo lưới
        float xBase = Mathf.Round((pos.x - origin.x) / cellSize) * cellSize + origin.x;
        float yBase = Mathf.Round((pos.y - origin.y) / cellSize) * cellSize + origin.y;

        // 4 vị trí có thể snap gần nhất
        candidates.Add(new Vector2(xBase, yBase));
        candidates.Add(new Vector2(xBase + cellSize, yBase));
        candidates.Add(new Vector2(xBase - cellSize, yBase));
        candidates.Add(new Vector2(xBase, yBase + cellSize));
        candidates.Add(new Vector2(xBase, yBase - cellSize));

        // --- Lọc theo hướng block ---
        List<Vector2> filtered = new List<Vector2>();

        foreach (var c in candidates)
        {
            if (blockDirection == Direction.HORIZONTAL)
            {
                // Y phải chẵn, X phải lẻ
                float yFix = Mathf.Round(c.y / 2f) * 2f + 1;
                float xFix = Mathf.Round(c.x / 2f) * 2f;

                filtered.Add(new Vector2(xFix, yFix));
            }
            else
            {
                // X phải chẵn, Y phải lẻ
                float xFix = Mathf.Round(c.x / 2f) * 2f + 1;
                float yFix = Mathf.Round(c.y / 2f) * 2f;

                filtered.Add(new Vector2(xFix, yFix));
            }
        }

        // --- Chọn điểm gần nhất ---
        Vector2 best = filtered[0];
        float bestDist = Vector2.Distance(pos, best);

        for (int i = 1; i < filtered.Count; i++)
        {
            float d = Vector2.Distance(pos, filtered[i]);
            if (d < bestDist)
            {
                bestDist = d;
                best = filtered[i];
            }
        }

        return best;
    }



    void OnMouseUp()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 1;
        isGragging = false;
        transform.position = SnapToGrid(transform.position);
    }





    void OnTriggerEnter2D(Collider2D other)
    {
        ProcessTriggerEnter2D(other);
    }

    private void ProcessTriggerEnter2D(Collider2D other)
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

    void OnTriggerExit2D(Collider2D other)
    {
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
