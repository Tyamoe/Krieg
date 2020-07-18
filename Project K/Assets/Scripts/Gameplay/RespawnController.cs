using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RespawnController : MonoBehaviour
{
    public ModeController ModeCtrl;

    public PlayerController p;

    Dictionary<int, float> here = new Dictionary<int, float>();

    private void Update()
    {
        for (int i = 0; i < here.Count; i++)
        {
            KeyValuePair<int, float> a = here.ElementAt(i);

            here[a.Key] -= Time.deltaTime;
            if(here[a.Key] <= 0.0f)
            {
                PlayerController player = PhotonView.Find(a.Key).GetComponent<PlayerController>();

                //Debug.Log("RespawnController Again");
                if (player)
                {
                    StartCoroutine(Respawn(player, true));

                    Debug.Log("RespawnController Again2");

                    here[a.Key] = 0.65f;
                }
                else
                {
                    Debug.Log("AHHHHHHHHHH");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("OnCollisionEnter");
        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("RespawnController Respawn");
            PlayerController player = collision.transform.GetComponent<PlayerController>();
            p = player;

            if (player)
            {
                StartCoroutine(Respawn(player));

                here[player.photonView.ViewID] = ModeCtrl.RespawnTimer + 0.5f;
            }
            else
            {
                Debug.Log("AHHHHHHHHHH " + collision.name);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        //Debug.Log("OnTriggerExit");

        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("RespawnController Respawn");
            PlayerController player = collision.transform.GetComponent<PlayerController>();
            p = player;

            if (player)
            {
                here.Remove(player.photonView.ViewID);
            }
            else
            {
                Debug.Log("AHHHHHHHHHH");
            }
        }
    }

    IEnumerator Respawn(PlayerController player, bool triggered = false)
    {
        yield return new WaitForSeconds(!triggered ? ModeCtrl.RespawnTimer : 0.5f);

        Transform t = ModeCtrl.GetRespawn();
        player.photonView.RPC("Respawn", RpcTarget.All, t.position, t.rotation);
    }
}
