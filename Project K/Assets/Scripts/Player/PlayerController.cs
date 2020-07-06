using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public PhotonView photonView;

    public WeaponManager weapons;
    public CameraController cam;
    public PlayerSettingsFunctions settingsFuncs;

    public Animator playerAnim;

    public GameObject[] Meshes;

    public Camera FPSCamera;
    public Camera EnvCamera;

    public GameObject PlayerUI;

    [Header("Variables/PvP")]
    public int Health = 100;

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

    //[Header("Custom Settings")]
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
            FPSCamera.fieldOfView = value;
            EnvCamera.fieldOfView = value;
        }
    }

    [Header("Variables/Movement")]
    public float PlayerSpeed = 20.0f;
    public float PlayerSprintSpeed = 4.0f;
    public float PlayerJumpHeight = 2.8f;
    public float PlayerClimbSpeed = 0.9f;
    public Vector3 PlayerGravity = new Vector3(0.0f, -9.8f, 0.0f);

    [Header("Display")]
    public Vector3 PlayerVelocity;

    public bool WeaponHeld = false;

    private CharacterController player;
    [HideInInspector]
    public HandIK handIK;

    private bool InputSprint = false;
    private float InputX;
    private float InputZ;

    private Vector3 lastPos;
    private bool moving = true;

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

    [HideInInspector]
    public bool PlayerControl = true;

    [HideInInspector]
    public bool playerADS = false;

    private bool Started = false;

    //[HideInInspector]
    public GameController gameCtrl;

    void Start()
    {
        if(!photonView.IsMine && Game.Instance.Networked)
        {
            FPSCamera.enabled = false;
            EnvCamera.enabled = false;

            foreach(GameObject obj in Meshes)
            {
                obj.layer = LayerMask.NameToLayer("Default");
            }

            PlayerUI.SetActive(false);

            return;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        /*cam.SensitivityX = SensitivityX;
        cam.SensitivityY = SensitivityY;
        cam.ADSSensitivityX = ADSSensitivityX;
        cam.ADSSensitivityY = ADSSensitivityY;

        FPSCamera.fieldOfView = FOV;
        EnvCamera.fieldOfView = FOV;*/

        //----------------------------

        toggleADS = false;
        toggleCrouch = false;
        SensitivityX = 2.0f;
        SensitivityY = 2.0f;
        ADSMultiplierX = 1.0f;
        ADSMultiplierY = 1.0f;
        FOV = 70.0f;

        //----------------------------

        player = GetComponent<CharacterController>();
        handIK = GetComponent<HandIK>();

        PlayerVelocity = Vector3.zero;

        InGameUI = PlayerUI.transform.Find("InGame").gameObject;
        SettingsUI = PlayerUI.transform.Find("Settings").gameObject;

        Hitmarker = InGameUI.transform.Find("Hitmarker").gameObject;
        Crosshair = InGameUI.transform.Find("Crosshair").gameObject;
        AmmoText = InGameUI.transform.Find("Ammo").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        DebugText = InGameUI.transform.Find("Debug").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

        DebugText.text = "Send Rate: " + PhotonNetwork.SendRate + " Serialize Rate: " + PhotonNetwork.SerializationRate;

        SettingsUI.SetActive(false);
        //Hitmarker.SetActive(false); default invis

        weapons.weapons[0].gameObject.SetActive(true);
        weapons.weapons[1].gameObject.SetActive(false);

        handIK.leftHandObj = weapons.weapons[0].Grip;
        handIK.rightHandObj = weapons.weapons[0].Trigger;

        if (Game.Instance.Networked)
            GetComponent<PhotonView>().RPC("WeaponSwap", RpcTarget.OthersBuffered, 0);

        Started = true;
    }
    
    void Update()
    {
        if (!photonView.IsMine && Game.Instance.Networked) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingsOpen = !SettingsOpen;
            SettingsUI.SetActive(SettingsOpen);

            if(SettingsOpen)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            PlayerControl = !SettingsOpen;
        }

        if (!PlayerControl)
        {
            return;
        }

        // Gravity
        PlayerVelocity += PlayerGravity * Time.deltaTime;
        
        // Landing
        if (player.isGrounded && PlayerVelocity.y < 0.0f || Climbing)
        {
            PlayerVelocity = Vector3.zero;
            Jumping = false;
        }

        // Jumping
        if (Input.GetButtonDown("Jump") && player.isGrounded /*|| IsGrounded()*/ && !Climbing)
        {
            PlayerVelocity += transform.up * PlayerJumpHeight;
            Jumping = true;
        }

        // Sprint & Speed Mods
        InputSprint = Input.GetKey(KeyCode.LeftShift);

        float multi = 1.0f;

        if(InputSprint)
        {
            multi = PlayerSprintSpeed;
        }

        if(Crouching)
        {
            multi = 0.75f;
        }

        // WASD
        InputX = Input.GetAxis("Horizontal") * multi;
        InputZ = Input.GetAxis("Vertical") * multi;

        // Moving
        if(Mathf.Abs(InputX) > 0.0f || Mathf.Abs(InputZ) > 0.0f)
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
                PlayerVelocity += transform.up * PlayerClimbSpeed;
            }
            else if (tempInZ < 0.0f)
            {
                PlayerVelocity += transform.up * -PlayerClimbSpeed;
            }
        }

        // Current Frame Movement
        Vector3 move = (transform.right * InputX) + (transform.forward * InputZ) + PlayerVelocity;

        player.Move(move * PlayerSpeed * Time.deltaTime);

        // Crouch
        if (Input.GetKeyDown(KeyCode.C))
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

        if (Input.GetKeyUp(KeyCode.C))
        {
            if (!toggleCrouch)
            {
                Crouching = false;

                Crouch(Crouching);
            }
        }

        // Weapon Change
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapons.weapons[0].gameObject.SetActive(true);
            weapons.weapons[1].gameObject.SetActive(false);

            //weapons.weapons[0].UpdateWeaponUI();

            handIK.leftHandObj = weapons.weapons[0].Grip;
            handIK.rightHandObj = weapons.weapons[0].Trigger;

            if (Game.Instance.Networked)
                GetComponent<PhotonView>().RPC("WeaponSwap", RpcTarget.OthersBuffered, 0);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapons.weapons[0].gameObject.SetActive(false);
            weapons.weapons[1].gameObject.SetActive(true);

            //weapons.weapons[1].UpdateWeaponUI();

            handIK.leftHandObj = weapons.weapons[1].Grip;
            handIK.rightHandObj = weapons.weapons[1].Trigger;

            if (Game.Instance.Networked)
                GetComponent<PhotonView>().RPC("WeaponSwap", RpcTarget.OthersBuffered, 1);
        }

        // Camera Angles TODO
        if (Input.GetKeyDown(KeyCode.F4))
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
        }

        lastPos = transform.position;

        DebugText.text = "Send Rate: " + PhotonNetwork.SendRate + " Ping: " + PhotonNetwork.GetPing() + " FPS: " + (1.0f / Time.deltaTime);

    }

    void LateUpdate()
    {
        //transform.forward = PlayerForward;
        //transform.right = PlayerRight;
        //transform.up = PlayerUp;
    }

    bool IsGrounded()
    {
        Vector3 feet = transform.Find("Cube").position;//transform.position - transform.up * 4.0f;
        Vector3 target = feet - transform.up * 0.4f;

        RaycastHit hit;
        if (Physics.Linecast(feet, target, out hit))
        {
            Debug.Log(hit.transform.name);
            return true;
        }

        return false;
    }

    void Crouch(bool crouched)
    {
        if (crouched)
        {
            Crouching = true;
            playerAnim.SetBool("Crouching", true);
            player.height = 4.9f;
            player.center = new Vector3(player.center.x, 2.9f, player.center.z);
        }
        else
        {
            Crouching = false;
            playerAnim.SetBool("Crouching", false);

            player.height = 8.0f;
            player.center = new Vector3(player.center.x, 4.0f, player.center.z);
        }
    }

    public int ChangeHealth(int damage)
    {
        Health -= damage;

        if(Health <= 0)
        {
            Health = 100;

            if(gameCtrl)
            {
                int index = Random.Range(0, gameCtrl.spawnPoints.Length);
                transform.position = gameCtrl.spawnPoints[index].position;
                transform.rotation = Quaternion.identity;

                if (Game.Instance.Networked)
                    GetComponent<PhotonView>().RPC("Respawn", RpcTarget.Others, index);

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

            if (gameCtrl)
            {
                int index = Random.Range(0, gameCtrl.spawnPoints.Length);
                transform.position = gameCtrl.spawnPoints[index].position;
                transform.rotation = Quaternion.identity;

                if (Game.Instance.Networked)
                    GetComponent<PhotonView>().RPC("Respawn", RpcTarget.AllBuffered, index);

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
    private void Respawn(int index)
    {
        if (gameCtrl)
        {
            transform.position = gameCtrl.spawnPoints[index].position;
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.position = new Vector3(0.0f, 40.0f, 0.0f);
            transform.rotation = Quaternion.identity;
        }
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
}
