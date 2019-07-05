using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayerHook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobbier = lobbyPlayer.GetComponent<LobbyPlayer>();
        Player gamer = gamePlayer.GetComponent<Player>();

        gamer.RpcChangeName(lobbier.playerName);
    }
}
