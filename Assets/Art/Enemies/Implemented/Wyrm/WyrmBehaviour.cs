using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyrmBehaviour : EnemyBehaviour
{
    public int projectileReferenceID = -1;

    protected override void Start() 
    { 
        base.Start();
        PatrolAnimation = ChaseAnimation = "WyrmWalk";
        ProjectileAnimation = "WyrmAttack";
        StopPursuitAtThisDistance = 3.5f;
    }

    override protected void Passover()
    {
        base.Passover();
        FlipToFacePlayer();
    }

    /// <summary>
    /// Called by ProjectileTrigger object when player is in reach
    /// </summary>
    override public void ProjectileTrigger() 
    { 
        // ensure projectileReference ID is only set once
        if(projectileReferenceID == -1)
        {
            // wyrms only use 1 projectile, it should be the first reference in their projectiles to use section
            projectileReferenceID = projectileManager.projectilesToUse[0].GetComponent<EnemyProjectile>().referenceID;
        }

        // if not found, warn
        if (projectileReferenceID == -1) { Debug.LogWarning(gameObject.name + " does not have a projectile listed in it's projectile manager"); }
        // else, shoot
        else { projectileManager.Shoot(projectileReferenceID, ProjectileAnimation); }
    }
}
