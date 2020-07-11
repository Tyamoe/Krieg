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

        if (!Custom)
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

            StartMatchText.text = startTime.ToString("F0");
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
        Debug.Log("Leaving Game");
    }

    #region MatchSetup

    public void StartMatch()
    {
        Lobby.SetActive(false);

        GameObject Map = PhotonNetwork.Instantiate("Maps/" + map, Vector3.zero, Quaternion.identity);
        GameObject Mode = PhotonNetwork.Instantiate("Modes/" + mode, Vector3.zero, Quaternion.identity);

        //ModeController modeCtrl = Mode.GetComponent<ModeController>();
        //modeCtrl.Initialize();

        Mode.GetPhotonView().RPC("Initialize", RpcTarget.AllBuffered);
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
        startVotes++;
        if(startVotes >= voteThreshold)
        {
            startTime = 5.0f;
            starting = true;

            startVotes = 0;
            botFillVotes = 0;
            mapChangeVotes = 0;
            modeChangeVotes = 0;

            voteStartMatchButton.interactable = false;
            voteBotFillButton.interactable = false;
            voteMapChangeButton.interactable = false;
            voteModeChangeButton.interactable = false;
        }

        UpdateVoteText();
        Debug.Log("CustomMade: " + PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull());
    }

    public void VoteBotFill()
    {
        botFillVotes++;
        if (botFillVotes >= voteThreshold)
        {
            botFillVotes = 0;
            fillBots = true;

            voteBotFillButton.interactable = false;

            BotFillText.text = "Bot Fill On";
        }

        UpdateVoteText();
    }

    public void VoteChangeMap()
    {
        mapChangeVotes++;
        if (mapChangeVotes >= voteThreshold)
        {
            mapChangeVotes = 0;
            RandomMap();
        }

        UpdateVoteText();
    }

    public void VoteChangeMode()
    {
        modeChangeVotes++;
        if (modeChangeVotes >= voteThreshold)
        {
            modeChangeVotes = 0;
            RandomMode();
        }

        UpdateVoteText();
    }

    void UpdateVoteText()
    {
        Debug.Log("UpdateVoteText");
        voteStartMatchText.text = startVotes + "/" + voteThreshold;
        voteBotFillText.text = botFillVotes + "/" + voteThreshold;
        voteMapChangeText.text = mapChangeVotes + "/" + voteThreshold;
        voteModeChangeText.text = modeChangeVotes + "/" + voteThreshold;
    }

    #endregion

    #region Listing

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerListing();

        voteThreshold = PhotonNetwork.PlayerList.Length;

        if(voteThreshold > 2)
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

        UpdateVoteText();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerListing();

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

        UpdateVoteText();
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
