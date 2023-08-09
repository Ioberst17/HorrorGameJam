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

public class PlayerController : Controller
{
    [Header("Player Specific Controller Values")]
    //External References
    DataManager dataManager;
    private GameController gameController;
    [SerializeField] private Transform cameraFocus;
    [SerializeField] private LayerMask whatIsEnemy;

    // Other player references e.g. animators and abilities
    private PlayerAnimator animator;
    public PlayerVisualEffectsController visualEffects;
    private PlayerPrimaryWeapon playerPrimaryWeapon;
    private PlayerShield playerShield;
    PlayerDash playerDash;
    PlayerJump playerJump;
    private GroundSlam groundSlam;
    private ChargePunch chargePunch;
    private PlayerSecondaryWeaponThrowHandler playerSecondaryWeaponThrowHandler;
    //Health points, magic points, soul points (currency)
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private PlayerMana playerMana;

    // alt idle animations
    public float thresholdToTransitionToIdle;
    public float thresHoldToLoopAlternativeIdleAnimation;

    //used to calculate "control intertia"
    public float ControlMomentum;

    [SerializeField] private Transform parentTransform;


    //Set all the initial values

    private void OnEnable()
    {
        EventSystem.current.onPlayerDeathTrigger += PlayerDeath;
        EventSystem.current.onGameFileLoaded += SetPosition;
    }

    override protected void Start()
    {
        base.Start();
        dataManager = DataManager.Instance;

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

    override protected void FixedUpdate() // these dataManager points are used during save to have a last known location
    {
        HitStunBlink();
        dataManager.sessionData.lastKnownWorldLocationX = transform.position.x;
        dataManager.sessionData.lastKnownWorldLocationY = transform.position.y;
    }

    //Does anything in the environment layer overlap with the circle while not on the way up
    override public void CheckGround()
    {
        if (RB.velocity.y == 0.0f) { _isGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundCheckRadius, whatIsGround); }
        else 
        {
            _isGrounded = false;
            if (!playerJump.IsJumping && !chargePunch.IsCharging) { animator.Play("PlayerFall"); }
        }

        if (RB.velocity.y <= 0.0f) { playerJump.IsJumping = false; }
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(wallCheck.position, GroundCheckRadius);

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

        if (!InHitStun && !playerDash.IsDashing && !chargePunch.IsCharging && !playerSecondaryWeaponThrowHandler.inActiveThrow)
        {
            if (_isGrounded && !playerJump.IsJumping) //if on ground
            {
                SetVelocity(MovementSpeed * ControlMomentum/10, RB.velocity.y);
                
                if(!IsAttacking && !playerJump.IsJumping)
                {
                    if ((RB.velocity.x == 0 && !playerShield.shieldOn && !IsCrouching) || DialogueManager.GetInstance().DialogueIsPlaying) // if still and not shielding or cutscene manager is on
                    {
                        if (gameController.timeSinceInput > thresHoldToLoopAlternativeIdleAnimation) { animator.Play("PlayerMeditate"); }
                        else if (gameController.timeSinceInput > thresholdToTransitionToIdle) { animator.Play("PlayerStandToMeditate"); }
                        else { IdlePlayer(); }     
                    }
                    else if(RB.velocity.x != 0 && !IsCrouching) // if moving
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
                SetVelocity(MovementSpeed * ControlMomentum/10, RB.velocity.y);
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

    //Calculated direction of hit for knockback direction.
    override public void HandleHitPhysics(Vector3 enemyPos, float knockbackMod)
    {
        if (!IsInvincible)
        {
            StartCoroutine(gameController.PlayHaptics());
            float knockbackForce = playerJump.JumpForce * (1 - knockbackMod);

            if (enemyPos.x >= transform.position.x) { SetVelocity(-knockbackForce / 3, 0.0f); }
            else { SetVelocity(knockbackForce / 3, 0.0f); }

            AddForce(0.0f, knockbackForce / 3);
        }
    }

    override protected void HitStunBlink()
    {
        if (InHitStun)
        {
            // calculate the blink time based on frequency
            float blinkTime = Mathf.Sin(Time.time * blinkFrequency);
            // set the sprite renderer to be visible if blink time is positive, otherwise invisible
            animator.SpriteEnabled(blinkTime > 0f);
        }
        else { animator.SpriteEnabled(true); }
    }

    void PlayerDeath()
    {
        animator.Play("PlayerDeath");
        transform.position = StartingLocation;
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

    private void OnDestroy()
    {
        EventSystem.current.onPlayerDeathTrigger -= PlayerDeath;
        EventSystem.current.onGameFileLoaded -= SetPosition;
    }
}
