using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    public GameObject[] Highlights;
    public bool Connected = false;

    public RawImage ConnectionStatus;
    public Text Server;

    public GameObject MainMenuPanel;
    public GameObject MatchListPanel;
    public GameObject CustomMatchPanel;
    public GameObject SettingsPanel;
    public GameObject ProfilePanel;

    public GameObject playerListingPrefab;
    public Transform playerListingContainer;

    public override void OnConnectedToMaster()
    {
        Connected = true;

        ConnectionStatus.color = Color.green;

        Server.text = PhotonNetwork.CloudRegion + ":" + PhotonNetwork.CurrentCluster + ":" + PhotonNetwork.CurrentLobby;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        Server.text = PhotonNetwork.CloudRegion + ":" + PhotonNetwork.CurrentCluster + ":" + PhotonNetwork.CurrentLobby;

       /* string roomName = "RoomMcRoomFace";
        RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOpt, new TypedLobby(null, LobbyType.Default));*/
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        Debug.Log("Lobby Stats");
        Debug.Log("-------------------------------------------");
        foreach (TypedLobbyInfo l in lobbyStatistics)
        {
            Debug.Log(l.Name + ":" + l.PlayerCount + ":" + l.RoomCount);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
        Connected = false;

        ConnectionStatus.color = Color.white;
    }

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {

    }

    public void BackToMain()
    {
        MainMenuPanel.SetActive(true);
        MatchListPanel.SetActive(false);
    }

}
