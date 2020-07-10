using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalPlayer : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public int Id;

    public void SelectLocalPlayer()
    {
        NetworkController net = GameObject.Find("NetworkController").GetComponent<NetworkController>();
        net.PlayerName = Name.text + "#" + Id.ToString();
        net.ConnectToMasterServer();
    }
}
