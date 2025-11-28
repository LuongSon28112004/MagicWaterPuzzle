using System;
using System.Collections;
using DG.Tweening;
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

    [Header("CountDown Timer")]
    [SerializeField] TimerCoutDown timer;
    [SerializeField] FreezeCountDown freezeTimer;

    private void OnEnable()
    {
        CustomeEventSystem.Instance.StartPlayAction += StartTimer;
    }

    private void OnDisable()
    {
        CustomeEventSystem.Instance.StartPlayAction -= StartTimer;
    }



    private void Start()
    {
        StartCoroutine(AnimationIntro());
        AddEventListener();
        InitTimerCountDown();
        InActiveBooster();
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
        timer.PauseCountDownTimer();
        freezeTimer.Init(20);
        freezeTimer.StartCountDownTimer();
        freezeTimer.OnTick += (timerr) =>
        {
            UpdateFreezeTimer(timerr, timer.TimeLeft);
        };

    }

    public void UpdateFreezeTimer(float timeLeft, float timerTimeleft)
    {
        timerAndLevel.UpdateFreezeTimer(timeLeft, timerTimeleft);
    }

    // timer
    private void InitTimerCountDown()
    {
        timer.Init(180);
        timerAndLevel.UpdateTimer(180f);

    }

    public void ResumeTimer()
    {
        timer.ResumeCountDownTimer();
        timerAndLevel.EndFreeze();
        freeze.Hide();
    }

    public void StartTimer()
    {
        timer.StartCountDownTimer();
        timer.OnTick += (timer) =>
        {
            UpdateTimer(timer);
        };
        ActiveBooster();
    }

    public void UpdateTimer(float timeLeft)
    {
        timerAndLevel.UpdateTimer(timeLeft);
    }

    private void InActiveBooster()
    {
        listBooster.InActiveAllBooster();
    }

    private void ActiveBooster()
    {
        listBooster.ActiveAllBooster();
    }

}
