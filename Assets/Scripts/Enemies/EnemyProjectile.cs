using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : ProjectileBase
{
    protected override void Start()
    {
        base.Start();
        UpdateInstanceID();
    }

    override protected void HitEnemyObject(Collision2D col) 
    {
        if (col.gameObject.GetComponent<PlayerShield>() != null ||
                col.gameObject.GetComponent<PlayerController>() != null ||
                    col.gameObject.GetComponent<Ammo>())
        {
            RigidbodyEnabled = false;
            if (projectile.isExplosive) { ExplosionHandler(); }
            else if (!projectile.isExplosive) { Remove(true); }
        }
    }

    override protected void UpdateInstanceID() { instanceID = gameObject.GetInstanceID(); }

    /// <summary>
    /// Used to turn on collider by timed projectiles, e.g. targeted spells
    /// </summary>
    /// <param name="instanceID"></param>
    override protected void ColliderOn(int instanceID) { if (instanceID == gameObject.GetInstanceID()) { hitCollider.enabled = true; Debug.Log("ColliderOn"); } }

    /// <summary>
    /// Used to turn off collider by timed projectiles, e.g. targeted spells
    /// </summary>
    /// <param name="instanceID"></param>
    override protected void ColliderOff(int instanceID) { if (instanceID == gameObject.GetInstanceID()) { hitCollider.enabled = false; } Debug.Log("ColliderOff"); }
}
