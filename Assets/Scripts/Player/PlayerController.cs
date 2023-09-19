using JetBrains.Annotations;
using System;
//using UnityEditorInternal;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : Controller
{
    [Header("Player Specific Controller Values")]
    //External References
    DataManager dataManager;
    GameController gameController;
    [SerializeField] Transform cameraFocus;
    [SerializeField] LayerMask whatIsEnemy;

    // Other player references e.g. animators and abilities
    PlayerAnimator animator;
    PlayerVisualEffectsController visualEffects;
    PlayerShield playerShield;
    PlayerDash playerDash;
    PlayerJump playerJump;
    ChargePunch chargePunch;
    PlayerThrowHandler playerSecondaryWeaponThrowHandler;

    // alt idle animations
    public float thresholdToTransitionToIdle;
    public float thresHoldToLoopAlternativeIdleAnimation;

    //used to calculate "control intertia"
    public float ControlMomentum;

    [SerializeField] private Transform parentTransform;

    override public bool IsAttacking
    {
        get { return _isAttacking; }
        set
        {
            if (value == false) { EventSystem.current.EndActiveMeleeTrigger(); }
            _isAttacking = value;
        }
    }


    private void OnEnable()
    {
        EventSystem.current.onPlayerDeathTrigger += PlayerDeath;
        EventSystem.current.onGameFileLoaded += SetPosition;
        EventSystem.current.onPlayerHitPostHealthTrigger += PlayerHitHandler;
    }

    private void OnDestroy()
    {
        EventSystem.current.onPlayerDeathTrigger -= PlayerDeath;
        EventSystem.current.onGameFileLoaded -= SetPosition;
        EventSystem.current.onPlayerHitPostHealthTrigger -= PlayerHitHandler;
    }

    override protected void Start()
    {
        base.Start();
        dataManager = DataManager.Instance;

        gameController = FindObjectOfType<GameController>();
        playerShield = GetComponentInChildren<PlayerShield>();
        playerJump = GetComponentInChildren<PlayerJump>();
        playerDash = GetComponentInChildren<PlayerDash>();
        chargePunch = GetComponentInChildren<ChargePunch>();

        
        ControlMomentum = 0;
        animator = GetComponentInChildren<PlayerAnimator>(true);
        visualEffects = GetComponentInChildren<PlayerVisualEffectsController>();

        playerSecondaryWeaponThrowHandler = GetComponentInChildren<PlayerThrowHandler>();
    }

    override protected void FixedUpdate() // these dataManager points are used during save to have a last known location
    {
        HitStunBlink();
        dataManager.sessionData.lastKnownWorldLocationX = transform.position.x;
        dataManager.sessionData.lastKnownWorldLocationY = transform.position.y;
    }

    /// <summary>
    /// Updates by using the groundcheck circle below - checks for overlap circle with the 'Environment' layer
    /// </summary>
    override public void CheckGround()
    {
        if (RB.velocity.y == 0.0f) { _isGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, whatIsGround); }
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
        else if (!_isGrounded) { playerJump.CanJump = false; }

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

    /// <summary>
    /// Called by the GameController, Cutscenes (Cutscene.cs), and by PlayerController for idling 'optional' parameters are used by cutscenes
    /// </summary>
    /// <param name="optionalCutsceneDestination"></param>
    /// <param name="optionalXAdjustment"></param>
    public void ApplyMovement(Vector3? optionalCutsceneDestination = null, float optionalXAdjustment = 0f) 
    {
        // if you have a target destination / cutscene, automate movement
        if (optionalCutsceneDestination.HasValue) { CutsceneMovementHandler(optionalCutsceneDestination, optionalXAdjustment); }

        if (!IsStunned && !InHitStun && !playerDash.IsDashing && !chargePunch.IsCharging && !playerSecondaryWeaponThrowHandler.inActiveThrow)
        {
            if (_isGrounded && !playerJump.IsJumping && !IsCrouching) //if on ground
            {
                SetVelocity(MovementSpeed * ControlMomentum/10, null);
                
                if(/*!IsAttacking && */!playerJump.IsJumping)
                {
                    if ((RB.velocity.x == 0 && !playerShield.shieldOn && !IsCrouching) || DialogueManager.GetInstance().DialogueIsPlaying) // if still and not shielding or cutscene manager is on
                    {
                        if (gameController.timeSinceInput > thresHoldToLoopAlternativeIdleAnimation) { animator.Play("PlayerMeditate"); }
                        else if (gameController.timeSinceInput > thresholdToTransitionToIdle) { animator.Play("PlayerStandToMeditate"); }
                        else { PlayerIdle(); }     
                    }
                    else if(RB.velocity.x != 0 && !IsCrouching) // if moving
                    {
                        PlayerRun();
                        visualEffects.PlayParticleSystem("MovementDust");
                    }
                }

            }
            else if (!_isGrounded) //If in air
            {
                IsRunning = false;
                SetVelocity(MovementSpeed * ControlMomentum / 10, null); 
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
            PlayerIdle();
            return false; 
        } 
        return true;
    }

    /// <summary>
    /// Updates player's momentum variable based on player input
    /// </summary>
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

    void PlayerHitHandler(Vector3 enemyPost, float knockBackMod, bool hitInActiveShieldZone) 
    {
        HandleHitPhysics(enemyPost, knockBackMod);
        if (!hitInActiveShieldZone) { StartCoroutine(HitStun()); }
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
        if(IsCrouching == true && gameController.YInput >= 0f) { IsCrouching = false; animator.Play("PlayerCrouchToStand"); SetVelocity(); }
        else if (IsCrouching == true && gameController.YInput < -0.8f && !playerDash.IsDashing) { animator.Play("PlayerCrouch"); SetVelocity(); }
        else if(IsCrouching == false && gameController.YInput < -0.8f) { IsCrouching = true; animator.Play("PlayerStandToCrouch"); SetVelocity(); }
        
    }
    public void PlayerIdle(bool doRegardlessOfPlayerInput = false) // if this optional value is called with a true, animator will play idle 
    {
        SetVelocity();
        IsRunning = false; IsCrouching = false; IsGrounded = true;
        ToggleRunAndIdleAnimationPriority("PlayerIdle");
        if (doRegardlessOfPlayerInput || gameController.XInput == 0)  { animator.Play("PlayerIdle");  }
    }

    public void PlayerRun()
    {
        IsRunning = true;
        ToggleRunAndIdleAnimationPriority("PlayerRun");
        animator.Play("PlayerRun");
    }

    // allows run and idle to switch priority; helpful when stopping
    void ToggleRunAndIdleAnimationPriority(string animationName)
    {
        // If running, run should have higher priority, else idle should
        if(animationName == "PlayerRun") { animator.UpdateAnimationStatePriority("PlayerRun", -2); animator.UpdateAnimationStatePriority("PlayerIdle", -3); }
        else if(animationName == "PlayerIdle") { animator.UpdateAnimationStatePriority("PlayerRun", -3); animator.UpdateAnimationStatePriority("PlayerIdle", -2); }
    }
}
