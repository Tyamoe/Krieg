using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public PhotonView photonView;

    [Header("PvP")]
    public int Health = 100;

    [Header("Movement")]
    public float PlayerSpeed = 20.0f;
    public float PlayerSprintSpeed = 4.0f;
    public float PlayerJumpHeight = 2.8f;
    public float PlayerClimbSpeed = 0.9f;

    public Vector3 PlayerGravity = new Vector3(0.0f, -9.8f, 0.0f);

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Controller References")]
    public CameraController cam;
    public WeaponManager weapons;
    public PlayerSettingsFunctions settingsFuncs;

    //[HideInInspector]
    public ModeController modeCtrl;

    [Header("Object References")]
    public Camera FPSCamera;
    public Camera EnvCamera;

    public Animator playerAnim;

    public GameObject PlayerUI;

    public GameObject[] Meshes;

    public Dictionary<WeaponClass, float> WeaponClassModifiers = new Dictionary<WeaponClass, float>()
    {
        {WeaponClass.Knife, 1.15f },
        {WeaponClass.Pistol, 1.0f },
        {WeaponClass.SMG, 0.98f },
        {WeaponClass.Shotgun, 0.95f },
        {WeaponClass.Rifle, 0.94f },
        {WeaponClass.Sniper, 0.92f },
    };

    /***************************************/
    /*************** Prop Var **************/
    /***************************************/

    private bool toggleCrouch_;
    public bool toggleCrouch
    {
        get { return toggleCrouch_; }
        set
        {
            if (toggleCrouch_ == value) return;

            toggleCrouch_ = value;
            if (!Started)
                settingsFuncs.CrouchToggle.isOn = value;
        }
    }

    private bool toggleADS_;
    public bool toggleADS
    {
        get { return toggleADS_; }
        set
        {
            if (toggleADS_ == value) return;

            toggleADS_ = value;
            if (!Started)
                settingsFuncs.ADSToggle.isOn = value;
            weapons.UpdateCustomSettings(value);
        }
    }

    private float SensitivityX_;
    public float SensitivityX
    {
        get { return SensitivityX_; }
        set
        {
            if (SensitivityX_ == value) return;

            SensitivityX_ = value;
            cam.SensitivityX = value;
            if (!Started)
                settingsFuncs.SensXSlider.value = value;
        }
    }

    private float SensitivityY_;
    public float SensitivityY
    {
        get { return SensitivityY_; }
        set
        {
            if (SensitivityY_ == value) return;

            SensitivityY_ = value;
            cam.SensitivityY = value;
            if (!Started)
                settingsFuncs.SensYSlider.value = value;
        }
    }

    private float ADSMultiplierX_;
    public float ADSMultiplierX
    {
        get { return ADSMultiplierX_; }
        set
        {
            if (ADSMultiplierX_ == value) return;

            ADSMultiplierX_ = value;
            cam.ADSMultiplierX = value;
            if (!Started)
                settingsFuncs.MultiXSlider.value = value;
        }
    }

    private float ADSMultiplierY_;
    public float ADSMultiplierY
    {
        get { return ADSMultiplierY_; }
        set
        {
            if (ADSMultiplierY_ == value) return;

            ADSMultiplierY_ = value;
            cam.ADSMultiplierY = value;
            if(!Started)
                settingsFuncs.MultiYSlider.value = value;
        }
    }

    private float FOV_;
    public float FOV
    {
        get { return FOV_; }
        set
        {
            if (FOV_ == value) return;

            FOV_ = value;
            //FPSCamera.fieldOfView = value;
            EnvCamera.fieldOfView = value;
        }
    }

    private float masterVolume;
    public float MasterVolume
    {
        get { return masterVolume; }
        set
        {
            if (masterVolume == value) return;

            masterVolume = value;
            AudioListener.volume = value / 10.0f;
            if (!Started)
                settingsFuncs.MasterSlider.value = value;
        }
    }

    private float uiVolume;
    public float UIVolume
    {
        get { return uiVolume / 10.0f; }
        set
        {
            if (uiVolume == value) return;

            uiVolume = value;
            if (!Started)
                settingsFuncs.UISlider.value = value;
        }
    }

    /***************************************/
    /**************** PRIVATE **************/
    /***************************************/

    private bool Started = false;

    [HideInInspector]
    public bool PlayerControl = true;
    [HideInInspector]
    public bool playerADS = false;
    [HideInInspector]
    public bool playerReload = false;

    // Ref
    private CharacterController player;

    private GameObject InGameUI;
    private GameObject SettingsUI;

    [HideInInspector]
    public GameObject Crosshair;
    [HideInInspector]
    public GameObject Hitmarker;

    [HideInInspector]
    public TMPro.TextMeshProUGUI AmmoText;
    [HideInInspector]
    public TMPro.TextMeshProUGUI DebugText;

    [HideInInspector]
    public HandIK handIK;

    // Status
    private Vector3 lastPos;
    private int currWeapon = 0;

    // Movement
    private bool moving = true;

    private bool InputSprint = false;
    [SerializeField]
    private float InputX;
    [SerializeField]
    private float InputZ;

    private Vector3 PlayerVelocity;

    private float WeaponMultiplier = 1.0f;

    private float calcInX1 = 0.33f;
    private float calcInX2 = 0.33f;
    private float calcInZ1 = 0.33f;
    private float calcInZ2 = 0.33f;

    // Stance
    [HideInInspector]
    public bool Crouching = false;

    [HideInInspector]
    public bool Jumping = false;

    [HideInInspector]
    public bool Climbing = false;
    [HideInInspector]
    public Ladder currLadder = null;

    // UI
    private bool SettingsOpen = false;

    // Key binds
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

    void Awake()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            FPSCamera.enabled = false;
            EnvCamera.enabled = false;

            FPSCamera.GetComponent<AudioListener>().enabled = false;

            foreach (GameObject obj in Meshes)
            {
                obj.layer = LayerMask.NameToLayer("Default");
            }

            PlayerUI.SetActive(false);

            return;
        }
        else
        {
            FPSCamera.enabled = true;
            EnvCamera.enabled = true;

            FPSCamera.GetComponent<AudioListener>().enabled = true;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        player = GetComponent<CharacterController>();
        handIK = GetComponent<HandIK>();

        InGameUI = PlayerUI.transform.Find("InGame").gameObject;
        SettingsUI = PlayerUI.transform.Find("Settings").gameObject;

        Hitmarker = InGameUI.transform.Find("Hitmarker").gameObject;
        Crosshair = InGameUI.transform.Find("Crosshair").gameObject;
        AmmoText = InGameUI.transform.Find("Ammo").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        DebugText = InGameUI.transform.Find("Debug").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Start()
    {
        if(!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        PlayerVelocity = Vector3.zero;
        SettingsUI.SetActive(false);

        foreach (WeaponController weapon in weapons.weapons)
        {
            weapon.gameObject.SetActive(false);
        }
        weapons.weapons[0].gameObject.SetActive(true);
        weapons.weapons[0].WeaponSwap(true);

        handIK.leftHandObj = weapons.weapons[0].Grip;
        handIK.rightHandObj = weapons.weapons[0].Trigger;

        WeaponMultiplier = WeaponClassModifiers[weapons.weapons[0].weaponClass];

        if (PhotonNetwork.IsConnected)
            GetComponent<PhotonView>().RPC("WeaponSwap", RpcTarget.OthersBuffered, 0);

        Started = true;

        DebugText.text = "Send Rate: " + PhotonNetwork.SendRate + " Serialize Rate: " + PhotonNetwork.SerializationRate;
    }

    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        if (Input.GetKeyDown(Keybinds[KeyActions.Menu]))
        {
            SettingsOpen = !SettingsOpen;
            SettingsUI.SetActive(SettingsOpen);

            if(SettingsOpen)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            PlayerControl = !SettingsOpen;
        }

        // Gravity
        PlayerVelocity += (PlayerGravity * WeaponMultiplier) * Time.deltaTime;

        // In Menu
        if (!PlayerControl)
        {
            playerAnim.SetBool("Moving", false);

            Vector3 move1 = PlayerVelocity;
            player.Move(move1 * PlayerSpeed * Time.deltaTime);
            return;
        }
        
        // Landing
        if (player.isGrounded && PlayerVelocity.y < 0.0f || Climbing)
        {
            PlayerVelocity = Vector3.zero;
            Jumping = false;
        }

        // Hit head
        if(Jumping && player.collisionFlags == CollisionFlags.Above)
        {
            PlayerVelocity += PlayerGravity * Time.deltaTime;
        }

        // Jumping
        if (Input.GetKeyDown(Keybinds[KeyActions.Jump]) && isGrounded() && !Climbing)
        {
            PlayerVelocity += transform.up * PlayerJumpHeight * WeaponMultiplier;
            Jumping = true;
        }

        // Sprint & Speed Mods
        InputSprint = Input.GetKey(Keybinds[KeyActions.Sprint]);

        float StateMultiplier = 1.0f;

        if (InputSprint)
        {
            StateMultiplier = PlayerSprintSpeed;
        }

        if(Crouching)
        {
            StateMultiplier = 0.75f;
        }

        if (Input.GetKey(Keybinds[KeyActions.Left]))
        {
            if (calcInX2 > 0.33f)
            {
                calcInX2 = 0.33f;
                calcInX1 = 0.33f;
            }
            else
            {
                if (calcInX1 < 1.0f)
                {
                    calcInX1 += Time.deltaTime * 1.0f;

                    InputX = -(1.0f + Mathf.Log(calcInX1)) * StateMultiplier * WeaponMultiplier;
                }
                else
                {
                    InputX = -1.0f * StateMultiplier * WeaponMultiplier;
                }
            }
        }
        else if (Input.GetKey(Keybinds[KeyActions.Right]))
        {
            if(calcInX1 > 0.33f)
            {
                calcInX1 = 0.33f;
                calcInX2 = 0.33f;
            }
            else
            {
                if (calcInX2 < 1.0f)
                {
                    calcInX2 += Time.deltaTime * 1.0f;

                    InputX = (1.0f + Mathf.Log(calcInX2)) * StateMultiplier * WeaponMultiplier;
                }
                else
                {
                    InputX = 1.0f * StateMultiplier * WeaponMultiplier;
                }
            }
        }
        else
        {
            InputX = 0.0f;
            calcInX1 = 0.33f;
            calcInX2 = 0.33f;
        }
        if (Input.GetKey(Keybinds[KeyActions.Forward]))
        {
            if (calcInZ2 > 0.33f)
            {
                calcInZ2 = 0.33f;
                calcInZ1 = 0.33f;
            }
            else
            {
                if (calcInZ1 < 1.0f)
                {
                    calcInZ1 += Time.deltaTime * 1.0f;

                    InputZ = (1.0f + Mathf.Log(calcInZ1)) * StateMultiplier * WeaponMultiplier;
                }
                else
                {
                    InputZ = 1.0f * StateMultiplier * WeaponMultiplier;
                }
            }
        }
        else if (Input.GetKey(Keybinds[KeyActions.Back]))
        {
            if (calcInZ1 > 0.33f)
            {
                calcInZ1 = 0.33f;
                calcInZ2 = 0.33f;
            }
            else
            {
                if (calcInZ2 < 1.0f)
                {
                    calcInZ2 += Time.deltaTime * 1.0f;

                    InputZ = -(1.0f + Mathf.Log(calcInZ2)) * StateMultiplier * WeaponMultiplier;
                }
                else
                {
                    InputZ = -1.0f * StateMultiplier * WeaponMultiplier;
                }
            }
        }
        else
        {
            InputZ = 0.0f;
            calcInZ1 = 0.33f;
            calcInZ2 = 0.33f;
        }

        // Moving
        if (Mathf.Abs(InputX) > 0.0f || Mathf.Abs(InputZ) > 0.0f)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
        playerAnim.SetBool("Moving", moving);

        // Climbing
        if(Climbing)
        {
            float tempInZ = InputZ;
            InputZ = 0.0f;

            if (tempInZ > 0.0f)
            {
                PlayerVelocity += transform.up * PlayerClimbSpeed * WeaponMultiplier;
            }
            else if (tempInZ < 0.0f)
            {
                PlayerVelocity += transform.up * -PlayerClimbSpeed * WeaponMultiplier;
            }
        }

        // Current Frame Movement
        Vector3 move = (transform.right * InputX) + (transform.forward * InputZ) + PlayerVelocity;

        player.Move(move * PlayerSpeed * Time.deltaTime);

        // Crouch
        if (Input.GetKeyDown(Keybinds[KeyActions.Crouch]))
        {
            if (toggleCrouch)
            {
                Crouching = !Crouching;
            }
            else
            {
                Crouching = true;
            }

            Crouch(Crouching);
        }

        if (Input.GetKeyUp(Keybinds[KeyActions.Crouch]))
        {
            if (!toggleCrouch)
            {
                Crouching = false;

                Crouch(Crouching);
            }
        }

        // Weapon Change
        if (Input.GetKeyDown(KeyCode.Alpha1) && !playerReload)
        {
            weapons.weapons[currWeapon].WeaponSwap(false);
            weapons.weapons[currWeapon].gameObject.SetActive(false);
            weapons.weapons[0].WeaponSwap(true);
            weapons.weapons[0].gameObject.SetActive(true);

            handIK.leftHandObj = weapons.weapons[0].Grip;
            handIK.rightHandObj = weapons.weapons[0].Trigger;

            currWeapon = 0;

            WeaponMultiplier = WeaponClassModifiers[weapons.weapons[0].weaponClass];

            if (PhotonNetwork.IsConnected)
                GetComponent<PhotonView>().RPC("WeaponSwap", RpcTarget.OthersBuffered, 0);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !playerReload)
        {
            weapons.weapons[currWeapon].WeaponSwap(false);
            weapons.weapons[currWeapon].gameObject.SetActive(false);
            weapons.weapons[1].WeaponSwap(true);
            weapons.weapons[1].gameObject.SetActive(true);

            handIK.leftHandObj = weapons.weapons[1].Grip;
            handIK.rightHandObj = weapons.weapons[1].Trigger;

            currWeapon = 1;

            WeaponMultiplier = WeaponClassModifiers[weapons.weapons[1].weaponClass];

            if (PhotonNetwork.IsConnected)
                GetComponent<PhotonView>().RPC("WeaponSwap", RpcTarget.OthersBuffered, 1);
        }

        if (Input.GetKeyDown(Keybinds[KeyActions.SwapDown]))
        {
            int prev = currWeapon;
            currWeapon--;

            currWeapon = currWeapon < 0 ? weapons.weapons.Count - 1 : currWeapon;

            weapons.weapons[prev].WeaponSwap(false);
            weapons.weapons[prev].gameObject.SetActive(false);
            weapons.weapons[currWeapon].WeaponSwap(true);
            weapons.weapons[currWeapon].gameObject.SetActive(true);

            handIK.leftHandObj = weapons.weapons[currWeapon].Grip;
            handIK.rightHandObj = weapons.weapons[currWeapon].Trigger;

            WeaponMultiplier = WeaponClassModifiers[weapons.weapons[currWeapon].weaponClass];

            if (PhotonNetwork.IsConnected)
                GetComponent<PhotonView>().RPC("WeaponSwap", RpcTarget.OthersBuffered, currWeapon);
        }
        else if (Input.GetKeyDown(Keybinds[KeyActions.SwapUp]))
        {
            int prev = currWeapon;
            currWeapon++;

            currWeapon = currWeapon >= weapons.weapons.Count ? 0 : currWeapon;

            weapons.weapons[prev].WeaponSwap(false);
            weapons.weapons[prev].gameObject.SetActive(false);
            weapons.weapons[currWeapon].WeaponSwap(true);
            weapons.weapons[currWeapon].gameObject.SetActive(true);

            handIK.leftHandObj = weapons.weapons[currWeapon].Grip;
            handIK.rightHandObj = weapons.weapons[currWeapon].Trigger;

            WeaponMultiplier = WeaponClassModifiers[weapons.weapons[currWeapon].weaponClass];

            if (PhotonNetwork.IsConnected)
                GetComponent<PhotonView>().RPC("WeaponSwap", RpcTarget.OthersBuffered, currWeapon);
        }

        // Dev Timer
        if (Input.GetKeyDown(KeyCode.Mouse4))
        {
            if(tempTimer)
                Debug.Log("Time: " + tempTime.ToString("F2"));
            tempTime = 0.0f;
            tempTimer = !tempTimer;
        }

        // Camera Angles TODO
        /*if (Input.GetKeyDown(KeyCode.F4))
        {
            cam.SetCamAngle(0);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            cam.SetCamAngle(1);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            cam.SetCamAngle(2);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            cam.SetCamAngle(3);
        }*/

        lastPos = transform.position;

        if(tempFrame >= 5.0f)
        {
            tempFrame = 0.0f;
            DebugText.text = "Health: " + Health + " Ping: " + PhotonNetwork.GetPing() + " FPS: " + (1.0f / Time.deltaTime);
        }

        if(tempTimer)
        {
            tempTime += Time.deltaTime;
            DebugText.text = "Timer: " + tempTime.ToString("F2") + " Ping: " + PhotonNetwork.GetPing() + " FPS: " + (1.0f / Time.deltaTime);
        }
        tempFrame += Time.deltaTime;
    }

    bool tempTimer = false;
    float tempTime = 0.0f;
    float tempFrame = 0.0f;

    bool isGrounded()
    {
        /*Vector3 feet = transform.Find("Cube").position;//transform.position - transform.up * 4.0f;
        Vector3 target = feet - transform.up * 0.4f;

        RaycastHit hit;
        if (Physics.Linecast(feet, target, out hit))
        {
            Debug.Log(hit.transform.name);
            return true;
        }

        return false;*/
        return player.isGrounded || PlayerVelocity.y > -0.050f;
    }

    void Crouch(bool crouched)
    {
        if (crouched)
        {
            Crouching = true;
            playerAnim.SetBool("Crouching", true);
            player.height = 4.9f;
            player.center = new Vector3(player.center.x, 2.9f, player.center.z);

            if(!playerReload)
            {
                handIK.LeftIK = 0.5f;
            }
        }
        else
        {
            Crouching = false;
            playerAnim.SetBool("Crouching", false);

            player.height = 8.0f;
            player.center = new Vector3(player.center.x, 4.0f, player.center.z);

            if (!playerReload)
            {
                handIK.LeftIK = 0.45f;
            }
        }
    }

    public int ChangeHealth(int damage)
    {
        Health -= damage;

        if(Health <= 0)
        {
            Health = 100;

            if(modeCtrl)
            {
                Transform t = modeCtrl.GetRespawn();
                transform.position = t.position;
                transform.rotation = t.rotation;

                if (PhotonNetwork.IsConnected)
                    GetComponent<PhotonView>().RPC("Respawn", RpcTarget.OthersBuffered, t);

            }
            else
            {
                transform.position = new Vector3(0.0f, 40.0f, 0.0f);
            }
            return 0;
        }

        return Health;
    }

    [PunRPC]
    public int ChangeHealthRPC(int damage)
    {
        Debug.Log("Damaged: " + damage + " | Health: " + (Health - damage));
        Health -= damage;

        if (Health <= 0)
        {
            Health = 100;

            if (modeCtrl)
            {
                Transform t = modeCtrl.GetRespawn();
                transform.position = t.position;
                transform.rotation = t.rotation;

                if (PhotonNetwork.IsConnected)
                    GetComponent<PhotonView>().RPC("Respawn", RpcTarget.OthersBuffered, t);

            }
            else
            {
                transform.position = new Vector3(0.0f, 40.0f, 0.0f);
            }
            return 0;
        }

        if(photonView.IsMine)
        {
            DebugText.text = "Health: " + Health + " Ping: " + PhotonNetwork.GetPing() + " FPS: " + (1.0f / Time.deltaTime);
        }

        return Health;
    }

    [PunRPC]
    private void Respawn(Transform t)
    {
        transform.position = t.position;
        transform.rotation = t.rotation;
        /*if (gameCtrl)
        {
            transform.position = gameCtrl.spawnPoints[index].position;
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.position = new Vector3(0.0f, 40.0f, 0.0f);
            transform.rotation = Quaternion.identity;
        }*/
    }

    [PunRPC]
    private void WeaponSwap(int currWeapon)
    {
        foreach(WeaponController weapon in weapons.weapons)
        {
            weapon.gameObject.SetActive(false);
        }
        weapons.weapons[currWeapon].gameObject.SetActive(true);
    }

    public void UpdateKeybinds(Dictionary<KeyActions, KeyCode> keybinds)
    {
        Keybinds = keybinds;

        foreach(WeaponController weapon in weapons.weapons)
        {
            weapon.ReloadKey = keybinds[KeyActions.Reload];
            weapon.ADSKey = keybinds[KeyActions.ADS];
            weapon.FireKey = keybinds[KeyActions.Fire];
        }
    }
}
