using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupOutOfTime : PopupUI
{
    [SerializeField] Button buttonExit;

    private void Start()
    {
        AddEventListener();
    }

    private void AddEventListener()
    {
        buttonExit.onClick.AddListener(ShowPopupLoseGame);
    }

    private void ShowPopupLoseGame()
    {
        Hide();
        UIManager.Instance.ShowPopup<PopupLose>(null);
    }
}
