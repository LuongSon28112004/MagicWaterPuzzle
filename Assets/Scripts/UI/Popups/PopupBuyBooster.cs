using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupBuyBooster : PopupUI
{
    [Header("Popup Bomb Booster Settings")]
    [SerializeField] Button buttonExit;
    [SerializeField] TextMeshProUGUI textCountCoin;


    private void Start()
    {
        AddEventListener();
        InitTextCountCoin();
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
