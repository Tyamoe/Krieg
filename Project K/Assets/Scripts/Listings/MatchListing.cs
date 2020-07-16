using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchListing : MonoBehaviourPunCallbacks
{
    public string matchName = "";
    public int playerCount = 0;
    public int maxPlayerCount = 0;

    public string mapName;

    public Text nameDisplay;
    public Text countDisplay;
    public Text mapNameDisplay;

    void Start()
    {
        nameDisplay.text = matchName;
        countDisplay.text = playerCount + "/" + maxPlayerCount;
    }

    void Update()
    {
        
    }

    public void UpdateListing(string uname, int count, int mcount, string details)
    {
        matchName = uname;
        playerCount = count;
        maxPlayerCount = mcount;

        nameDisplay.text = matchName;
        countDisplay.text = playerCount + "/" + maxPlayerCount;
        mapNameDisplay.text = details;
    }

    public void JoinMatch()
    {
        PhotonNetwork.JoinRoom(matchName);
    }
}
