using System.IO;
using UnityEditor;
using UnityEngine;

public static class SetupDailyRewardConfig
{
    private const string FOLDER = "Assets/Resources/Configs";
    private const string ASSET_PATH = "Assets/Resources/Configs/DailyRewardConfig.asset";

    [MenuItem("MagicWaterPuzzle/Daily Reward/Create Default Config Asset")]
    public static void CreateDefault()
    {
        if (!Directory.Exists(FOLDER))
        {
            Directory.CreateDirectory(FOLDER);
            AssetDatabase.Refresh();
        }

        DailyRewardConfig existing = AssetDatabase.LoadAssetAtPath<DailyRewardConfig>(ASSET_PATH);
        if (existing != null)
        {
            Debug.Log("[SetupDailyRewardConfig] Asset already exists at " + ASSET_PATH);
            Selection.activeObject = existing;
            EditorGUIUtility.PingObject(existing);
            return;
        }

        DailyRewardConfig cfg = ScriptableObject.CreateInstance<DailyRewardConfig>();
        cfg.rewards = new DailyRewardEntry[7];

        // Default: Day 1..6 give coin, Day 7 gives a Hammer booster (big-finish reward).
        int[] defaultCoins = { 100, 150, 200, 300, 400, 600, 0 };

        for (int i = 0; i < 7; i++)
        {
            int day = i + 1;
            if (day == 7)
            {
                cfg.rewards[i] = new DailyRewardEntry
                {
                    dayLabel = "Day 7",
                    rewardType = DailyRewardType.Booster,
                    amount = 3,
                    boosterName = "Hammer"
                };
            }
            else
            {
                cfg.rewards[i] = new DailyRewardEntry
                {
                    dayLabel = "Day " + day,
                    rewardType = DailyRewardType.Coin,
                    amount = defaultCoins[i],
                    boosterName = ""
                };
            }
        }

        AssetDatabase.CreateAsset(cfg, ASSET_PATH);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = cfg;
        EditorGUIUtility.PingObject(cfg);
        Debug.Log("[SetupDailyRewardConfig] Created default config at " + ASSET_PATH);
    }
}
