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
    public string statusModifier;
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
            if (isExplosive) { ExplosionHandler(); }
            else if (!isExplosive) { PassDamage(weaponID, weaponLevel, transform.position, statusModifier);}        
        }


        if (col.gameObject.tag == "Boundary" || col.gameObject.layer == BreakableEnviroLayer)
        {
            if (isExplosive)  { ExplosionHandler(); }
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
            EventSystem.current.AttackHitTrigger(weaponID, weaponLevel, GetComponentInParent<Transform>().position, statusModifier);
        }
    }

    private void ExplosionHandler()
    {
        if (GetComponent<Explode>() != null) { GetComponent<Explode>().AmmoExplosion(2f, 10f, weaponID, weaponLevel) ; }
        else { Debug.Log("Trying to call Explode function, but does not have Explode.cs script attached"); }
        DisableInteractions();
    }

    private void PassDamage(int weaponID, int weaponLevel, Vector3 position, string statusModifier)
    {
        EventSystem.current.AttackHitTrigger(weaponID, weaponLevel, transform.position, statusModifier);
        Instantiate(Resources.Load("VFXPrefabs/DamageImpact"), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void DisableInteractions() { GetComponent<Collider2D>().enabled = false; GetComponent<SpriteRenderer>().enabled = false; }

}
