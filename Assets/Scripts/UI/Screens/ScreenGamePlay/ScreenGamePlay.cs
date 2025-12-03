using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGamePlay : ScreenUI
{
    [SerializeField] GameObject top;
    [SerializeField] GameObject bottom;
    [Header("Component")]
    [Header("Timer and Level")]
    [SerializeField] TimerAndLevel timerAndLevel;
    [Header("BG Freeze")]
    [SerializeField] Freeze freeze;
    [Header("Menu")]
    [SerializeField] Button BackButton;
    [SerializeField] Button PauseButton;

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

        listBooster.FreezeButton.onClick.RemoveAllListeners();
        listBooster.BombButton.onClick.RemoveAllListeners();
        listBooster.HammerButton.onClick.RemoveAllListeners();
        listBooster.FreezeButton.onClick.AddListener(FreezeClick);
        listBooster.BombButton.onClick.AddListener(BombClick);
        listBooster.HammerButton.onClick.AddListener(HammerClick);
    }

    private void HammerClick()
    {
        if (LevelManager.Instance.boardCtrl.BlockInstances.Count == 0) return;
        LevelManager.Instance.BoosterHammerUsed = true;
        UIManager.Instance.ShowPopup<PopupHammerBooster>(null);
        HideButton();
    }


    private void BombClick()
    {
        if (LevelManager.Instance.boardCtrl.BlockInstances.Count == 0) return;
        UIManager.Instance.ShowPopup<PopupBombBooster>(null);
        // PopupBombBooster popupBombBooster = UIManager.Instance.GetPopup<PopupBombBooster>();
        // if (popupBombBooster == null)
        // {
        //     UIManager.Instance.ShowPopup<PopupBombBooster>(null);
        // }
        // else
        // {
        //     StartCoroutine(popupBombBooster.PlayBombBooster());
        // }
        HideButton();
    }
    private void HideButton()
    {
        listBooster.HideBooster();
        BackButton.transform.DOScale(Vector3.zero, 0.4f);
        BackButton.interactable = false;
        PauseButton.transform.DOScale(Vector3.zero, 0.4f);
        PauseButton.interactable = false;
        AddEventListener();
    }

    public void ShowButton()
    {
        listBooster.ShowBooster();
        BackButton.transform.DOScale(Vector3.one, 0.4f);
        BackButton.interactable = true;
        PauseButton.transform.DOScale(Vector3.one, 0.4f);
        PauseButton.interactable = true;
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
