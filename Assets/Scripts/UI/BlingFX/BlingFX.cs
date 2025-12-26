using DG.Tweening;
using UnityEngine;

public class BlingFX : MonoBehaviour
{
    [Header("Scale Config")]
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float randomDelayRange = 0.5f;

    private Tween blingTween;

    void Start()
    {
        float delay = Random.Range(0f, randomDelayRange);
        Invoke(nameof(PlayBling), delay);
    }

    public void PlayBling()
    {
        transform.localScale = Vector3.one * minScale;

        blingTween = transform
            .DOScale(maxScale, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        blingTween?.Kill();
    }
}
