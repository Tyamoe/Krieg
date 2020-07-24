using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ScoreboardController : MonoBehaviour
{
    public Transform ScoreboardContainer;
    public GameObject scoreboardListing;

    [Space]
    public List<ScoreboardListing> scoreboardList = new List<ScoreboardListing>();

    public void AddListing(int pId, ModeController mode)
    {
        GameObject listing = Instantiate(scoreboardListing, ScoreboardContainer, false);
        ScoreboardListing l = listing.GetComponent<ScoreboardListing>();

        if (PhotonView.Find(pId))
            l.UpdateListing(PhotonView.Find(pId).GetComponent<PlayerController>(), mode);
        else
            Debug.Log("uhhh " + pId);

        scoreboardList.Add(l);
    }
}
