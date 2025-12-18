using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopUp_Hard : MonoBehaviour
{
    [SerializeField] private RectTransform rectTitle;
    [SerializeField] private RectTransform rectTop;
    [SerializeField] private RectTransform rectBot;
    [SerializeField] private CanvasGroup canvasGroup;

    void Start()
    {
        CallStart();
    }

    public void CallStart()
    {
        canvasGroup.alpha = 1;

        rectBot.anchoredPosition = new Vector3(-1311f, 216f, 0);
        rectTop.anchoredPosition = new Vector3(1311f, -216f, 0);

        DOTween.Kill(this);

        Sequence sq = DOTween.Sequence().SetId(this);

        sq.Append(rectBot.DOAnchorPosX(rectBot.anchoredPosition.x + 1300, 2f)
            .SetEase(Ease.OutQuad));

        sq.Join(rectTop.DOAnchorPosX(rectTop.anchoredPosition.x - 1300, 2f)
            .SetEase(Ease.OutQuad));

        // sq.AppendCallback(() =>
        // {
        //     rectTitle.localScale = Vector3.one;
        //     rectTitle
        //         .DOScale(1.15f, 0.18f)
        //         .SetEase(Ease.OutBack)
        //         .OnComplete(() =>
        //         {
        //             rectTitle.DOScale(1f, 0.12f).SetEase(Ease.OutQuad);
        //         });
        // });

        DOVirtual.DelayedCall(1.5f, () =>
        {
            canvasGroup.DOFade(0, 0.25f).OnComplete(OnHide);
        });
    }

    public void OnHide()
    {
        gameObject.SetActive(false);
    }
}
