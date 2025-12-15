using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopupOutOfTime : PopupUI
{
    [SerializeField] Button buttonExit;
    [Header("Coin")]
    [SerializeField] private TextMeshProUGUI textCoin;

    private void Start()
    {
        //xóa bớt tim khi hết time chơi
        int CurrentHearts = PlayerPrefs.GetInt("Hearts", 0);
        CurrentHearts -= 1;
        PlayerPrefs.SetInt("Hearts", CurrentHearts);
        AddEventListener();
        //init textCoin
        LoadCoin();
    }

    private void LoadCoin()
    {
        textCoin.text = UserData.coin.ToString();
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
