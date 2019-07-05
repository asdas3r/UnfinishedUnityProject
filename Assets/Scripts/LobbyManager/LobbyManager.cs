using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkLobbyManager
{
    public static LobbyManager singleton;

    [SerializeField] [Range(0,10)] int startMatchTime = 3;

    [HideInInspector]
    //public bool isMatchmaking = false;
    protected ulong currentMatchID;
    protected bool disconnectServer = false;

    void OnEnable()
    {
        Debug.Log("Lobby Manager Loaded!");
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        offlineScene = "";

        DontDestroyOnLoad(gameObject);
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        if (SceneManager.GetSceneAt(0).name != lobbyScene)
        {
            MenuNavigation.singleton.isMenuAnimated = false;
            MenuNavigation.singleton.SwitchPanelTo(null);
        }
    }
    
    public delegate void BackNetworkDelegate();
    public BackNetworkDelegate backDelegate;

    //--------
    
    public void AddPlayer()
    {
        TryToAddPlayer();
    }

    public void RemovePlayer(NetworkLobbyPlayer player)
    {
        player.RemovePlayer();
    }

    /*public void SimpleBackClbk()
    {

    }*/

    public void SaveSettingsClbk()
    {
        Settings.singleton.WriteValues();
    }

    /*public void StopHostClbk()
    {
        matchMaker.DestroyMatch((NetworkID)currentMatchID, 0, OnDestroyMatch);
        disconnectServer = true;
    }*/

    public void StopClientClbk()
    {
        StopClient();

        StopMatchMaker();

        Debug.Log("Stop client and MatchMaker");
    }

    public void BackPlayScreen()
    {
        Debug.Log("Stop Matchmaker");
        StopMatchMaker();
    }

    public void BackLobbyScreen()
    {
        StopHost();
    }

    //--------

    /*public override void OnStartHost()
    {
        base.OnStartHost();

        //backDelegate = StopHostClbk;
    }*/

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        currentMatchID = (System.UInt64)matchInfo.networkId;
    }

    public override void OnDestroyMatch(bool success, string extendedInfo)
    {
        base.OnDestroyMatch(success, extendedInfo);

        if (disconnectServer)
        {
            StopMatchMaker();
            StopHost();
            Debug.Log("Stop Host and MatchMaker");
        }
    }
    
    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

        LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();

        newPlayer.ToggleReadyButton(numPlayers + 1 >= minPlayers);

        SetToggleReadyButton(numPlayers + 1 >= minPlayers);

        return obj;
    }

    public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
    {
        SetToggleReadyButton(numPlayers + 1 >= minPlayers);
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        SetToggleReadyButton(numPlayers >= minPlayers);
    }

    private void SetToggleReadyButton(bool evaluation)
    {
        for (int i = 0; i < lobbySlots.Length; i++)
        {
            LobbyPlayer player = lobbySlots[i] as LobbyPlayer;

            if (player != null)
            {
                player.ToggleReadyButton(evaluation);
            }
        }
    }

    //------

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        
        if (!NetworkServer.active)
        {
            backDelegate = StopClientClbk;
            MenuNavigation.singleton.LobbyButton();
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        StopClient();
        MenuNavigation.singleton.BackButton();

        if (conn.lastError != NetworkError.Ok)
        {
            if (LogFilter.logError) { Debug.LogError("Client disconnected due to error: " + conn.lastError); }
        }

        Debug.Log("Client disconnected from server: " + conn);
    }

    bool CheckAllPlayersReady()
    {
        bool allPlayersReady = true;

        for (int i = 0; i < lobbySlots.Length; i++)
        {
            if (lobbySlots[i] != null)
            {
                allPlayersReady &= lobbySlots[i].readyToBegin;
            }
        }

        return allPlayersReady;
    }

    public override void OnLobbyServerPlayersReady()
    {
        if (CheckAllPlayersReady())
        {
            StartCoroutine(StartMatchCountdown());
        }
    }

    IEnumerator StartMatchCountdown()
    {
        int seconds = startMatchTime;
        float timer = seconds;

        for (int i = 0; i < lobbySlots.Length; i++)
        {
            if (lobbySlots[i] != null)
            {
                (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(seconds);
            }
        }

        while (seconds > 0)
        {
            yield return null;

            timer -= Time.deltaTime;
            //Debug.Log(timer + " - " + seconds);

            if (!CheckAllPlayersReady())
            {
                seconds = 0;
                timer = seconds - 1;
            }

            if (timer <= seconds - 1)
            {
                --seconds;
                for (int i = 0; i < lobbySlots.Length; i++)
                {
                    if (lobbySlots[i] != null)
                    {
                        (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(seconds);
                    }
                }
            }
        }

        if (seconds == 0)
        {
            //Debug.Log("GO!");
            yield return new WaitForSeconds(0.5f);

            /*for (int i = 0; i < lobbySlots.Length; i++)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as LobbyPlayer).RpcSaveLobbyPlayer();
                }
            }*/

            ServerChangeScene(playScene);
        }
    }
}
