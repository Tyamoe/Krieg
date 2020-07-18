using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum SpawnSafety
{
    Good = 0,
    OK = 1,
    Bad = 2,
    No = 3,
}

public class SpawnController : MonoBehaviour
{
    Dictionary<int, PlayerController> NearbyEnemies = new Dictionary<int, PlayerController>();

    public PlayerController pp;

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("SpawnController");
        if (collider.gameObject.name.Contains("Player"))
        {
            PlayerController player = collider.gameObject.GetComponent<PlayerController>();

            pp = player;

            // If Enemy
            NearbyEnemies[player.photonView.ViewID] = player;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        //Debug.Log("SpawnControllere");
        if (collider.gameObject.name.Contains("Player"))
        {
            PlayerController player = collider.gameObject.GetComponent<PlayerController>();

            pp = player;

            if (NearbyEnemies.ContainsKey(player.photonView.ViewID))
                NearbyEnemies.Remove(player.photonView.ViewID);
        }
    }

    public SpawnSafety isSafe()
    {
        if(NearbyEnemies.Count > 3)
        {
            return SpawnSafety.No;
        }

        return (SpawnSafety)NearbyEnemies.Count;
    }
}
