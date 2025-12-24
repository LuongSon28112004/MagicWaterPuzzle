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
    private Sequence clickLock;

    void OnMouseDown()
    {
        if (UIBlockChecker.IsPointerOverUI())
            return;

        if (BlockVisual.blockIce.IsActive)
        {
            transform.localScale = Vector3.one;

            // Kill an toàn
            if (clickLock != null && clickLock.IsActive())
                clickLock.Kill();

            // TẠO sequence mới
            clickLock = DOTween.Sequence();
            clickLock.Append(transform.DOScale(0.8f, 0.2f));
            clickLock.Append(transform.DOScale(1f, 0.2f));

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

        LevelManager.Instance.StartPlay();
    }

    void OnDisable()
    {
        clickLock?.Kill();
    }



    void OnMouseUp()
    {
        if (BlockVisual.blockIce.IsActive) return;

        // Ngăn thả khi đang trên UI
        if (UIBlockChecker.IsPointerOverUI())
            return;


        //reset blockNormal
        blockNormal = Vector2.zero;


        // kết thúc kéo
        isGragging = false;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;

        // Snap vị trí
        transform.position = SnapToGrid(transform.position);


        ResetZ();
        //Reset rotate MỘT LẦN DUY NHẤT
        if (rotateTween != null && rotateTween.IsActive())
            rotateTween.Kill();

        transform.DORotate(
            new Vector3(0, 0, transform.eulerAngles.z),
            0.15f
        ).SetEase(Ease.OutQuad);

        if (LevelManager.Instance.BoosterHammerUsed || !IsMove)
            return;

        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
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
        if (isFill)
        {
            ResetZ();
            //Reset rotate MỘT LẦN DUY NHẤT
            if (rotateTween != null && rotateTween.IsActive())
                rotateTween.Kill();

            transform.DORotate(
                new Vector3(0, 0, transform.eulerAngles.z),
                0.15f
            ).SetEase(Ease.OutQuad);
        }
        if (!IsMove || LevelManager.Instance.BoosterHammerUsed)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
            rb.linearDamping = 0;
            rb.angularDamping = 0;
            // snap pos
            transform.position = SnapToGrid(transform.position);
            return;
        }
        isGragging = true;
        Vector3 target = GetMouseWorldPos() + offset;

        if (IsBlockedDirection(target))
        {
            return;
        }
        RotateMove(target);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.linearDamping = 0;
        rb.angularDamping = 0;
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, target, ref smoothVelocity, smoothTime);
        rb.MovePosition(smoothPos);
    }

    private Tweener zTween;
    private float liftZ = -1f;
    private float normalZ = 0f;

    private void LiftZ()
    {
        if (Mathf.Approximately(transform.position.z, liftZ)) return;

        zTween?.Kill();
        zTween = transform.DOMoveZ(liftZ, 0.08f)
            .SetEase(Ease.OutQuad);
    }

    private void ResetZ()
    {
        zTween?.Kill();
        zTween = transform.DOMoveZ(normalZ, 0.12f)
            .SetEase(Ease.OutQuad);
    }



    private Tweener rotateTween;
    private Vector3 lastPos;
    private float maxTilt = 10f;

    private void RotateMove(Vector3 target)
    {
        Vector3 delta = target - lastPos;
        lastPos = target;

        float tiltX = Mathf.Clamp(delta.y * 80f, -maxTilt, maxTilt);
        float tiltY = Mathf.Clamp(-delta.x * 80f, -maxTilt, maxTilt);

        //LiftZ(); //nâng block khi nghiêng

        if (rotateTween == null || !rotateTween.IsActive())
        {
            rotateTween = transform
                .DORotate(new Vector3(tiltX, tiltY, transform.eulerAngles.z), 0.12f)
                .SetEase(Ease.OutQuad);

        }
        else
        {
            rotateTween.ChangeEndValue(
                new Vector3(tiltX, tiltY, transform.eulerAngles.z),
                true
            );
        }
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

        WaterPipe waterPipe = other.GetComponentInParent<WaterPipe>();
        if (waterPipe == null)
        {
            return;
        }

        if (waterPipe.PipeKeyLock.IsLocked)
        {
            return;
        }

        ProcessTriggerMove(other);
        StartCoroutine(ProcessTriggerWaterPipe(other));
    }

    void ProcessTriggerMove(Collider2D other)
    {
        if ((other.GetComponentInParent<BaseBlock>() != null) && isGragging)
        {
            Vector3 pos = transform.position;
            pos.z = -1f;
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

    private void CheckKeyLock()
    {
        GameObject PipeLockObj = LevelManager.Instance.findObjectHasKeyColor(blockVisual.blockTypeVariant.blockKeyLock.ColorKey);
        if (PipeLockObj != null)
        {
            blockVisual.blockTypeVariant.blockKeyLock.UnlockKeyLock(PipeLockObj);
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
            StartCoroutine(PlayParticleBlock());
            yield return StartCoroutine(BlockVisual.blockTypeVariant.FillWater(addCapacity * 1.0f / maxCapacity));
        }
        else
        {
            int addCapacity = currentCapacity + remainingCapacity;
            currentCapacity += remainingCapacity;
            waterPipe.WaterTypeCounters[0].count -= remainingCapacity;
            StartCoroutine(PlayParticleBlock());
            yield return StartCoroutine(BlockVisual.blockTypeVariant.FillWater(addCapacity * 1.0f / maxCapacity));
        }
        CheckKeyLock();

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
        yield return new WaitForSeconds(0.1f);
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
            yield return new WaitForSeconds(0.8f);
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

    protected virtual IEnumerator PlayParticleBlock()
    {
        yield break;
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

        // blockNormal = Vector2.zero;
    }

    protected bool CheckSameColor(WaterTypeColor waterTypeColor, BlockColor blockColorVisual)
    {
        string a = waterTypeColor.ToString();
        string b = blockColorVisual.ToString();
        return a.Contains(b);
    }
}
