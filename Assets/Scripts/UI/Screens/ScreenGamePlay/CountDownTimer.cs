using System;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public float Duration;      // Tổng thời gian
    public float TimeLeft;     // Thời gian còn lại
    public bool IsRunning;    // Có đang chạy không
    public bool IsPaused;      // Có đang pause không

    public Action<float> OnTick;       // Gọi mỗi frame: trả về TimeLeft
    public Action OnCompleted;         // Khi đếm xong

    public void Init(float duration)
    {
        Duration = duration;
        TimeLeft = duration;
    }

    public void StartCountDownTimer()
    {
        TimeLeft = Duration;
        IsRunning = true;
        IsPaused = false;
    }

    public virtual void StopCountDownTimer()
    {
        IsRunning = false;
        IsPaused = false;
        TimeLeft = 0f;
        OnCompleted?.Invoke();
    }

    public void PauseCountDownTimer()
    {
        if (IsRunning)
            IsPaused = true;
    }

    public void ResumeCountDownTimer()
    {
        if (IsPaused)
            IsPaused = false;
    }

    public void ResetCountDownTimer()
    {
        TimeLeft = Duration;
        IsPaused = false;
        IsRunning = false;
    }

    public void Update()
    {
        if (!IsRunning || IsPaused) return;

        TimeLeft -= Time.deltaTime;

        OnTick?.Invoke(TimeLeft);

        if (TimeLeft <= 0f)
        {
            StopCountDownTimer();
        }
    }
}
