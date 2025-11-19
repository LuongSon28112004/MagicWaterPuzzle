using System.Collections.Generic;
using UnityEngine;

public class BlockCube : BaseBlock
{
     private Vector3 offset;
    private float zCoord;
    private Rigidbody2D rb;

    // Hướng bị chặn
    private Vector2 blockNormal = Vector2.zero;

    private void Awake()
    {
        blockType = BlockType.ONE;
        maxCapacity = 1;
        blockID = 2;
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
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;

        // Ưu tiên hướng mạnh nhất
        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
            blockNormal = new Vector2(Mathf.Sign(normal.x), 0);
        else
            blockNormal = new Vector2(0, Mathf.Sign(normal.y));
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        blockNormal = Vector2.zero;
    }
}
