using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FeedListing : MonoBehaviour
{
    public TextMeshProUGUI FeedDetails;
    public Image FeedIcon;

    public void UpdateListing(string details)
    {
        FeedDetails.text = details;
    }
}
