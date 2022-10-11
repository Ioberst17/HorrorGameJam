using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellhoundBehavior : MonoBehaviour
{
    bool HellhoundisAttacking = false;
    bool justAttacked = false;
    
    private Vector2 newVelocity;
    EnemyController enemyController;
    [SerializeField] private float HellhoundStartupFrames;
    [SerializeField] private float HellhoundActiveFrames;
    [SerializeField] private float HellhoundRecoveryFrames;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //this is effectively the fixedupdate block
    public void HellhoundPassover()
    {
        if (!justAttacked)
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
                enemyController.animator.Play("HellhoundRun");
                HoundPatrol();

            }
            else
            {
                enemyController.animator.Play("HellhoundRun");
                HoundChase();
            }
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
    private void HoundPatrol()
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
        private void HoundChase()
    {
        if (enemyController.playerLocation.position.x >= transform.position.x)
        {
            newVelocity.Set(enemyController.patrolSpeed * 1.5f, enemyController.rb.velocity.y);
            enemyController.rb.velocity = newVelocity;
        }
        else
        {
            newVelocity.Set(-enemyController.patrolSpeed * 1.5f, enemyController.rb.velocity.y);
            enemyController.rb.velocity = newVelocity;
        }

    }

    public void PounceTrigger()
    {
        if (!justAttacked && enemyController.rb.velocity.y < 0.25f)
        {
            justAttacked = true;
            newVelocity.Set(0, enemyController.rb.velocity.y);
            enemyController.animator.Play("HellhoundCrouch");
            StartCoroutine(PounceStartup());
        }
        
    }

    IEnumerator PounceStartup()
    {
        yield return new WaitForSeconds(HellhoundStartupFrames);
        if (enemyController.damageInterupt)
        {
            enemyController.damageInterupt = false;
        }
        else
        {
            enemyController.rb.AddForce(new Vector2(2.0f * enemyController.facingDirection, 5.0f), ForceMode2D.Impulse);
            enemyController.isAttacking = true;
            enemyController.animator.Play("HellhoundAirborne");
            yield return new WaitForSeconds(HellhoundActiveFrames);
            enemyController.isAttacking = false;
            yield return new WaitForSeconds(HellhoundRecoveryFrames);
            enemyController.animator.Play("HellHoundStand");
        }
        justAttacked = false;
    }
}
