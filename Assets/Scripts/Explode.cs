using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Explode : MonoBehaviour
{
    private List<Collider2D> GetObjectsInExplosionRadius(float radius) 
    {
        List<Collider2D> touchedObjects = Physics2D.OverlapCircleAll(transform.position, radius).Where(x => x.GetComponent<IDamageable>() !=null).ToList();
        return touchedObjects;
    }

    public void StandardExplosion(float radius, float pushForce, int damageToGive)
    {
        AddExplosionEffects();

        foreach (Collider2D touchedObject in GetObjectsInExplosionRadius(radius))
        {
            PushObject(touchedObject, radius, pushForce);
            PassStandardDamage(touchedObject, radius, damageToGive);
        }
        Destroy(gameObject, 1f);
    }

    public void AmmoExplosion(float radius, float pushForce, string weaponName)
    {
        AddExplosionEffects(weaponName);
        foreach (Collider2D touchedObject in GetObjectsInExplosionRadius(radius))
        {
            PushObject(touchedObject, radius, pushForce);
            PassAmmoDamage(touchedObject, radius);
        }
        Destroy(gameObject, 1f);
    }

    private void PushObject(Collider2D touchedObject, float radius, float pushForce)
    {
        Rigidbody rigidbody = touchedObject.GetComponent<Rigidbody>();
        if (rigidbody != null) { rigidbody.AddExplosionForce(pushForce, transform.position, radius); }
    }

    private void PassStandardDamage(Collider2D touchedObject, float pushForce, int damageToGive)
    {
        var target = touchedObject.gameObject.GetComponent<IDamageable>();

        if (touchedObject.gameObject.layer == LayerMask.NameToLayer("Player"))
        { /*touchedObject.gameObject.GetComponentInChildren<PlayerShield>().DamageHandler(gameObject.GetComponent<Collider2D>());*/ }

        else if (touchedObject.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        { touchedObject.gameObject.GetComponent<EnemyHealth>().TakeDamage(damageToGive); }

        else { target.Hit(damageToGive, transform.position, gameObject); } // last case is breakable objects
    }

    private void PassAmmoDamage(Collider2D touchedObject, float pushForce)
    {
        var target = touchedObject.gameObject.GetComponent<IDamageable>();

        if (touchedObject.gameObject.layer == LayerMask.NameToLayer("Player"))
        { EventSystem.current.PlayerShieldHitTrigger(touchedObject); }

        else if (touchedObject.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        { 
            var enemyController = touchedObject.GetComponent<EnemyController>();
            if (enemyController != null) { EventSystem.current.EnemyAmmoHitTrigger(GetComponent<ProjectileBase>().projectile.baseDamage, 
                                                                                    transform.position, 
                                                                                    null, 
                                                                                    enemyController); } }

        else { target.Hit(touchedObject.gameObject); } // last case is breakable objects
    }

    

    private void AddExplosionEffects(string weaponName = "")
    {
        if(weaponName == "Fireworks") // customize later if other VFX are added
        {
            Instantiate(Resources.Load("VFXPrefabs/" + weaponName), transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(Resources.Load("VFXPrefabs/Explosion"), transform.position, Quaternion.identity);
        }
        FindObjectOfType<AudioManager>().PlaySFX("WeaponExplosion");
    }

}
