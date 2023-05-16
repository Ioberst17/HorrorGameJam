using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SiblingComponentUtils;
using SpriteGlow;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerPrimaryWeapon))]
public class ChargePunch : MonoBehaviour
{
    PlayerPrimaryWeapon playerPrimaryWeapon;
    public float maxChargeTime = 2f;      // the maximum time the punch can be charged for
    public float chargeSpeed = 1f;        // the speed at which the punch charge increases
    [SerializeField] public int damageToPass;
    [SerializeField] private float maxDamageToAdd = 10f;
    [SerializeField] private float holdTimeNormalized;
    [SerializeField] int attackDirection;

    [SerializeField] private float chargeTime; // the current charge time
    public bool IsCharging { get; set; } // whether the punch is currently being charged


    [Header("VFX Related")]
    private GameObject visualEffects;
    private GameObject particleParent;
    ParticleSystem partSystem1, partSystem2, partSystem3, partSystem4, partSystem5;
    ParticleSystem[] particleSystems;
    bool particleTrigger;
    [SerializeField] private SpriteGlowEffect spriteGlow; private bool glowTrigger;
    float glowFrequency = 5, glowAmplitude = 5;
    float outlineFrequency = 2, outlineAmplitude = 2;
    [SerializeField] int glowVal;
    [SerializeField] int outlineVal;
    int glowMidPoint = 5, outlineMidpoint = 5;

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
        Sound punch1Sound = FindObjectOfType<AudioManager>().GetSFX("ChargePunch1");
        punch1Length = punch1Sound.clip.length;
        ParticleSystemsOn(false);
    }

    void LoadVFXReferences()
    {
        visualEffects = transform.GetSibling("VisualEffects").gameObject;
        particleParent = visualEffects.transform.Find("PunchWindParticles").gameObject;
        particleSystems = particleParent.GetComponentsInChildren<ParticleSystem>();
        spriteGlow = visualEffects.GetComponentInParent<SpriteGlowEffect>();
    }

    public void Execute() { IsCharging = true; }

    public void Release(int attackDirection) { this.attackDirection = attackDirection; IsCharging = false; ReleasePunch(); }

    void FixedUpdate()
    {
        if (IsCharging)
        {
            chargeTime = chargeTime + (Time.deltaTime * chargeSpeed);
            CalcForce();
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

    private void HandleChargeSound()
    {
        if (chargeTime > 0.35f)
        {
            if (falconSFXFlag == false)
            {
                FindObjectOfType<AudioManager>().PlaySFX("ChargePunch1");
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

    void FinishSound() { FindObjectOfType<AudioManager>().PlaySFX("ChargePunch2"); falconSFXFlag = false; sfxPlayTime = 0; falconSFXPlaying = false; }

    private void HandleChargeVFX() 
    {
        if (chargeTime > 0.2f) 
        {
            EventSystem.current.StartChargedAttackTrigger(holdTimeNormalized, gameObject.transform, null);
            if (particleTrigger == false) { ParticleSystemsOn(true); }
            if (glowTrigger == false) { GlowOn(true); spriteGlow.GlowBrightness = 0; spriteGlow.OutlineWidth = 0; }
        } 
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


    void ReleasePunch()
    {
        playerPrimaryWeapon.damageToPass = playerPrimaryWeapon.minDamage + damageToPass;
        IsCharging = false;
        chargeTime = 0;
        HandleFinishSound();
        ParticleSystemsOn(false); GlowOn(false);
        StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection));
        //Instantiate(punchEffect, transform.position, Quaternion.identity);
        EventSystem.current.FinishChargedAttackTrigger();
    }
}
