using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Game : MonoBehaviour
{
    public bool Networked;

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

        PhotonNetwork.SendRate = 64;
        PhotonNetwork.SerializationRate = 64;

        audioSource = GetComponent<AudioSource>();

        buttonHighSFX = ButtonHighSFX;
        buttonLowSFX = ButtonLowSFX;
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
}
