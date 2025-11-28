using UnityEngine;

public class TimerAndLevel : MonoBehaviour
{
    public TextLevel textLevel;
    public Timer timer;
    public TimerFreeze timerFreeze;


    // change TextLevel
    public void SetTextLevel(int level)
    {
        textLevel.ChangeLevel(level);
    }


    // freeze timer
    public void StartFreeze()
    {
        AudioManager.Instance.PlayOneShot("Freeze", 1f);
        timerFreeze.Show();
        timer.Hide();
    }

    public void EndFreeze()
    {
        AudioManager.Instance.PlayOneShot("EndFreeze", 1f);
        timer.Show();
        timerFreeze.Hide();
    }

    public void UpdateFreezeTimer(float value, float timerTimeLeft)
    {
        timerFreeze.SetTimerCountDown(value);
        timerFreeze.SetTimer(timerTimeLeft);
    }

    //timer
    public void UpdateTimer(float value)
    {
        timer.SetTimer(value);
    }


}
