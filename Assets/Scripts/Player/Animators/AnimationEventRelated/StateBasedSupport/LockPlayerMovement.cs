using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPlayerMovement : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventSystem.current.LockPlayerXMovementTrigger();
    }
}
