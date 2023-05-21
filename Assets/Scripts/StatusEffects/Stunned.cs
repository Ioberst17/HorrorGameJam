using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunned : StatusEffect
{
    // These variables control the damage and duration of the effect
    private StatusEffectData statusData = new StatusEffectData();

    public override void Start()
    {
        base.Start();

        effectDuration = statusData.stunnedDuration;
        nameOfSFXToPlay = statusData.stunnedSFXName;
        loopSFX = statusData.shouldLoopStunned;
        affectsMovement = true;
    }

    public override void Execute() { base.Execute(); }

    public override void FixedUpdate() { base.FixedUpdate(); }
}
