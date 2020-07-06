﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HandIK : MonoBehaviour
{
    public PhotonView photonView;

    public Animator playerAnim;

    public bool ikActive = false;

    public Transform leftHandObj = null;
    public Transform rightHandObj = null;
    public Transform lookObj = null;

    public float PositionIK = 0.5f;
    public float RotationIK = 0.5f;

    void OnAnimatorIK()
    {
        if (!photonView.IsMine && Game.Instance.Networked) return;
        if (playerAnim)
        {
            if (ikActive)
            {
                if (lookObj != null)
                {
                    //playerAnim.SetLookAtWeight(0.1f);
                    //playerAnim.SetLookAtPosition(lookObj.position);
                }
                
                if (rightHandObj != null)
                {
                    playerAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, PositionIK);
                    playerAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, RotationIK);
                    playerAnim.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    playerAnim.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
                if (leftHandObj != null)
                {
                    playerAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, PositionIK);
                    playerAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, RotationIK);
                    playerAnim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    playerAnim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
            }
            else
            {
                playerAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                playerAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                playerAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                playerAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                playerAnim.SetLookAtWeight(0);
            }
        }
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        if (!photonView.IsMine && Game.Instance.Networked) return;

        if (Input.GetKeyDown(KeyCode.L))
        {
            ikActive = !ikActive;
        }
    }
}
