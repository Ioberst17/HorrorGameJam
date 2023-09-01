using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEnemyAttackOnStateExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventSystem.current.EnemyEndActiveMeleeTrigger(animator.transform.parent.gameObject.GetInstanceID());
    }
}
