using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Supports animation events on projectile's that have animations e.g. toggling the collider on and off for specific key frames
/// </summary>
public class ProjectileAnimationEventSupport : MonoBehaviour
{
    // Need to use the parent gameObject ID:
    // this script is attached to the animator component, which is a child of the projectile gameObject
    // the parent is the home of the collider and the rest of the object, and uses a different ID
    public void ColliderOn() { Debug.Log(transform.parent.gameObject.GetInstanceID()); EventSystem.current.ProjectileColliderOnTrigger(transform.parent.gameObject.GetInstanceID()); }
    public void ColliderOff() { Debug.Log(transform.parent.gameObject.GetInstanceID()); EventSystem.current.ProjectileColliderOnTrigger(transform.parent.gameObject.GetInstanceID()); }

    public void LaunchProjectile() { Debug.Log(transform.parent.gameObject.GetInstanceID()); EventSystem.current.ProjectileLaunchTrigger(transform.parent.gameObject.GetInstanceID()); }
}
