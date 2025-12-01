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
        listBooster.BombButton.onClick.AddListener(BombClick);
    }

    private void BombClick()
    {
        StartCoroutine(BombClickCoroutine());

    }

    private IEnumerator BombClickCoroutine()
    {
        AudioManager.Instance.PlayOneShot("Explo", 1f);
        GameObject ParticleBombHammerSrc = Resources.Load<GameObject>("Particles/BlockBombHammerBreakEffect");
        GameObject block = LevelManager.Instance.findObjectNearOrigin();

        // Instantiate tại vị trí block nhưng không parent
        GameObject ParticleBombHammer = Instantiate(
            ParticleBombHammerSrc,
            block.transform.position,
            Quaternion.identity
        );

        // Play particle
        ParticleBombHammerBreakEffect particleBombHammerBreakEffect =
            ParticleBombHammer.GetComponent<ParticleBombHammerBreakEffect>();
        particleBombHammerBreakEffect.PlayParticle();

        // Xóa block sau khi particle chạy
        block.SetActive(false);
        LevelManager.Instance.boardCtrl.BlockInstances.Remove(block.transform);

        yield return new WaitForSeconds(0.6f);
        if (LevelManager.Instance.boardCtrl.BlockInstances.Count == 0)
        {
            //Show Popup Win
            AudioManager.Instance.PlayOneShot("Win", 1f);
            UIManager.Instance.ShowPopup<PopupWin>(null);
        }
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
