using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : PopupUI
{
    [SerializeField] Button buttonClose;

    private void Start()
    {
        AddEventListener();
    }

    private void AddEventListener()
    {
        buttonClose.onClick.AddListener(CloseClick);
    }

    private void CloseClick()
    {
        Hide();
    }
}
