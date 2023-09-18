using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalysisDemonBehavior : EnemyBehaviour
{
    override protected void Start()
    {
        base.Start();
        flipCoolDownMax = 25;
    }

    override protected void Passover()
    {        
        UpdatePatrolID();
        
        if (!enemyController.playerInZone) { Patrol(); }
        else { Chase(); }

        Flip();
    }

    override protected void Patrol()
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
    override protected void Chase()
    {
        if(flipCoolDown == 0)
        {
            if (enemyController.playerLocation.position.x >= transform.position.x - 0.1f)
            {
                enemyController.SetVelocity
                    (enemyController.MovementSpeed * 1.5f,
                    null);
            }
            else if (enemyController.playerLocation.position.x < transform.position.x + 0.1f)
            {
                enemyController.SetVelocity(
                    -enemyController.MovementSpeed * 1.5f, 
                    null);
            }
            else { enemyController.SetVelocity(0, null); }
        }

        attackManager.StartAttack(0, "ParalysisDemonSlash");
        
    }

}
