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
    [Header("Help and Support")]
    [SerializeField] Button helpButton;
    [SerializeField] Button gilfCodeButton;
    [SerializeField] Button joinButton;



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
        helpButton.onClick.AddListener(HelpClick);
        gilfCodeButton.onClick.AddListener(GilfClick);
        joinButton.onClick.AddListener(JoinClick);
    }

    private void JoinClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
    }

    private void GilfClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
    }

    public async void HelpClick()
    {
        await UserDataFirebaseManager.Instance
            .LinkGoogleAccount((res) =>
            {
                if (res) UIManager.Instance.NotifyContent("Login Success");
                else UIManager.Instance.NotifyContent("Login Failed");
            });
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
