using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectData
{
    public float movementModifier = .5f;

    // Burnable
    public int burnDamage = 3;
    public float burnDuration = 5;

    // Frozen
    public int freezeDamage = 1;
    public float freezeDuration = 5;

    // Poisoned "DemonBlood"
    public int poisonedDamage = 1;
    public int poisonedDuration = 5;

    // Stunned
    public int stunnedDuration = 3;
    public string stunnedSFXName = "Stunned";
    public bool shouldLoopStunned = true;


    public StatusEffectData()
    {
    }
}
