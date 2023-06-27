using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ComponentFinder;
using UnityEngine.InputSystem;

public class PlayerSecondaryWeapon : MonoBehaviour
{
    // EXTERNAL REFERENCES
    private GameController gameController;
    private SecondaryWeaponsManager secondaryWeaponsManager;
    private GameObject utilities;
    private PlayerAnimator animator;
    [SerializeField] private SpriteRenderer weaponSprite;
    private float weaponWidth;
    private float weaponHeight;

    [Header("Ammo Settings")]
    public List<GameObject> ammoPrefabs;
    public Transform projectileSpawnPoint; // assigned in inspector
    private float projectileSpawnOffset = 0f; // distance from weaponSprite and projectile generation
    private float projectileXOffset;
    private float projectileYOffset;
    public int ammoSpeed;
    private int ammoGravity = 0;

    [Header("Weapon Settings")]
    private WeaponDatabase weaponDatabase;
    [SerializeField] private GameObject fixedDistanceAmmo;
    private bool fixedDistanceCheck;
    [SerializeField] private int currentWeaponID;
    public Weapons currentWeapon;
    [SerializeField] private int currentWeaponLevel;
    public int currentAmmoIndex;
    [SerializeField] private string currentWeaponSpriteLocation;

    [Header("Throw Weapon Settings")]
    public PlayerSecondaryWeaponThrowHandler throwHandler;

    [Header("Weapon Rotation Settings")]
    [SerializeField] private float weaponDirection;
    private GameObject player;
    private PlayerController playerController;
    private Vector3 lookDirection;
    private Vector3 playerPos;
    float bufferZone, bufferZoneMax, bufferZoneMin;

    private float rotationZ; //used for weapon direction

    void Start()
    {
        // subscribe to events
        EventSystem.current.onUpdateSecondaryWeaponTrigger += WeaponChanged;
        EventSystem.current.onWeaponFire += WeaponFired;
        EventSystem.current.onWeaponStopTrigger += WeaponStop;

        // get object and component references
        gameController = FindObjectOfType<GameController>();

        player = GameObject.Find("Player");
        animator = GetComponentInChildrenByNameAndType<PlayerAnimator>("Animator", player, true);
        var parentOfWeaponSprite = GetComponentInChildrenByNameAndType<Transform>("Weapon", animator.gameObject);
        weaponSprite = GetComponentInChildrenByNameAndType<SpriteRenderer>("SpriteAndAnimations", parentOfWeaponSprite.gameObject);
        secondaryWeaponsManager = player.GetComponent<SecondaryWeaponsManager>();
        playerController = transform.parent.GetComponent<PlayerController>();
        fixedDistanceAmmo = GetComponentInChildrenByNameAndType<Transform>("FixedDistanceAmmo", animator.gameObject).gameObject;
        throwHandler = GetComponent<PlayerSecondaryWeaponThrowHandler>();

        utilities = GameObject.Find("Utilities");
        weaponDatabase = utilities.GetComponentInChildren<WeaponDatabase>();


        // load ammo prefabs to a list
        ammoPrefabs = Resources.LoadAll<GameObject>("AmmoPrefabs").ToList();

        // go through the list and sort them in order by ammo IDs
        ammoPrefabs.Sort((randomAmmo, ammoToCompareTo) => randomAmmo.GetComponent<Ammo>().GetAmmoID().CompareTo(ammoToCompareTo.GetComponent<Ammo>().GetAmmoID()));
    
        StopFixedFire();
    }

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
        if (weaponDatabase.data.entries[weaponID].isShot == true) { Shoot();}
        else if (weaponDatabase.data.entries[weaponID].isFixedDistance == true) { FixedDistanceFire(); fixedDistanceCheck = true; }
        else { Debug.Log("Check WeaponDatabase, weapon is missing a TRUE value for if ammo should be shot, thrown, be a fixed distance, etc."); }
    }

    private void WeaponStop() { StopFixedFire(); }

    private void Shoot()
    {
        GameObject shot = Instantiate(ammoPrefabs[currentAmmoIndex], projectileSpawnPoint.position, projectileSpawnPoint.transform.rotation);
        FindObjectOfType<AudioManager>().PlaySFX("WeaponFire");

        shot.GetComponent<Rigidbody2D>().gravityScale = ammoGravity;

        shot.GetComponent<Rigidbody2D>().AddForce(projectileSpawnPoint.transform.right * ammoSpeed);
    }

    public void HandleThrowing(string inputState, string currentControlScheme)
    {
        throwHandler.HandleThrowing(inputState, currentControlScheme);
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
        for(int i = 0; i < ammoPrefabs.Count; i++)
        {
            if (ammoPrefabs[i].GetComponent<Ammo>().weaponID == weaponID &&
                ammoPrefabs[i].GetComponent<Ammo>().weaponLevel == weaponLevel) 
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
        EventSystem.current.onWeaponStopTrigger -= WeaponStop;
    }
}