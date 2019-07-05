using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Animator anim;
    [Space]
    [SerializeField] MenuPanel[] menuPanels;

    public MenuPanel currentPanel = null;

    public delegate void BackMenuDelegate();
    public BackMenuDelegate backDelegate;
    public Dictionary<string, BackMenuDelegate> previousPanels = new Dictionary<string, BackMenuDelegate>();
    public static MenuNavigation singleton;
    public bool isMenuAnimated = true;

    LobbyManager networkManager;
    EventSystem eventSystem;
    bool backButton = false;
    GameObject lastSelectedGameObject;

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
        currentPanel = null;
        networkManager = LobbyManager.singleton;
        eventSystem = EventSystem.current;
        StartCoroutine(OnLoadGame());
    }

    void Update()
    {
        NavigationInputsActivations();
        CancelEvents();
    }

    public void SwitchPanelTo(string panel)
    {
        StartCoroutine(SwitchPanels(panel));
    }

    IEnumerator SwitchPanels(string panel)
    {
        string previousPanelName = "null";
        if (currentPanel != null)
        {
            previousPanelName = currentPanel.name;
            currentPanel.panelObject.GetComponent<CanvasGroup>().interactable = false;

            if (!isMenuAnimated)
            {
                yield return null;
            }
            else
            {
                yield return StartCoroutine(PlayAnimation(currentPanel, false));
            }

            //currentPanel.panelObject.GetComponent<CanvasGroup>().interactable = true;
            currentPanel.panelObject.SetActive(false);
        }

        /*if (backButton && previousPanels.ContainsKey(currentPanel.name))
        {
            Debug.Log("Back Delegated!");
            previousPanels[currentPanel.name]();
        }*/

        MenuPanel nextPanel = null;

        foreach (MenuPanel p in menuPanels)
        {
            if (!string.IsNullOrEmpty(panel) && p.name.Equals(panel))
            {
                nextPanel = p;
            }
        }

        eventSystem.firstSelectedGameObject = null;
        currentPanel = nextPanel;

        string nextPanelName = "null";
        if (currentPanel != null)
        {
            nextPanelName = currentPanel.name;
            currentPanel.panelObject.SetActive(true);
            currentPanel.panelObject.GetComponent<CanvasGroup>().interactable = true;
            if (!isMenuAnimated)
            {
                yield return null;
            }
            else
            {
                yield return StartCoroutine(PlayAnimation(currentPanel, true));
            }
            //currentPanel.panelObject.GetComponent<CanvasGroup>().interactable = true;
        }

        Debug.Log("Panel '" + previousPanelName + "' switched to '" + nextPanelName + "'");

        backButton = false;
    }

    IEnumerator PlayAnimation(MenuPanel panel, bool isIn)
    {
        anim.SetBool("In", isIn);
        anim.SetTrigger(panel.name);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

    public void PlayButton()
    {
        GetPanelByName("Play").SetBackPanel(currentPanel);
        previousPanels["Play"] = BackPlay;
        SwitchPanelTo("Play");
    }

    public void BackPlay()
    {
        networkManager.BackPlayScreen();
        BackButtonAction();
    }

    public void LobbyButton()
    {
        GetPanelByName("Lobby").SetBackPanel(currentPanel);
        previousPanels["Lobby"] = BackLobby;
        SwitchPanelTo("Lobby");
    }

    public void BackLobby()
    {
        networkManager.backDelegate();
        BackButtonAction();
    }

    public void OptionsButton()
    {
        GetPanelByName("Settings").SetBackPanel(currentPanel);
        previousPanels["Settings"] = BackOptions;
        SwitchPanelTo("Settings");
    }

    public void BackOptions()
    {
        networkManager.SaveSettingsClbk();
        BackButtonAction();
    }

    public void BackButtonAction()
    {
        backButton = true;
        if (currentPanel.backPanel != null)
        {
            SwitchPanelTo(currentPanel.backPanel.name);
        }
        else
        {
            SwitchPanelTo(string.Empty);
        }
    }

    public void BackButton()
    {
        previousPanels[currentPanel.name]();
    }

    public void MainMenu()
    {
        SwitchPanelTo("MainMenu");
    }

    public void ExitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public MenuPanel GetPanelByName(string name)
    {
        MenuPanel panel = null;

        foreach (MenuPanel p in menuPanels)
        {
            if (!string.IsNullOrEmpty(name) && p.name.Equals(name))
            {
                panel = p;
            }
        }

        return panel;
    }

    void LoginWindow()
    {
        Debug.Log("LoginWindow_Choice");

        if (!Settings.singleton.LoadValues())
        {
            loadingScreen.SetActive(false);
            SwitchPanelTo("Login");
        }
        else
        {
            loadingScreen.SetActive(false);
            SwitchPanelTo("MainMenu");
        }
    }

    IEnumerator OnLoadGame()
    {
        Debug.Log("OnLoad");
        yield return new WaitWhile(() => !LocalizationManager.singleton.isReady);
        //yield return null;
        LoginWindow();
    }

    private void NavigationInputsActivations()
    {
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            if (eventSystem.firstSelectedGameObject == null)
            {
                if (currentPanel.firstToSelect != null)
                {
                    eventSystem.firstSelectedGameObject = currentPanel.firstToSelect;
                }
                else
                {
                    eventSystem.firstSelectedGameObject = FindObjectOfType<Selectable>().gameObject;
                }
                eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
                lastSelectedGameObject = eventSystem.firstSelectedGameObject;
            }
            else if (eventSystem.currentSelectedGameObject == null)
            {
                eventSystem.SetSelectedGameObject(lastSelectedGameObject);

                AxisEventData axisData = new AxisEventData(eventSystem);

                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    axisData.moveDir = MoveDirection.Right;
                }
                else
                {
                    axisData.moveDir = MoveDirection.Left;
                }

                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    axisData.moveDir = MoveDirection.Up;
                }
                else
                {
                    axisData.moveDir = MoveDirection.Down;
                }

                ExecuteEvents.Execute(lastSelectedGameObject, axisData, ExecuteEvents.moveHandler);
            }
            else if (eventSystem.currentSelectedGameObject != lastSelectedGameObject)
            {
                lastSelectedGameObject = eventSystem.currentSelectedGameObject;
            }
        }

        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            if (eventSystem.IsPointerOverGameObject())
            {
                foreach (GameObject o in ExtendedStandaloneInputModule.GetPointerEventData().hovered)
                {
                    if (o == eventSystem.currentSelectedGameObject)
                    {
                        break;
                    }
                    Button button = o.GetComponent(typeof(Button)) as Button;
                    //&& button.navigation.mode != Navigation.Mode.None
                    if (button != null)
                    {
                        Animator anim = button.GetComponent<Animator>();
                        if (eventSystem.currentSelectedGameObject != null)
                        {
                            if (anim == null)
                            {
                                break;
                            }
                            anim.ResetTrigger("Highlighted");
                            anim.SetTrigger("Normal");
                        }
                    }
                }
            }
        }

        if ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {
            if (eventSystem.currentSelectedGameObject != null && (eventSystem.currentSelectedGameObject.GetComponent(typeof(TMPro.TMP_InputField)) as TMPro.TMP_InputField == null))
            {
                eventSystem.SetSelectedGameObject(null);
            }

            if (eventSystem.IsPointerOverGameObject())
            {
                foreach (GameObject o in ExtendedStandaloneInputModule.GetPointerEventData().hovered)
                {
                    if (o == eventSystem.currentSelectedGameObject)
                    {
                        break;
                    }
                    Button button = o.GetComponent(typeof(Button)) as Button;
                    if (button != null)
                    {
                        Animator anim = button.GetComponent<Animator>();
                        if (anim == null)
                        {
                            break;
                        }
                        anim.ResetTrigger("Normal");
                        anim.SetTrigger("Highlighted");
                    }
                }
            }
        }
    }

    void CancelEvents()
    {
        if (Input.GetButtonDown("Cancel") && currentPanel.backPanel != null)
        {
            bool isNeededToCancel = false;

            foreach (Selectable s in Selectable.allSelectablesArray)
            {
                TMPro.TMP_InputField inputField = s.GetComponent<TMPro.TMP_InputField>();

                if (inputField != null && inputField.gameObject.GetComponent<InputCancelSubmitHandle>() != null)
                {
                    if (inputField.gameObject.GetComponent<InputCancelSubmitHandle>().wasFocused)
                    {
                        isNeededToCancel = true;
                    }
                }

                if (!isNeededToCancel && s.GetComponent<TMPro.TMP_Dropdown>() != null && s.GetComponent<TMPro.TMP_Dropdown>().transform.childCount != 3)
                {
                    isNeededToCancel = true;
                }
            }

            if (!isNeededToCancel)
            {
                BackButton();
                AudioManager.instance.Play("ButtonClick");
            }
        }
    }
}
