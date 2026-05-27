using UnityEditor;
using UnityEngine;

public static class DailyRewardDevTools
{
    [MenuItem("Tools/Daily Reward/Make Next Day Available")]
    public static void MakeNextDayAvailable()
    {
        // If not playing, load data first to avoid overwriting with default values
        if (!Application.isPlaying)
        {
            SaveDataManager.Load();
        }

        UserData.lastDailyClaimDate = "";
        SaveDataManager.Save();

        Debug.Log("<color=green>[DevTool] Next day is now available for claiming!</color>");

        // Refresh UI if playing and popup is active
        if (Application.isPlaying)
        {
            var popup = Object.FindFirstObjectByType<PopupDailyReward>();
            if (popup != null)
            {
                popup.RefreshUI();
            }
        }
    }

    [MenuItem("Tools/Daily Reward/Force Claim Next Day (Play Mode Only)")]
    public static void ForceClaimNextDay()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("[DevTool] Force Claim can only be executed in Play Mode!");
            EditorUtility.DisplayDialog("Error", "Force Claim can only be executed in Play Mode!", "OK");
            return;
        }

        // Clear last claim date to allow claiming
        UserData.lastDailyClaimDate = "";

        if (DailyRewardManager.TryClaimToday(out var granted))
        {
            Debug.Log($"<color=green>[DevTool] Successfully claimed next day's reward: {granted.amount} {granted.rewardType} ({granted.boosterName})</color>");
            
            // Refresh UI if popup is active
            var popup = Object.FindFirstObjectByType<PopupDailyReward>();
            if (popup != null)
            {
                popup.RefreshUI();
            }
        }
        else
        {
            Debug.LogError("[DevTool] Failed to claim reward. Make sure DailyRewardConfig is set up.");
        }
    }
}
