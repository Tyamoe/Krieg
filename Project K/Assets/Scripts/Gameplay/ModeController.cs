using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ModeController : MonoBehaviourPunCallbacks, IPunObservable
{
    public string modeName = "FFA";

    [Header("Team")]
    public bool Teams = true;

    [Header("Respawn")]
    public bool Respawn = true;
    public bool TeamSpawn = false;
    public float RespawnTimer = 2.0f;

    [Header("Scoring")]
    public float TimeLimit = 5.0f; // Minutes
    public float ScoreLimit = 30.0f;

    [Header("Health")]
    public int MaxHealth = 100;
    public bool Regen = true;
    public float RegenTime = 1.5f;
    public float RegenTimeReset = 2.0f;
    public int RegenAmount = 15;

    [Header("Ref")]
    public List<PlayerController> players = new List<PlayerController>();

    // Private
    MapController Map;

    //Dictionary<int, Score> Scores = new Dictionary<int, Score>();
    int myId = -1;
    float score = 0.0f;

    private List<GameObject> modeUI = new List<GameObject>();
    private List<TextMeshProUGUI> matchTimers = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> startTimers = new List<TextMeshProUGUI>();

    private List<TextMeshProUGUI> Team1Score = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> Team2Score = new List<TextMeshProUGUI>();

    // Sync Vars
    private bool Starting = false;
    private bool Started = false;
    private float startTimer = 5.0f;
    private float matchTimer = 5.0f;

    private void Start()
    {
        matchTimer = TimeLimit * 60.0f;
    }

    void Update()
    {
        if(Started)
        {
            matchTimer -= Time.deltaTime;

            foreach (TextMeshProUGUI mText in matchTimers)
            {
                float minutes = Mathf.Floor(matchTimer / 60);
                float seconds = Mathf.RoundToInt(matchTimer % 60);

                if (seconds < 10)
                {
                    mText.text = minutes.ToString("F0") + ":" + "0" + Mathf.RoundToInt(seconds).ToString();
                }
                else
                {
                    mText.text = minutes.ToString("F0") + ":" + seconds.ToString();
                }
            }

            if(matchTimer <= 0.0f)
            {
                Started = false;
            }
        }
        else
        {
            if(Starting)
            {
                startTimer -= Time.deltaTime;

                foreach (TextMeshProUGUI sText in startTimers)
                {
                    sText.text = startTimer.ToString("F0");
                }

                if(startTimer <= 0.0f)
                {
                    Starting = false;
                    Started = true;

                    foreach (TextMeshProUGUI sText in startTimers)
                    {
                        sText.transform.parent.gameObject.SetActive(false);
                    }

                    foreach (PlayerController p in players)
                    {
                        p.PlayerLocked = false;
                        p.PlayerControl = true;
                    }
                }
            }
        }
    }

    [PunRPC]
    public void Initialize()
    {
        GameObject map = GameObject.FindGameObjectWithTag("Map");
        Map = map.GetComponent<MapController>();

        Transform spawn = Map.GetRandomSpawn();
        GameObject obj = PhotonNetwork.Instantiate("Player", spawn.position, spawn.rotation);

        PlayerController p = obj.GetComponent<PlayerController>();
        p.modeCtrl = this;
        p.Health = MaxHealth;
        players.Add(p);

        myId = p.photonView.ViewID;

        //PhotonNetwork.LocalPlayer.TagObject = p.gameObject;

        /*Debug.Log("1");
        ExitGames.Client.Photon.Hashtable playerHash = new ExitGames.Client.Photon.Hashtable();
        Debug.Log("2");
        playerHash.Add("playerObject", playerHash);
        Debug.Log("3");

        PhotonNetwork.SetPlayerCustomProperties(playerHash);
        Debug.Log("4");*/

        GameObject o = obj.transform.Find("PlayerUI").Find("InGame").Find("ModeUI").gameObject;
        modeUI.Add(o);

        matchTimer = TimeLimit * 60.0f;

        // Match Timer
        TextMeshProUGUI m = o.transform.Find("MatchTimer").Find("MatchTimerText").GetComponent<TextMeshProUGUI>();
        matchTimers.Add(m);
        float minutes = Mathf.Floor(matchTimer / 60);
        float seconds = Mathf.RoundToInt(matchTimer % 60);

        if (seconds < 10)
        {
            m.text = minutes.ToString("F0") + ":" + "0" + Mathf.RoundToInt(seconds).ToString();
        }
        else
        {
            m.text = minutes.ToString("F0") + ":" + seconds.ToString();
        }

        // Start Timer
        TextMeshProUGUI s = o.transform.Find("StartTimer").Find("StartTimerText").GetComponent<TextMeshProUGUI>();
        startTimers.Add(s);
        s.text = startTimer.ToString("F0");

        // Model Label
        TextMeshProUGUI ml = o.transform.Find("ModeLabel").Find("ModeLabelText").GetComponent<TextMeshProUGUI>();
        ml.text = modeName;
        ml = o.transform.Find("ModeLabel").Find("ModeWinCondition").GetComponent<TextMeshProUGUI>();
        ml.text = ScoreLimit.ToString("F0") + " Points";

        // Scores
        TextMeshProUGUI ts1 = o.transform.Find("Team1Score").Find("ScoreText").GetComponent<TextMeshProUGUI>();
        Team1Score.Add(ts1);
        ts1.text = "0";
        TextMeshProUGUI ts2 = o.transform.Find("Team2Score").Find("ScoreText").GetComponent<TextMeshProUGUI>();
        Team2Score.Add(ts2);
        ts2.text = "0";

        // Start
        if (PhotonNetwork.IsMasterClient)
        {
            Starting = true;
        }
    }

    public Transform GetRespawn()
    {
        return Map.GetRandomSpawn();
    }

    [PunRPC]
    public void PlayerDied(int playerId, int enemyId)
    {
        GameObject player = PhotonView.Find(playerId).gameObject;//(GameObject)playerObj.CustomProperties["playerObject"];
        if(player)
        {
            player.transform.position = Map.GetRespawnWait().position;
            player.transform.rotation = Map.GetRespawnWait().rotation;
        }
        else
        {
            Debug.Log("Oof");
        }

        if(PhotonNetwork.IsMasterClient)
            StartCoroutine(RespawnPlayer(player));

        if(myId == enemyId)
        {
            score += 1.0f;
            if(score >= ScoreLimit)
            {
                Debug.Log("Wonnered");
            }

            Team1Score[0].text = score.ToString("F0");
        }
    }

    IEnumerator RespawnPlayer(GameObject playerObj)
    {
        yield return new WaitForSeconds(RespawnTimer);

        Transform t = GetRespawn();
        playerObj.GetComponent<PhotonView>().RPC("Respawn", RpcTarget.AllBuffered, t.position, t.rotation);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Starting);
            stream.SendNext(Started);
            stream.SendNext(startTimer);
            stream.SendNext(matchTimer);
        }
        else
        {
            Starting = (bool)stream.ReceiveNext();
            Started = (bool)stream.ReceiveNext();
            startTimer = (float)stream.ReceiveNext();
            matchTimer = (float)stream.ReceiveNext();
        }
    }
}
