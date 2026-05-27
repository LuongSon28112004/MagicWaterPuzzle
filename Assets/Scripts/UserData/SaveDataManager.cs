using System.IO;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coin;
    public int level;
    public List<BoosterCounter> listBoosterCounters;
    public int dailyStreakDay;
    public string lastDailyClaimDate;
}

public static class SaveDataManager
{
    private static string saveFilePath = Path.Combine(Application.persistentDataPath, "UserData.json");

    public static void Save()
    {
        PlayerData data = new PlayerData
        {
            coin = UserData.coin,
            level = UserData.level,
            listBoosterCounters = UserData.listBoosterCounters,
            dailyStreakDay = UserData.dailyStreakDay,
            lastDailyClaimDate = UserData.lastDailyClaimDate
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"[SaveDataManager] Dữ liệu đã được lưu tại: {saveFilePath}");

        if (UserDataFirebaseManager.Instance != null && !string.IsNullOrEmpty(UserDataFirebaseManager.Instance.CurrentUserId))
        {
            List<Dictionary<string, object>> boostersList = new List<Dictionary<string, object>>();
            if (UserData.listBoosterCounters != null)
            {
                foreach (var booster in UserData.listBoosterCounters)
                {
                    boostersList.Add(new Dictionary<string, object>
                    {
                        { "name", booster.name },
                        { "count", booster.count }
                    });
                }
            }

            int currentHearts = HeartSystem.Instance != null ? HeartSystem.Instance.CurrentHearts : PlayerPrefs.GetInt("Hearts", 5);

            Dictionary<string, object> firebaseData = new Dictionary<string, object>
            {
                { "Coin", UserData.coin },
                { "Level", UserData.level },
                { "Heart", currentHearts },
                { "Boosters", boostersList },
                { "DailyStreakDay", UserData.dailyStreakDay },
                { "LastDailyClaimDate", UserData.lastDailyClaimDate }
            };

            UserDataFirebaseManager.Instance.SaveUserData(UserDataFirebaseManager.Instance.CurrentUserId, firebaseData);
        }
    }

    public static void Load()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("[SaveDataManager] Không tìm thấy file UserData.json, tạo dữ liệu mặc định...");

            UserData.listBoosterCounters = new List<BoosterCounter>
            {
                new BoosterCounter { name = "Freeze", count = 1 },
                new BoosterCounter { name = "Bomb", count = 1 },
                new BoosterCounter { name = "Hammer", count = 1 },
            };
            UserData.dailyStreakDay = 0;
            UserData.lastDailyClaimDate = "";
            PlayerPrefs.SetInt("Hearts", 5);

            Save();
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        UserData.coin = data.coin;
        UserData.level = data.level;

        if (data.listBoosterCounters != null)
            UserData.listBoosterCounters = data.listBoosterCounters;
        else
            UserData.listBoosterCounters = new List<BoosterCounter>();

        UserData.dailyStreakDay = data.dailyStreakDay;
        UserData.lastDailyClaimDate = data.lastDailyClaimDate ?? "";

        Debug.Log("[SaveDataManager] Dữ liệu đã được tải thành công!");
    }

    public static void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("[SaveDataManager] File UserData.json đã bị xóa.");
        }
    }
}
