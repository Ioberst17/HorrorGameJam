using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using static PlayerAttackManager;
/// <summary>
/// Handles information that updates an entities state and physical movement 
/// e.g. Like player and enemy objects, NOT objects like GameController
/// </summary>
public class Controller : MonoBehaviour
{
    [Header("Generic Controller Values & References")]
    private int filler; // temp: properties can't be listed under a header, so temp 'filler' field si here
    [SerializeField] protected Rigidbody2D _rb; public Rigidbody2D RB { get { return _rb; } set { _rb = value; } }
    [SerializeField] protected Collider2D _hitBox; public Collider2D HitBox { get { return _hitBox; } set { _hitBox = value; } }
    [SerializeField] protected float _groundCheckRadius = .15f; public float GroundCheckRadius { get { return _groundCheckRadius; } set { _groundCheckRadius = value; } } // default value, can be overriden 
    protected Transform _groundCheck; public Transform GroundCheck { get { return _groundCheck; } set { _groundCheck = value; } }
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform wallCheck;

    [SerializeField] protected float _movementSpeed; public float MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }

    public int FacingDirection { get; set; } = 1;
    protected Vector2 oldVelocity;
    // used to load in a new velocity
    protected Vector2 _newVelocity; public Vector2 NewVelocity { get { return _newVelocity; } set { _newVelocity = value; } }
    protected float _newVelocityX;
    protected float _newVelocityY;
    protected Vector2 newForce;

    protected Vector3 _startingLocation; public Vector3 StartingLocation { get { return _startingLocation; } set { _startingLocation = value; } }

    // POSSIBLE STATES
    [SerializeField] protected bool _isGrounded; virtual public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    [SerializeField] protected bool _justLanded; virtual public bool JustLanded { get { return _justLanded; } set { _justLanded = value; } }
    [SerializeField] protected bool _isCrouching; virtual public bool IsCrouching { get { return _isCrouching; } set { _isCrouching = value; } }
    [SerializeField] protected bool _isRunning; virtual public bool IsRunning { get { return _isRunning; } set { _isRunning = value; } }
    [SerializeField] protected bool _isAgainstWall; virtual public bool IsAgainstWall { get { return _isAgainstWall; } set { _isAgainstWall = value; } }
    [SerializeField] protected bool _isOnCeiling; virtual public bool IsOnCeiling { get { return _isOnCeiling; } set { _isOnCeiling = value; } }
    [SerializeField] protected bool _isWallHanging; virtual public bool IsWallHanging { get { return _isWallHanging; } set { _isWallHanging = value; } }
    [SerializeField] protected bool _canWallJump; virtual public bool CanWallJump { get { return _canWallJump; } set { _canWallJump = value; } }
    [SerializeField] protected bool _isAttacking; virtual public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    [SerializeField] protected bool _isChargingAttack; virtual public bool IsChargingAttack { get { return _isChargingAttack; } set { _isChargingAttack = value; } }
    virtual public bool IsAttackingOrChargingAttack { get { return (IsAttacking || IsChargingAttack); } }
    [SerializeField] protected bool _isShooting; virtual public bool IsShooting { get { return _isShooting; } set { _isShooting = value; } }
    [SerializeField] protected bool _isDead; virtual public bool IsDead { get { return _isDead; } set { _isDead = true; } }
    [SerializeField] protected bool _isInvincible; virtual public bool IsInvincible { get { return _isInvincible; } set { _isInvincible = value; } }
    public int InvincibilityCount { get; set; }
    protected int InvincibilitySet { get; set; }
    [SerializeField] protected bool _isStunned; virtual public bool IsStunned { get { return _isStunned; } set { _isStunned = value; } } // this is from a status effect
    [SerializeField] protected bool _inHitStun; virtual public bool InHitStun {  get { return _inHitStun; }  set  { _inHitStun = value; } } // this is from getting hit
    protected float HitStunLength { get; set; } = 1f;
    protected float blinkFrequency = 30f; // higher, isfaster blinking for hit stun

    [SerializeField] protected bool _canMove; virtual public bool CanMove { get { return _canMove; } set { _canMove = value; } }
    [SerializeField] protected bool _canMoveX; virtual public bool CanMoveX { get { return _canMoveX; } set { _canMoveX = value; } }
    [SerializeField] protected bool _canMoveY; virtual public bool CanMoveY { get { return _canMoveY; } set { _canMoveY = value; } }

    // STANDARD FUNCTIONS FOR OBJECTS WITH CONTROLLERS
    virtual protected void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        HitBox = GetComponent<Collider2D>();
        (CanMove, CanMoveX, CanMoveY) = (true, true, true);
        DefineGround();
    }

    virtual public void DefineGround() 
    {
        GroundCheck = transform.Find("GroundCheck");
        if(GroundCheck == null) { Debug.Log("Object " + name + "is missing a GroundCheck object in its hierarchy to tell what is ground"); }
        whatIsGround = LayerMask.GetMask("Environment"); 
    }

    // TO APPLY HIT STUN
    virtual protected void FixedUpdate()
    {
        CheckGround();
        HitStunBlink();
    }

    virtual public void HandleHitPhysics(Vector3 position, float knockbackMod) { }


    /// <summary>
    /// Triggers invincibilility and hitstun state for player
    /// </summary>
    /// <returns></returns>
    virtual public IEnumerator HitStun()
    {
        (InHitStun, IsInvincible) = (true, true);
        yield return new WaitForSeconds(HitStunLength); // waits a certain number of seconds
        (InHitStun, IsInvincible) = (false, false);
    }

    /// <summary>
    /// Defined and called in player and enemy child controller's, both are called in FixedUpdate; toggled by the HitStun coroutine
    /// </summary>
    virtual protected void HitStunBlink() 
    {
        
    }

    virtual public void CheckGround()
    {
        if (RB.velocity.y == 0.0f) { _isGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, whatIsGround); }
        else { _isGrounded = false; }
    }

    // COMMON PHYSICS APPLICATIONS
    /// <summary>
    /// Used to set velocity of a controller to 0
    /// </summary>
    public void SetVelocity() { SetVelocity(0, 0); }
    /// <summary>
    /// Used to set velocity; if a float value is set to null, then the existing velocity will be used
    /// </summary>
    /// <param name="xVel"></param>
    /// <param name="yVel"></param>
    public void SetVelocity(float? xVel, float? yVel) 
    {
        if (xVel == null) { xVel = RB.velocity.x; }
        if (yVel == null) { yVel = RB.velocity.y; }

        VelocitySetter(xVel, yVel);
    }

    private void VelocitySetter(float? xVel, float? yVel)
    {
        if (!CanMoveX) { xVel = 0; }
        if(!CanMoveY) { yVel = 0; }
        if(!CanMove) { xVel = 0;yVel = 0; }

        _newVelocityX = xVel.GetValueOrDefault();
        _newVelocityY = yVel.GetValueOrDefault();

        NewVelocity = new Vector2(_newVelocityX, _newVelocityY);
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

    public void SetGravityScale(float gravityScale)
    {
        RB.gravityScale = gravityScale;
    }

    public void StatusModifier(string mod)
    {
        Debug.Log("Applying a status effect named: " + mod + " to: " + gameObject.name);
        if (mod == "DemonBlood") { if (GetComponentInChildren<Poisoned>() != null) { GetComponentInChildren<Poisoned>().Execute(); } }
        else if (mod == "Burn") { if (GetComponentInChildren<Burnable>() != null) { GetComponentInChildren<Burnable>().Execute(); } }
        else if (mod == "Freeze") { if (GetComponentInChildren<Frozen>() != null) { GetComponentInChildren<Frozen>().Execute(); } }
        else if (mod == "Stunned") { if (GetComponentInChildren<Stunned>() != null) { GetComponentInChildren<Stunned>().Execute(); } }
    }
}
