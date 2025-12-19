using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : PopupUI
{
    [SerializeField] Button buttonClose;
    [SerializeField] Button buttonSoundFX;
    [SerializeField] Button buttonSoundMusic;
    [SerializeField] Button buttonVibrate;
    [SerializeField] bool onSoundFX;
    [SerializeField] bool onSoundMusic;
    [SerializeField] bool onVibrate;
    [Header("Image src")]
    [SerializeField] Image ImageSoundSlack;
    [SerializeField] Image ImageMusicSlack;
    [SerializeField] Image ImageVirbateSlack;



    private void Start()
    {
        AddEventListener();
        onSoundFX = AudioManager.AudioSoundSetting;
        onSoundMusic = AudioManager.AudioMusicSetting;
        onVibrate = AudioManager.AudioVibrateSetting;
        InitIconFX();
    }

    private void InitIconFX()
    {
        if (!onSoundFX)
        {
            ImageSoundSlack.gameObject.SetActive(true);
        }

        if (!onSoundMusic)
        {
            ImageMusicSlack.gameObject.SetActive(true);
        }

        if (!onVibrate)
        {
            ImageVirbateSlack.gameObject.SetActive(true);
        }
    }

    private void AddEventListener()
    {
        buttonClose.onClick.AddListener(CloseClick);
        buttonSoundFX.onClick.AddListener(SoundFXClick);
        buttonSoundMusic.onClick.AddListener(SoundMusicClick);
        buttonVibrate.onClick.AddListener(VirbrateClick);

    }

    private void VirbrateClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        if (onVibrate)
        {
            ImageVirbateSlack.gameObject.SetActive(true);
            onVibrate = false;
            PlayerPrefs.SetInt("vibrate_setting", 0);
        }
        else
        {
            ImageVirbateSlack.gameObject.SetActive(false);
            onVibrate = true;
            PlayerPrefs.SetInt("vibrate_setting", 1);
        }

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
