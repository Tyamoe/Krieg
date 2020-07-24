using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeamListing : MonoBehaviour
{
    public TextMeshProUGUI TeamName;

    public void UpdateListing(string t)
    {
        TeamName.text = t;
    }
}
