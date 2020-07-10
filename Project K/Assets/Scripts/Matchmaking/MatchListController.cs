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
        //PhotonNetwork.NickName = "Player" + UnityEngine.Random.Range(1, 1001);

        Debug.Log("Name: " + PhotonNetwork.NickName);
    }

    public override void OnJoinedLobby()
    {
        //gameObject.SetActive(false);




        //FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.tyamoe.com/krieg/rooms/");
        /*FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://home681734469.1and1-data.host/krieg/rooms/");
        request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

        request.Credentials = new NetworkCredential("u89290814", "VJDaRTKXu9S2mY");

        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream);
        Console.WriteLine(reader.ReadToEnd());

        Console.WriteLine($"Directory List Complete, status {response.StatusDescription}");

        reader.Close();
        response.Close();*/
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");

        UpdateMatchListings(roomList);
    }

    public void UpdateMatchListings(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            Debug.Log("Match Name: " + room.Name);

            if (matchListing != null && matchListing.ContainsKey(room.Name))
            {
                GameObject listing = matchListing[room.Name];
                MatchListing m = listing.GetComponent<MatchListing>();

                m.UpdateListing(room.Name, room.PlayerCount, room.MaxPlayers);
            }
            else
            {
                GameObject listing = Instantiate(matchListPrefab, matchContainer);
                MatchListing m = listing.GetComponent<MatchListing>();

                m.UpdateListing(room.Name, room.PlayerCount, room.MaxPlayers);

                matchListing.Add(room.Name, listing);
            }
        }
    }
}
