using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    //varous different objects and values to be set
    private Rigidbody2D rb;
    private BoxCollider2D cc;
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
    private Transform wallCheck;
    [SerializeField]
    private Transform cameraFocus;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private LayerMask whatIsEnemy;

    private Animator animator;
    private PlayerParticleSystems visualEffects;

    //all related to dash functionality and tracking. 
    public bool canDash;
    public float dashLength;
    public float dashSpeed;
    public Camera mainCam;

    //To make the player temporarily unable to control themselves

    public bool isAttacking;


    //private float xInput;
    public int facingDirection = 1;
    private Vector2 oldVelocity;
    private Vector2 newVelocity;
    private Vector2 newForce;

    //used to calculate "control interta"
    public float ControlMomentum;

    public bool isGrounded;
    //private bool isOnSlope;
    private bool isJumping;
    public bool isDashing;
    //private bool canWalkOnSlope;
    private bool canJump;
    public bool isAgainstWall;
    public bool canWallJump;
    public int StartingMP;


    [SerializeField] private PlayerHealth playerHealth;
    //Health points, magic points, soul points (currency)
    public int MP;
    public int SP;
    [SerializeField] private bool hasShield;

    [SerializeField]
    private Transform StartingLocation;


    //Set all the initial values
    private void Start()
    {
        EventSystem.current.onPlayerHitTrigger += Hit;
        EventSystem.current.onPlayerDeathTrigger += PlayerDeath;

        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<BoxCollider2D>();
        MP = StartingMP;
        SP = 0;
        ControlMomentum = 0;
        animator = GetComponent<Animator>();
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        visualEffects = transform.Find("VisualEffects").gameObject.GetComponent<PlayerParticleSystems>();
        canDash = true;
        isDashing = false;

        if (GetComponent<PlayerHealth>() != null) { playerHealth = GetComponent<PlayerHealth>(); }
        else { Debug.Log("PlayerHealth.cs is being requested as a component of the same object as PlayerController.cs, but could not be found on the object"); }
    }

    //Does anything in the environment layer overlap with the circle while not on the way up
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
            if (!isJumping)
            {
                animator.Play("PlayerFall");
            }
            //Debug.Log("isinair " + rb.velocity.y);
        }

        if (rb.velocity.y <= 0.0f)
        {
            isJumping = false;
        }
        else
        {
            isJumping = true;
            animator.Play("PlayerJump");
        }

        if (isGrounded && !isJumping)
        {
            canJump = true;
            canWallJump = true;

            if (!isDashing)
            {
                canDash = true;
            }
        }
        GameController.isGrounded = isGrounded;
    }

    
    //dash handling function
    public void Dash()
    {
        if (canDash && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            canDash = false;
            StartCoroutine(DashHandler());
        }
    }

    public void CheckWall()
    {
        isAgainstWall = Physics2D.OverlapCircle(wallCheck.position, groundCheckRadius, whatIsGround);
    }

    //Jump handling function. Negates previous momentum on jump
    public void Jump()
    {
        if (!isDashing && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (canJump)
            {
                canJump = false;
                isJumping = true;
                SetVelocity();
                AddForce(0f, jumpForce);
                animator.Play("PlayerJump");
                PlayRandomJumpSound();
            }
            else if (isAgainstWall && canWallJump)
            {
                ControlMomentum = 20 * -facingDirection;
                Flip();
                canWallJump = false;
                isJumping = true;
                SetVelocity();
                AddForce(jumpForce / 2 * -facingDirection, jumpForce);
                PlayRandomJumpSound();
                animator.Play("PlayerJump");
                visualEffects.PlayEffect("MovementDust");
            }
        }
        
    }

    //called by the GameController
    public void ApplyMovement()
    {
        if (!playerHealth.inHitstun && !isDashing)
        {
            if (isGrounded && !isJumping) //if on ground
            {

                newVelocity.Set(movementSpeed * ControlMomentum/10, rb.velocity.y);
                rb.velocity = newVelocity;
                if(!isAttacking && !isJumping)
                {
                    if (rb.velocity.x == 0 || DialogueManager.GetInstance().dialogueIsPlaying)
                    {
                        SetVelocity();
                        if(GameController.xInput == 0)
                        {
                            animator.Play("PlayerIdle");
                        }
                    }
                    else
                    {
                        animator.Play("PlayerRun");
                        visualEffects.PlayEffect("MovementDust");
                    }
                }

            }
            else if (!isGrounded) //If in air
            {
                newVelocity.Set(movementSpeed * ControlMomentum/10, rb.velocity.y);
                rb.velocity = newVelocity;
            }
        }
    }
    private Vector3 GetNormalizedMouseDirectionFromPlayer()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerPos = mainCam.GetComponent<Camera>().WorldToScreenPoint(transform.position);
        Vector3 direction = mousePos - playerPos;
        direction = direction.normalized;
        return direction;
    }


    //This is the function that actually performs the dash
    IEnumerator DashHandler()
    {
        isDashing = true;
        rb.gravityScale = 0;
        if (GameController.xInput == 0 & GameController.yInput == 0)
        {
            Vector3 direction = GetNormalizedMouseDirectionFromPlayer();
            SetVelocity(direction.x * movementSpeed * 2, direction.y * movementSpeed * 2);
            //newVelocity.Set(movementSpeed * 2 * facingDirection, 0);
        }
        else
        {
            float inputPositive = movementSpeed * 2;
            float inputNegative = movementSpeed * -2;

            if (GameController.xInput > 0 && GameController.yInput > 0) { newVelocity.Set(inputPositive, inputPositive);}
            else if (GameController.xInput > 0 && GameController.yInput < 0) { newVelocity.Set(inputPositive, inputNegative); }
            else if (GameController.xInput < 0 && GameController.yInput > 0) { newVelocity.Set(inputNegative, inputPositive); }
            else if (GameController.xInput < 0 && GameController.yInput < 0) { newVelocity.Set(inputNegative, inputNegative); }
            else if (GameController.xInput > 0) { newVelocity.Set(inputPositive, 0); }
            else if (GameController.xInput < 0) { newVelocity.Set(inputNegative, 0); }
            else if (GameController.yInput > 0) { newVelocity.Set(0, inputPositive); }
            else if (GameController.yInput < 0) { newVelocity.Set(0, inputNegative); }

            rb.velocity = newVelocity;
        }
        FindObjectOfType<AudioManager>().PlaySFX("Dash1");
        //animator.Play("PlayerDash");
        yield return DashAfterImageHandler();
        isDashing = false;
        rb.gravityScale = 3;
        newVelocity.Set(0, 0);
        rb.velocity = newVelocity;
    }
    // Handles generation of after images and dash length
    IEnumerator DashAfterImageHandler()
    {
        var startTime = Time.time;
        {
            while(dashLength > Time.time - startTime)
            {
                PlayerAfterImageObjectPool.Instance.PlaceAfterImage(gameObject.transform);
                yield return null;
            }
        }
        yield return null;
    }

    //flips the model
    public void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    //processes if the player should take damage, and if so, how much, then calculates for death. damageType Numbers: 0 is one hit damage, 1 is damage over time. 
    //Calculated direction of hit for knockback direction.
    public void Hit(Vector3 enemyPos, int damageNumber, int damageType, float damageMod, float knockbackMod)
    {
        if (!playerHealth.isInvincible)
        {
            float knockbackForce = jumpForce * (1 - knockbackMod);

            if (enemyPos.x >= transform.position.x) { SetVelocity(-knockbackForce / 3, 0.0f); }
            else { SetVelocity(knockbackForce / 3, 0.0f); }

            AddForce(0.0f, knockbackForce / 3);
        }
    }

    void PlayerDeath()
    {
        animator.Play("PlayerDeath");
        transform.position = StartingLocation.position;
    }

    public void gainSP(int SPAmount) { SP += SPAmount; }

    public void SetVelocity() { SetVelocity(0, 0); }

    public void SetVelocity(float xVel, float yVel)
    {
        newVelocity.Set(xVel, yVel);
        rb.velocity = newVelocity;
    }

    public void AddForce(float xDirForce, float yDirForce)
    {
        newForce.Set(xDirForce, yDirForce);
        rb.AddForce(newForce, ForceMode2D.Impulse);
    }

    private void PlayRandomJumpSound()
    {
        int jumpAssetChoice = Random.Range(1, 9);
        string jumpAssetToUse = "Jump" + jumpAssetChoice.ToString();
        FindObjectOfType<AudioManager>().PlaySFX(jumpAssetToUse);
    }

    private void OnDestroy()
    {
        EventSystem.current.onPlayerDeathTrigger -= PlayerDeath;
        EventSystem.current.onPlayerHitTrigger -= Hit;
    }
}
