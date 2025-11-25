using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenHome : ScreenUI
{
    [SerializeField] private Button buttonPlay;
    [SerializeField] private ScrollRect scrollRect;


    public void Start()
    {
        buttonPlay.onClick.AddListener(OnClickPlay);
        //scrollRect.onValueChanged.AddListener(OnScroll);
        ScrollToBottom();
    }

    private void OnScroll(Vector2 value)
    {
        ScrollToBottom();
    }

    private void OnClickPlay()
    {
        StartCoroutine(PlayCoroutine());
    }

    private IEnumerator PlayCoroutine()
    {
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
