using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupConfirm : PopupUI
{
    [SerializeField] private Text txtMessage;
    [SerializeField] private Button btnYes;
    [SerializeField] private Button btnNo;

    private Action onYesClick;

    public void ShowConfirm(string message, Action onYes)
    {
        if (txtMessage != null)
        {
            txtMessage.text = message;
        }
        onYesClick = onYes;

        if (btnYes != null)
        {
            btnYes.onClick.RemoveAllListeners();
            btnYes.onClick.AddListener(OnYesClicked);
        }

        if (btnNo != null)
        {
            btnNo.onClick.RemoveAllListeners();
            btnNo.onClick.AddListener(OnNoClicked);
        }
    }

    private void OnYesClicked()
    {
        onYesClick?.Invoke();
        Hide();
    }

    private void OnNoClicked()
    {
        Hide();
    }
}
