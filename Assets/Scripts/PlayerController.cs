using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameController GameController;
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float numberOfJumps;
    [SerializeField]
    private float groundCheckRadius;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float slopeCheckDistance;
    [SerializeField]
    private float maxSlopeAngle;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private Transform cameraFocus;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private LayerMask whatIsEnemy;



    //private float xInput;
    public int facingDirection = 1;
    private Vector2 oldVelocity;
    private Vector2 newVelocity;
    private Vector2 newForce;

    private bool isGrounded;
    //private bool isOnSlope;
    private bool isJumping;
    //private bool canWalkOnSlope;
    private bool canJump;
    //private bool isAgainstWall;
    public int StartingHP;
    public int StartingMP;

    public int HP;
    public int MP;
    public int AttackDamage;

    private Collider2D[] hitlist;

    [SerializeField]
    private GameObject attackHori;
    [SerializeField]
    private Transform AHPoint1;
    [SerializeField]
    private Transform AHPoint2;
    [SerializeField]
    private GameObject attackUp;
    [SerializeField]
    private GameObject attackDown;
    [SerializeField]
    private float activeFrames;
    public int attackLagValue;
    private int attackLagTimer;

    private Rigidbody2D rb;
    private BoxCollider2D cc;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<BoxCollider2D>();
        HP = StartingHP;
        MP = StartingMP;
        attackLagTimer = attackLagValue;
        AttackDamage = 5;
}
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(attackLagTimer > 0)
        {
            attackLagTimer -= 1;
        }
        if (attackHori.activeSelf)
        {
            //Debug.Log("Swinging");

            if (Physics2D.OverlapArea(AHPoint1.position, AHPoint2.position, whatIsEnemy))
            {
                hitlist = Physics2D.OverlapAreaAll(AHPoint1.position, AHPoint2.position, whatIsEnemy);
                int i = 0;
                while(i < hitlist.Length)
                {
                    //Debug.Log(hitlist[i]);
                    if (hitlist[i].GetType() == typeof(UnityEngine.BoxCollider2D))
                    {
                        GameController.passHit(hitlist[i].name, AttackDamage);
                    }
                    i++;
                }
                
            }
        }
    }

    public void CheckGround()
    {
        if (rb.velocity.y == 0.0f)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            //Debug.Log("isgrounded " + rb.velocity.y);
        }
        else
        {
            isGrounded = false;
            //Debug.Log("isinair " + rb.velocity.y);
        }

        if (rb.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if (isGrounded && !isJumping)
        {
            canJump = true;
        }

    }
    
    public void Jump()
    {
        if (canJump)
        {
            canJump = false;
            isJumping = true;
            newVelocity.Set(0.0f, 0.0f);
            rb.velocity = newVelocity;
            newForce.Set(0.0f, jumpForce);
            rb.AddForce(newForce, ForceMode2D.Impulse);
        }
    }

    public void ApplyMovement()
    {
        if (isGrounded && !isJumping) //if on ground
        {
            
            newVelocity.Set(movementSpeed * GameController.xInput, rb.velocity.y);
            rb.velocity = newVelocity;

        }
        else if (!isGrounded) //If in air
        {

           
            newVelocity.Set(movementSpeed * GameController.xInput, rb.velocity.y);
            rb.velocity = newVelocity;
        }

    }

    //0 is up, 1 is down, 2 is neutral
    public void Attack(int attackDirection)
    {
        //Debug.Log("attack called 2");
        if (attackLagTimer == 0)
        {
            StartCoroutine(AttackActiveFrames(attackDirection));
        }
    }

    IEnumerator AttackActiveFrames(int attackDirection) // is called by the trigger event for powerups to countdown how long the power lasts
    {
        switch (attackDirection)
        {
            case 0:
                attackUp.SetActive(true);
                yield return new WaitForSeconds(activeFrames); // waits a certain number of seconds
                attackUp.SetActive(false);
                break;
            case 1:
                attackDown.SetActive(true);
                yield return new WaitForSeconds(activeFrames); // waits a certain number of seconds
                attackDown.SetActive(false);
                break;
            case 2:
                Debug.Log("Neutral Swing");
                attackHori.SetActive(true);
                yield return new WaitForSeconds(activeFrames); // waits a certain number of seconds
                attackHori.SetActive(false);
                break;
        }
    }

    public void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    //processes if the player should take damage, and if so, how much, then calculates for death. damageType Numbers: 0 is one hit damage, 1 is damage over time.
    public void takeDamage(int damageNumber, int damageType)
    {
        HP -= damageNumber;
        if(HP <= 0)
        {
            Debug.Log("Death");
        }
    }
}
