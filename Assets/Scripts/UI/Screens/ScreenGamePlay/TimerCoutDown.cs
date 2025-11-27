using UnityEngine;

public class TimerCoutDown : CountdownTimer
{
    public override void StopCountDownTimer()
    {
        base.StopCountDownTimer();
        UIManager.Instance.ShowPopup<PopupOutOfTime>(null);
    }
}
