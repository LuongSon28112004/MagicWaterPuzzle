using System;
using System.Collections.Generic;
using UnityEngine;

public static class DailyRewardManager
{
    private const string DATE_FORMAT = "yyyy-MM-dd";
    private const int VN_UTC_OFFSET_HOURS = 7;
    private const int STREAK_LENGTH = 7;

    private static DailyRewardConfig cachedConfig;
    private static bool serverSyncDone = false;

    public static event Action OnStateChanged;

    public static void ResetServerSync()
    {
        serverSyncDone = false;
    }

    public static DailyRewardConfig Config
    {
        get
        {
            if (cachedConfig == null)
            {
                cachedConfig = Resources.Load<DailyRewardConfig>("Configs/DailyRewardConfig");
                if (cachedConfig == null)
                {
                    Debug.LogError("[DailyRewardManager] Missing Resources/Configs/DailyRewardConfig.asset");
                }
            }
            return cachedConfig;
        }
    }

    public static DateTime VNNow() => DateTime.UtcNow.AddHours(VN_UTC_OFFSET_HOURS);
    public static string TodayString() => VNNow().ToString(DATE_FORMAT);

    /// <summary>1..7 — the slot the player would claim NEXT.</summary>
    public static int NextStreakDay()
    {
        if (UserData.dailyStreakDay <= 0) return 1;
        if (HasClaimedToday()) return UserData.dailyStreakDay;
        if (MissedADay()) return 1;
        int next = UserData.dailyStreakDay + 1;
        if (next > STREAK_LENGTH) next = 1;
        return next;
    }

    public static bool HasClaimedToday()
    {
        return !string.IsNullOrEmpty(UserData.lastDailyClaimDate)
               && UserData.lastDailyClaimDate == TodayString();
    }

    private static bool MissedADay()
    {
        if (string.IsNullOrEmpty(UserData.lastDailyClaimDate)) return false;
        if (!DateTime.TryParse(UserData.lastDailyClaimDate, out DateTime last)) return true;
        DateTime today = VNNow().Date;
        TimeSpan diff = today - last.Date;
        return diff.TotalDays > 1;
    }

    /// <summary>Popup shows whenever the player hasn't claimed today yet.</summary>
    public static bool ShouldShowPopupToday() => !HasClaimedToday();

    public static bool TryClaimToday(out DailyRewardEntry granted)
    {
        granted = null;
        if (HasClaimedToday()) return false;

        DailyRewardConfig cfg = Config;
        if (cfg == null) return false;

        int dayToClaim = NextStreakDay();
        DailyRewardEntry entry = cfg.GetRewardByIndex(dayToClaim - 1);
        if (entry == null)
        {
            Debug.LogWarning("[DailyRewardManager] No entry configured for day " + dayToClaim);
            return false;
        }

        GrantReward(entry);

        UserData.dailyStreakDay = dayToClaim;
        UserData.lastDailyClaimDate = TodayString();

        granted = entry;
        SaveDataManager.Save();
        OnStateChanged?.Invoke();
        return true;
    }

    private static void GrantReward(DailyRewardEntry entry)
    {
        switch (entry.rewardType)
        {
            case DailyRewardType.Coin:
                UserData.coin += entry.amount;
                UserData.RaiseCoinChanged();
                break;

            case DailyRewardType.Booster:
                if (string.IsNullOrEmpty(entry.boosterName))
                {
                    Debug.LogWarning("[DailyRewardManager] Booster reward has empty boosterName");
                    return;
                }
                if (UserData.listBoosterCounters == null)
                {
                    UserData.listBoosterCounters = new List<BoosterCounter>();
                }
                BoosterCounter found = UserData.listBoosterCounters.Find(b => b.name == entry.boosterName);
                if (found != null) found.count += entry.amount;
                else UserData.listBoosterCounters.Add(new BoosterCounter { name = entry.boosterName, count = entry.amount });
                break;
        }
    }

    /// <summary>
    /// Pulls LastDailyClaimDate / DailyStreakDay from Firebase to defeat local-clock cheating.
    /// If the stored date is in the future relative to local VN time, we trust the server and
    /// block claiming until local clock catches up.
    /// </summary>
    public static void SyncFromServer(Action onDone)
    {
        if (serverSyncDone) { onDone?.Invoke(); return; }

        var fb = UserDataFirebaseManager.Instance;
        if (fb == null || string.IsNullOrEmpty(fb.CurrentUserId))
        {
            onDone?.Invoke();
            return;
        }

        fb.GetUserData(fb.CurrentUserId, data =>
        {
            try
            {
                if (data != null)
                {
                    if (data.TryGetValue("LastDailyClaimDate", out object dateObj) && dateObj != null)
                    {
                        string serverDate = dateObj.ToString();
                        if (IsServerDateAhead(serverDate))
                        {
                            UserData.lastDailyClaimDate = serverDate;
                        }
                    }
                    if (data.TryGetValue("DailyStreakDay", out object streakObj) && streakObj != null)
                    {
                        int serverStreak = Convert.ToInt32(streakObj);
                        UserData.dailyStreakDay = Mathf.Max(UserData.dailyStreakDay, serverStreak);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[DailyRewardManager] SyncFromServer parse error: " + ex.Message);
            }
            serverSyncDone = true;
            onDone?.Invoke();
        });
    }

    private static bool IsServerDateAhead(string serverDate)
    {
        if (string.IsNullOrEmpty(serverDate)) return false;
        if (string.IsNullOrEmpty(UserData.lastDailyClaimDate)) return true;
        return string.Compare(serverDate, UserData.lastDailyClaimDate, StringComparison.Ordinal) > 0;
    }
}
