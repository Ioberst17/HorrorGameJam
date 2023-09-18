using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Frozen : StatusEffect
{
    // These variables control the damage and duration of the burn.
    private StatusEffectData statusData = new StatusEffectData();

    public override void Start()
    {
        base.Start();

        effectDuration = statusData.freezeDuration;
        damageToPass = statusData.freezeDamage;
        affectsMovement = true;
        useAnimator = true;
    }

    public override void Execute() { base.Execute(); }

    public override void FixedUpdate() { base.FixedUpdate(); }
}
