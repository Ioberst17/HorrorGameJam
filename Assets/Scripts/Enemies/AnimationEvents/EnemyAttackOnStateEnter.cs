using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackOnStateEnter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventSystem.current.EnemyActiveMeleeTrigger(animator.transform.parent.gameObject.GetInstanceID()); 
    }
}
