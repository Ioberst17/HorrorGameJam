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
    public bool isThrown;
    public bool isExplosive;
    public Animator animator;
    private int BreakableEnviroLayer;

    public int GetAmmoID() { return ammoID;}

    private void Start() { animator = GetComponent<Animator>(); BreakableEnviroLayer = LayerMask.NameToLayer("BreakableEnviro"); }

    private void OnCollisionEnter2D(Collision2D col) { if (!isFixedDistance) { HandleStandardAmmo(col); }}

    private void OnTriggerStay2D(Collider2D other) { if (isFixedDistance) { HandleFixedDistanceAmmo(other); } }

    private void HandleStandardAmmo(Collision2D col)
    {
        if (col.gameObject.GetComponent<EnemyController>() != null)
        {
            var enemyController = col.gameObject.GetComponent<EnemyController>();
            if (isThrown) { StartCoroutine(ExplodeCoroutine()); }
            else if (!isThrown)
            {
                enemyController.AmmoHandler(weaponID, weaponLevel);
                Instantiate(Resources.Load("VFXPrefabs/DamageImpact"), transform.position, Quaternion.identity);
                Destroy(gameObject);
            }        
        }


        if (col.gameObject.tag == "Boundary" || col.gameObject.layer == BreakableEnviroLayer)
        {
            if (isThrown) { StartCoroutine(ExplodeCoroutine()); }
            else
            {
                Instantiate(Resources.Load("VFXPrefabs/DamageImpact"), transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            
        }
    }

    private void HandleFixedDistanceAmmo(Collider2D other)
    {
        if(other.gameObject.GetComponent<EnemyController>() != null)
        {
            var enemyController = other.gameObject.GetComponent<EnemyController>();
            enemyController.AmmoHandler(weaponID, weaponLevel);
        }
    }



    IEnumerator ExplodeCoroutine()
    {
        animator.SetTrigger("Explode");
        FindObjectOfType<AudioManager>().PlaySFX("WeaponExplosion");
        if (GetComponent<Explode>() != null) { GetComponent<Explode>().ExplosionDamage(2f, 10f, 10); }
        else { Debug.Log("Add the Explode.cs script to the ammo prefab that is triggering this script"); }
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return 0;
    }

}
