using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SiblingComponentUtils;
using SpriteGlow;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerPrimaryWeapon))]
public class ChargePunch : MonoBehaviour
{
    // external references
    PlayerPrimaryWeapon playerPrimaryWeapon;
    AudioManager audioManager;

    // internal variables
    public float maxChargeTime = 2f;      // the maximum time the punch can be charged for
    public float chargeSpeed = 1f;        // the speed at which the punch charge increases
    private float minChargeTime = 0.35f; // minimum time before charge counts
    [SerializeField] public int damageToPass;
    [SerializeField] private float maxDamageToAdd = 10f;
    [SerializeField] private float holdTimeNormalized;
    [SerializeField] int attackDirection;

    [SerializeField] private float chargeTime; // the current charge time
    [SerializeField] private bool _isCharging; public bool IsCharging { get { return _isCharging; } set { _isCharging = value; } } // whether the punch is currently being charged


    // MODIFY COLLIDERS BASED ON CHARGE LENGTH, update based on the size of the charge punch sprite
    [SerializeField] private Vector3 _upperRightCorner; public Vector3 UpperRightCorner { get { return _upperRightCorner; } set { _upperRightCorner = value; } }
    [SerializeField] private Vector3 _bottomLeftCorner; public Vector3 BottomLeftCorner { get { return _bottomLeftCorner; } set { _bottomLeftCorner = value; } }


    [Header("VFX Related")]
    // particle system related
    private GameObject visualEffects;
    private GameObject particleParent;
    ParticleSystem partSystem1, partSystem2, partSystem3, partSystem4, partSystem5;
    ParticleSystem[] particleSystems;
    bool particleTrigger;

    // player glow related
    [SerializeField] private SpriteGlowEffect spriteGlow; private bool glowTrigger;
    float glowFrequency = 5, glowAmplitude = 5;
    float outlineFrequency = 2, outlineAmplitude = 2;
    [SerializeField] int glowVal;
    [SerializeField] int outlineVal;
    int glowMidPoint = 5, outlineMidpoint = 5;
    
    // charge punch sprite related
    public SpriteRenderer PunchSprite { get; set; }
    Bounds punchSpriteBounds;
    public float maxScale = 2.0f; // SETS MAX SCALE OF SPRITE IN CHARGING, IMPORTANT FOR COLLISIONS
    public float offsetAmount = 1.0f;
    private float visibleDuration = 0.2f;
    private Color originalColor;
    private Vector2 originalLocalPosition;
    private float fadeDuration = 0.5f;


    //// charge punch sprite animation related
    //[SerializeField] Animator animator;
    //private string animationName = "ChargePunchAnimation";
    //[SerializeField] private bool playAnimation = false;
    //[SerializeField] private bool animationPlayed = false;
    //Vector3 animationPosition;

    //SFX Related
    private bool falconSFXFlag;
    private bool falconSFXPlaying;
    private float punch1Length;
    private float sfxPlayTime;
    private float additionalSFXWaitTime;

    private void Start()
    {
        playerPrimaryWeapon = GetComponent<PlayerPrimaryWeapon>();
        LoadVFXReferences();
        audioManager = FindObjectOfType<AudioManager>();
        Sound punch1Sound = audioManager.GetSFX("ChargePunch1");
        punch1Length = punch1Sound.clip.length;
        ParticleSystemsOn(false);
    }

    void LoadVFXReferences()
    {
        // Particles
        visualEffects = transform.GetSibling("VisualEffects").gameObject;
        particleParent = visualEffects.transform.Find("PunchWindParticles").gameObject;
        particleSystems = particleParent.GetComponentsInChildren<ParticleSystem>();
        // Glow
        spriteGlow = visualEffects.GetComponentInParent<SpriteGlowEffect>();
        // Sprite
        PunchSprite = ComponentFinder.GetComponentInChildrenByNameAndType<SpriteRenderer>("ChargePunchSprite", gameObject, true);
        PunchSprite.gameObject.SetActive(true);
        originalColor = PunchSprite.color;
        originalLocalPosition = PunchSprite.transform.localPosition;
        PunchSprite.gameObject.SetActive(false);
        // Animation
        //animator = ComponentFinder.GetComponentInChildrenByNameAndType<Animator>(animationName, visualEffects, true);
    }

    public void Execute() 
    { 
        IsCharging = true;
        // sprite renderer
        PunchSprite.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        PunchSprite.transform.localPosition += new Vector3(offsetAmount, 0.0f, 0.0f);
        // animation
        //animationPlayed = false;
    }

    public void Release(int attackDirection) { this.attackDirection = attackDirection; IsCharging = false; ReleasePunch(); /*animator.SetTrigger("ChargePunchRelease"); playAnimation = true;*/ } // punch is released by LateUpdate(), so animation can finish playing

    void FixedUpdate()
    {
        if (IsCharging)
        {
            chargeTime = chargeTime + (Time.deltaTime * chargeSpeed);
            CalcForce();
            HandleSpriteSize();
            HandleChargeVFX();
            HandleChargeSound();
            if (glowTrigger) { HandleGlow(); }

        }
        if (falconSFXPlaying) { sfxPlayTime += Time.deltaTime; }
    }

    private void LateUpdate() // handles animation playing
    {
        //ReleasePunchAnimation();
    }

    private void CalcForce()
    {
        holdTimeNormalized = Mathf.Clamp01(chargeTime / maxChargeTime);
        damageToPass = (int)(holdTimeNormalized * maxDamageToAdd);
    }

    private void HandleSpriteSize() // scales with charge punch time
    {
        float scale = Mathf.Lerp(1.0f, maxScale, chargeTime / maxChargeTime);
        PunchSprite.transform.localScale = new Vector3(scale, scale, 1.0f);
    }

    private void HandleChargeSound()
    {
        if (chargeTime > minChargeTime)
        {
            if (falconSFXFlag == false)
            {
                audioManager.PlaySFX("ChargePunch1");
                audioManager.LoopSFX("ChargePunchBackground", true);
                audioManager.LoopSFX("ChargePunchBackground2", true);
                falconSFXFlag = true; falconSFXPlaying = true;
            }
        }
    }

    void HandleFinishSound()
    {
        if (falconSFXFlag == true)
        {
            additionalSFXWaitTime = punch1Length - sfxPlayTime;
            if (additionalSFXWaitTime > 0.00f) { Invoke("FinishSound", additionalSFXWaitTime); }
            else { Invoke("FinishSound", 0); }
        }
    }

    void HandleGlow()
    {
        // Calculate the oscillating values for glow and outline properties
        glowVal = Mathf.Max(1, Mathf.RoundToInt(Mathf.Sin(Time.time * glowFrequency) * glowAmplitude + glowMidPoint));
        outlineVal = Mathf.Max(1, Mathf.RoundToInt(Mathf.Sin(Time.time * outlineFrequency) * outlineAmplitude + outlineMidpoint));

        spriteGlow.GlowBrightness = glowVal;
        spriteGlow.OutlineWidth = outlineVal;
    }

    // Invoked in Handle Finish Sound, although reference says 0; this is due to Invoke allowing a wait time before executing
    void FinishSound() 
    {
        audioManager.LoopSFX("ChargePunchBackground", false);
        audioManager.LoopSFX("ChargePunchBackground2", false);
        audioManager.PlaySFX("ChargePunch2"); falconSFXFlag = false; sfxPlayTime = 0; falconSFXPlaying = false; 
    }

    private void HandleChargeVFX() 
    {
        if (chargeTime > minChargeTime) 
        {
            EventSystem.current.StartChargedAttackTrigger(holdTimeNormalized, gameObject.transform, null);
            if (particleTrigger == false) { ParticleSystemsOn(true); }
            if (glowTrigger == false) { GlowOn(true); spriteGlow.GlowBrightness = 0; spriteGlow.OutlineWidth = 0; }
        } 
    }

    void UpdateSpriteBounds()
    {
        punchSpriteBounds = PunchSprite.bounds;
        UpperRightCorner = punchSpriteBounds.max;
        BottomLeftCorner = punchSpriteBounds.min;
        PunchSprite.enabled = true; // enabled so that PlayerPrimaryWeapon can validate that it's on for collisions
    }

    void HandleFinishVFX()
    {
        if(chargeTime > minChargeTime) { StartCoroutine(FadeInFadeOutSprite()); } // only call if hit a minimum threshold
        else { ResetColorAndPosition(); }
        
        //playAnimation = false; 
        //animationPlayed = false;
        // Get the upper right and bottom left corners of the punch sprite in world coordinates
    }

    private IEnumerator FadeInFadeOutSprite()
    {
        PunchSprite.gameObject.SetActive(true);
        PunchSprite.color = originalColor;

        float timer = 0.0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            // Calculate the current alpha value based on the fade duration
            float alpha = Mathf.Lerp(1.0f, 0.0f, timer / fadeDuration);

            // Set the target color with the modified alpha value
            Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // Update the sprite's color
            PunchSprite.color = targetColor;

            yield return null;
        }

        ResetColorAndPosition();
    }

    void ResetColorAndPosition()
    {
        // Ensure the final color is set correctly
        PunchSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.0f);
        PunchSprite.gameObject.SetActive(false);

        // reset position
        PunchSprite.transform.localPosition = originalLocalPosition;
        PunchSprite.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    void ParticleSystemsOn(bool status)
    {
        if (status == true) { foreach (ParticleSystem ps in particleSystems) { ps.Play(); } particleTrigger = true; }
        else { foreach (ParticleSystem ps in particleSystems) { ps.Stop(); } particleTrigger = false; }
    }

    void GlowOn(bool status)
    {
        if (status == true) { glowTrigger = true; }
        else { glowTrigger = false; spriteGlow.GlowBrightness = 0; spriteGlow.OutlineWidth = 0; }
    }

    //void ReleasePunchAnimation() // ensure animation is played before releasing punch
    //{
    //    if (playAnimation && !animationPlayed) // checks to see if animation has started playing, but hasn't finished
    //    {
    //        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)) // checks if animation finished
    //        {
    //            // Set the punch sprite position to the animation's final position
    //            punchSprite.transform.localPosition = animationPosition;

    //            animationPlayed = true;

    //            ReleasePunch();
    //        }
    //    }
    //}

    void ReleasePunch()
    {
        playerPrimaryWeapon.damageToPass = playerPrimaryWeapon.minDamage + damageToPass;
        IsCharging = false;
        UpdateSpriteBounds();
        StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection));
        HandleFinishSound();
        HandleFinishVFX();
        ParticleSystemsOn(false); GlowOn(false); chargeTime = 0;
        //Instantiate(punchEffect, transform.position, Quaternion.identity);
        EventSystem.current.FinishChargedAttackTrigger();
    }
}
