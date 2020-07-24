using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TeamController : MonoBehaviourPunCallbacks
{
    public ModeController modeCtrl;

    public List<Team> teamList = new List<Team>();

    [Space]
    public Transform TeamsContainer;
    public GameObject teamListing;

    public void AddTeam(Team newTeam, ModeController mode)
    {
        teamList.Add(newTeam);

        GameObject listing = Instantiate(teamListing, TeamsContainer, false);
        TeamListing l = listing.GetComponent<TeamListing>();
        ScoreboardController s = listing.GetComponent<ScoreboardController>();

        l.UpdateListing(newTeam.Name);

        for(int i = 0; i < newTeam.Members.Count; i++)
        {
            s.AddListing(newTeam.Members[i].Key, mode);
        }
    }

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
