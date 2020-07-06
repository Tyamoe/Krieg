using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.ConnectToRegion();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        Debug.Log("Connected on " + PhotonNetwork.CloudRegion + " :: " + PhotonNetwork.PhotonServerSettings.ToString());
    }
}
