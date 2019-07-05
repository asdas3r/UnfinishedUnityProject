using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer
{
    [SerializeField] TMPro.TMP_Text playerNameText;
    [SerializeField] Button readyButton;
    [SerializeField] Button waitingButton;
    [SerializeField] Color readyColor;
    [SerializeField] Color notReadyColor;
    [SerializeField] Color waitingColor;
    [Space]
    [SerializeField] Color thisPlayerBG;
    [SerializeField] Color otherPlayersBG;

    [SyncVar(hook = "OnNameChange")]
    public string playerName = "";

    private bool isReady = false;

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        StartCoroutine(LateAddPlayer());
    }
    
    IEnumerator LateAddPlayer()
    {
        Debug.Log("Adding Player to Lobby...");

        yield return new WaitWhile(() => LobbyList.singleton == null);

        if (LobbyList.singleton)
        {
            LobbyList.singleton.AddPlayer(this);
        }

        Debug.Log("Waited for singleton");

        if (isLocalPlayer)
        {
            SetupLocalPlayer();
        }
        else
        {
            SetupOtherPlayer();
        }

        OnNameChange(playerName);
    }
    
    /*public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        SetupLocalPlayer();
    }*/

    public void ChangeReadyButtonColors(Color c)
    {
        ColorBlock readyButtonCB = readyButton.colors;
        readyButtonCB.normalColor = c;
        readyButtonCB.pressedColor = c;
        readyButtonCB.highlightedColor = c;
        readyButtonCB.disabledColor = c;

        readyButton.colors = readyButtonCB;
    }

    public void SetupOtherPlayer()
    {
        ChangeReadyButtonColors(waitingColor);

        readyButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = "...";
        readyButton.interactable = false;
        GetComponent<Button>().image.color = otherPlayersBG;

        OnClientReady(false);
    }

    public void SetupLocalPlayer()
    {
        ChangeReadyButtonColors(notReadyColor);

        readyButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = LocalizationManager.singleton.GetLocalizedValue("lobby_player_notready");
        readyButton.interactable = true;
        GetComponent<Button>().image.color = thisPlayerBG;

        CmdNameChanged(Settings.singleton.playerName);

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClicked);
    }

    public override void OnClientReady(bool readyState)
    {
        base.OnClientReady(readyState);

        if (readyState)
        {
            ChangeReadyButtonColors(readyColor);

            TMPro.TMP_Text buttonText = readyButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
            buttonText.text = LocalizationManager.singleton.GetLocalizedValue("lobby_player_ready");
        }
        else
        {
            ChangeReadyButtonColors(isLocalPlayer? notReadyColor : waitingColor);

            TMPro.TMP_Text buttonText = readyButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
            buttonText.text = isLocalPlayer ? LocalizationManager.singleton.GetLocalizedValue("lobby_player_notready") : "...";
        }
    }

    public void OnNameChange(string name)
    {
        playerName = name;
        playerNameText.text = playerName;
    }

    public void OnReadyClicked()
    {
        isReady = !isReady;
        if (isReady)
        {
            SendReadyToBeginMessage();
        }
        else
        {
            SendNotReadyToBeginMessage();
        }
    }

    public void ToggleReadyButton(bool isEnabled)
    {
        readyButton.gameObject.SetActive(isEnabled);
        waitingButton.gameObject.SetActive(!isEnabled);
        /*if (isEnabled)
        {
            SendNotReadyToBeginMessage();
            isReady = false;
        }*/
    }

    [Command]
    public void CmdNameChanged(string name)
    {
        playerName = name;
    }

    [ClientRpc]
    public void RpcUpdateCountdown(int seconds)
    {
        if (seconds == -1)
        {
            LobbyList.singleton.SetStatusText("");
            return;
        }
        LobbyList.singleton.SetStatusText(LocalizationManager.singleton.GetLocalizedValue("lobby_status_starting") + ": " + seconds);
    }

    /*[ClientRpc]
    public void RpcSaveLobbyPlayer()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }*/
}
