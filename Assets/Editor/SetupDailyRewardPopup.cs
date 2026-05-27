using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class SetupDailyRewardPopup
{
    private const string PREFAB_PATH = "Assets/Resources/UI/Popups/PopupDailyReward.prefab";

    [MenuItem("Tools/Setup Daily Reward Popup")]
    public static void Setup()
    {
        string folder = "Assets/Resources/UI/Popups";
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();
        }

        // Create temporary Root object
        GameObject root = new GameObject("PopupDailyReward");
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        CanvasRenderer rootCR = root.AddComponent<CanvasRenderer>();
        Image rootImage = root.AddComponent<Image>();
        rootImage.color = new Color(0f, 0f, 0f, 0.75f);
        rootImage.raycastTarget = true;

        PopupDailyReward popupComp = root.AddComponent<PopupDailyReward>();

        // Load resources
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Texture/UI/bg2.png");
        Sprite closeSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Texture/UI/X.png");
        Sprite checkSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Texture/UI/V 1.png");
        Sprite borderSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Texture/UI/Bgline2.png");
        Sprite highlightSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Texture/UI/bg_stargreen.png");
        Font mainFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/_Game/Font/royal_kingdom_font.otf");
        if (mainFont == null)
        {
            try
            {
                mainFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            catch
            {
                try
                {
                    mainFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }
                catch
                {
                    Debug.LogWarning("Default font not found.");
                }
            }
        }

        // Main Popup container
        GameObject mainPopup = new GameObject("MainPopup");
        mainPopup.transform.SetParent(root.transform, false);
        RectTransform mainRect = mainPopup.AddComponent<RectTransform>();
        mainRect.anchorMin = new Vector2(0.5f, 0.5f);
        mainRect.anchorMax = new Vector2(0.5f, 0.5f);
        mainRect.pivot = new Vector2(0.5f, 0.5f);
        mainRect.sizeDelta = new Vector2(800f, 960f);

        CanvasRenderer mainCR = mainPopup.AddComponent<CanvasRenderer>();
        Image mainImage = mainPopup.AddComponent<Image>();
        if (bgSprite != null)
        {
            mainImage.sprite = bgSprite;
            mainImage.type = Image.Type.Sliced;
        }
        else
        {
            mainImage.color = new Color(0.12f, 0.12f, 0.22f, 1f);
        }

        // Title text
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(mainPopup.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -80f);
        titleRect.sizeDelta = new Vector2(-100f, 80f);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "DAILY REWARDS";
        titleText.fontSize = 52;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        if (mainFont != null) titleText.font = mainFont;

        // Subtitle text
        GameObject subTitleObj = new GameObject("Subtitle");
        subTitleObj.transform.SetParent(mainPopup.transform, false);
        RectTransform subTitleRect = subTitleObj.AddComponent<RectTransform>();
        subTitleRect.anchorMin = new Vector2(0f, 1f);
        subTitleRect.anchorMax = new Vector2(1f, 1f);
        subTitleRect.pivot = new Vector2(0.5f, 1f);
        subTitleRect.anchoredPosition = new Vector2(0f, -150f);
        subTitleRect.sizeDelta = new Vector2(-100f, 50f);

        Text subTitleText = subTitleObj.AddComponent<Text>();
        subTitleText.text = "Login every day to receive rewards!";
        subTitleText.fontSize = 24;
        subTitleText.alignment = TextAnchor.MiddleCenter;
        subTitleText.color = new Color(0.75f, 0.85f, 0.95f, 1f);
        if (mainFont != null) subTitleText.font = mainFont;

        // Close Button
        GameObject closeBtnObj = new GameObject("ButtonClose");
        closeBtnObj.transform.SetParent(mainPopup.transform, false);
        RectTransform closeRect = closeBtnObj.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.pivot = new Vector2(0.5f, 0.5f);
        closeRect.anchoredPosition = new Vector2(-45f, -45f);
        closeRect.sizeDelta = new Vector2(70f, 70f);

        Image closeImage = closeBtnObj.AddComponent<Image>();
        if (closeSprite != null)
        {
            closeImage.sprite = closeSprite;
        }
        else
        {
            closeImage.color = Color.red;
        }

        Button closeButton = closeBtnObj.AddComponent<Button>();
        closeButton.targetGraphic = closeImage;

        // Slots Container
        GameObject slotsContainer = new GameObject("SlotsContainer");
        slotsContainer.transform.SetParent(mainPopup.transform, false);
        RectTransform slotsContainerRect = slotsContainer.AddComponent<RectTransform>();
        slotsContainerRect.anchorMin = new Vector2(0.5f, 0.5f);
        slotsContainerRect.anchorMax = new Vector2(0.5f, 0.5f);
        slotsContainerRect.pivot = new Vector2(0.5f, 0.5f);
        slotsContainerRect.anchoredPosition = new Vector2(0f, -60f);
        slotsContainerRect.sizeDelta = new Vector2(720f, 560f);

        // Define slot layouts
        // Row 1: Days 1, 2, 3, 4
        // Row 2: Days 5, 6, 7 (Day 7 is larger)
        Vector2[] positions = new Vector2[]
        {
            new Vector2(-262.5f, 130f),  // Day 1
            new Vector2(-87.5f, 130f),   // Day 2
            new Vector2(87.5f, 130f),    // Day 3
            new Vector2(262.5f, 130f),   // Day 4
            new Vector2(-255f, -130f),   // Day 5
            new Vector2(-70f, -130f),    // Day 6
            new Vector2(185f, -130f)     // Day 7
        };

        Vector2[] sizes = new Vector2[]
        {
            new Vector2(150f, 220f),     // Day 1
            new Vector2(150f, 220f),     // Day 2
            new Vector2(150f, 220f),     // Day 3
            new Vector2(150f, 220f),     // Day 4
            new Vector2(160f, 220f),     // Day 5
            new Vector2(160f, 220f),     // Day 6
            new Vector2(300f, 220f)      // Day 7
        };

        DailyRewardSlotUI[] slotUIs = new DailyRewardSlotUI[7];
        for (int i = 0; i < 7; i++)
        {
            bool isDay7 = (i == 6);
            string dayName = $"Day {i + 1}";
            slotUIs[i] = CreateSlotUI(
                slotsContainer.transform,
                dayName,
                positions[i],
                sizes[i],
                mainFont,
                borderSprite,
                checkSprite,
                highlightSprite != null ? highlightSprite : borderSprite,
                isDay7
            );
        }

        // Connect references using SerializedObject
        SerializedObject so = new SerializedObject(popupComp);
        SerializedProperty propSlots = so.FindProperty("slots");
        SerializedProperty propTitle = so.FindProperty("titleText");
        SerializedProperty propSubTitle = so.FindProperty("subTitleText");
        SerializedProperty propClose = so.FindProperty("ButtonClose");
        SerializedProperty propAnim = so.FindProperty("animType");
        SerializedProperty propMain = so.FindProperty("mainPopUp");

        if (propTitle != null) propTitle.objectReferenceValue = titleText;
        if (propSubTitle != null) propSubTitle.objectReferenceValue = subTitleText;
        if (propClose != null) propClose.objectReferenceValue = closeButton;
        if (propMain != null) propMain.objectReferenceValue = mainRect;
        if (propAnim != null) propAnim.enumValueIndex = 2; // ScalePunch

        if (propSlots != null)
        {
            propSlots.ClearArray();
            propSlots.arraySize = 7;
            for (int i = 0; i < 7; i++)
            {
                SerializedProperty propSlot = propSlots.GetArrayElementAtIndex(i);
                propSlot.FindPropertyRelative("button").objectReferenceValue = slotUIs[i].button;
                propSlot.FindPropertyRelative("icon").objectReferenceValue = slotUIs[i].icon;
                propSlot.FindPropertyRelative("amountText").objectReferenceValue = slotUIs[i].amountText;
                propSlot.FindPropertyRelative("dayLabelText").objectReferenceValue = slotUIs[i].dayLabelText;
                propSlot.FindPropertyRelative("claimedOverlay").objectReferenceValue = slotUIs[i].claimedOverlay;
                propSlot.FindPropertyRelative("todayHighlight").objectReferenceValue = slotUIs[i].todayHighlight;
            }
        }
        so.ApplyModifiedProperties();

        // Save as Prefab
        PrefabUtility.SaveAsPrefabAsset(root, PREFAB_PATH);
        Object.DestroyImmediate(root);

        Debug.Log($"<color=green>[Setup] Successfully created PopupDailyReward prefab at {PREFAB_PATH}!</color>");
    }

    private static DailyRewardSlotUI CreateSlotUI(Transform parent, string name, Vector2 position, Vector2 size, Font font, Sprite bgSprite, Sprite checkSprite, Sprite highlightSprite, bool isBig)
    {
        GameObject slotObj = new GameObject(name);
        slotObj.transform.SetParent(parent, false);
        RectTransform rect = slotObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image bgImage = slotObj.AddComponent<Image>();
        if (bgSprite != null)
        {
            bgImage.sprite = bgSprite;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = isBig ? new Color(1f, 0.92f, 0.7f, 1f) : new Color(0.9f, 0.9f, 0.95f, 1f);
        }
        else
        {
            bgImage.color = isBig ? new Color(0.35f, 0.25f, 0.5f, 1f) : new Color(0.18f, 0.18f, 0.28f, 1f);
        }

        Button button = slotObj.AddComponent<Button>();
        button.targetGraphic = bgImage;

        // Today Highlight (outline or glow)
        GameObject highlight = new GameObject("TodayHighlight");
        highlight.transform.SetParent(slotObj.transform, false);
        RectTransform highlightRect = highlight.AddComponent<RectTransform>();
        highlightRect.anchorMin = Vector2.zero;
        highlightRect.anchorMax = Vector2.one;
        highlightRect.offsetMin = new Vector2(-6f, -6f);
        highlightRect.offsetMax = new Vector2(6f, 6f);
        Image highlightImage = highlight.AddComponent<Image>();
        if (highlightSprite != null)
        {
            highlightImage.sprite = highlightSprite;
            highlightImage.type = Image.Type.Sliced;
            highlightImage.color = new Color(1f, 0.8f, 0f, 1f);
        }
        else
        {
            highlightImage.color = new Color(1f, 0.8f, 0.1f, 1f);
        }
        highlight.SetActive(false);

        // Day Label Text
        GameObject dayLabelObj = new GameObject("DayLabel");
        dayLabelObj.transform.SetParent(slotObj.transform, false);
        RectTransform dayLabelRect = dayLabelObj.AddComponent<RectTransform>();
        dayLabelRect.anchorMin = new Vector2(0f, 1f);
        dayLabelRect.anchorMax = new Vector2(1f, 1f);
        dayLabelRect.pivot = new Vector2(0.5f, 1f);
        dayLabelRect.anchoredPosition = new Vector2(0f, -12f);
        dayLabelRect.sizeDelta = new Vector2(-20f, 35f);

        Text dayText = dayLabelObj.AddComponent<Text>();
        dayText.text = name;
        dayText.fontSize = isBig ? 24 : 20;
        dayText.fontStyle = FontStyle.Bold;
        dayText.alignment = TextAnchor.MiddleCenter;
        dayText.color = isBig ? new Color(0.2f, 0.1f, 0f, 1f) : new Color(0.1f, 0.1f, 0.2f, 1f);
        if (font != null) dayText.font = font;

        // Icon Image
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(slotObj.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(0f, 5f);
        iconRect.sizeDelta = isBig ? new Vector2(110f, 110f) : new Vector2(80f, 80f);

        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.color = Color.white;
        iconImage.preserveAspect = true;

        // Amount Text
        GameObject amountObj = new GameObject("Amount");
        amountObj.transform.SetParent(slotObj.transform, false);
        RectTransform amountRect = amountObj.AddComponent<RectTransform>();
        amountRect.anchorMin = new Vector2(0f, 0f);
        amountRect.anchorMax = new Vector2(1f, 0f);
        amountRect.pivot = new Vector2(0.5f, 0f);
        amountRect.anchoredPosition = new Vector2(0f, 12f);
        amountRect.sizeDelta = new Vector2(-20f, 35f);

        Text amountText = amountObj.AddComponent<Text>();
        amountText.text = "x100";
        amountText.fontSize = isBig ? 24 : 20;
        amountText.fontStyle = FontStyle.Bold;
        amountText.alignment = TextAnchor.MiddleCenter;
        amountText.color = isBig ? new Color(0.2f, 0.1f, 0f, 1f) : new Color(0.1f, 0.1f, 0.2f, 1f);
        if (font != null) amountText.font = font;

        // Claimed Overlay
        GameObject claimed = new GameObject("ClaimedOverlay");
        claimed.transform.SetParent(slotObj.transform, false);
        RectTransform claimedRect = claimed.AddComponent<RectTransform>();
        claimedRect.anchorMin = Vector2.zero;
        claimedRect.anchorMax = Vector2.one;
        claimedRect.offsetMin = Vector2.zero;
        claimedRect.offsetMax = Vector2.zero;

        Image claimedImage = claimed.AddComponent<Image>();
        claimedImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject checkObj = new GameObject("Checkmark");
        checkObj.transform.SetParent(claimed.transform, false);
        RectTransform checkRect = checkObj.AddComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0.5f, 0.5f);
        checkRect.anchorMax = new Vector2(0.5f, 0.5f);
        checkRect.pivot = new Vector2(0.5f, 0.5f);
        checkRect.sizeDelta = new Vector2(60f, 60f);

        Image checkImage = checkObj.AddComponent<Image>();
        if (checkSprite != null)
        {
            checkImage.sprite = checkSprite;
        }
        else
        {
            checkImage.color = Color.green;
        }
        claimed.SetActive(false);

        // Populate DailyRewardSlotUI
        DailyRewardSlotUI ui = new DailyRewardSlotUI();
        ui.button = button;
        ui.icon = iconImage;
        ui.amountText = amountText;
        ui.dayLabelText = dayText;
        ui.claimedOverlay = claimed;
        ui.todayHighlight = highlight;

        return ui;
    }

    [MenuItem("Tools/Reset Daily Reward Data")]
    public static void ResetDailyRewardData()
    {
        // 1. Delete local JSON save file
        string saveFilePath = Path.Combine(Application.persistentDataPath, "UserData.json");
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("<color=yellow>[Reset] Deleted local save file UserData.json.</color>");
        }

        // 2. Clear PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("<color=yellow>[Reset] Cleared all PlayerPrefs.</color>");

        // 3. Reset static UserData variables
        UserData.dailyStreakDay = 0;
        UserData.lastDailyClaimDate = "";

        // 4. Force DailyRewardManager server sync reset
        DailyRewardManager.ResetServerSync();

        Debug.Log("<color=green>[Reset] Daily Reward data reset successful! Restart game to check.</color>");
    }
}

