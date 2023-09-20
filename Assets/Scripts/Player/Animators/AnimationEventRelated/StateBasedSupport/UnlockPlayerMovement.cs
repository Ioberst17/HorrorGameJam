using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockPlayerMovement : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventSystem.current.UnlockPlayerXMovementTrigger();
    }
}
