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

    public void SetTimer(string value)
    {
        timer.text = value;
    }

    public void SetTimerCountDown(string value)
    {
        timerCountDown.text = value;
    }
}
