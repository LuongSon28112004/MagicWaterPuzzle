using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupLose : PopupUI
{
    [SerializeField] Button buttonRetry;

    private void Start()
    {
        AddEventListener();
    }

    private void AddEventListener()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        buttonRetry.onClick.AddListener(BackToMenu);
    }

    private void BackToMenu()
    {
        GameManager.Instance.BackToMenu();
    }
}
