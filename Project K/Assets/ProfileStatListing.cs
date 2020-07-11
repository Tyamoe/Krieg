using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileStatListing : MonoBehaviour
{
    public TextMeshProUGUI Id;
    public TextMeshProUGUI Stat;

    public void UpdateListing(string name, string stat)
    {
        Id.text = name;
        Stat.text = stat;
    }
}
