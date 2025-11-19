using System.Collections.Generic;
using UnityEngine;

public class BlockTwo : BaseBlock
{
   private Vector3 offset;
    private float zCoord;
    private Rigidbody2D rb;

    [SerializeField] List<Vector2> occupiedOffsets;

    // Hướng bị chặn
    private Vector2 blockNormal = Vector2.zero;

    private void Awake()
    {
        blockType = BlockType.TWO;
        maxCapacity = 2;
        blockID = 12;
        currentCapacity = maxCapacity;

        rb = GetComponent<Rigidbody2D>(); // Kéo nhưng không bị lực vật lý đẩy
    }

    void OnMouseDown()
    {
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        Vector3 target = GetMouseWorldPos() + offset;

        // Kiểm tra hướng bị chặn
        if (IsBlockedDirection(target))
            return;

        // Dùng MovePosition để tránh xuyên
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

    void OnMouseUp()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 1;

        // // Ensure there are occupied offsets before accessing
        // if (occupiedOffsets == null || occupiedOffsets.Count == 0)
        //     return;

        // // Sort offsets by distance to the block's center (closest first) and take the first
        // occupiedOffsets.Sort((a, b) => a.sqrMagnitude.CompareTo(b.sqrMagnitude));
        // Vector2 pos = occupiedOffsets[occupiedOffsets.Count - 1];
        // transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        occupiedOffsets.Clear();

        // lấy vị trí cell hoặc object mà bạn muốn snap vào
        Vector2 snapPos = other.transform.position;

        occupiedOffsets.Add(snapPos);

        // Direction block bị chặn
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
        blockNormal = Vector2.zero;
    }
    
}
