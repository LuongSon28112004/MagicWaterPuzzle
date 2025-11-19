using UnityEngine;

public class BlockDragManager2D : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask draggableLayer; // Layer cho các object có thể kéo

    private Rigidbody2D draggedRb;
    private Vector3 offset;
    private float distance;

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#endif

#if UNITY_ANDROID || UNITY_IOS
        HandleTouch();
#endif
    }

    // ==========================
    //         MOUSE
    // ==========================
    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, draggableLayer);

            if (hit.collider != null)
            {
                draggedRb = hit.collider.attachedRigidbody;

                if (draggedRb != null)
                {
                    offset = (Vector3)draggedRb.position - cam.ScreenToWorldPoint(Input.mousePosition);
                }
            }
        }

        if (Input.GetMouseButton(0) && draggedRb != null)
        {
            MoveWithCollision(draggedRb, cam.ScreenToWorldPoint(Input.mousePosition) + offset);
        }

        if (Input.GetMouseButtonUp(0))
        {
            draggedRb = null;
        }
    }

    // ==========================
    //         TOUCH
    // ==========================
    void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);
        Vector2 touchPos = cam.ScreenToWorldPoint(t.position);

        if (t.phase == TouchPhase.Began)
        {
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero, 0f, draggableLayer);

            if (hit.collider != null)
            {
                draggedRb = hit.collider.attachedRigidbody;

                if (draggedRb != null)
                {
                    offset = (Vector3)draggedRb.position - cam.ScreenToWorldPoint(t.position);
                }
            }
        }

        if (t.phase == TouchPhase.Moved && draggedRb != null)
        {
            MoveWithCollision(draggedRb, cam.ScreenToWorldPoint(t.position) + offset);
        }

        if (t.phase == TouchPhase.Ended)
        {
            draggedRb = null;
        }
    }

    // ==========================
    //  Di chuyển có kiểm tra va chạm
    // ==========================
    void MoveWithCollision(Rigidbody2D rb, Vector3 targetPoint)
    {
        Vector2 moveDir = (Vector2)(targetPoint - (Vector3)rb.position);
        float moveDist = moveDir.magnitude;
        moveDir.Normalize();

        // SweepTest2D tương đương là Cast
        RaycastHit2D[] results = new RaycastHit2D[1];
        int hitCount = rb.Cast(moveDir, new ContactFilter2D().NoFilter(), results, moveDist);

        if (hitCount == 0)
        {
            // Không chạm vào gì -> di chuyển bình thường
            rb.MovePosition(targetPoint);
        }
        else
        {
            // Nếu va chạm -> dừng trước vật cản
            //rb.MovePosition(results[0].point - moveDir * 0.01f);
        }
    }
}
