using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryWeaponsManager : WeaponsManager
{
    private float lastWeaponUseTime;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        EventSystem.current.onWeaponAddAmmoTrigger += AddAmmo;
        EventSystem.current.onAmmoCheckTrigger += CanWeaponBeFired;
        EventSystem.current.onWeaponFireTrigger += WeaponFired;

        player = GameObject.Find("Player");
        playerSecondaryWeapon = player.GetComponentInChildren<PlayerSecondaryWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void WeaponUIUpdate()
    {
        if (weaponList.Count > 0)
        {
            string weaponName = weaponList[currentWeaponIndex].name;
            int weaponAmmo = weaponList[currentWeaponIndex].ammo;

            int weaponID = weaponList[currentWeaponIndex].id;
            int weaponLevel = weaponList[currentWeaponIndex].level;

            EventSystem.current.UpdateSecondaryWeaponUITrigger(weaponName, weaponAmmo);
            EventSystem.current.UpdateSecondaryWeaponTrigger(weaponID, weaponLevel);
        }
    }

    public void AddAmmo(int ammoChange)
    {
        AddAmmo(currentWeaponID, ammoChange);
        WeaponUIUpdate();
    } // used for adding to current weapon

    public void AddAmmo(int weaponID, int ammoChange)
    {
        for (int i = 0; i < weaponList.Count; i++)
        { if (weaponList[i].id == weaponID) { weaponList[i].ammo += ammoChange; } }
    } // used when needing to add to a weapon with a specific ID e.g. non-current weapon

    public void AddAmmo(string weaponName, int ammoChange)
    {
        for (int i = 0; i < weaponList.Count; i++)
        { if (weaponList[i].name == weaponName) { weaponList[i].ammo += ammoChange; } }
    }

    public void WeaponFired(int weaponID, int weaponLevel, int ammoChange, int direction)
    {
        weaponList[currentWeaponIndex].ammo += ammoChange;
        WeaponUIUpdate();
    }

    private void CanWeaponBeFired(int fireDirection) // used as a check before firing a weapon and decrementing inventory
    {
        bool hasAmmo = weaponList[currentWeaponIndex].ammo > 0;
        bool doesNotExceedFireRate = Time.time > lastWeaponUseTime + weaponList[currentWeaponIndex].fireRate;
        bool canThrow = !playerSecondaryWeapon.inActiveThrow;

        if (hasAmmo && doesNotExceedFireRate && canThrow)
        {
            lastWeaponUseTime = Time.time;

            EventSystem.current.WeaponFireTrigger(
                currentWeaponID,
                weaponList[currentWeaponIndex].level,
                -1,
                weaponList[currentWeaponIndex].ammo); // send the weapon fire 
        }
        else if (!hasAmmo) { EventSystem.current.WeaponStopTrigger(); }
    }
    private new void OnDestroy()
    {
        base.OnDestroy();
        EventSystem.current.onWeaponAddAmmoTrigger -= AddAmmo;
        EventSystem.current.onAmmoCheckTrigger -= CanWeaponBeFired;
        EventSystem.current.onWeaponFireTrigger -= WeaponFired;
    }

}
