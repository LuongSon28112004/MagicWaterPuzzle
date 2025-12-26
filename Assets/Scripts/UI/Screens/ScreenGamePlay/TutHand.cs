using DG.Tweening;
using UnityEngine;

public class TutHand : MonoBehaviour
{
    private Tween handTween;

    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();

        handTween = rect
            .DOAnchorPosY(400, 1.25f)   // đi lên 400
            .SetRelative(true)       // tính tương đối so với vị trí hiện tại
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.InOutSine);
    }

    void OnDisable()
    {
        handTween?.Kill();
    }
}
