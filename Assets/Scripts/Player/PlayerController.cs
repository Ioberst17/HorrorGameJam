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


    [SerializeField] private PlayerHealth playerHealth;
    //Health points, magic points, soul points (currency)
    public int MP;
    public int SP;
    [SerializeField] private bool hasShield;

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
        EventSystem.current.onPlayerHitTrigger += Hit;
        EventSystem.current.onPlayerDeathTrigger += PlayerDeath;

        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<BoxCollider2D>();
        //HP = StartingHP;
        MP = StartingMP;
        SP = 0;
        ControlMomentum = 0;
        animator = GetComponent<Animator>();
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        visualEffects = transform.Find("VisualEffects").gameObject.GetComponent<PlayerParticleSystems>();
        attackLagTimer = 0;
        AttackDamage = 10;
        canDash = true;
        isDashing = false;
        isAttacking = false;

        if (GetComponent<PlayerHealth>() != null) { playerHealth = GetComponent<PlayerHealth>(); }
        else { Debug.Log("PlayerHealth.cs is being requested as a component of the same object as PlayerController.cs, but could not be found on the object"); }
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
        if (!playerHealth.inHitstun && !isDashing)
        {
            if (isGrounded && !isJumping) //if on ground
            {

                newVelocity.Set(movementSpeed * ControlMomentum/50, rb.velocity.y);
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
                        animator.Play("PlayerRun");
                        visualEffects.PlayEffect("MovementDust");
                    }
                }

            }
            else if (!isGrounded) //If in air
            {
                newVelocity.Set(movementSpeed * ControlMomentum/50, rb.velocity.y);
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
            newVelocity.Set(direction.x * movementSpeed * 2, direction.y * movementSpeed * 2);
            //newVelocity.Set(movementSpeed * 2 * facingDirection, 0);
            rb.velocity = newVelocity;
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
                    i++;
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
                    i++;
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
                    i++;
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
    public void Hit(Vector3 enemyPos, int damageNumber, int damageType, float damageMod, float knockbackMod)
    {
        if (!playerHealth.isInvincible)
        {
            if (enemyPos.x >= transform.position.x)
            {
                float knockbackForce = jumpForce * (1 - knockbackMod);
                newVelocity.Set(-knockbackForce / 3, 0.0f);
                rb.velocity = newVelocity;
                newForce.Set(0.0f, knockbackForce / 3);
                rb.AddForce(newForce, ForceMode2D.Impulse);
            }
            else
            {
                float knockbackForce = jumpForce * (1 - knockbackMod);
                newVelocity.Set(knockbackForce / 3, 0.0f);
                rb.velocity = newVelocity;
                newForce.Set(0.0f, knockbackForce / 3);
                rb.AddForce(newForce, ForceMode2D.Impulse);
            }
        }
    }

    void PlayerDeath()
    {
        animator.Play("PlayerDeath");
        transform.position = StartingLocation.position;
    }

    public void gainSP(int SPAmount) { SP += SPAmount; }

    private void OnDestroy()
    {
        EventSystem.current.onPlayerDeathTrigger -= PlayerDeath;
        EventSystem.current.onPlayerHitTrigger -= Hit;
    }
}
