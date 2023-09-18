using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : Shield
{
    EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    protected override void PassThroughDamage(Collider2D collision, float damageMod, float knockbackMod)
    {
        if (collision.gameObject.GetComponentInParent<AttackManager>())
        {
            lastReceivedAttack = collision.gameObject.GetComponentInParent<AttackManager>().MostRecentAttack;
            damageToPass = lastReceivedAttack.baseDamage;

            Debug.Log(gameObject.name + " was hit by enemy " + collision.gameObject.name);
            EventSystem.current.EnemyMeleeHitTrigger(damageToPass,
                                                    collision.gameObject.transform.position,
                                                    null,
                                                    enemyController);
        }
        else if (collision.gameObject.GetComponent<Ammo>() != null)
        {
            var ammo = collision.gameObject.GetComponent<Ammo>();
            var projectile = collision.gameObject.GetComponent<ProjectileBase>().projectile;
            Debug.Log("Enemy was hit by ammo!");
            EventSystem.current.EnemyAmmoHitTrigger(projectile.baseDamage,
                                                    GetComponentInParent<Transform>().position, 
                                                    projectile.statusModifier, 
                                                    enemyController);
        }
        else if (collision.gameObject.GetComponent<Explode>() != null)
        {
            //EventSystem.current.PlayerHitHealthTrigger(
            //                  collision.gameObject.transform.position,
            //                  10,
            //                  1,
            //                  damageMod,
            //                  knockbackMod,
            //                  hitWithinActiveShieldZone);
        }
    }
}
