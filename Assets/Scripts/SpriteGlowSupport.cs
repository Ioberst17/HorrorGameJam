using SpriteGlow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGlowSupport : MonoBehaviour
{
    [SerializeField] private SpriteGlowEffect spriteGlow; private bool glowTrigger;
    float glowFrequency = 5, glowAmplitude = 5;
    float outlineFrequency = 2, outlineAmplitude = 2;
    [SerializeField] int glowVal;
    [SerializeField] int outlineVal;
    int glowMidPoint = 5, outlineMidpoint = 5;

    void HandleGlow()
    {
        // Calculate the oscillating values for glow and outline properties
        glowVal = Mathf.Max(1, Mathf.RoundToInt(Mathf.Sin(Time.time * glowFrequency) * glowAmplitude + glowMidPoint));
        outlineVal = Mathf.Max(1, Mathf.RoundToInt(Mathf.Sin(Time.time * outlineFrequency) * outlineAmplitude + outlineMidpoint));

        spriteGlow.GlowBrightness = glowVal;
        spriteGlow.OutlineWidth = outlineVal;
    }

    void GlowOn(bool status)
    {
        if (status == true) { glowTrigger = true; }
        else { glowTrigger = false; spriteGlow.GlowBrightness = 0; spriteGlow.OutlineWidth = 0; }
    }
}
