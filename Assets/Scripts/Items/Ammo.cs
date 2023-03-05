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
            enemyController.AmmoDamage(weaponID, weaponLevel);
            if (isThrown) { StartCoroutine(ExplodeCoroutine()); }
            else if (!isThrown)
            {
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
            enemyController.AmmoDamage(weaponID, weaponLevel);
        }
    }



    IEnumerator ExplodeCoroutine()
    {
        animator.SetTrigger("Explode");
        FindObjectOfType<AudioManager>().PlaySFX("WeaponExplosion");
        Destroy(gameObject, 1.5f);
        yield return 0;
    }

}
