using UnityEngine;
using System;
using master;

public class HeartSystem : MonoBehaviour
{
    public static HeartSystem Instance { get; private set; }

    private int maxHearts = 5;
    private int currentHearts = 0;
    private int secondsPerHeart = 1800; // 30 phút

    private float timer;

    private void Awake()
    {
        Instance = this;
        Debug.Log("HeartSystem Awake");
    }

    public float Timer => timer;

    public int MaxHearts { get => maxHearts; set => maxHearts = value; }
    public int CurrentHearts { get => currentHearts; set => currentHearts = value; }
    public int SecondsPerHeart { get => secondsPerHeart; set => secondsPerHeart = value; }

    public event Action OnHeartChanged;


    private void RaiseChange()
    {
        OnHeartChanged?.Invoke();
    }

    /// <summary>
    /// Trừ 1 heart khi chơi game. Không cho xuống dưới 0.
    /// </summary>
    public void UseHeart()
    {
        if (CurrentHearts > 0)
        {
            CurrentHearts--;
            SaveHearts();
            RaiseChange();
        }
    }

    /// <summary>
    /// Gọi static để trừ tim an toàn ở bất kỳ Scene nào (ngay cả khi HeartSystem bị huỷ).
    /// </summary>
    public static void ConsumeHeart()
    {
        if (Instance != null)
        {
            Instance.UseHeart();
        }
        else
        {
            int current = PlayerPrefs.GetInt("Hearts", 0);
            if (current > 0)
            {
                current--;
                PlayerPrefs.SetInt("Hearts", current);
                
                // Nếu vừa rớt xuống dưới max, reset timer offline
                if (current + 1 >= 5) 
                {
                    PlayerPrefs.SetString("LastQuitTime", DateTime.Now.ToBinary().ToString());
                    PlayerPrefs.SetFloat("Timer", 0);
                }
                
                SaveDataManager.Save();
            }
        }
    }

    /// <summary>
    /// Thêm heart từ gift người khác (không giới hạn max).
    /// </summary>
    public void AddHeartsFromGift(int amount)
    {
        CurrentHearts += amount;
        SaveHearts();
        RaiseChange();
    }

    void Start()
    {
        LoadHearts();
        RaiseChange(); // ← Update UI ngay khi mở game
    }

    void Update()
    {
        if (CurrentHearts >= MaxHearts)
        {
            // Tim >= max → dừng bộ đếm và reset timer về 0
            // Khi tim giảm xuống < max, bộ đếm sẽ bắt đầu lại từ 0
            if (timer != 0)
            {
                timer = 0;
                SaveHearts();
                RaiseChange();
            }
            return;
        }

        timer += Time.deltaTime;

        // Mỗi frame ta luôn update UI realtime
        RaiseChange();

        if (timer >= SecondsPerHeart)
        {
            timer = 0;
            CurrentHearts++;
            SaveHearts();
            RaiseChange(); // ← cập nhật UI khi +1 tim
        }
    }

    void OnApplicationQuit()
    {
        SaveHearts();
    }

    // ---------------- SAVE & LOAD -----------------

    public void SaveHearts()
    {
        PlayerPrefs.SetInt("Hearts", CurrentHearts);
        PlayerPrefs.SetFloat("Timer", timer);
        PlayerPrefs.SetString("LastQuitTime", DateTime.Now.ToBinary().ToString());
    }

    void LoadHearts()
    {
        CurrentHearts = PlayerPrefs.GetInt("Hearts", 0);
        timer = PlayerPrefs.GetFloat("Timer", 0);

        string savedTime = PlayerPrefs.GetString("LastQuitTime", "");

        if (!string.IsNullOrEmpty(savedTime))
        {
            DateTime last = DateTime.FromBinary(Convert.ToInt64(savedTime));
            TimeSpan diff = DateTime.Now - last;

            double totalSec = diff.TotalSeconds;

            // Chỉ hồi tim khi đang < max (tim từ gift vượt max thì không hồi thêm)
            if (CurrentHearts < MaxHearts)
            {
                // Gộp timer cũ + thời gian offline để tính chính xác
                double totalTimerSec = timer + totalSec;
                int heartsRecovered = (int)(totalTimerSec / SecondsPerHeart);
                double remain = totalTimerSec % SecondsPerHeart;

                CurrentHearts += heartsRecovered;
                timer = (float)remain;

                // Clamp: regen không được vượt max
                if (CurrentHearts >= MaxHearts)
                {
                    CurrentHearts = MaxHearts;
                    timer = 0;
                }
            }
            else
            {
                // Tim >= max (từ gift) → không cần regen, reset timer
                timer = 0;
            }
        }
        SaveHearts();
    }
}
