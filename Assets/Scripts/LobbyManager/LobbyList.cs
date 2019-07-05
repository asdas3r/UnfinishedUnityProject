using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyList : MonoBehaviour
{
    public static LobbyList singleton;

    [SerializeField] GameObject lobbyPlayerPrefab;
    [SerializeField] Transform lobbyPlayerParent;
    [SerializeField] TMPro.TMP_Text lobbyNameText;
    [SerializeField] TMPro.TMP_Text lobbyStatusText;

    protected VerticalLayoutGroup layout;

    string lobbyName = "";

    public void OnEnable()
    {
        Debug.Log("LobbyList Singleton created");

        singleton = this;

        layout = lobbyPlayerParent.GetComponent<VerticalLayoutGroup>();
    }

    void Start()
    {
        if (LobbyManager.singleton)
        {
            lobbyName = LobbyManager.singleton.matchName;
        }

        lobbyNameText.text = " '" + lobbyName + "'";
        lobbyStatusText.text = " ";
    }

    private void Update()
    {
        if (layout)
            layout.childAlignment = Time.frameCount % 2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
    }

    public void AddPlayer(LobbyPlayer player)
    {
        Debug.Log("Adding Player");
        player.transform.SetParent(lobbyPlayerParent, false);
    }

    public void SetStatusText(string statusText)
    {
        lobbyStatusText.text = statusText;
    }
}
