using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MatchmakingController : MonoBehaviourPunCallbacks
{
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

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log(PhotonNetwork.NickName);

        EnterGame();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed");
    }

    void EnterGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(SceneIndex);
            Debug.Log("Creating Game");
        }
    }
}
