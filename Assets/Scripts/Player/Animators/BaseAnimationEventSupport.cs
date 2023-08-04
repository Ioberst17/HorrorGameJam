using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimationEventSupport : MonoBehaviour
{
    // these are called from animations
    public void ChargePunchRelease() { EventSystem.current.ChargePunchTrigger(); } // base charge punch animation
    public void GroundSlamDrop() { EventSystem.current.GroundSlamDropTrigger(); } // base ground slam animation
}
