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
    [Header("Image src")]
    [SerializeField] Image ImageSoundSlack;
    [SerializeField] Image ImageMusicSlack;
    [SerializeField] Image ImageVirbateSlack;



    private void Start()
    {
        AddEventListener();
        onSoundFX = AudioManager.AudioSoundSetting;
        onSoundMusic = AudioManager.AudioMusicSetting;
    }

    private void AddEventListener()
    {
        buttonClose.onClick.AddListener(CloseClick);
        buttonSoundFX.onClick.AddListener(SoundFXClick);
        buttonSoundMusic.onClick.AddListener(SoundMusicClick);

    }

    private void SoundMusicClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        if (onSoundMusic)
        {
            ImageMusicSlack.gameObject.SetActive(true);
            AudioManager.Instance.PauseMusic();
            onSoundMusic = false;
            PlayerPrefs.SetInt("audio_music_setting", 0);
        }
        else
        {
            ImageMusicSlack.gameObject.SetActive(false);
            AudioManager.Instance.ResumeMusic();
            onSoundMusic = true;
            PlayerPrefs.SetInt("audio_music_setting", 1);
        }
    }

    private void SoundFXClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        if (onSoundFX)
        {
            ImageSoundSlack.gameObject.SetActive(true);
            AudioManager.Instance.PauseSound();
            onSoundFX = false;
            PlayerPrefs.SetInt("audio_sound_setting", 0);
        }
        else
        {
            ImageSoundSlack.gameObject.SetActive(false);
            AudioManager.Instance.ResumeSound();
            onSoundFX = true;
            PlayerPrefs.SetInt("audio_sound_setting", 1);
        }
    }

    private void CloseClick()
    {
        Hide();
    }
}
