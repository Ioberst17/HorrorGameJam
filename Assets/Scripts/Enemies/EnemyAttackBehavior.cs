using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stores the actual behavior of enemies and how they interact with the world / player (e.g. when and how they attack)
public class EnemyAttackBehavior : MonoBehaviour
{
    // outside references
    protected EnemyController enemyController;
    protected EnemyHealth enemyHealth;

    // cooldowns
    protected int flipCoolDown;
    protected int flipCoolDownMax; // set in child classes

    // used for speed
    protected float targetXVelocity = 0;
    protected float targetYVelocity = 0;
    protected float chaseSpeed;

    virtual protected void Start() 
    { 
        enemyHealth = GetComponentInParent<EnemyHealth>();
        enemyController = GetComponentInParent<EnemyController>();
        enemyController.IsAttacking = true;
    }

    virtual protected void FixedUpdate()
    {
        if(flipCoolDown > 0) { --flipCoolDown; }
        // do something, if not stunned and not dead
        if ((!enemyController.IsStunned || !enemyController.InHitStun) && !enemyController.IsDead) 
        { Passover(); }                      
    }
    virtual protected void Passover()
    {

    }

    virtual protected void Patrol()
    {
        switch (enemyController.patrolID)
        {
            case 0:
                enemyController.SetVelocity(enemyController.MovementSpeed, enemyController.RB.velocity.y);
                break;
            case 1:
                enemyController.SetVelocity(-enemyController.MovementSpeed, enemyController.RB.velocity.y);
                break;
            case 2:
                enemyController.SetVelocity(enemyController.MovementSpeed, enemyController.RB.velocity.y);
                break;
            default:
                break;
        }
    }

    virtual protected void UpdatePatrolID()
    {
        // Determine which patrol point to use based on the enemy's position
        if (enemyController.transform.position.x >= enemyController.patrol1Point.x) { enemyController.patrolID = 1; }
        else if (enemyController.transform.position.x <= enemyController.patrol2Point.x) { enemyController.patrolID = 2; }
    }

    // Generic chase behavior, pursue the player in x / y direction
    virtual protected void Chase()
    {
        chaseSpeed = enemyController.MovementSpeed * 1.5f;
        targetXVelocity = (enemyController.playerLocation.position.x >= transform.position.x) ? chaseSpeed : -chaseSpeed;
        if (enemyController.EnemyInfo.isFlying) { targetYVelocity = (enemyController.playerLocation.position.y < transform.position.y) ? -enemyController.MovementSpeed : enemyController.MovementSpeed; }

        enemyController.SetVelocity(targetXVelocity, targetYVelocity);
    }

    // Flip the enemy's sprite if it doesn't have x velocity in the way it should be going, then set flip cool down
    virtual protected void Flip()
    {
        if ((enemyController.RB.velocity.x >= 0.5f && enemyController.FacingDirection == -1) ||
            (enemyController.RB.velocity.x <= -0.5f && enemyController.FacingDirection == 1))
        {
            if (flipCoolDown == 0) {enemyController.Flip(); flipCoolDown = flipCoolDownMax; }
        }
    }

    virtual protected void FlipToFacePlayer()
    {
        if(enemyController.playerInZone == true) 
        {
            if (transform.position.x <= enemyController.playerLocation.position.x)
            {
                if (enemyController.FacingDirection == -1) { enemyController.Flip(); }
            }
            else { if (enemyController.FacingDirection == 1) { enemyController.Flip(); } }
        }
    }
}

