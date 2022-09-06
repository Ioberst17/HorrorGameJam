using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public int ammoSpeed = 500;
    public int upAngle = 90;
    public int downAngle = -90;
    public int standardAngle = 0;
    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.onWeaponFireTrigger += WeaponFired;
    }

    // Update is called once per frame
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

    private void WeaponFired(int weaponID, int ammoChange, int direction)
    {
        GameObject shot = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectilePrefab.transform.rotation);

        shot.GetComponent<Rigidbody2D>().AddForce(transform.right * ammoSpeed);
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onWeaponFireTrigger -= WeaponFired;
    }
}
