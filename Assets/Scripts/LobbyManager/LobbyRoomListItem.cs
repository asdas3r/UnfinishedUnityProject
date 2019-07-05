using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class LobbyRoomListItem : MonoBehaviour
{
    public delegate void RoomJoinDelegate(MatchInfoSnapshot info);
    public RoomJoinDelegate roomJoinCallback;

    [SerializeField] TMPro.TMP_Text roomNameText;
    [SerializeField] TMPro.TMP_Text roomFillText;
    [SerializeField] Button joinButton;
    [SerializeField] GameObject isPrivateLock;
    [SerializeField] GameObject isPrivateOpenArms;

    private MatchInfoSnapshot matchinfo;

    public void ShowMatchInfo(MatchInfoSnapshot info, RoomJoinDelegate callback)
    {
        matchinfo = info;

        roomJoinCallback = callback;

        roomNameText.text = info.name;
        roomFillText.text = info.currentSize + "/" + info.maxSize;

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(JoinRoom);

        if (info.isPrivate)
        {
            isPrivateLock.SetActive(true);
            isPrivateOpenArms.SetActive(false);
        }
        else
        {
            isPrivateLock.SetActive(false);
            isPrivateOpenArms.SetActive(true);
        }
    }

    public void JoinRoom()
    {
        roomJoinCallback.Invoke(matchinfo);
    }

}
