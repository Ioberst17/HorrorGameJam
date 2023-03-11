using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : Shield
{
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private int parryDamage = 10;

    new void Start()
    {
        base.Start();
        playerController = GetComponentInParent<PlayerController>();
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    public override void SpecificDamageChecks(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyController>() != null && collision.gameObject.GetComponent<EnemyController>().isAttacking) { checkStatus = true; }
        else if(collision.gameObject.GetComponent<EnemyProjectile>()) { checkStatus = true; }
        else if (collision.gameObject.GetComponent<Explode>() != null) { checkStatus = true; }
    }
    public override void CheckObjectType()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Player")) { shieldedObject = "Player"; }
        else { Debug.Log("Shield.cs is attached to an object that does not need it"); }

        AddCollisionsToCheckFor(shieldedObject);
    }

    public override void AddCollisionsToCheckFor(string shieldedObject)
    {
        if (shieldedObject == "Player") { layer1ToCheck = LayerMask.NameToLayer("Enemy"); layer2ToCheck = LayerMask.NameToLayer("Player Ammo"); }
        base.AddCollisionsToCheckFor(shieldedObject);
    }

    public override void PassThroughDamage(Collider2D collision, float damageMod, float knockbackMod) 
    {
        if (collision.gameObject.GetComponent<EnemyController>() != null)
        {
            EventSystem.current.PlayerHitCalcTrigger(
                              collision.gameObject.transform.position,
                              collision.gameObject.GetComponent<EnemyController>().damageValue,
                              1,
                              damageMod,
                              knockbackMod);
        }
        else if (collision.gameObject.GetComponent<EnemyProjectile>() != null)
        {
            EventSystem.current.PlayerHitCalcTrigger(
                              collision.gameObject.transform.position,
                              collision.gameObject.GetComponent<EnemyProjectile>().damageValue,
                              1,
                              damageMod,
                              knockbackMod);
        }
        else if (collision.gameObject.GetComponent<Explode>() != null)
        {
            EventSystem.current.PlayerHitCalcTrigger(
                              collision.gameObject.transform.position,
                              10,
                              1,
                              damageMod,
                              knockbackMod);
        }
    }

    public override void ReturnDamage(Collider2D collision)
    {
        Instantiate(Resources.Load("VFXPrefabs/BulletImpact"), collision.transform.position, Quaternion.identity);
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        { 
            collision.gameObject.GetComponent<IDamageable>().Hit(collision.gameObject.GetComponent<EnemyController>().damageValue, transform.position); // last case is breakable objects
        }
    }

}
