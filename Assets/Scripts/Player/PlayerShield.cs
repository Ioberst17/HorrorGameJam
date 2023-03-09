using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : Shield
{
    private PlayerController playerController;
    private PlayerHealth playerHealth;

    new void Start()
    {
        base.Start();
        playerController = GetComponentInParent<PlayerController>();
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    public override void SpecificDamageChecks(Collider2D collision)
    {
        if (shieldedObject == "Player" && collision.gameObject.GetComponent<EnemyController>().isAttacking) { checkStatus = true; }
        else if (shieldedObject == "Player" && collision.gameObject.GetComponent<Explode>() != null) { checkStatus = true; }
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
            EventSystem.current.PlayerHitTrigger(
                              collision.gameObject.transform.position,
                              collision.gameObject.GetComponent<EnemyController>().damageValue,
                              1,
                              damageMod,
                              knockbackMod);
        }
        if (collision.gameObject.GetComponent<Explode>() != null)
        {
            EventSystem.current.PlayerHitTrigger(
                              collision.gameObject.transform.position,
                              10,
                              1,
                              damageMod,
                              knockbackMod);
        }
    }

}
