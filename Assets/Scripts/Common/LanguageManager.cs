using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameLanguage
{
    English = 0,
    Vietnamese = 1
}

public class LanguageManager : MonoBehaviour
{
    private static LanguageManager instance;
    public static LanguageManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("LanguageManager");
                instance = go.AddComponent<LanguageManager>();
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    public static event Action OnLanguageChanged;

    private const string LANGUAGE_PREF_KEY = "GameLanguage";

    private Dictionary<string, string> englishDictionary = new Dictionary<string, string>();
    private Dictionary<string, string> vietnameseDictionary = new Dictionary<string, string>();
    private bool isLoaded = false;

    public GameLanguage CurrentLanguage
    {
        get
        {
            return (GameLanguage)PlayerPrefs.GetInt(LANGUAGE_PREF_KEY, (int)GameLanguage.English);
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }
        Initialize();
    }

    public void Initialize()
    {
        if (isLoaded) return;
        LoadLocalizationFile();
        isLoaded = true;
    }

    public void SetLanguage(GameLanguage language)
    {
        PlayerPrefs.SetInt(LANGUAGE_PREF_KEY, (int)language);
        PlayerPrefs.Save();
        OnLanguageChanged?.Invoke();
    }

    public string GetTranslation(string key)
    {
        Initialize();

        if (string.IsNullOrEmpty(key)) return string.Empty;

        string searchKey = key.Trim();

        if (CurrentLanguage == GameLanguage.Vietnamese)
        {
            if (vietnameseDictionary.TryGetValue(searchKey, out string viVal))
                return viVal;
        }
        else
        {
            if (englishDictionary.TryGetValue(searchKey, out string enVal))
                return enVal;
        }

        // Fallback: Check the opposite language dictionary, then default to the key itself
        if (englishDictionary.TryGetValue(searchKey, out string fallbackEn))
            return fallbackEn;
        if (vietnameseDictionary.TryGetValue(searchKey, out string fallbackVi))
            return fallbackVi;

        return key;
    }

    private void LoadLocalizationFile()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Localization");
        if (textAsset == null)
        {
            Debug.LogWarning("[LanguageManager] Localization.csv not found in Resources folder!");
            return;
        }

        englishDictionary.Clear();
        vietnameseDictionary.Clear();

        string text = textAsset.text;
        List<List<string>> grid = ParseCSV(text);

        for (int i = 1; i < grid.Count; i++) // Skip header row
        {
            List<string> row = grid[i];
            if (row.Count >= 3)
            {
                string key = row[0].Trim();
                string enValue = row[1].Replace("\\n", "\n");
                string viValue = row[2].Replace("\\n", "\n");

                if (!string.IsNullOrEmpty(key))
                {
                    englishDictionary[key] = enValue;
                    vietnameseDictionary[key] = viValue;
                }
            }
        }
    }

    private List<List<string>> ParseCSV(string text)
    {
        List<List<string>> grid = new List<List<string>>();
        List<string> currentRow = new List<string>();
        System.Text.StringBuilder currentToken = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                currentRow.Add(currentToken.ToString());
                currentToken.Clear();
            }
            else if ((c == '\n' || c == '\r') && !inQuotes)
            {
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
                {
                    i++; // skip '\n'
                }
                currentRow.Add(currentToken.ToString());
                currentToken.Clear();
                grid.Add(new List<string>(currentRow));
                currentRow.Clear();
            }
            else
            {
                currentToken.Append(c);
            }
        }

        if (currentToken.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(currentToken.ToString());
            grid.Add(currentRow);
        }

        return grid;
    }
}
