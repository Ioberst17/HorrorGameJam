using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public int ammoID; //used in damage calculations when looking up in Weapon Database

    void Update()
    {
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<EnemyController>() != null)
        {
            var enemyController = col.gameObject.GetComponent<EnemyController>();
            enemyController.AmmoDamage(ammoID);
            Destroy(gameObject);
        }

        if(col.gameObject.tag == "Boundary")
        {
            Destroy(gameObject);
        }
    }

}
