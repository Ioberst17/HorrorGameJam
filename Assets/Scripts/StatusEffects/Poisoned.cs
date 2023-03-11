using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poisoned : StatusEffect
{
    // These variables control the damage and duration of the burn.
    private StatusEffectData statusData = new StatusEffectData();
    public bool isBurning = false;

    public override void Start()
    {
        base.Start();

        effectDuration = statusData.poisonedDuration;
        damageToPass = statusData.poisonedDamage;
    }

    public override void Execute() { base.Execute(); }

    public override void Update() { base.Update(); }
}
