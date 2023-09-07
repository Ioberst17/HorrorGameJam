using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerBehaviour : EnemyBehaviour
{
    protected override void Start()
    {
        base.Start();
        patrolAnimation = chaseAnimation = "DeathBringerWalk";
    }

    /// <summary>
    /// Called by AttackTrigger object when player is in reach
    /// </summary>
    override public void AttackTrigger()
    {
        if (!(enemyController.IsAttackingOrChargingAttack))
        {
            attackManager.StartAttack(0, "DeathBringerSlash");
        }
    }

    /// <summary>
    /// Called by ProjectileTrigger object when player is in reach
    /// </summary>
    override public void ProjectileTrigger()
    {
        if (!(enemyController.IsAttackingOrChargingAttack))
        {
            projectileManager.Shoot(1, "DeathBringerCast");
        }
    }
}
