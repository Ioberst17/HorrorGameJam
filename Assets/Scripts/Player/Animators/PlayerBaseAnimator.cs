using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets up basic information to pass onto child player animators
/// </summary>
public class PlayerBaseAnimator : ObjectAnimator
{
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();

        // update base animation hierarchy
        UpdateAnimationStatePriority("PlayerJump", -1);
        UpdateAnimationStatePriority("PlayerWallLand", -1);
        UpdateAnimationStatePriority("PlayerRun", -2);
        UpdateAnimationStatePriority("PlayerIdle", -3);
        UpdateAnimationStatePriority("PlayerCrouch", -3);
    }

    protected override void BuildAnimationStateDictionary()
    {
        // Get the current Animator Controller (runtimeAnimatorController)
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        // Now, populate the dictionaries with the animation state names and priority levels
        animationStates.Clear();
        animationStatesWithHashAsKey.Clear();

        foreach (string state in playerAnimationStateList.animationStates)
        {
            var animationProperties = new AnimationProperties(state, 0);
            animationStates.Add(state, animationProperties);
            animationStatesWithHashAsKey.Add(animationProperties.animationHash, animationProperties);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
