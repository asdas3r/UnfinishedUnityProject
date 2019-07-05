using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings singleton;

    [HideInInspector] public string playerName;
    [HideInInspector] public float mouseSensitivity;
    [HideInInspector] public float fieldOfView;
    [HideInInspector] public float volume;
    [HideInInspector] public Resolution resolution;
    [HideInInspector] public bool isFullscreen;
    [HideInInspector] public int qualityLevel;
    [HideInInspector] public string language;

    string playerNameKey = "Player Name";
    string mouseSensitivityKey = "Mouse Sensitivity";
    string fieldOfViewKey = "Field Of View";
    string volumeKey = "Volume";
    string resolutionWidthKey = "Screen Resolution Width";
    string resolutionHeightKey = "Screen Resolution Height";
    string isFullscreenKey = "Fullscreen";
    string qualityLevelKey = "Quality";
    string languageKey = "Language";

    void Awake()
    {
        Debug.Log("Settings singleton created");

        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
        //DeleteSaved();
    }

    void Start()
    {
        SetValues();
    }

    public void SetDefaultValues()
    {
        playerName = "";
        mouseSensitivity = 1.0f;
        fieldOfView = 65.0f;
        volume = 1.0f;
        resolution.width = Screen.currentResolution.width;
        resolution.height = Screen.currentResolution.height;
        isFullscreen = false;
        qualityLevel = 2;
        language = "en-US";
        //language = LocalizationManager.singleton.SetDefaultLocal();
    }

    public void WriteValues()
    {
        PlayerPrefs.SetString(playerNameKey, playerName);
        PlayerPrefs.SetFloat(mouseSensitivityKey, mouseSensitivity);
        PlayerPrefs.SetFloat(fieldOfViewKey, fieldOfView);
        PlayerPrefs.SetFloat(volumeKey, volume);
        PlayerPrefs.SetInt(resolutionWidthKey, resolution.width);
        PlayerPrefs.SetInt(resolutionHeightKey, resolution.height);
        PlayerPrefs.SetInt(isFullscreenKey, isFullscreen? 1 : 0 );
        PlayerPrefs.SetInt(qualityLevelKey, qualityLevel);
        PlayerPrefs.SetString(languageKey, language);

        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs Saved");
    }

    public bool LoadValues()
    {
        Debug.Log("Load Values");
        if (!(PlayerPrefs.HasKey(playerNameKey) && PlayerPrefs.HasKey(mouseSensitivityKey) 
            && PlayerPrefs.HasKey(fieldOfViewKey) && PlayerPrefs.HasKey(volumeKey) && PlayerPrefs.HasKey(languageKey)
            && PlayerPrefs.HasKey(resolutionWidthKey) && PlayerPrefs.HasKey(resolutionHeightKey) 
            && PlayerPrefs.HasKey(isFullscreenKey) && PlayerPrefs.HasKey(qualityLevelKey)))
        {
            return false;
        }

        playerName = PlayerPrefs.GetString(playerNameKey);
        mouseSensitivity = PlayerPrefs.GetFloat(mouseSensitivityKey);
        fieldOfView = PlayerPrefs.GetFloat(fieldOfViewKey);
        volume = PlayerPrefs.GetFloat(volumeKey);
        resolution.width = PlayerPrefs.GetInt(resolutionWidthKey);
        resolution.height = PlayerPrefs.GetInt(resolutionHeightKey);
        isFullscreen = PlayerPrefs.GetInt(isFullscreenKey) == 1 ? true : false;
        qualityLevel = PlayerPrefs.GetInt(qualityLevelKey);
        language = PlayerPrefs.GetString(languageKey);
        Debug.Log("Values loaded");
        return true;
    }

    public void SetPlayerName(string name)
    {
        if (name != null && name.Trim() != "")
        {
            playerName = name;
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        volume = value;
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
        resolution.width = width;
        resolution.height = height;
    }

    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
        isFullscreen = value; 
    }

    public void SetQuality(int value)
    {
        QualitySettings.SetQualityLevel(value);
        qualityLevel = value;
    }

    public void SetLanguage(string languageLocal)
    {
        LocalizationManager.singleton.LoadLocalizedText(languageLocal);
        language = languageLocal;
    }

    public void DeleteSaved()
    {
        Debug.Log("Delete Values");
        PlayerPrefs.DeleteKey(playerNameKey);
        PlayerPrefs.DeleteKey(mouseSensitivityKey);
        PlayerPrefs.DeleteKey(fieldOfViewKey);
        PlayerPrefs.DeleteKey(volumeKey);
        PlayerPrefs.DeleteKey(resolutionWidthKey);
        PlayerPrefs.DeleteKey(resolutionHeightKey);
        PlayerPrefs.DeleteKey(isFullscreenKey);
        PlayerPrefs.DeleteKey(qualityLevelKey);
        PlayerPrefs.DeleteKey(languageKey);
    }

    public void SetValues()
    {
        if (!LoadValues())
        {
            SetDefaultValues();
        }

        SetVolume(volume);
        SetLanguage(language);
        //SetResolution(resolution.width, resolution.height);
        //SetFullScreen(isFullscreen);
        //SetQuality(qualityLevel);

        Debug.Log("Values set!");
    }
}
