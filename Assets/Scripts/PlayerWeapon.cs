using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    //ammo-related
    public List<GameObject> ammoPrefabs;
    public Transform projectileSpawnPoint;
    public int ammoSpeed = 500;
    // weapon rotation
    public int upAngle = 90;
    public int downAngle = -90;
    public int standardAngle = 0;

    // Start is used to subscribe to weapon events
    void Start()
    {
        EventSystem.current.onWeaponFireTrigger += WeaponFired;

        ammoPrefabs = Resources.LoadAll<GameObject>("AmmoPrefabs").ToList();
    }

    // Update is called once per frame, used mainly for weapon direction
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey("up"))
        {
            gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * upAngle);
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey("down")))
        {
            gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * downAngle);
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(Vector3.forward * standardAngle);
        }
    }

    private void WeaponFired(int weaponID, int weaponLevel, int ammoChange, int direction)
    {
        int ammoIndex = weaponID + ((2 * (weaponID - 1)-1)) + (weaponLevel-1); //formula for index assumes three levels for every weapon + prefabs in resources folder
        
        GameObject shot = Instantiate(ammoPrefabs[ammoIndex], projectileSpawnPoint.position, ammoPrefabs[ammoIndex].transform.rotation);

        shot.GetComponent<Rigidbody2D>().AddForce(transform.right * ammoSpeed);
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onWeaponFireTrigger -= WeaponFired;
    }
}
