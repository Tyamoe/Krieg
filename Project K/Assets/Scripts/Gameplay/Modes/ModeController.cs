using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ModeController : MonoBehaviourPunCallbacks
{
    [Header("Respawn")]
    public bool Respawn;
    public bool TeamSpawn;
    public float RespawnTimer;

    [Header("Scoring")]
    public float TimeLimit;
    public float ScoreLimit;

    [Header("Ref")]
    public PlayerController[] players;

    // Private
    MapController Map;

    [PunRPC]
    public void Initialize()
    {
        GameObject map = GameObject.FindGameObjectWithTag("Map");
        Map = map.GetComponent<MapController>();

        Transform spawn = Map.GetRandomSpawn();
        GameObject obj = PhotonNetwork.Instantiate("Player", spawn.position, spawn.rotation);
        obj.GetComponent<PlayerController>().modeCtrl = this;
    }

    public Transform GetRespawn()
    {
        return Map.GetRandomSpawn();
    }

    [PunRPC]
    public void PlayerDied(GameObject playerObj)
    {

    }
}
