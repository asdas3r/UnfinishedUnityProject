using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class LobbyHostJoin : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text statusText;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform roomListParent;
    [SerializeField] uint roomSize = 2;

    LobbyManager networkManager;
    LocalizationManager localizationManager;
    List<GameObject> roomList = new List<GameObject>();

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

    void OnEnable()
    {
        localizationManager = LocalizationManager.singleton;
        RefreshRoomList();
    }

    void Awake()
    {
        networkManager = LobbyManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
            Debug.Log("Start MatchMaker");
        }

        roomPassword = "";
    }

    public void CreateRoom()
    {
        if (roomName != "" && roomName != null)
        {
            Debug.Log("Creating lobby " + roomName);
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, roomPassword, "", "", 0, 0, networkManager.OnMatchCreate);
            networkManager.matchName = roomName;
            networkManager.backDelegate = networkManager.BackLobbyScreen;
            MenuNavigation.singleton.LobbyButton();
        }
        else
        {
            Debug.Log("Room name is empty");
        }
    }

    public void RefreshRoomList()
    {
        ClearRoomList();

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
            Debug.Log("Start MatchMaker");
        }

        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        statusText.text = localizationManager.GetLocalizedValue("play_join_loading");
    }

    private void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        statusText.text = "";

        if (matches == null)
        {
            statusText.text = localizationManager.GetLocalizedValue("play_join_error");
            return;
        }

        ClearRoomList();
        foreach (MatchInfoSnapshot info in matches)
        {
            GameObject roomListItemObject = Instantiate(roomListItemPrefab) as GameObject;
            roomListItemObject.transform.SetParent(roomListParent, false);

            LobbyRoomListItem roomListItem = roomListItemObject.GetComponent<LobbyRoomListItem>();
            if (!success || roomListItem != null)
            {
                roomListItem.ShowMatchInfo(info, JoinRoom);
            }

            roomList.Add(roomListItemObject);
        }

        if (roomList.Count == 0)
        {
            statusText.text = localizationManager.GetLocalizedValue("play_join_norooms");
        }
    }

    private void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot info)
    {
        networkManager.matchMaker.JoinMatch(info.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        networkManager.matchName = info.name;
        if (info.isPrivate)
        {
            // ------ todo: open password dialog 
        }
        StartCoroutine(WaitJoin());
    }

    IEnumerator WaitJoin()
    {
        ClearRoomList();

        int waittime = 10;

        while (waittime > 0)
        {
            statusText.text = localizationManager.GetLocalizedValue("play_join_connecting") + waittime + "...";
            yield return new WaitForSeconds(1);
            waittime--;
        }

        statusText.text = localizationManager.GetLocalizedValue("play_join_connecting");

        MatchInfo match = networkManager.matchInfo;
        if (match != null)
        {
            networkManager.matchMaker.DropConnection(match.networkId, match.nodeId, 0, networkManager.OnDropConnection);
            networkManager.StopHost();
        }
    }

}
