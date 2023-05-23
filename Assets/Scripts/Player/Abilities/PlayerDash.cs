using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    // references to other objects
    private GameController gameController;
    [SerializeField] private PlayerController controller;

    // internal properties
    [SerializeField] private bool _canDash; public bool CanDash { get { return _canDash; } set { _canDash = value; } }
    [SerializeField] private bool _isDashing; public bool IsDashing { get { return _isDashing; } set { _isDashing = value; } }
    [SerializeField] private float _dashLength = 0.25f; public float DashLength { get { return _dashLength; } set { _dashLength = value; } }

    [SerializeField] private int _dashSpeed = 15; public int DashSpeed { get { return _dashSpeed; } set { _dashSpeed = value; } }
    [SerializeField] private int dashCooldownNumber;

    private int dashcooldown;
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        controller = FindObjectOfType<PlayerController>();
        _canDash = true;
        _isDashing = false;
        dashcooldown = 0;
    }

    private void FixedUpdate()
    {
        if (dashcooldown > 0)
        {
            dashcooldown--;
        }
    }
    //dash handling function
    public void Execute()
    {
        if (_canDash && !DialogueManager.GetInstance().DialogueIsPlaying)
        {
            _canDash = false;
            StartCoroutine(DashHandler());
        }
    }

    //This is the function that actually performs the dash
    IEnumerator DashHandler()
    {
        if(dashcooldown == 0)
        {
            _isDashing = true;
            controller.Rb.gravityScale = 0;
            if (gameController.XInput == 0 & gameController.YInput == 0)
            {
                Vector3 direction = GetNormalizedLookDirection();
                controller.SetVelocity(direction.x * controller.MovementSpeed * 2, direction.y * controller.MovementSpeed * 2);
                //newVelocity.Set(movementSpeed * 2 * facingDirection, 0);
            }
            else { HandleMultiDirectionalDash(); }

            FindObjectOfType<AudioManager>().PlaySFX("Dash1");
            //animator.Play("PlayerDash");
            yield return DashAfterImageHandler();
            _isDashing = false;
            controller.Rb.gravityScale = 3;
            controller.SetVelocity(0, 0);
            dashcooldown = dashCooldownNumber;
            
        }
        
    }
    // Handles generation of after images and dash length
    IEnumerator DashAfterImageHandler()
    {
        var startTime = Time.time;
        {
            while (_dashLength > Time.time - startTime)
            {
                PlayerAfterImageObjectPool.Instance.PlaceAfterImage(gameObject.transform);
                yield return null;
            }
        }
        yield return null;
    }

    private void HandleMultiDirectionalDash() // used if no input is being used by the player
    {
        float inputPositive = controller.MovementSpeed * 2;
        float inputNegative = controller.MovementSpeed * -2;

        if (gameController.XInput > 0 && gameController.YInput > 0) { controller.SetVelocity(inputPositive, inputPositive); }
        else if (gameController.XInput > 0 && gameController.YInput < 0) { controller.SetVelocity(inputPositive, inputNegative); }
        else if (gameController.XInput < 0 && gameController.YInput > 0) { controller.SetVelocity(inputNegative, inputPositive); }
        else if (gameController.XInput < 0 && gameController.YInput < 0) { controller.SetVelocity(inputNegative, inputNegative); }
        else if (gameController.XInput > 0) { controller.SetVelocity(inputPositive, 0); }
        else if (gameController.XInput < 0) { controller.SetVelocity(inputNegative, 0); }
        else if (gameController.YInput > 0) { controller.SetVelocity(0, inputPositive); }
        else if (gameController.YInput < 0) { controller.SetVelocity(0, inputNegative); }
    }

    public Vector3 GetNormalizedLookDirection() // either mouse or right joystick information
    {
        Vector3 direction = gameController.lookInput - gameController.playerPositionScreen;
        return direction.normalized;
    }
}
