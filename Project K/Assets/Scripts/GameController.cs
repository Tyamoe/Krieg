using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;

    void Awake()
    {
        if (!GameObject.Find("Game"))
        {
            GameObject obj = new GameObject("Game");
            obj.AddComponent<Game>();

            Game.Instance.Networked = false;
        }
    }

    void Start()
    {
        Debug.Log("Create PLayer? " + Game.Instance.Networked.ToString());
        if (Game.Instance.Networked)
            CreatePlayer();
    }

    void CreatePlayer()
    {
        //GameObject obj = PhotonNetwork.Instantiate("Player", spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        //obj.GetComponent<PlayerController>().gameCtrl = this;

        Debug.Log("Instantiating Player");
    }
}
