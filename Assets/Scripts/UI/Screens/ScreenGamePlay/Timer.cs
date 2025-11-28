using NUnit.Framework;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;
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
}
