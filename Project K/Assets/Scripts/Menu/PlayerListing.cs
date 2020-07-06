using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{
    public Text playerName;

    public void UpdateListing(string pName)
    {
        playerName.text = pName;
    }
}
