using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MatchmakingController : MonoBehaviourPunCallbacks
{
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Matchmaking()
    {
        //SearchButton.interactable = false;

        /*Debug.Log("Test1");
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Test2");*/

        //RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        //PhotonNetwork.JoinOrCreateRoom("Room123", roomOpt, new TypedLobby(null, LobbyType.Default));

        /*RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.CreateRoom("Room123", roomOpt);

        Debug.Log("Server: " + "Room123");*/
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Test3");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Test3.5");
        RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.CreateRoom("Room123", roomOpt);

        Debug.Log("Server: " + "Room123");
    }
}
