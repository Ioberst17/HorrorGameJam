using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSecondaryWeapon : MonoBehaviour
{
    // EXTERNAL REFERENCES
    private GameController gameController;
    private SecondaryWeaponsManager secondaryWeaponsManager;
    private GameObject utilities;

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
    [SerializeField] private Weapons currentWeapon;
    [SerializeField] private int currentWeaponLevel;
    [SerializeField] private int currentAmmoIndex;
    [SerializeField] private string currentWeaponSpriteLocation;

    [Header("Throw Weapon Settings")]
    [SerializeField] private float maxForceHoldDownTime = 2f;
    [SerializeField] private float maxThrowBand = 2f;
    [SerializeField] private static int maxThrowSpeed = 10;
    private static int minThrowSpeed = 5;
    public int throwSpeed;
    private int throwGravity = 1;
    private float throwKeyHeldDownStart;
    private float throwKeyReleased;
    [SerializeField] private float throwKeyHeldDownTime;
    [SerializeField] private float holdTimeNormalized;
    [SerializeField] private float throwDistanceNormalized;
    public bool inActiveThrow = false;
    private float currentThrowForce = 0;
    private int currentThrowDirection = 0;


    [Header("Weapon Rotation Settings")]
    private GameObject player;
    private PlayerController playerController;
    private Vector3 lookDirection;
    private Vector3 playerPos;
    private Vector3 throwMouseStartingPos;
    private Vector3 throwMouseFinishingPos;
    float bufferZone, bufferZoneMax, bufferZoneMin;
    private float throwDistanceToPass;
    [SerializeField] private float weaponDirection;
    private float rotationZ; //used for weapon direction

    //particle systems
    private ParticleSystem flamethrower;
    private ParticleSystem gunfire;

    void Start()
    {
        // subscribe to events
        EventSystem.current.onUpdateSecondaryWeaponTrigger += WeaponChanged;
        EventSystem.current.onWeaponFireTrigger += WeaponFired;
        EventSystem.current.onWeaponStopTrigger += WeaponStop;

        // get object and component references
        gameController = FindObjectOfType<GameController>();

        player = GameObject.Find("Player");
        secondaryWeaponsManager = player.GetComponent<SecondaryWeaponsManager>();
        playerController = transform.parent.GetComponent<PlayerController>();
        weaponSprite = GameObject.Find("WeaponSprite").GetComponent<SpriteRenderer>();
        fixedDistanceAmmo = gameObject.transform.GetChild(0).gameObject.transform.Find("FixedDistanceAmmo").gameObject;

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

    public void HandleWeaponDirection(string activeControlScheme)
    {
        if(activeControlScheme == "Gamepad")
        {
            weaponDirection = Mathf.Atan2(gameController.lookInput.y, gameController.lookInput.x) * Mathf.Rad2Deg;
            if (gameController.XInput > 0.2) { transform.rotation = Quaternion.Euler(0f, 0f, 0f); } // if moving right more than slightly, point right
            else if (gameController.XInput < -0.2) { transform.rotation = Quaternion.Euler(0f, 0f, 180f); } // if moving left more than slightly, point left
            else if (gameController.XInput < 0.2 && gameController.XInput > -0.2) // if basically still
            {
                if (gameController.lookInput.x > 0 || gameController.lookInput.y > 0) { transform.rotation = Quaternion.Euler(0f, 0f, weaponDirection); } // if input, point in the direction of the input
                else if (playerController.FacingDirection == 1) { transform.rotation = Quaternion.Euler(0f, 0f, 0f); } // if still facing right, point right
                else if (playerController.FacingDirection == -1) { transform.rotation = Quaternion.Euler(0f, 0f, 180f); } // if still facing left, point left
            }
        }
        else if(activeControlScheme == "Keyboard and Mouse") 
        {
            Vector2 rotation = (Vector2)gameController.lookInput - (Vector2)gameController.playerPositionScreen;
            rotationZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }
    }


    private void WeaponFired(int weaponID, int weaponLevel, int ammoChange, int direction)
    {
        if (weaponDatabase.data.entries[weaponID].isShot == true) { Shoot();}
        else if (weaponDatabase.data.entries[weaponID].isThrown == true) { Throw(direction); }
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

    private void Throw(int direction) { inActiveThrow = true; secondaryWeaponsManager.changingIsBlocked = true; currentThrowDirection = direction; }

    public void HandleThrowing(string inputState, string currentControlScheme)
    {
        if (inputState == "Button Clicked")
        {
            if (currentControlScheme == "Keyboard&Mouse") { throwMouseStartingPos = gameController.playerPositionScreen; }
            if (currentControlScheme == "Gamepad") { throwMouseStartingPos = gameController.playerPositionScreen; } //throwMouseStartingPos = Input.mousePosition; //throwKeyHeldDownStart = Time.time;
        }
        else if (inputState == "Button Released")
        {
            throwKeyReleased = Time.time;
            if (inActiveThrow) { ThrowWeapon(currentThrowDirection, currentThrowForce); }
        }
        if (inputState == "Button Held")
        {
            if (inActiveThrow)
            {
                if (currentControlScheme == "Keyboard&Mouse") { throwMouseFinishingPos = gameController.lookInput; }
                throwDistanceToPass = Vector2.Distance(throwMouseFinishingPos, throwMouseStartingPos);
                currentThrowForce = CalcThrowForce(throwDistanceToPass);
            }
        }
    }

    private float CalcThrowForce(float holdForce)
    {
        throwDistanceNormalized = Mathf.Clamp01(holdForce / maxThrowBand);
        float force = throwDistanceNormalized * maxThrowSpeed;
        force += minThrowSpeed; 
        EventSystem.current.StartChargedAttackTrigger(throwDistanceNormalized, projectileSpawnPoint.transform, force);
        return force;
    }

    void ThrowWeapon(int direction, float throwSpeed)
    {
        GameObject toss = Instantiate(ammoPrefabs[currentAmmoIndex], projectileSpawnPoint.position, projectileSpawnPoint.transform.rotation);
        FindObjectOfType<AudioManager>().PlaySFX("WeaponToss");

        toss.GetComponent<Rigidbody2D>().gravityScale = throwGravity;

        if( 10 > transform.rotation.eulerAngles.z && transform.rotation.eulerAngles.z > -10) 
        {
            toss.GetComponent<Rigidbody2D>().AddForce(projectileSpawnPoint.transform.right.normalized * throwSpeed, ForceMode2D.Impulse);
        }
        else {toss.GetComponent<Rigidbody2D>().AddForce((projectileSpawnPoint.transform.right).normalized * throwSpeed, ForceMode2D.Impulse); }

        inActiveThrow = false;
        secondaryWeaponsManager.changingIsBlocked = false;
        EventSystem.current.FinishChargedAttackTrigger();
    }

    private void FixedDistanceFire() { fixedDistanceAmmo.SetActive(true); ToggleFlamethrowerEffects(true); }
    private void StopFixedFire() { fixedDistanceAmmo.SetActive(false); ToggleFlamethrowerEffects(false); }

    private void ToggleFlamethrowerEffects(bool flamethrowerState) { FindObjectOfType<AudioManager>().LoopSFX("Flamethrower", flamethrowerState); }

    public void Flip() // used in game controller to flip projectile spawnpoint when player changes direction; should only be called if using horizontal / vertical direction only firing
    {
            gameObject.transform.localScale = new Vector3(
                transform.localScale.x,
                transform.localScale.y * -1,
                transform.localScale.z);
    }

    private void WeaponChanged(int weaponID, string weaponName, int weaponLevel)
    {
        currentWeapon = weaponDatabase.ReturnItemFromID(weaponID);
        UpdateAmmoUsed(weaponID, weaponLevel);
        UpdateWeaponSpriteAndFirePoint(weaponName);
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

    void UpdateWeaponSpriteAndFirePoint(string weaponName) { UpdateSprite(weaponName); }

    void UpdateSprite(string weaponName) 
    {
        currentWeaponSpriteLocation = "Sprites/Weapons/" + weaponName;
        weaponSprite.sprite = Resources.Load<Sprite>(currentWeaponSpriteLocation);
        if(weaponSprite.sprite != null) { UpdateFirePoint(); }
        else { Debug.LogFormat("Failed to load a weapon sprite from Resources. Check if the file path ({0}) and currentWeaponName used ({1}) are correct", currentWeaponSpriteLocation, weaponName); }
    }

    void UpdateFirePoint()
    {
        // Get the position of the pivot point of the sprite in world space
        Vector3 pivotPointPos = weaponSprite.transform.TransformPoint(weaponSprite.sprite.pivot / weaponSprite.sprite.pixelsPerUnit);

        // Set the z-coordinate to 0, so the spawn point is positioned on the same plane as the player
        Vector3 firePointPos = new Vector3(pivotPointPos.x, pivotPointPos.y, 0f);

        // Set the position of the projectileSpawnPoint
        projectileSpawnPoint.transform.position = firePointPos;
    }


    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onUpdateSecondaryWeaponTrigger -= WeaponChanged;
        EventSystem.current.onWeaponFireTrigger -= WeaponFired;
        EventSystem.current.onWeaponStopTrigger -= WeaponStop;
    }
}