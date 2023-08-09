using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellhoundBehavior : EnemyAttackBehavior
{
    public bool justAttacked = false;
    
    [SerializeField] private float HellhoundStartupFrames;
    [SerializeField] private float HellhoundActiveFrames;
    [SerializeField] private float HellhoundRecoveryFrames;

    override protected void Start() 
    { 
        base.Start();
        enemyController.IsAttacking = false;
        enemyController.GroundCheckRadius = 0.05f;
    }

    // Override the base passover called in Parent Fixed Update
    override protected void Passover()
    {
        if (!justAttacked)
        {
            UpdatePatrolID();

            if (!enemyController.playerInZone)
            {
                enemyController.animator.Play("HellhoundRun");
                Patrol();
            }
            else
            {
                enemyController.animator.Play("HellhoundRun");
                Chase();
            }
        }
        if (enemyController.RB.velocity.x >= 0.5f && enemyController.FacingDirection == -1 && !enemyController.IsAttacking)
        {
            enemyController.Flip();
        }
        else if (enemyController.RB.velocity.x <= -0.5f && enemyController.FacingDirection == 1 && !enemyController.IsAttacking)
        {
            enemyController.Flip();
        }
    }

    override protected void Chase()
    {
        if (enemyController.playerLocation.position.x >= transform.position.x)
        {
            enemyController.SetVelocity(enemyController.MovementSpeed * 1.5f, enemyController.RB.velocity.y);
        }
        else { enemyController.SetVelocity(-enemyController.MovementSpeed * 1.5f, enemyController.RB.velocity.y); }

    }

    public void PounceTrigger()
    {
        if (!justAttacked && enemyController.RB.velocity.y < 0.25f)
        {
            justAttacked = true;
            enemyController.SetVelocity(0.25f * enemyController.FacingDirection, enemyController.RB.velocity.y);
            enemyController.animator.Play("HellhoundCrouch");
            StartCoroutine(PounceStartup());
        }
        
    }

    IEnumerator PounceStartup()
    {
        yield return new WaitForSeconds(HellhoundStartupFrames);
        if (enemyHealth.damageInterupt)
        {
            enemyHealth.damageInterupt = false;
        }
        else
        {
            enemyController.AddForce(6.0f * enemyController.FacingDirection, 3.0f);
            enemyController.IsAttacking = true;
            enemyController.animator.Play("HellhoundAirborne");
            yield return new WaitForSeconds(HellhoundActiveFrames);
            yield return new WaitUntil(()=> enemyController.IsGrounded);
            enemyController.IsAttacking = false;
            yield return new WaitForSeconds(HellhoundRecoveryFrames);
            enemyController.animator.Play("HellHoundStand");
        }
        justAttacked = false;
    }
}
