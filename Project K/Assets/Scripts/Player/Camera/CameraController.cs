﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    public PhotonView photonView;

    public PlayerController player;

    [Header("Pivots")]
    public Transform PlayerPivot;
    public Transform WeaponPivot;

    [Header("Camera Angles")]
    public Transform OverTheShoulder;
    public Transform SideProfile;
    public Transform Profile;

    private Vector3 fv = Vector3.forward;

    [Space]
    public float MinCameraAngle = -40.0f;
    public float MaxCameraAngle = 40.0f;

    public float MinCameraAngle_Weapon = -30.0f;
    public float MaxCameraAngle_Weapon = 30.0f;

    [Header("Camera Collision")]
    public GameObject Camera;

    [Space]
    public float LerpSmooth = 10.0f;

    [HideInInspector]
    public float WeaponTiltX;

    private float RotatonX;
    private float RotatonY;
    private float RotatonXX;

    private float InputX;
    private float InputY;

    private Vector3 defPos;
    private int currAngle = 0;

    // Custom Settings
    private float SensitivityX_ = 100.0f;
    public float SensitivityX
    {
        get { return SensitivityX_; }
        set
        {
            if (SensitivityX_ == value * 100.0f) return;

            SensitivityX_ = value * 100.0f;
            //Debug.Log("Sens X: " + value);
        }
    }

    private float SensitivityY_ = 100.0f;
    public float SensitivityY
    {
        get { return SensitivityY_; }
        set
        {
            if (SensitivityY_ == value * 100.0f) return;

            SensitivityY_ = value * 100.0f;
        }
    }

    public float ADSMultiplierX = 1.0f;

    public float ADSMultiplierY = 1.0f;

    void Awake()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        defPos = transform.localPosition;
    }
    
    void Start()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        RotatonX = transform.eulerAngles.x;
        RotatonY = PlayerPivot.eulerAngles.y;

        RotatonXX = WeaponPivot.eulerAngles.x;
    }
    
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        if (!player.PlayerControl) return;

        InputX = Input.GetAxis("Mouse X");
        InputY = Input.GetAxis("Mouse Y");

        RotatonX += InputY * -SensitivityX_ * (player.playerADS ? ADSMultiplierX : 1.0f) * Time.deltaTime;

        RotatonXX += InputY * -SensitivityX_ * (player.playerADS ? ADSMultiplierY : 1.0f) * Time.deltaTime;

        RotatonY += InputX * SensitivityY_ * (player.playerADS ? ADSMultiplierY : 1.0f) * Time.deltaTime;

        RotatonX = Mathf.Clamp(RotatonX, MinCameraAngle, MaxCameraAngle);

        RotatonXX = Mathf.Clamp(RotatonXX, MinCameraAngle, MaxCameraAngle);

        Quaternion newRot = Quaternion.Euler(RotatonX, transform.eulerAngles.y, transform.eulerAngles.z);

        //if(currAngle == 0)
        {
            transform.rotation = newRot;
        }

        newRot = Quaternion.Euler(PlayerPivot.eulerAngles.x, RotatonY, PlayerPivot.eulerAngles.z);
        PlayerPivot.rotation = newRot;

        //newRot = Quaternion.Euler(WeaponPivot.eulerAngles.x, WeaponPivot.eulerAngles.y, RotatonX);
        //WeaponPivot.rotation = newRot;

        WeaponTiltX = RotatonX;

        newRot = Quaternion.Euler(RotatonXX, WeaponPivot.eulerAngles.y, WeaponPivot.eulerAngles.z);
        //WeaponPivot.rotation = newRot;
    }

    float accumY = 0.0f;

    public void UpdateEulerX(float rotX)
    {
        RotatonX += rotX;
        RotatonXX += rotX;

        accumY += rotX;
    }

    public float accumLerp = 0.45f;
    float timeAccum = 0.0f;

    IEnumerator LerpAccum(float acum)
    {
        timeAccum = accumLerp;

        float dif1 = RotatonX - acum;
        float dif2 = RotatonXX - acum;

        while (timeAccum > 0.0f)
        {
            //RotatonX = Mathf.Lerp(RotatonX, RotatonX - acum, accumLerp);
            //RotatonXX = Mathf.Lerp(RotatonXX, RotatonXX - acum, accumLerp);

            RotatonX += dif1 * (Time.deltaTime * (1.0f / accumLerp));
            RotatonXX += dif2 * (Time.deltaTime * (1.0f / accumLerp));

            timeAccum -= Time.deltaTime;

            yield return new WaitForSeconds(0.00f);
        }

        timeAccum = 0.0f;
    }

    public void UnAccum()
    {
        RotatonX -= accumY;
        RotatonXX -= accumY;

        //RotatonX = Mathf.Lerp(RotatonX, RotatonX - accumY, accumLerp);
        //RotatonXX = Mathf.Lerp(RotatonXX, RotatonXX - accumY, accumLerp);

        //StartCoroutine(LerpAccum(accumY));

        accumY = 0.0f;
    }

    public void UpdateEulerY(float rotY)
    {
        RotatonY += rotY;
    }

    public void SetCamAngle(int profile)
    {
        if(profile == 1)
        {
            transform.parent = OverTheShoulder;
            transform.localPosition = Vector3.zero;
            transform.rotation = OverTheShoulder.rotation;

            if (currAngle == 0)
            {

            }
        }
        else  if (profile == 2)
        {
            transform.parent = SideProfile;
            transform.localPosition = Vector3.zero;
            transform.rotation = SideProfile.rotation;
        }
        else if (profile == 3)
        {
            transform.parent = Profile;
            transform.localPosition = Vector3.zero;
            transform.rotation = Profile.rotation;
        }
        else
        {
            transform.parent = PlayerPivot;
            transform.localPosition = defPos;
        }

        currAngle = profile;
    }
}
