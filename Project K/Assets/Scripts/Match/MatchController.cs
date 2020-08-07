using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MatchController : MonoBehaviourPunCallbacks
{
    [Header("Match")]
    public bool Custom = false;
    public string[] Maps;
    public Sprite[] MapIcons;
    public string[] Modes;
    public TextMeshProUGUI MatchStatus;

    [Header("Ref")]
    public GameObject Lobby;

    [Header("Listing")]
    public Transform MatchPlayerList;
    public GameObject MatchPlayerListing;

    [Header("UI")]
    public TextMeshProUGUI StartMatchText;
    public TextMeshProUGUI BotFillText;
    public TextMeshProUGUI MapNameText;
    public TextMeshProUGUI ModeNameText;

    [Space]
    public TextMeshProUGUI voteStartMatchText;
    public TextMeshProUGUI voteBotFillText;
    public TextMeshProUGUI voteMapChangeText;
    public TextMeshProUGUI voteModeChangeText;

    [Space]
    public Button voteStartMatchButton;
    public Button voteBotFillButton;
    public Button voteMapChangeButton;
    public Button voteModeChangeButton;

    [Space]
    public Image MapIcon;

    // Private
    string map;
    int mapIndex = -1;
    string mode;
    int modeIndex = -1;

    bool starting = false;
    float startTime = 0.0f;

    bool fillBots = false;

    [Space]
    [SerializeField]
    int voteThreshold = 1;

    void Start()
    {
        UpdatePlayerListing();
        UpdateVoteText();

        if (!Custom && PhotonNetwork.IsMasterClient)
        {
            RandomMatch();
        }
    }

    void Update()
    {
        if(starting)
        {
            startTime -= Time.deltaTime;

            if(startTime <= 0.0f)
            {
                startTime = 0.0f;
                starting = false;
                StartMatch();
            }

            StartMatchText.text = "Starting In " + startTime.ToString("F0");

            photonView.RPC("UpdateStartTimeRPC", RpcTarget.Others, startTime);
        }
    }

    [PunRPC]
    void UpdateStartTimeRPC(float startTime_)
    {
        StartMatchText.text = startTime_.ToString("F0");
    }

    public void LeaveRoom()
    {
        if (!starting)
        {
            Game.PlayHigh();
            Game.LeftMatch = true;

            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
        Debug.Log("Leaving Game");
    }

    #region MatchSetup

    public void StartMatch()
    {
        photonView.RPC("StartMatchRPC", RpcTarget.OthersBuffered);
        Lobby.SetActive(false);

        GameObject Map = PhotonNetwork.Instantiate("Maps/" + map, Vector3.zero, Quaternion.identity);
        GameObject Mode = PhotonNetwork.Instantiate("Modes/" + mode, Vector3.zero, Quaternion.identity);

        //ModeController modeCtrl = Mode.GetComponent<ModeController>();
        //modeCtrl.Initialize();

        Mode.GetPhotonView().RPC("Initialize", RpcTarget.AllBuffered);

        if(fillBots)
        {

        }

        voteStartMatchButton.interactable = true;
        voteBotFillButton.interactable = true;
        voteMapChangeButton.interactable = true;
        voteModeChangeButton.interactable = true;
    }

    [PunRPC]
    void StartMatchRPC()
    {
        Lobby.SetActive(false);

        voteStartMatchButton.interactable = true;
        voteBotFillButton.interactable = true;
        voteMapChangeButton.interactable = true;
        voteModeChangeButton.interactable = true;
    }

    public void RandomMatch()
    {
        RandomMap();
        RandomMode();

        ExitGames.Client.Photon.Hashtable p = new ExitGames.Client.Photon.Hashtable();
        p.Add("Map", map);
        p.Add("Mode", mode);

        PhotonNetwork.CurrentRoom.SetCustomProperties(p);

        Debug.Log("CustomMade: " + PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull());
    }

    public void RandomMap()
    {
        if (Maps.Length == 1)
        {
            mapIndex = 0;
        }
        else
        {
            int mapIndex_ = Random.Range(0, Maps.Length);

            while (mapIndex_ == mapIndex)
            {
                mapIndex_ = Random.Range(0, Maps.Length);
            }
            mapIndex = mapIndex_;
        }

        map = Maps[mapIndex];
        MapIcon.sprite = MapIcons[mapIndex];

        MapNameText.text = map;

        voteMapChangeButton.interactable = true;
        voteMapChangeText.text = mapChangeVotes + "/" + voteThreshold;

        photonView.RPC("UpdateMapRPC", RpcTarget.OthersBuffered, mapIndex);
    }

    [PunRPC]
    void UpdateMapRPC(int map_)
    {
        map = Maps[map_];
        MapIcon.sprite = MapIcons[map_];

        MapNameText.text = map;

        voteMapChangeButton.interactable = true;
        voteMapChangeText.text = mapChangeVotes + "/" + voteThreshold;
    }

    public void RandomMode()
    {
        if (Modes.Length == 1)
        {
            modeIndex = 0;
        }
        else
        {
            int modeIndex_ = Random.Range(0, Modes.Length);

            while (modeIndex_ == modeIndex)
            {
                modeIndex_ = Random.Range(0, Maps.Length);
            }

            modeIndex = modeIndex_;
        }

        mode = Modes[modeIndex];

        ModeNameText.text = mode;

        voteModeChangeButton.interactable = true;
        voteModeChangeText.text = modeChangeVotes + "/" + voteThreshold;

        photonView.RPC("UpdateModeRPC", RpcTarget.OthersBuffered, modeIndex);
    }

    [PunRPC]
    void UpdateModeRPC(int mode_)
    {
        mode = Modes[mode_];

        ModeNameText.text = mode;

        voteModeChangeButton.interactable = true;
        voteModeChangeText.text = modeChangeVotes + "/" + voteThreshold;
    }

    #endregion

    #region VoteFuncs

    [SerializeField]
    int startVotes = 0;
    [SerializeField]
    int botFillVotes = 0;
    [SerializeField]
    int mapChangeVotes = 0;
    [SerializeField]
    int modeChangeVotes = 0;

    public void VoteStart()
    {
        Game.PlayHigh();

        voteStartMatchButton.interactable = false;

        startVotes++;
        photonView.RPC("UpdateVoteStart", RpcTarget.OthersBuffered);

        UpdateVoteText();
        //Debug.Log("CustomMade: " + PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull());
    }

    [PunRPC]
    void UpdateVoteStart()
    {
        startVotes++;
        UpdateVoteText();
    }

    public void VoteBotFill()
    {
        Game.PlayHigh();

        voteBotFillButton.interactable = false;

        botFillVotes++;
        photonView.RPC("UpdateBotFill", RpcTarget.OthersBuffered);

        UpdateVoteText();
    }

    [PunRPC]
    void UpdateBotFill()
    {
        botFillVotes++;
        UpdateVoteText();
    }

    public void VoteChangeMap()
    {
        Game.PlayHigh();

        voteMapChangeButton.interactable = false;

        mapChangeVotes++;
        photonView.RPC("UpdateChangeMap", RpcTarget.OthersBuffered);

        UpdateVoteText();
    }

    [PunRPC]
    void UpdateChangeMap()
    {
        mapChangeVotes++;
        UpdateVoteText();
    }

    public void VoteChangeMode()
    {
        Game.PlayHigh();

        voteModeChangeButton.interactable = false;

        modeChangeVotes++;
        photonView.RPC("UpdateChangeMode", RpcTarget.OthersBuffered);

        UpdateVoteText();
    }

    [PunRPC]
    void UpdateChangeMode()
    {
        modeChangeVotes++;
        UpdateVoteText();
    }

    void UpdateVoteText()
    {
        MatchStatus.text = "Searching For Players (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";

        voteStartMatchText.text = startVotes + "/" + voteThreshold;
        voteBotFillText.text = botFillVotes + "/" + voteThreshold;
        voteMapChangeText.text = mapChangeVotes + "/" + voteThreshold;
        voteModeChangeText.text = modeChangeVotes + "/" + voteThreshold;

        // Start
        if (startVotes >= voteThreshold)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("IsMasterClient: " + PhotonNetwork.NickName);
                startTime = 3.0f;
                starting = true;
            }

            startVotes = 0;
            botFillVotes = 0;
            mapChangeVotes = 0;
            modeChangeVotes = 0;

            voteStartMatchButton.interactable = false;
            voteBotFillButton.interactable = false;
            voteMapChangeButton.interactable = false;
            voteModeChangeButton.interactable = false;
        }

        // Bot
        if (botFillVotes >= voteThreshold)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                fillBots = true;
            }

            botFillVotes = 0;

            voteBotFillButton.interactable = false;

            BotFillText.text = "Bot Fill On";
        }

        // Map
        if (mapChangeVotes >= voteThreshold)
        {
            mapChangeVotes = 0;

            if (PhotonNetwork.IsMasterClient)
                RandomMap();
        }

        // Mode
        if (modeChangeVotes >= voteThreshold)
        {
            modeChangeVotes = 0;

            if (PhotonNetwork.IsMasterClient)
                RandomMode();
        }
    }

    [PunRPC]
    void UpdateVoteRPC(int voteThreshold_, int startVotes_, int botFillVotes_, int mapChangeVotes_, int modeChangeVotes_)
    {
        Debug.Log("UpdateVoteTextRPC");
        Debug.Log(PhotonNetwork.NickName);
        Debug.Log("Hekloks: " + voteThreshold_ + " " + startVotes_ + " " + botFillVotes_ + " " + mapChangeVotes_ + " " + modeChangeVotes_);

        voteStartMatchText.text = startVotes_ + "/" + voteThreshold_;
        voteBotFillText.text = botFillVotes_ + "/" + voteThreshold_;
        voteMapChangeText.text = mapChangeVotes_ + "/" + voteThreshold_;
        voteModeChangeText.text = modeChangeVotes_ + "/" + voteThreshold_;

        startVotes = startVotes_;
        botFillVotes = botFillVotes_;
        mapChangeVotes = mapChangeVotes_;
        modeChangeVotes = modeChangeVotes_;

        voteThreshold = voteThreshold_;

        // Start
        if (startVotes >= voteThreshold)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("IsMasterClient: " + PhotonNetwork.NickName);
                startTime = 5.0f;
                starting = true;
            }

            startVotes = 0;
            botFillVotes = 0;
            mapChangeVotes = 0;
            modeChangeVotes = 0;

            voteStartMatchButton.interactable = false;
            voteBotFillButton.interactable = false;
            voteMapChangeButton.interactable = false;
            voteModeChangeButton.interactable = false;
        }

        // Bot
        if (botFillVotes >= voteThreshold)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                fillBots = true;
            }

            botFillVotes = 0;

            voteBotFillButton.interactable = false;

            BotFillText.text = "Bot Fill On";
        }

        // Map
        if (mapChangeVotes >= voteThreshold)
        {
            if (PhotonNetwork.IsMasterClient)
                RandomMap();

            mapChangeVotes = 0;
        }

        // Mode
        if (modeChangeVotes >= voteThreshold)
        {
            if (PhotonNetwork.IsMasterClient)
                RandomMode();

            modeChangeVotes = 0;
        }
    }

    #endregion

    #region Listing

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
        UpdatePlayerListing();

        if (PhotonNetwork.IsMasterClient)
        {
            voteThreshold = PhotonNetwork.PlayerList.Length;

            if (voteThreshold > 2)
            {
                switch (voteThreshold)
                {
                    case 3:
                        voteThreshold = 2;
                        break;
                    case 4:
                        voteThreshold = 2;
                        break;
                    case 5:
                        voteThreshold = 3;
                        break;
                    case 6:
                        voteThreshold = 3;
                        break;
                    case 7:
                        voteThreshold = 4;
                        break;
                    default:
                        voteThreshold = Mathf.FloorToInt(voteThreshold / 2.0f);
                        break;
                }
            }

            photonView.RPC("UpdateThreshold", RpcTarget.OthersBuffered, voteThreshold);
            UpdateVoteText();
        }
    }

    [PunRPC]
    void UpdateThreshold(int thresh)
    {
        voteThreshold = thresh;
        UpdateVoteText();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom");
        UpdatePlayerListing();

        if (PhotonNetwork.IsMasterClient)
        {
            voteThreshold = PhotonNetwork.PlayerList.Length;

            if (voteThreshold > 2)
            {
                switch (voteThreshold)
                {
                    case 3:
                        voteThreshold = 2;
                        break;
                    case 4:
                        voteThreshold = 2;
                        break;
                    case 5:
                        voteThreshold = 3;
                        break;
                    case 6:
                        voteThreshold = 3;
                        break;
                    case 7:
                        voteThreshold = 4;
                        break;
                    default:
                        voteThreshold = Mathf.FloorToInt(voteThreshold / 2.0f);
                        break;
                }
            }

            photonView.RPC("UpdateThreshold", RpcTarget.OthersBuffered, voteThreshold);
            UpdateVoteText();
        }
    }

    private void UpdatePlayerListing()
    {
        for(int i = 0; i < MatchPlayerList.childCount; i++)
        {
            Destroy(MatchPlayerList.GetChild(i).gameObject);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject listing = Instantiate(MatchPlayerListing, MatchPlayerList, false);
            PlayerListing l = listing.GetComponent<PlayerListing>();

            if(player.IsLocal)
            {
                Color c = listing.GetComponent<Image>().color;
                listing.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0.65f);
            }

            l.Name.text = "" + player.NickName.Split('#')[0];
            if (player.CustomProperties.ContainsKey("PlayerDetails"))
            {
                l.Details.text = (string)player.CustomProperties["PlayerDetails"];
            }
            else
            {
                l.Details.text = player.ActorNumber + "";
            }
            //ExitGames.Client.Photon.Hashtable p;
            //player.SetCustomProperties(p);
        }
    }

    #endregion
}
