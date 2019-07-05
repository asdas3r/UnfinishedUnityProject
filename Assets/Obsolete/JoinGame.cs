using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour
{
    [SerializeField]
    private Text statusText;
    [SerializeField]
    private GameObject roomListItemPrefab;
    [SerializeField]
    private Transform roomListParent;

    NetworkLobbyManager networkManager;
    List<GameObject> roomList = new List<GameObject>();

    void Start()
    {
        networkManager = (NetworkLobbyManager) NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        statusText.text = "Загрузка...";
    }

    private void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        statusText.text = "";

        if (matches == null)
        {
            statusText.text = "Ошибка поиска.";
            return;
        }

        ClearRoomList();
        foreach (MatchInfoSnapshot info in matches)
        {
            GameObject roomListItemObject = Instantiate(roomListItemPrefab);
            roomListItemObject.transform.SetParent(roomListParent);

            LobbyRoomListItem roomListItem = roomListItemObject.GetComponent<LobbyRoomListItem>();
            if (!success || roomListItem != null)
            {
                roomListItem.ShowMatchInfo(info, JoinRoom);
            }

            roomList.Add(roomListItemObject);
        }

        if (roomList.Count == 0)
        {
            statusText.text = "Свободных комнат не найдено.";
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
        StartCoroutine(WaitJoin());
        MenuNavigation.singleton.LobbyButton();
    }

    IEnumerator WaitJoin()
    {
        ClearRoomList();

        int waittime = 10;

        while (waittime > 0)
        {
            statusText.text = "Подождите, идет соединение..." + waittime + "...";

            yield return new WaitForSeconds(1);

            waittime--;

        }

        statusText.text = "Не удалось подключиться к комнате.";

        MatchInfo match = networkManager.matchInfo;
        if (match != null)
        {
            networkManager.matchMaker.DropConnection(match.networkId, match.nodeId, 0, networkManager.OnDropConnection);
            networkManager.StopHost();
        }
    }
}
