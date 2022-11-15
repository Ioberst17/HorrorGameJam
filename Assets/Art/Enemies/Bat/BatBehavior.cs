using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehavior : MonoBehaviour
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
    public void BatPassover()
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
            BatPatrol();
        }
        else
        {
            BatChase();
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

    private void BatPatrol()
    {
        switch (enemyController.patrolID)
        {
            case 0:
                if(transform.position.y < enemyController.patrol1Point.y)
                {
                    newVelocity.Set(enemyController.patrolSpeed, enemyController.patrolSpeed);
                }
                else if (transform.position.y > enemyController.patrol1Point.y)
                {
                    newVelocity.Set(enemyController.patrolSpeed, -enemyController.patrolSpeed);
                }
                else
                {
                    newVelocity.Set(enemyController.patrolSpeed, 0);
                }
                enemyController.rb.velocity = newVelocity;
                break;
            case 1:
                if (transform.position.y < enemyController.patrol1Point.y)
                {
                    newVelocity.Set(-enemyController.patrolSpeed, enemyController.patrolSpeed);
                }
                else if (transform.position.y > enemyController.patrol1Point.y)
                {
                    newVelocity.Set(-enemyController.patrolSpeed, -enemyController.patrolSpeed);
                }
                else
                {
                    newVelocity.Set(-enemyController.patrolSpeed, 0);
                }
                enemyController.rb.velocity = newVelocity;
                break;
            case 2:
                if (transform.position.y < enemyController.patrol1Point.y)
                {
                    newVelocity.Set(enemyController.patrolSpeed, enemyController.patrolSpeed);
                }
                else if (transform.position.y > enemyController.patrol1Point.y)
                {
                    newVelocity.Set(enemyController.patrolSpeed, -enemyController.patrolSpeed);
                }
                else
                {
                    newVelocity.Set(enemyController.patrolSpeed, 0);
                }
                enemyController.rb.velocity = newVelocity;
                break;
            default:
                break;
        }
    }
    private void BatChase()
    {
        if (enemyController.playerLocation.position.x >= transform.position.x)
        {
            if(enemyController.playerLocation.position.y < transform.position.y)
            {
                newVelocity.Set(enemyController.patrolSpeed * 1.5f, -enemyController.patrolSpeed);
            }
            else
            {
                newVelocity.Set(enemyController.patrolSpeed * 1.5f, enemyController.patrolSpeed);
            }
            
            enemyController.rb.velocity = newVelocity;
        }
        else
        {
            if (enemyController.playerLocation.position.y < transform.position.y)
            {
                newVelocity.Set(-enemyController.patrolSpeed * 1.5f, -enemyController.patrolSpeed);
            }
            else
            {
                newVelocity.Set(-enemyController.patrolSpeed * 1.5f, enemyController.patrolSpeed);
            }
            enemyController.rb.velocity = newVelocity;
        }
    }

}

