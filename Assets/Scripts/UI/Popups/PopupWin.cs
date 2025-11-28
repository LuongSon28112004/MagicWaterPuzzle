
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupWin : PopupUI
{
    [SerializeField] Image AnimationWin;
    [SerializeField] Button Claim;
    [SerializeField] Button ClaimX2;

    private void Awake()
    {
        AddAnimationWin();
        AddEventListener();
    }

    private void AddEventListener()
    {
        Claim.onClick.AddListener(ClaimClick);
    }

    private void ClaimClick()
    {
        GameManager.Instance.BackToMenu();
    }

    private void AddAnimationWin()
    {
        RectTransform rect = AnimationWin.GetComponent<RectTransform>();

        // Quay thuận chiều kim đồng hồ từ 0 đến 360 và loop mãi mãi
        rect
            .DORotate(new Vector3(0, 0, -360), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)     // xoay đều
            .SetLoops(-1);            // lặp vô hạn
    }

}
