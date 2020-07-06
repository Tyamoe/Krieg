using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class WeaponController : MonoBehaviour
{
    public PhotonView photonView;

    [Header("References/Pivots")]
    public Transform WeaponPivot;
    public Transform WeaponPivotHidden;

    public Transform CameraPivot;
    public Transform SightPivot;

    public Transform LeftHand;
    public Transform GunMagazine;

    public Transform Grip;
    public Transform Trigger;

    public float ForwardOffset = 7.5f;
    [SerializeField]
    private float forwardOffset = 0.0f;

    [Header("Variables/Shooting")]
    public float RPM = 900.0f;
    public int Damage = 25;

    [Header("Variables/Recoil")]
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

    [Header("Variables/ADS")]
    public bool toggleADS = false;
    [Range(0.0f, 5.0f)]
    public float ADSForwardLean = 1.4f;
    [Range(0.0f, 5.0f)]
    public float ADSDownwardLean = 0.3f;
    public float ADSSpeed = 0.2f;

    [Header("Variables/Hipfire")]
    [Range(0.0f, 1.0f)]
    public float HipfireSpreadX = 0.5f;
    [Range(0.0f, 1.0f)]
    public float HipfireSpreadY = 0.5f;

    [Header("Variables/Reload")]
    public int MagSize = 30;
    public int ClipCount = 3;
    public float ReloadSpeed = 2.1f;

    [Header("References")]
    public CameraController cameraCtrl;
    public PlayerController player;
    public Animator playerAnim;

    [Header("References/Audio")]
    public AudioSource audioSource;
    public AudioClip FireSFX;
    public AudioClip FireHitSFX;
    public AudioClip NoAmmoSFX;

    // Private References
    private RawImage Hitmarker;
    private GameObject Crosshair;

    private TMPro.TextMeshProUGUI AmmoText;

    [Header("Prefab References")]
    public GameObject MuzzleFlash;
    public GameObject ImpactDust;
    public GameObject BulletImpact;

    /***************************************/
    /**************** PRIVATE **************/
    /***************************************/

    private bool playerControl = true;

    // Recoil
    private bool recoilReset = true;
    private float recoilResetTimer = 0.0f;
    private Vector3 RecoilVector = Vector3.zero;
    private Vector3 tempRecoilVector = Vector3.zero;
    private float weaponShift = 0.0f;

    private Transform BulletSpawn;

    [HideInInspector]
    public bool flashActive = false;
    private bool canShoot = true;

    // Fire mode
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

    private bool ADSToggled = false;

    private float adsTime = 0.0f;
    private Vector3 adsVector = Vector3.zero;

    private int defualtMask;
    private int adsMask;

    // Reload
    private bool reloading = false;
    private bool triggerReload = false;
    private float reloadTime = 2.0f;

    private int magSize = 0;
    private int clipCount = 0;

    private int reserveAmmo = 0;

    private Transform tempReload = null;

    // Audio
    private bool noAmmoPlay = true;

    [PunRPC]
    private void ActivateMuzzleFlash()
    {
        GameObject f = Instantiate(MuzzleFlash, BulletSpawn.position, BulletSpawn.rotation);
        Destroy(f, 0.5f);
    }

    [PunRPC]
    private void ActivateImpactDust(Vector3 position, Quaternion rotation)
    {
        GameObject f = Instantiate(ImpactDust, position, rotation);
        Destroy(f, 0.5f);
    }

    [PunRPC]
    private void PlayFireSFX(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(FireSFX, position);
    }

    [PunRPC]
    private void DamageEnemy(PlayerController enemy)
    {
        enemy.ChangeHealth(Damage);
    }

    void OnEnable()
    {
        if(AmmoText)
            UpdateWeaponUI();
    }

    void Start()
    {
        WeaponChange();

        if (!photonView.IsMine && Game.Instance.Networked)
        {
            return;
        }

        forwardOffset = ForwardOffset;

        fireCooldown = RPM / 60.0f;
        fireCooldown = 1.0f / fireCooldown;
        currFireCooldown = 0.0f;

        magSize = MagSize;
        clipCount = ClipCount;

        reserveAmmo = clipCount * MagSize;

        AmmoText = player.AmmoText;
        AmmoText.text = magSize + "/" + reserveAmmo;

        initCameraPos = CameraPivot.localPosition;

        adsMask = 1 << LayerMask.NameToLayer("PlayerViewGun") | (1 << LayerMask.NameToLayer("PlayerUI"));
        defualtMask = (1 << LayerMask.NameToLayer("PlayerViewGun")) | (1 << LayerMask.NameToLayer("PlayerViewModel")) | (1 << LayerMask.NameToLayer("PlayerUI"));

        Crosshair = player.Crosshair;
        Hitmarker = player.Hitmarker.GetComponent<RawImage>();

        //Crosshair = GameObject.Find("Crosshair");
    }
    
    void Update()
    {
        if (!photonView.IsMine && Game.Instance.Networked) return;

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

            WeaponRotation = new Vector3(WeaponRotation.x, WeaponRotation.y - forwardOffset, WeaponRotation.z);
            WeaponRotationDef = WeaponRotation;
        }

        // Reload Trigger
        if((Input.GetKeyDown(KeyCode.R) && !reloading && MagSize < magSize && playerControl) || triggerReload)
        {
            if(reserveAmmo <= 0)
            {
                canShoot = false;
            }
            else
            {
                triggerReload = false;
                reloading = true;

                triggerUnADS = true;

                reloadTime = ReloadSpeed;
                playerAnim.SetFloat("ReloadTime", reloadTime);

                tempReload = GunMagazine;
                GunMagazine.parent = LeftHand;

                playerAnim.SetFloat("ReloadSpeed", 1.0f / ReloadSpeed);

                Crosshair.SetActive(false);
            }
        }

        // Reload Animation / Timers
        if(reloading)
        {
            reloadTime -= Time.deltaTime;
            playerAnim.SetFloat("ReloadTime", reloadTime);
            
            if(reloadTime <= 0.0f)
            {
                reloading = false;
                playerAnim.SetFloat("ReloadTime", -1.0f);
                reloadTime = 0.0f;

                Crosshair.SetActive(true);

                //playerAnim.speed = 1.0f;

                GunMagazine.parent = WeaponPivot;
                GunMagazine.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
                GunMagazine.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                GunMagazine.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                GunMagazine = tempReload;

                int mag = MagSize;
                reserveAmmo += mag;
                MagSize = Mathf.Clamp(reserveAmmo, 0, magSize); //magSize;
                reserveAmmo -= MagSize;// magSize - mag;
                ClipCount--;

                UpdateWeaponUI();
            }
        }

        // ADS Trigger
        if (Input.GetMouseButtonDown(1) && !reloading && playerControl || triggerADS)
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

                    adsVector = SightPivot.position - CameraPivot.position;
                }
                else
                {
                    triggerUnADS = false;

                    ads = false;
                    adsDone = false;

                    player.FPSCamera.cullingMask = defualtMask;
                }
            }
            else
            {
                triggerADS = false;

                ads = true;
                adsDone = false;

                adsTime = ADSSpeed;

                adsVector = SightPivot.position - CameraPivot.position;
            }
        }
        
        // Update ADS Sight Line
        if ((Input.GetMouseButton(1) && playerControl || ADSToggled) && !reloading)
        {
            if (adsDone)
            {
                CameraPivot.position = SightPivot.position;
            }
        }

        // ADS Animation / Timers
        if (ads && !adsDone)
        {
            CameraPivot.position += adsVector * (Time.deltaTime * (1.0f / ADSSpeed));

            // * WEAPON ROTATION OFFSET *
            forwardOffset -= ForwardOffset * ((2.0f * Time.deltaTime) * (1.0f / ADSSpeed));
            forwardOffset = Mathf.Clamp(forwardOffset, 0.0f, ForwardOffset);

            adsTime -= Time.deltaTime;
            if (adsTime <= 0.00f)
            {
                CameraPivot.position = SightPivot.position;
                adsDone = true;

                Crosshair.SetActive(false);

                RecoilVector = Vector3.zero;

                player.FPSCamera.cullingMask = adsMask;

                forwardOffset = 0.0f;
            }
        }

        // UnADS Trigger
        if (Input.GetMouseButtonUp(1) && playerControl || triggerUnADS)
        {
            if (!toggleADS || triggerUnADS)
            {
                ADSToggled = false;
                triggerUnADS = false;

                ads = false;
                adsDone = false;

                player.FPSCamera.cullingMask = defualtMask;
            }
        }

        // UnADS Animation
        if (!ads && !adsDone)
        {
            CameraPivot.localPosition = Vector3.Lerp(CameraPivot.localPosition, initCameraPos, ADSSpeed / 2.0f);

            // * WEAPON ROTATION OFFSET *
            forwardOffset += ForwardOffset * ((2.0f * Time.deltaTime) * (1.0f / ADSSpeed));
            forwardOffset = Mathf.Clamp(forwardOffset, 0.0f, ForwardOffset);

            if (Vector3.Distance(CameraPivot.localPosition, initCameraPos) < 0.01f)
            {
                CameraPivot.localPosition = initCameraPos;
                adsDone = true;

                forwardOffset = ForwardOffset;

                if (!reloading)
                {
                    Crosshair.SetActive(true);
                }
            }
        }

        player.playerADS = ads;

        // Shooting Trigger
        if (Input.GetMouseButton(0) && playerControl)
        {
            // Spray / Tap Detection
            if(!HeldDown)
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
            if (canShoot && MagSize > 0 && !reloading)
            {
                canShoot = false;

                MagSize--;
                UpdateWeaponUI();

                if (MagSize == 0 && reserveAmmo > 0)
                {
                    triggerReload = true;
                }

                Vector3 target = Vector3.zero;

                if (ads && adsDone)
                {
                    target = player.FPSCamera.transform.position + (Vector3.Normalize(player.FPSCamera.transform.forward + RecoilVector) * 10000.0f);
                }
                else
                {
                    Vector2 r = Random.insideUnitCircle;
                    Vector3 recoilV = new Vector3(r.x * HipfireSpreadX, r.y * HipfireSpreadY, 0.0f);
                    recoilV *= 0.5f;

                    Vector3 RecoilV = recoilV.x * player.FPSCamera.transform.right;
                    RecoilV += recoilV.y * player.FPSCamera.transform.up;

                    target = player.FPSCamera.transform.position + (Vector3.Normalize(player.FPSCamera.transform.forward + RecoilV) * 10000.0f);
                }

                RaycastHit hit;
                if (Physics.Linecast(player.FPSCamera.transform.position, target, out hit))
                {
                    PlayerController enemy = hit.transform.gameObject.GetComponent<PlayerController>();
                    if (enemy && enemy != player)
                    {
                        Color c = Color.green;
                        Collider coll = hit.collider;
                        var boxColl = coll as BoxCollider;
                        if (boxColl != null)
                        {
                            if(boxColl.size.magnitude < 5.0f)
                            {
                                c = Color.blue;
                            }
                        }

                        audioSource.PlayOneShot(FireHitSFX);

                        /*if (Game.Instance.Networked)
                            GetComponent<PhotonView>().RPC("DamageEnemy", RpcTarget.Others, enemy);*/
                        if (Game.Instance.Networked)
                            enemy.gameObject.GetPhotonView().RPC("ChangeHealthRPC", RpcTarget.AllBuffered, Damage);

                        /*int currHealth = enemy.ChangeHealth(Damage);
                        if(currHealth == 0)
                        {
                            c = Color.red;
                        }*/

                        IEnumerator coroutine;
                        coroutine = ShowHitmarker(0.45f, c);
                        StartCoroutine(coroutine);
                    }
                    else
                    {
                        // Bullet World Impact
                        GameObject g = Instantiate(ImpactDust, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                        Destroy(g, 0.5f);

                        if (Game.Instance.Networked)
                            GetComponent<PhotonView>().RPC("ActivateImpactDust", RpcTarget.Others, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

                        g = Instantiate(BulletImpact, hit.point + hit.normal * 0.05f, Quaternion.FromToRotation(Vector3.up, hit.normal));
                        Destroy(g, 8.5f);

                        audioSource.PlayOneShot(FireSFX);
                    }
                }
                else
                {
                    audioSource.PlayOneShot(FireSFX);
                }

                if (Game.Instance.Networked)
                    GetComponent<PhotonView>().RPC("PlayFireSFX", RpcTarget.Others, transform.position);

                float magx = RecoilMagnitudeX;
                float magy = RecoilMagnitudeY;

                // ADS Recoil Modifiers
                if (!HeldDown)
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
                
                // Muzzle Flash
                if (!flashActive)
                {
                    flashActive = true;
                    GameObject f = Instantiate(MuzzleFlash, BulletSpawn.position, BulletSpawn.rotation);
                    MuzzleFlash m = f.GetComponent<MuzzleFlash>();
                    m.parentWeapon = this;
                    f.transform.parent = BulletSpawn;

                    if (Game.Instance.Networked)
                        GetComponent<PhotonView>().RPC("ActivateMuzzleFlash", RpcTarget.Others);
                }
            }
            else if (MagSize <= 0 || reloading)
            {
                //audioSource.clip = NoAmmoSFX;

                if(noAmmoPlay)
                {
                    IEnumerator coroutine;
                    coroutine = PlayNoAmmo(0.5f);
                    StartCoroutine(coroutine);

                    //audioSource.PlayOneShot(NoAmmoSFX);
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
            }
        }
        else if(!recoilReset && playerControl)   // Recoil Reset Timers
        {
            recoilResetTimer -= Time.deltaTime;
            if(recoilResetTimer <= 0.0f)
            {
                recoilReset = true;

                RecoilVector = Vector3.zero;

                WeaponRotation = WeaponRotationDef;

                weaponShift = 0.0f;
            }
            else
            {
                RecoilVector += tempRecoilVector * (Time.deltaTime * (1.0f / RecoilReset));

                Vector3 euler = WeaponPivot.eulerAngles;
                float weaponShift1 = weaponShift * (Time.deltaTime * (1.0f / RecoilReset)) * -RecoilDeltaY;

                WeaponRotation = Vector3.Lerp(WeaponRotation, WeaponRotationDef, RecoilLerp);
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
        if(Input.GetMouseButtonUp(0) && playerControl)
        {
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

        newRot = Quaternion.Euler(WeaponRotationDef);
        WeaponPivotHidden.rotation = newRot;
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

    public void UpdateWeaponUI()
    {
        AmmoText.text = MagSize + "/" + (reserveAmmo < 0 ? 0 : reserveAmmo);
    }

    void WeaponChange()
    {
        GameObject a = WeaponPivot.Find("BulletExit").gameObject;//GameObject.Find("BulletExit");
        if (!a)
        {
            a = WeaponPivot.gameObject;
        }

        BulletSpawn = a.transform;

        //Debug.Log(BulletSpawn.name);
    }

}
