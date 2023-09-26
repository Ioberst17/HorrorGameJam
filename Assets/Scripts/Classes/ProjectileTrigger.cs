using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrigger : CombatTrigger
{
    protected override void Start()
    {
        base.Start();
        FunctionCalledOnTrigger = enemyBehaviour.ProjectileTrigger;
    }
}
