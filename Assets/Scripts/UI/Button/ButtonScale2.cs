using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class ButtonScaler2 : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Scale")]
    public Vector3 startScale = Vector3.one;
    public Vector3 endScale = new Vector3(0.9f, 0.9f, 1f);

    [Header("Target")]
    [SerializeField] private Transform targetTF;

    [Header("Events")]
    [SerializeField] protected UnityEvent eventOnPointDown;
    [SerializeField] protected UnityEvent eventOnPointUp;

    //[Header("Feedback")]
    // [SerializeField] private bool isActiveSound = true;
    // [SerializeField] private bool ignoreVib = false;

    private bool isPointerDown;
    private bool isPointerInside;

    private void Awake()
    {
        if (targetTF == null)
            targetTF = transform;

        targetTF.localScale = startScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        isPointerInside = true;

        targetTF.DOKill();
        targetTF
            .DOScale(endScale, 0.1f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);

        eventOnPointDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPointerDown) return;

        isPointerDown = false;

        targetTF.DOKill();
        targetTF
            .DOScale(startScale, 0.1f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);

        eventOnPointUp?.Invoke();

        // Không trigger click nếu đang drag hoặc trượt ra ngoài
        if (eventData.dragging || !isPointerInside)
            return;

        // if (isActiveSound)
        //     AudioManager.Instance.PlayOneShot("");

        // if (!ignoreVib)
        //     AudioManager.Instance.PlayVibrate();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;

        // Nếu kéo ra ngoài khi đang giữ -> scale lại
        if (isPointerDown)
        {
            targetTF.DOKill();
            targetTF
                .DOScale(startScale, 0.1f)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }
    }

    private void OnDisable()
    {
        targetTF.DOKill();
        targetTF.localScale = startScale;

        isPointerDown = false;
        isPointerInside = false;
    }
}
