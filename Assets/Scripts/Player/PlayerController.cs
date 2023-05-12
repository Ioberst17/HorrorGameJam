using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
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

    private Animator animator;
    public PlayerParticleSystems visualEffects;
    private PlayerPrimaryWeapon playerPrimaryWeapon;

    // other ability references
    PlayerDash playerDash;
    [SerializeField] PlayerJump playerJump;

    //To make the player temporarily unable to control themselves

    public bool isAttacking;
    private GroundSlam groundSlam;
    private ChargePunch chargePunch;

    public int FacingDirection { get; set; } = 1;
    private Vector2 oldVelocity;
    public Vector2 NewVelocity { get; set; }
    private Vector2 newForce;

    //used to calculate "control interta"
    public float ControlMomentum;

    [SerializeField] private bool _isGrounded; public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    //private bool isOnSlope;
    //private bool canWalkOnSlope;
    [SerializeField] private bool _isAgainstWall; public bool IsAgainstWall { get { return _isAgainstWall; } set { _isAgainstWall = value; } }
    [SerializeField] private bool _canWallJump; public bool CanWallJump { get { return _canWallJump; } set { _canWallJump = value; } }


    //Health points, magic points, soul points (currency)
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private PlayerMana playerMana;

    [SerializeField] private bool hasShield;

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
        playerPrimaryWeapon = FindObjectOfType<PlayerPrimaryWeapon>();
        playerJump = GetComponentInChildren<PlayerJump>();
        playerDash = GetComponentInChildren<PlayerDash>();
        chargePunch = GetComponentInChildren<ChargePunch>();

        MovementSpeed = 10;
        ControlMomentum = 0;
        animator = GetComponent<Animator>();
        visualEffects = transform.Find("VisualEffects").gameObject.GetComponent<PlayerParticleSystems>();

        if (GetComponent<PlayerHealth>() != null) { playerHealth = GetComponent<PlayerHealth>(); }
        else { Debug.Log("PlayerHealth.cs is being requested as a component of the same object as PlayerController.cs, but could not be found on the object"); }
        groundSlam = GetComponentInChildren<GroundSlam>();
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
        }
    }

    public void CheckWall() { _isAgainstWall = Physics2D.OverlapCircle(wallCheck.position, groundCheckRadius, whatIsGround); }

    //called by the GameController, Cutscenes (Cutscene.cs), and by PlayerController for idling
    //'optional' parameters are used by cutscenes
    public void ApplyMovement(Vector3? optionalCutsceneDestination = null, float optionalXAdjustment = 0f) 
    {
        // if you have a target destination / cutscene, automate movement
        if (optionalCutsceneDestination.HasValue) { CutsceneMovementHandler(optionalCutsceneDestination, optionalXAdjustment); }

        if (groundSlam.IsGroundSlam == true) { SetVelocity(0, Rb.velocity.y); } // make sure a groundslam's velocity is maintained

        if (!playerHealth.inHitStun && !playerDash.IsDashing && !chargePunch.IsCharging)
        {
            if (_isGrounded && !playerJump.IsJumping) //if on ground
            {
                SetVelocity(MovementSpeed * ControlMomentum/10, Rb.velocity.y);
                
                if(!isAttacking && !playerJump.IsJumping)
                {
                    if (Rb.velocity.x == 0 || DialogueManager.GetInstance().DialogueIsPlaying) // if still
                    {
                        IdlePlayer();
                    }
                    else // if moving
                    {
                        animator.Play("PlayerRun");
                        visualEffects.PlayEffect("MovementDust");
                    }
                }

            }
            else if (!_isGrounded) //If in air
            {
                SetVelocity(MovementSpeed * ControlMomentum/10, Rb.velocity.y);
            }
        }
        else if (chargePunch.IsCharging)
        {
            if (CheckIfAnimationIsPlaying("PlayerBasicAttack")){ }
            else { animator.Play("PlayerCharge"); } 
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

    public bool CheckIfAnimationIsPlaying(string animationName) 
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f) { return true; } // if the current state is that animation, and it's normalized play time is less than 1, it's in play
        return false;
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
        if (!playerHealth.isInvincible)
        {
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

    public void IdlePlayer(bool doRegardlessOfPlayerInput = false) // if this optional value is called with a true, animator will play idle 
    {
        SetVelocity();
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
