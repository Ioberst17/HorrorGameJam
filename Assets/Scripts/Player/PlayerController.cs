using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
//using UnityEditorInternal;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    //varous different objects and values to be set
    public Rigidbody2D Rb { get; set; }
    private BoxCollider2D cc;
    [SerializeField] private GameController gameController;
    [SerializeField] private float _movementSpeed; public float MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform cameraFocus;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsEnemy;

    private PlayerAnimator animator;
    public PlayerVisualEffectsController visualEffects;
    private PlayerPrimaryWeapon playerPrimaryWeapon;
    private PlayerShield playerShield;

    // other ability references
    PlayerDash playerDash;
    [SerializeField] PlayerJump playerJump;

    // alt idle animations
    public float thresholdToTransitionToIdle = 5;
    public float thresHoldToLoopAlternativeIdleAnimation = 5.5f;

    //To make the player temporarily unable to control themselves

    public bool isAttacking;
    private GroundSlam groundSlam;
    private ChargePunch chargePunch;
    private PlayerSecondaryWeaponThrowHandler playerSecondaryWeaponThrowHandler;

    public int FacingDirection { get; set; } = 1;
    private Vector2 oldVelocity;
    public Vector2 NewVelocity { get; set; }
    private Vector2 newForce;

    //used to calculate "control intertia"
    public float ControlMomentum;

    [SerializeField] private bool _isGrounded; public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    [SerializeField] private bool _isCrouching; public bool IsCrouching { get { return _isCrouching; } set { _isCrouching = value; } }
    [SerializeField] private bool _isRunning; public bool IsRunning { get { return _isRunning; } set { _isRunning = value; } }
    //private bool isOnSlope;
    //private bool canWalkOnSlope;
    [SerializeField] private bool _isAgainstWall; public bool IsAgainstWall { get { return _isAgainstWall; } set { _isAgainstWall = value; } }
    [SerializeField] private bool _isWallHanging; public bool IsWallHanging { get { return _isWallHanging; } set { _isWallHanging = value; } }
    [SerializeField] private bool _canWallJump; public bool CanWallJump { get { return _canWallJump; } set { _canWallJump = value; } }


    //Health points, magic points, soul points (currency)
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private PlayerMana playerMana;

    [SerializeField]
    private Transform StartingLocation;
    [SerializeField] private Transform parentTransform;
    DataManager dataManager;


    //Set all the initial values

    private void OnEnable()
    {
        EventSystem.current.onPlayerDeathTrigger += PlayerDeath;
        EventSystem.current.onGameFileLoaded += SetPosition;
    }

    private void Start()
    {
        dataManager = DataManager.Instance;

        Rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<BoxCollider2D>();
        gameController = FindObjectOfType<GameController>();
        playerPrimaryWeapon = GetComponentInChildren<PlayerPrimaryWeapon>();
        playerShield = GetComponentInChildren<PlayerShield>();
        playerJump = GetComponentInChildren<PlayerJump>();
        playerDash = GetComponentInChildren<PlayerDash>();
        chargePunch = GetComponentInChildren<ChargePunch>();

        
        ControlMomentum = 0;
        animator = GetComponentInChildren<PlayerAnimator>(true);
        visualEffects = GetComponentInChildren<PlayerVisualEffectsController>();

        if (GetComponent<PlayerHealth>() != null) { playerHealth = GetComponent<PlayerHealth>(); }
        else { Debug.Log("PlayerHealth.cs is being requested as a component of the same object as PlayerController.cs, but could not be found on the object"); }
        groundSlam = GetComponentInChildren<GroundSlam>();
        playerSecondaryWeaponThrowHandler = GetComponentInChildren<PlayerSecondaryWeaponThrowHandler>();
    }

    private void FixedUpdate() // these dataManager points are used during save to have a last known location
    {
        dataManager.sessionData.lastKnownWorldLocationX = transform.position.x;
        dataManager.sessionData.lastKnownWorldLocationY = transform.position.y;
    }

    //Does anything in the environment layer overlap with the circle while not on the way up
    public void CheckGround()
    {
        if (Rb.velocity.y == 0.0f) // if y velocity is 0
        {
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        }
        else 
        {
            _isGrounded = false;
            if (!playerJump.IsJumping && !chargePunch.IsCharging) { animator.Play("PlayerFall"); }
        }

        if (Rb.velocity.y <= 0.0f) { playerJump.IsJumping = false; }
        else
        {
            playerJump.IsJumping = true;
            if (!chargePunch.IsCharging) { animator.Play("PlayerJump"); }
        }

        if (_isGrounded && !playerJump.IsJumping)
        {
            playerJump.CanJump = true;
            _canWallJump = true;

            if (!playerDash.IsDashing) { playerDash.CanDash = true; }
            PlayerCrouch(); 
        }
        else if (!_isGrounded)
        {
            playerJump.CanJump = false;
        }

        if(_isAgainstWall && !_isGrounded) { animator.Play("PlayerWallLand"); _isWallHanging = true; }
        else { _isWallHanging = false; }
    }

    public void CheckWall() 
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(wallCheck.position, groundCheckRadius);

        // Check if any of the colliders have the desired tag
        _isAgainstWall = false;
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Boundary"))
            {
                _isAgainstWall = true;
                break; // No need to check further if we found a wall
            }
        }
    }

    //called by the GameController, Cutscenes (Cutscene.cs), and by PlayerController for idling
    //'optional' parameters are used by cutscenes
    public void ApplyMovement(Vector3? optionalCutsceneDestination = null, float optionalXAdjustment = 0f) 
    {
        // if you have a target destination / cutscene, automate movement
        if (optionalCutsceneDestination.HasValue) { CutsceneMovementHandler(optionalCutsceneDestination, optionalXAdjustment); }

        if (!playerHealth.inHitStun && !playerDash.IsDashing && !chargePunch.IsCharging && !playerSecondaryWeaponThrowHandler.inActiveThrow)
        {
            if (_isGrounded && !playerJump.IsJumping) //if on ground
            {
                SetVelocity(MovementSpeed * ControlMomentum/10, Rb.velocity.y);
                
                if(!isAttacking && !playerJump.IsJumping)
                {
                    if ((Rb.velocity.x == 0 && !playerShield.shieldOn && !IsCrouching) || DialogueManager.GetInstance().DialogueIsPlaying) // if still and not shielding or cutscene manager is on
                    {
                        if (gameController.timeSinceInput > thresHoldToLoopAlternativeIdleAnimation) { animator.Play("PlayerMeditate"); }
                        else if (gameController.timeSinceInput > thresholdToTransitionToIdle) { animator.Play("PlayerStandToMeditate"); }
                        else { IdlePlayer(); }     
                    }
                    else if(Rb.velocity.x != 0) // if moving
                    {
                        animator.Play("PlayerRun");
                        IsRunning = true;
                        visualEffects.PlayParticleSystem("MovementDust");
                    }
                }

            }
            else if (!_isGrounded) //If in air
            {
                IsRunning = false;
                SetVelocity(MovementSpeed * ControlMomentum/10, Rb.velocity.y);
            }
        }
        else if (chargePunch.IsCharging) 
        {
            if (animator.CheckIfAnimationIsPlaying("PlayerBasicAttack")){ }
            else { if (!IsCrouching) { animator.Play("PlayerCharge"); } } 
        }
        else if (playerSecondaryWeaponThrowHandler.inActiveThrow)
        {
            if (!IsCrouching) { animator.Play("PlayerCharge"); }
        }
    }

    void CutsceneMovementHandler(Vector3? optionalCutsceneDestination = null, float optionalXAdjustment = 0f)
    {
        float targetWorldXPosition = optionalCutsceneDestination.Value.x - transform.position.x + optionalXAdjustment;
        if (targetWorldXPosition > 0) { SetVelocity(); gameController.XInput = 1; } // if distance / destination is right, pause and then move right
        else if (targetWorldXPosition < 0) { SetVelocity(); gameController.XInput = -1; } // else, do the same and move left
    }

    public bool MovePlayerToPosition(Vector3 pointToReach, float adjustmentInXDirection) // used by cutscenes to move a player to a point
    {
        ApplyMovement(pointToReach, adjustmentInXDirection);

        // return false to break the while loop that is calling this function, if player has reached destination
        // use ints, since player and position floats exactly overlapping is rare
        if ((int)transform.position.x == (int)(pointToReach.x + adjustmentInXDirection)) 
        {
            IdlePlayer();
            return false; 
        } 
        return true;
    }

    public void UpdateMomentum()
    {
        if (gameController.XInput > 0 && ControlMomentum < 10)
        {
            if (ControlMomentum == 0) { ControlMomentum = 5; }
            else { ControlMomentum += 1; }
        }

        else if (gameController.XInput < 0 && ControlMomentum > -10)
        {
            if (ControlMomentum == 0) { ControlMomentum = -5; }
            else { ControlMomentum -= 1; }
        }
        else if (gameController.XInput == 0)
        {
            if (ControlMomentum > 0) { ControlMomentum -= 1; }
            else if (ControlMomentum < 0) { ControlMomentum += 1; }
        }

        if (ControlMomentum > 10) { ControlMomentum -= 1; }
        else if (gameController.XInput < 0 && ControlMomentum < -10) { ControlMomentum += 1; }
    }

    //flips the model
    public void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    //Calculated direction of hit for knockback direction.
    public void Hit(Vector3 enemyPos, float knockbackMod)
    {
        if (!playerHealth.IsInvincible)
        {
            StartCoroutine(gameController.PlayHaptics());
            float knockbackForce = playerJump.JumpForce * (1 - knockbackMod);

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

    public void SetPosition(DataManager.GameData gameData)
    {
        Debug.Log("Attempting to set position...");
        parentTransform.position = new Vector2(gameData.lastKnownWorldLocationX, gameData.lastKnownWorldLocationY);
    }

    // handles crouching logic, assumes player is grounded
    void PlayerCrouch()
    {
        if(IsCrouching == true && gameController.YInput >= 0) { IsCrouching = false; animator.Play("PlayerCrouchToStand"); }
        else if (IsCrouching == true && gameController.YInput < 0) { animator.Play("PlayerCrouch"); }
        else if(IsCrouching == false && gameController.YInput < 0) { IsCrouching = true; animator.Play("PlayerStandToCrouch"); }
        
    }
    public void IdlePlayer(bool doRegardlessOfPlayerInput = false) // if this optional value is called with a true, animator will play idle 
    {
        SetVelocity();
        IsRunning = false; IsCrouching = false; IsGrounded = true;
        if (doRegardlessOfPlayerInput || gameController.XInput == 0)  { animator.Play("PlayerIdle");  }
    }

    public void SetVelocity() { SetVelocity(0, 0); }

    public void SetVelocity(float xVel, float yVel)
    {
        NewVelocity = new Vector2(xVel, yVel);
        Rb.velocity = NewVelocity;
    }

    public void AddForce(float xDirForce, float yDirForce)
    {
        newForce.Set(xDirForce, yDirForce);
        Rb.AddForce(newForce, ForceMode2D.Impulse);
    }

    private void OnDestroy()
    {
        EventSystem.current.onPlayerDeathTrigger -= PlayerDeath;
        EventSystem.current.onGameFileLoaded -= SetPosition;
    }
}
