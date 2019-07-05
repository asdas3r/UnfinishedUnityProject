using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    void Start()
    {
        PauseMenu.isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isPaused = pauseMenu.activeSelf;
    }

    public void GameSettings()
    {
        //MenuNavigation.singleton.SwitchPanelTo("Settings", false);
        MenuNavigation.singleton.isMenuAnimated = false;
        MenuNavigation.singleton.GetPanelByName("Settings").SetBackPanel(null);
        MenuNavigation.singleton.SwitchPanelTo("Settings");
    }
}
