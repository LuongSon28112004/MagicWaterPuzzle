using TMPro;
using UnityEngine;

public class TextLevel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textLevel;

    private void Start()
    {
        textLevel = GetComponent<TextMeshProUGUI>();
    }

    public void ChangeLevel(int level)
    {
        textLevel.text = "LEVEL " + level.ToString();
    }
}
