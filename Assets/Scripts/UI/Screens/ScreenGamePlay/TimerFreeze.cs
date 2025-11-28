using TMPro;
using UnityEngine;

public class TimerFreeze : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] TextMeshProUGUI timerCountDown;
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetTimer(float value)
    {
        if (value <= 30)
        {
            timer.color = Color.red;
        }
        else
        {
            timer.color = Color.white;
        }
        timer.text = Contacts.formatTime(value);
    }

    public void SetTimerCountDown(float value)
    {
        timerCountDown.text = Contacts.formatTime(value);
    }
}
