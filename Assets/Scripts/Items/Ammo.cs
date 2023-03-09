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
                EventSystem.current.AttackHitTrigger(weaponID, weaponLevel, transform.position);
                Instantiate(Resources.Load("VFXPrefabs/DamageImpact"), transform.position, Quaternion.identity);
                Destroy(gameObject);
            }        
        }


        if (col.gameObject.tag == "Boundary" || col.gameObject.layer == BreakableEnviroLayer)
        {
            if (isThrown) { StartCoroutine(ExplodeCoroutine()); }
            else
            {
                if(col.gameObject.GetComponent<IDamageable>() != null) { col.gameObject.GetComponent<IDamageable>().Hit(); }
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
            EventSystem.current.AttackHitTrigger(weaponID, weaponLevel, GetComponentInParent<Transform>().position);
        }
    }



    IEnumerator ExplodeCoroutine()
    {
        CreateExplosion();
        DisableInteractions();
        yield return 0;
    }

    private void CreateExplosion()
    {
        Instantiate(Resources.Load("VFXPrefabs/Explosion"), transform.position, Quaternion.identity);
        FindObjectOfType<AudioManager>().PlaySFX("WeaponExplosion");
        if (GetComponent<Explode>() != null) { GetComponent<Explode>().ExplosionDamage(2f, 10f, 10); }
        else { Debug.Log("Add the Explode.cs script to the ammo prefab that is triggering this script"); }
    }

    private void DisableInteractions() { GetComponent<Collider2D>().enabled = false; GetComponent<SpriteRenderer>().enabled = false; }

}
