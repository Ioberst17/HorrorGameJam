using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material glowMaterial;
    public bool glowOn;
    private Texture2D spriteSheetTexture;

    private void Start()
    {
        GetHierarchyReferences();
    }

    private void GetHierarchyReferences()
    {
        spriteRenderer = GameObject.Find("PlayerModel").GetComponent<SpriteRenderer>();
        // Assign the glow material to the SpriteRenderer
        spriteRenderer.material = glowMaterial;
    }

    private void SetGlowIntensity(float intensity)
    {
        glowMaterial.SetFloat("_GlowIntensity", intensity);
    }

    private void Update()
    {
        if (glowOn)
        {
            // Set the glow intensity to a value greater than zero to enable the effect
            SetGlowIntensity(0.5f);
        }
        else
        {
            // Set the glow intensity to zero to disable the effect
            SetGlowIntensity(0f);
        }
    }
}
