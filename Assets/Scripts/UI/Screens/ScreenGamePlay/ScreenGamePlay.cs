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

    // timer
    private void InitTimerCountDown()
    {
        timer.Init(20);
        timerAndLevel.UpdateTimer(formatTime(20));

    }

    public void StartTimer()
    {
        timer.StartCountDownTimer();
        timer.OnTick += (timer) =>
        {
            UpdateTimer(timer);
        };
    }

    public void UpdateTimer(float timeLeft)
    {
        timerAndLevel.UpdateTimer(formatTime(timeLeft));
    }

    private string formatTime(float timeLeft)
    {
        TimeSpan ts = TimeSpan.FromSeconds(timeLeft);

        //Nếu bạn chỉ muốn mm:ss
        string formatted = ts.ToString(@"mm\:ss");

        // Nếu muốn hh:mm (không dùng giây)
        //string formatted = ts.ToString(@"hh\:mm");
        return formatted;
    }




}
