using System;
using System.Collections;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class ScreenGamePlay : ScreenUI
{
    [SerializeField] GameObject top;
    [SerializeField] GameObject bottom;
    [Header("Timer and Level")]
    [SerializeField] TimerAndLevel timerAndLevel;
    [Header("BG Freeze")]
    [SerializeField] Freeze freeze;

    [Header("Booster")]
    [SerializeField] ListBooster listBooster;



    private void Start()
    {
        StartCoroutine(AnimationIntro());
        AddEventListener();
    }


    private IEnumerator AnimationIntro()
    {
        RectTransform rectTop = top.GetComponent<RectTransform>();
        RectTransform rectBottom = bottom.GetComponent<RectTransform>();

        float valueRectTop = rectTop.anchoredPosition.y;
        float valueRectBottom = rectBottom.anchoredPosition.y;

        rectTop.anchoredPosition = new Vector3(0, 250, 0);
        rectBottom.anchoredPosition = new Vector3(0, -250, 0);
        yield return new WaitForSeconds(0.6f);
        rectTop.DOAnchorPosY(valueRectTop, 0.4f).SetEase(Ease.Linear);
        rectBottom.DOAnchorPosY(valueRectBottom, 0.4f).SetEase(Ease.Linear);

    }
    private void AddEventListener()
    {
        listBooster.FreezeButton.onClick.AddListener(FreezeClick);
    }

    private void FreezeClick()
    {
        freeze.Show();
        timerAndLevel.StartFreeze();
    }
}
