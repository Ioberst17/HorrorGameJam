using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyrmBehaviour : EnemyBehaviour
{
    protected override void Start() 
    { 
        base.Start();
        IdleAnimation = "WyrmIdle";
        PatrolAnimation = ChaseAnimation = "WyrmWalk";
        ProjectileAnimation = "WyrmAttack";
        StopPursuitAtThisDistance = 3.5f;
    }

    override protected void Passover()
    {
        base.Passover();
        FlipToFacePlayer();
    }
}
