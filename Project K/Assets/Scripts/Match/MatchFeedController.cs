using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFeedController : MonoBehaviour
{
    public Transform FeedContainer;
    public GameObject feedListing;

    public void AddFeedListing(string details, Color color)
    {
        GameObject listing = Instantiate(feedListing, FeedContainer, false);
        FeedListing l = listing.GetComponent<FeedListing>();

        l.FeedDetails.color = color;
        l.UpdateListing(details);

        if(FeedContainer.childCount > 6)
        {
            Destroy(FeedContainer.GetChild(0).gameObject);
        }

        Destroy(listing, 6.5f);
    }
}
