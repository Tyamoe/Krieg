using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerStats
{
    public PlayerController player;
    public MatchFeedController feedCtrl;
    public TeamController teamCtrl;

    public float score = 0.0f;
}

public class Team
{
    public string Name = "";
    public int ID = 0;

    public float Score = 0.0f;

    public int Count = 0;
    public List<KeyValuePair<int, int>> Members = new List<KeyValuePair<int, int>>(); // id, actor#
}

public class ModeController : MonoBehaviourPunCallbacks, IPunObservable
{
    public string modeName = "FFA";

    [Header("Team")]
    public bool Teams = false;
    public int TeamCount = 0;

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
    public PlayerController player;

    // Private
    MapController Map;

    // Team
    public Dictionary<int, Team> TeamCtrl = new Dictionary<int, Team>();

    int teamNeedsPlayer = -1;

    private GameObject ModeUI;

    private TextMeshProUGUI matchTimerText;
    private TextMeshProUGUI startTimerText;

    private TextMeshProUGUI Team1ScoreText;
    private TextMeshProUGUI Team2ScoreText;

    public int playerCount = 1;
    int myId = -1;
    [HideInInspector]
    public float score = 0.0f;

    private bool Setup = false;
    bool Inited = false;

    public List<int> tempIdList = new List<int>();

    float score1 = 0.0f;
    float score2 = 0.0f;

    // Sync Vars
    private bool Starting = false;
    private bool Started = false;
    private float startTimer = 5.0f;
    private float matchTimer = 5.0f;

    private int syncCount = 0;

    public Dictionary<int, PlayerStats> Scores = new Dictionary<int, PlayerStats>();

    Dictionary<float, int> ScoreOrder = new Dictionary<float, int>();

    // 1
    void Awake()
    {
        Debug.Log("ModeController Awake");
        matchTimer = TimeLimit * 60.0f;

        if(!Teams)
        {

        }
    }

    // 3
    void Start()
    {
        photonView.RPC("SyncPlayerListRPC", RpcTarget.OthersBuffered, myId, player.playerName);

        tempIdList.Add(myId);

        Debug.Log("ModeController Start");

        //if (photonView.IsMine)
        {
            //string msg = player.playerName + " Connected";
            //photonView.RPC("SendToFeed", RpcTarget.AllBuffered, msg);
        }
    }

    // 4
    void Update()
    {
        syncCount++;

        if (Started)
        {
            matchTimer -= Time.deltaTime;

            //foreach (TextMeshProUGUI mText in matchTimers)
            {
                float minutes = Mathf.Floor(matchTimer / 60);
                float seconds = Mathf.Floor(matchTimer % 60);

                if (seconds < 10)
                {
                    matchTimerText.text = minutes.ToString("F0") + ":" + "0" + Mathf.RoundToInt(seconds).ToString();
                }
                else
                {
                    matchTimerText.text = minutes.ToString("F0") + ":" + seconds.ToString();
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

                //foreach (TextMeshProUGUI sText in startTimers)
                {
                    startTimerText.text = startTimer.ToString("F0");
                }

                if(startTimer <= 0.0f)
                {
                    Starting = false;
                    Started = true;

                    //foreach (TextMeshProUGUI sText in startTimers)
                    {
                        startTimerText.transform.parent.gameObject.SetActive(false);
                    }

                    //foreach (PlayerController p in players)
                    {
                        player.PlayerLocked = false;
                        player.PlayerControl = true;

                        Setup = true;
                    }
                }
            }
        }

        if(!Setup && Started)
        {
            //foreach (TextMeshProUGUI sText in startTimers)
            {
                startTimerText.transform.parent.gameObject.SetActive(false);
            }

            //foreach (PlayerController p in players)
            {
                player.PlayerLocked = false;
                player.PlayerControl = true;
            }

            Setup = true;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            foreach(KeyValuePair<int, PlayerStats> p in Scores)
            {
                Debug.Log(p.Key + " is " + p.Value.player.playerName + " : " + p.Value.score.ToString("F0"));
            }
            Debug.Log("|------------------|");
            for (int i = 0; i < TeamCtrl.Count; i++)
            {
                Debug.Log(TeamCtrl[i].Name + ": " + TeamCtrl[i].Count + " | " + (TeamCtrl[i].Count >= 1 ? TeamCtrl[i].Members[0].Key.ToString() : "") + " | " + (TeamCtrl[i].Count >= 2 ? TeamCtrl[i].Members[1].Key.ToString() : ""));
            }
        }
    }

    // 2
    [PunRPC]
    public void Initialize()
    {
        Debug.Log("ModeController Initialize");
        GameObject map = GameObject.FindGameObjectWithTag("Map");
        Map = map.GetComponent<MapController>();
        Map.RespawnCtrl.ModeCtrl = this;

        Transform spawn = Map.GetRandomStartSpawn();
        GameObject playerObj = PhotonNetwork.Instantiate("Player", spawn.position, spawn.rotation);

        PlayerController playerCtrl = playerObj.GetComponent<PlayerController>();
        playerCtrl.modeCtrl = this;
        playerCtrl.mapCtrl = Map;
        playerCtrl.Health = MaxHealth;
        player = playerCtrl;

        myId = playerCtrl.photonView.ViewID;

        Scores[myId] = new PlayerStats();
        Scores[myId].player = player;
        Scores[myId].feedCtrl = player.feedCtrl;
        Scores[myId].teamCtrl = player.teamCtrl;

        GameObject modeUI = playerObj.transform.Find("PlayerUI").Find("InGame").Find("ModeUI").gameObject;
        ModeUI = modeUI;

        matchTimer = TimeLimit * 60.0f;

        // Match Timer
        TextMeshProUGUI m = modeUI.transform.Find("MatchTimer").Find("MatchTimerText").GetComponent<TextMeshProUGUI>();
        matchTimerText = m;
        float minutes = Mathf.Floor(matchTimer / 60);
        float seconds = Mathf.Floor(matchTimer % 60);

        if (seconds < 10)
        {
            m.text = minutes.ToString("F0") + ":" + "0" + Mathf.RoundToInt(seconds).ToString();
        }
        else
        {
            m.text = minutes.ToString("F0") + seconds.ToString();
        }

        // Start Timer
        TextMeshProUGUI s = modeUI.transform.Find("StartTimer").Find("StartTimerText").GetComponent<TextMeshProUGUI>();
        startTimerText = s;
        s.text = startTimer.ToString("F0");

        // Model Label
        TextMeshProUGUI ml = modeUI.transform.Find("ModeLabel").Find("ModeLabelText").GetComponent<TextMeshProUGUI>();
        ml.text = modeName;
        ml = modeUI.transform.Find("ModeLabel").Find("ModeWinCondition").GetComponent<TextMeshProUGUI>();
        ml.text = ScoreLimit.ToString("F0") + " Points";

        // Scores
        TextMeshProUGUI ts1 = modeUI.transform.Find("Team1Score").Find("ScoreText").GetComponent<TextMeshProUGUI>();
        Team1ScoreText = ts1;
        ts1.text = "0";
        TextMeshProUGUI ts2 = modeUI.transform.Find("Team2Score").Find("ScoreText").GetComponent<TextMeshProUGUI>();
        Team2ScoreText = ts2;
        ts2.text = "0";

        string msg = player.playerName + " Connected";
        photonView.RPC("SendToFeed", RpcTarget.AllBuffered, msg);

        photonView.RPC("JoinTeam", RpcTarget.MasterClient, myId, PhotonNetwork.LocalPlayer.ActorNumber);

        // Start
        if (PhotonNetwork.IsMasterClient)
        {
            Starting = true;
        }

        Inited = true;
    }

    [PunRPC]
    void JoinTeam(int playerId, int roomId)
    {
        if(teamNeedsPlayer == -1)
        {
            teamNeedsPlayer = TeamCtrl.Count;

            TeamCtrl[teamNeedsPlayer] = new Team();
            TeamCtrl[teamNeedsPlayer].ID = teamNeedsPlayer;
            TeamCtrl[teamNeedsPlayer].Name = "Team " + teamNeedsPlayer.ToString();
        }

        TeamCtrl[teamNeedsPlayer].Count++;
        TeamCtrl[teamNeedsPlayer].Members.Add(new KeyValuePair<int, int>(playerId, roomId));

        player.teamCtrl.AddTeam(TeamCtrl[teamNeedsPlayer], this);

        photonView.RPC("SyncTeamsRPC", RpcTarget.Others, teamNeedsPlayer, TeamCtrl[teamNeedsPlayer].Count, playerId, roomId);

        if (Teams)
        {
            for (int i = 0; i < TeamCtrl.Count; i++)
            {
                if(TeamCtrl[teamNeedsPlayer].Count >= TeamCtrl[i].Count)
                {
                    teamNeedsPlayer = i;
                }
            }
        }
        else
        {
            teamNeedsPlayer = -1;
        }
    }

    [PunRPC]
    void SyncTeamsRPC(int team, int count, int playerId, int roomId)
    {
        if(count == 1)
            TeamCtrl[team] = new Team();

        TeamCtrl[team].ID = team;
        TeamCtrl[team].Count = count;
        TeamCtrl[team].Members.Add(new KeyValuePair<int, int>(playerId, roomId));
        TeamCtrl[team].Name = "Team " + team.ToString();

        player.teamCtrl.AddTeam(TeamCtrl[team], this);
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
        {
            //StartCoroutine(RespawnPlayer(player));
        }

        Scores[enemyId].score += 1.0f;

        if(myId == enemyId)
        {
            score += 1.0f;
            if(score >= ScoreLimit)
            {
                Debug.Log("Wonnered");
            }

            Team1ScoreText.text = score.ToString("F0");
        }

        Debug.Log("Died: " + enemyId + " | | " + playerId);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Died2: " + Scores[enemyId].player.playerName + " | | " + Scores[playerId].player.playerName);

            string msg = Scores[enemyId].player.playerName + " Killed " + Scores[playerId].player.playerName;
            photonView.RPC("SendToFeed", RpcTarget.All, msg);
        }
    }

    [PunRPC]
    void SendToFeed(string msg)
    {
        Debug.Log("ModeController SendToFeed");
        Scores[myId].feedCtrl.AddFeedListing(msg, Color.black);
    }

    IEnumerator RespawnPlayer(GameObject playerObj)
    {
        Debug.Log("Prep4Resoawned: " + playerObj.GetComponent<PhotonView>().ViewID);
        yield return new WaitForSeconds(RespawnTimer);

        Debug.Log("Resoawned: " + playerObj.GetComponent<PhotonView>().ViewID);

        Transform t = GetRespawn();
        playerObj.GetComponent<PhotonView>().RPC("Respawn", RpcTarget.All, t.position, t.rotation);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("Boob");
        if (!Inited) return;

        if (stream.IsWriting)
        {
            stream.SendNext(Starting);
            stream.SendNext(Started);
            stream.SendNext(startTimer);
            stream.SendNext(matchTimer);

            float s1 = 0.0f;
            float s2 = 0.0f;

            foreach (KeyValuePair<int, PlayerStats> p in Scores)
            {
                float s = Mathf.Min(s1, p.Value.score);
                s1 = Mathf.Max(s1, p.Value.score);
                s2 = Mathf.Max(s2, s);
            }

            stream.SendNext(s1);
            stream.SendNext(s2);

            score1 = s1;
            score2 = s2;

            if(syncCount % 6 == 0)
            {
                stream.SendNext(true);

                //Debug.Log("Hey " + syncCount);

                SyncMinimap();
            }
            else
            {
                stream.SendNext(false);
            }
        }
        else
        {
            Starting = (bool)stream.ReceiveNext();
            Started = (bool)stream.ReceiveNext();
            startTimer = (float)stream.ReceiveNext();
            matchTimer = (float)stream.ReceiveNext();

            score1 = (float)stream.ReceiveNext();
            score2 = (float)stream.ReceiveNext();

            bool b = (bool)stream.ReceiveNext();

            if (b)
            {
                SyncMinimap();
            }
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            if (score1 != score)
            {
                Team2ScoreText.text = score1.ToString("F0");
            }
            else
            {
                Team2ScoreText.text = score2.ToString("F0");
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!Setup && Started)
        {
            //foreach (TextMeshProUGUI sText in startTimers)
            {
                startTimerText.transform.parent.gameObject.SetActive(false);
            }

            //foreach (PlayerController p in players)
            {
                player.PlayerLocked = false;
                player.PlayerControl = true;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        string msg = newPlayer.NickName.Split('#')[0] + " Left The Game";
        player.modeCtrl.photonView.RPC("SendToFeed", RpcTarget.All, msg);

        for(int i = 0; i < TeamCtrl.Count; i++)
        {
            for (int j = 0; j < TeamCtrl[i].Members.Count; j++)
            {
                KeyValuePair<int, int> member = TeamCtrl[i].Members[j];
                if (member.Value == newPlayer.ActorNumber)
                {
                    TeamCtrl[i].Count--;
                    TeamCtrl[i].Members.RemoveAt(j);

                    teamNeedsPlayer = i;

                    if (TeamCtrl[i].Count == 0)
                    {

                    }

                    return;
                }
            }
        }
    }

    void SyncMinimap()
    {
        List<Vector2> pos = new List<Vector2>();

        foreach(KeyValuePair<int, PlayerStats> p in Scores)
        {
            if(p.Key != myId)
                pos.Add(Map.GetMinimapPos(p.Value.player.transform.position));
        }

        player.UpdateMinimap(pos);

        //Debug.Log("Bye " + pos.Count);
    }

    [PunRPC]
    void SyncPlayerListRPC(int id, string name)
    {
        PlayerController p = PhotonView.Find(id).GetComponent<PlayerController>();
        if(p.playerName == "")
        {
            p.playerName = name;
        }
        Scores[id] = new PlayerStats();
        Scores[id].player = p;

        tempIdList.Add(id);
        playerCount++;
    }
}
