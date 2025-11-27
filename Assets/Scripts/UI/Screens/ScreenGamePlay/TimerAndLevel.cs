using UnityEngine;

public class TimerAndLevel : MonoBehaviour
{
    public TextLevel textLevel;
    public Timer timer;
    public TimerFreeze timerFreeze;


    public void SetTextLevel(int level)
    {
        textLevel.ChangeLevel(level);
    }

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

    public void UpdateTimer(string value)
    {
        timer.SetTimer(value);
    }


}
