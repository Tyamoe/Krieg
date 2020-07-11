using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerListing : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Details;

    public void UpdateListing(string pName, string pDet)
    {
        Name.text = pName;
        Details.text = pDet;
    }
}
