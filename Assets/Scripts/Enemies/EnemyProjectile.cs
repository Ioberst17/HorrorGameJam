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
        if (col.gameObject.GetComponent<PlayerController>() != null)
        {
            if (projectile.isExplosive) { ExplosionHandler(); }
            else if (!projectile.isExplosive) { Remove(); }
        }
    }
    override protected void UpdateInstanceID() { Debug.Log("Instance ID is: " + gameObject.GetInstanceID()); instanceID = gameObject.GetInstanceID(); }

    override protected void ColliderOn(int instanceID) { if (instanceID == gameObject.GetInstanceID()) { hitCollider.enabled = true; Debug.Log("ColliderOn"); } }

    override protected void ColliderOff(int instanceID) { if (instanceID == gameObject.GetInstanceID()) { hitCollider.enabled = false; } Debug.Log("ColliderOff"); }
}
