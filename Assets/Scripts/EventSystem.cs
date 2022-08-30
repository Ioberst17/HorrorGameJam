using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventSystem : MonoBehaviour
{
    public static EventSystem current; // Singleton structure

    private void Awake()
    {
        current = this;
    }

    public event Action <int, int> onWeaponAmmoTrigger;
    public void WeaponAmmoTrigger(int weaponID, int ammo)
    {
        if (onWeaponAmmoTrigger != null)
        {
            onWeaponAmmoTrigger(weaponID, ammo);
        }
    }

    public event Action <int, int> onWeaponLevelTrigger;
    public void WeaponLevelTrigger(int weaponID, int level)
    {
        if (onWeaponLevelTrigger != null)
        {
            onWeaponLevelTrigger(weaponID, level);
        }
    }

}
