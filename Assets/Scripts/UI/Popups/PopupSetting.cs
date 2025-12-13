using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : PopupUI
{
    [SerializeField] Button buttonClose;
    [SerializeField] Button buttonSoundFX;
    [SerializeField] Button buttonSoundMusic;
    [SerializeField] bool onSoundFX;
    [SerializeField] bool onSoundMusic;

    private void Start()
    {
        AddEventListener();
    }

    private void AddEventListener()
    {
        buttonClose.onClick.AddListener(CloseClick);
        buttonSoundFX.onClick.AddListener(SoundFXClick);
        buttonSoundMusic.onClick.AddListener(SoundMusicClick);

    }

    private void SoundMusicClick()
    {
        AudioManager.Instance.StopMusic();
    }

    private void SoundFXClick()
    {
        AudioManager.Instance.StopSFX();
    }

    private void CloseClick()
    {
        Hide();
    }
}
