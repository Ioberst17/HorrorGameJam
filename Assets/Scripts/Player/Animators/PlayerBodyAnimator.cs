using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyAnimator : PlayerBaseAnimator
{
    // used by other player parts to sync animations as needed
    public AnimatorStateInfo currentBaseAnimatorState;
    public bool currentAnimationStateHasCompleteTag;
    PlayerController playerController;
    GroundSlam groundSlam;
    [SerializeField] protected List<string> attackAnimations = new List<string> { "PlayerBasicAttack1",
                                                                                    "PlayerBasicAttack2",
                                                                                    "PlayerBasicAttack3",
                                                                                    "PlayerCrouchAttack",
                                                                                    "PlayerNeutralAir",
                                                                                    "PlayerForwardAir",
                                                                                    "PlayerGroundSlam",
                                                                                    "PlayerChargePunch",
                                                                                    "PlayerSideKick",
                                                                                    "PlayerSideKnee",
                                                                                    "PlayerSideThrow" };

    override public void Start()
    {
        base.Start();
        playerController = GetComponentInParent<PlayerController>();
        groundSlam = FindObjectOfType<GroundSlam>();
    }

    private void Update()
    {
        currentBaseAnimatorState = animator.GetCurrentAnimatorStateInfo(0);
        currentAnimationStateHasCompleteTag = animator.GetCurrentAnimatorStateInfo(0).IsTag("Complete");
        CheckIfAttackFlagShouldBeCancelled();
    }

    /// <summary>
    /// Failsafe to ensure that player's IsAttacking status is turned off if not animation an attack
    /// </summary>
    void CheckIfAttackFlagShouldBeCancelled()
    {
        if (!attackAnimations.Contains(CurrentAnimationName))
        {
            if (playerController.IsAttacking != false) { playerController.IsAttacking = false; }
            if (previousAnimationName == "PlayerGroundSlam" ||
                (previousAnimationName != "PlayerGroundSlam" && CurrentAnimationName != "PlayerGroundSlam"))
            { groundSlam.IsGroundSlam = false; }
        }
    }
}
