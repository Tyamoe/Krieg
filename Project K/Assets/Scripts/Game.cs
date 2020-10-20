using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Game : MonoBehaviour
{
    public bool Networked;
    public static bool LeftMatch = false;

    private static string PlayerNameID = "";
    private static string PlayerName = "";
    private static string PlayerId = "";

    public static string Name
    {
        get { return PlayerName;  }
        set
        {
            PlayerNameID = value.Split('#')[1];
            PlayerName = value.Split('#')[0];
        }
    }

    public static string ID
    {
        get
        {
            if (PlayerId != "" && PhotonNetwork.IsConnected)
                return PlayerId;
            else if (!PhotonNetwork.IsConnected)
                return "";
            else
            {
                PlayerId = GenID();
                return PlayerId;
            }
        }
        set { PlayerId = value; }
    }

    [Space]
    public AudioClip ButtonHighSFX;
    public AudioClip ButtonLowSFX;

    private static AudioSource audioSource;
    private static AudioClip buttonHighSFX;
    private static AudioClip buttonLowSFX;

    private static Game _instance;

    public static Game Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        PhotonNetwork.SendRate = 42;
        PhotonNetwork.SerializationRate = 42;

        audioSource = GetComponent<AudioSource>();

        buttonHighSFX = ButtonHighSFX;
        buttonLowSFX = ButtonLowSFX;

        //WebGLInput.captureAllKeyboardInput = true;
    }

    private static float mapToRange(float val, float r1s, float r1e, float r2s, float r2e)
    {
        return (val - r1s) / (r1e - r1s) * (r2e - r2s) + r2s;
    }

    private static string GenID()
    {
        if (PlayerName == "") return "";

        string id = "";

        int j = 0;
        foreach(char c in PlayerName)
        {
            int i = (int)c;
            i = Mathf.CeilToInt(mapToRange((float)i, 0.0f, 300.0f, 0.0f, 10000.0f) / 7.0f);
            i |= 10 + j;

            id += i.ToString();

            j++;
        }
        id += "l";
        j = 0;
        foreach (char c in PlayerNameID)
        {
            int i = (int)c;
            i |= 7 + j * (i / 2);

            id += i.ToString();

            j++;
        }

        return id;
    }

    public static void PlayHigh(float vol = 0.2f)
    {
        audioSource.PlayOneShot(buttonHighSFX, vol);
    }
    public static void PlayLow(float vol = 0.2f)
    {
        audioSource.PlayOneShot(buttonHighSFX, vol);
    }

    void Start()
    {

    }

    private void OnApplicationQuit()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }

    public void ExitCalled()
    { 
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
    }
}
