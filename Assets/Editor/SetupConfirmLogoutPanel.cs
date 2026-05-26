using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// Editor script to setup localization & Confirm Logout Panel
/// Menu: Tools > Setup Confirm Logout Panel
/// </summary>
public class SetupConfirmLogoutPanel
{
    [MenuItem("Tools/Setup Confirm Logout Panel")]
    public static void Setup()
    {
        string prefabPath = "Assets/Resources/UI/Popups/PopupSetting.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at: {prefabPath}");
            return;
        }

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        PopupSetting popupSetting = prefabRoot.GetComponent<PopupSetting>();
        if (popupSetting == null)
        {
            Debug.LogError("Component PopupSetting not found!");
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            return;
        }

        Transform existing = prefabRoot.transform.Find("ConfirmLogoutPanel");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        Font defaultFont = null;
        try
        {
            defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch
        {
            try
            {
                defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            catch
            {
                Debug.LogWarning("Default font not found.");
            }
        }

        // === Confirm Logout Overlay ===
        GameObject panel = new GameObject("ConfirmLogoutPanel");
        panel.transform.SetParent(prefabRoot.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image overlayImage = panel.AddComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0.6f);
        overlayImage.raycastTarget = true;

        GameObject dialogBox = new GameObject("DialogBox");
        dialogBox.transform.SetParent(panel.transform, false);
        RectTransform dialogRect = dialogBox.AddComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
        dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
        dialogRect.pivot = new Vector2(0.5f, 0.5f);
        dialogRect.sizeDelta = new Vector2(700f, 400f);

        Image dialogImage = dialogBox.AddComponent<Image>();
        dialogImage.color = new Color(0.15f, 0.15f, 0.25f, 1f);

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(dialogBox.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.offsetMin = new Vector2(30f, -90f);
        titleRect.offsetMax = new Vector2(-30f, -20f);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.font = defaultFont;
        titleText.text = "Xác nhận đăng xuất";
        titleText.fontSize = 42;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;

        GameObject messageObj = new GameObject("Message");
        messageObj.transform.SetParent(dialogBox.transform, false);
        RectTransform msgRect = messageObj.AddComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0f, 0.35f);
        msgRect.anchorMax = new Vector2(1f, 0.75f);
        msgRect.offsetMin = new Vector2(30f, 0f);
        msgRect.offsetMax = new Vector2(-30f, 0f);

        Text msgText = messageObj.AddComponent<Text>();
        msgText.font = defaultFont;
        msgText.text = "Bạn có chắc chắn muốn đăng xuất không?\nDữ liệu hiện tại sẽ được thay bằng tài khoản mới.";
        msgText.fontSize = 30;
        msgText.color = new Color(0.85f, 0.85f, 0.85f, 1f);
        msgText.alignment = TextAnchor.MiddleCenter;
        msgText.horizontalOverflow = HorizontalWrapMode.Wrap;
        msgText.verticalOverflow = VerticalWrapMode.Overflow;

        GameObject okBtnObj = new GameObject("OkButton");
        okBtnObj.transform.SetParent(dialogBox.transform, false);
        RectTransform okRect = okBtnObj.AddComponent<RectTransform>();
        okRect.anchorMin = new Vector2(0.05f, 0.05f);
        okRect.anchorMax = new Vector2(0.45f, 0.3f);
        okRect.offsetMin = Vector2.zero;
        okRect.offsetMax = Vector2.zero;

        Image okBtnImage = okBtnObj.AddComponent<Image>();
        okBtnImage.color = new Color(0.2f, 0.7f, 0.3f, 1f);
        Button okButton = okBtnObj.AddComponent<Button>();
        okButton.targetGraphic = okBtnImage;

        GameObject okTextObj = new GameObject("Text");
        okTextObj.transform.SetParent(okBtnObj.transform, false);
        RectTransform okTextRect = okTextObj.AddComponent<RectTransform>();
        okTextRect.anchorMin = Vector2.zero;
        okTextRect.anchorMax = Vector2.one;
        okTextRect.offsetMin = Vector2.zero;
        okTextRect.offsetMax = Vector2.zero;

        Text okText = okTextObj.AddComponent<Text>();
        okText.font = defaultFont;
        okText.text = "Đồng ý";
        okText.fontSize = 34;
        okText.fontStyle = FontStyle.Bold;
        okText.color = Color.white;
        okText.alignment = TextAnchor.MiddleCenter;

        GameObject cancelBtnObj = new GameObject("CancelButton");
        cancelBtnObj.transform.SetParent(dialogBox.transform, false);
        RectTransform cancelRect = cancelBtnObj.AddComponent<RectTransform>();
        cancelRect.anchorMin = new Vector2(0.55f, 0.05f);
        cancelRect.anchorMax = new Vector2(0.95f, 0.3f);
        cancelRect.offsetMin = Vector2.zero;
        cancelRect.offsetMax = Vector2.zero;

        Image cancelBtnImage = cancelBtnObj.AddComponent<Image>();
        cancelBtnImage.color = new Color(0.7f, 0.25f, 0.25f, 1f);
        Button cancelButton = cancelBtnObj.AddComponent<Button>();
        cancelButton.targetGraphic = cancelBtnImage;

        GameObject cancelTextObj = new GameObject("Text");
        cancelTextObj.transform.SetParent(cancelBtnObj.transform, false);
        RectTransform cancelTextRect = cancelTextObj.AddComponent<RectTransform>();
        cancelTextRect.anchorMin = Vector2.zero;
        cancelTextRect.anchorMax = Vector2.one;
        cancelTextRect.offsetMin = Vector2.zero;
        cancelTextRect.offsetMax = Vector2.zero;

        Text cancelText = cancelTextObj.AddComponent<Text>();
        cancelText.font = defaultFont;
        cancelText.text = "Thoát";
        cancelText.fontSize = 34;
        cancelText.fontStyle = FontStyle.Bold;
        cancelText.color = Color.white;
        cancelText.alignment = TextAnchor.MiddleCenter;

        // === Language Buttons Container ===
        Transform existingLang = prefabRoot.transform.Find("LanguagePanel");
        if (existingLang != null)
        {
            Object.DestroyImmediate(existingLang.gameObject);
        }

        GameObject langPanel = new GameObject("LanguagePanel");
        langPanel.transform.SetParent(prefabRoot.transform, false);
        RectTransform langRect = langPanel.AddComponent<RectTransform>();
        langRect.anchorMin = new Vector2(0.5f, 0.5f);
        langRect.anchorMax = new Vector2(0.5f, 0.5f);
        langRect.pivot = new Vector2(0.5f, 0.5f);
        langRect.sizeDelta = new Vector2(500f, 100f);
        langRect.anchoredPosition = new Vector2(0f, -280f);

        GameObject engBtnObj = new GameObject("EnglishButton");
        engBtnObj.transform.SetParent(langPanel.transform, false);
        RectTransform engRect = engBtnObj.AddComponent<RectTransform>();
        engRect.anchorMin = new Vector2(0f, 0.5f);
        engRect.anchorMax = new Vector2(0f, 0.5f);
        engRect.pivot = new Vector2(0f, 0.5f);
        engRect.sizeDelta = new Vector2(220f, 75f);
        engRect.anchoredPosition = new Vector2(10f, 0f);

        Image engImage = engBtnObj.AddComponent<Image>();
        engImage.color = new Color(0.2f, 0.7f, 0.3f, 1f);
        Button engButton = engBtnObj.AddComponent<Button>();
        engButton.targetGraphic = engImage;

        GameObject engTextObj = new GameObject("Text");
        engTextObj.transform.SetParent(engBtnObj.transform, false);
        RectTransform engTextRect = engTextObj.AddComponent<RectTransform>();
        engTextRect.anchorMin = Vector2.zero;
        engTextRect.anchorMax = Vector2.one;
        engTextRect.offsetMin = Vector2.zero;
        engTextRect.offsetMax = Vector2.zero;

        Text engText = engTextObj.AddComponent<Text>();
        engText.font = defaultFont;
        engText.text = "English";
        engText.fontSize = 28;
        engText.fontStyle = FontStyle.Bold;
        engText.color = Color.white;
        engText.alignment = TextAnchor.MiddleCenter;

        GameObject vieBtnObj = new GameObject("VietnameseButton");
        vieBtnObj.transform.SetParent(langPanel.transform, false);
        RectTransform vieRect = vieBtnObj.AddComponent<RectTransform>();
        vieRect.anchorMin = new Vector2(1f, 0.5f);
        vieRect.anchorMax = new Vector2(1f, 0.5f);
        vieRect.pivot = new Vector2(1f, 0.5f);
        vieRect.sizeDelta = new Vector2(220f, 75f);
        vieRect.anchoredPosition = new Vector2(-10f, 0f);

        Image vieImage = vieBtnObj.AddComponent<Image>();
        vieImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        Button vieButton = vieBtnObj.AddComponent<Button>();
        vieButton.targetGraphic = vieImage;

        GameObject vieTextObj = new GameObject("Text");
        vieTextObj.transform.SetParent(vieBtnObj.transform, false);
        RectTransform vieTextRect = vieTextObj.AddComponent<RectTransform>();
        vieTextRect.anchorMin = Vector2.zero;
        vieTextRect.anchorMax = Vector2.one;
        vieTextRect.offsetMin = Vector2.zero;
        vieTextRect.offsetMax = Vector2.zero;

        Text vieText = vieTextObj.AddComponent<Text>();
        vieText.font = defaultFont;
        vieText.text = "Tiếng Việt";
        vieText.fontSize = 28;
        vieText.fontStyle = FontStyle.Bold;
        vieText.color = Color.white;
        vieText.alignment = TextAnchor.MiddleCenter;

        // === Auto-localize existing text elements ===
        AutoLocalizeTexts(prefabRoot);

        // === Serialized References ===
        SerializedObject so = new SerializedObject(popupSetting);
        SerializedProperty propPanel = so.FindProperty("confirmLogoutPanel");
        SerializedProperty propOk = so.FindProperty("confirmLogoutOkButton");
        SerializedProperty propCancel = so.FindProperty("confirmLogoutCancelButton");
        SerializedProperty propEng = so.FindProperty("buttonEnglish");
        SerializedProperty propVie = so.FindProperty("buttonVietnamese");

        if (propPanel != null) propPanel.objectReferenceValue = panel;
        if (propOk != null) propOk.objectReferenceValue = okButton;
        if (propCancel != null) propCancel.objectReferenceValue = cancelButton;
        if (propEng != null) propEng.objectReferenceValue = engButton;
        if (propVie != null) propVie.objectReferenceValue = vieButton;

        so.ApplyModifiedProperties();
        panel.SetActive(false);

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        Debug.Log("<color=green>[Setup] Fully set up PopupSetting prefab with localized elements!</color>");
    }

    private static void AutoLocalizeTexts(GameObject root)
    {
        // 1. Scan UI.Text components
        Text[] uiTexts = root.GetComponentsInChildren<Text>(true);
        foreach (Text txt in uiTexts)
        {
            // Skip confirm dialogue children and language buttons so we don't overwrite them
            if (txt.transform.IsChildOf(root.transform.Find("ConfirmLogoutPanel")) ||
                txt.transform.IsChildOf(root.transform.Find("LanguagePanel")))
                continue;

            string content = txt.text.Trim().ToLower();
            AddLocalization(txt.gameObject, content);
        }

        // 2. Scan TMPro components
        TextMeshProUGUI[] tmpTexts = root.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI txt in tmpTexts)
        {
            if (txt.transform.IsChildOf(root.transform.Find("ConfirmLogoutPanel")) ||
                txt.transform.IsChildOf(root.transform.Find("LanguagePanel")))
                continue;

            string content = txt.text.Trim().ToLower();
            AddLocalization(txt.gameObject, content);
        }
    }

    private static void AddLocalization(GameObject go, string rawText)
    {
        string keyName = "";
        string clean = rawText.ToLower().Trim();

        if (clean.Contains("sound") || clean.Contains("âm thanh") || clean.Contains("am thanh"))
        {
            keyName = "sound";
        }
        else if (clean.Contains("music") || clean.Contains("nhạc") || clean.Contains("nhac"))
        {
            keyName = "music";
        }
        else if (clean.Contains("vibrat") || clean.Contains("rung"))
        {
            keyName = "vibrate";
        }
        else if (clean.Contains("gift") || clean.Contains("gilf") || clean.Contains("mã") || clean.Contains("code"))
        {
            keyName = "giftcode";
        }
        else if (clean.Contains("join") || clean.Contains("tham gia"))
        {
            keyName = "join";
        }
        else if (clean.Contains("help") || clean.Contains("support") || clean.Contains("trợ giúp") || clean.Contains("hỗ trợ"))
        {
            return;
        }
        else if (clean.Contains("setting") || clean.Contains("cài đặt") || clean.Contains("cai dat"))
        {
            keyName = "settings";
        }
        else
        {
            return;
        }

        LocalizedText loc = go.GetComponent<LocalizedText>();
        if (loc == null)
        {
            loc = go.AddComponent<LocalizedText>();
        }

        // Gán key bằng SerializedObject để Unity lưu chính xác vào Prefab
        SerializedObject soLoc = new SerializedObject(loc);
        SerializedProperty propKey = soLoc.FindProperty("key");
        if (propKey != null)
        {
            propKey.stringValue = keyName;
        }
        soLoc.ApplyModifiedProperties();
    }
}
