using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttack3Finished : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventSystem.current.PlayerComboTrigger(3);
    }
}
