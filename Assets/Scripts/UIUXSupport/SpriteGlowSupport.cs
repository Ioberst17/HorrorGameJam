using SpriteGlow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteGlowEffect))]
public class SpriteGlowSupport : MonoBehaviour
{
    [SerializeField] private SpriteGlowEffect spriteGlow;
    [SerializeField] float glowFrequency = 5; 
    [Range(0f, 5f)] [SerializeField] float glowAmplitude = 5;
    [SerializeField] float outlineFrequency = 2;
    [SerializeField] float outlineAmplitude = 2;
    [SerializeField] int glowVal;
    [SerializeField] int outlineVal;
    [Range(0f, 10f)][SerializeField] float glowMidPoint = 5;
    [Range(0f, 10f)][SerializeField] float outlineMidpoint = 5;

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

    public void PlayGlow()
    {
        initialGlowBrightness = spriteGlow.GlowBrightness;
        initialOutlineWidth = spriteGlow.OutlineWidth;
        isGlowing = true;
        glowTimer = 0.0f;
    }

    private void Update()
    {
        if (glowLoop)
        {
            HandleGlow();
        }
        if (isGlowing)
        {
            HandleGlow();

            glowTimer += Time.deltaTime;
            if (glowTimer >= glowDuration)
            {
                isGlowing = false;
                spriteGlow.GlowBrightness = initialGlowBrightness;
                spriteGlow.OutlineWidth = initialOutlineWidth;
            }
        }
    }
}
