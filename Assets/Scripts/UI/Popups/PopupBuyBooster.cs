using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupBuyBooster : PopupUI
{
    [Header("Popup Bomb Booster Settings")]
    [SerializeField] private TypeCollectBooster typeCollectBooster;
    [SerializeField] Button buttonExit;
    [SerializeField] TextMeshProUGUI textCountCoin;
    [Header("Button Buy")]
    [SerializeField] Sprite yellowButton;
    [SerializeField] Image imageBuyButton_1;
    [SerializeField] Image imageBuyButton_2;
    [SerializeField] Button buttonBuy;
    [SerializeField] Image iconBooster;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] Sprite bombSprite;
    [SerializeField] Sprite freezeSprite;
    [SerializeField] Sprite hammerSprite;


    private void Start()
    {
        AddEventListener();
        InitTextCountCoin();
        InitButton();
    }

    public void ChangeTypeBooster(TypeCollectBooster type)
    {
        typeCollectBooster = type;
        switch (typeCollectBooster)
        {
            case TypeCollectBooster.BOMB:
                iconBooster.sprite = bombSprite;
                titleText.text = Contacts.Instance.GetTutBooster(TypeCollectBooster.BOMB);
                textName.text = "BOMB";
                break;
            case TypeCollectBooster.FREEZE:
                iconBooster.sprite = freezeSprite;
                titleText.text = Contacts.Instance.GetTutBooster(TypeCollectBooster.FREEZE);
                textName.text = "FREEZE";
                break;
            case TypeCollectBooster.HAMMER:
                iconBooster.sprite = hammerSprite;
                titleText.text = Contacts.Instance.GetTutBooster(TypeCollectBooster.HAMMER);
                textName.text = "HAMMER";
                break;
            default:
                break;
        }
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
        buttonBuy.onClick.AddListener(BuyBooster);
    }

    private void BuyBooster()
    {
        StartCoroutine(BuyBoosterCoroutine());
    }

    private IEnumerator BuyBoosterCoroutine()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        if (UserData.coin >= 1500)
        {
            int initialCoin = UserData.coin;
            UserData.coin -= 1500;
            // Cập nhật lại số coin trên popup từ từ
            float duration = 0.5f; // Thời gian chuyển đổi
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                int displayedCoin = Mathf.RoundToInt(Mathf.Lerp(initialCoin, UserData.coin, t));
                textCountCoin.text = displayedCoin.ToString();
                yield return null;
            }
            // Thêm 2 booster vào kho
            if (typeCollectBooster == TypeCollectBooster.BOMB)
            {
                UserData.listBoosterCounters[1].count += 2;
            }
            else if (typeCollectBooster == TypeCollectBooster.FREEZE)
            {
                UserData.listBoosterCounters[0].count += 2;
            }
            else if (typeCollectBooster == TypeCollectBooster.HAMMER)
            {
                UserData.listBoosterCounters[2].count += 2;
            }
            SaveDataManager.Save();
            // Cập nhật lại số lượng booster trên màn hình chơi
            var UI = UIManager.Instance.GetScreen<ScreenGamePlay>();
            UI.InitCountBooster();
            yield return new WaitForSeconds(0.2f);
            if (typeCollectBooster == TypeCollectBooster.BOMB)
            {
                UI.BombClick();
            }
            else if (typeCollectBooster == TypeCollectBooster.FREEZE)
            {
                UI.FreezeClick();
            }
            else if (typeCollectBooster == TypeCollectBooster.HAMMER)
            {
                UI.HammerClick();
            }
            Hide();
        }
    }

    private void ExitClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        Hide();
    }
}
