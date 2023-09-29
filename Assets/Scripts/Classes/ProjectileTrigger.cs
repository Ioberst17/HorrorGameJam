using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrigger : CombatTrigger
{
    /// <summary>
    /// Assigns a function to call when the trigger is hit
    /// </summary>
    protected override void Start()
    {
        base.Start();
        FunctionCalledOnTrigger = enemyBehaviour.ProjectileTrigger;
    }
}
