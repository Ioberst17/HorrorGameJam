using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimator : BodyPartAnimator
{
    // Start is called before the first frame update
    override public void Start()
    {
        priorityAnimationStates["PlayerLand"] = new AnimationProperties("PlayerLand", 2);
        base.Start();
    }
}
