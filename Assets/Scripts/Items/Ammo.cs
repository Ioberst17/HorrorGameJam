using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : ProjectileBase
{
    public int GetAmmoID() { return ammoID;}

    override protected void Start() { base.Start(); animator = GetComponent<Animator>(); }

    override protected void HitEnemyObject(Collision2D col)
    {
        if (col.gameObject.GetComponent<EnemyController>() != null ||
                col.gameObject.GetComponent<EnemyProjectile>() != null)
        {
            RigidbodyEnabled = false;
            if (projectile.isExplosive) { ExplosionHandler(); }
            else if (!projectile.isExplosive) { Remove(true); }
        }
    }
}
