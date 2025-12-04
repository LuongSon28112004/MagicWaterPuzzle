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
    [Header("Collect Booster")]
    [SerializeField] Transform CollectBooster;
    [SerializeField] Transform BGBlack;
    [SerializeField] CollectBooster collectBooster;
    private void OnEnable()
    {
        CustomeEventSystem.Instance.StartPlayAction += StartTimer;
        collectBooster.DoneAction += ShowTut;
    }

    private void OnDisable()
    {
        CustomeEventSystem.Instance.StartPlayAction -= StartTimer;
        collectBooster.DoneAction -= ShowTut;
    }

    private void ShowTut(bool resuit)
    {
        if (resuit)
        {
            if (collectBooster.TypeCollectBooster == TypeCollectBooster.FREEZE)
            {
                listBooster.freezeBoosterConfig.PlayTutorial();
            }
            else if (collectBooster.TypeCollectBooster == TypeCollectBooster.BOMB)
            {
                listBooster.bombBoosterConfig.PlayTutorial();
            }
            else if (collectBooster.TypeCollectBooster == TypeCollectBooster.HAMMER)
            {
                listBooster.hammerBoosterConfig.PlayTutorial();
            }
            BGBlack.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        StartCoroutine(AnimationIntro());
        AddEventListener();
        InitTimerCountDown();
        InActiveBooster();
        if (GameManager.Instance.Level == 1)
        {
            CollectBooster.gameObject.SetActive(true);
            collectBooster.setTypeCollect(TypeCollectBooster.FREEZE);
        }
        else if (GameManager.Instance.Level == 2)
        {
            CollectBooster.gameObject.SetActive(true);
            collectBooster.setTypeCollect(TypeCollectBooster.BOMB);
        }
        else if (GameManager.Instance.Level == 3)
        {
            CollectBooster.gameObject.SetActive(true);
            collectBooster.setTypeCollect(TypeCollectBooster.HAMMER);
        }
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
        if (BGBlack.gameObject.activeSelf)
        {
            BGBlack.gameObject.SetActive(false);
            listBooster.hammerBoosterConfig.StopTutorial();
            LevelManager.Instance.StartPlay();
        }
        if (LevelManager.Instance.boardCtrl.BlockInstances.Count == 0) return;
        LevelManager.Instance.BoosterHammerUsed = true;
        UIManager.Instance.ShowPopup<PopupHammerBooster>(null);
        HideButton();
    }


    private void BombClick()
    {
        if (BGBlack.gameObject.activeSelf)
        {
            BGBlack.gameObject.SetActive(false);
            listBooster.bombBoosterConfig.StopTutorial();
            LevelManager.Instance.StartPlay();

        }
        if (LevelManager.Instance.boardCtrl.BlockInstances.Count == 0) return;
        UIManager.Instance.ShowPopup<PopupBombBooster>(null);
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
        if (BGBlack.gameObject.activeSelf)
        {
            BGBlack.gameObject.SetActive(false);
            listBooster.freezeBoosterConfig.StopTutorial();
            LevelManager.Instance.StartPlay();

        }
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
