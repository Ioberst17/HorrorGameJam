using System;
using UnityEngine;
//using static UnityEditor.Progress;

public class EventSystem : MonoBehaviour
{
    public static EventSystem current; // Singleton structure

    private void Awake() { current = this; }

    // ANIMATION EVENTS, RELATED TO VARIOUS PLAYER ACTIONS

    // base animator

    public event Action onChargePunchRelease;

    public void ChargePunchTrigger() { if (onChargePunchRelease != null) { onChargePunchRelease(); } }    
    
    public event Action onGroundSlamDrop;

    public void GroundSlamDropTrigger() { if (onGroundSlamDrop != null) { onGroundSlamDrop(); } }    
    
    public event Action onGroundSlamFinish;    
    public void GroundSlamFinished() { if (onGroundSlamFinish != null) { onGroundSlamFinish(); } }    
    
    public event Action onThrowWeaponRelease;

    public void ThrowWeaponTrigger() { if (onThrowWeaponRelease != null) { onThrowWeaponRelease(); } }    
    
    public event Action inActiveMeleeAttack;

    public void ActiveMeleeTrigger() { if (inActiveMeleeAttack != null) { inActiveMeleeAttack(); } }    
    public event Action endActiveMeleeAttack;

    public void EndActiveMeleeTrigger() { if (endActiveMeleeAttack != null) { endActiveMeleeAttack(); } }

    public event Action<int> playerCombo;
    public void PlayerComboTrigger(int comboNumber) { if (playerCombo != null) { playerCombo(comboNumber); } }

    // right arm
    public event Action OnShotFired;

    public void OnFireAnimationRelease() { if (OnShotFired != null) { OnShotFired(); } }

    // ANIMATIONS RELATED TO ENEMIES

    public event Action<int> enemyInActiveMeleeAttack;

    public void EnemyActiveMeleeTrigger(int instanceID) { if (enemyInActiveMeleeAttack != null) { enemyInActiveMeleeAttack(instanceID); } }
    public event Action<int> enemyEndActiveMeleeAttack;

    public void EnemyEndActiveMeleeTrigger(int instanceID) { if (enemyEndActiveMeleeAttack != null) { enemyEndActiveMeleeAttack(instanceID); } }

    // Other ANIMATION

    public event Action<int> projectileColliderOn;
    public void ProjectileColliderOnTrigger(int instanceID) { if(projectileColliderOn != null) { projectileColliderOn(instanceID); } }    
    
    public event Action<int> projectileColliderOff;
    public void ProjectileColliderOffTrigger(int instanceID) { if(projectileColliderOff != null) { projectileColliderOff(instanceID); } }

    public event Action<int> projectileLaunch;
    public void ProjectileLaunchTrigger(int instanceID) { if (projectileLaunch != null) { projectileLaunch(instanceID); } }

    public event Action<int> targetedSpellTrigger;
    public void TargetedSpellTrigger(int instanceID) { if(targetedSpellTrigger != null) { targetedSpellTrigger(instanceID); } }

    public event Action<int> objectIsShooting;
    public void ObjectIsShootingTrigger(int instanceID) { if (objectIsShooting != null) { objectIsShooting(instanceID); } }

    public event Action<int> objectIsNotShooting;
    public void ObjectIsNotShootingTrigger(int instanceID) { if (objectIsNotShooting != null) { objectIsNotShooting(instanceID); } }




    // COMBAT-CALCULATIONS (WITH ENEMIES)

    public event Action<int, Vector3, string, EnemyController> onEnemyEnviroDamage;
    public void EnemyEnviroDamage(int damage, Vector3 position, string statusModifier, EnemyController enemyController)
    { { if (onEnemyEnviroDamage != null) { onEnemyEnviroDamage(damage, position, statusModifier, enemyController); } } }

    public event Action<int, Vector3, string, EnemyController> onEnemyAmmoHitCollision;
    public void EnemyAmmoHitTrigger(int baseDamage, Vector3 position, string statusModifier, EnemyController enemyController)
    { { if (onEnemyAmmoHitCollision != null) { onEnemyAmmoHitCollision(baseDamage, position, statusModifier, enemyController); } } }

    public event Action<int, Vector3, string, EnemyController> onEnemyMeleeHitCollision;
    public void EnemyMeleeHitTrigger(int attackDamage, Vector3 playerPosition, string statusModifier, EnemyController enemyController)
    { { if (onEnemyMeleeHitCollision != null) { onEnemyMeleeHitCollision(attackDamage, playerPosition, statusModifier, enemyController); } } }    
    
    public event Action<int, Vector3, string, EnemyController> onEnemyParryCollision;
    public void EnemyParryHitTrigger(int attackDamage, Vector3 playerPosition, string statusModifier, EnemyController enemyController)
    { { if (onEnemyParryCollision != null) { onEnemyParryCollision(attackDamage, playerPosition, statusModifier, enemyController); } } }
    
    public event Action<int, int> onWaveFinished;
    public void WaveFinishedTrigger(int areaID, int waveNum) { if (onWaveFinished != null) { onWaveFinished(areaID, waveNum); } }

    public event Action <int> onAllWavesFinished;
    public void AllWavesFinishedTrigger(int areaID) { if (onAllWavesFinished != null) { onAllWavesFinished(areaID); } }





    // WEAPON-RELATED EVENTS

    public event Action<int> onWeaponAddAmmoTrigger; // used to add ammo
    public void WeaponAddAmmoTrigger(int ammo) { if (onWeaponAddAmmoTrigger != null) { onWeaponAddAmmoTrigger(ammo); } }

    public event Action <int, int, int, int> onPlayerShotInformation; // used for calling an animation
    public void CacheShotInformation(int weaponID, int weaponLevel, int ammoChange, int currentAmmoLevel)
    {
        if (onPlayerShotInformation != null)
        {
            onPlayerShotInformation(weaponID, weaponLevel, ammoChange, currentAmmoLevel);
        }
    }

    public event Action<int, int, int, int> onWeaponFire; // used for weapon firing
    public void ReleaseAmmo (int weaponID, int weaponLevel, int ammoChange, int currentAmmoLevel)
    {
        if (onWeaponFire != null)
        {
            onWeaponFire(weaponID, weaponLevel, ammoChange, currentAmmoLevel);
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

    public event Action<Vector3, int, string, float, float, bool> onPlayerHitCalcTrigger;

    public void PlayerHitHealthTrigger(Vector3 attackerPosition, int damageNumber, string damageType, float damageMod, float knockbackMod, bool hitInActiveShieldZone) 
    { 
        if(onPlayerHitCalcTrigger != null) 
        { 
            onPlayerHitCalcTrigger(attackerPosition, damageNumber, damageType, damageMod, knockbackMod, hitInActiveShieldZone); 
        } 
    }    
    
    public event Action<Vector3, float, bool> onPlayerHitPostHealthTrigger;

    public void PlayerHitPostHealthTrigger(Vector3 attackerPosition, float knockbackMod, bool hitInActiveShieldZone) 
    { 
        if(onPlayerHitPostHealthTrigger != null) 
        {
            onPlayerHitPostHealthTrigger(attackerPosition, knockbackMod, hitInActiveShieldZone); 
        } 
    }

    public event Action onPlayerDeathTrigger;
    public void PlayerDeathTrigger() { if(onPlayerDeathTrigger != null) { onPlayerDeathTrigger(); } }


    public event Action<int> onAddHealthTrigger;

    public void AddHealthTrigger(int healthToAdd) { if (onAddHealthTrigger != null) { onAddHealthTrigger(healthToAdd); } }





    // QUEST RELATED

    public event Action<string, int> onQuestUpdateTrigger;

    public void UpdateQuestTrigger(string questToUpdate, int subQuestIndex) { if (onQuestUpdateTrigger != null) { onQuestUpdateTrigger(questToUpdate, subQuestIndex); } }





    // OTHER PLAYER-RELATED

    public event Action<PickupableItem> onItemPickupTrigger;

    public void ItemPickupTrigger(PickupableItem item) { if(onItemPickupTrigger != null) { onItemPickupTrigger(item); } }

    
    public event Action<PlayerSkills.SkillType> onSkillUnlock;

    public void SkillUnlockTrigger(PlayerSkills.SkillType skill) {if(onSkillUnlock != null) { onSkillUnlock(skill); } }

    public event Action <DataManager.GameData> onGameFileLoaded;

    public void GameFileLoadedTrigger(DataManager.GameData dataToSend) { if(onGameFileLoaded != null) { onGameFileLoaded(dataToSend); } }

    public event Action lockPlayerXMovement;
    public void LockPlayerXMovementTrigger() { if (lockPlayerXMovement != null) { lockPlayerXMovement(); } }
    
    public event Action unlockPlayerXMovement;
    public void UnlockPlayerXMovementTrigger() { if (unlockPlayerXMovement != null) { unlockPlayerXMovement(); } }

}
