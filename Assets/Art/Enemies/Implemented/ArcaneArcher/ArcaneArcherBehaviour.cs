using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneArcherBehaviour : EnemyBehaviour
{
    protected override void Start()
    {
        base.Start();
        IdleAnimation = "ArcaneArcherIdle";
        PatrolAnimation = ChaseAnimation = "ArcaneArcherRun";
        ProjectileAnimation = "ArcaneArcherShoot";
        StopPursuitAtThisDistance = 5;
    }

    override protected void Passover()
    {
        base.Passover();
        FlipToFacePlayer();
    }
}
