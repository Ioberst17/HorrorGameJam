using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Placed on the same object as the PlayerBody animator. It supports keyframe animation events, by being triggered from a keyframe of an animation
/// </summary>
public class PlayerBodyAnimationEventSupport : MonoBehaviour
{
    // these are called from animations
    public void ChargePunchRelease() { EventSystem.current.ChargePunchTrigger(); } // base charge punch animation
    public void GroundSlamDrop() { EventSystem.current.GroundSlamDropTrigger(); } // base ground slam animation

    public void GroundSlamFinished() { EventSystem.current.GroundSlamFinished(); }
    public void ReleaseThrow() { EventSystem.current.ThrowWeaponTrigger(); } // base release throw animation

    // for melees
    public void InActiveMelee() { EventSystem.current.ActiveMeleeTrigger(); }
    public void EndInActiveMelee() { EventSystem.current.EndActiveMeleeTrigger(); }

    // for movement

    public void LockXDirectionMovement() { EventSystem.current.LockPlayerXMovementTrigger(); Debug.Log("Player XMovement locked"); }
    public void UnlockXDirectionMovement() { EventSystem.current.UnlockPlayerXMovementTrigger(); Debug.Log("Player XMovement unlocked"); }
}
