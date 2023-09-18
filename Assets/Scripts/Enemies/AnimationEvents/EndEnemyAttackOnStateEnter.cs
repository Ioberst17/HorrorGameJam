using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEnemyAttackOnStateEnter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventSystem.current.EnemyEndActiveMeleeTrigger(animator.transform.parent.gameObject.GetInstanceID());
    }
}
