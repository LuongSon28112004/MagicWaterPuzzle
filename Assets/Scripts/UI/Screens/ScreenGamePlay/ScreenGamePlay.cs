using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Button RestartButton;

    [Header("Booster")]
    [SerializeField] ListBooster listBooster;

    [Header("CountDown Timer")]
    [SerializeField] TimerCoutDown timer;
    [SerializeField] FreezeCountDown freezeTimer;
    [Header("Collect Booster")]
    [SerializeField] Transform CollectBooster;
    [SerializeField] Transform BGBlack;
    [SerializeField] CollectBooster collectBooster;
    [Header("Tut Level 1")]
    [SerializeField] TutHand tutHand;
    [Header("HardLevel")]
    [SerializeField] GameObject HardLevel;
    [Header("Key Lock")]
    [SerializeField] KeyLock keyLock;

    public TimerCoutDown Timer { get => timer; set => timer = value; }

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
        InitLevelText();
        StartCoroutine(AnimationIntro());
        AddEventListener();
        InitCountBooster();
        InitLockBooster();
        InActiveBooster();
        if (GameManager.Instance.Level == 8)
        {
            CollectBooster.gameObject.SetActive(true);
            collectBooster.setTypeCollect(TypeCollectBooster.FREEZE);
            AudioManager.Instance.PlayOneShot("BoosterAppear_1", 0.75f);
        }
        else if (GameManager.Instance.Level == 10)
        {
            CollectBooster.gameObject.SetActive(true);
            collectBooster.setTypeCollect(TypeCollectBooster.BOMB);
            AudioManager.Instance.PlayOneShot("BoosterAppear_1", 0.75f);
        }
        else if (GameManager.Instance.Level == 13)
        {
            CollectBooster.gameObject.SetActive(true);
            collectBooster.setTypeCollect(TypeCollectBooster.HAMMER);
            AudioManager.Instance.PlayOneShot("BoosterAppear_1", 0.75f);
        }

        if (GameManager.Instance.Level == 7 || GameManager.Instance.Level == 12 || GameManager.Instance.Level == 17)
        {
            HardLevel.SetActive(true);
            AudioManager.Instance.PlayVibrate();
        }

        if (GameManager.Instance.Level == 14)
        {
            keyLock.ShowIntro();
        }
    }

    public void ActiveTut()
    {
        tutHand.gameObject.SetActive(true);
    }

    public void DeactiveTut()
    {
        tutHand.gameObject.SetActive(false);
    }

    private void InitLockBooster()
    {
        if (GameManager.Instance.Level < 8)
        {
            listBooster.freezeBoosterConfig.LockBooster();
            listBooster.bombBoosterConfig.LockBooster();
            listBooster.hammerBoosterConfig.LockBooster();
        }
        else if (GameManager.Instance.Level < 10)
        {
            listBooster.freezeBoosterConfig.UnlockBooster();
            listBooster.bombBoosterConfig.LockBooster();
            listBooster.hammerBoosterConfig.LockBooster();
        }
        else if (GameManager.Instance.Level < 13)
        {
            listBooster.freezeBoosterConfig.UnlockBooster();
            listBooster.bombBoosterConfig.UnlockBooster();
            listBooster.hammerBoosterConfig.LockBooster();
        }
        else
        {
            listBooster.freezeBoosterConfig.UnlockBooster();
            listBooster.bombBoosterConfig.UnlockBooster();
            listBooster.hammerBoosterConfig.UnlockBooster();
        }

    }

    private void InitLevelText()
    {
        int level = GameManager.Instance.Level;
        timerAndLevel.SetTextLevel(level);
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
        //active Tut level 1
        if (GameManager.Instance.Level == 1)
        {
            ActiveTut();
        }
    }

    public void HideAnimationIntro()
    {
        RectTransform rectTop = top.GetComponent<RectTransform>();
        RectTransform rectBottom = bottom.GetComponent<RectTransform>();

        float valueRectTop = rectTop.anchoredPosition.y;
        float valueRectBottom = rectBottom.anchoredPosition.y;

        rectTop.DOAnchorPosY(250, 0.4f).SetEase(Ease.Linear);
        rectBottom.DOAnchorPosY(-250, 0.4f).SetEase(Ease.Linear);
    }
    private void AddEventListener()
    {
        //booster
        listBooster.FreezeButton.onClick.RemoveAllListeners();
        listBooster.BombButton.onClick.RemoveAllListeners();
        listBooster.HammerButton.onClick.RemoveAllListeners();
        listBooster.FreezeButton.onClick.AddListener(FreezeClick);
        listBooster.BombButton.onClick.AddListener(BombClick);
        listBooster.HammerButton.onClick.AddListener(HammerClick);
        //pause
        PauseButton.onClick.AddListener(PauseClick);
        //Back
        BackButton.onClick.AddListener(BackClick);
        // Restart
        RestartButton.onClick.AddListener(RestartClick);
    }

    private void RestartClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        StartCoroutine(ShowPopupRestart());
    }

    private IEnumerator ShowPopupRestart()
    {
        yield return new WaitForSeconds(0.2f);
        var ui = UIManager.Instance.ShowPopup<PopupExitLevel>(null);
        ui.InitMode(ModeShowPopupExit.RESTART);
    }

    private void BackClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        StartCoroutine(ShowPopupBack());
    }

    private IEnumerator ShowPopupBack()
    {
        yield return new WaitForSeconds(0.2f);
        var ui = UIManager.Instance.ShowPopup<PopupExitLevel>(null);
        ui.InitMode(ModeShowPopupExit.MENU);
    }

    private void PauseClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        StartCoroutine(ShowPopupPause());
    }

    private IEnumerator ShowPopupPause()
    {
        yield return new WaitForSeconds(0.2f);
        UIManager.Instance.ShowPopup<PopupSetting>(null);
    }

    public void HammerClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        if (UserData.listBoosterCounters[2].count == 0 && GameManager.Instance.Level != 8 && GameManager.Instance.Level != 10 && GameManager.Instance.Level != 13)
        {
            var ui = UIManager.Instance.ShowPopup<PopupBuyBooster>(null);
            ui.ChangeTypeBooster(TypeCollectBooster.HAMMER);
            return;
        }
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
        if (GameManager.Instance.Level != 8 && GameManager.Instance.Level != 10 && GameManager.Instance.Level != 13)
        {
            UserData.listBoosterCounters[2].count -= 1;
            if (UserData.listBoosterCounters[2].count == 0)
            {
                List<BoosterCounter> boosterConfigs = UserData.listBoosterCounters;
                listBooster.InitCountBooster(boosterConfigs[0].count > 0 ? true : false, boosterConfigs[1].count > 0 ? true : false, false, new List<int> { boosterConfigs[0].count, boosterConfigs[1].count, 0 });
            }
            SaveDataManager.Save();
            listBooster.UpdateText();
        }
    }


    public void BombClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        if (UserData.listBoosterCounters[1].count == 0 && GameManager.Instance.Level != 8 && GameManager.Instance.Level != 10 && GameManager.Instance.Level != 13)
        {
            var ui = UIManager.Instance.ShowPopup<PopupBuyBooster>(null);
            ui.ChangeTypeBooster(TypeCollectBooster.BOMB);
            return;
        }
        if (BGBlack.gameObject.activeSelf)
        {
            BGBlack.gameObject.SetActive(false);
            listBooster.bombBoosterConfig.StopTutorial();
            LevelManager.Instance.StartPlay();

        }
        if (LevelManager.Instance.boardCtrl.BlockInstances.Count == 0) return;
        UIManager.Instance.ShowPopup<PopupBombBooster>(null);
        HideButton();
        if (GameManager.Instance.Level != 8 && GameManager.Instance.Level != 10 && GameManager.Instance.Level != 13)
        {
            UserData.listBoosterCounters[1].count -= 1;
            if (UserData.listBoosterCounters[1].count == 0)
            {
                List<BoosterCounter> boosterConfigs = UserData.listBoosterCounters;
                listBooster.InitCountBooster(boosterConfigs[0].count > 0 ? true : false, false, boosterConfigs[2].count > 0 ? true : false, new List<int> { boosterConfigs[0].count, 0, boosterConfigs[2].count });
            }
            SaveDataManager.Save();
            listBooster.UpdateText();
        }

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



    public void FreezeClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        if (UserData.listBoosterCounters[0].count == 0 && GameManager.Instance.Level != 8 && GameManager.Instance.Level != 10 && GameManager.Instance.Level != 13)
        {
            var ui = UIManager.Instance.ShowPopup<PopupBuyBooster>(null);
            ui.ChangeTypeBooster(TypeCollectBooster.FREEZE);
            return;
        }
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

        if (GameManager.Instance.Level != 8 && GameManager.Instance.Level != 10 && GameManager.Instance.Level != 13)
        {
            UserData.listBoosterCounters[0].count -= 1;
            if (UserData.listBoosterCounters[0].count == 0)
            {
                List<BoosterCounter> boosterConfigs = UserData.listBoosterCounters;
                listBooster.InitCountBooster(false, boosterConfigs[1].count > 0 ? true : false, boosterConfigs[2].count > 0 ? true : false, new List<int> { 0, boosterConfigs[1].count, boosterConfigs[2].count });
            }
            SaveDataManager.Save();
            listBooster.UpdateText();
        }

    }

    public void UpdateFreezeTimer(float timeLeft, float timerTimeleft)
    {
        timerAndLevel.UpdateFreezeTimer(timeLeft, timerTimeleft);
    }

    // timer
    public void InitTimerCountDown(int timerStart = 180)
    {
        timer.Init(timerStart);
        timerAndLevel.UpdateTimer(timerStart);

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

    public void InitCountBooster()
    {
        List<BoosterCounter> boosterConfigs = UserData.listBoosterCounters;
        List<int> counts = new List<int> { boosterConfigs[0].count, boosterConfigs[1].count, boosterConfigs[2].count };
        listBooster.InitCountBooster(boosterConfigs[0].count > 0 ? true : false, boosterConfigs[1].count > 0 ? true : false, boosterConfigs[2].count > 0 ? true : false, counts);
    }

    private void InActiveBooster()
    {
        listBooster.InActiveAllBooster();
    }

    private void ActiveBooster()
    {
        if (GameManager.Instance.Level >= 8 && GameManager.Instance.Level < 10)
        {
            listBooster.freezeBoosterConfig.Active();
        }
        else if (GameManager.Instance.Level >= 10 && GameManager.Instance.Level < 13)
        {
            listBooster.freezeBoosterConfig.Active();
            listBooster.bombBoosterConfig.Active();
        }
        else if (GameManager.Instance.Level >= 13)
        {
            listBooster.freezeBoosterConfig.Active();
            listBooster.bombBoosterConfig.Active();
            listBooster.hammerBoosterConfig.Active();
        }
    }

}
