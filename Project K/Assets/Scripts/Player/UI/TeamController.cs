using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TeamController : MonoBehaviourPunCallbacks
{
    public ModeController modeCtrl;

    //public List<>
    [Space]
    public Transform TeamsContainer;
    public GameObject teamListing;

    public void UpdateTeamsListing()
    {
        for (int i = 0; i < TeamsContainer.childCount; i++)
        {
            Destroy(TeamsContainer.GetChild(i).gameObject);
        }
        
        foreach(KeyValuePair<int, Player> p in PhotonNetwork.CurrentRoom.Players)
        {

        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

    }
}
