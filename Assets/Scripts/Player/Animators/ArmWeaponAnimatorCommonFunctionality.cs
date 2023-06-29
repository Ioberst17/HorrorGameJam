using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmWeaponAnimatorCommonFunctionality : BodyPartAnimator
{

    // THIS SCRIPT CONTAINS COMMON FUNCTIONALITY FOR THE WEAPON AND ARM SPRITES
    // IT PRIMARILY REOLVES AROUND PLAYING ANIMATIONS RELATED TO SHOOTING - initial shots, shots within a timeframe, continuous fire

    [Header("References Needed for Arms and Weapons Functionality")]
    public Transform parentTransform;
    public GameController gameController;
    public PlayerController playerController;
    public PlayerSecondaryWeapon playerSecondaryWeapon;
    public WeaponDatabase weaponDatabase;
    public Weapons currentWeapon;

    [Header("Shooting Variables")]
    public Vector3 initialArmPosition;
    public bool startedShotOnLeft;
    public float elapsedTime;
    public float rotationTime = 10f;
    public float rotationRightSpeed = 25;
    public float rotationLeftSpeed = 5f;
    public float rotationSpeedToUse = 100f;
    public Quaternion shotDirectionRotation;
    public float shotRotation;
    public bool externalCallToBreakArmReturnLoop;
    public float refirePauseTime = 3f;

    public bool isShootCoroutineRunning;
    public bool isRestartedShootCoroutineRunning;
    public bool isContinuousShootCoroutineRunning;
    public Coroutine playerShootCoroutine;
    public Coroutine playerShootRestartedCoroutine;
    public Coroutine playerShootContinousCoroutine;
    public bool farAlongEnoughToReset; // what direction the hand shoot return to idle

    // regarding end condition of arm rotation
    private bool _oneHandedWeaponInUse, _twoHandedWeaponInUse;
    public bool oneHandedWeaponInUse
    {
        get { return _oneHandedWeaponInUse; }
        set
        {
            if(value == true)
            {
                _oneHandedWeaponInUse = value;
                // Set the value of the float variable
                endingArmRotation = -90; 
                // Set the another bool to false
                _twoHandedWeaponInUse = false;
            }
        }
    }
    public bool twoHandedWeaponInUse
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
    private float endingArmRotation;
    public float armFacingDownThresholdValue = 5f; // the angle that is 'reasonable' when checking if the shooting arm has returned
    public bool isArmFacingEndPosition;

    override public void Start()
    {
        base.Start();

        // Set priority animations
        priorityAnimationStates["PlayerShoot"] = new AnimationProperties("PlayerShoot", 2);
        priorityAnimationStates["PlayerShootReturn"] = new AnimationProperties("PlayerShootReturn", 2);
        priorityAnimationStates["PlayerThrow"] = new AnimationProperties("PlayerThrow", 2);

        // get key references
        gameController = FindObjectOfType<GameController>();
        playerController = FindObjectOfType<PlayerController>();
        weaponDatabase = FindObjectOfType<WeaponDatabase>();
        playerSecondaryWeapon = playerController.GetComponentInChildren<PlayerSecondaryWeapon>();
        parentTransform = transform.parent.transform;
        initialArmPosition = parentTransform.localPosition;
        EventSystem.current.onUpdateSecondaryWeaponTrigger += OnWeaponSwitch;
    }
    private void OnDestroy()
    {
        EventSystem.current.onUpdateSecondaryWeaponTrigger -= OnWeaponSwitch;
    }

    override public void AssignNewAnimations(string objectName)
    {
        base.AssignNewAnimations(objectName);
    }

    void OnWeaponSwitch(int weaponID, string weaponName, int weaponLevel)
    {
        currentWeapon = weaponDatabase.ReturnItemFromID(weaponID);
        if(currentWeapon.isOneHanded) { oneHandedWeaponInUse = true; }
        if(currentWeapon.isTwoHanded) { twoHandedWeaponInUse = true; }
    }

    // called by secondary weaponsmanager, since it's invoked it does not register a refernce
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

    // Called by PlayerSecondaryWeapon, which sends the request to PlayerAnimator
    public IEnumerator PlayerShootCoroutine()
    {
        isShootCoroutineRunning = true;
        farAlongEnoughToReset = false;
        startedShotOnLeft = false;

        if (playerController.FacingDirection == -1) { startedShotOnLeft = true; }

        // Kept because for some odd reason rotations are faster on one side vs the other. Need to debug.
        if (playerController.FacingDirection == 1) { rotationTime = rotationRightSpeed; }
        else { rotationTime = rotationLeftSpeed; }

        // Set priority of starting animation, play it
        animator.SetLayerWeight(1, 1);
        animator.Play("PlayerShoot", layer: 1);

        // update arm position to where mouse is pointed
        SetPointingDirection();
        farAlongEnoughToReset = true;

        // now wait
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Pause the animator at the current frame of the starting animation
        animator.speed = 0f;

        yield return StartCoroutine(ReturnToStartingPosition());

        PlayClosingAnimation();
        ResetShootCoroutineVariables();
    }

    // handles restarting player shoot (checks to see if an existing function is running)
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

        yield return new WaitForSeconds(refirePauseTime);

        // Pause the animator at the current frame of the starting animation
        animator.speed = 0f;

        yield return StartCoroutine(ReturnToStartingPosition());

        PlayClosingAnimation();
        ResetShootCoroutineVariables();
    }

    // Get the direction from the arm's current position to the mouse position
    virtual public void SetPointingDirection(bool optionalForceLeftFacingModification = false) // optional is used by continuous fire
    {
        Vector2 rotation = (Vector2)gameController.lookInput - (Vector2)gameController.playerPositionScreen;
        var rotationZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        if (startedShotOnLeft || optionalForceLeftFacingModification)
        {
            rotationZ = MapLeftFacingValues(rotationZ);
        }

        transform.parent.localRotation = Quaternion.Euler(0, 0, rotationZ);

        shotDirectionRotation = transform.parent.localRotation;
    }

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
            SetDirectionOverTime(transform.parent.rotation.eulerAngles.z);

            DetermineIfFinished(); 

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void SetDirectionOverTime(float currentRotationZ)
    {
        var direction = -1; // works for all standard rotations
        if (twoHandedWeaponInUse && transform.parent.localRotation.eulerAngles.z > 270) { direction = 1; } // ensure weapon goes up, if pointing down

        currentRotationZ += direction * rotationSpeedToUse * Time.deltaTime; // Adjust rotation speed based on frame rate
        
        // Assign the new rotation to the transform
        transform.parent.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, currentRotationZ);
    }

    // used to check if the arm rotation has finished setting direction over time
    private void DetermineIfFinished()
    {
        float zRotation = transform.rotation.eulerAngles.z;
        if (oneHandedWeaponInUse) { endingArmRotation = -90f; }
        else if (twoHandedWeaponInUse) { endingArmRotation = 0; }

        float angleDifference = Mathf.DeltaAngle(zRotation, endingArmRotation);

        if (Mathf.Abs(angleDifference) <= armFacingDownThresholdValue) // check if arm is within a reasonable threshold of done
        {
            isArmFacingEndPosition = true;
        }
    }

    // used when returning a single / discrete shot weapon to the player's hand
    void PlayClosingAnimation()
    {
        // Play the ending animation
        animator.speed = 1f; // Resume animator speed
        animator.Play("PlayerShootReturn", layer: 1);
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetLayerWeight(1, 0);
    }

    void ResetShootCoroutineVariables()
    {
        isShootCoroutineRunning = false;
        isRestartedShootCoroutineRunning = false;
        farAlongEnoughToReset = false;
        playerShootCoroutine = null;
        playerShootRestartedCoroutine = null;
        playerShootContinousCoroutine = null;
        // Animation sequence completed
        if (startedShotOnLeft == true) { startedShotOnLeft = false;/* PhysicsExtensions.Flip(gameObject)*/; }
        transform.parent.localPosition = initialArmPosition;
        transform.parent.localRotation = new Quaternion();
    }

    public void PlayerShootContinuous()
    {
        if (playerShootContinousCoroutine != null) { return; }
        else 
        {
            isContinuousShootCoroutineRunning = true;
            playerShootContinousCoroutine = StartCoroutine(PlayerShootContinousCoroutine()); 
        }
    }

    virtual public IEnumerator PlayerShootContinousCoroutine()
    {
        while(isContinuousShootCoroutineRunning) 
        {
            if (playerController.FacingDirection == -1) 
            { 
                SetPointingDirection(true);
                Debug.Log("Is recognizing player is facing left");
            }
            else { SetPointingDirection(); }

            transform.parent.localPosition = initialArmPosition;

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
