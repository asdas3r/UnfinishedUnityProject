using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    public void DropConnection()
    {
        Debug.Log("Dropping connection.");
        MatchInfo match = networkManager.matchInfo;
        if (networkManager.matchMaker != null)
        {
            networkManager.matchMaker.DropConnection(match.networkId, match.nodeId, 0, networkManager.OnDropConnection);
            networkManager.StopHost();
        }
    }
}
