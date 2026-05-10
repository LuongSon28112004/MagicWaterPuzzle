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

    void Start()
    {
        LoadHearts();
        RaiseChange(); // ← Update UI ngay khi mở game
    }

    void Update()
    {
        if (CurrentHearts >= MaxHearts) return;

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

            // Tính số tim hồi được
            int heartsRecovered = (int)(totalSec / SecondsPerHeart);
            double remain = totalSec % SecondsPerHeart;

            CurrentHearts += heartsRecovered;
            timer += (float)remain;

            // Clamp giá trị
            if (CurrentHearts >= MaxHearts)
            {
                //CurrentHearts = MaxHearts;
                timer = 0;
            }
        }
        SaveHearts();
    }
}
