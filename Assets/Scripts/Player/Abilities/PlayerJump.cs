using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerJump : MonoBehaviour
{
    // references to other objects
    private GameController gameController;
    private PlayerController controller;
    private PlayerDash dash;
    private PlayerAnimator animator;
    private PlayerParticleSystems visualEffects;

    // internal properties
    [SerializeField] private float numberOfJumps;
    [SerializeField] private bool _isJumping;  public bool IsJumping { get { return _isJumping; } set { _isJumping = value; } }
    [SerializeField] private float _jumpForce; public float JumpForce { get { return _jumpForce; } set { _jumpForce = value; } }
    [SerializeField] private bool _canJump = true; public bool CanJump { get { return _canJump; } set { _canJump = value; } } 

    // Start is called before the first frame update
    void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        controller = FindObjectOfType<PlayerController>();
        animator = ComponentFinder.GetComponentInChildrenByNameAndType<PlayerAnimator>("Animator", transform.parent.gameObject);
        dash = ComponentFinder.GetComponentInChildrenByNameAndType<PlayerDash>("Dash", transform.parent.gameObject);
        visualEffects = controller.transform.Find("VisualEffects").gameObject.GetComponent<PlayerParticleSystems>();
    }

    public void Execute()
    {
        if (!dash.IsDashing && !DialogueManager.GetInstance().DialogueIsPlaying)
        {
            if (_canJump)
            {
                _canJump = false;
                _isJumping = true;
                controller.SetVelocity();
                controller.AddForce(0f, _jumpForce);
                
                // VFX/SFX
                animator.Play("PlayerJump");
                PlayRandomJumpSound();
            }
            else if (controller.IsAgainstWall && controller.CanWallJump && !controller.IsGrounded)
            {
                controller.ControlMomentum = 20 * -controller.FacingDirection;
                controller.Flip();
                controller.CanWallJump = false;
                _isJumping = true;
                controller.SetVelocity();
                controller.AddForce(_jumpForce / 2 * -controller.FacingDirection, _jumpForce);

                // VFX/SFX
                PlayRandomJumpSound();
                animator.Play("PlayerJump");
                controller.visualEffects.PlayParticleSystem("MovementDust");
            }
        }
    }

    public void PlayRandomJumpSound()
    {
        int jumpAssetChoice = Random.Range(1, 9);
        string jumpAssetToUse = "Jump" + jumpAssetChoice.ToString();
        FindObjectOfType<AudioManager>().PlaySFX(jumpAssetToUse);
    }
}
