using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// State behaviour to destroy and object after an animation is complete
/// </summary>
public class DestroyObjectOnStateExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.transform.parent.gameObject != null) { Destroy(animator.transform.parent.gameObject); }
        else { Destroy(animator.transform.gameObject); }
    }
}
