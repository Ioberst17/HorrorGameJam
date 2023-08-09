using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehavior : EnemyAttackBehavior
{
    // Override the base passover called in Parent FixedUpdate
    override protected void Passover()
    {
        UpdatePatrolID();

        // Check if the player is in the attack zone
        if (enemyController.playerInZone) { Chase(); } // If yes, chase
        else { Patrol(); } // If no, patrol

        Flip();
    }

    // Override the Patrol method, called when player not in zone
    override protected void Patrol()
    {
        targetXVelocity = 0;
        targetYVelocity = 0;

        switch (enemyController.patrolID)
        {
            case 0:
                targetXVelocity = enemyController.MovementSpeed;
                targetYVelocity = enemyController.MovementSpeed;
                break;
            case 1:
                targetXVelocity = -enemyController.MovementSpeed;
                targetYVelocity = enemyController.MovementSpeed;
                break;
            case 2:
                targetXVelocity = enemyController.MovementSpeed;
                targetYVelocity = enemyController.MovementSpeed;
                break;

            default:
                break;
        }

        if (transform.position.y < enemyController.patrol1Point.y)
        {
            enemyController.SetVelocity(targetXVelocity, targetYVelocity);
        }
        else if (transform.position.y > enemyController.patrol1Point.y)
        {
            enemyController.SetVelocity(targetXVelocity, -targetYVelocity);
        }
        else { enemyController.SetVelocity(targetXVelocity, 0); }
    }
}

