using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ArmWeaponAnimatorCommonFunctionality : BodyPartAnimator
{

    // THIS SCRIPT CONTAINS COMMON FUNCTIONALITY FOR THE WEAPON AND ARM SPRITES
    // IT PRIMARILY REOLVES AROUND PLAYING ANIMATIONS RELATED TO SHOOTING - initial shots, shots within a timeframe, continuous fire

    [Header("References Needed for Arms and Weapons Functionality")]
    public Transform transformToRotateAround;
    protected GameController gameController;
    protected PlayerController playerController;
    protected PlayerSecondaryWeapon playerSecondaryWeapon;
    protected WeaponDatabase weaponDatabase;
    protected Weapons currentWeapon;
    BaseAnimator baseAnimator;

    [Header("Shooting Variables")]
    public Vector3 initialArmPosition;
    public Vector3 initialArmPositionWhenCrouching;
    public bool startedShotOnLeft;
    public float elapsedTime;
    public float rotationTime = 10f;
    public float rotationRightSpeed = 25;
    public float rotationLeftSpeed = 5f;
    public float rotationSpeedToUse = 100f;
    public Quaternion shotDirectionRotation;
    public float shotRotation;
    public bool externalCallToBreakArmReturnLoop;
    protected float postShotPause = 2f;

    public bool isShootCoroutineRunning;
    public bool isRestartedShootCoroutineRunning;
    public bool isContinuousShootCoroutineRunning;
    public Coroutine playerShootCoroutine;
    public Coroutine playerShootRestartedCoroutine;
    public Coroutine playerShootContinousCoroutine;
    public bool farAlongEnoughToReset; // what direction the hand shoot return to idle
    protected string[] animationsThatDoNotOverrideShooting = new string[6] { "PlayerIdle", "PlayerCrouch", "PlayerRun", "PlayerJump", "PlayerFall", "PlayerLand" };

    // regarding end condition of arm rotation
    private bool _oneHandedWeaponInUse, _twoHandedWeaponInUse;
    public bool OneHandedWeaponInUse
    {
        get { return _oneHandedWeaponInUse; }
        set
        {
            if (value == true)
            {
                _oneHandedWeaponInUse = value;
                // Set the value of the float variable
                endingArmRotation = -90;
                // Set the another bool to false
                _twoHandedWeaponInUse = false;
            }
        }
    }
    public bool TwoHandedWeaponInUse
    {
        get { return _twoHandedWeaponInUse; }
        set
        {
            if (value == true)
            {
                _twoHandedWeaponInUse = value;
                // Set the value of the float variable
                endingArmRotation = 0;
                // Set the another bool to false
                _oneHandedWeaponInUse = false;
            }
        }
    }
    protected float armPivot; // used by children to define an adjustment, e.g. how much and right and left arms are offset when pointing a gun
    private float endingArmRotation;
    public float armFinishedRotatingThreshold = 10f; // the angle that is 'reasonable' when checking if the shooting arm has returned
    public bool isArmFacingEndPosition;

    override public void Start()
    {
        base.Start();

        // get key references
        gameController = FindObjectOfType<GameController>();
        playerController = FindObjectOfType<PlayerController>();
        weaponDatabase = FindObjectOfType<WeaponDatabase>();
        baseAnimator = FindObjectOfType<BaseAnimator>();
        playerSecondaryWeapon = playerController.GetComponentInChildren<PlayerSecondaryWeapon>();
        transformToRotateAround = transform.parent.parent.transform;
        initialArmPosition = transformToRotateAround.localPosition;
        EventSystem.current.onUpdateSecondaryWeaponTrigger += OnWeaponSwitch;
        EventSystem.current.OnShotFired += PauseAnimator; // called by animation Keyframes
    }
    private void OnDestroy()
    {
        EventSystem.current.onUpdateSecondaryWeaponTrigger -= OnWeaponSwitch;
        EventSystem.current.OnShotFired -= PauseAnimator;
    }

    // used by child animations to set an animation when needed e.g. weapon change
    override public void AssignNewAnimations(string objectName) { base.AssignNewAnimations(objectName); }

    void OnWeaponSwitch(int weaponID, string weaponName, int weaponLevel)
    {
        currentWeapon = weaponDatabase.ReturnItemFromID(weaponID);
        if (currentWeapon.isOneHanded) { OneHandedWeaponInUse = true; }
        if (currentWeapon.isTwoHanded) { TwoHandedWeaponInUse = true; }
    }

    // BELOW THIS ARE FUNCTIONS TO HANDLE THE ROTATION OF GAMEOBJECTS RELATED TO SHOOTING ANIMATIONS

    // overrides check if new animation can play to handle shooting cases, and syncing body parts to base animator if using a 'complete' interaction
    override public string CheckIfNewAnimationCanPlay(string newAnimation)
    {
        GetCurrentAnimationInfo();

        // sync with base animator
        if (baseAnimator.currentAnimationStateHasCompleteTag)
        {
            if (priorityAnimationFlag) { ResetShootCoroutineVariables(); }
            shouldNewAnimationPlay = true;
            return baseAnimator.currentAnimationStateName;
        }

        // if shoot is running and a low priority animation is running, then let shoot animation continue
        if (priorityAnimationFlag && NewAnimationDoesNotOverridePriority(newAnimation) && newAnimation != "PlayerShoot") { shouldNewAnimationPlay = false; return ""; }

        // else compare priority

        // Check if currentAnimationState is null
        if (currentAnimationState == null) { Debug.Log($"{GetType().Name}: currentAnimationState is null"); }

        if (currentAnimationState == null) { ResetShootCoroutineVariables(); shouldNewAnimationPlay = true; return newAnimation; } // currentAnimatStation loads as null if a player tries an action with a 'complete' tag
        else if (animationStates[newAnimation].priorityLevel > currentAnimationState.priorityLevel || currentAnimation.normalizedTime >= 1f) { shouldNewAnimationPlay = true; return newAnimation; } // if so, then it can play
        else { shouldNewAnimationPlay = false; return currentAnimationState.animationName; }
    }


    private bool NewAnimationDoesNotOverridePriority(string newAnimation)
    {
        return System.Array.Exists(animationsThatDoNotOverrideShooting, animation => animation == newAnimation);
    }

    // called by secondary weaponsmanager, since it's invoked it does not register a reference
    public void PlayerShoot()
    {
        if (playerShootCoroutine != null)
        {
            // stop current instance of coroutine
            StopCoroutine(playerShootCoroutine);
            playerShootCoroutine = null;

            // stop current sub coroutines
            StopCoroutine(ReturnToStartingPosition());
            externalCallToBreakArmReturnLoop = true;

            // call a restarted player shoot
            RestartPlayerShoot();
        }
        else { playerShootCoroutine = StartCoroutine(PlayerShootCoroutine()); }
    }

    // handles inital player shot (not used if a shot is fired before completion, a function below is)
    public IEnumerator PlayerShootCoroutine()
    {
        priorityAnimationFlag = true;
        isShootCoroutineRunning = true;
        farAlongEnoughToReset = false;
        startedShotOnLeft = false;

        if (playerController.FacingDirection == -1) { startedShotOnLeft = true; }

        // Kept because for unknown reason rotations are faster on one side vs the other. Need to debug.
        if (playerController.FacingDirection == 1) { rotationTime = rotationRightSpeed; }
        else { rotationTime = rotationLeftSpeed; }

        animator.Play("PlayerShoot");
        if (!playerController.IsCrouching) { transformToRotateAround.localPosition = initialArmPosition; }
        else { transformToRotateAround.localPosition = initialArmPositionWhenCrouching; }

        // update arm position to where mouse is pointed
        SetPointingDirection();
        farAlongEnoughToReset = true;

        // now wait
        yield return new WaitForSeconds(postShotPause);

        yield return StartCoroutine(ReturnToStartingPosition());

        ClosingAnimationChanges();
        ResetShootCoroutineVariables();
    }

    // handles restarting player shoot if already handling a shot (checks to see if an existing function is running)
    private void RestartPlayerShoot()
    {
        if (playerShootRestartedCoroutine != null)
        {
            StopCoroutine(playerShootRestartedCoroutine);
            playerShootRestartedCoroutine = null;
        }
        StopCoroutine(ReturnToStartingPosition()); externalCallToBreakArmReturnLoop = true;
        playerShootRestartedCoroutine = StartCoroutine(RestartPlayerShootCoroutine());
    }

    // a shortened version of the initial player shoot without the startup animation
    private IEnumerator RestartPlayerShootCoroutine()
    {
        isShootCoroutineRunning = true;
        isRestartedShootCoroutineRunning = true;
        if (playerController.FacingDirection == -1) { startedShotOnLeft = true; }
        else { startedShotOnLeft = false; }

        SetPointingDirection();

        yield return new WaitForSeconds(postShotPause);

        yield return StartCoroutine(ReturnToStartingPosition());

        ClosingAnimationChanges();
        ResetShootCoroutineVariables();
    }

    // Get the direction from the arm's current position to the mouse position
    virtual public void SetPointingDirection(bool optionalForceLeftFacingModification = false) // optional is used by continuous fire
    {
        Vector2 rotation = (Vector2)gameController.lookInput - (Vector2)gameController.playerPositionScreen;
        var rotationZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        if (startedShotOnLeft || optionalForceLeftFacingModification) { rotationZ = MapLeftFacingValues(rotationZ); }

        // done to adjust arms e.g. left arm is slightly off angle compared to right arm when shooting
        rotationZ += AdjustArms(rotationZ);

        transformToRotateAround.localRotation = Quaternion.Euler(0, 0, rotationZ);

        shotDirectionRotation = transformToRotateAround.localRotation;
    }

    // this is used to slightly adjust the right and left arms when a weapon is pointed in a direction
    // for context if a gun is pointed at 45 degrees and up, the right and left arms will be slightly lower / more and not pointing in exactly the same direction as the weapon
    virtual protected float AdjustArms(float inputValue)
    {

        // Clamp the inputValue between 0 and 360 degrees to handle all cases
        inputValue = Mathf.Repeat(inputValue, 360f);

        // flag used to define upper and lower bounds, if false bounds are reversed e.g. input of 360 should receive 0 adjustment, but 270 would receive maximum
        bool smallToBig = true;
        float lowerBound = 0f;
        float upperBound = armPivot;

        if (inputValue >= 270f && inputValue < 360f)
        {
            smallToBig = false;
        }

        // Determine the interpolation range based on the smallToBig flag
        if (!smallToBig)
        {
            lowerBound = armPivot;
            upperBound = 0f;
        }

        // Perform interpolation based on the quadrant the inputValue falls into
        if (inputValue >= 0f && inputValue < 90f) { return Mathf.Lerp(lowerBound, upperBound, inputValue / 90f); }
        else if (inputValue >= 90f && inputValue < 180f) { return Mathf.Lerp(lowerBound, upperBound, (inputValue - 90f) / 90f); }
        else if (inputValue >= 180f && inputValue < 270f) { return Mathf.Lerp(lowerBound, upperBound, (inputValue - 180f) / 90f); }
        else if (inputValue >= 270f && inputValue < 360f) { return Mathf.Lerp(lowerBound, upperBound, (inputValue - 270f) / 90f) * -1; }
        else if (inputValue >= -90f && inputValue < 0f) { return Mathf.Lerp(lowerBound, upperBound, (inputValue + 90f) / 90f) * -1; }
        else /* (inputValue >= -180f && inputValue < -90f) */ { return Mathf.Lerp(lowerBound, upperBound, (inputValue + 180f) / 90f); }
    }

    // used when facing left
    public float MapLeftFacingValues(float value)
    {
        if (value >= 90f && value <= 180f) { return 90f - (value - 90f); }
        else if (value >= -180f && value <= -90f) { return -90f + (-value - 90f); }
        else
        {
            Debug.Log("Value out of range!");
            return value;
        }
    }

    // Rotate the arm down from rotation angle
    IEnumerator ReturnToStartingPosition()
    {
        externalCallToBreakArmReturnLoop = false;
        isArmFacingEndPosition = false;
        elapsedTime = 0f;
        while (elapsedTime < rotationTime // time to finish, auto breaks if taking too long
            && !isArmFacingEndPosition // end condition
            && !externalCallToBreakArmReturnLoop) // an external breaker used when restarting coroutines
        {
            SetDirectionOverTime(transformToRotateAround.rotation.eulerAngles.z);

            DetermineIfFinished();

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void SetDirectionOverTime(float currentRotationZ)
    {
        var direction = -1; // works for all standard rotations
        if (TwoHandedWeaponInUse && transformToRotateAround.localRotation.eulerAngles.z > 270) { direction = 1; } // ensure weapon goes up, if pointing down

        currentRotationZ += direction * rotationSpeedToUse * Time.deltaTime; // Adjust rotation speed based on frame rate

        currentRotationZ += AdjustArms(currentRotationZ);
    }

    // used to check if the arm rotation has finished setting direction over time
    private void DetermineIfFinished()
    {
        float zRotation = transformToRotateAround.rotation.eulerAngles.z;
        if (OneHandedWeaponInUse) { endingArmRotation = -90f; }
        else if (TwoHandedWeaponInUse) { endingArmRotation = 0; }

        float angleDifference = Mathf.DeltaAngle(zRotation, endingArmRotation);

        if (Mathf.Abs(angleDifference) <= armFinishedRotatingThreshold) // check if arm is within a reasonable threshold of done
        {
            isArmFacingEndPosition = true;
        }
    }

    void PauseAnimator() { animator.speed = 0; }

    // used when returning a single / discrete shot weapon to the player's hand
    void ClosingAnimationChanges()
    {
        animator.speed = 1f; // Resume animator speed
        // placeholder for other changes
    }

    void ResetShootCoroutineVariables()
    {
        animator.speed = 1f;
        priorityAnimationFlag = false;
        isShootCoroutineRunning = false;
        isRestartedShootCoroutineRunning = false;
        farAlongEnoughToReset = false;
        playerShootCoroutine = null;
        playerShootRestartedCoroutine = null;
        playerShootContinousCoroutine = null;
        // Animation sequence completed
        if (startedShotOnLeft == true) { startedShotOnLeft = false; }
        transformToRotateAround.localPosition = initialArmPosition;
        transformToRotateAround.localRotation = new Quaternion();
    }

    // used by continously shooting weapons e.g. flamethrower
    public void PlayerShootContinuous()
    {
        if (playerShootContinousCoroutine != null) { return; }
        else
        {
            isContinuousShootCoroutineRunning = true;
            playerShootContinousCoroutine = StartCoroutine(PlayerShootContinousCoroutine());
        }
    }

    // points a continuously firing weapon int he direction of player input
    virtual public IEnumerator PlayerShootContinousCoroutine()
    {
        while (isContinuousShootCoroutineRunning)
        {
            if (playerController.FacingDirection == -1)
            {
                SetPointingDirection(true);
                Debug.Log("Is recognizing player is facing left");
            }
            else { SetPointingDirection(); }

            transformToRotateAround.localPosition = initialArmPosition;

            yield return null;
        }
        ResetShootCoroutineVariables();
    }

    // this is invoked by playersecondary weapon (hence it won't show up in references) to end continous fire when the moiuse button is lifted
    public void StopContinousFire()
    {
        isContinuousShootCoroutineRunning = false;
    }
}
