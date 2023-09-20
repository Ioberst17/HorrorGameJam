using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlamFinished : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventSystem.current.GroundSlamFinished();
    }
}
