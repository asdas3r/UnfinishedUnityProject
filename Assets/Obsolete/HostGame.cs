using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour
{
    [SerializeField]
    private uint roomSize = 2;

    NetworkLobbyManager networkManager;
    string roomName;
    string roomPassword;

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    public void SetRoomPassword(string pw)
    {
        roomPassword = pw;
    }

    void Start()
    {
        networkManager = (NetworkLobbyManager) NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        roomPassword = "";
    }

    public void CreateRoom()
    {
        if (roomName != "" && roomName != null)
        {
            Debug.Log("Creating lobby " + roomName);
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, roomPassword, "", "", 0, 0, networkManager.OnMatchCreate);
            MenuNavigation.singleton.LobbyButton();
        }
    }
}
