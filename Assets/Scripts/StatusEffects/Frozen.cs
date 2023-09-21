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
        animationStartState = "IceStart";
        animationEndState = "IceMelt";
    }

    public override void Execute() { base.Execute(); }

    public override void FixedUpdate() { base.FixedUpdate(); }

    /// <summary>
    /// The Freeze status effect handles its VFX separately vs. other status effects, which use the base implamentation
    /// </summary>
    /// <param name="state"></param>
    protected override void VFXHandler(bool state)
    {
        if (state == true) 
        {
            spriteRenderer.enabled = true;
            AnimationHandler(state);
        }
        else 
        { 
            AnimationHandler(state);
        }
    }
}
