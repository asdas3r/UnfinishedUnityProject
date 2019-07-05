using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MenuNavigation1 : MonoBehaviour
{
    [SerializeField] Animator menuAnimator;
    [SerializeField] GameObject[] menuObjects;

    public static MenuNavigation1 singleton;

    LobbyManager networkManager;
    List<string> lastScreenName = new List<string>();

    void Awake()
    {
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
    }

    void Start()
    {
        networkManager = LobbyManager.singleton;
        StartCoroutine(OnLoadGame());
    }

    public void PlayButton()
    {
        StartCoroutine(AnimationTransition("PlayScreen", 1));
        //networkManager.backDelegate = networkManager.BackPlayScreen;
    }

    public void LobbyButton()
    {
        StartCoroutine(AnimationTransition("LobbyScreen", 1));
    }

    public void OptionsButton()
    {
        StartCoroutine(AnimationTransition("SettingsScreen", 2));
        //networkManager.backDelegate = networkManager.SaveSettingsClbk;
    }

    public void BackButton()
    {
        if (lastScreenName != null && lastScreenName.Count > 0)
        {
            StartCoroutine(AnimationTransition(String.Copy(lastScreenName[lastScreenName.Count - 1]), -1));
            //networkManager.BackButton();
        }
    }

    public void MainMenu()
    {
        StartCoroutine(AnimationTransition("MainMenu", 2));
    }

    void LoginWindow()
    {
        Debug.Log("LoginWindow_Choice");

        if (!Settings.singleton.LoadValues())
        {
            menuAnimator.SetInteger("Variant", 1);

            for (int i = 0; i < menuObjects.Length; i++)
            {
                if (menuObjects[i].name.Equals("LoadingScreen"))
                {
                    menuObjects[i].SetActive(false);
                }

                if (menuObjects[i].name.Equals("MainMenu"))
                {
                    menuObjects[i].SetActive(false);
                }

                if (menuObjects[i].name.Equals("Login"))
                {
                    menuObjects[i].SetActive(true);
                }
            }
        }
        else
        {
            menuAnimator.SetInteger("Variant", 2);

            for (int i = 0; i < menuObjects.Length; i++)
            {
                if (menuObjects[i].name.Equals("LoadingScreen"))
                {
                    menuObjects[i].SetActive(false);
                }

                if (menuObjects[i].name.Equals("MainMenu"))
                {
                    menuObjects[i].SetActive(true);
                }

                if (menuObjects[i].name.Equals("Login"))
                {
                    menuObjects[i].SetActive(false);
                }
            }
        }
    }

    IEnumerator OnLoadGame()
    {
        Debug.Log("OnLoad");
        yield return new WaitWhile(() => !LocalizationManager.singleton.isReady);
        LoginWindow();
    }

    public void SwitchPanelTo(GameObject panel)
    {

    }

    public void ExitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    IEnumerator AnimationTransition(string objectName, int variant)
    {
        menuAnimator.SetTrigger("FadeOut");
        menuAnimator.SetInteger("Variant", 0);

        for (int i = 0; i < menuObjects.Length; i++)
        {
            if (menuObjects[i].activeSelf == true)
            {
                menuObjects[i].GetComponent<CanvasGroup>().interactable = false;

                if (variant != -1)
                {
                    lastScreenName.Add(menuObjects[i].name);
                }
            }
        }
        //Debug.Log(menuAnimator.GetCurrentAnimatorStateInfo(0).length + " - " + menuAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        yield return new WaitForSeconds(menuAnimator.GetCurrentAnimatorStateInfo(0).length);

        menuAnimator.SetInteger("Variant", variant);

        menuAnimator.ResetTrigger("FadeOut");

        int index = 0;

        for (int i = 0; i < menuObjects.Length; i++)
        {
            if (menuObjects[i].name.Equals(objectName))
            {
                if (variant == -1)
                {
                    lastScreenName.Remove(lastScreenName[lastScreenName.Count - 1]);
                }

                menuObjects[i].SetActive(true);
                index = i;
            }
            else
            {
                menuObjects[i].SetActive(false);
            }
        }

        //yield return new WaitForSeconds(menuAnimator.GetCurrentAnimatorStateInfo(0).length);

        menuObjects[index].GetComponent<CanvasGroup>().interactable = true;
    }
}
