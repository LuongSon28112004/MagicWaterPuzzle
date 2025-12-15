using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupBuyBooster : PopupUI
{
    [Header("Popup Bomb Booster Settings")]
    [SerializeField] Button buttonExit;
    [SerializeField] TextMeshProUGUI textCountCoin;
    [Header("Button Buy")]
    [SerializeField] Sprite yellowButton;
    [SerializeField] Image imageBuyButton_1;
    [SerializeField] Image imageBuyButton_2;


    private void Start()
    {
        AddEventListener();
        InitTextCountCoin();
        InitButton();
    }

    private void InitButton()
    {
        if (UserData.coin >= 1500)
        {
            imageBuyButton_1.sprite = yellowButton;
            imageBuyButton_2.sprite = yellowButton;
        }
    }

    private void InitTextCountCoin()
    {
        textCountCoin.text = UserData.coin.ToString();
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
