using System;
using UnityEngine;
using UnityEngine.UI;

public enum ModeShowPopupExit
{
    MENU,
    RESTART
}

public class PopupExitLevel : PopupUI
{
    [SerializeField] ModeShowPopupExit ModeShowPopupExit;
    [SerializeField] Button CloseButton;
    [SerializeField] Button GiveUpButton;

    private void Start()
    {
        AddEventListener();
    }

    private void OnEnable()
    {
        AddEventListener();
    }

    public void InitMode(ModeShowPopupExit modeShowPopupExit)
    {
        this.ModeShowPopupExit = modeShowPopupExit;
    }

    private void AddEventListener()
    {
        CloseButton.onClick.RemoveAllListeners();
        GiveUpButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(CloseClick);
        GiveUpButton.onClick.AddListener(GiveUpClick);
    }

    private void GiveUpClick()
    {
        var ui = UIManager.Instance.ShowPopup<PopupQuitBreak>(null);
        if (ModeShowPopupExit == ModeShowPopupExit.MENU)
        {
            ui.StartModeQuitBreak(ModeOutgame.BACK_TO_MENU);
        }
        else if (ModeShowPopupExit == ModeShowPopupExit.RESTART)
        {
            ui.StartModeQuitBreak(ModeOutgame.RESTART);
        }
        int CurrentHearts = PlayerPrefs.GetInt("Hearts", 0);
        if (CurrentHearts > 0)
        {
            CurrentHearts -= 1;
            PlayerPrefs.SetInt("Hearts", CurrentHearts);
        }
    }

    private void CloseClick()
    {
        Hide();
    }
}
