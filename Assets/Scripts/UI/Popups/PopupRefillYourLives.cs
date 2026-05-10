using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupRefillYourLives : PopupUI
{
    [SerializeField] private Button buttonClose;
    [SerializeField] private Button buttonBuyHeart;

    [Header("Button Buy Heart")]
    [SerializeField] private Transform GrayButtonBuyHeart;
    [SerializeField] private Transform YellowButtonBuyHeart;
    [SerializeField] private int heartCost = 1500;

    private bool isBuying = false;

    private void Start()
    {
        buttonClose.onClick.AddListener(CloseClick);
        buttonBuyHeart.onClick.AddListener(BuyHeart);
    }

    private void OnEnable()
    {
        CheckCoinAndUpdateButton();
    }

    private void CheckCoinAndUpdateButton()
    {
        if (UserData.coin >= heartCost )
        {
            GrayButtonBuyHeart.gameObject.SetActive(false);
            YellowButtonBuyHeart.gameObject.SetActive(true);
        }
        else
        {
            GrayButtonBuyHeart.gameObject.SetActive(true);
            YellowButtonBuyHeart.gameObject.SetActive(false);
        }
    }

    private void BuyHeart()
    {
        if(UserData.coin < heartCost)
        {
            UIManager.Instance.NotifyContent("Bạn không đủ tiền để mua tim");
            return;
        }
        if (isBuying) return;

        AudioManager.Instance.PlayOneShot("ClickButton", 1f);

        if (UserData.coin >= heartCost)
        {
            isBuying = true;

            // Trừ vàng
            UserData.coin -= heartCost;

            // Hồi đầy tim
            if (HeartSystem.Instance != null)
            {
                HeartSystem.Instance.CurrentHearts = HeartSystem.Instance.MaxHearts;
                HeartSystem.Instance.SaveHearts();
            }

            // Lưu local + Firebase
            SaveDataManager.Save();

            isBuying = false;
            UIManager.Instance.NotifyContent("Mua tim thành công");
            Hide();
        }
    }

    private void CloseClick()
    {
        Hide();
    }
}
