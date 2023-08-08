using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimator : BodyPartAnimator
{
    // used by other player parts to sync animations as needed
    public AnimatorStateInfo currentBaseAnimatorState;
    public bool currentAnimationStateHasCompleteTag;
    public string currentAnimationStateName;

    override public void Start()
    {
        base.Start();
    }

    private void Update()
    {
        currentBaseAnimatorState = animator.GetCurrentAnimatorStateInfo(0);
        currentAnimationStateHasCompleteTag = animator.GetCurrentAnimatorStateInfo(0).IsTag("Complete");
        currentAnimationStateName = animationStatesWithHashAsKey[currentBaseAnimatorState.shortNameHash].animationName;
    }
}
