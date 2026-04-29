using System;
using System.Collections;
using DG.Tweening;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum StatusChoice
{
    Shop,
    Home,
    RANKING,
}

public class PopupTab : PopupUI, IPointerDownHandler, IPointerUpHandler
{
    [Header("Buttons")]
    [SerializeField] private Button ShopButton;
    [SerializeField] private Button HomeButton;
    [SerializeField] private Button RankingButton;
    [Header("TextName")]
    [SerializeField] private TextMeshProUGUI TextShop;
    [SerializeField] private TextMeshProUGUI TextHome;
    [SerializeField] private TextMeshProUGUI TextRanking;
    [Header("Coin")]
    [SerializeField] private TextMeshProUGUI textCoin;
    [Header("Settings")]
    [SerializeField] private Button settingsButton;
    [Header("Heart System")]
    [SerializeField] private HeartSystem heartSystem;
    [SerializeField] private TextMeshProUGUI textTimerHeart;
    [SerializeField] private TextMeshProUGUI textCountHeart;

    [Header("Parents")]
    [SerializeField] private GameObject ChoicePanel;
    [SerializeField] private Transform Parent;

    [Header("Target Coin")]
    public RectTransform coinTarget;
    private StatusChoice currentStatus;
    [Header("Panel Lock")]
    [SerializeField] private Transform PanelLock;
    [Header("Layout")]
    [SerializeField] private Transform TopLayout;

    // Swipe
    private Vector2 touchStart;
    private float minSwipeDistance = 100f;

    private void OnEnable()
    {
        heartSystem.OnHeartChanged += UpdateHeartUI;
        UpdateHeartUI();
    }

    private void OnDisable()
    {
        heartSystem.OnHeartChanged -= UpdateHeartUI;
    }


    void Start()
    {
        InitCoin();
        AddButtonListeners();
        currentStatus = StatusChoice.Home; // hoặc cái nào là mặc định hiển thị
    }

    private void Update()
    {
        UpdateHeartUI(); // Cập nhật UI mỗi frame
    }


    private void UpdateHeartUI()
    {
        // Cập nhật số tim
        textCountHeart.text = heartSystem.CurrentHearts.ToString();

        // Nếu đủ 5 tim → hiển thị MAX
        if (heartSystem.CurrentHearts >= heartSystem.MaxHearts)
        {
            textTimerHeart.text = "MAX";
            return;
        }

        // Ngược lại → hiển thị thời gian còn lại để hồi 1 tim
        float remain = heartSystem.SecondsPerHeart - heartSystem.Timer;

        if (remain < 0) remain = 0;

        TimeSpan t = TimeSpan.FromSeconds(remain);

        textTimerHeart.text = $"{t.Minutes:00}:{t.Seconds:00}";
    }


    private void InitCoin()
    {
        textCoin.text = UserData.coin.ToString();
    }

    public IEnumerator UpdateCoin(int coin)
    {
        yield return new WaitForSeconds(1f);
        int currentCoin = UserData.coin;
        for (int i = 0; i <= coin / 50 - 1; i++)
        {
            currentCoin += 50;
            textCoin.text = currentCoin.ToString();
            yield return new WaitForSeconds(0.01f);
        }
        UserData.coin += coin;
        SaveDataManager.Save();
    }

    private void AddButtonListeners()
    {
        ShopButton.onClick.AddListener(() => ChangeStatusChoicePanel(StatusChoice.Shop));
        HomeButton.onClick.AddListener(() => ChangeStatusChoicePanel(StatusChoice.Home));
        RankingButton.onClick.AddListener(() => ChangeStatusChoicePanel(StatusChoice.RANKING));
        //Settings
        settingsButton.onClick.AddListener(SettingsClick);

    }

    private void SettingsClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        StartCoroutine(ShowPopupSettings());
    }

    private IEnumerator ShowPopupSettings()
    {
        yield return new WaitForSeconds(0.2f);
        UIManager.Instance.ShowPopup<PopupSetting>(null);
    }

    private void ChangeStatusChoicePanel(StatusChoice newChoice)
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        if (newChoice == currentStatus) return;

        if (newChoice == StatusChoice.RANKING)
        {
            TopLayout.gameObject.SetActive(false);
        }
        else
        {
            TopLayout.gameObject.SetActive(true);
        }


        //Sound
        AudioManager.Instance.PlayOneShot("BLJ_UI_Tab_04", 1f);
        // Đưa tab cũ về parent gốc
        ExitChild(currentStatus);

        // Gán tab mới vào ChoicePanel
        Button chosenButton = GetButton(newChoice);
        chosenButton.transform.SetParent(ChoicePanel.transform, false);
        Button oldButton = GetButton(currentStatus);
        chosenButton.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f);
        oldButton.transform.DOScale(Vector3.one, 0.1f);
        ActiveText(newChoice);
        InactiveOldText(currentStatus);
        ChoicePanel.transform.SetSiblingIndex(GetSiblingIndex(newChoice));


        // Reset vị trí, anchor trung tâm
        RectTransform rt = chosenButton.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        currentStatus = newChoice;
        ShowPopUpTab(currentStatus);
    }

    private void InactiveOldText(StatusChoice currentStatus)
    {
        switch (currentStatus)
        {
            case StatusChoice.Shop:
                TextShop.gameObject.SetActive(false);
                break;
            case StatusChoice.Home:
                TextHome.gameObject.SetActive(false);
                break;
            case StatusChoice.RANKING:
                TextRanking.gameObject.SetActive(false);
                break;

        }
    }

    private void ActiveText(StatusChoice currentStatus)
    {
        switch (currentStatus)
        {
            case StatusChoice.Shop:
                TextShop.gameObject.SetActive(true);
                break;
            case StatusChoice.Home:
                TextHome.gameObject.SetActive(true);
                break;
            case StatusChoice.RANKING:
                TextRanking.gameObject.SetActive(true);
                break;

        }
    }

    private void ShowPopUpTab(StatusChoice currentStatus)
    {
        switch (currentStatus)
        {
            case StatusChoice.Shop:
                UIManager.Instance.ShowScreen<ScreenShop>();
                break;
            case StatusChoice.Home:
                UIManager.Instance.ShowScreen<ScreenHome>();
                break;
            case StatusChoice.RANKING:
                UIManager.Instance.ShowScreen<ScreenRanking>();
                break;
        }
    }

    private void ExitChild(StatusChoice choice)
    {
        Button button = GetButton(choice);
        if (button == null) return;

        // Trả về parent gốc
        button.transform.SetParent(Parent, false);

        // Sắp xếp lại đúng thứ tự
        button.transform.SetSiblingIndex(GetSiblingIndex(choice));
    }

    private Button GetButton(StatusChoice choice)
    {
        return choice switch
        {
            StatusChoice.Shop => ShopButton,
            StatusChoice.Home => HomeButton,
            StatusChoice.RANKING => RankingButton,

            _ => null
        };
    }

    private int GetSiblingIndex(StatusChoice choice)
    {
        return choice switch
        {
            StatusChoice.Shop => 0,
            StatusChoice.Home => 1,
            StatusChoice.RANKING => 2,
            _ => 0
        };
    }

    // ========================
    //       SWIPE SYSTEM
    // ========================

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStart = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 touchEnd = eventData.position;
        float deltaX = touchEnd.x - touchStart.x;

        if (Mathf.Abs(deltaX) < minSwipeDistance)
            return;

        if (deltaX > 0)
            SwipeRight();
        else
            SwipeLeft();
    }

    private void SwipeLeft()
    {
        switch (currentStatus)
        {
            case StatusChoice.Home:
                ChangeStatusChoicePanel(StatusChoice.RANKING);
                break;
            case StatusChoice.Shop:
                ChangeStatusChoicePanel(StatusChoice.Home);
                break;
        }
    }

    private void SwipeRight()
    {
        switch (currentStatus)
        {
            case StatusChoice.Home:
                ChangeStatusChoicePanel(StatusChoice.Shop);
                break;
            case StatusChoice.RANKING:
                ChangeStatusChoicePanel(StatusChoice.Home);
                break;
        }
    }

    //panel lock
    public void SetPanelLock(bool isLock)
    {
        PanelLock.gameObject.SetActive(isLock);
    }
}
