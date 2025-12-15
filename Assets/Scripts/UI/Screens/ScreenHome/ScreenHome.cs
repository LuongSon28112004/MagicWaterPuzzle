using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenHome : ScreenUI
{
    [Header("Screen Home Component")]
    [SerializeField] private Button buttonPlay;
    [SerializeField] private ScrollRect scrollRect;
    [Header("List Level Text")]
    [SerializeField] private List<TextMeshProUGUI> listLevelText;

    [Header("Shiny Effect Button")]
    [SerializeField] ShinyEffectForUGUI shinyEffectForUGUI_1;
    [SerializeField] ShinyEffectForUGUI shinyEffectForUGUI_2;

    private void OnEnable()
    {
        StartCoroutine(ShinyEffectButtonPlay());
    }
    public void Start()
    {
        buttonPlay.onClick.AddListener(OnClickPlay);
        //scrollRect.onValueChanged.AddListener(OnScroll);
        ScrollToBottom();
        StartCoroutine(ShinyEffectButtonPlay());
        LoadListLevelText();
    }

    private void LoadListLevelText()
    {
        int currentLevel = UserData.level;
        for (int i = 0; i < listLevelText.Count; i++)
        {
            listLevelText[i].text = currentLevel.ToString();
            currentLevel++;
        }
    }

    private IEnumerator ShinyEffectButtonPlay()
    {
        while (true)
        {
            shinyEffectForUGUI_1.Play(3f);
            yield return new WaitForSeconds(0.28f);
            shinyEffectForUGUI_2.Play(3f);
            yield return new WaitForSeconds(4f);
        }
    }

    private void OnScroll(Vector2 value)
    {
        ScrollToBottom();
    }

    private void OnClickPlay()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        StartCoroutine(PlayCoroutine());
    }

    private IEnumerator PlayCoroutine()
    {
        int currentHearts = PlayerPrefs.GetInt("Hearts", 0);
        if (currentHearts <= 0)
        {
            UIManager.Instance.ShowPopup<PopupRefillYourLives>(null);
            yield break;
        }
        PopupLoading popupLoading = UIManager.Instance.GetPopup<PopupLoading>();
        if (popupLoading == null)
        {
            UIManager.Instance.ShowPopup<PopupLoading>(null);
        }
        else
        {
            popupLoading.LoadingSuccess = false;
            popupLoading.Mode = Mode.LoadingLevel;
            UIManager.Instance.ShowPopup<PopupLoading>(null);
        }
        while (popupLoading != null && !popupLoading.LoadingSuccess)
        {
            yield return null;
        }
        //UIManager.Instance.HideAllPopup();
        GameManager.Instance.StartGame();
    }

    public void ScrollToBottom(float duration = 0.25f)
    {
        StartCoroutine(ScrollBottomCoroutine(duration));
    }

    private IEnumerator ScrollBottomCoroutine(float duration)
    {
        yield return null; // chờ layout update

        scrollRect.DOVerticalNormalizedPos(0f, duration);
    }
}
