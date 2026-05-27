using System;
using System.Collections.Generic;

[System.Serializable]
public class BoosterCounter
{
    public string name;
    public int count;
}

public static class UserData
{
    public static int coin = 1000;
    public static int level = 1;
    public static List<BoosterCounter> listBoosterCounters = new List<BoosterCounter>();

    // ----- Daily Reward -----
    // 0 = never claimed; 1..7 = current streak position (loops back to 1 after 7).
    public static int dailyStreakDay = 0;
    // VN date string "yyyy-MM-dd" of the last successful claim.
    public static string lastDailyClaimDate = "";

    public static event Action OnCoinChanged;
    public static void RaiseCoinChanged() => OnCoinChanged?.Invoke();
}
