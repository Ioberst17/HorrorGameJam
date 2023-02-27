using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalysisDemonBehavior : MonoBehaviour
{

    private Vector2 newVelocity;
    EnemyController enemyController;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyController.isAttacking = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PDPassover()
    {
        if (enemyController.transform.position.x >= enemyController.patrol1Point.x)
        {
            enemyController.patrolID = 1;
        }
        else if (enemyController.transform.position.x <= enemyController.patrol2Point.x)
        {
            enemyController.patrolID = 2;
        }
        if (!enemyController.playerInZone)
        {
            PDemonPatrol();
        }
        else
        {
            PDemonChase();
        }
        if (enemyController.rb.velocity.x >= 0.5f && enemyController.facingDirection == -1)
        {
            enemyController.Flip();
        }
        else if (enemyController.rb.velocity.x <= -0.5f && enemyController.facingDirection == 1)
        {
            enemyController.Flip();
        }
    }

    private void PDemonPatrol()
    {
        switch (enemyController.patrolID)
        {
            case 0:
                newVelocity.Set(enemyController.patrolSpeed, enemyController.rb.velocity.y);
                enemyController.rb.velocity = newVelocity;
                break;
            case 1:
                newVelocity.Set(-enemyController.patrolSpeed, enemyController.rb.velocity.y);
                enemyController.rb.velocity = newVelocity;
                break;
            case 2:
                newVelocity.Set(enemyController.patrolSpeed, enemyController.rb.velocity.y);
                enemyController.rb.velocity = newVelocity;
                break;
            default:
                break;
        }
    }
    private void PDemonChase()
    {
        if (enemyController.playerLocation.position.x > transform.position.x)
        {
            newVelocity.Set(enemyController.patrolSpeed * 1.5f, enemyController.rb.velocity.y);
            enemyController.rb.velocity = newVelocity;
        }
        else if (enemyController.playerLocation.position.x < transform.position.x)
        {
            newVelocity.Set(-enemyController.patrolSpeed * 1.5f, enemyController.rb.velocity.y);
            enemyController.rb.velocity = newVelocity;
        }
        else
        {
            newVelocity.Set(0, enemyController.rb.velocity.y);
            enemyController.rb.velocity = newVelocity;
        }
    }

}
