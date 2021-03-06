﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public enum KeyActions
{
    Forward = 0,
    Back = 1,
    Left = 2,
    Right = 3,

    Sprint = 4,
    Jump = 5,
    Crouch = 6,

    Reload = 7,
    Use = 8,

    Grenade = 9,

    ADS = 10,
    Fire = 11,

    SwapDown = 12,
    SwapUp = 13,

    Menu = 14,
    Scoreboard = 15,
}

public class PlayerSettingsFunctions : MonoBehaviourPunCallbacks
{
    public PlayerController player;

    [Header("General")]
    public GameObject SaveSettingsButton;

    [Header("Tabs")]
    public GameObject GameplayTabButton;
    public GameObject AudioTabButton;
    public GameObject KeybindTabButton;
    public GameObject PerformanceTabButton;

    [Space]
    public GameObject GameplayTabPanel;
    public GameObject AudioTabPanel;
    public GameObject KeybindTabPanel;
    public GameObject PerformanceTabPanel;

    [Header("Gameplay Tab")]
    public Slider FovSlider;
    public Slider BrightnessSlider;

    [Space]
    public Slider SensXSlider;
    public Slider SensYSlider;
    public Slider MultiXSlider;
    public Slider MultiYSlider;

    [Space]
    public Toggle ADSToggle;
    public Toggle CrouchToggle;

    [Space]
    public TextMeshProUGUI FovDisplay;
    public TextMeshProUGUI BrightnessDisplay;
    public TextMeshProUGUI SensXDisplay;
    public TextMeshProUGUI SensYDisplay;
    public TextMeshProUGUI MultiXDisplay;
    public TextMeshProUGUI MultiYDisplay;

    [Header("Audio Tab")]
    public Slider MasterSlider;
    public Slider UISlider;
    public TextMeshProUGUI MasterDisplay;
    public TextMeshProUGUI UIDisplay;

    [Header("Keybinds Tab")]
    public TextMeshProUGUI JumpInput;
    public GameObject[] KeyInputs;

    [Header("Performance Tab")]
    public Slider ResolutionSlider;
    public Toggle VSyncToggle;
    public Slider RenderSlider;
    public GameObject ModelQuality;
    public GameObject TextureQuality;
    public GameObject Shadows;
    public GameObject ShadowQual;
    public GameObject MSAAQuality;
    public GameObject ANISQuality;
    public Slider ShadowSlider;
    public GameObject ShadowCascades;
    public Toggle AmbientOcToggle;
    public Toggle DepthOfFieldToggle;

    [Space]
    public TextMeshProUGUI ResolutionDisplay;
    public TextMeshProUGUI RenderDisplay;
    public TextMeshProUGUI ShadowDisplay;

    [Header("Ref")]
    public RenderController renderCtrl;

    struct Settings
    {
        // Gameplay
        public float fov;
        public float brightness;

        public float SensitivityX;
        public float SensitivityY;
        public float ADSMultiplierX;
        public float ADSMultiplierY;

        public bool toggleADS;
        public bool toggleCrouch;

        // Audio
        public float masterVolume;
        public float uiVolume;

        // Performance
        public float resolutionScale;
        public bool toggleVSync;

        public float renderDistance;
        public int modelQuality;
        public int textureQuality;
        public int shadows;
        public int shadowQuality;
        public int MSAA;
        public int anisotropicFiltering;
        public float shadowDistance;
        public int shadowCascades;

        public bool ambientOcclusion;
        public bool DOF;
    }

    Dictionary<KeyCode, bool> bannedKeys = new Dictionary<KeyCode, bool>()
    {
        { KeyCode.LeftControl, true },
        { KeyCode.RightControl, true },
        { KeyCode.F11, true },
        { KeyCode.F12, true },

        { KeyCode.Alpha1, true },
        { KeyCode.Alpha2, true },
        { KeyCode.Alpha3, true },
        { KeyCode.Alpha4, true },
    };

    Dictionary<KeyActions, KeyCode> Keybinds = new Dictionary<KeyActions, KeyCode>()
    {
        { KeyActions.Forward, KeyCode.W },
        { KeyActions.Back, KeyCode.S },
        { KeyActions.Left, KeyCode.A },
        { KeyActions.Right, KeyCode.D },

        { KeyActions.Sprint, KeyCode.LeftShift },
        { KeyActions.Jump, KeyCode.Space },
        { KeyActions.Crouch, KeyCode.C },

        { KeyActions.Reload, KeyCode.R },
        { KeyActions.Use, KeyCode.F },

        { KeyActions.Grenade, KeyCode.G },

        { KeyActions.ADS, KeyCode.Mouse1 },
        { KeyActions.Fire, KeyCode.Mouse0 },

        { KeyActions.SwapDown, KeyCode.Q },
        { KeyActions.SwapUp, KeyCode.E },

        { KeyActions.Menu, KeyCode.Escape },
        { KeyActions.Scoreboard, KeyCode.Tab },
    };

    bool rebindActive = false;
    KeyActions currRebind = KeyActions.Forward;

    Settings settings;

    bool Started = false;

    void Awake()
    {
        RenderSettings.ambientLight = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        settings = new Settings();

        Started = false;
        LoadSaved();
        GameplayTab();
    }

    void Start()
    {
        Started = true;
    }

    void Update()
    {
        if(rebindActive)
        {
            if (Input.anyKeyDown)
            {
                KeyCode rawCode = KeyCode.A;
                foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kcode))
                    {
                        rawCode = kcode;
                        Debug.Log("KeyCode down: " + kcode);
                        break;
                    }
                }

                if (rawCode == Keybinds[currRebind])
                {
                    rebindActive = false;

                    KeyInputs[(int)currRebind].GetComponent<Image>().fillCenter = true;
                    KeyInputs[(int)currRebind].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

                    gameObject.GetComponent<Image>().raycastTarget = true;
                }
                else if(bannedKeys.ContainsKey(rawCode))
                {
                    StartCoroutine(DoSomething(KeyInputs[(int)currRebind].GetComponentInChildren<TextMeshProUGUI>(), "Invalid"));
                }
                else if (Keybinds.ContainsValue(rawCode))
                {
                    StartCoroutine(DoSomething(KeyInputs[(int)currRebind].GetComponentInChildren<TextMeshProUGUI>(), "Used"));
                }
                else
                {
                    rebindActive = false;

                    KeyInputs[(int)currRebind].GetComponent<Image>().fillCenter = true;
                    KeyInputs[(int)currRebind].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                    KeyInputs[(int)currRebind].GetComponentInChildren<TextMeshProUGUI>().text = rawCode.ToString();

                    Keybinds[currRebind] = rawCode;

                    gameObject.GetComponent<Image>().raycastTarget = true;

                    SettingsChanged();
                }
            }

            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                rebindActive = false;

                KeyInputs[(int)currRebind].GetComponent<Image>().fillCenter = true;
                KeyInputs[(int)currRebind].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

                gameObject.GetComponent<Image>().raycastTarget = true;
            }
        }
    }

    IEnumerator DoSomething(TextMeshProUGUI tmp, string msg)
    {
        string prev = tmp.text;
        tmp.text = msg;
        yield return new WaitForSeconds(0.25f);
        tmp.text = prev;
    }

    public override void OnDisable()
    {
        Started = false;
        LoadSaved();
        Started = true;
    }

    public override void OnEnable()
    {
        Vector3 v = GameplayTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        GameplayTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);

        v = AudioTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        AudioTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);

        v = KeybindTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        KeybindTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);

        v = PerformanceTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        PerformanceTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);

        //GameplayTab();
    }

    void LoadSaved()
    {
        // Gameplay
        settings.fov = PlayerPrefs.GetFloat(Game.ID + "fov", 75);
        settings.brightness = PlayerPrefs.GetFloat(Game.ID + "brightness", 70);

        settings.SensitivityX = PlayerPrefs.GetFloat(Game.ID + "SensitivityX", 1.0f);
        settings.SensitivityY = PlayerPrefs.GetFloat(Game.ID + "SensitivityY", 1.0f);
        settings.ADSMultiplierX = PlayerPrefs.GetFloat(Game.ID + "ADSMultiplierX", 1.0f);
        settings.ADSMultiplierY = PlayerPrefs.GetFloat(Game.ID + "ADSMultiplierY", 1.0f);

        settings.toggleADS = PlayerPrefs.GetInt(Game.ID + "toggleADS", 1) == 0 ? false : true;
        settings.toggleCrouch = PlayerPrefs.GetInt(Game.ID + "toggleCrouch", 0) == 0 ? false : true;

        // Set UI Elements
        FovSlider.value = settings.fov;
        BrightnessSlider.value = settings.brightness;

        SensXSlider.value = settings.SensitivityX;
        SensYSlider.value = settings.SensitivityY;
        MultiXSlider.value = settings.ADSMultiplierX;
        MultiYSlider.value = settings.ADSMultiplierY;

        ADSToggle.isOn = settings.toggleADS;
        if (player)
        {
            player.toggleADS = settings.toggleADS;
        }

        CrouchToggle.isOn = settings.toggleCrouch;

        //-----------------------------------------------------------------//
        //*****************************************************************//
        //-----------------------------------------------------------------//

        // Keybind
        Keybinds[KeyActions.Forward] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Forward", (int)Keybinds[KeyActions.Forward]);
        Keybinds[KeyActions.Back] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Back", (int)Keybinds[KeyActions.Back]);
        Keybinds[KeyActions.Left] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Left", (int)Keybinds[KeyActions.Left]);
        Keybinds[KeyActions.Right] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Right", (int)Keybinds[KeyActions.Right]);

        Keybinds[KeyActions.Sprint] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Sprint", (int)Keybinds[KeyActions.Sprint]);
        Keybinds[KeyActions.Jump] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Jump", (int)Keybinds[KeyActions.Jump]);
        Keybinds[KeyActions.Crouch] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Crouch", (int)Keybinds[KeyActions.Crouch]);

        Keybinds[KeyActions.Reload] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Reload", (int)Keybinds[KeyActions.Reload]);
        Keybinds[KeyActions.Use] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Use", (int)Keybinds[KeyActions.Use]);

        Keybinds[KeyActions.Grenade] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Grenade", (int)Keybinds[KeyActions.Grenade]);

        Keybinds[KeyActions.ADS] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "ADS", (int)Keybinds[KeyActions.ADS]);
        Keybinds[KeyActions.Fire] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Fire", (int)Keybinds[KeyActions.Fire]);

        Keybinds[KeyActions.SwapDown] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "SwapDown", (int)Keybinds[KeyActions.SwapDown]);
        Keybinds[KeyActions.SwapUp] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "SwapUp", (int)Keybinds[KeyActions.SwapUp]);

        Keybinds[KeyActions.Menu] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Menu", (int)Keybinds[KeyActions.Menu]);
        Keybinds[KeyActions.Scoreboard] = (KeyCode)PlayerPrefs.GetInt(Game.ID + "Scoreboard", (int)Keybinds[KeyActions.Scoreboard]);

        for(int i = 0; i < System.Enum.GetValues(typeof(KeyActions)).Length; i++)
        {
            KeyInputs[i].GetComponentInChildren<TextMeshProUGUI>().text = Keybinds[(KeyActions)i].ToString();
        }

        player.UpdateKeybinds(Keybinds);

        //-----------------------------------------------------------------//
        //*****************************************************************//
        //-----------------------------------------------------------------//

        // Audio
        settings.masterVolume = PlayerPrefs.GetFloat(Game.ID + "masterVolume", 0.6f);
        settings.uiVolume = PlayerPrefs.GetFloat(Game.ID + "uiVolume", 1.0f);

        // Set UI Elements
        MasterSlider.value = settings.masterVolume;
        UISlider.value = settings.uiVolume;

        //-----------------------------------------------------------------//
        //*****************************************************************//
        //-----------------------------------------------------------------//

        // Performance
        settings.resolutionScale = PlayerPrefs.GetFloat(Game.ID + "resolutionScale", 8.0f);
        settings.toggleVSync = PlayerPrefs.GetInt(Game.ID + "toggleVSync", 1) == 0 ? false : true;
        settings.renderDistance = PlayerPrefs.GetFloat(Game.ID + "renderDistance", 600.0f);

        settings.modelQuality = PlayerPrefs.GetInt(Game.ID + "modelQuality", 1);
        settings.textureQuality = PlayerPrefs.GetInt(Game.ID + "textureQuality", 2);
        settings.shadows = PlayerPrefs.GetInt(Game.ID + "shadows", 1);
        settings.shadowQuality = PlayerPrefs.GetInt(Game.ID + "shadowQuality", 1);
        settings.MSAA = PlayerPrefs.GetInt(Game.ID + "MSAA", 2);
        settings.anisotropicFiltering = PlayerPrefs.GetInt(Game.ID + "anisotropicFiltering", 1);
        settings.shadowDistance = PlayerPrefs.GetFloat(Game.ID + "shadowDistance", 400.0f);
        settings.shadowCascades = PlayerPrefs.GetInt(Game.ID + "shadowCascades", 4);

        settings.ambientOcclusion = PlayerPrefs.GetInt(Game.ID + "ambientOcclusion", 0) == 0 ? false : true;
        settings.DOF = PlayerPrefs.GetInt(Game.ID + "DOF", 0) == 0 ? false : true;

        // Set UI Elements
        ResolutionSlider.value = settings.resolutionScale;
        VSyncToggle.isOn = settings.toggleVSync;
        RenderSlider.value = settings.renderDistance;

        ChangeModelQuality(settings.modelQuality);
        ChangeTextureQuality(settings.textureQuality);
        ChangeShadowQuality(settings.shadows);
        ChangeShadowQuality2(settings.shadowQuality);
        ChangeMSAA(settings.MSAA);
        ChangeAnis(settings.anisotropicFiltering);

        ShadowSlider.value = settings.shadowDistance;

        ChangeShadowCascades(settings.shadowCascades);

        AmbientOcToggle.isOn = settings.ambientOcclusion;
        DepthOfFieldToggle.isOn = settings.DOF;

        //-----------------------------------------------------------------//
        //*****************************************************************//
        //-----------------------------------------------------------------//

        SaveSettingsButton.GetComponent<Image>().fillCenter = false;
        SaveSettingsButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

    public void SaveSettings()
    {
        // Gameplay
        PlayerPrefs.SetFloat(Game.ID + "fov", settings.fov);
        PlayerPrefs.SetFloat(Game.ID + "brightness", settings.brightness);

        PlayerPrefs.SetFloat(Game.ID + "SensitivityX", settings.SensitivityX);
        PlayerPrefs.SetFloat(Game.ID + "SensitivityY", settings.SensitivityY);
        PlayerPrefs.SetFloat(Game.ID + "ADSMultiplierX", settings.ADSMultiplierX);
        PlayerPrefs.SetFloat(Game.ID + "ADSMultiplierY", settings.ADSMultiplierY);

        PlayerPrefs.SetInt(Game.ID + "toggleADS", settings.toggleADS ? 1 : 0);
        PlayerPrefs.SetInt(Game.ID + "toggleCrouch", settings.toggleCrouch ? 1 : 0);

        // Keybind
        PlayerPrefs.SetInt(Game.ID + "Forward", (int)Keybinds[KeyActions.Forward]);
        PlayerPrefs.SetInt(Game.ID + "Back", (int)Keybinds[KeyActions.Back]);
        PlayerPrefs.SetInt(Game.ID + "Left", (int)Keybinds[KeyActions.Left]);
        PlayerPrefs.SetInt(Game.ID + "Right", (int)Keybinds[KeyActions.Right]);

        PlayerPrefs.SetInt(Game.ID + "Sprint", (int)Keybinds[KeyActions.Sprint]);
        PlayerPrefs.SetInt(Game.ID + "Jump", (int)Keybinds[KeyActions.Jump]);
        PlayerPrefs.SetInt(Game.ID + "Crouch", (int)Keybinds[KeyActions.Crouch]);

        PlayerPrefs.SetInt(Game.ID + "Reload", (int)Keybinds[KeyActions.Reload]);
        PlayerPrefs.SetInt(Game.ID + "Use", (int)Keybinds[KeyActions.Use]);

        PlayerPrefs.SetInt(Game.ID + "Grenade", (int)Keybinds[KeyActions.Grenade]);

        PlayerPrefs.SetInt(Game.ID + "ADS", (int)Keybinds[KeyActions.ADS]);
        PlayerPrefs.SetInt(Game.ID + "Fire", (int)Keybinds[KeyActions.Fire]);

        PlayerPrefs.SetInt(Game.ID + "SwapDown", (int)Keybinds[KeyActions.SwapDown]);
        PlayerPrefs.SetInt(Game.ID + "SwapUp", (int)Keybinds[KeyActions.SwapUp]);

        PlayerPrefs.SetInt(Game.ID + "Menu", (int)Keybinds[KeyActions.Menu]);
        PlayerPrefs.SetInt(Game.ID + "Scoreboard", (int)Keybinds[KeyActions.Scoreboard]);

        // Audio
        PlayerPrefs.SetFloat(Game.ID + "masterVolume", settings.masterVolume);
        PlayerPrefs.SetFloat(Game.ID + "uiVolume", settings.uiVolume);

        // Performance
        PlayerPrefs.SetFloat(Game.ID + "resolutionScale", settings.resolutionScale);
        PlayerPrefs.SetInt(Game.ID + "VSync", settings.toggleVSync ? 1 : 0);
        PlayerPrefs.SetFloat(Game.ID + "renderDistance", settings.renderDistance);

        PlayerPrefs.SetInt(Game.ID + "modelQuality", settings.modelQuality);
        PlayerPrefs.SetInt(Game.ID + "textureQuality", settings.textureQuality);
        PlayerPrefs.SetInt(Game.ID + "shadows", settings.shadows);
        PlayerPrefs.SetInt(Game.ID + "shadowQuality", settings.shadowQuality);
        PlayerPrefs.SetInt(Game.ID + "MSAA", settings.MSAA);
        PlayerPrefs.SetInt(Game.ID + "anisotropicFiltering", settings.anisotropicFiltering);
        PlayerPrefs.SetFloat(Game.ID + "shadowDistance", settings.shadowDistance);
        PlayerPrefs.SetInt(Game.ID + "shadowCascades", settings.shadowCascades);

        PlayerPrefs.SetInt(Game.ID + "ambientOcclusion", settings.ambientOcclusion ? 1 : 0);
        PlayerPrefs.SetInt(Game.ID + "DOF", settings.DOF ? 1 : 0);

        SaveSettingsButton.GetComponent<Image>().fillCenter = false;
        SaveSettingsButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        Game.PlayHigh(settings.uiVolume);
    }

    void SettingsChanged()
    {
        SaveSettingsButton.GetComponent<Image>().fillCenter = true;
        SaveSettingsButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }

    IEnumerator DoSomething(GameObject OBJ)
    {
        yield return new WaitForSeconds(0.05f);

        Vector3 v = OBJ.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;

        if(Mathf.Abs(v.y) > 0.1f)
        {
            //Debug.Log(OBJ.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position);
            OBJ.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);
        }
    }

    public void LeaveRoom()
    {
        //string msg = player.playerName + " Left The Game";
        //player.modeCtrl.photonView.RPC("SendToFeed", RpcTarget.All, msg);

        Game.LeftMatch = true;

        Debug.Log("LeaveRoom");
        //PhotonNetwork.DestroyPlayerObjects()
        //PhotonNetwork.Destroy(player.gameObject);
        Debug.Log("Destroy");
        PhotonNetwork.LeaveRoom();
        Debug.Log("Now");
        //PhotonNetwork.LoadLevel(0);
        Debug.Log("ssss");

        //PhotonNetwork.SendAllOutgoingCommands

        Game.PlayHigh(settings.uiVolume);
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if(Game.LeftMatch)
            PhotonNetwork.LoadLevel(0);
        Debug.Log("Leaving Match");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        //PhotonNetwork.LoadLevel(0);
        Debug.Log("Leaving Game");
    }

    #region GameplayTab

    public void GameplayTab()
    {
        GameplayTabPanel.SetActive(true);
        /*Vector3 v = GameplayTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;

        Debug.Log(GameplayTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position);
        GameplayTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);*/

        StartCoroutine(DoSomething(GameplayTabPanel));

        AudioTabPanel.SetActive(false);
        KeybindTabPanel.SetActive(false);
        PerformanceTabPanel.SetActive(false);

        GameplayTabButton.GetComponent<Image>().fillCenter = true;
        GameplayTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        AudioTabButton.GetComponent<Image>().fillCenter = false;
        AudioTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        KeybindTabButton.GetComponent<Image>().fillCenter = false;
        KeybindTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        PerformanceTabButton.GetComponent<Image>().fillCenter = false;
        PerformanceTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        Game.PlayHigh(settings.uiVolume);
    }

    public void UpdateFOV(Slider inFloat)
    {
        if(settings.fov != inFloat.value || !Started)
        {
            if (player)
                player.FOV = inFloat.value;
            FovDisplay.text = inFloat.value.ToString("F0");

            settings.fov = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateBrightness(Slider inFloat)
    {
        if (settings.brightness != inFloat.value || !Started)
        {
            float brightness = inFloat.value;
            //Screen.brightness = brightness / 100.0f;
            BrightnessDisplay.text = inFloat.value.ToString("F2");

            RenderSettings.ambientLight = new Color(brightness / 100.0f, brightness / 100.0f, brightness / 100.0f, 1.0f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            //RenderSettings.
            settings.brightness = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateSensitivityX(Slider inFloat)
    {
        if (settings.SensitivityX != inFloat.value || !Started)
        {
            if (player)
                player.SensitivityX = inFloat.value;
            SensXDisplay.text = inFloat.value.ToString("F2");

            settings.SensitivityX = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateSensitivityY(Slider inFloat)
    {
        if (settings.SensitivityY != inFloat.value || !Started)
        {
            if (player)
                player.SensitivityY = inFloat.value;
            SensYDisplay.text = inFloat.value.ToString("F2");

            settings.SensitivityY = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateADSSensitivityX(Slider inFloat)
    {
        if (settings.ADSMultiplierX != inFloat.value || !Started)
        {
            if (player)
                player.ADSMultiplierX = inFloat.value;
            MultiXDisplay.text = inFloat.value.ToString("F2");

            settings.ADSMultiplierX = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateADSSensitivityY(Slider inFloat)
    {
        if (settings.ADSMultiplierY != inFloat.value || !Started)
        {
            if (player)
                player.ADSMultiplierY = inFloat.value;
            MultiYDisplay.text = inFloat.value.ToString("F2");

            settings.ADSMultiplierY = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateADSTgl(Toggle inBool)
    {
        if (player)
        { 
            player.toggleADS = inBool.isOn;
        }

        settings.toggleADS = inBool.isOn;
        SettingsChanged();
    }

    public void UpdateCrouchTgl(Toggle inBool)
    {
        if (player)
        {
            player.toggleCrouch = inBool.isOn;
        }

        settings.toggleCrouch = inBool.isOn;
        SettingsChanged();
    }

    #endregion

    #region AudioTab

    public void AudioTab()
    {
        AudioTabPanel.SetActive(true);

        /*Vector3 v = AudioTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        AudioTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);*/

        StartCoroutine(DoSomething(AudioTabPanel));

        KeybindTabPanel.SetActive(false);
        PerformanceTabPanel.SetActive(false);
        GameplayTabPanel.SetActive(false);

        AudioTabButton.GetComponent<Image>().fillCenter = true;
        AudioTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        GameplayTabButton.GetComponent<Image>().fillCenter = false;
        GameplayTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        KeybindTabButton.GetComponent<Image>().fillCenter = false;
        KeybindTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        PerformanceTabButton.GetComponent<Image>().fillCenter = false;
        PerformanceTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        Game.PlayHigh(settings.uiVolume);
    }

    public void UpdateMasterVolume(Slider inFloat)
    {
        if (player)
            player.MasterVolume = inFloat.value;
        MasterDisplay.text = inFloat.value.ToString("F3");

        settings.masterVolume = inFloat.value;
        SettingsChanged();
    }

    public void UpdateUIVolume(Slider inFloat)
    {
        if (player)
            player.UIVolume = inFloat.value;
        UIDisplay.text = inFloat.value.ToString("F3");

        settings.uiVolume = inFloat.value;
        SettingsChanged();
    }

    #endregion

    #region KeybindTab

    public void KeybindTab()
    {
        KeybindTabPanel.SetActive(true);

        /*Vector3 v = KeybindTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        KeybindTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);*/

        StartCoroutine(DoSomething(KeybindTabPanel));

        PerformanceTabPanel.SetActive(false);
        GameplayTabPanel.SetActive(false);
        AudioTabPanel.SetActive(false);

        KeybindTabButton.GetComponent<Image>().fillCenter = true;
        KeybindTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        GameplayTabButton.GetComponent<Image>().fillCenter = false;
        GameplayTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        AudioTabButton.GetComponent<Image>().fillCenter = false;
        AudioTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        PerformanceTabButton.GetComponent<Image>().fillCenter = false;
        PerformanceTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        Game.PlayHigh(settings.uiVolume);
    }

    public void Rebind(int action)
    {
        if(rebindActive)
        {
            rebindActive = false;

            KeyInputs[(int)currRebind].GetComponent<Image>().fillCenter = true;
            KeyInputs[(int)currRebind].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

            gameObject.GetComponent<Image>().raycastTarget = true;
        }

        rebindActive = true;
        currRebind = (KeyActions)action;

        KeyInputs[action].GetComponent<Image>().fillCenter = false;
        KeyInputs[action].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        gameObject.GetComponent<Image>().raycastTarget = false;
    }

    #endregion

    #region PerformanceTab

    public void PerformanceTab()
    {
        PerformanceTabPanel.SetActive(true);

        /*Vector3 v = PerformanceTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        PerformanceTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);*/

        StartCoroutine(DoSomething(PerformanceTabPanel));

        GameplayTabPanel.SetActive(false);
        AudioTabPanel.SetActive(false);
        KeybindTabPanel.SetActive(false);

        PerformanceTabButton.GetComponent<Image>().fillCenter = true;
        PerformanceTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        GameplayTabButton.GetComponent<Image>().fillCenter = false;
        GameplayTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        AudioTabButton.GetComponent<Image>().fillCenter = false;
        AudioTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        KeybindTabButton.GetComponent<Image>().fillCenter = false;
        KeybindTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        Game.PlayHigh(settings.uiVolume);
    }

    float mapToRange(float val, float r1s, float r1e, float r2s, float r2e)
    {
        return (val - r1s) / (r1e - r1s) * (r2e - r2s) + r2s;
    }

    public void UpdateResolutionQuality(Slider inFloat)
    {
        float res = Mathf.Clamp(inFloat.value, 0.0f, 8.0f);
        float x = Mathf.Clamp(inFloat.value - 8.0f, 0.0f, 2.0f);

        float resolution = mapToRange(res, 0.0f, 8.0f, 0.0f, 1.0f) + mapToRange(x, 0.0f, 2.0f, 0.0f, 0.5f);

        //int width = Mathf.RoundToInt(Screen.currentResolution.width * resolution);
        // int height = Mathf.RoundToInt(Screen.currentResolution.height * resolution);

        //Screen.SetResolution(width, height, true);

        if (renderCtrl)
            renderCtrl.UpdateResolution(resolution);
        ResolutionDisplay.text = inFloat.value.ToString("F2");

        //Debug.Log(QualitySettings.resolutionScalingFixedDPIFactor);

        //QualitySettings.resolutionScalingFixedDPIFactor = resolution;

        //Debug.Log(QualitySettings.resolutionScalingFixedDPIFactor);

        settings.resolutionScale = inFloat.value;
        SettingsChanged();
    }

    public void UpdateVSync(Toggle inBool)
    {
        if(inBool.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        settings.toggleVSync = inBool.isOn;
        SettingsChanged();
    }

    public void UpdateRenderDistance(Slider inFloat)
    {
        //player.FPSCamera.farClipPlane = inFloat.value;
        if (player)
            player.EnvCamera.farClipPlane = inFloat.value;
        RenderDisplay.text = inFloat.value.ToString("F2");

        settings.renderDistance = inFloat.value;
        SettingsChanged();
    }

    public void ChangeModelQuality(int selection)
    {
        if(settings.modelQuality != selection || !Started)
        {
            Image[] img = ModelQuality.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = ModelQuality.GetComponentsInChildren<TextMeshProUGUI>();

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;
            txt[3].color = Color.white;

            img[selection].fillCenter = true;
            txt[selection + 1].color = Color.black;

            switch (selection)
            {
                case 0:
                    QualitySettings.lodBias = 0.3f;
                    break;
                case 1:
                    QualitySettings.lodBias = 1.0f;
                    break;
                case 2:
                    QualitySettings.lodBias = 2.0f;
                    break;
            }

            settings.modelQuality = selection;
            SettingsChanged();
        }
    }

    public void ChangeTextureQuality(int selection)
    {
        if (settings.textureQuality != selection || !Started)
        {
            Image[] img = TextureQuality.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = TextureQuality.GetComponentsInChildren<TextMeshProUGUI>();

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;//////////////////////////////////
            txt[3].color = Color.white;

            img[2 - selection].fillCenter = true;
            txt[2 - selection + 1].color = Color.black;

            QualitySettings.masterTextureLimit = selection;

            settings.textureQuality = selection;
            SettingsChanged();
        }
    }

    public void ChangeShadowQuality(int selection)
    {
        if (settings.shadows != selection || !Started)
        {
            Image[] img = Shadows.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = Shadows.GetComponentsInChildren<TextMeshProUGUI>();

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;
            txt[3].color = Color.white;

            img[selection].fillCenter = true;
            txt[selection + 1].color = Color.black;

            switch (selection)
            {
                case 0:
                    QualitySettings.shadows = ShadowQuality.Disable;
                    break;
                case 1:
                    QualitySettings.shadows = ShadowQuality.HardOnly;
                    break;
                case 2:
                    QualitySettings.shadows = ShadowQuality.All;
                    break;
            }

            settings.shadows = selection;
            SettingsChanged();
        }
    }

    public void ChangeShadowQuality2(int selection)
    {
        if (settings.shadowQuality != selection || !Started)
        {
            Image[] img = ShadowQual.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = ShadowQual.GetComponentsInChildren<TextMeshProUGUI>();

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;
            txt[3].color = Color.white;

            img[selection].fillCenter = true;
            txt[selection + 1].color = Color.black;

            switch (selection)
            {
                case 0:
                    QualitySettings.shadowResolution = ShadowResolution.Low;
                    break;
                case 1:
                    QualitySettings.shadowResolution = ShadowResolution.Medium;
                    break;
                case 2:
                    QualitySettings.shadowResolution = ShadowResolution.High;
                    break;
            }

            settings.shadowQuality = selection;
            SettingsChanged();
        }
    }

    public void ChangeMSAA(int selection)
    {
        if (settings.MSAA != selection || !Started)
        {
            Image[] img = MSAAQuality.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = MSAAQuality.GetComponentsInChildren<TextMeshProUGUI>();

            int s = Mathf.RoundToInt(Mathf.Sqrt(selection));

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;
            img[3].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;
            txt[3].color = Color.white;
            txt[4].color = Color.white;

            img[s].fillCenter = true;
            txt[s + 1].color = Color.black;

            QualitySettings.antiAliasing = selection;

            settings.MSAA = selection;
            SettingsChanged();
        }
    }

    public void ChangeAnis(int selection)
    {
        if (settings.anisotropicFiltering != selection || !Started)
        {
            Image[] img = ANISQuality.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = ANISQuality.GetComponentsInChildren<TextMeshProUGUI>();

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;
            txt[3].color = Color.white;

            img[selection].fillCenter = true;
            txt[selection + 1].color = Color.black;

            switch (selection)
            {
                case 0:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    break;
                case 1:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    break;
                case 2:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                    break;
            }

            settings.anisotropicFiltering = selection;
            SettingsChanged();
        }
    }

    public void UpdateShadowDistance(Slider inFloat)
    {
        QualitySettings.shadowDistance = inFloat.value;
        ShadowDisplay.text = inFloat.value.ToString("F2");

        settings.shadowDistance = inFloat.value;
        SettingsChanged();
    }

    public void ChangeShadowCascades(int selection)
    {
        if (selection != settings.shadowCascades || !Started)
        {
            Image[] img = ShadowCascades.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = ShadowCascades.GetComponentsInChildren<TextMeshProUGUI>();

            int s = selection / 2;

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;
            txt[3].color = Color.white;

            img[s].fillCenter = true;
            txt[s + 1].color = Color.black;

            QualitySettings.shadowCascades = selection;

            settings.shadowCascades = selection;
            SettingsChanged();
        }
    }

    public void UpdateAmbientOc(Toggle inBool)
    {
        if (inBool.isOn != settings.ambientOcclusion || !Started)
        {
            try
            {
                MonoBehaviour tempScript = (MonoBehaviour)player.EnvCamera.GetComponent("AmbientOcclusion");
                if (inBool.isOn)
                {
                    tempScript.enabled = true;
                }
                else
                {
                    tempScript.enabled = false;
                }

                settings.ambientOcclusion = inBool.isOn;
                SettingsChanged();
            }
            catch
            {
                Debug.Log("No AO post processing found");
                return;
            }
        }
    }

    public void UpdateDOF(Toggle inBool)
    {
        if(inBool.isOn != settings.DOF || !Started)
        {
            try
            {
                MonoBehaviour tempScript = (MonoBehaviour)player.EnvCamera.GetComponent("DepthOfField");
                if (inBool.isOn)
                {
                    tempScript.enabled = true;
                }
                else
                {
                    tempScript.enabled = false;
                }

                settings.DOF = inBool.isOn;
                SettingsChanged();
            }
            catch
            {
                Debug.Log("No DOF post processing found");
                return;
            }
        }
    }

    #endregion
}
