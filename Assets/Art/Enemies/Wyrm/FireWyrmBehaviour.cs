using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWyrmBehaviour : EnemyBehaviour
{
    protected override void Start()
    {
        base.Start();
        patrolAnimation = chaseAnimation = "FireWyrmWalk";
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
        projectileManager.Shoot(2, "FireWyrmAttack");
    }
}
