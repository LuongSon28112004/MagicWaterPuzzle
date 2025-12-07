using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RotateFX : MonoBehaviour
{
    [SerializeField] private Image iconFX;

    void Start()
    {
        iconFX = GetComponent<Image>();
        iconFX.rectTransform
                    .DORotate(new Vector3(0, 0, -360), 20f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.Linear);

    }
}
