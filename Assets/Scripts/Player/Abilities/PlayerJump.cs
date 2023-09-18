using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerJump : MonoBehaviour
{
    // references to other objects
    private PlayerController playerController;
    private PlayerDash dash;
    private PlayerAnimator animator;
    private PlayerVisualEffectsController visualEffects;

    // internal properties
    [SerializeField] private float numberOfJumps;
    [SerializeField] private bool _isJumping;  public bool IsJumping { get { return _isJumping; } set { _isJumping = value; } }
    [SerializeField] private float _jumpForce; public float JumpForce { get { return _jumpForce; } set { _jumpForce = value; } }
    [SerializeField] private bool _canJump = true; public bool CanJump { get { return _canJump; } set { _canJump = value; } } 

    void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        animator = ComponentFinder.GetComponentInChildrenByNameAndType<PlayerAnimator>("Animator", transform.parent.gameObject);
        dash = ComponentFinder.GetComponentInChildrenByNameAndType<PlayerDash>("Dash", transform.parent.gameObject);
        visualEffects = ComponentFinder.GetComponentInChildrenByNameAndType<PlayerVisualEffectsController>("VisualEffects", transform.parent.gameObject);
    }

    public void Execute()
    {
        if (!dash.IsDashing && !DialogueManager.GetInstance().DialogueIsPlaying)
        {
            if (_canJump)
            {
                _canJump = false;
                _isJumping = true;
                playerController.SetVelocity();
                playerController.AddForce(0f, _jumpForce);
                
                // VFX/SFX
                animator.Play("PlayerJump");
                PlayRandomJumpSound();
            }
            else if (playerController.IsAgainstWall && playerController.CanWallJump && !playerController.IsGrounded)
            {
                playerController.ControlMomentum = 20 * -playerController.FacingDirection;
                playerController.Flip();
                playerController.CanWallJump = false;
                _isJumping = true;
                playerController.SetVelocity();
                playerController.AddForce(_jumpForce / 2 * -playerController.FacingDirection, _jumpForce);

                // VFX/SFX
                PlayRandomJumpSound();
                animator.Play("PlayerJump");
                visualEffects.PlayParticleSystem("MovementDust");
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
