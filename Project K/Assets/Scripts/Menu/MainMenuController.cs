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
    }

    public override void OnJoinedLobby()
    {
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        Debug.Log("MainMenuController OnLobbyStatisticsUpdate");
        Debug.Log("-------------------------------------------");
        foreach (TypedLobbyInfo l in lobbyStatistics)
        {
            Debug.Log(l.Name + " : " + l.PlayerCount + " : " + l.RoomCount);
        }
        Debug.Log("-------------------------------------------");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("MainMenuController OnDisconnected");

        Connected = false;
        ConnectionStatus.color = Color.white;
    }

    public void Matchmaking()
    {
        Game.PlayLow();

        string roomName = "Room1234";

        RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOpt, new TypedLobby(null, LobbyType.Default));
    }

    public void LogOut()
    {
        Game.PlayHigh();

        PhotonNetwork.Disconnect();

        MainMenuPanel.SetActive(true);
        MatchListPanel.SetActive(false);
        CustomMatchPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        ProfilePanel.SetActive(false);
    }

    public void Host()
    {
        Game.PlayLow();

        CustomMatchPanel.SetActive(true);
        MatchListPanel.SetActive(false);
        ProfilePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
    }

    public void List()
    {
        Game.PlayLow();

        MatchListPanel.SetActive(true);
        ProfilePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        CustomMatchPanel.SetActive(false);
    }

    public void Profile()
    {
        Game.PlayLow();

        ProfilePanel.SetActive(true);
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        MatchListPanel.SetActive(false);
        CustomMatchPanel.SetActive(false);
    }

    public void Settings()
    {
        Game.PlayLow();

        SettingsPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        MatchListPanel.SetActive(false);
        CustomMatchPanel.SetActive(false);
        ProfilePanel.SetActive(false);
    }

    public void BackToMain()
    {
        Game.PlayHigh();

        MainMenuPanel.SetActive(true);
        MatchListPanel.SetActive(false);
        CustomMatchPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        ProfilePanel.SetActive(false);
    }

}
