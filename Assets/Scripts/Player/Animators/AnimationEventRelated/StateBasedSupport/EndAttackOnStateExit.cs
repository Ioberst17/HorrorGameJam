using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAttackOnStateExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventSystem.current.EndActiveMeleeTrigger();
    }
}
