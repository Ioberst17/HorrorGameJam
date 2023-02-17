using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public string weaponName;
    public int weaponID; //used in damage calculations when looking up in Weapon Database
    public int weaponLevel;
    public int ammoID;
    public bool isFixedDistance;


    public int GetAmmoID()
    {
        return ammoID;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!isFixedDistance) { HandleStandardAmmo(col); }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isFixedDistance) { HandleFixedDistanceAmmo(other); }
    }

    private void HandleStandardAmmo(Collision2D col)
    {
        if (col.gameObject.GetComponent<EnemyController>() != null)
        {
            var enemyController = col.gameObject.GetComponent<EnemyController>();
            enemyController.AmmoDamage(weaponID, weaponLevel);
            Destroy(gameObject);
        }

        if (col.gameObject.tag == "Boundary")
        {
            Destroy(gameObject);
        }
    }

    private void HandleFixedDistanceAmmo(Collider2D other)
    {
        if(other.gameObject.GetComponent<EnemyController>() != null)
        {
            var enemyController = other.gameObject.GetComponent<EnemyController>();
            enemyController.AmmoDamage(weaponID, weaponLevel);
        }
    }

    /*private void OnTriggerStay2D(Collider2D col)
    {
        if (isFixedDistance && col.gameObject.GetComponent<EnemyController>() != null)
        {
            var enemyController = col.gameObject.GetComponent<EnemyController>();
            enemyController.AmmoDamage(weaponID, weaponLevel);
        }
    }*/

}
