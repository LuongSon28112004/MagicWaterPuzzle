using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupOutOfTime : PopupUI
{
    [SerializeField] Button buttonExit;
    [Header("Coin")]
    [SerializeField] private TextMeshProUGUI textCoin;
    [SerializeField] private Button buttonBuyMoreTime;
    [SerializeField] private Image image_1;
    [SerializeField] private Image image_2;
    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI textTimer;
    [SerializeField] private Sprite ImageBtnYellow;

    private void Start()
    {
        //xóa bớt tim khi hết time chơi
        int CurrentHearts = PlayerPrefs.GetInt("Hearts", 0);
        CurrentHearts -= 1;
        PlayerPrefs.SetInt("Hearts", CurrentHearts);
        AddEventListener();
        //init textCoin
        LoadCoin();
        //init button 
        LoadPanelButton();
    }

    private void LoadPanelButton()
    {
        if (UserData.coin >= 1350)
        {
            image_1.sprite = ImageBtnYellow;
            image_2.sprite = ImageBtnYellow;
        }
    }

    private void LoadCoin()
    {
        textCoin.text = UserData.coin.ToString();
    }

    private void AddEventListener()
    {
        buttonExit.onClick.AddListener(ShowPopupLoseGame);
        buttonBuyMoreTime.onClick.AddListener(BuyMoreTime);
    }

    private void BuyMoreTime()
    {
        if (UserData.coin < 1350)
        {
            return;
        }
        UserData.coin -= 1350;
        LoadCoin();
        SaveDataManager.Save();
        var UI = UIManager.Instance.GetScreen<ScreenGamePlay>();
        UI.Timer.Duration = 20;
        UI.Timer.StartCountDownTimer();
        Hide();
    }

    private void ShowPopupLoseGame()
    {
        Hide();
        UIManager.Instance.ShowPopup<PopupLose>(null);
    }
}
