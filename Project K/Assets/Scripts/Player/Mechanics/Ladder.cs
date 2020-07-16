using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    PlayerController player;

    public bool inUse = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<PlayerController>();
        if(player && !inUse)
        {
            player.Climbing = true;
            player.currLadder = this;

            inUse = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        player = other.GetComponent<PlayerController>();
        if (player && inUse)
        {
            player.Climbing = false;
            player.currLadder = null;

            inUse = false;
        }
    }
}
