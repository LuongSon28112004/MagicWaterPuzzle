using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupBuyBooster : PopupUI
{
    [Header("Popup Bomb Booster Settings")]
    [SerializeField] Button buttonExit;


    private void Start()
    {
        AddEventListener();
    }

    private void AddEventListener()
    {
        buttonExit.onClick.AddListener(ExitClick);
    }

    private void ExitClick()
    {
        Hide();
    }
}
