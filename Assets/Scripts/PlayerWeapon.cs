using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    //ammo-related
    public List<GameObject> ammoPrefabs;
    public Transform projectileSpawnPoint; // assigned in inspector
    public int ammoSpeed = 500;
    private int ammoGravity = 0;
    public int throwSpeed = 10;
    private int throwGravity = 1;
    public WeaponDatabase weaponDatabase; // used to check whether ammo should be shot or thrown
    // weapon rotation
    private PlayerController playerController;
    private int upAngle = 90;
    private int downAngle = -90;
    private int standardAngle = 0;
    private Transform firePoint;

    // Start is used to subscribe to weapon events
    void Start()
    {
        EventSystem.current.onWeaponFireTrigger += WeaponFired;
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        firePoint = gameObject.transform.GetChild(0).gameObject.transform;
        playerController = transform.parent.GetComponent<PlayerController>();

        // load ammo prefabs to a list
        ammoPrefabs = Resources.LoadAll<GameObject>("AmmoPrefabs").ToList();
        
        // go through the list and sort them by ammo IDs
        ammoPrefabs.Sort((randomAmmo, ammoToCompareTo) => randomAmmo.GetComponent<Ammo>().ammoID.CompareTo(ammoToCompareTo.GetComponent<Ammo>().ammoID));
    }

    // Update is called once per frame, used mainly for weapon direction
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey("up"))
        {
            if (playerController.facingDirection == -1) // if player is turned around
            { gameObject.transform.rotation = Quaternion.Euler(-Vector3.forward * upAngle); }
            else { gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * upAngle); }

        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey("down")))
        {
            if(playerController.facingDirection == -1) // if player is turned around
            { gameObject.transform.rotation = Quaternion.Euler(-Vector3.forward * downAngle); }
            else { gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * downAngle); }
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * standardAngle);
        }
    }

    private void WeaponFired(int weaponID, int weaponLevel, int ammoChange, int direction)
    {
        int ammoIndex = weaponID + ((2 * (weaponID - 1)-1)) + (weaponLevel-1); //formula for index assumes three levels for every weapon + prefabs in resources folder

        if (weaponDatabase.weaponDatabase.entries[weaponID].isShot == true) // if ammo is mean to be shot, apply this logic
        {
            GameObject shot = Instantiate(ammoPrefabs[ammoIndex], projectileSpawnPoint.position, projectileSpawnPoint.transform.rotation); 
            FindObjectOfType<AudioManager>().PlaySFX("WeaponFire");

            shot.GetComponent<Rigidbody2D>().gravityScale = ammoGravity;

            shot.GetComponent<Rigidbody2D>().AddForce(projectileSpawnPoint.transform.right * ammoSpeed);
        }
        else // if ammo is meant to be thrown / tossed, apply this logic
        {
            GameObject toss = Instantiate(ammoPrefabs[ammoIndex], projectileSpawnPoint.position, projectileSpawnPoint.transform.rotation);
            FindObjectOfType<AudioManager>().PlaySFX("WeaponToss");

            toss.GetComponent<Rigidbody2D>().gravityScale = throwGravity;
            if (direction == 0) // throwing on ground
            {
                toss.GetComponent<Rigidbody2D>().AddForce((projectileSpawnPoint.transform.right + projectileSpawnPoint.transform.up).normalized * throwSpeed, ForceMode2D.Impulse);
            }
            else if (direction == 1) // throwing upwards
            {
                if(firePoint.rotation == Quaternion.Euler(0, -180, 0)){ Debug.Log("Edit this"); }
                toss.GetComponent<Rigidbody2D>().AddForce((projectileSpawnPoint.transform.right).normalized * throwSpeed, ForceMode2D.Impulse);
            }
            else if (direction == -1) // throwing downwards
            {
                toss.GetComponent<Rigidbody2D>().AddForce((projectileSpawnPoint.transform.right).normalized * throwSpeed, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("Check PlayerWeapon.CS for throwing direction misfiring!");
            }

        }


    }

    public void Flip() // used in game controller to flip projectile spawnpoint when player changes direction
    {
        projectileSpawnPoint.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onWeaponFireTrigger -= WeaponFired;
    }
}