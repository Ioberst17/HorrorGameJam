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

    [SerializeField] float glowDuration = 1.0f;
    [SerializeField] bool glowLoop = false;
    private bool isGlowing = false;
    private float glowTimer = 0.0f;
    private float initialGlowBrightness = 0;
    private int initialOutlineWidth = 0;

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

    public void PlayGlow()
    {
        Debug.Log("Attempting play glow");
        initialGlowBrightness = spriteGlow.GlowBrightness;
        initialOutlineWidth = spriteGlow.OutlineWidth;
        isGlowing = true;
        glowTimer = 0.0f;
    }

    private void Update()
    {
        if (isGlowing)
        {
            HandleGlow();

            glowTimer += Time.deltaTime;
            if (glowTimer >= glowDuration)
            {
                if (glowLoop)
                {
                    glowTimer = 0.0f;
                    spriteGlow.GlowBrightness = initialGlowBrightness;
                    spriteGlow.OutlineWidth = initialOutlineWidth;
                }
                else
                {
                    isGlowing = false;
                    spriteGlow.GlowBrightness = initialGlowBrightness;
                    spriteGlow.OutlineWidth = initialOutlineWidth;
                }
            }
        }
    }
}
