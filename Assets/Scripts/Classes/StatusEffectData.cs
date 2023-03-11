using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectData
{
    public float movementModifier = .5f;

    // Burnable
    public int burnDamage = 2;
    public float burnDuration = 5;

    // Poisoned "DemonBlood"
    public int poisonedDamage = 1;
    public int poisonedDuration = 5;

    public StatusEffectData()
    {
    }
}
