using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreboardListing : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI Ping;

    [Space]
    public PlayerController player;

    public int id;
    public ModeController modeCtrl;

    public void UpdateListing(PlayerController player_, ModeController mode)
    {
        modeCtrl = mode;

        player = player_;
        id = player.photonView.ViewID;

        Name.text = player.playerName;
    }

    bool post = true;
    void FixedUpdate()
    {
        if(post)
        {
            if(!player)
            {
                Destroy(this.gameObject);
            }
            Score.text = modeCtrl.Scores[id].score.ToString("F0");
            Ping.text = player.Ping.ToString();
        }
        post = !post;
    }
}
