﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum WeaponClass
{
    Knife = 0,
    Pistol = 1,
    SMG = 2,
    Shotgun = 3,
    Rifle = 4,
    Sniper = 5,
}

public class WeaponController : MonoBehaviour
{
    public PhotonView photonView;

    [Space]
    public WeaponClass weaponClass;

    [Space]
    [Header("Firing")]
    public bool semiAuto = false;
    [Tooltip("Auto: ~500 -> ~1100 | Semi: ~200 -> ~400 | Bolt: ~30 -> 120")]
    public float RPM = 900.0f;
    public int Damage = 25;

    [Space]
    public LayerMask IgnoreMask;

    [Space]
    [Tooltip("Time between shots")]
    public float ShotDelay = 0.0f;

    [Header("Recoil")]
    // How Far
    [Range(0.0f, 1.0f)]
    public float RecoilMagnitudeX = 0.5f;
    [Range(0.0f, 180.0f)]
    public float RecoilMagnitudeY = 60.0f;

    // Favoured Direction
    [Range(-1.0f, 1.0f)]
    public float RecoilDeltaX = -1.0f;
    [Range(-1.0f, 1.0f)]
    public float RecoilDeltaY = 1.0f;

    // How fast
    [Range(0.0f, 5.0f)]
    public float RecoilVelocityX = 1.0f;
    [Range(0.0f, 5.0f)]
    public float RecoilVelocityY = 1.0f;

    public float RecoilReset = 1.0f;
    public float RecoilLerp = 1.1f;

    [Header("ADS")]
    public float ADSSpeed = 0.2f;
    public bool toggleADS = false;

    [Space]
    public Transform ADSPos;
    /*[Range(0.0f, 5.0f)]
    public float ADSForwardLean = 1.4f;
    [Range(0.0f, 5.0f)]
    public float ADSDownwardLean = 0.3f;*/

    // Guns Tilt when not ads
    public float ForwardOffset = 7.5f;

    [Header("Hipfire")]
    [Range(0.0f, 1.0f)]
    public float HipfireSpreadX = 0.5f;
    [Range(0.0f, 1.0f)]
    public float HipfireSpreadY = 0.5f;

    [Header("Reload")]
    public float ReloadSpeed = 2.1f;
    public int MagSize = 30;
    public int ClipCount = 3;

    [Header("Controller References")]
    public PlayerController player;
    public BotController bot;
    public CameraController cameraCtrl;
    public Animator playerAnim;

    [Header("Transform References")]
    public Transform WeaponPivot;
    public Transform WeaponPivotHidden;
    public Transform CameraPivot;

    [Space]
    public Transform LeftHand;

    [Space]
    public Transform GunMagazine;
    public Transform BarrelExit;
    public Transform SightPivot;
    public Transform Grip;
    public Transform Trigger;

    [Header("Audio References")]
    public AudioSource audioSource;
    public AudioClip FireSFX;
    public AudioClip FireHitSFX;
    public AudioClip NoAmmoSFX;

    [Header("Prefab References")]
    public GameObject MuzzleFlash;
    public GameObject TracerPrefab;

    /***************************************/
    /**************** PRIVATE **************/
    /***************************************/

    private bool playerControl = true;
    private bool Active = false;

    private bool Started = false;

    // Bot
    private bool isBot = false;

    // Private References
    private RawImage Hitmarker;
    private GameObject Crosshair;

    private TMPro.TextMeshProUGUI AmmoText;

    // Key Binds
    [HideInInspector]
    public KeyCode ReloadKey = KeyCode.R;
    [HideInInspector]
    public KeyCode ADSKey = KeyCode.Mouse1;
    [HideInInspector]
    public KeyCode FireKey = KeyCode.Mouse0;

    // Recoil
    private bool recoilReset = true;
    private float recoilResetTimer = 0.0f;
    private Vector3 RecoilVector = Vector3.zero;
    private Vector3 tempRecoilVector = Vector3.zero;
    private float weaponShift = 0.0f;

    [HideInInspector]
    public bool flashActive = false;
    private bool canShoot = true;

    // Fire mode
    private bool released = true;

    private float releaseTime = 0.0f;
    private float framesHeld = 0.0f;
    private bool HeldDown = false;
    private bool recentlyReleased = false;

    private float fireCooldown;
    private float currFireCooldown;

    // ADS
    private Vector3 initCameraPos = Vector3.zero;

    private bool ads = false;
    private bool adsDone = false;
    private bool triggerADS = false;
    private bool triggerUnADS = false;

    private float forwardOffset = 0.0f;

    private bool ADSToggled = false;

    private float adsTime = 0.0f;
    private Vector3 adsVector = Vector3.zero;

    private int defualtMask;
    private int adsMask;

    // Hipfire
    private float hipfireSpreadX;
    private float hipfireSpreadY;

    // Reload
    private bool reloading = false;
    private bool triggerReload = false;
    private float reloadTime = 2.0f;

    private int magSize = 0;
    private int clipCount = 0;

    private int reserveAmmo = 0;

    private Transform tempReload = null;

    private Vector3 locPos;
    private Vector3 locPos1;
    private Vector3 locPos2;

    private bool reloadAds = false;

    // Audio
    private bool noAmmoPlay = true;

    void OnEnable()
    {
        //locPos1 = WeaponPivotHidden.localPosition;
        Active = false;
    }

    void OnDisable()
    {
        Active = false;
    }

    void Awake()
    {
        if(!player && bot)
        {
            isBot = true;
        }

        locPos1 = WeaponPivotHidden.localPosition;
        locPos2 = GunMagazine.localPosition;
    }

    void Start()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        forwardOffset = ForwardOffset;

        fireCooldown = RPM / 60.0f;
        fireCooldown = 1.0f / fireCooldown;
        currFireCooldown = 0.0f;

        ShotDelay = fireCooldown;

        magSize = MagSize;
        clipCount = ClipCount;

        reserveAmmo = clipCount * MagSize;

        AmmoText = player.AmmoText;
        AmmoText.text = magSize + "/" + reserveAmmo;

        initCameraPos = CameraPivot.localPosition;
        locPos = GunMagazine.localPosition;

        adsMask = 1 << LayerMask.NameToLayer("PlayerViewGun") | (1 << LayerMask.NameToLayer("PlayerUI"));
        defualtMask = (1 << LayerMask.NameToLayer("PlayerViewGun")) | (1 << LayerMask.NameToLayer("PlayerViewModel")) | (1 << LayerMask.NameToLayer("PlayerUI"));

        Crosshair = player.Crosshair;
        Hitmarker = player.Hitmarker.GetComponent<RawImage>();

        UpdateHipfireSpread();

        Started = true;

        WeaponPivotHidden.localPosition = locPos1;
        player.FPSCamera.fieldOfView = 75.0f;
        WeaponPivot.localEulerAngles = new Vector3(0.0f, 355.0f, 0.0f);
    }
    
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        if(playerControl != player.PlayerControl)
        {
            triggerUnADS = true;
        }

        playerControl = player.PlayerControl;

        //Vector3 WeaponRotation = new Vector3(cameraCtrl.WeaponTiltX, WeaponPivot.eulerAngles.y, WeaponPivot.eulerAngles.z);
        Vector3 WeaponRotation = new Vector3(WeaponPivot.eulerAngles.x, WeaponPivot.eulerAngles.y, WeaponPivot.eulerAngles.z);
        Vector3 WeaponRotationDef = new Vector3(WeaponPivotHidden.eulerAngles.x, WeaponPivotHidden.eulerAngles.y, WeaponPivotHidden.eulerAngles.z);

        {
            Vector3 target = player.FPSCamera.transform.position + (player.FPSCamera.transform.forward * 10000.0f);
            Vector3 relativePos = target - WeaponPivot.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            WeaponRotation = rotation.eulerAngles;

            WeaponRotation = new Vector3(WeaponRotation.x, WeaponRotation.y/* - forwardOffset*/, WeaponRotation.z);
            WeaponRotationDef = WeaponRotation;
        }

        // Reload Trigger
        if((Input.GetKeyDown(ReloadKey) && !reloading && MagSize < magSize && playerControl) || triggerReload)
        {
            if(reserveAmmo <= 0)
            {
                canShoot = false;
            }
            else
            {
                reloadAds = player.playerADS;

                triggerReload = false;
                reloading = true;

                triggerUnADS = true;

                reloadTime = ReloadSpeed;
                playerAnim.SetFloat("ReloadTime", reloadTime);

                //tempReload = GunMagazine;
                GunMagazine.parent = LeftHand;
                GunMagazine.localPosition = Vector3.zero;

                playerAnim.SetFloat("ReloadSpeed", 1.0f / ReloadSpeed);

                Crosshair.SetActive(false);

                player.playerReload = true;

                player.handIK.LeftIK = 0.3f;
                if (PhotonNetwork.IsConnected)
                {
                    photonView.RPC("UpdateLeftIK", RpcTarget.Others, 0.0f);
                }
            }
        }

        // Reload Animation / Timers
        if(reloading)
        {
            reloadTime -= Time.deltaTime;
            playerAnim.SetFloat("ReloadTime", reloadTime);

            float ik = mapToRange(reloadTime - Time.deltaTime, 0.0f, ReloadSpeed, 0.0f, 0.3f);
            player.handIK.LeftIK = ik;

            if (reloadTime <= 0.0f)
            {
                reloading = false;
                playerAnim.SetFloat("ReloadTime", -1.0f);
                reloadTime = 0.0f;

                GunMagazine.parent = WeaponPivot;
                //GunMagazine.localPosition = locPos;
                GunMagazine.localPosition = locPos2;
                GunMagazine.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                GunMagazine.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                //GunMagazine = tempReload;

                int mag = MagSize;
                reserveAmmo += mag;
                MagSize = Mathf.Clamp(reserveAmmo, 0, magSize);
                reserveAmmo -= MagSize;
                ClipCount--;

                UpdateWeaponUI();

                if (reloadAds && Input.GetMouseButton(1))
                {
                    triggerADS = true;
                    reloadAds = false;
                }
                else
                {
                    Crosshair.SetActive(true);
                }

                player.playerReload = false;

                player.handIK.LeftIK = 0.45f;
                if (PhotonNetwork.IsConnected)
                {
                    photonView.RPC("UpdateLeftIK", RpcTarget.Others, 0.45f);
                }
            }
        }

        // ADS Trigger
        if (Input.GetKeyDown(ADSKey) && !reloading && playerControl || triggerADS)
        {
            if(toggleADS)
            {
                ADSToggled = !ADSToggled;

                if(ADSToggled)
                {
                    triggerADS = false;

                    ads = true;
                    adsDone = false;

                    adsTime = ADSSpeed;

                    //adsVector = SightPivot.position - CameraPivot.position;
                    adsVector = ADSPos.localPosition - WeaponPivotHidden.localPosition;
                    //locPos1 = WeaponPivotHidden.localPosition;
                }
                else
                {
                    triggerUnADS = false;

                    ads = false;
                    adsDone = false;

                    adsTime = ADSSpeed;
                    adsVector = WeaponPivotHidden.localPosition - locPos1;

                    player.FPSCamera.cullingMask = defualtMask;
                }
            }
            else
            {
                triggerADS = false;

                ads = true;
                adsDone = false;

                adsTime = ADSSpeed;

                //adsVector = SightPivot.position - CameraPivot.position;
                adsVector = ADSPos.localPosition - WeaponPivotHidden.localPosition;
                //locPos1 = WeaponPivotHidden.localPosition;
            }
        }
        
        // Update ADS Sight Line
        if ((Input.GetKey(ADSKey) && playerControl || ADSToggled) && !reloading)
        {
            if (adsDone && ads)
            {
                //CameraPivot.position = SightPivot.position;
                WeaponPivotHidden.localPosition = ADSPos.localPosition;
                WeaponPivotHidden.eulerAngles = ADSPos.eulerAngles;
                //WeaponPivot.localPosition = new Vector3(0.0f, 0.2f, WeaponPivot.localPosition.z);
            }
        }

        // ADS Animation / Timers
        if (ads && !adsDone)
        {
            //CameraPivot.position += adsVector * (Time.deltaTime * (1.0f / ADSSpeed));
            WeaponPivotHidden.localPosition += adsVector * (Time.deltaTime * (1.0f / ADSSpeed));

            player.FPSCamera.fieldOfView -= 20.0f * (Time.deltaTime * (1.0f / ADSSpeed));

            // * WEAPON ROTATION OFFSET *
            forwardOffset -= ForwardOffset * ((2.0f * Time.deltaTime) * (1.0f / ADSSpeed));
            forwardOffset = Mathf.Clamp(forwardOffset, 0.0f, ForwardOffset);

            adsTime -= Time.deltaTime;
            if (adsTime <= 0.00f)
            {
                player.FPSCamera.fieldOfView = 55.0f;
                //CameraPivot.position = SightPivot.position;
                //WeaponPivot.localPosition = new Vector3(0.0f, 0.0f, WeaponPivot.localPosition.z);
                WeaponPivot.localEulerAngles = Vector3.zero;

                WeaponPivotHidden.localPosition = ADSPos.localPosition;

                adsDone = true;

                Crosshair.SetActive(false);

                RecoilVector = Vector3.zero;

                //player.FPSCamera.cullingMask = adsMask;

                forwardOffset = 0.0f;
            }
        }

        // UnADS Trigger
        if (Input.GetKeyUp(ADSKey) && playerControl || triggerUnADS)
        {
            if (!toggleADS || triggerUnADS)
            {
                ADSToggled = false;
                triggerUnADS = false;

                ads = false;
                adsDone = false;

                adsTime = ADSSpeed;
                adsVector = WeaponPivotHidden.localPosition - locPos1;

                player.FPSCamera.cullingMask = defualtMask;
            }
        }

        // UnADS Animation
        if (!ads && !adsDone)
        {
            //CameraPivot.localPosition = Vector3.Lerp(CameraPivot.localPosition, initCameraPos, ADSSpeed / 2.0f);
            //WeaponPivotHidden.localPosition = Vector3.Lerp(WeaponPivotHidden.localPosition, locPos1, ADSSpeed / 2.0f);
            WeaponPivotHidden.localPosition -= adsVector * (Time.deltaTime * (1.0f / ADSSpeed));

            player.FPSCamera.fieldOfView += 20.0f * (Time.deltaTime * (1.0f / ADSSpeed));

            // * WEAPON ROTATION OFFSET *
            forwardOffset += ForwardOffset * ((2.0f * Time.deltaTime) * (1.0f / ADSSpeed));
            forwardOffset = Mathf.Clamp(forwardOffset, 0.0f, ForwardOffset);

            //if (Vector3.Distance(CameraPivot.localPosition, initCameraPos) < 0.01f)
            //if (Vector3.Distance(WeaponPivotHidden.localPosition, locPos1) < 0.01f)

            adsTime -= Time.deltaTime;
            if (adsTime <= 0.00f)
            {
                //CameraPivot.localPosition = initCameraPos;
                WeaponPivotHidden.localPosition = locPos1;
                adsDone = true;

                player.FPSCamera.fieldOfView = 75.0f;

                //WeaponPivot.localPosition = new Vector3(0.0f, 0.0f, WeaponPivot.localPosition.z);
                WeaponPivot.localEulerAngles = new Vector3(0.0f, 355.0f, 0.0f);

                forwardOffset = ForwardOffset;

                adsVector = Vector3.zero;

                if (!reloading)
                {
                    Crosshair.SetActive(true);
                }
            }
        }

        player.playerADS = ads;

        // Shooting Trigger
        if (Input.GetKey(FireKey) && playerControl)
        {
            // Spray / Tap Detection
            if (!HeldDown && !semiAuto)
            {
                // Spraying if trigger held from > 500ms
                if(framesHeld > 0.5f)
                {
                    HeldDown = true;
                    framesHeld = 0.0f;
                }
                else
                {
                    framesHeld += Time.deltaTime;
                }
                recoilResetTimer = RecoilReset;
            }
            
            // Firing Bullet
            if (canShoot && MagSize > 0 && !reloading && released)
            {
                canShoot = false;

                player.handIK.LeftIK = 0.44f;
                player.handIK.RightIK = 0.435f;
                //WeaponPivot.localEulerAngles = new Vector3(0.0f, -4.0f, -0.15f);
                WeaponRotation += new Vector3(-0.1f, 0.0f, -0.1f);

                MagSize--;
                UpdateWeaponUI();

                if (MagSize == 0 && reserveAmmo > 0)
                {
                    triggerReload = true;
                }

                //Vector3 target = Vector3.zero;
                Vector3 dir = Vector3.zero;

                if (ads && adsDone)
                {
                    dir = Vector3.Normalize(player.FPSCamera.transform.forward + RecoilVector);
                }
                else
                {
                    Vector2 r = Random.insideUnitCircle;
                    Vector3 recoilV = new Vector3(r.x * hipfireSpreadX, r.y * hipfireSpreadY, 0.0f);
                    recoilV *= 0.5f;

                    Vector3 RecoilV = recoilV.x * player.FPSCamera.transform.right;
                    RecoilV += recoilV.y * player.FPSCamera.transform.up;

                    dir = Vector3.Normalize(player.FPSCamera.transform.forward + RecoilV);
                }
                //target = player.FPSCamera.transform.position + (dir * 10000.0f);

                //RaycastHit[] hits = Physics.RaycastAll(player.FPSCamera.transform.position, dir, 1000.0f);
                RaycastHit hit;
                if (Physics.Raycast(player.FPSCamera.transform.position, dir, out hit, 1000.0f, ~IgnoreMask))
                {
                    PlayerController enemy = null;
                    float multiplier = 0.0f;
                    int addDamage = 0;

                    PlayerCollider enemyCol = hit.transform.gameObject.GetComponent<PlayerCollider>();
                    if (enemyCol)
                    {
                        enemy = enemyCol.player;
                        multiplier = enemyCol.multiplier - 1.0f;
                        addDamage = Mathf.CeilToInt(multiplier * (float)Damage);
                    }

                    if (enemy && enemy != player)
                    {
                        Color c = Color.green;
                        if(multiplier > 0.0f)
                        {
                            c = Color.blue;
                        }

                        audioSource.PlayOneShot(FireHitSFX);

                        if (PhotonNetwork.IsConnected)
                            enemy.gameObject.GetPhotonView().RPC("ChangeHealthRPC", RpcTarget.All, (Damage + addDamage), player.FPSCamera.transform.forward, player.photonView.ViewID, enemy.photonView.ViewID);

                        IEnumerator coroutine;
                        coroutine = ShowHitmarker(0.45f, c);
                        StartCoroutine(coroutine);
                    }
                    else
                    {
                        audioSource.PlayOneShot(FireSFX);
                    }
                }
                else
                {
                    audioSource.PlayOneShot(FireSFX);
                }

                if (PhotonNetwork.IsConnected)
                    GetComponent<PhotonView>().RPC("PlayFireSFX", RpcTarget.Others, transform.position);

                float magx = RecoilMagnitudeX;
                float magy = RecoilMagnitudeY;

                // ADS Recoil Modifiers
                if (!HeldDown && !semiAuto)
                {
                    magx *= 0.85f;
                    magy *= 0.85f;
                }

                if (ads && adsDone)
                {
                    magx *= 0.65f;
                    magy *= 0.65f;
                }

                // Weapon Shake / Recoil Calculation
                float recoilX = ((1.0f / RecoilVelocityX) * Time.deltaTime) * magx * RecoilDeltaX;
                float recoilY = ((1.0f / RecoilVelocityY) * Time.deltaTime) * magy * RecoilDeltaY;

                if(ads && adsDone)
                {
                    float sd = recoilX * 45.0f;
                    cameraCtrl.UpdateEulerY(sd);
                }
                else
                {
                    RecoilVector += recoilX * player.FPSCamera.transform.right;
                    //RecoilVector += recoilY * (FPSCamera.transform.up);
                }

                //  Vertical Recoil
                float camRot = recoilY * -1.0f;
                cameraCtrl.UpdateEulerX(camRot);
                
                // Muzzle Flash & Tracer
                if (!flashActive)
                {
                    flashActive = true;
                    GameObject f = Instantiate(MuzzleFlash, BarrelExit.position, BarrelExit.rotation);
                    MuzzleFlash m = f.GetComponent<MuzzleFlash>();
                    m.parentWeapon = this;
                    f.transform.parent = BarrelExit;

                    if (PhotonNetwork.IsConnected)
                        GetComponent<PhotonView>().RPC("ActivateMuzzleFlash", RpcTarget.Others);

                    // Tracer
                    if (PhotonNetwork.IsConnected)
                    {
                        GetComponent<PhotonView>().RPC("InitiateTracing", RpcTarget.All, dir);
                    }
                    else
                    {
                        GameObject f150 = Instantiate(TracerPrefab, BarrelExit.position, BarrelExit.rotation);
                        f150.GetComponent<Tracer>().direction = dir;
                    }
                }

                if (semiAuto)
                {
                    released = false;
                }
            }
            else if (MagSize <= 0 || reloading)
            {
                if(noAmmoPlay)
                {
                    IEnumerator coroutine;
                    coroutine = PlayNoAmmo(0.5f);
                    StartCoroutine(coroutine);
                }
            }
            else // Shooting Cooldown
            {
                currFireCooldown -= Time.deltaTime;

                if(currFireCooldown <= 0.0f)
                {
                    currFireCooldown = fireCooldown;
                    canShoot = true;
                }
                else if(currFireCooldown <= fireCooldown / 2.0f)
                {
                    player.handIK.LeftIK = 0.45f;
                    player.handIK.RightIK = 0.45f;
                    //WeaponPivot.localEulerAngles = new Vector3(0.0f, -4.0f, 0.0f);
                    WeaponRotation -= new Vector3(-0.15f, 0.0f, -0.15f);
                }
            }
        }
        else if(!recoilReset && playerControl)   // Recoil Reset Timers
        {
            if(!reloading)
            {
                player.handIK.LeftIK = 0.45f;
                player.handIK.RightIK = 0.45f;
                WeaponPivot.localEulerAngles = new Vector3(0.0f, -4.0f, 0.0f);
            }

            recoilResetTimer -= Time.deltaTime;
            if(recoilResetTimer <= 0.0f)
            {
                recoilReset = true;

                RecoilVector = Vector3.zero;

                weaponShift = 0.0f;

                //cameraCtrl.UnAccum();
            }
            else
            {
                RecoilVector += tempRecoilVector * (Time.deltaTime * (1.0f / RecoilReset));

                Vector3 euler = WeaponPivot.eulerAngles;
                float weaponShift1 = weaponShift * (Time.deltaTime * (1.0f / RecoilReset)) * -RecoilDeltaY;

                //WeaponRotation = Vector3.Lerp(WeaponRotation, WeaponRotationDef, RecoilLerp);
            }
        }
        
        // Fire Rate Timers
        if (recentlyReleased && playerControl)
        {
            releaseTime += Time.deltaTime;

            if(releaseTime >= fireCooldown)
            {
                releaseTime = 0.0f;
                recentlyReleased = false;

                currFireCooldown = fireCooldown;
                canShoot = true;
            }
        }

        // Shooting Trigger Release
        if(Input.GetKeyUp(FireKey) && playerControl)
        {
            released = true;

            framesHeld = 0.0f;
            HeldDown = false;

            if(!recentlyReleased)
            {
                recentlyReleased = true;
            }

            recoilReset = false;

            recoilResetTimer = RecoilReset;
            tempRecoilVector = Vector3.zero - RecoilVector;
        }

        Quaternion newRot = Quaternion.Euler(WeaponRotation);
        WeaponPivot.rotation = newRot;

        //newRot = Quaternion.Euler(WeaponRotationDef);
        //WeaponPivotHidden.rotation = newRot;
    }

    public void WeaponSwap(bool active)
    {
        if (Active == active) return;

        if (active)
        {
            if (Started)
            {
                UpdateWeaponUI();
                UpdateHipfireSpread(true);

                if (player.playerADS)
                {
                    triggerADS = true;
                }
                player.playerReload = false;
            }
        }
        else
        {
            if (Started)
            {
                ADSToggled = false;
                triggerUnADS = false;

                ads = false;
                adsDone = false;

                player.FPSCamera.cullingMask = defualtMask;

                //
                CameraPivot.localPosition = initCameraPos;
                adsDone = true;

                forwardOffset = ForwardOffset;

                Crosshair.SetActive(false);

                //
                if (reloading)
                {
                    reloading = false;
                    playerAnim.SetFloat("ReloadTime", -1.0f);
                    reloadTime = 0.0f;

                    GunMagazine.parent = WeaponPivot;
                    GunMagazine.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
                    GunMagazine.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    GunMagazine.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    GunMagazine = tempReload;
                }

                //
                canShoot = true;
                currFireCooldown = fireCooldown;
                //player.playerADS = false;
                HeldDown = false;
                framesHeld = 0.0f;

                recoilReset = true;
                RecoilVector = Vector3.zero;
                weaponShift = 0.0f;

                releaseTime = 0.0f;
                recentlyReleased = false;
                currFireCooldown = fireCooldown;

                player.playerReload = false;
            }
        }
    }

    IEnumerator ShowHitmarker(float showTime, Color hitmarkerColor)
    {
        Hitmarker.color = hitmarkerColor;
        yield return new WaitForSeconds(showTime);
        Hitmarker.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    IEnumerator PlayNoAmmo(float time)
    {
        audioSource.PlayOneShot(NoAmmoSFX);
        noAmmoPlay = false;
        yield return new WaitForSeconds(time);
        noAmmoPlay = true;
    }

    float mapToRange(float val, float r1s, float r1e, float r2s, float r2e)
    {
        return (val - r1s) / (r1e - r1s) * (r2e - r2s) + r2s;
    }

    public void UpdateHipfireSpread(float x = 1.0f, float y = 1.0f, bool scale = true)
    {
        hipfireSpreadX = HipfireSpreadX * x;
        hipfireSpreadY = HipfireSpreadY * y;

        if(scale)
        {
            float sx = mapToRange(hipfireSpreadX, 0.0f, 1.0f, 0.8f, 1.8f);
            float sy = mapToRange(hipfireSpreadY, 0.0f, 1.0f, 0.8f, 1.8f);

            Crosshair.GetComponent<RectTransform>().localScale = new Vector3(sx, sy, 1.0f);
        }
    }
    public void UpdateHipfireSpread(bool scale)
    {
        if (scale)
        {
            float sx = mapToRange(hipfireSpreadX, 0.0f, 1.0f, 0.8f, 1.8f);
            float sy = mapToRange(hipfireSpreadY, 0.0f, 1.0f, 0.8f, 1.8f);

            Crosshair.GetComponent<RectTransform>().localScale = new Vector3(sx, sy, 1.0f);
        }
    }

    public void UpdateWeaponUI()
    {
        AmmoText.text = MagSize + "/" + (reserveAmmo < 0 ? 0 : reserveAmmo);
    }

    public void ResetAmmo()
    {
        MagSize = magSize;
        ClipCount = clipCount;

        reserveAmmo = clipCount * MagSize;

        if(Active)
            AmmoText.text = magSize + "/" + reserveAmmo;
    }

    [PunRPC]
    void InitiateTracing(Vector3 diri)
    {
        GameObject f = Instantiate(TracerPrefab, BarrelExit.position, BarrelExit.rotation);
        f.GetComponent<Tracer>().direction = diri;
    }

    [PunRPC]
    private void TracerRPC()
    {
        GameObject f = Instantiate(TracerPrefab, BarrelExit.position, BarrelExit.rotation);
        f.GetComponent<Tracer>().direction = player.FPSCamera.transform.forward;
    }

    [PunRPC]
    private void ActivateMuzzleFlash()
    {
        GameObject f = Instantiate(MuzzleFlash, BarrelExit.position, BarrelExit.rotation);
        Destroy(f, 0.5f);
    }

    [PunRPC]
    private void PlayFireSFX(Vector3 position)
    {
        //Debug.Log("PlayFireSFX: " + PhotonNetwork.NickName);
        //AudioSource.PlayClipAtPoint(FireSFX, position);
        audioSource.PlayOneShot(FireSFX);
    }
}
