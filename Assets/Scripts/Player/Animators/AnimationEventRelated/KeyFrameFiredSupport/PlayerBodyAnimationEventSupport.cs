using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyAnimationEventSupport : MonoBehaviour
{
    // these are called from animations
    public void ChargePunchRelease() { EventSystem.current.ChargePunchTrigger(); } // base charge punch animation
    public void GroundSlamDrop() { EventSystem.current.GroundSlamDropTrigger(); } // base ground slam animation
    public void ReleaseThrow() { EventSystem.current.ThrowWeaponTrigger(); } // base release throw animation

    // for melees
    public void InActiveMelee() { EventSystem.current.ActiveMeleeTrigger(); }
    public void EndInActiveMelee() { EventSystem.current.EndActiveMeleeTrigger(); }
}
