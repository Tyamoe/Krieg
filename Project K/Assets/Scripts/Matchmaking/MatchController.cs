using UnityEngine;
using Photon.Pun;

public class MatchController : MonoBehaviourPunCallbacks
{
    public int SceneIndex = 1;

    //private bool Loaded = false;

    public override void OnEnable()
    {
        Debug.Log("OnEnable");
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        Debug.Log("OnDisable");
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        /*if(!Loaded)
        {
            Loaded = true;
            PhotonNetwork.LeaveRoom();
            return;
        }*/

        Debug.Log("OnJoinedRoom");

        EnterGame();
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
