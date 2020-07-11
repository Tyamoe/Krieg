using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<Transform> Respawn_SpawnPoints;

    List<Transform> currRespawn_SpawnPoints;

    Dictionary<int, float> Spawns;

    void Awake()
    {
        Transform respawn = transform.Find("Respawn");
        Respawn_SpawnPoints = new List<Transform>(respawn.GetComponentsInChildren<Transform>());
        currRespawn_SpawnPoints = new List<Transform>(Respawn_SpawnPoints);
    }

    void Update()
    {

    }

    public Transform GetRandomSpawn()
    {
        int index = Random.Range(0, currRespawn_SpawnPoints.Count);

        Transform t = currRespawn_SpawnPoints[index];

        currRespawn_SpawnPoints.RemoveAt(index);

        return t;
    }
}
