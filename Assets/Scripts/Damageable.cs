using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    List<WeaponData.WeaponType> _thingsThatCanDamageMe = new List<WeaponData.WeaponType>();

    public Action<WeaponData> OnHit;

    public bool CanItHurtMe(WeaponData.WeaponType weaponType)
    {
        return _thingsThatCanDamageMe.Contains(weaponType);
    }

    public void Hit(WeaponData weapon)
    {
        if (_thingsThatCanDamageMe.Contains(weapon.Type))
        {
            // I've been hit
            OnHit?.Invoke(weapon);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
