using System;
using UnityEngine;

public enum DailyRewardType
{
    Coin,
    Booster
}

[Serializable]
public class DailyRewardEntry
{
    public string dayLabel = "Day 1";
    public DailyRewardType rewardType = DailyRewardType.Coin;
    public int amount = 100;
    public string boosterName = "";
    public Sprite icon;
}

[CreateAssetMenu(fileName = "DailyRewardConfig", menuName = "MagicWaterPuzzle/Daily Reward Config")]
public class DailyRewardConfig : ScriptableObject
{
    [Tooltip("7 entries: Day 1 ... Day 7. Streak resets to Day 1 if the player misses a day, and loops back to Day 1 after Day 7.")]
    public DailyRewardEntry[] rewards = new DailyRewardEntry[7];

    public DailyRewardEntry GetRewardByIndex(int index)
    {
        if (rewards == null || index < 0 || index >= rewards.Length) return null;
        return rewards[index];
    }
}
