using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using static PlayerPrimaryWeapon;

// Handles information that updates an entities state and physical movement
// e.g. Like player and enemy objects, NOT objects like GameController
public class Controller : MonoBehaviour
{
    [Header("Generic Controller Values & References")]
    private int filler; // temp: properties can't be listed under a header, so temp 'filler' field si here
    [SerializeField] protected Rigidbody2D _rb; public Rigidbody2D RB { get { return _rb; } set { _rb = value; } }
    [SerializeField] public float GroundCheckRadius { get; set; } = .15f; // default value, can be overriden 
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform wallCheck;

    [SerializeField] protected float _movementSpeed; public float MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }

    public int FacingDirection { get; set; } = 1;
    protected Vector2 oldVelocity;
    protected Vector2 _newVelocity; public Vector2 NewVelocity { get { return _newVelocity; } set { _newVelocity = value; } }
    protected Vector2 newForce;

    protected Vector3 _startingLocation; public Vector3 StartingLocation { get { return _startingLocation; } set { _startingLocation = value; } }

    // POSSIBLE STATES
    [SerializeField] protected bool _isGrounded; public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    [SerializeField] protected bool _justLanded; public bool JustLanded { get { return _justLanded; } set { _justLanded = value; } }
    [SerializeField] protected bool _isCrouching; public bool IsCrouching { get { return _isCrouching; } set { _isCrouching = value; } }
    [SerializeField] protected bool _isRunning; public bool IsRunning { get { return _isRunning; } set { _isRunning = value; } }
    [SerializeField] protected bool _isAgainstWall; public bool IsAgainstWall { get { return _isAgainstWall; } set { _isAgainstWall = value; } }
    [SerializeField] protected bool _isOnCeiling; public bool IsOnCeiling { get { return _isOnCeiling; } set { _isOnCeiling = value; } }
    [SerializeField] protected bool _isWallHanging; public bool IsWallHanging { get { return _isWallHanging; } set { _isWallHanging = value; } }
    [SerializeField] protected bool _canWallJump; public bool CanWallJump { get { return _canWallJump; } set { _canWallJump = value; } }
    [SerializeField] protected bool _isAttacking; public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    [SerializeField] protected bool _isDead; public bool IsDead { get { return _isDead; } set { _isDead = true; } }
    [SerializeField] protected bool _isInvincible; public bool IsInvincible { get { return _isInvincible; } set { _isInvincible = value; } }
    public int InvincibilityCount { get; set; }
    protected int InvincibilitySet { get; set; }
    [SerializeField] protected bool _isStunned; public bool IsStunned { get { return _isStunned; } set { _isStunned = value; } } // this is from a status effect
    [SerializeField] protected bool _inHitStun; public bool InHitStun {  get { return _inHitStun; }  set  { _inHitStun = value; } } // this is from getting hit
    protected float hitStunLength = 1.5f;
    protected float blinkFrequency = 30f; // higher, isfaster blinking for hit stun

    // STANDARD FUNCTIONS FOR OBJECTS WITH CONTROLLERS
    virtual protected void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        DefineGround();
    }

    virtual public void DefineGround() 
    {
        groundCheck = transform.Find("GroundCheck");
        if(groundCheck == null) { Debug.Log("Object " + name + "is missing a GroundCheck object in its hierarchy to tell what is ground"); }
        whatIsGround = LayerMask.GetMask("Environment"); 
    }

    // TO APPLY HIT STUN
    virtual protected void FixedUpdate()
    {
        CheckGround();
        HitStunBlink();
    }

    virtual public void HandleHitPhysics(Vector3 position, float knockbackMod) { }

    virtual public IEnumerator HitStun()
    {
        (InHitStun, IsInvincible) = (true, true);
        yield return new WaitForSeconds(hitStunLength); // waits a certain number of seconds
        (InHitStun, IsInvincible) = (false, false);
    }

    // defined and called in player and enemy child controller's, both are called in FixedUpdate
    virtual protected void HitStunBlink() 
    {
        
    }

    virtual protected IEnumerator Invincibility(float length)
    {
        IsInvincible = true;
        yield return new WaitForSeconds(length); // waits a certain number of seconds
        IsInvincible = false;
    }

    virtual public void CheckGround()
    {
        if (RB.velocity.y == 0.0f) { _isGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundCheckRadius, whatIsGround); }
        else { _isGrounded = false; }
    }

    // COMMON PHYSICS APPLICATIONS

    public void SetVelocity() { SetVelocity(0, 0); }

    public void SetVelocity(float xVel, float yVel)
    {
        NewVelocity = new Vector2(xVel, yVel);
        RB.velocity = NewVelocity;
    }

    public void AddForce(float xDirForce, float yDirForce)
    {
        newForce.Set(xDirForce, yDirForce);
        RB.AddForce(newForce, ForceMode2D.Impulse);
    }

    public void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
}
