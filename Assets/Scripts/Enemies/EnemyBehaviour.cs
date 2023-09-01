using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    // outside references
    protected EnemyController enemyController;
    protected EnemyHealth enemyHealth;
    protected EnemyDataLoader enemyData;
    protected EnemyAttackManager attackManager;
    protected EnemyProjectileManager projectileManager;

    // cooldowns
    protected int flipCoolDown;
    protected int flipCoolDownMax = 15; 

    // used for speed
    protected float targetXVelocity = 0;
    protected float targetYVelocity = 0;
    protected float chaseSpeed;
    
    // used during passover function
    protected string patrolAnimation, chaseAnimation;


    virtual protected void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyController = GetComponent<EnemyController>();
        enemyData = GetComponent<EnemyDataLoader>();
        attackManager = GetComponentInChildren<EnemyAttackManager>();
        projectileManager = GetComponentInChildren<EnemyProjectileManager>();
    }

    virtual protected void FixedUpdate()
    {
        if (flipCoolDown > 0) { --flipCoolDown; }
        // do something, if not stunned and not dead
        if ((!enemyController.IsStunned || 
            !enemyController.InHitStun) 
            && !enemyController.IsDead)
        { 
            Passover(); 
        }
    }

    /// <summary>
    /// The major update function of an enemy behavior; defined in specific enemy classes
    /// </summary>
    virtual protected void Passover()
    {
        // if attacking or charging attack, don't do this
        if (!(enemyController.IsAttackingOrChargingAttack))
        {
            UpdatePatrolID();

            if (!enemyController.playerInZone)
            {
                enemyController.animator.Play(patrolAnimation);
                Patrol();
            }
            else
            {
                enemyController.animator.Play(chaseAnimation);
                Chase();
            }
        }

        Flip();
    }

    /// <summary>
    /// Defines how an enemy is meant to 'patrol' a given area; toggles between two patrol points: P1 and P2
    /// </summary>
    virtual protected void Patrol()
    {
        switch (enemyController.patrolID)
        {
            case 0:
                enemyController.SetVelocity(enemyController.MovementSpeed, null);
                break;
            case 1:
                enemyController.SetVelocity(-enemyController.MovementSpeed, null);
                break;
            case 2:
                enemyController.SetVelocity(enemyController.MovementSpeed, null);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Called to toggle the direction of patrol
    /// </summary>
    virtual protected void UpdatePatrolID()
    {
        // Determine which patrol point to use based on the enemy's position
        if (enemyController.transform.position.x >= enemyController.patrol1Point.x) { enemyController.patrolID = 1; }
        else if (enemyController.transform.position.x <= enemyController.patrol2Point.x) { enemyController.patrolID = 2; }
    }

    /// <summary>
    /// Generic chase behavior, pursue the player in x / y direction
    /// </summary>
    virtual protected void Chase()
    {
        chaseSpeed = enemyController.MovementSpeed * 1.5f;
        targetXVelocity = (enemyController.playerLocation.position.x >= transform.position.x) ? chaseSpeed : -chaseSpeed;
        if (enemyData.data.isFlying) { targetYVelocity = (enemyController.playerLocation.position.y < transform.position.y) ? -enemyController.MovementSpeed : enemyController.MovementSpeed; }

        enemyController.SetVelocity(targetXVelocity, targetYVelocity);
    }

    /// <summary>
    /// Overriden in child classes for specific attacks
    /// </summary>
    virtual public void AttackTrigger() { }    
    
    /// <summary>
    /// Overriden in child classes for specific attacks
    /// </summary>
    virtual public void ProjectileTrigger() { }

    // COMMON PHYSICS BEHAVIORS

    /// <summary>
    /// Flip the enemy's sprite if it doesn't have x velocity in the way it should be going, then set flip cool down
    /// </summary>
    virtual protected void Flip()
    {
        if (HeadedInTheWrongDirection())
        {
            if (flipCoolDown == 0) { enemyController.Flip(); flipCoolDown = flipCoolDownMax; }
        }
    }

    bool HeadedInTheWrongDirection()
    {
        return (HeadedRightButFacingLeft() || HeadedLeftButFacingRight() ||
                 WalkingIntoARightFacingBarrier() || WalkingIntoALeftFacingBarrier());
    }

    bool HeadedRightButFacingLeft() { return (enemyController.RB.velocity.x >= 0.5f && enemyController.FacingDirection == -1); }

    bool HeadedLeftButFacingRight() { return (enemyController.RB.velocity.x <= -0.5f && enemyController.FacingDirection == 1); }

    bool WalkingIntoARightFacingBarrier() { return ((enemyController.RB.velocity.x > 0 && enemyController.RB.velocity.x < 0.0001f)  && enemyController.FacingDirection == 1); }
    bool WalkingIntoALeftFacingBarrier() { return ((enemyController.RB.velocity.x < 0 && enemyController.RB.velocity.x > -0.0001f) && enemyController.FacingDirection == -1); }

    /// <summary>
    /// Flip the enemy's sprite to face player if in zone
    /// </summary>
    virtual protected void FlipToFacePlayer()
    {
        if (enemyController.playerInZone == true)
        {
            // if player is to the left of enemy
            if (transform.position.x > enemyController.playerLocation.position.x)
            {
                // and enemy is facing right
                if (enemyController.FacingDirection == 1) { enemyController.Flip(); }
            }
            // if player is to the right of enemy
            else if (transform.position.x < enemyController.playerLocation.position.x)
            {
                // and enemy is facing left
                if (enemyController.FacingDirection == -1) { enemyController.Flip(); }
            }
        }
    }
}
