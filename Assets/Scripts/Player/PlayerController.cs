using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //To make the player temporarily unable to control themselves
    public bool inHitstun;

    public bool isAttacking;
    public bool isInvincible;

    //private float xInput;
    public int facingDirection = 1;
    private Vector2 oldVelocity;
    private Vector2 newVelocity;
    private Vector2 newForce;

    //used to calculate "control interta"
    public float ControlMomentum;

    private bool isGrounded;
    //private bool isOnSlope;
    private bool isJumping;
    public bool isDashing;
    //private bool canWalkOnSlope;
    private bool canJump;
    public bool isAgainstWall;
    public bool canWallJump;
    public int StartingHP;
    public int StartingMP;


    //Health points, magic points, soul points (currency)
    public int HP;
    public int MP;
    public int SP;

    //these are all related to attack information
    public int AttackDamage;
    //List of objects hit by an attack, used to let the player hit multiple things with one swing
    private Collider2D[] hitlist;
    //These are all the objects used for the physical hit detection
    [SerializeField]
    private GameObject attackHori;
    [SerializeField]
    private Transform AHPoint1;
    [SerializeField]
    private Transform AHPoint2;
    [SerializeField]
    private GameObject attackUp;
    [SerializeField]
    private Transform AUPoint1;
    [SerializeField]
    private Transform AUPoint2;
    [SerializeField]
    private GameObject attackDown;
    [SerializeField]
    private Transform ADPoint1;
    [SerializeField]
    private Transform ADPoint2;
    [SerializeField]
    private float activeFrames;
    [SerializeField]
    private float recoveryFrames;
    [SerializeField]
    private float startupFrames;

    public int attackLagValue;
    public int attackLagTimer;
    [SerializeField]
    private Transform StartingLocation;


    //Set all the initial values
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<BoxCollider2D>();
        HP = StartingHP;
        MP = StartingMP;
        SP = 0;
        ControlMomentum = 0;
        animator = GetComponent<Animator>();
        visualEffects = transform.Find("VisualEffects").gameObject.GetComponent<PlayerParticleSystems>();
        attackLagTimer = 0;
        AttackDamage = 10;
        canDash = true;
        inHitstun = false;
        isInvincible = false;
        isDashing = false;
        isAttacking = false;
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
        AttackHelper();
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
                newVelocity.Set(0.0f, 0.0f);
                rb.velocity = newVelocity;
                newForce.Set(0.0f, jumpForce);
                rb.AddForce(newForce, ForceMode2D.Impulse);
                animator.Play("PlayerJump");
                { // play random jump sound
                    int jumpAssetChoice = Random.Range(1, 9);
                    string jumpAssetToUse = "Jump" + jumpAssetChoice.ToString();
                    FindObjectOfType<AudioManager>().PlaySFX(jumpAssetToUse);
                }

            }
            else if (isAgainstWall && canWallJump)
            {
                ControlMomentum = 30 * -facingDirection;
                Flip();
                canWallJump = false;
                isJumping = true;
                newVelocity.Set(0.0f, 0.0f);
                rb.velocity = newVelocity;
                newForce.Set(jumpForce / 2 * -facingDirection, jumpForce);
                rb.AddForce(newForce, ForceMode2D.Impulse);
                animator.Play("PlayerJump");
                { // play random jump sound
                    int jumpAssetChoice = Random.Range(1, 9);
                    string jumpAssetToUse = "Jump" + jumpAssetChoice.ToString();
                    FindObjectOfType<AudioManager>().PlaySFX(jumpAssetToUse);
                }
                visualEffects.PlayEffect("MovementDust");
            }
        }
        
    }

    //called by the GameController
    public void ApplyMovement()
    {
        if (!inHitstun && !isDashing)
        {
            if (isGrounded && !isJumping) //if on ground
            {

                newVelocity.Set(movementSpeed * ControlMomentum/15, rb.velocity.y);
                rb.velocity = newVelocity;
                if(!isAttacking && !isJumping)
                {
                    if (rb.velocity.x == 0 || DialogueManager.GetInstance().dialogueIsPlaying)
                    {
                        newVelocity.Set(0, 0);
                        rb.velocity = newVelocity;
                        if(GameController.xInput == 0)
                        {
                            animator.Play("PlayerIdle");
                        }
                        
                    }
                    else
                    {
                        //Debug.Log("running");
                        animator.Play("PlayerRun");
                        visualEffects.PlayEffect("MovementDust");
                    }
                }

            }
            else if (!isGrounded) //If in air
            {
                newVelocity.Set(movementSpeed * ControlMomentum/15, rb.velocity.y);
                rb.velocity = newVelocity;
            }
        }
    }

    //0 is up, 1 is down, 2 is neutral
    public void Attack(int attackDirection)
    {
        //Debug.Log("attack called 2");
        if (!isAttacking && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            isAttacking = true;
            animator.Play("PlayerBasicAttack");
            FindObjectOfType<AudioManager>().PlaySFX("PlayerMelee");
            StartCoroutine(AttackActiveFrames(attackDirection));
        }
    }

    //controlls the attack frame data
    IEnumerator AttackActiveFrames(int attackDirection) // is called by the trigger event for powerups to countdown how long the power lasts
    {
        switch (attackDirection)
        {
            case 0:
                yield return new WaitForSeconds(startupFrames);
                attackUp.SetActive(true);
                yield return new WaitForSeconds(activeFrames); // waits a certain number of seconds
                attackUp.SetActive(false);
                yield return new WaitForSeconds(recoveryFrames);
                isAttacking = false;
                break;
            case 1:
                yield return new WaitForSeconds(startupFrames);
                attackDown.SetActive(true);
                yield return new WaitForSeconds(activeFrames); // waits a certain number of seconds
                attackDown.SetActive(false);
                yield return new WaitForSeconds(recoveryFrames);
                isAttacking = false;
                break;
            case 2:
                yield return new WaitForSeconds(startupFrames);
                attackHori.SetActive(true);
                yield return new WaitForSeconds(activeFrames); // waits a certain number of seconds
                attackHori.SetActive(false);
                yield return new WaitForSeconds(recoveryFrames);
                isAttacking = false;
                break;
            default:
                isAttacking = false;
                break;
        }
    }

    //This is the function that actually performs the dash
    IEnumerator DashHandler()
    {
        isDashing = true;
        rb.gravityScale = 0;
        if(GameController.xInput == 0)
        {
            newVelocity.Set(movementSpeed * 2 * facingDirection, 0);
            rb.velocity = newVelocity;
        }
        else
        {
            if(GameController.xInput > 0)
            {
                newVelocity.Set(movementSpeed * 2, 0);
                rb.velocity = newVelocity;
            }
            else
            {
                newVelocity.Set(movementSpeed * -2, 0);
                rb.velocity = newVelocity;
            }
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

    //This opperates the attack hit detection
    public void AttackHelper()
    {
        //normal attack
        if (attackHori.activeSelf)
        {
            //Debug.Log("Swinging");
            

            if (Physics2D.OverlapArea(AHPoint1.position, AHPoint2.position, whatIsEnemy))
            {
                hitlist = Physics2D.OverlapAreaAll(AHPoint1.position, AHPoint2.position, whatIsEnemy);
                int i = 0;
                while (i < hitlist.Length)
                {
                    //Debug.Log(hitlist[i]);
                    if (hitlist[i].GetType() == typeof(UnityEngine.CapsuleCollider2D))
                    {
                        GameController.passHit(hitlist[i].name, AttackDamage, transform.position);
                    }
                    ++i;
                }

            }
        }
        //up attack
        if (attackUp.activeSelf)
        {
            //Debug.Log("Swinging");
            //animator.Play("PlayUp");

            if (Physics2D.OverlapArea(AUPoint1.position, AUPoint2.position, whatIsEnemy))
            {
                hitlist = Physics2D.OverlapAreaAll(AUPoint1.position, AUPoint2.position, whatIsEnemy);
                int i = 0;
                while (i < hitlist.Length)
                {
                    //Debug.Log(hitlist[i]);
                    if (hitlist[i].GetType() == typeof(UnityEngine.CapsuleCollider2D))
                    {
                        GameController.passHit(hitlist[i].name, AttackDamage, transform.position);
                    }
                    ++i;
                }

            }
        }
        //down attack
        if (attackDown.activeSelf)
        {
            //Debug.Log("Swinging");
            //animator.Play("PlayerAttackDown");

            if (Physics2D.OverlapArea(ADPoint1.position, ADPoint2.position, whatIsEnemy))
            {
                newVelocity.Set(0.0f, 0.0f);
                rb.velocity = newVelocity;
                newForce.Set(0.0f, jumpForce * 0.66f);
                rb.AddForce(newForce, ForceMode2D.Impulse);

                hitlist = Physics2D.OverlapAreaAll(ADPoint1.position, ADPoint2.position, whatIsEnemy);
                int i = 0;
                while (i < hitlist.Length)
                {
                    //Debug.Log(hitlist[i]);
                    if (hitlist[i].GetType() == typeof(UnityEngine.CapsuleCollider2D))
                    {
                        GameController.passHit(hitlist[i].name, AttackDamage, transform.position);
                    }
                    ++i;
                }

            }
        }
    }

   

    //flips the model
    public void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    

    //processes if the player should take damage, and if so, how much, then calculates for death. damageType Numbers: 0 is one hit damage, 1 is damage over time. 
    //Calculated direction of hit for knockback direction.
    public void takeDamage(Vector3 enemyPos, int damageNumber, int damageType)
    {
        if (!isInvincible)
        {
            StartCoroutine(hitStun());
            HP -= damageNumber;
            if (enemyPos.x >= transform.position.x)
            {
                newVelocity.Set(-5.0f, 0.0f);
                rb.velocity = newVelocity;
                newForce.Set(0.0f, jumpForce);
                rb.AddForce(newForce, ForceMode2D.Impulse);
            }
            else
            {
                newVelocity.Set(5.0f, 0.0f);
                rb.velocity = newVelocity;
                newForce.Set(0.0f, jumpForce);
                rb.AddForce(newForce, ForceMode2D.Impulse);
            }
            if (HP <= 0)
            {
                Debug.Log("Death");
                animator.Play("PlayerDeath");
                transform.position = StartingLocation.position;
                HP = StartingHP;
            }
        }
    }

    IEnumerator hitStun()
    {
        inHitstun = true;
        StartCoroutine(Invincibility());
        animator.Play("PlayerHurt");
        FindObjectOfType<AudioManager>().PlaySFX("PlayerHit");
        yield return new WaitForSeconds(1); // waits a certain number of seconds
        inHitstun = false;
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        //animator.Play("PlayerHit");
        yield return new WaitForSeconds(1.5f); // waits a certain number of seconds
        isInvincible = false;
    }

    public void gainSP(int SPAmount)
    {
        SP += SPAmount;
    }
}
