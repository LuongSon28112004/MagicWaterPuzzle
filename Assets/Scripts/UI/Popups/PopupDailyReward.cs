using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DailyRewardSlotUI
{
    public Button button;
    public Image icon;
    public Text amountText;
    public Text dayLabelText;
    public GameObject claimedOverlay;
    public GameObject todayHighlight;
}

public class PopupDailyReward : PopupUI
{
    [Header("Daily Reward")]
    [SerializeField] private DailyRewardSlotUI[] slots = new DailyRewardSlotUI[7];

    [Header("Localization Keys (optional)")]
    [SerializeField] private Text titleText;
    [SerializeField] private string titleKey = "daily_reward_title";
    [SerializeField] private Text subTitleText;
    [SerializeField] private string subTitleKey = "daily_reward_subtitle";

    private DailyRewardConfig config;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        isCache = true;
    }

    public override void Show(Action onClose)
    {
        base.Show(onClose);
        config = DailyRewardManager.Config;
        ApplyLocalization();
        RefreshUI();
    }

    private void ApplyLocalization()
    {
        if (LanguageManager.Instance == null) return;
        if (titleText != null && !string.IsNullOrEmpty(titleKey))
            titleText.text = LanguageManager.Instance.GetTranslation(titleKey);
        if (subTitleText != null && !string.IsNullOrEmpty(subTitleKey))
            subTitleText.text = LanguageManager.Instance.GetTranslation(subTitleKey);
    }

    public void RefreshUI()
    {
        if (config == null) config = DailyRewardManager.Config;
        if (config == null) return;

        int nextDay = DailyRewardManager.NextStreakDay();
        bool claimedToday = DailyRewardManager.HasClaimedToday();

        for (int i = 0; i < slots.Length; i++)
        {
            DailyRewardEntry entry = config.GetRewardByIndex(i);
            DailyRewardSlotUI slot = slots[i];
            if (entry == null || slot == null) continue;

            int slotDay = i + 1;
            bool isToday = (slotDay == nextDay);

            if (slot.icon != null && entry.icon != null) slot.icon.sprite = entry.icon;
            if (slot.amountText != null) slot.amountText.text = "x" + entry.amount;
            if (slot.dayLabelText != null) slot.dayLabelText.text = FormatDayLabel(slotDay, entry);
            if (slot.todayHighlight != null) slot.todayHighlight.SetActive(isToday);

            // Mark all days BEFORE today as already-claimed in this streak, plus today if claimed.
            bool alreadyClaimedSlot = (slotDay < nextDay) || (isToday && claimedToday);
            if (slot.claimedOverlay != null) slot.claimedOverlay.SetActive(alreadyClaimedSlot);

            int capturedIndex = i;
            if (slot.button != null)
            {
                slot.button.onClick.RemoveAllListeners();
                slot.button.interactable = isToday && !claimedToday;
                slot.button.onClick.AddListener(() => OnClickSlot(capturedIndex));
            }
        }
    }

    private string FormatDayLabel(int day, DailyRewardEntry entry)
    {
        if (!string.IsNullOrEmpty(entry.dayLabel)) return entry.dayLabel;
        if (LanguageManager.Instance != null)
        {
            string template = LanguageManager.Instance.GetTranslation("daily_reward_day_n");
            if (template != "daily_reward_day_n") return template.Replace("{0}", day.ToString());
        }
        return "Day " + day;
    }

    private void OnClickSlot(int index)
    {
        int slotDay = index + 1;
        if (slotDay != DailyRewardManager.NextStreakDay()) return;

        if (DailyRewardManager.TryClaimToday(out DailyRewardEntry granted))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayOneShot("ClickButton", 1f);

            string msg = BuildClaimMessage(granted);
            UIManager.Instance.NotifyContent(msg);
            RefreshUI();
        }
    }

    private string BuildClaimMessage(DailyRewardEntry entry)
    {
        string suffix = entry.rewardType == DailyRewardType.Coin
            ? "Coin"
            : entry.boosterName;

        if (LanguageManager.Instance != null)
        {
            string tpl = LanguageManager.Instance.GetTranslation("daily_reward_received");
            if (tpl != "daily_reward_received")
            {
                return tpl.Replace("{0}", entry.amount.ToString()).Replace("{1}", suffix);
            }
        }
        return $"+{entry.amount} {suffix}";
    }
}
