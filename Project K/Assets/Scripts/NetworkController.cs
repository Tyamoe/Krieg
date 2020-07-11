using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public string PlayerName;

    public GameObject PlayerPanel;
    public GameObject LoadingCircle;

    public Transform LocalPlayerList;
    public GameObject LocalPlayerListing;

    public TMP_InputField nameInput;

    public Button CreateButton;

    private void Awake()
    {
        PlayerPanel.SetActive(true);
    }

    void Start()
    {
        string localPlayers = PlayerPrefs.GetString("localPlayers");
        string[] players = localPlayers.Split(',');

        //Debug.Log("All: " + localPlayers);

        foreach (string player in players)
        {
            if (player == "") break;

            //Debug.Log("Each: " + player);

            string[] nameId = player.Split('#');

            GameObject listing = Instantiate(LocalPlayerListing, LocalPlayerList, false);
            LocalPlayer l = listing.GetComponent<LocalPlayer>();

            PlayerName = l.Name.text = nameId[0];
            if (nameId.Length >= 2)
                l.Id = int.Parse(nameId[1]);
        }
    }

    public void CreateLocalPlayer()
    {
        GameObject listing = Instantiate(LocalPlayerListing, LocalPlayerList, false);
        LocalPlayer l = listing.GetComponent<LocalPlayer>();

        PlayerName = l.Name.text = nameInput.text;
        int id = int.Parse(Random.Range(0, 9) + "" + Random.Range(0, 9) + "" + Random.Range(0, 9) + "" + Random.Range(0, 9));

        l.Id = id;

        string localPlayers = PlayerPrefs.GetString("localPlayers");
        localPlayers += PlayerName + "#" + id.ToString() + ",";

        PlayerPrefs.SetString("localPlayers", localPlayers);

        l.SelectLocalPlayer();
    }

    IEnumerator InitializePlayer()
    {
        yield return new WaitForSeconds(1.5f);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ConnectToMasterServer()
    {
        CreateButton.interactable = false;

        LoadingCircle.SetActive(true);

        StartCoroutine(InitializePlayer());
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LoadingCircle.SetActive(false);
        CreateButton.interactable = true;

        PlayerPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        LoadingCircle.SetActive(false);
        PlayerPanel.SetActive(false);

        PhotonNetwork.NickName = PlayerName;

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinLobby();

        Debug.Log("Connected on " + PhotonNetwork.CloudRegion + " :: " + PhotonNetwork.PhotonServerSettings.ToString());
    }
}
