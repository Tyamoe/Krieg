using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    public MatchListController matchListCtrl;

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

    void Awake()
    {
        MainMenuPanel.SetActive(true);
        MatchListPanel.SetActive(true);
        CustomMatchPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        ProfilePanel.SetActive(false);
    }

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

    public void Matchmaking()
    {
        string roomName = "Room1234";

        RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOpt, new TypedLobby(null, LobbyType.Default));
    }

    public void LogOut()
    {
        PhotonNetwork.Disconnect();

        MainMenuPanel.SetActive(true);
        MatchListPanel.SetActive(false);
        CustomMatchPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        ProfilePanel.SetActive(false);
    }

    public void Host()
    {
        CustomMatchPanel.SetActive(true);
        MatchListPanel.SetActive(false);
        ProfilePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
    }

    public void List()
    {
        MatchListPanel.SetActive(true);
        ProfilePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        CustomMatchPanel.SetActive(false);
    }

    public void Profile()
    {
        ProfilePanel.SetActive(true);
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        MatchListPanel.SetActive(false);
        CustomMatchPanel.SetActive(false);
    }

    public void Settings()
    {
        SettingsPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        MatchListPanel.SetActive(false);
        CustomMatchPanel.SetActive(false);
        ProfilePanel.SetActive(false);
    }

    public void BackToMain()
    {
        MainMenuPanel.SetActive(true);
        MatchListPanel.SetActive(false);
        CustomMatchPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        ProfilePanel.SetActive(false);
        
        if(matchListCtrl.matchList.Count > 0)
            Debug.Log("1111Custom: " + matchListCtrl.matchList[0].CustomProperties.ToStringFull());
    }

}
