using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PlayerWeapon : MonoBehaviour
{
    private SpriteRenderer weaponSprite;

    //ammo-related
    public List<GameObject> ammoPrefabs;
    public Transform projectileSpawnPoint; // assigned in inspector
    public int ammoSpeed = 500;
    private int ammoGravity = 0;

    public WeaponDatabase weaponDatabase;
    private GameObject fixedDistanceAmmo;
    [SerializeField]
    private int currentWeaponID;
    [SerializeField]
    private int currentWeaponLevel;
    [SerializeField]
    private int currentAmmoIndex;

    //throwing specific
    [SerializeField]
    private float maxForceHoldDownTime = 2f;
    [SerializeField]
    private static int maxThrowSpeed = 10;
    private static int minThrowSpeed = 5;
    public int throwSpeed;
    private int throwGravity = 1;
    private float throwKeyHeldDownStart;
    private float throwKeyReleased;
    [SerializeField]
    private float throwKeyHeldDownTime;
    [SerializeField]
    private float holdTimeNormalized;
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
    private float rotationZ; //used for weapon direction

    void Start()
    {
        EventSystem.current.onUpdatePlayerWeaponTrigger += WeaponChanged;
        EventSystem.current.onWeaponFireTrigger += WeaponFired;
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
    }

    // Update is called once per frame, used mainly for weapon direction
    void Update()
    {
        HandleWeaponDirection();

        HandleThrowing();

        HandleFixedDistanceFire();
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
        else if (weaponDatabase.weaponDatabase.entries[weaponID].isFixedDistance == true) { FixedDistanceFire(); }
        else { Debug.Log("Check WeaponDatabase, weapon is missing a TRUE value for if ammo should be shot, thrown, be a fixed distance, etc."); }
    }

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
            throwKeyHeldDownStart = Time.time;
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
                throwKeyHeldDownTime = Time.time - throwKeyHeldDownStart;
                currentThrowForce = CalcThrowForce(throwKeyHeldDownTime);
            }
        }
    }

    private float CalcThrowForce(float holdTime)
    {
        holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        float force = holdTimeNormalized * maxThrowSpeed;
        force += minThrowSpeed;
        EventSystem.current.StartTossingWeaponTrigger(holdTimeNormalized, projectileSpawnPoint.transform, force);
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
        EventSystem.current.FinishTossingWeaponTrigger();
    }

    private void FixedDistanceFire()
    {
        if (Input.GetMouseButton(1)) { fixedDistanceAmmo.SetActive(true);}
    }

    private void HandleFixedDistanceFire()
    {
        if (!Input.GetMouseButton(1)) { fixedDistanceAmmo.SetActive(false); }
    }

    public void Flip() // used in game controller to flip projectile spawnpoint when player changes direction; should only be called if using horizontal / vertical direction only firing
    {
        gameObject.transform.Rotate(0.0f, 180.0f, 0.0f);
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
        EventSystem.current.onUpdatePlayerWeaponTrigger -= WeaponChanged;
        EventSystem.current.onWeaponFireTrigger -= WeaponFired;
    }
}