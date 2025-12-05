using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StatusChoice
{
    Shop,
    Home,
    RANKING,
}

public class PopupTab : PopupUI
{
    [Header("Buttons")]
    [SerializeField] private Button ShopButton;
    [SerializeField] private Button HomeButton;
    [SerializeField] private Button RankingButton;
    [Header("TextName")]
    [SerializeField] private TextMeshProUGUI TextShop;
    [SerializeField] private TextMeshProUGUI TextHome;
    [SerializeField] private TextMeshProUGUI TextRanking;

    [Header("Parents")]
    [SerializeField] private GameObject ChoicePanel;
    [SerializeField] private Transform Parent;

    private StatusChoice currentStatus;

    void Start()
    {
        AddButtonListeners();
        currentStatus = StatusChoice.Home; // hoặc cái nào là mặc định hiển thị
    }

    private void AddButtonListeners()
    {
        ShopButton.onClick.AddListener(() => ChangeStatusChoicePanel(StatusChoice.Shop));
        HomeButton.onClick.AddListener(() => ChangeStatusChoicePanel(StatusChoice.Home));
        RankingButton.onClick.AddListener(() => ChangeStatusChoicePanel(StatusChoice.RANKING));
    }

    private void ChangeStatusChoicePanel(StatusChoice newChoice)
    {
        if (newChoice == currentStatus) return;


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
                UIManager.Instance.ShowScreen<ScreenLock>();
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
}
