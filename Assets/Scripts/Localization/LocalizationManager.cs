using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager singleton;

    public const string LOCAL_NOT_FOUND = "LOCAL_NOT_FOUND";

    public Dictionary<string, string> localizedText;

    public bool isReady { get; private set; }

    private string languageLocal;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public string SetDefaultLocal()
    {
        string loc = "";

        if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
        {
            loc = "ru-RU";
        }
        else
        {
            loc = "en-US";
        }

        return loc;
    }

    /*public string GetValue(string key)
    {
        return localizedText[key];
    }*/

    public void LoadLocalizedText(string fileLocal)
    {
        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, "localization_" + fileLocal + ".json");

        if (!File.Exists(filePath))
        {
            Debug.LogError("Path not found");
        }
        else
        {
            string dataStringJson = File.ReadAllText(filePath);
            LocalizationData data = JsonUtility.FromJson<LocalizationData>(dataStringJson);

            for (int i = 0; i < data.localizationStrings.Length; i++)
            {
                localizedText.Add(data.localizationStrings[i].key, data.localizationStrings[i].value);
            }
            Debug.Log("Dictionary " + fileLocal + " has " + localizedText.Count + " entries");
        }

        isReady = true;
        Debug.Log("Localization ready");
    }

    public string GetLocalizedValue(string keyValue)
    {
        if (localizedText.ContainsKey(keyValue))
        {
            return localizedText[keyValue];
        }

        return LOCAL_NOT_FOUND;
    }
}
