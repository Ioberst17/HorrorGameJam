using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryWeaponsManager : WeaponsManager
{
    private float lastWeaponUseTime;
    private PlayerController playerController;
    private ChargePunch chargePunch;

    // Variables that track whether a weapon can be fired
    bool hasAmmo;
    bool doesNotExceedFireRate;
    bool canThrow;
    bool notChargePunching;
    bool isNotWallHanging;
    bool isNotMeleeing;

    public override void Start()
    {
        base.Start();
        EventSystem.current.onWeaponAddAmmoTrigger += AddAmmo;
        EventSystem.current.onPlayerShotInformation += WeaponFired;

        player = GameObject.Find("Player");
        playerController = player.GetComponentInChildren <PlayerController>();
        playerSecondaryWeapon = player.GetComponentInChildren<PlayerSecondaryWeapon>();
        chargePunch = player.GetComponentInChildren<ChargePunch>();
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
            EventSystem.current.UpdateSecondaryWeaponTrigger(weaponID, weaponName, weaponLevel);
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
        { if (weaponList[i].id == weaponID) { AddIfBelowAmmoLimit(ammoChange, i); } }
    } // used when needing to add to a weapon with a specific ID e.g. non-current weapon

    public void AddAmmo(string weaponName, int ammoChange)
    {
        for (int i = 0; i < weaponList.Count; i++)
        { if (weaponList[i].name == weaponName) { AddIfBelowAmmoLimit(ammoChange, i); } }
    }

    void AddIfBelowAmmoLimit(int ammoChange, int weaponIndex)
    {
        if (weaponList[weaponIndex].ammoLimit <= weaponList[weaponIndex].ammo + ammoChange) { weaponList[weaponIndex].ammo = weaponList[weaponIndex].ammoLimit; }
        else { weaponList[weaponIndex].ammo += ammoChange; }
    }

    public void WeaponFired(int weaponID, int weaponLevel, int ammoChange, int direction)
    {
        weaponList[currentWeaponIndex].ammo += ammoChange;
        WeaponUIUpdate();
    }

    public void CanWeaponBeFired() // used as a check before firing a weapon and decrementing inventory
    {
        hasAmmo = weaponList[currentWeaponIndex].ammo > 0;
        doesNotExceedFireRate = Time.time > lastWeaponUseTime + weaponList[currentWeaponIndex].fireRate;
        canThrow = !playerSecondaryWeapon.throwHandler.InActiveThrow; 
        notChargePunching = !chargePunch.IsCharging;
        isNotWallHanging = !playerController.IsWallHanging;
        isNotMeleeing = !playerController.IsAttacking;

        if (hasAmmo && doesNotExceedFireRate && canThrow && notChargePunching && isNotWallHanging && isNotMeleeing)
        {
            lastWeaponUseTime = Time.time;

            // this info will be cached in weaponAnimator, and it will call fire using this info when the weapon has reached the physical firing point in an animation
            EventSystem.current.CacheShotInformation(
            currentWeaponID,
            weaponList[currentWeaponIndex].level,
            -1,
            weaponList[currentWeaponIndex].ammo);

            if (currentWeapon.isFixedDistance) // used by flamethrowers and other 'continous fire' weapons
            {
                animator.PlayFunction("PlayerShootContinuous", PlayerAnimator.PlayerPart.RightArm);
                animator.PlayFunction("PlayerShootContinuous", PlayerAnimator.PlayerPart.LeftArm);
                animator.PlayFunction("PlayerShootContinuous", PlayerAnimator.PlayerPart.Weapon); 
            }
            else if(currentWeapon.isShot) // discrete shot weapons
            {
                animator.PlayFunction("PlayerShoot", PlayerAnimator.PlayerPart.RightArm);
                animator.PlayFunction("PlayerShoot", PlayerAnimator.PlayerPart.LeftArm);
                animator.PlayFunction("PlayerShoot", PlayerAnimator.PlayerPart.Weapon); // this will make the actual call of when to release ammo
            }
        }
        else if (!hasAmmo) { EventSystem.current.WeaponStopTrigger(); }
    }
    private new void OnDestroy()
    {
        base.OnDestroy();
        EventSystem.current.onWeaponAddAmmoTrigger -= AddAmmo;
        EventSystem.current.onPlayerShotInformation -= WeaponFired;
    }

}
