using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBehaviour : EnemyBehaviour
{
    public int attackSet;
    public int attackCooldown;
    private int spiderCooldown;

    override protected void Start()
    {
        base.Start();

        enemyController.IsAttacking = false;
        enemyController.IsOnCeiling = true;
        enemyController.SpriteRenderer.flipY = true;
        enemyController.SetGravityScale(0);
        enemyController.GroundCheckRadius = 0.15f;
    }

    override protected void Passover()
    {
        // cool downs
        if (attackCooldown > 0) { attackCooldown--; }
        if (spiderCooldown > 0) { spiderCooldown--; }

        UpdatePatrolID();

        Flip();

        // behaviors
        if (enemyController.IsGrounded)
        {
            if (enemyController.JustLanded)
            {
                enemyController.JustLanded = false;
                enemyController.animator.Play("SpiderLand");
                enemyController.IsAttacking = false;
            }
            if (enemyController.IsAttacking)
            {
                enemyController.IsAttacking = false;
                enemyController.animator.Play("SpiderWalk");
                spiderCooldown = 8;
            }
        }

        if (enemyController.PlayerInZone)
        {
            if (enemyController.IsOnCeiling)
            {
                enemyController.IsOnCeiling = false;
                CeilingAttack();
                enemyController.SpriteRenderer.flipY = false;
                enemyController.SetGravityScale(1);
            }
            else if(!enemyController.JustLanded && enemyController.IsGrounded && !enemyController.IsAttacking) { StartCoroutine(GroundAttack()); }
        }
        else if (!enemyController.IsAttacking) { Patrol(); }
        
    }

    private void CeilingAttack()
    {
        enemyController.animator.Play("SpiderFall");
        enemyController.IsAttacking = true;
        enemyController.JustLanded = true;
    }
    IEnumerator GroundAttack()
    {
        Debug.Log("Spider Attack!");
        attackCooldown = attackSet;

        FlipToFacePlayer();

        enemyController.animator.Play("SpiderCrouch");
        yield return new WaitForSeconds(0.50f);
        if (enemyHealth.DamageInterrupt)
        {
            enemyController.IsAttacking = false;
            enemyHealth.DamageInterrupt = false;
        }
        else
        {
            enemyController.animator.Play("SpiderPounce");
            enemyController.SetVelocity();
            enemyController.AddForce(4.0f * enemyController.FacingDirection, 1.0f);
            enemyController.IsAttacking = true;
        }
        yield return new WaitForSeconds(0.01f);
    }

    override protected void Flip()
    {
        // Flip logic
        if (enemyController.RB.velocity.x >= 0.5f && enemyController.FacingDirection == -1 && !enemyController.IsAttacking && enemyController.IsGrounded)
        {
            if (flipCoolDown == 0)
            {
                enemyController.Flip();
                flipCoolDown = 10;
            }
        }
        else if (enemyController.RB.velocity.x <= -0.5f && enemyController.FacingDirection == 1 && !enemyController.IsAttacking && enemyController.IsGrounded)
        {
            if (flipCoolDown == 0)
            {
                enemyController.Flip();
                flipCoolDown = 10;
            }
        }
    }
}
