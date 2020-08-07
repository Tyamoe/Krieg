using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MatchmakingController : MonoBehaviourPunCallbacks
{
    public MatchListController matchListCtrl;

    [Space]
    public int SceneIndex = 1;

    public override void OnEnable()
    {
        //Debug.Log("OnEnable");
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        //Debug.Log("OnDisable");
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SearchAndJoin()
    {
        bool joined = false;
        if(matchListCtrl.matchList.Count > 0)
        {
            foreach(RoomInfo room in matchListCtrl.matchList)
            {
                if(room.PlayerCount < 0)
                {
                    joined = true;
                    PhotonNetwork.JoinRoom(room.Name);
                    break;
                }
            }

        }

        if (!joined)
        {
            string roomName = "Room" + Random.Range(0, 9).ToString() + Random.Range(0, 9).ToString() + Random.Range(0, 9).ToString() + Random.Range(0, 9).ToString();

            RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOpt, new TypedLobby(null, LobbyType.Default));
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("MatchmakingController OnJoinedRoom");
        Debug.Log(PhotonNetwork.NickName);

        Game.LeftMatch = false;

        // Load Match or Join
        EnterGame();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("MatchmakingController OnJoinRoomFailed");
    }

    void EnterGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("MatchmakingController Creating Game");
            PhotonNetwork.LoadLevel(SceneIndex);
        }
    }
}
