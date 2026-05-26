using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string key;

    private Text uiText;
    private TextMeshProUGUI tmpText;

    private void Awake()
    {
        uiText = GetComponent<Text>();
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        UpdateText();
        LanguageManager.OnLanguageChanged += UpdateText;
    }

    private void OnDestroy()
    {
        LanguageManager.OnLanguageChanged -= UpdateText;
    }

    public void SetKey(string newKey)
    {
        key = newKey;
        UpdateText();
    }

    private void UpdateText()
    {
        if (string.IsNullOrEmpty(key)) return;

        string currentText = LanguageManager.Instance.GetTranslation(key);

        if (uiText != null)
        {
            uiText.text = currentText;
        }
        else if (tmpText != null)
        {
            tmpText.text = currentText;
        }
    }
}
