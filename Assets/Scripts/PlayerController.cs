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

    private Rigidbody2D rb;
    private BoxCollider2D cc;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<BoxCollider2D>();
        HP = StartingHP;
        MP = StartingMP;
    }
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //SlopeCheck();
        //CheckGround();

        //ApplyMovement();
    }
    //public void CheckInput()
    //{
    //    xInput = Input.GetAxisRaw("Horizontal");
    //    Debug.Log(xInput);

    //    if (xInput == 1 && facingDirection == -1)
    //    {
    //        Flip();
    //    }
    //    else if (xInput == -1 && facingDirection == 1)
    //    {
    //        Flip();
    //    }

    //    if (Input.GetButtonDown("Jump") || Input.GetKeyDown("up") || Input.GetKeyDown(KeyCode.W))
    //    {
    //        Jump();
    //    }

    //}
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
            //Debug.Log("This one");
            //float newSpeed = rb.velocity.x;
            //if (newSpeed >= (movementSpeed * xInput) || newSpeed <= -(movementSpeed * xInput))
            //{
            //    newVelocity.Set(movementSpeed * xInput, 0.0f);
            //    rb.velocity = newVelocity;
            //}
            //else
            //{
            //    newVelocity.Set(newSpeed + ((movementSpeed * xInput) / 10), rb.velocity.y);
            //    rb.velocity = newVelocity;
            //}
            newVelocity.Set(movementSpeed * GameController.xInput, rb.velocity.y);
            rb.velocity = newVelocity;

        }
        else if (!isGrounded) //If in air
        {

            //float newSpeed = rb.velocity.x;
            //if (newSpeed >= movementSpeed * xInput || newSpeed <= -(movementSpeed * xInput))
            //{
            //    newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
            //    rb.velocity = newVelocity;
            //}
            //else
            //{
            //    newVelocity.Set(newSpeed + ((movementSpeed * xInput) / 20), rb.velocity.y);
            //    rb.velocity = newVelocity;
            //}
            newVelocity.Set(movementSpeed * GameController.xInput, rb.velocity.y);
            rb.velocity = newVelocity;
        }

    }

    public void Attack()
    {

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
