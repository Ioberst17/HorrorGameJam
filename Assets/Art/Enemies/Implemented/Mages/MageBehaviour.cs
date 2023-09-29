using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBehaviour : EnemyBehaviour
{
    protected override void Start()
    {
        base.Start();
        IdleAnimation = "MageIdle";
        PatrolAnimation = ChaseAnimation = "MageRun";
        StopPursuitAtThisDistance = 1;
    }

    override public void AttackTrigger()
    {
        if (!enemyController.IsAttackingOrChargingAttack)
        {
            attackManager.StartAttack(0, "MageAttack");
        }
    }

    //override protected void Passover()
    //{
    //    base.Passover();
    //    FlipToFacePlayer();
    //}
}
