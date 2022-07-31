using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    private float xInput;
    private int facingDirection = 1;
    private Vector2 oldVelocity;
    private Vector2 newVelocity;
    private Vector2 newForce;

    private bool isGrounded;
    //private bool isOnSlope;
    private bool isJumping;
    //private bool canWalkOnSlope;
    private bool canJump;
    //private bool isAgainstWall;

    public int HP = 100;
    public int MP = 100;

    private Rigidbody2D rb;
    private BoxCollider2D cc;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<BoxCollider2D>();
        HP = 100;
        MP = 100;
    }
    private void Update()
    {
        CheckInput();
    }

    private void FixedUpdate()
    {
        //SlopeCheck();
        CheckGround();

        ApplyMovement();
    }
    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        Debug.Log(xInput);

        if (xInput == 1 && facingDirection == -1)
        {
            Flip();
        }
        else if (xInput == -1 && facingDirection == 1)
        {
            Flip();
        }

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown("up") || Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

    }
    private void CheckGround()
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
    
    private void Jump()
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

    private void ApplyMovement()
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
            newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
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
            newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
            rb.velocity = newVelocity;
        }

    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

}
