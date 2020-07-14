using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Game : MonoBehaviour
{
    public bool Networked;

    private static Game _instance;

    public static Game Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        PhotonNetwork.SendRate = 64;
        PhotonNetwork.SerializationRate = 64;
    }

    void Start()
    {

    }
}
