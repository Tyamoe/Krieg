using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public List<WeaponController> weapons;

    void Awake()
    {
        weapons = new List<WeaponController>();
        weapons.AddRange(transform.GetComponentsInChildren<WeaponController>());
    }

    void Start()
    {
       // weapons = new List<WeaponController>();

        /*foreach(WeaponController weapon in transform.GetComponentsInChildren< WeaponController>())
        {
            weapons.Add(weapon);
        }*/

        //weapons.AddRange(transform.GetComponentsInChildren<WeaponController>());
    }

    void Update()
    {
        
    }

    public void UpdateCustomSettings(bool ADStgl)
    {
        foreach(WeaponController weapon in weapons)
        {
            weapon.toggleADS = ADStgl;
        }
    }
}
