using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SiblingComponentUtils;


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

    [SerializeField] private float chargeTime;             // the current charge time
    [SerializeField] public bool isCharging;                // whether the punch is currently being charged


    // VFX Related
    private GameObject visualEffects;
    ParticleSystem partSystem1, partSystem2, partSystem3, partSystem4;
    [SerializeField] bool particleTrigger;

    //SFX Related
    private bool falconSFXFlag;
    private bool falconSFXPlaying;
    private float punch1Length;
    private float sfxPlayTime;
    private float additionalSFXWaitTime;

    private void Start()
    {
        playerPrimaryWeapon = GetComponent<PlayerPrimaryWeapon>();
        LoadParticleSystems();
        Sound punch1Sound = FindObjectOfType<AudioManager>().GetSFX("ChargePunch1");
        punch1Length = punch1Sound.clip.length;
        ParticleSystemsOn(false);
    }

    void LoadParticleSystems()
    {
        visualEffects = transform.GetSibling("VisualEffects").gameObject;
        partSystem1 = visualEffects.transform.Find("PunchWindParticlesTop1").GetComponent<ParticleSystem>();
        partSystem2 = visualEffects.transform.Find("PunchWindParticlesTop2").GetComponent<ParticleSystem>();
        partSystem3 = visualEffects.transform.Find("PunchWindParticlesFloor1").GetComponent<ParticleSystem>();
        partSystem4 = visualEffects.transform.Find("PunchWindParticlesFloor2").GetComponent<ParticleSystem>();
    }

    public void Execute() { isCharging = true; }

    public void Release(int attackDirection) { this.attackDirection = attackDirection; isCharging = false; ReleasePunch(); }

    void Update()
    {
        if (isCharging)
        {
            chargeTime = chargeTime + (Time.deltaTime * chargeSpeed);
            CalcForce();
            HandleChargeVFX();
            HandleChargeSound();
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

    void FinishSound() { FindObjectOfType<AudioManager>().PlaySFX("ChargePunch2"); falconSFXFlag = false; sfxPlayTime = 0; falconSFXPlaying = false; }

    private void HandleChargeVFX() 
    { 
        if (chargeTime > 0.2f) 
        {
            EventSystem.current.StartChargedAttackTrigger(holdTimeNormalized, gameObject.transform, null);
            if (particleTrigger == false) { ParticleSystemsOn(true); }
        } 
    }

    void ParticleSystemsOn(bool status)
    {
        if (status == true) { partSystem1.Play(); partSystem2.Play(); partSystem3.Play(); partSystem4.Play(); particleTrigger = true; }
        else { partSystem1.Stop(); partSystem2.Stop(); partSystem3.Stop(); partSystem4.Stop(); particleTrigger = false; }
    }

    void ReleasePunch()
    {
        playerPrimaryWeapon.damageToPass = playerPrimaryWeapon.minDamage + damageToPass;
        StartCoroutine(playerPrimaryWeapon.AttackActiveFrames(attackDirection));
        isCharging = false;
        chargeTime = 0;
        HandleFinishSound();
        ParticleSystemsOn(false);
        //Instantiate(punchEffect, transform.position, Quaternion.identity);
        EventSystem.current.FinishChargedAttackTrigger();
    }
}
