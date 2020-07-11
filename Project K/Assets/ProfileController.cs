using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProfileController : MonoBehaviour
{
    public Transform ListingContainer;
    public GameObject StatListing;

    string[] statId =
    {
        "Kills",
        "Deaths",
        "Games Played",
    };

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        for (int i = 0; i < ListingContainer.childCount; i++)
        {
            Destroy(ListingContainer.GetChild(i).gameObject);
        }

        string rawStat = PlayerPrefs.GetString(PhotonNetwork.NickName + "Stats", "0,0:1,0:2,0");

        string[] stats = rawStat.Split(':');

        foreach(string stat in stats)
        {
            string[] s = stat.Split(',');
            int index = int.Parse(s[0]);

            GameObject listing = Instantiate(StatListing, ListingContainer);
            ProfileStatListing sp = listing.GetComponent<ProfileStatListing>();

            sp.UpdateListing(statId[index], s[1]);
        }
    }
}
