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

    public event Action<int, Vector3, string, EnemyController> onEnemyEnviroDamage;
    public void EnemyEnviroDamage(int damage, Vector3 position, string statusModifier, EnemyController enemyController)
    { { if (onEnemyEnviroDamage != null) { onEnemyEnviroDamage(damage, position, statusModifier, enemyController); } } }

    public event Action<int, int, Vector3, string, EnemyController> onEnemyHitCollision;
    public void EnemyHitTrigger(int weaponID, int weaponLevel, Vector3 position, string statusModifier, EnemyController enemyController)
    { { if (onEnemyHitCollision != null) { onEnemyHitCollision(weaponID, weaponLevel, position, statusModifier, enemyController); } } }

    public event Action<int, int> onWaveFinished;

    public void WaveFinishedTrigger(int areaID, int waveNum) { if (onWaveFinished != null) { onWaveFinished(areaID, waveNum); } }

    public event Action <int> onAllWavesFinished;
    public void AllWavesFinishedTrigger(int areaID) { if (onAllWavesFinished != null) { onAllWavesFinished(areaID); } }

    // WEAPON-RELATED EVENTS

    public event Action<int> onWeaponAddAmmoTrigger; // used to add ammo
    public void WeaponAddAmmoTrigger(int ammo) { if (onWeaponAddAmmoTrigger != null) { onWeaponAddAmmoTrigger(ammo); } }


    public event Action onAmmoCheckTrigger; // used to check for ammo BEFORE  weapon AmmoTrigger is called to fire

    public void AmmoCheckTrigger() { if (onAmmoCheckTrigger != null) { onAmmoCheckTrigger(); } }


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

    public event Action<float, Transform, float?> onStartChargingUITrigger; // used for UI items displayed on throw

    public void StartChargedAttackTrigger(float throwForceDisplayed, Transform throwPoint, float? throwForce)
    {
        if (onStartChargingUITrigger != null)
        {
            onStartChargingUITrigger(throwForceDisplayed, throwPoint, throwForce);
        }
    }

    public event Action onFinshChargingUITrigger; // used when toss weapon has started launch (after force calculated)

    public void FinishChargedAttackTrigger() { if (onFinshChargingUITrigger != null) {onFinshChargingUITrigger(); } }

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

    public event Action<int, string, int> onUpdateSecondaryWeaponTrigger;

    public void UpdateSecondaryWeaponTrigger(int weaponID, string weaponName, int weaponLevel) { if(onUpdateSecondaryWeaponTrigger != null) { onUpdateSecondaryWeaponTrigger(weaponID, weaponName, weaponLevel); } }

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

    public event Action<PickupableItem> onItemPickupTrigger;

    public void ItemPickupTrigger(PickupableItem item) { if(onItemPickupTrigger != null) { onItemPickupTrigger(item); } }

    
    public event Action<PlayerSkills.SkillType> onSkillUnlock;

    public void SkillUnlockTrigger(PlayerSkills.SkillType skill) {if(onSkillUnlock != null) { onSkillUnlock(skill); } }

    public event Action <DataManager.GameData> onGameFileLoaded;

    public void GameFileLoadedTrigger(DataManager.GameData dataToSend) { if(onGameFileLoaded != null) { onGameFileLoaded(dataToSend); } }

}
