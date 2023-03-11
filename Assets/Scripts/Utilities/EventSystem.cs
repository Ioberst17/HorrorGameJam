using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using static UnityEditor.Progress;

public class EventSystem : MonoBehaviour
{
    public static EventSystem current; // Singleton structure

    private void Awake() { current = this; }

    // COMBAT-CALCULATIONS (WITH ENEMIES)

    public event Action<int, Vector3, string> onEnemyEnviroDamage;
    public void EnemyEnviroDamage(int damage, Vector3 position, string statusModifier)
    { { if (onEnemyEnviroDamage != null) { onEnemyEnviroDamage(damage, position, statusModifier); } } }

    public event Action<int, int, Vector3, string> onEnemyHitCollision;
    public void AttackHitTrigger(int weaponID, int weaponLevel, Vector3 position, string statusModifier)
    { { if (onEnemyHitCollision != null) { onEnemyHitCollision(weaponID, weaponLevel, position, statusModifier); } } }

    // WEAPON-RELATED EVENTS

    public event Action<int> onWeaponAddAmmoTrigger; // used to add ammo
    public void WeaponAddAmmoTrigger(int ammo) { if (onWeaponAddAmmoTrigger != null) { onWeaponAddAmmoTrigger(ammo); } }


    public event Action <int> onAmmoCheckTrigger; // used to check for ammo BEFORE  weapon AmmoTrigger is called to fire

    public void AmmoCheckTrigger(int fireDirection) { if (onAmmoCheckTrigger != null) { onAmmoCheckTrigger(fireDirection); } }


    public event Action <int, int, int, int> onWeaponFireTrigger; // used for weapon firing
    public void WeaponFireTrigger(int weaponID, int weaponLevel, int ammoChange, int currentAmmoLevel)
    {
        if (onWeaponFireTrigger != null)
        {
            onWeaponFireTrigger(weaponID, weaponLevel, ammoChange, currentAmmoLevel);
        }
    }

    
    public event Action onWeaponStopTrigger;

    public void WeaponStopTrigger() { if(onWeaponStopTrigger != null) { onWeaponStopTrigger(); } }

    public event Action<float, Transform, float> onStartTossingTrigger; // used for UI items displayed on throw
    public void StartTossingWeaponTrigger(float throwForceDisplayed, Transform throwPoint, float throwForce)
    {
        if (onStartTossingTrigger != null)
        {
            onStartTossingTrigger(throwForceDisplayed, throwPoint, throwForce);
        }
    }

    public event Action onFinishTossingTrigger; // used when toss weapon has started launch (after force calculated)

    public void FinishTossingWeaponTrigger() { if (onFinishTossingTrigger != null) {onFinishTossingTrigger(); } }

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

    public event Action<string, int> onUpdatePrimaryWeaponUITrigger;

    public void UpdatePrimaryWeaponUITrigger(string currentWeapon, int updatedAmmo)
    {
        if (onUpdatePrimaryWeaponUITrigger != null)
        {
            onUpdatePrimaryWeaponUITrigger(currentWeapon, updatedAmmo);
        }
    }

    public event Action<int, int> onUpdatePrimaryWeaponTrigger;

    public void UpdatePrimaryWeaponTrigger(int weaponID, int weaponLevel) { if (onUpdatePrimaryWeaponTrigger != null) { onUpdatePrimaryWeaponTrigger(weaponID, weaponLevel); } }


    public event Action<string, int> onUpdateSecondaryWeaponUITrigger;

    public void UpdateSecondaryWeaponUITrigger(string currentWeapon, int updatedAmmo)
    {
        if (onUpdateSecondaryWeaponUITrigger != null)
        {
            onUpdateSecondaryWeaponUITrigger(currentWeapon, updatedAmmo);
        }
    }

    public event Action<int, int> onUpdateSecondaryWeaponTrigger;

    public void UpdateSecondaryWeaponTrigger(int weaponID, int weaponLevel) { if(onUpdateSecondaryWeaponTrigger != null) { onUpdateSecondaryWeaponTrigger(weaponID, weaponLevel); } }

    // PLAYER HEALTH-RELATED

    public event Action<Collider2D> onPlayerShieldHitTrigger;

    public void PlayerShieldHitTrigger(Collider2D attacker) { if(onPlayerShieldHitTrigger != null) { onPlayerShieldHitTrigger(attacker); } }

    public event Action<Vector3, int, int, float, float> onPlayerHitCalcTrigger;

    public void PlayerHitCalcTrigger(Vector3 enemyPos, int damageNumber, int damageType, float damageMod, float knockbackMod) 
    { 
        if(onPlayerHitCalcTrigger != null) 
        { 
            onPlayerHitCalcTrigger(enemyPos, damageNumber, damageType, damageMod, knockbackMod); 
        } 
    }

    public event Action onPlayerDeathTrigger;
    public void PlayerDeathTrigger() { if(onPlayerDeathTrigger != null) { onPlayerDeathTrigger(); } }


    public event Action<int> onAddHealthTrigger;

    public void AddHealthTrigger(int healthToAdd) { if (onAddHealthTrigger != null) { onAddHealthTrigger(healthToAdd); } }

    // OTHER PLAYER-RELATED

    public event Action<int, int> onItemPickupTrigger;

    public void ItemPickupTrigger(int itemID, int amount) { if(onItemPickupTrigger != null) { onItemPickupTrigger(itemID, amount); } }

    
    public event Action<PlayerSkills.SkillType> onSkillUnlock;

    public void SkillUnlockTrigger(PlayerSkills.SkillType skill) {if(onSkillUnlock != null) { onSkillUnlock(skill); } }

}
