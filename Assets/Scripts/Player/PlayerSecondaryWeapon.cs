using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSecondaryWeapon : MonoBehaviour
{
    private SpriteRenderer weaponSprite;

    //ammo-related
    public List<GameObject> ammoPrefabs;
    public Transform projectileSpawnPoint; // assigned in inspector
    public int ammoSpeed = 500;
    private int ammoGravity = 0;

    public WeaponDatabase weaponDatabase;
    [SerializeField] private GameObject fixedDistanceAmmo;
    private bool fixedDistanceCheck;
    [SerializeField]
    private int currentWeaponID;
    [SerializeField]
    private int currentWeaponLevel;
    [SerializeField]
    private int currentAmmoIndex;

    //throwing specific
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


    // weapon rotation
    public Camera cameraToUse; //assign in inspector
    private GameObject player;
    private PlayerController playerController;
    private int upAngle = 90;
    private int downAngle = -90;
    private int standardAngle = 0;
    private Transform firePoint;
    private Vector3 mousePos;
    private Vector3 playerPos;
    private Vector3 throwMouseStartingPos;
    private Vector3 throwMouseFinishingPos;
    private float throwDistanceToPass;
    private float rotationZ; //used for weapon direction

    //particle systems
    private ParticleSystem flamethrower;
    private ParticleSystem gunfire;

    void Start()
    {
        EventSystem.current.onUpdateSecondaryWeaponTrigger += WeaponChanged;
        EventSystem.current.onWeaponFireTrigger += WeaponFired;
        EventSystem.current.onWeaponStopTrigger += WeaponStop;
        player = GameObject.Find("Player");
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        weaponSprite = GameObject.Find("WeaponSprite").GetComponent<SpriteRenderer>();
        firePoint = gameObject.transform.GetChild(0).gameObject.transform.Find("FirePointSprite");
        fixedDistanceAmmo = gameObject.transform.GetChild(0).gameObject.transform.Find("FixedDistanceAmmo").gameObject;
        playerController = transform.parent.GetComponent<PlayerController>();
        cameraToUse = GameObject.Find("Main Camera").GetComponent<Camera>();

        // load ammo prefabs to a list
        ammoPrefabs = Resources.LoadAll<GameObject>("AmmoPrefabs").ToList();

        // go through the list and sort them in order by ammo IDs
        ammoPrefabs.Sort((randomAmmo, ammoToCompareTo) => randomAmmo.GetComponent<Ammo>().GetAmmoID().CompareTo(ammoToCompareTo.GetComponent<Ammo>().GetAmmoID()));
    
        StopFixedFire();
    }

    // Update is called once per frame, used mainly for weapon direction
    void Update()
    {
        HandleWeaponDirection();

        HandleThrowing();
    }

    public bool WeaponIsPointedToTheRight()
    {
        bool weaponIsPointedRight = false;
        mousePos = Input.mousePosition;
        playerPos = cameraToUse.WorldToScreenPoint(transform.position);

        if (mousePos.x > playerPos.x) { weaponIsPointedRight = true; }
        
        return weaponIsPointedRight;
    }

    private void HandleWeaponDirection()
    {
        mousePos = cameraToUse.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        rotationZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }


    private void WeaponFired(int weaponID, int weaponLevel, int ammoChange, int direction)
    {
        if (weaponDatabase.weaponDatabase.entries[weaponID].isShot == true) { Shoot();}
        else if (weaponDatabase.weaponDatabase.entries[weaponID].isThrown == true) { Throw(direction); }
        else if (weaponDatabase.weaponDatabase.entries[weaponID].isFixedDistance == true) { FixedDistanceFire(); fixedDistanceCheck = true; }
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

    private void Throw(int direction)
    {
        inActiveThrow = true;
        currentThrowDirection = direction;
    }

    private void HandleThrowing()
    {
        if (Input.GetMouseButtonDown(1))
        {
            throwMouseStartingPos = cameraToUse.WorldToScreenPoint(transform.position); //throwMouseStartingPos = Input.mousePosition; //throwKeyHeldDownStart = Time.time;
        }
        if (Input.GetMouseButtonUp(1))
        {
            throwKeyReleased = Time.time;
            if (inActiveThrow) { ThrowWeapon(currentThrowDirection, currentThrowForce); }
        }
        if (Input.GetMouseButton(1))
        {
            if (inActiveThrow)
            {
                throwMouseFinishingPos = Input.mousePosition;
                throwDistanceToPass = Math.Abs(throwMouseFinishingPos.y - throwMouseStartingPos.y);
                //throwKeyHeldDownTime = Time.time - throwKeyHeldDownStart;
                //currentThrowForce = CalcThrowForce(throwKeyHeldDownTime);
                currentThrowForce = CalcThrowForce(throwDistanceToPass);
            }
        }
    }

    private float CalcThrowForce(float holdForce)
    {
        //holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        throwDistanceNormalized = Mathf.Clamp01(holdForce / maxThrowBand);
        float force = throwDistanceNormalized * maxThrowSpeed;
        force += minThrowSpeed;
        EventSystem.current.StartChargedAttackTrigger(throwDistanceNormalized, projectileSpawnPoint.transform, force);
        //EventSystem.current.StartTossingWeaponTrigger(holdTimeNormalized, projectileSpawnPoint.transform, force);
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
            //toss.GetComponent<Rigidbody2D>().AddForce((projectileSpawnPoint.transform.right + projectileSpawnPoint.transform.up).normalized * throwSpeed, ForceMode2D.Impulse);
        }
        else {toss.GetComponent<Rigidbody2D>().AddForce((projectileSpawnPoint.transform.right).normalized * throwSpeed, ForceMode2D.Impulse); }

        inActiveThrow = false;
        EventSystem.current.FinishChargedAttackTrigger();
    }

    private void FixedDistanceFire() { if (Input.GetMouseButton(1)) { fixedDistanceAmmo.SetActive(true); ToggleFlamethrowerEffects(true); }}
    private void StopFixedFire() { fixedDistanceAmmo.SetActive(false); ToggleFlamethrowerEffects(false); }

    private void ToggleFlamethrowerEffects(bool flamethrowerState) { FindObjectOfType<AudioManager>().LoopSFX("Flamethrower", flamethrowerState); }

    public void Flip() // used in game controller to flip projectile spawnpoint when player changes direction; should only be called if using horizontal / vertical direction only firing
    {
        gameObject.transform.localScale = new Vector3(
          transform.localScale.x,
          transform.localScale.y*-1,
          transform.localScale.z);
    }

    private void WeaponChanged(int weaponID, int weaponLevel)
    {
        UpdateAmmoUsed(weaponID, weaponLevel);

        // TO ADD: intended to handle weapon sprite changes as well?
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

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onUpdateSecondaryWeaponTrigger -= WeaponChanged;
        EventSystem.current.onWeaponFireTrigger -= WeaponFired;
        EventSystem.current.onWeaponStopTrigger -= WeaponStop;
    }
}