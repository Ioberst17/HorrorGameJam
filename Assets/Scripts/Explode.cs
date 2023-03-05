using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public void ExplosionDamage(float GrenadeRadius, float ExplosionForce)
    {
        Collider2D[] touchedObjects = Physics2D.OverlapCircleAll(transform.position, GrenadeRadius).Where(x => x is IDamageable).ToArray();

        foreach (Collider2D touchedObject in touchedObjects)
        {
            Rigidbody rigidbody = touchedObject.GetComponent<Rigidbody>();
            if (rigidbody != null) { rigidbody.AddExplosionForce(ExplosionForce, transform.position, GrenadeRadius); }

            var target = touchedObject.gameObject.GetComponent<IDamageable>();
            //target.takeDamage();

        }
        Destroy(gameObject);
    }
}
