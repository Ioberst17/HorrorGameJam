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
        if (col.gameObject.tag == "Enemy")
        {
            EventSystem.current.AttackHitTrigger(ammoID);
            Destroy(gameObject);
        }
    }

}
