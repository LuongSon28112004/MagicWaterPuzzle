using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] TextMeshProUGUI helpButtonText;
    [SerializeField] Button gilfCodeButton;
    [SerializeField] Button joinButton;

    [Header("Confirm Logout")]
    [SerializeField] GameObject confirmLogoutPanel;
    [SerializeField] Button confirmLogoutOkButton;
    [SerializeField] Button confirmLogoutCancelButton;

    [Header("Language Setup")]
    [SerializeField] Button buttonEnglish;
    [SerializeField] Button buttonVietnamese;



    private void Start()
    {
        AddEventListener();
        onSoundFX = AudioManager.AudioSoundSetting;
        onSoundMusic = AudioManager.AudioMusicSetting;
        onVibrate = AudioManager.AudioVibrateSetting;
        InitIconFX();
        UpdateLoginButtonState();
        UpdateLanguageSelectionUI();
        UpdateConfirmLogoutTexts();

        // Ẩn panel confirm logout ban đầu
        if (confirmLogoutPanel != null)
            confirmLogoutPanel.SetActive(false);
    }

    private void OnEnable()
    {
        // Mỗi lần mở popup, cập nhật lại trạng thái nút Login/Logout và ngôn ngữ
        UpdateLoginButtonState();
        UpdateLanguageSelectionUI();
        UpdateConfirmLogoutTexts();
    }

    /// <summary>
    /// Cập nhật text và trạng thái nút Login/Logout dựa trên trạng thái đăng nhập Google và Ngôn ngữ từ CSV
    /// </summary>
    private void UpdateLoginButtonState()
    {
        // Tự động tìm Text component nếu chưa được kéo thả trong Inspector
        if (helpButtonText == null && helpButton != null)
        {
            helpButtonText = helpButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (helpButtonText == null) return;

        if (UserDataFirebaseManager.Instance != null && UserDataFirebaseManager.Instance.IsGoogleLinked)
        {
            helpButtonText.text = LanguageManager.Instance.GetTranslation("logout");
        }
        else
        {
            helpButtonText.text = LanguageManager.Instance.GetTranslation("login_google");
        }
    }

    private void InitIconFX()
    {
        ImageSoundSlack.gameObject.SetActive(!onSoundFX);
        ImageMusicSlack.gameObject.SetActive(!onSoundMusic);
        ImageVirbateSlack.gameObject.SetActive(!onVibrate);
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

        if (buttonEnglish != null)
            buttonEnglish.onClick.AddListener(EnglishLanguageClick);
        if (buttonVietnamese != null)
            buttonVietnamese.onClick.AddListener(VietnameseLanguageClick);
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
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);

        if (UserDataFirebaseManager.Instance.IsGoogleLinked)
        {
            // Đã đăng nhập → Hiện popup xác nhận đăng xuất
            ShowConfirmLogout();
        }
        else
        {
            // Chưa đăng nhập → Login with Google
            await UserDataFirebaseManager.Instance
                .LinkGoogleAccount((res) =>
                {
                    if (res) UIManager.Instance.NotifyContent("Login Success");
                    else UIManager.Instance.NotifyContent("Login Failed");
                    UpdateLoginButtonState();
                });
        }
    }

    /// <summary>
    /// Hiện panel xác nhận đăng xuất
    /// </summary>
    private void ShowConfirmLogout()
    {
        if (confirmLogoutPanel == null) return;

        confirmLogoutPanel.SetActive(true);

        // Gỡ listener cũ để tránh đăng ký nhiều lần
        confirmLogoutOkButton.onClick.RemoveAllListeners();
        confirmLogoutCancelButton.onClick.RemoveAllListeners();

        confirmLogoutOkButton.onClick.AddListener(OnConfirmLogoutOk);
        confirmLogoutCancelButton.onClick.AddListener(OnConfirmLogoutCancel);
    }

    /// <summary>
    /// Người dùng ấn OK → Thực hiện đăng xuất
    /// </summary>
    private void OnConfirmLogoutOk()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);

        if (confirmLogoutPanel != null)
            confirmLogoutPanel.SetActive(false);

        UserDataFirebaseManager.Instance.LogoutAndCreateNewAccount((res) =>
        {
            UpdateLoginButtonState();
        });
    }

    /// <summary>
    /// Người dùng ấn Cancel → Đóng panel confirm
    /// </summary>
    private void OnConfirmLogoutCancel()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);

        if (confirmLogoutPanel != null)
            confirmLogoutPanel.SetActive(false);
    }

    private void VirbrateClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        onVibrate = !onVibrate;
        ImageVirbateSlack.gameObject.SetActive(!onVibrate);
        AudioManager.AudioVibrateSetting = onVibrate;
        PlayerPrefs.Save();
    }

    private void SoundMusicClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        onSoundMusic = !onSoundMusic;
        ImageMusicSlack.gameObject.SetActive(!onSoundMusic);
        AudioManager.AudioMusicSetting = onSoundMusic;
        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.volume = onSoundMusic ? 0.3f * AudioManager.Instance.Ratio_Sound : 0;
            if (onSoundMusic)
            {
                AudioManager.Instance.ResumeMusic();
            }
            else
            {
                AudioManager.Instance.PauseMusic();
            }
        }
        PlayerPrefs.Save();
    }

    private void SoundFXClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        onSoundFX = !onSoundFX;
        ImageSoundSlack.gameObject.SetActive(!onSoundFX);
        AudioManager.AudioSoundSetting = onSoundFX;
        PlayerPrefs.Save();
    }

    private void CloseClick()
    {
        Hide();
    }

    // ==================== XỬ LÝ CHUYỂN ĐỔI NGÔN NGỮ ====================

    private void EnglishLanguageClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        LanguageManager.Instance.SetLanguage(GameLanguage.English);
        UpdateLanguageSelectionUI();
        UpdateLoginButtonState();
        UpdateConfirmLogoutTexts();
    }

    private void VietnameseLanguageClick()
    {
        AudioManager.Instance.PlayOneShot("ClickButton", 1f);
        LanguageManager.Instance.SetLanguage(GameLanguage.Vietnamese);
        UpdateLanguageSelectionUI();
        UpdateLoginButtonState();
        UpdateConfirmLogoutTexts();
    }

    /// <summary>
    /// Thay đổi màu sắc/độ sáng của các nút ngôn ngữ để biết nút nào đang được chọn
    /// </summary>
    private void UpdateLanguageSelectionUI()
    {
        if (buttonEnglish == null || buttonVietnamese == null) return;

        Image engImg = buttonEnglish.GetComponent<Image>();
        Image vieImg = buttonVietnamese.GetComponent<Image>();

        if (engImg == null || vieImg == null) return;

        bool isVietnamese = LanguageManager.Instance.CurrentLanguage == GameLanguage.Vietnamese;

        // Nút được chọn -> sáng hơn
        Color activeColor = new Color(1f, 1f, 1f, 1f);

        // Nút không được chọn -> tối hơn một chút
        Color inactiveColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        engImg.color = !isVietnamese ? activeColor : inactiveColor;
        vieImg.color = isVietnamese ? activeColor : inactiveColor;
    }

    /// <summary>
    /// Dịch động toàn bộ text trong bảng Xác nhận Đăng xuất từ CSV
    /// </summary>
    private void UpdateConfirmLogoutTexts()
    {
        if (confirmLogoutPanel == null) return;

        Text titleText = confirmLogoutPanel.transform.Find("DialogBox/Title")?.GetComponent<Text>();
        Text messageText = confirmLogoutPanel.transform.Find("DialogBox/Message")?.GetComponent<Text>();
        Text okText = confirmLogoutPanel.transform.Find("DialogBox/OkButton/Text")?.GetComponent<Text>();
        Text cancelText = confirmLogoutPanel.transform.Find("DialogBox/CancelButton/Text")?.GetComponent<Text>();

        if (titleText != null)
            titleText.text = LanguageManager.Instance.GetTranslation("confirm_title");

        if (messageText != null)
            messageText.text = LanguageManager.Instance.GetTranslation("confirm_msg");

        if (okText != null)
            okText.text = LanguageManager.Instance.GetTranslation("confirm_ok");

        if (cancelText != null)
            cancelText.text = LanguageManager.Instance.GetTranslation("confirm_cancel");
    }
}
