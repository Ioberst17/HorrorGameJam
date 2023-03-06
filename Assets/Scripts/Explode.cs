using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Explode : MonoBehaviour
{
    private List<Collider2D> GetObjectsInExplosionRadius(float radius) 
    {
        List<Collider2D> touchedObjects = Physics2D.OverlapCircleAll(transform.position, radius).Where(x => x.GetComponent<IDamageable>() !=null).ToList();
        return touchedObjects;
    }

    public void ExplosionDamage(float radius, float pushForce, int damageToGive)
    {
        foreach (Collider2D touchedObject in GetObjectsInExplosionRadius(radius))
        {
            PushObject(touchedObject, radius, pushForce);
            PassDamage(touchedObject, radius, damageToGive);
        }
        Destroy(gameObject, 1f);
    }

    public void ExplosionDamageAmmo(float radius, float pushForce, int weaponID, int weaponLevel)
    {

    }

    private void PushObject(Collider2D touchedObject, float radius, float pushForce)
    {
        Rigidbody rigidbody = touchedObject.GetComponent<Rigidbody>();
        if (rigidbody != null) { rigidbody.AddExplosionForce(pushForce, transform.position, radius); }
    }

    private void PassDamage(Collider2D touchedObject, float pushForce, int damageToGive)
    {
        var target = touchedObject.gameObject.GetComponent<IDamageable>();

        if (touchedObject.gameObject.layer == LayerMask.NameToLayer("Player"))
        { touchedObject.gameObject.GetComponentInChildren<PlayerShield>().DamageHandler(gameObject.GetComponent<Collider2D>()); }

        else if (touchedObject.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        { touchedObject.gameObject.GetComponent<EnemyController>().TakeDamage(damageToGive); }

        else { target.Hit(damageToGive); } // last case is breakable objects
    }

}
