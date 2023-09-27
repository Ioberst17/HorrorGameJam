using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttack1Finished : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventSystem.current.PlayerComboTrigger(1);
    }
}
