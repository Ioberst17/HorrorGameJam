using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using System.Linq;
using static ComponentFinder;

[RequireComponent(typeof(PlayerPrimaryWeapon))]
public class ChargePunch : MonoBehaviour
{
    // external references
    GameController gameController;
    CameraBehavior cameraBehavior;
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
    [SerializeField] List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    ParticleSystem partSystem1, partSystem2, partSystem3, partSystem4, partSystem5;
    bool particleTrigger;

    // delayed one time trigger particles
    ParticleSystem initialBuildParticleBlast;
    string initialBuildParticleBlastObjectName = "ChargePunchInitialBlast";
    ParticleSystem chargePunchElectricityInitial;
    string chargePunchElectricityInitialObjectName = "ChargePunchInitialElectricity";
    ParticleSystem chargePunchElectricityInitial2;
    string chargePunchElectricityInitial2ObjectName = "ChargePunchInitialElectricity2";
    ParticleSystem chargePunchElectricityFinal;
    string chargePunchElectricityFinalObjectName = "ChargePunchFinalElectricity";   
    ParticleSystem chargePunchFireSpiral;
    string chargePunchFireSpiralObjectName = "ChargePunchFireSpiral";


    [SerializeField] float chargePunchElectricityStagger; // used to match electricity start and SFX loop

    // player glow related
    [SerializeField] private SpriteGlowEffect spriteGlow; private bool glowTrigger;
    float glowFrequency = 5, glowAmplitude = 4;
    float outlineFrequency = 2, outlineAmplitude = 2;
    [SerializeField] int glowVal;
    [SerializeField] int outlineVal;
    int glowMidPoint = 6, outlineMidpoint = 5;
    
    // charge punch sprite related
    public SpriteRenderer PunchSprite { get; set; }
    Bounds punchSpriteBounds;
    public float maxScale = 2.0f; // SETS MAX SCALE OF SPRITE IN CHARGING, IMPORTANT FOR COLLISIONS
    public float offsetAmount = 1.0f;
    private float visibleDuration = 0.2f;
    private Color originalColor;
    private Vector2 originalLocalPosition;
    private float fadeDuration = 0.5f;

    //SFX Related
    private bool falconSFXFlag;
    private bool falconSFXPlaying;
    private float punch1Length;
    private float sfxPlayTime;
    private float additionalSFXWaitTime;

    private void Start()
    {
        playerPrimaryWeapon = GetComponent<PlayerPrimaryWeapon>();
        gameController = FindObjectOfType<GameController>();
        LoadVFXReferences();
        audioManager = FindObjectOfType<AudioManager>();
        Sound punch1Sound = audioManager.GetSFX("ChargePunch1");
        punch1Length = punch1Sound.clip.length;
        ParticleSystemsOn(false);
    }

    void LoadVFXReferences()
    {
        // PARTICLES
        visualEffects = transform.GetSibling("VisualEffects").gameObject;

        // load in particles from PunchWindParticles
        particleParent = visualEffects.transform.Find("PunchWindParticles").gameObject;
        ParticleSystem[] childParticleSystems = particleParent.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in childParticleSystems) { particleSystems.Add(ps); }

        // add charge punch electricity + initial build particles
        chargePunchElectricityInitial = GetComponentInChildrenByNameAndType<ParticleSystem>(chargePunchElectricityInitialObjectName, visualEffects, true);
        chargePunchElectricityInitial2 = GetComponentInChildrenByNameAndType<ParticleSystem>(chargePunchElectricityInitial2ObjectName, visualEffects, true);
        chargePunchElectricityFinal = GetComponentInChildrenByNameAndType<ParticleSystem>(chargePunchElectricityFinalObjectName, visualEffects, true);
        initialBuildParticleBlast = GetComponentInChildrenByNameAndType<ParticleSystem>(initialBuildParticleBlastObjectName, visualEffects, true);
        chargePunchFireSpiral = GetComponentInChildrenByNameAndType<ParticleSystem>(chargePunchFireSpiralObjectName, visualEffects, true);
        
        particleSystems.Add(chargePunchElectricityInitial);
        particleSystems.Add(chargePunchElectricityInitial2);
        particleSystems.Add(chargePunchElectricityFinal);
        particleSystems.Add(initialBuildParticleBlast);
        particleSystems.Add(chargePunchFireSpiral);

        // screen shake related to build particles blast
        cameraBehavior = FindObjectOfType<CameraBehavior>();

        // GLOW
        var spriteGlowParent = GetComponentInChildrenByNameAndType<Transform>("Base", transform.parent.gameObject);
        spriteGlow = GetComponentInChildrenByNameAndType<SpriteGlowEffect>("SpriteAndAnimations", spriteGlowParent.gameObject);

        // SPRITE-RELATED
        PunchSprite = GetComponentInChildrenByNameAndType<SpriteRenderer>("ChargePunchSprite", gameObject, true);
        PunchSprite.gameObject.SetActive(true);
        originalColor = PunchSprite.color;
        originalLocalPosition = PunchSprite.transform.localPosition;
        PunchSprite.gameObject.SetActive(false);
    }

    public void Execute() 
    { 
        IsCharging = true;
        // sprite renderer
        PunchSprite.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        PunchSprite.transform.localPosition += new Vector3(offsetAmount, 0.0f, 0.0f);
    }

    public void Release(int attackDirection) { this.attackDirection = attackDirection; IsCharging = false; ReleasePunch();  } 

    void FixedUpdate()
    {
        if (!IsCharging) { chargePunchElectricityInitial.Stop(); chargePunchElectricityFinal.Stop(); chargePunchFireSpiral.Stop(); } // run to ensure a stop

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
        if (status == true)
        {
            foreach (ParticleSystem ps in particleSystems)
            {
                // first wave of effects after start
                if (ps.name == chargePunchElectricityInitialObjectName || 
                    ps.name == chargePunchElectricityInitial2ObjectName || 
                    ps.name == initialBuildParticleBlastObjectName) 
                {
                    // specific to chargePunchElectricity and backing SFX track, stagger the start of electricity to match SFX backing track
                    // also used by initial build particle blast
                    Invoke("TriggerOneTimeBuildFX", minChargeTime + chargePunchElectricityStagger);
                }
                // second wave of effects
                else if(ps.name == chargePunchElectricityFinalObjectName || ps.name == chargePunchFireSpiralObjectName)
                {
                    Invoke("TriggerFinalBuildFX", maxChargeTime);
                }
                // else it should play at start
                else { ps.Play();  }
                particleTrigger = true;
            }
        }
        else { foreach (ParticleSystem ps in particleSystems) { ps.Stop(); } particleTrigger = false; }
    }

    void TriggerOneTimeBuildFX() 
    {
        if (IsCharging) // make sure still charging before loading in, otherwise player may execute punch and these will fire after the fact
        {
            particleSystems.FirstOrDefault(ps => ps == chargePunchElectricityInitial).Play();
            particleSystems.FirstOrDefault(ps => ps == chargePunchElectricityInitial2).Play();
            particleSystems.FirstOrDefault(ps => ps == initialBuildParticleBlast).Play();

            cameraBehavior.ShakeScreen(0.5f);
            StartCoroutine(gameController.PlayHaptics());
        }
    }

    void TriggerFinalBuildFX()
    {
        if (IsCharging) 
        { 
            particleSystems.FirstOrDefault(ps => ps == chargePunchElectricityFinal).Play(); 
            particleSystems.FirstOrDefault(ps => ps == chargePunchFireSpiral).Play(); 
        }
    }

    void GlowOn(bool status)
    {
        if (status == true) { glowTrigger = true; }
        else { glowTrigger = false; spriteGlow.GlowBrightness = 0; spriteGlow.OutlineWidth = 0; }
    }

    void ReleasePunch()
    {
        playerPrimaryWeapon.damageToPass = playerPrimaryWeapon.minDamage + damageToPass;
        IsCharging = false;
        UpdateSpriteBounds();
        StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection, "PlayerBasicAttack"));
        HandleFinishSound();
        HandleFinishVFX();
        ParticleSystemsOn(false); GlowOn(false); chargeTime = 0; holdTimeNormalized = 0;
        //Instantiate(punchEffect, transform.position, Quaternion.identity);
        EventSystem.current.FinishChargedAttackTrigger();
    }
}
