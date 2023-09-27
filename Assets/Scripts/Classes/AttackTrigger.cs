using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : CombatTrigger
{
    protected override void Start()
    {
        base.Start();
        FunctionCalledOnTrigger = enemyBehaviour.AttackTrigger;
    }
}
