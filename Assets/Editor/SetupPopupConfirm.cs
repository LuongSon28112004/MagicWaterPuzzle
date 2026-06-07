using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SetupPopupConfirm
{
    [MenuItem("Tools/Setup Popup Confirm")]
    public static void Setup()
    {
        DoSetup();
    }

    [InitializeOnLoadMethod]
    private static void AutoSetup()
    {
        // Only auto run if it doesn't exist
        string newPrefabPath = "Assets/Resources/UI/Popups/PopupConfirm.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath) == null)
        {
            EditorApplication.delayCall += DoSetup;
        }
    }

    private static void DoSetup()
    {
        string newPrefabPath = "Assets/Resources/UI/Popups/PopupConfirm.prefab";
        string sourcePrefabPath = "Assets/Resources/UI/Popups/PopupExitLevel.prefab";
        
        if (AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath) == null)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(sourcePrefabPath) != null)
            {
                AssetDatabase.CopyAsset(sourcePrefabPath, newPrefabPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError($"[Setup] Cannot find source prefab {sourcePrefabPath}");
                return;
            }
        }

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(newPrefabPath);
        if (prefabRoot == null) return;

        // Remove old script
        PopupExitLevel oldScript = prefabRoot.GetComponent<PopupExitLevel>();
        if (oldScript != null)
        {
            Object.DestroyImmediate(oldScript, true);
        }

        // Add new script
        PopupConfirm newScript = prefabRoot.GetComponent<PopupConfirm>();
        if (newScript == null)
        {
            newScript = prefabRoot.AddComponent<PopupConfirm>();
        }

        // Find elements
        Button[] buttons = prefabRoot.GetComponentsInChildren<Button>(true);
        Button btnNo = null;
        Button btnYes = null;

        if (buttons.Length >= 2)
        {
            // PopupExitLevel has CloseButton, GiveUpButton
            foreach (var b in buttons)
            {
                string bName = b.gameObject.name.ToLower();
                if (bName.Contains("giveup") || bName.Contains("yes") || bName.Contains("ok") || bName.Contains("confirm"))
                {
                    btnYes = b;
                }
                else if (bName.Contains("close") || bName.Contains("no") || bName.Contains("cancel"))
                {
                    btnNo = b;
                }
            }
            if (btnYes == null) btnYes = buttons[1];
            if (btnNo == null) btnNo = buttons[0];

            Text yesText = btnYes.GetComponentInChildren<Text>(true);
            if (yesText != null) yesText.text = "Có";

            Text noText = btnNo.GetComponentInChildren<Text>(true);
            if (noText != null) noText.text = "Không";
        }

        Text txtMsg = null;
        Text[] texts = prefabRoot.GetComponentsInChildren<Text>(true);
        foreach (Text t in texts)
        {
            if (t.transform.parent.GetComponent<Button>() == null)
            {
                // Find main text, typically a larger box or not the title
                if (t.gameObject.name.Contains("Message") || t.gameObject.name.Contains("Desc"))
                {
                    txtMsg = t;
                    break;
                }
                if (txtMsg == null)
                {
                    txtMsg = t;
                }
            }
        }
        
        if (txtMsg != null)
        {
            txtMsg.text = "Bạn có chắc chắn muốn xóa hay không?";
            txtMsg.alignment = TextAnchor.MiddleCenter;
            txtMsg.fontSize = 40;
        }

        // Title text if any
        foreach (Text t in texts)
        {
            if (t != txtMsg && t.transform.parent.GetComponent<Button>() == null)
            {
                if (t.gameObject.name.Contains("Title") || t.fontSize >= 40)
                {
                    t.text = "Xác nhận";
                }
            }
        }

        // Apply references
        SerializedObject so = new SerializedObject(newScript);
        if (btnYes != null) so.FindProperty("btnYes").objectReferenceValue = btnYes;
        if (btnNo != null) so.FindProperty("btnNo").objectReferenceValue = btnNo;
        if (txtMsg != null) so.FindProperty("txtMessage").objectReferenceValue = txtMsg;
        so.ApplyModifiedProperties();

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, newPrefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        Debug.Log("<color=green>[Setup] Created and configured PopupConfirm prefab successfully!</color>");
    }
}
