using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

/// <summary>
/// Handles particle systems, SpriteGlowEffect.cs, and sprite changes related to the player
/// </summary>
public class PlayerVisualEffectsController : MonoBehaviour
{
    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    [Serializable]
    public class SpriteGlowContainer 
    { 
        public string name;
        public int nameHash;
        public SpriteGlowEffect spriteGlow;
        public SpriteRenderer spriteRenderer;

        public SpriteGlowContainer(string name, SpriteGlowEffect spriteGlow)
        {
            this.name = name;
            this.nameHash = name.GetHashCode();
            this.spriteGlow = spriteGlow;
            this.spriteRenderer = spriteGlow.GetComponent<SpriteRenderer>();
        }
    }

    public List<SpriteGlowContainer> spriteGlows = new List<SpriteGlowContainer>();

    // tracks if glow is on or off
    bool glowTrigger;
    // standard spriteglow values
    float glowFrequency = 5, glowAmplitude = 4;
    float outlineFrequency = 2, outlineAmplitude = 2;
    [SerializeField] int glowVal;
    [SerializeField] int outlineVal;
    int glowMidPoint = 6, outlineMidpoint = 5;

    [Serializable]
    public class SpriteRendererContainer
    {
        public string name;
        public int nameHash;
        public SpriteRenderer sprite;
        public Color startingColor;
        public Vector3 startingLocalPosition;
        public Vector3 startingLocalScale;

        public SpriteRendererContainer(string name, SpriteRenderer sprite, Color startingColor, Vector3 startingLocalPosition, Vector3 startingLocalScale)
        {
            this.name = sprite.gameObject.name;
            this.nameHash = name.GetHashCode();
            this.sprite = sprite;
            this.startingColor = startingColor;
            this.startingLocalPosition = startingLocalPosition;
            this.startingLocalScale = startingLocalScale;
        }
    }

    [SerializeField] public List<SpriteRendererContainer> sprites = new List<SpriteRendererContainer>();

    private void Awake()
    {
        FillParticleSystemList();
        GetSpriteGlowEffect();
        GetSprites();
    }

    // BELOW THIS IS PARTICLE SYSTEM FUNCTIONS

    /// <summary>
    /// Load in particle systems from hierarchy
    /// </summary>
    private void FillParticleSystemList()
    {
        particleSystems.Clear();
        ParticleSystem[] childParticleSystems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in childParticleSystems)
        {
            if (ps.transform.parent == transform) { particleSystems.Add(ps); }
            if (ps.transform.parent.gameObject.name.Contains("IndividualSystems")) { particleSystems.Add(ps); }
        }
    }

    /// <summary>
    /// Play a specific particle system based on it's a string (its gameObject name)
    /// </summary>
    /// <param name="particleSystemName"></param>
    public void PlayParticleSystem(string particleSystemName)
    {
        ParticleSystem ps = particleSystems.Find(p => p.name == particleSystemName);
        if (ps != null)
        {
            ps.Play();
        }
        else
        {
            Debug.LogWarning("Particle system with the name " + particleSystemName + " not found.");
        }
    }

    /// <summary>
    /// Coroutine to play with a delay
    /// </summary>
    /// <param name="particleSystemName"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public IEnumerator PlayParticleSystemWithDelay(string particleSystemName, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayParticleSystem(particleSystemName);
    }

    /// <summary>
    /// Turn on a particle system loop
    /// </summary>
    /// <param name="particleSystemName"></param>
    /// <param name="loop"></param>
    public void LoopParticleSystem(string particleSystemName, bool loop)
    {
        ParticleSystem ps = particleSystems.Find(p => p.name == particleSystemName);
        if (ps != null)
        {
            var mainModule = ps.main;
            mainModule.loop = loop;
        }
        else
        {
            Debug.LogWarning("Particle system with the name " + particleSystemName + " not found.");
        }
    }

    /// <summary>
    /// Stop a given particle system, given it's game object name
    /// </summary>
    /// <param name="particleSystemName"></param>
    public void StopParticleSystem(string particleSystemName)
    {
        ParticleSystem ps = particleSystems.Find(p => p.name == particleSystemName);
        if (ps != null)
        {
            ps.Stop();
        }
        else
        {
            Debug.LogWarning("Particle system with the name " + particleSystemName + " not found.");
        }
    }





    // BELOW ARE FUNCTIONS RELATED TO MANAGING SPRITEGLOWEFFECT.CS
    /// <summary>
    /// Load Sprite Glow Effects from hierarchy
    /// </summary>
    void GetSpriteGlowEffect()
    {
        spriteGlows.Clear();
        var playerAnimator = transform.parent.transform.Find("Animator");
        SpriteGlowEffect[] childSpriteGlows = playerAnimator.GetComponentsInChildren<SpriteGlowEffect>();
        foreach (SpriteGlowEffect sGE in childSpriteGlows)
        {
            spriteGlows.Add(new SpriteGlowContainer(sGE.transform.parent.gameObject.name, sGE)); 
        }
    }

    public void GlowOn(bool status)
    {
        foreach(SpriteGlowContainer sGC in spriteGlows)
        {
            if (sGC.name == "Base")
            {
                if (status == true) { glowTrigger = true; }
                else
                {
                    glowTrigger = false;
                    sGC.spriteGlow.GlowBrightness = 0;
                    sGC.spriteGlow.OutlineWidth = 0;
                }
            }
        }
    }

    public void HandleGlow()
    {
        foreach(SpriteGlowContainer sGC in spriteGlows)
        {
            // only applies to the base player sprite
            if(sGC.name == "Base")
            {
                // Calculate the oscillating values for glow and outline properties
                glowVal = Mathf.Max(1, Mathf.RoundToInt(Mathf.Sin(Time.time * glowFrequency) * glowAmplitude + glowMidPoint));
                outlineVal = Mathf.Max(1, Mathf.RoundToInt(Mathf.Sin(Time.time * outlineFrequency) * outlineAmplitude + outlineMidpoint));

                sGC.spriteGlow.GlowBrightness = glowVal;
                sGC.spriteGlow.OutlineWidth = outlineVal;
            }
        }
    }

    // BELOW ARE FUNCTIONS RELATED TO MANAGING SPRITES
    void GetSprites()
    {
        sprites.Clear();
        SpriteRenderer[] childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in childSpriteRenderers)
        {
            if (sr.transform.parent == transform) { sprites.Add(new SpriteRendererContainer(sr.gameObject.name, sr, sr.color, sr.transform.localPosition, sr.transform.localScale)); }
            if (sr.transform.parent.gameObject.name.Contains("IndividualSystems")) { sprites.Add(new SpriteRendererContainer(sr.gameObject.name, sr, sr.color, sr.transform.localPosition, sr.transform.localScale)); }
        }
    }

    public void ToggleSpriteEnabled(string spriteName, bool state)
    {
        GetSpriteContainer(spriteName, out SpriteRendererContainer spriteContainer, out SpriteRenderer sprite);
        spriteContainer.sprite.enabled = state;
    }

    public (Vector3, Vector3) GetSpriteUpperRightAndLowerLeftCorners(string spriteName)
    {
        float playerBottomThresholdInLocalUnits = -.475f;
        // get a sprite by name based on its container
        GetSpriteContainer(spriteName, out SpriteRendererContainer spriteContainer, out SpriteRenderer spriteRenderer);

        Vector3 spriteSize = spriteRenderer.bounds.size;

        Vector3 minPoint = spriteRenderer.transform.localPosition + new Vector3(-spriteSize.x / 2f, -spriteSize.y / 2f, 0f);
        Vector3 maxPoint = spriteRenderer.transform.localPosition + new Vector3(spriteSize.x / 2f, spriteSize.y / 2f, 0f);

        // Check if the y-value is below the threshold
        if (minPoint.y < playerBottomThresholdInLocalUnits)
        {
            float yDifference = playerBottomThresholdInLocalUnits - minPoint.y;
            minPoint.y += yDifference;
            maxPoint.y += yDifference;
        }

        return (maxPoint, minPoint);
    }

    public IEnumerator FadeInFadeOutSprite(string spriteName, float fadeDuration)
    {
        GetSpriteContainer(spriteName, out SpriteRendererContainer spriteContainer, out SpriteRenderer sprite);

        sprite.gameObject.SetActive(true);
        sprite.color = spriteContainer.startingColor;

        float timer = 0.0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            // Calculate the current alpha value based on the fade duration
            float alpha = Mathf.Lerp(1.0f, 0.0f, timer / fadeDuration);

            // Set the target color with the modified alpha value
            Color targetColor = new Color(spriteContainer.startingColor.r,
                                          spriteContainer.startingColor.g,
                                          spriteContainer.startingColor.b, 
                                          alpha);

            // Update the sprite's color
            sprite.color = targetColor;

            yield return null;
        }

        ResetColorAndPosition(spriteName);
    }

    public void ResetColorAndPosition(string spriteName)
    {
        GetSpriteContainer(spriteName, out SpriteRendererContainer spriteContainer, out SpriteRenderer sprite);

        // Ensure the final color is set correctly
        sprite.color = new Color(spriteContainer.startingColor.r, spriteContainer.startingColor.g, spriteContainer.startingColor.b, 0.0f);
        sprite.gameObject.SetActive(false);

        // reset position
        sprite.transform.localPosition = spriteContainer.startingLocalPosition;
        sprite.transform.localScale = spriteContainer.startingLocalScale;
    }

    public void SetSpriteLocalScale(string spriteName, Vector3 localScale) 
    {
        GetSpriteContainer(spriteName, out SpriteRendererContainer spriteContainer, out SpriteRenderer sprite);
        spriteContainer.sprite.transform.localScale = localScale;
    }

    public void SetSpriteLocalPosition(string spriteName, Vector3 localPosition) 
    {
        GetSpriteContainer(spriteName, out SpriteRendererContainer spriteContainer, out SpriteRenderer sprite);
        spriteContainer.sprite.transform.localPosition = localPosition;
    }    
    
    public void IncrementSpriteLocalPosition(string spriteName, Vector3 localPosition) 
    {
        GetSpriteContainer(spriteName, out SpriteRendererContainer spriteContainer, out SpriteRenderer sprite);
        spriteContainer.sprite.transform.localPosition += localPosition;
    }

    void GetSpriteContainer(string spriteName, out SpriteRendererContainer spriteContainer, out SpriteRenderer sprite)
    {
        int spriteHash = spriteName.GetHashCode();
        spriteContainer = sprites.Find(sr => sr.nameHash == spriteHash);
        sprite = spriteContainer.sprite;
    }

}
