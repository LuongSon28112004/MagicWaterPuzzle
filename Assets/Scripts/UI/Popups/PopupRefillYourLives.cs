using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupRefillYourLives : PopupUI
{
    [SerializeField] private Button buttonClose;

    private void Start()
    {
        buttonClose.onClick.AddListener(CloseClick);
    }

    private void CloseClick()
    {
        Hide();
    }
}
