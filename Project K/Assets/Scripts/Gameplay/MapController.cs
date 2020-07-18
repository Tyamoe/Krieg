using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public float MapSizeX = 220.0f;
    public float MapSizeY = 340.0f;
    public float MinimapSizeX = 700.0f;
    public float MinimapSizeY = 1200.0f;

    [Space]
    public List<SpawnController> Respawn_SpawnPoints;
    public Transform RespawnWait;
    public RespawnController RespawnCtrl;

    List<SpawnController> currRespawn_SpawnPoints;

    Dictionary<int, float> Spawns;

    void Awake()
    {
        RespawnWait = transform.Find("RespawnWait");
        RespawnCtrl = RespawnWait.GetComponent<RespawnController>();

        Transform respawn = transform.Find("Respawn");
        Respawn_SpawnPoints = new List<SpawnController>(respawn.GetComponentsInChildren<SpawnController>());
        currRespawn_SpawnPoints = new List<SpawnController>(Respawn_SpawnPoints);
    }

    void Update()
    {

    }

    float mapToRange(float val, float r1s, float r1e, float r2s, float r2e)
    {
        return (val - r1s) / (r1e - r1s) * (r2e - r2s) + r2s;
    }

    public Vector2 GetMinimapPos(Vector3 playerPos)
    {
        float x = mapToRange(playerPos.x, MapSizeX / -2.0f, MapSizeX / 2.0f, MinimapSizeX / -2.0f, MinimapSizeX / 2.0f);
        float y = mapToRange(playerPos.z, MapSizeY / -2.0f, MapSizeY / 2.0f, MinimapSizeY / -2.0f, MinimapSizeY / 2.0f);

        float xx = playerPos.x / MapSizeX;
        float yy = playerPos.z / MapSizeY;

        xx *= MinimapSizeX;
        yy *= MinimapSizeY;

        return new Vector2(xx, yy);
    }

    public Transform GetRespawnWait()
    {
        return RespawnWait;
    }

    public Transform GetRandomSpawn()
    {
        int index = Random.Range(0, Respawn_SpawnPoints.Count);

        SpawnSafety isSafe = Respawn_SpawnPoints[index].isSafe();

        while(isSafe == SpawnSafety.Bad || isSafe == SpawnSafety.No || isSafe == SpawnSafety.OK)
        {
            index = Random.Range(0, Respawn_SpawnPoints.Count);
            isSafe = Respawn_SpawnPoints[index].isSafe();
        }

        Transform t = Respawn_SpawnPoints[index].transform;

        //currRespawn_SpawnPoints.RemoveAt(index);

        return t;
    }
}
