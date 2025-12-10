using System;
using System.Collections;
using DG.Tweening.Core.Enums;
using UnityEngine;

public enum ModeOutgame
{
    BACK_TO_MENU,
    RESTART,
}

public class PopupQuitBreak : PopupUI
{
    [SerializeField] private ModeOutgame modeOutgame;

    public void StartModeQuitBreak(ModeOutgame modeOutgame)
    {
        this.modeOutgame = modeOutgame;
        StartCoroutine(ProcessMode());
    }

    private IEnumerator ProcessMode()
    {
        yield return new WaitForSeconds(0.5f);
        if (modeOutgame == ModeOutgame.BACK_TO_MENU)
        {
            GameManager.Instance.BackToMenu();
        }

        else if (modeOutgame == ModeOutgame.RESTART)
        {
            GameManager.Instance.StartGame();
        }
    }
}
