using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static ComponentFinder;
using static UnityEngine.GraphicsBuffer;

public class RightArmAnimator : BodyPartAnimator
{
    GameController gameController;
    PlayerController playerController;
    Transform parentTransform;

    [SerializeField] private bool isCoroutineRunning;
    [SerializeField] private bool isRestartedCoroutineRunning;
    Coroutine restartedCoroutine;

    Vector3 initialArmPosition;
    bool startedShotOnLeft;
    [SerializeField] private float rotationTime = 10f;
    [SerializeField] private float rotationRight = 2500;
    [SerializeField] private float rotationLeft = 100f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private Quaternion shotDirectionRotation;
    [SerializeField] private Vector2 mouseDirection;
    [SerializeField] private Vector2 mouseDirectionMod;
    [SerializeField] private float mouseDirectionModVal;
    [SerializeField] private Quaternion endRotation;
    [SerializeField] private Quaternion endOfShotRotation = new Quaternion(0, 0, -90f, 1);
    [SerializeField] private float elapsedTime;
    [SerializeField] float shotRotation;
    [SerializeField] float shotRotationMod;
    [SerializeField] Vector3 rotationAxis;
    [SerializeField] bool handReturnClockwise = true; // what direction the hand shoot return to idle
    [SerializeField] bool farAlongEnoughToReset; // what direction the hand shoot return to idle
    public float thresholdValue; // the angle that is 'reasonable' when checking if the shooting arm has returned
    public bool isArmFacingDown;
    public bool rotationAxisToggle1 = true;
    public bool rotationAxisToggle2 = true;
    public bool toggle3 = true;

    override public void Start()
    {
        priorityAnimationStates.Add("PlayerShoot");
        priorityAnimationStates.Add("PlayerShootReturn");
        base.Start();

        gameController = FindObjectOfType<GameController>();
        playerController = FindObjectOfType<PlayerController>();
        parentTransform = transform.parent.transform;
        initialArmPosition = parentTransform.localPosition;
    }

    public IEnumerator PlayerShoot()
    {
        if (isCoroutineRunning) // if there's a current coroutine running
        {
            if (farAlongEnoughToReset) // check if it's far along enough to reset
            {
                RestartPlayerShoot(); // run a reset shoot couroutine
                StopCoroutine(PlayerShoot()); // and stop the initializer playershoot coroutine
                if (ReturnArmToSide() != null) { StopCoroutine(ReturnArmToSide()); }
                yield break;
            }
            else { yield break;  } // else do nothing
        }

        // else start
        isCoroutineRunning = true;
        farAlongEnoughToReset = false;
        startedShotOnLeft = false;
        if (playerController.FacingDirection == -1) { startedShotOnLeft = true; /* PhysicsExtensions.Flip(gameObject);*/ }
        Debug.Log("Animation started");

        if(playerController.FacingDirection == 1) { rotationTime = rotationRight; }
        else { rotationTime = rotationLeft; }

        // Set priority of starting animation, play it
        animator.SetLayerWeight(1, 1);
        animator.Play("PlayerShoot", layer: 1);

        // update arm position to where mouse is pointed
        SetArmDirection();
        farAlongEnoughToReset = true;

        // now wait
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Pause the animator at the current frame of the starting animation
        animator.speed = 0f;

        yield return StartCoroutine(ReturnArmToSide());

        PlayClosingAnimation();
        ResetShootCoroutineVariables();
    }

    // handles restarting player shoot (checks to see if an existing function is running)
    private void RestartPlayerShoot()
    {
        if (isRestartedCoroutineRunning)
        {
            StopCoroutine(restartedCoroutine);
            if(ReturnArmToSide() != null) { StopCoroutine(ReturnArmToSide()); }
        }
        restartedCoroutine = StartCoroutine(RestartPlayerShootCoroutine());
    }

    // a shortened version of the initial player shoot without the startup animation
    private IEnumerator RestartPlayerShootCoroutine()
    {
        isCoroutineRunning = true;
        isRestartedCoroutineRunning = true;

        SetArmDirection();

        yield return new WaitForSeconds(1f);

        // Pause the animator at the current frame of the starting animation
        animator.speed = 0f;

        yield return StartCoroutine(ReturnArmToSide());

        PlayClosingAnimation();
        ResetShootCoroutineVariables();
    }

    // Get the direction from the arm's current position to the mouse position
    private void SetArmDirection()
    {
        Vector2 rotation = (Vector2)gameController.lookInput - (Vector2)gameController.playerPositionScreen;
        var rotationZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.parent.rotation = Quaternion.Euler(0, 0, rotationZ);

        shotDirectionRotation = transform.parent.rotation;
    }

    // Rotate the arm down from rotation angle
    IEnumerator ReturnArmToSide()
    {
        isArmFacingDown = false;
        elapsedTime = 0f;
        while (elapsedTime < rotationTime && !isArmFacingDown)
        {
            AdjustArmRotationOverTime(transform.parent.rotation.eulerAngles.z);

            DetermineIfFacingDown(); // end condition

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void AdjustArmRotationOverTime(float currentRotationZ)
    {
        var direction = -1; // -1 means moving counterclockwise; 1 for clockwise
        if (startedShotOnLeft) { direction = 1; }

        currentRotationZ += direction * rotationSpeed * Time.deltaTime; // Adjust rotation speed based on frame rate
                                                            // Assign the new rotation to the transform
        transform.parent.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, currentRotationZ);
    }

    private void DetermineIfFacingDown()
    {
        float zRotation = transform.rotation.eulerAngles.z;
        float targetZ = -90f;

        float angleDifference = Mathf.DeltaAngle(zRotation, targetZ);

        if (Mathf.Abs(angleDifference) <= thresholdValue)
        {
            isArmFacingDown = true;
        }
    }

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
        isCoroutineRunning = false;
        isRestartedCoroutineRunning = false;
        farAlongEnoughToReset = false;
        // Animation sequence completed
        if (startedShotOnLeft == true) { startedShotOnLeft = false;/* PhysicsExtensions.Flip(gameObject)*/; }
        transform.parent.localPosition = initialArmPosition;
        transform.parent.localRotation = new Quaternion();
        Debug.Log("Animation sequence completed!");
    }
}
