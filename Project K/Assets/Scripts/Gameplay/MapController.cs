using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<SpawnController> Respawn_SpawnPoints;
    public Transform RespawnWait;

    List<SpawnController> currRespawn_SpawnPoints;

    Dictionary<int, float> Spawns;

    void Awake()
    {
        RespawnWait = transform.Find("RespawnWait");

        Transform respawn = transform.Find("Respawn");
        Respawn_SpawnPoints = new List<SpawnController>(respawn.GetComponentsInChildren<SpawnController>());
        currRespawn_SpawnPoints = new List<SpawnController>(Respawn_SpawnPoints);
    }

    void Update()
    {

    }

    public Transform GetRespawnWait()
    {
        return RespawnWait;
    }

    public Transform GetRandomSpawn()
    {
        int index = Random.Range(0, Respawn_SpawnPoints.Count);

        SpawnSafety isSafe = Respawn_SpawnPoints[index].isSafe();

        while(isSafe == SpawnSafety.Bad || isSafe == SpawnSafety.No)
        {
            index = Random.Range(0, Respawn_SpawnPoints.Count);
            isSafe = Respawn_SpawnPoints[index].isSafe();
        }

        Transform t = Respawn_SpawnPoints[index].transform;

        //currRespawn_SpawnPoints.RemoveAt(index);

        return t;
    }
}
