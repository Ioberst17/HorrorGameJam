using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Burnable : StatusEffect
{
    // These variables control the damage and duration of the burn.
    private StatusEffectData statusData = new StatusEffectData();

    public override void Start()
    {
        base.Start();

        effectDuration = statusData.burnDuration;
        damageToPass = statusData.burnDamage;
    }

    public override void Execute() { base.Execute(); }

    public override void FixedUpdate() { base.FixedUpdate(); }


}
