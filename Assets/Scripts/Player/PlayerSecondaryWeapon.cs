using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ComponentFinder;
using UnityEngine.InputSystem;

public class PlayerSecondaryWeapon : ProjectileManager
{
    // EXTERNAL REFERENCES
    GameController gameController;
    PlayerAnimator animator;

    [Header("Weapon Settings")]
    WeaponDatabase weaponDatabase;
    PlayerProjectileDatabase playerProjectileDatabase;
    [SerializeField] GameObject fixedDistanceAmmo;

    [SerializeField] int currentWeaponID;
    public Weapons currentWeapon;
    [SerializeField] int currentWeaponLevel;
    [SerializeField] private string currentWeaponSpriteLocation;

    [Header("Throw Weapon Settings")]
    public PlayerThrowHandler throwHandler;

    [Header("Weapon Rotation Settings")]
    [SerializeField] float weaponDirection;
    Vector3 lookDirection;
    Vector3 playerPos;
    float bufferZone, bufferZoneMax, bufferZoneMin;

    override protected void Start()
    {
        base.Start();
        // get object and component references
        gameController = FindObjectOfType<GameController>();
        playerProjectileDatabase = FindObjectOfType<PlayerProjectileDatabase>();
        BuildProjectileInfoDictionaries();
        animator = FindObjectOfType<PlayerAnimator>();
        var parentOfWeaponSprite = GetComponentInChildrenByNameAndType<Transform>("Weapon", animator.gameObject);
        weaponSprite = GetComponentInChildrenByNameAndType<SpriteRenderer>("SpriteAndAnimations", parentOfWeaponSprite.gameObject);
        projectileSpawnPoint = GetComponentInChildrenByNameAndType<Transform>("FirePointSprite");
        fixedDistanceAmmo = GetComponentInChildrenByNameAndType<Transform>("FixedDistanceAmmo", animator.gameObject).gameObject;
        throwHandler = GetComponent<PlayerThrowHandler>();
        weaponDatabase = FindObjectOfType<WeaponDatabase>();
        
        // ensure fixed fire is off
        StopFixedFire();

        // subscribe to events
        EventSystem.current.onUpdateSecondaryWeaponTrigger += WeaponChanged;
        EventSystem.current.onWeaponFire += WeaponFired;
        EventSystem.current.onWeaponStopTrigger += StopFixedFire;
    }
    // Called in start of child function
    protected override void LoadProjectileObjects()
    {
        // load ammo prefabs to a list
        projectilesToUse = Resources.LoadAll<GameObject>("AmmoPrefabs").ToList();

        // go through the list and sort them in order by ammo IDs
        projectilesToUse.Sort((randomAmmo, ammoToCompareTo) => randomAmmo.GetComponent<Ammo>().GetAmmoID().CompareTo(ammoToCompareTo.GetComponent<Ammo>().GetAmmoID()));
    }

    protected override void BuildProjectileInfoDictionaries()
    {
        foreach (var projectile in playerProjectileDatabase.data.entries)
        {
            projectiles.Add(projectile); 
        }

        base.BuildProjectileInfoDictionaries();
    }

    /// <summary>
    /// Used to determine weapon point direction
    /// </summary>
    /// <returns></returns>
    public bool WeaponIsPointedToTheRight()
    {
        bool weaponIsPointedRight = false;
        lookDirection = gameController.lookInput;
        playerPos = gameController.playerPositionScreen;

        bufferZone = 0.5f; // a buffer used to prevent constant flipping if the character's mouse is next to the player
        bufferZoneMin = playerPos.x - bufferZone;
        bufferZoneMax = playerPos.x + bufferZone;

        if (lookDirection.x > playerPos.x && lookDirection.x > bufferZoneMax) { weaponIsPointedRight = true; }
        else if (lookDirection.x < playerPos.x && lookDirection.x < bufferZoneMin) { weaponIsPointedRight = false; }

        return weaponIsPointedRight;
    }

    private void WeaponFired(int weaponID, int weaponLevel, int ammoChange, int direction)
    {
        // Note: throw logic by contrast is almost exclusively handled in PlayerSecondaryWeaponThrowHandler
        if (weaponDatabase.data.entries[weaponID].isShot == true) { Shoot(projectilesToUse[currentAmmoIndex]);}
        else if (weaponDatabase.data.entries[weaponID].isFixedDistance == true) { FixedDistanceFire(); }
        else { Debug.Log("Check WeaponDatabase, weapon is missing a TRUE value for if ammo should be shot, thrown, be a fixed distance, etc."); }
    }
    public void HandleThrowing(string inputState, string currentControlScheme)
    {
        throwHandler.Execute(inputState, currentControlScheme);
    }

    private void FixedDistanceFire() { fixedDistanceAmmo.SetActive(true); ToggleFlamethrowerEffects(true); }
    private void StopFixedFire() 
    { 
        fixedDistanceAmmo.SetActive(false);
        ToggleFlamethrowerEffects(false); 
        animator.PlayFunction("StopContinousFire", PlayerAnimator.PlayerPart.RightArm); 
        animator.PlayFunction("StopContinousFire", PlayerAnimator.PlayerPart.LeftArm); 
        animator.PlayFunction("StopContinousFire", PlayerAnimator.PlayerPart.Weapon); 
    }

    private void ToggleFlamethrowerEffects(bool flamethrowerState) { FindObjectOfType<AudioManager>().LoopSFX("Flamethrower", flamethrowerState); }

    private void WeaponChanged(int weaponID, string weaponName, int weaponLevel)
    {
        currentWeapon = weaponDatabase.ReturnItemFromID(weaponID); 
        UpdateAmmoUsed(weaponID, weaponLevel);
        UpdateFirePoint();
    }

    private void UpdateAmmoUsed(int weaponID, int weaponLevel)
    {
        for(int i = 0; i < projectilesToUse.Count; i++)
        {
            if (projectilesToUse[i].GetComponent<Ammo>().weaponID == weaponID &&
                projectilesToUse[i].GetComponent<Ammo>().weaponLevel == weaponLevel) 
            {
                currentWeaponID = weaponID;
                currentWeaponLevel = weaponLevel;
                currentAmmoIndex = i;
            }
        }
    }

    void UpdateFirePoint()
    {
        // Set the z-coordinate to 0, so the spawn point is positioned on the same plane as the player
        Vector3 firePointPos = new Vector3(currentWeapon.firePointXPosition, currentWeapon.firePointYPosition, 0f);

        // Set the position of the projectileSpawnPoint
        projectileSpawnPoint.transform.localPosition = firePointPos;
    }


    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onUpdateSecondaryWeaponTrigger -= WeaponChanged;
        EventSystem.current.onWeaponFire -= WeaponFired;
        EventSystem.current.onWeaponStopTrigger -= StopFixedFire;
    }
}