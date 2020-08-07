using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using System;
using System.IO;
using System.Net;
using System.Text;

public class MatchListController : MonoBehaviourPunCallbacks
{
    public List<RoomInfo> matchList;
    public Dictionary<string, GameObject> matchListing;

    public Transform matchContainer;
    public GameObject matchListPrefab;

    void Start()
    {
        matchList = new List<RoomInfo>();
        matchListing = new Dictionary<string, GameObject>();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("MatchListController OnConnectedToMaster");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("MatchListController OnJoinedLobby");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("MatchListController OnRoomListUpdate");

        UpdateMatchListings(roomList);
    }

    public void UpdateMatchListings(List<RoomInfo> roomList)
    {
        for (int i = 0; i < matchContainer.childCount; i++)
        {
            Destroy(matchContainer.GetChild(i).gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            Debug.Log("Match Name: " + room.Name + " Here: " + room.RemovedFromList);
            Debug.Log("Room: " + room.ToStringFull());

            if(room.RemovedFromList && room.MaxPlayers == 0)
            {
                continue;
            }
            
            GameObject listing = Instantiate(matchListPrefab, matchContainer);
            MatchListing m = listing.GetComponent<MatchListing>();

            string roomDetails = "";

            if (room.CustomProperties.ContainsKey("Mode"))
            {
                roomDetails = (string)room.CustomProperties["Mode"];
            }
            else
            {
                roomDetails = "*";
            }

            roomDetails += " on ";

            if (room.CustomProperties.ContainsKey("Map"))
            {
                roomDetails += (string)room.CustomProperties["Map"];
            }
            else
            {
                roomDetails += "*";
            }

            m.UpdateListing(room.Name, room.PlayerCount, room.MaxPlayers, roomDetails);

            matchListing.Add(room.Name, listing);
            matchList.Add(room);

            Debug.Log("Custom: " + room.CustomProperties.ToStringFull());
        }
    }
}
