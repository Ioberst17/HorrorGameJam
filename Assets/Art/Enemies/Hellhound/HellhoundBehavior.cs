using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellhoundBehavior : MonoBehaviour
{
    public bool justAttacked = false;
    
    private Vector2 newVelocity;
    EnemyController enemyController;
    [SerializeField] private float HellhoundStartupFrames;
    [SerializeField] private float HellhoundActiveFrames;
    [SerializeField] private float HellhoundRecoveryFrames;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private Transform groundCheck;
    public bool isGrounded;

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
        if (enemyController.rb.velocity.y == 0.0f)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

            //Debug.Log("isgrounded " + rb.velocity.y);
        }
        else
        {
            isGrounded = false;
        }
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
        if (enemyController.rb.velocity.x >= 0.5f && enemyController.facingDirection == -1 && !enemyController.isAttacking)
        {
            enemyController.Flip();
        }
        else if (enemyController.rb.velocity.x <= -0.5f && enemyController.facingDirection == 1 && !enemyController.isAttacking)
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
            newVelocity.Set(0.25f * enemyController.facingDirection, enemyController.rb.velocity.y);
            enemyController.rb.velocity = newVelocity;
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
            enemyController.rb.AddForce(new Vector2(6.0f * enemyController.facingDirection, 3.0f), ForceMode2D.Impulse);
            enemyController.isAttacking = true;
            enemyController.animator.Play("HellhoundAirborne");
            yield return new WaitForSeconds(HellhoundActiveFrames);
            yield return new WaitUntil(()=>isGrounded);
            enemyController.isAttacking = false;
            yield return new WaitForSeconds(HellhoundRecoveryFrames);
            enemyController.animator.Play("HellHoundStand");
        }
        justAttacked = false;
    }
}
