using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBehavior : MonoBehaviour
{
    private EnemyController enemyController;
    private SpriteRenderer spriteRenderer;
    public bool OnCeiling;
    public bool isGrounded;
    public bool initialFall;
    public int attackCooldown;
    public int attackSet;
    private Vector2 newVelocity;
    [SerializeField]
    private LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        OnCeiling = true;
        spriteRenderer.flipY = true;
        enemyController.rb.gravityScale = 0;
        initialFall = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCooldown > 0)
        {
            attackCooldown--;
        }
        if (enemyController.rb.velocity.y == 0.0f)
        {
            isGrounded = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y -.5f), .15f, whatIsGround);
        }
        else
        {
            isGrounded = false;
        }
        if (initialFall && isGrounded)
        {
            initialFall = false;
            enemyController.animator.Play("SpiderLand");
            enemyController.isAttacking = false;
        }
        if(enemyController.isAttacking && isGrounded)
        {
            enemyController.isAttacking = false;
            enemyController.animator.Play("SpiderWalk");
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
    public void spiderPassover()
    {
        if (enemyController.transform.position.x >= enemyController.patrol1Point.x)
        {
            enemyController.patrolID = 1;
        }
        else if (enemyController.transform.position.x <= enemyController.patrol2Point.x)
        {
            enemyController.patrolID = 2;
        }
        if (enemyController.playerInZone)
        {
            if (OnCeiling)
            {
                OnCeiling = false;
                SpiderAttackCeiling();
                spriteRenderer.flipY = false;
                enemyController.rb.gravityScale = 1;
            }
            else if(!initialFall && isGrounded && !enemyController.isAttacking && (attackCooldown == 0))
            {
                SpiderAttackGround();
            }
        }
        else
        {
            SpiderPatrolFloor();
        }
        
    }
    private void SpiderPatrolFloor()
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
    private void SpiderAttackCeiling()
    {
        enemyController.animator.Play("SpiderFall");
        enemyController.isAttacking = true;
        initialFall = true;
    }
    private void SpiderAttackGround()
    {
        Debug.Log("Spider Attack!");
        attackCooldown = attackSet;
        enemyController.animator.Play("SpiderPounce");
        if (transform.position.x <= enemyController.playerLocation.position.x)
        {
            if(enemyController.facingDirection == -1)
            {
                enemyController.Flip();
            }
            
        }
        else
        {
            if (enemyController.facingDirection == 1)
            {
                enemyController.Flip();
            }

        }
        enemyController.rb.AddForce(new Vector2(2.0f * enemyController.facingDirection, 5.0f), ForceMode2D.Impulse);
        enemyController.isAttacking = true;
    }
}
