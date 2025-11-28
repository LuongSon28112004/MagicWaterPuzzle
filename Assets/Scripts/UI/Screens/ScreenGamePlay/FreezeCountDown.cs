using UnityEngine;

public class FreezeCountDown : CountdownTimer
{
    public override void StopCountDownTimer()
    {
        base.StopCountDownTimer();
        ScreenGamePlay screenGamePlay = UIManager.Instance.GetScreen<ScreenGamePlay>();
        screenGamePlay.ResumeTimer();
    }
}
