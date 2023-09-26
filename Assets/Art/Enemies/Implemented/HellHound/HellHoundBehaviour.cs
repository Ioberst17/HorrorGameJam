using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class HellHoundBehaviour : EnemyBehaviour
{
    protected override void Start()
    {
        base.Start();
        PatrolAnimation = ChaseAnimation = "HellHoundRun";
    }

    /// <summary>
    /// Called by AttackTrigger object when player is in reach
    /// </summary>
    override public void AttackTrigger()
    {
        if (!(enemyController.IsAttackingOrChargingAttack) && enemyController.RB.velocity.y < 0.25f)
        {
            enemyController.SetVelocity(0.25f * enemyController.FacingDirection, null);
            attackManager.StartChargeAttack(0, "HellHoundBite", "HellHoundStand");
        }
    }
}
