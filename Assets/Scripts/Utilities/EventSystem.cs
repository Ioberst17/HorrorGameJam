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

    // COMBAT-CALCULATIONS
    public event Action<int, int> onAttackCollision;
    public void AttackHitTrigger(int weaponID, int weaponLevel)
    {
        {
            if (onAttackCollision != null)
            {
                onAttackCollision(weaponID, weaponLevel);
            }
        }
    }

    // WEAPON-RELATED EVENTS

    public event Action<int> onWeaponAddAmmoTrigger; // used to add ammo
    public void WeaponAddAmmoTrigger(int ammo)
    {
        if (onWeaponAddAmmoTrigger != null)
        {
            onWeaponAddAmmoTrigger(ammo);
        }
    }

    public event Action <int> onAmmoCheckTrigger; // used to check for ammo BEFORE  weapon AmmoTrigger is called to fire

    public void AmmoCheckTrigger(int fireDirection)
    {
        if (onAmmoCheckTrigger != null)
        {
            onAmmoCheckTrigger(fireDirection);
        }
    }

    public event Action <int, int, int, int> onWeaponFireTrigger; // used for weapon firing
    public void WeaponFireTrigger(int weaponID, int weaponLevel, int ammo, int direction)
    {
        if (onWeaponFireTrigger != null)
        {
            onWeaponFireTrigger(weaponID, weaponLevel, ammo, direction);
        }
    }

    public event Action<float> onStartTossingTrigger; // used for displaying force of player's throw, when throwing 
    public void StartTossingWeaponTrigger(float throwForce)
    {
        if (onStartTossingTrigger != null)
        {
            onStartTossingTrigger(throwForce);
        }
    }

    public event Action onFinishTossingTrigger; // used when 

    public void FinishTossingWeaponTrigger()
    {
        if (onFinishTossingTrigger != null)
        {
            onFinishTossingTrigger();
        }
    }

    public event Action <int, int> onWeaponLevelTrigger; // used when a weapon is leveled up
    public void WeaponLevelTrigger(int weaponID, int level)
    {
        if (onWeaponLevelTrigger != null)
        {
            onWeaponLevelTrigger(weaponID, level);
        }
    }

    public event Action<int> onWeaponChangeTrigger; // used when the player's current weapon is changed
    public void WeaponChangeTrigger(int weaponID)
    {
        if (onWeaponChangeTrigger != null)
        {
            onWeaponChangeTrigger(weaponID);
        }
    }

    public event Action<string, int> onUpdateWeaponUITrigger;

    public void UpdateWeaponUITrigger(string currentWeapon, int updatedAmmo)
    {
        if (onUpdateWeaponUITrigger != null)
        {
            onUpdateWeaponUITrigger(currentWeapon, updatedAmmo);
        }
    }

    public event Action<int, int> onUpdatePlayerWeaponTrigger;

    public void UpdatePlayerWeaponTrigger(int weaponID, int weaponLevel)
    {
        if(onUpdatePlayerWeaponTrigger != null)
        {
            onUpdatePlayerWeaponTrigger(weaponID, weaponLevel);
        }
    }

}
