using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    // references to other objects
    private GameController gameController;
    [SerializeField] private PlayerController controller;
    // vfx references
    private GameObject visualEffects;
    [SerializeField] ParticleSystem dashParticles;

    // internal properties
    [SerializeField] private bool _canDash; public bool CanDash { get { return _canDash; } set { _canDash = value; } }
    [SerializeField] private bool _isDashing; public bool IsDashing { get { return _isDashing; } set { _isDashing = value; } }
    [SerializeField] private float _dashLength = 0.25f; public float DashLength { get { return _dashLength; } set { _dashLength = value; } }

    [SerializeField] private int _dashSpeed = 15; public int DashSpeed { get { return _dashSpeed; } set { _dashSpeed = value; } }
    [SerializeField] private int dashCooldownNumber;


    public int dashcooldown;


    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        controller = FindObjectOfType<PlayerController>();
        visualEffects = transform.GetSibling("VisualEffects").gameObject;
        dashParticles = ComponentFinder.GetComponentInChildrenByNameAndType<ParticleSystem>("DashParticleEffect", visualEffects, true);
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
            FindObjectOfType<AudioManager>().PlaySFX("Dash1");
            dashParticles.Play();
            if (gameController.XInput == 0 & gameController.YInput == 0)
            {
                controller.SetVelocity(controller.MovementSpeed * 2 * controller.FacingDirection, 0);
            }
            else { HandleMultiDirectionalDash(); }
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
        if (gameController.PlayerInput.currentControlScheme == "Keyboard & Mouse")
        {
            float baseInput = controller.MovementSpeed * 2;
            controller.SetVelocity(gameController.XInput * baseInput, gameController.YInput * baseInput);

            //if (gameController.XInput > 0 && gameController.YInput > 0) { controller.SetVelocity(gameController.XInput * baseInput, gameController.YInput* baseInput); }
            //else if (gameController.XInput > 0 && gameController.YInput < 0) { controller.SetVelocity(gameController.XInput * baseInput, gameController.YInput * baseInput); }
            //else if (gameController.XInput < 0 && gameController.YInput > 0) { controller.SetVelocity(gameController.XInput * baseInput, gameController.YInput * baseInput); }
            //else if (gameController.XInput < 0 && gameController.YInput < 0) { controller.SetVelocity(gameController.XInput * baseInput, gameController.YInput * baseInput); }
            //else if (gameController.XInput > 0) { controller.SetVelocity(gameController.XInput * baseInput, 0); }
            //else if (gameController.XInput < 0) { controller.SetVelocity(gameController.XInput * baseInput, 0); }
            //else if (gameController.YInput > 0) { controller.SetVelocity(0, gameController.YInput * baseInput); }
            //else if (gameController.YInput < 0) { controller.SetVelocity(0, gameController.YInput * baseInput); }
        }
        else if (gameController.PlayerInput.currentControlScheme == "Gamepad")
        {
            controller.SetVelocity(gameController.XInput * controller.MovementSpeed * 2, gameController.YInput * controller.MovementSpeed * 2);
        }
    }

    public Vector3 GetNormalizedLookDirection() // either mouse or right joystick information
    {
        Vector3 direction = gameController.lookInput - gameController.playerPositionScreen;
        return direction.normalized;
    }
}
