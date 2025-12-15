using System;
using System.Collections;
using System.Collections.Generic;
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

public enum BlockMoveDirection
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
    [SerializeField] private BlockVisual blockVisual;
    [SerializeField] private BlockColor blockColorVisual;

    // ice Point
    [SerializeField] private List<Transform> listIcePos;


    // check kéo
    [SerializeField] protected bool isGragging = false;
    [SerializeField] protected bool IsMove = true;
    [SerializeField] protected bool isFill = false;
    private Vector3 smoothVelocity = Vector3.zero;
    private float smoothTime = 0.03f; // mượt hơn khi giảm giá trị


    // Hướng bị chặn
    protected Vector2 blockNormal = Vector2.zero;

    //Block particle
    [SerializeField] protected List<BlockParticle> blockParticles;
    // Tween Rorate
    private Tween rotateTween;


    //Getter And Setter
    public Direction BlockDirection { get => blockDirection; set => blockDirection = value; }
    public BlockColor BlockColorVisual { get => blockColorVisual; set => blockColorVisual = value; }
    public List<Transform> ListIcePos { get => listIcePos; set => listIcePos = value; }
    public BlockVisual BlockVisual { get => blockVisual; set => blockVisual = value; }



    // Hàm kiểm tra góc an toàn
    protected bool ApproxAngle(float target, float tolerance = 1f, float z = 0)
    {
        return Mathf.Abs(Mathf.DeltaAngle(z, target)) < tolerance;
    }

    // Move Direction
    public virtual void AddMoveDirection(MoveDir moveDir)
    {
        BlockVisual.blockTypeVariant.SetDirMove(moveDir, blockDirection, true);
    }

    // visual
    public virtual void AddVisualColor(BlockColor color)
    {
        blockColorVisual = color;
        BlockVisual.blockTypeVariant.AddVisual(color);
    }

    public virtual void AddIceBlock(int count)
    {
        BlockVisual.blockIce.ActiveIce(count, BlockDirection);
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

    public int TakeRemainingCapacity()
    {
        return maxCapacity - currentCapacity;
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
    private Sequence ClickLock;
    void OnMouseDown()
    {
        if (UIBlockChecker.IsPointerOverUI())
            return;
        // nếu chuột đang ở trên UI → không cho nhấc block
        if (BlockVisual.blockIce.IsActive)
        {
            transform.localScale = Vector3.one;
            ClickLock.Kill();
            ClickLock.Append(transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.2f));
            ClickLock.Append(transform.DOScale(Vector3.one, 0.2f));
            AudioManager.Instance.PlayOneShot("Rockblock", 1f);
            return;
        }
        if (LevelManager.Instance.BoosterHammerUsed)
        {
            CustomeEventSystem.Instance.UserBoosterHammer(gameObject);
            return;
        }
        if (!IsMove) return;
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();
        // bắt đầu game nếu có lượt kéo
        LevelManager.Instance.StartPlay();
    }

    void OnMouseUp()
    {
        if (BlockVisual.blockIce.IsActive) return;
        // Ngăn kéo khi chuột đang trên UI
        if (UIBlockChecker.IsPointerOverUI())
            return;
        if (LevelManager.Instance.BoosterHammerUsed || !IsMove)
        {
            transform.position = SnapToGrid(transform.position);
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            isGragging = false;
            return;
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        isGragging = false;
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
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
        if (BlockVisual.blockIce.IsActive) return;
        // Ngăn kéo khi chuột đang trên UI
        if (UIBlockChecker.IsPointerOverUI())
            return;
        if (!IsMove || LevelManager.Instance.BoosterHammerUsed)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
            rb.linearDamping = 0;
            rb.angularDamping = 0;
            // nếu kéo mà gặp gatepipe thì snap lại luôn
            transform.position = SnapToGrid(transform.position);
            return;
        }
        isGragging = true;
        Vector3 target = GetMouseWorldPos() + offset;

        if (IsBlockedDirection(target))
        {
            return;
        }
        //RotateMove(target);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.linearDamping = 0;
        rb.angularDamping = 0;
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, target, ref smoothVelocity, smoothTime);
        rb.MovePosition(smoothPos);
    }

    private void RotateMove(Vector3 target)
    {
        float tiltX = 0f;
        float tiltY = 0f;

        // Xoay theo trục ngang
        if (target.x > transform.position.x)
            tiltY = -5f;
        else if (target.x < transform.position.x)
            tiltY = 5f;

        // Xoay theo trục dọc
        if (target.y > transform.position.y)
            tiltX = 5f;
        else if (target.y < transform.position.y)
            tiltX = -5f;

        // Kill tween xoay cũ
        if (rotateTween != null && rotateTween.IsActive())
            rotateTween.Kill();

        // Tween xoay mới
        rotateTween = transform
            .DORotate(new Vector3(tiltX, tiltY, transform.rotation.eulerAngles.z), 0.1f)
            .OnComplete(() =>
            {
                // Kill tween cũ lần nữa trước khi reset xoay
                if (rotateTween != null && rotateTween.IsActive())
                    rotateTween.Kill();

                rotateTween = transform.DORotate(
                    new Vector3(0, 0, transform.rotation.eulerAngles.z),
                    0.1f
                );
            });
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
        if (BlockVisual.blockTypeVariant.blockMoveDir.BlockMoveDirection == BlockMoveDirection.HORIZONTAL)
        {
            mousePoint.y = 0;
        }
        else if (BlockVisual.blockTypeVariant.blockMoveDir.BlockMoveDirection == BlockMoveDirection.VERTICAL)
        {
            mousePoint.x = 0;
        }
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
            pos.z = -0.1f;
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
            Vector3 pos = SnapToGrid(transform.position);
            transform.position = SnapToPipe(pos, waterPipe);
            yield return new WaitForSeconds(0.15f);
            yield return StartCoroutine(FillPipeAndBlock(waterPipe));
            if (currentCapacity >= maxCapacity) yield break;
            // thả di chuyển ra khi đã fill song
            IsMove = true;
            // mở fill ra để được phép fill nhưng cái tiếp theo
            isFill = false;
        }
    }

    protected virtual Vector3 SnapToPipe(Vector3 pos, WaterPipe waterPipe)
    {
        return Vector3.zero;
    }

    private IEnumerator FillPipeAndBlock(WaterPipe waterPipe)
    {
        //play sound
        AudioManager.Instance.PlayOneShot("WaterPOURvar1S1", 1);
        AudioManager.Instance.PlayOneShot("WaterPOURvar2S1", 1);
        StartCoroutine(waterPipe.FillColorWater(SetHeightWaterFall(waterPipe.DirectionPipe, waterPipe.transform.position), maxCapacity - currentCapacity, (value) =>
        {
            Debug.Log(value);
        }));
        yield return StartCoroutine(ProcessFillWaterBlock(waterPipe));
    }

    protected virtual float SetHeightWaterFall(DirectionPipe directionPipe, Vector3 pipeTransform)
    {
        return 1;
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
            yield return StartCoroutine(BlockVisual.blockTypeVariant.FillWater(addCapacity * 1.0f / maxCapacity));
        }
        else
        {
            int addCapacity = currentCapacity + remainingCapacity;
            currentCapacity += remainingCapacity;
            waterPipe.WaterTypeCounters[0].count -= remainingCapacity;
            PlayParticleBlock();
            yield return StartCoroutine(BlockVisual.blockTypeVariant.FillWater(addCapacity * 1.0f / maxCapacity));
        }

        waterPipe.UpdateListWaterTypeCounter();
        StartCoroutine(CheckDoneFilling());

    }

    // check filling
    protected virtual IEnumerator CheckDoneFilling()
    {
        if (currentCapacity < maxCapacity) yield break;
        transform.position -= new Vector3(0, 0, 0.5f);
        AudioManager.Instance.PlayOneShot("ClearBlock", 1f);
        LevelManager.Instance.boardCtrl.BreakIceBlock();

        SetNonClick();
        yield return new WaitForSeconds(0.2f);
        BlockVisual.blockImpactParticle.PlayParticle();
        BlockVisual.soapBubbleEmitterVariant.PlayParticle();

        int dir = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        float distance = 25f;

        Vector3 start = transform.position;
        Vector3 up1 = start + new Vector3(0, 0f, 0f);

        Vector3 left = up1 + new Vector3(-3f, -0.5f, -0.5f);
        Vector3 right = up1 + new Vector3(3f, -0.5f, -0.5f);
        Vector3 Mid = (left + right) / 2;

        Vector3 exit = right + new Vector3(dir * distance, 0, 0);

        // Tăng độ phân giải path => cực mượt
        int resolution = 180;

        Vector3[] path = new Vector3[]
        {
        start,
        up1,
        left,
        Mid,
        right,
        exit
        };
        if (dir == -1)
        {
            path = new Vector3[]
            {
            start,
            up1,
            right,
            Mid,
            left,
            exit
            };
        }

        BlockVisual.blockTrailsParticle.PlayParticle();
        transform.DOPath(
            path,
            1.5f,
            PathType.CatmullRom,
            PathMode.Full3D,
            resolution,
            Color.white
        )
        .SetEase(Ease.InQuint);
        transform.DORotate(new Vector3(transform.rotation.eulerAngles.x, dir * -45f, transform.rotation.eulerAngles.z), 1.5f).SetEase(Ease.InQuint);
        // Tăng tốc mạnh về cuối

        transform.DOScale(new Vector3(1.85f, 1.85f, 1.85f), 1.5f)
            .SetEase(Ease.InQuint);
        yield return new WaitForSeconds(0.3f);
        BlockVisual.soapBubbleEmitterVariant.StopParticle();
        yield return new WaitForSeconds(1.7f);

        // remove block

        LevelManager.Instance.boardCtrl.BlockInstances.Remove(transform);
        if (LevelManager.Instance.boardCtrl.BlockInstances.Count == 0)
        {

            StartCoroutine(LevelManager.Instance.boardCtrl.ScaleZeroObjects());
            ScreenGamePlay screenGamePlay = UIManager.Instance.GetScreen<ScreenGamePlay>();
            screenGamePlay.HideAnimationIntro();
            yield return new WaitForSeconds(1.8f);
            StartCoroutine(GameManager.Instance.ChangeState(GameState.Win));
        }


    }





    // tắt click and collider
    protected void SetNonClick()
    {
        IsMove = false;
        for (int i = 0; i < blockParticles.Count; i++)
        {
            blockParticles[i].SetNonClick();
        }
    }


    ////////////////////////////////////////////////

    // public IEnumerator ProcessFillWaterPipe(WaterPipe waterPipe)
    // {
    //     yield return StartCoroutine(waterPipe.PipeLineCtrl.FillColor(maxCapacity - currentCapacity));
    // }

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
