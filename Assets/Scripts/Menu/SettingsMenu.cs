using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField playerNameField;
    [SerializeField] Slider mouseSensSlider;
    [SerializeField] Slider FOVSlider;
    [SerializeField] Slider volumeSlider;
    [SerializeField] TMPro.TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle isFullscreenToggle;
    [SerializeField] TMPro.TMP_Dropdown qualityDropdown;
    [SerializeField] Toggle[] languageToggles;

    Resolution[] resolutions;
    Settings settings;

    bool isFullscreenToggled = false;

    void OnEnable()
    {
        settings = Settings.singleton;
        ScreenResolutions();
        LoadSettings();
        foreach (Toggle t in languageToggles)
        {
            t.onValueChanged.RemoveAllListeners();
            t.onValueChanged.AddListener(delegate { SetLanguage(t); });
        }
    }

    void LateUpdate()
    {
        if (!isFullscreenToggled && Screen.fullScreen != isFullscreenToggle.isOn)
        {
            isFullscreenToggle.isOn = Screen.fullScreen;
        }
        isFullscreenToggled = false;
    }

    public void SetPlayerName(string playerName)
    {
        if (!string.IsNullOrEmpty(playerName))
        {
            settings.SetPlayerName(playerName);
        }
    }

    public void SetSensitivity(float sensitivity)
    {
        settings.mouseSensitivity = sensitivity;
    }

    public void SetFieldOfView(float fov)
    {
        settings.fieldOfView = fov;
    }

    public void SetVolume(float value)
    {
        settings.SetVolume(value);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        settings.SetFullScreen(isFullscreen);
        isFullscreenToggled = true;
    }

    public void SetResolution(int resIndex)
    {
        Resolution resolution = resolutions[resIndex];
        settings.SetResolution(resolution.width, resolution.height);
        //Debug.Log(Screen.currentResolution.width + " x " + Screen.currentResolution.height);
    }

    public void SetQuality(int qualityIndex)
    {
        settings.SetQuality(qualityIndex + 1);
    }

    public void SetLanguage(Toggle toggle)
    {
        if (toggle.isOn && !settings.language.Equals(toggle.name))
        {
            string language = settings.language;

            settings.SetLanguage(toggle.name);
            RefreshDropdown();

            foreach (Toggle t in languageToggles)
            {
                if (language.Equals(t.name))
                {
                    t.isOn = false;
                }
            }
        }
        else if (!toggle.isOn && settings.language.Equals(toggle.name))
        {
            toggle.isOn = true;
            Debug.Log(toggle.name + " - " + toggle.isOn);
        }
    }

    public void RefreshDropdown()
    {
        List<TMPro.TMP_Dropdown.OptionData> dataList = qualityDropdown.options;

        dataList[0].text = LocalizationManager.singleton.GetLocalizedValue("settings_graph_quality_low");
        dataList[1].text = LocalizationManager.singleton.GetLocalizedValue("settings_graph_quality_mid");
        dataList[2].text = LocalizationManager.singleton.GetLocalizedValue("settings_graph_quality_high");
        qualityDropdown.RefreshShownValue();
    }

    private void ScreenResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> resOptions = new List<string>();

        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            resOptions.Add(option);

            if (resolutions[i].width == settings.resolution.width && resolutions[i].height == settings.resolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void LoadSettings()
    {
        playerNameField.text = settings.playerName;
        mouseSensSlider.value = settings.mouseSensitivity;
        FOVSlider.value = settings.fieldOfView;
        volumeSlider.value = AudioListener.volume;
        isFullscreenToggle.isOn = Screen.fullScreen;
        qualityDropdown.value = QualitySettings.GetQualityLevel() - 1;

        foreach (Toggle t in languageToggles)
        {
            t.isOn = false;
            if (settings.language.Equals(t.name))
            {
                t.isOn = true;
            }
        }

        RefreshDropdown();
    }

}
