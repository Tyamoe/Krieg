using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public Animator muzzleFlash;

    [HideInInspector]
    public WeaponController parentWeapon;
    
    void Start()
    {
        float t = muzzleFlash.runtimeAnimatorController.animationClips[0].length;
        t /= 4.0f;
        Destroy(gameObject, t);
        
    }
    
    void Update()
    {
        
    }

    void OnDestroy()
    {
        if(parentWeapon)
            parentWeapon.flashActive = false;
    }
}
