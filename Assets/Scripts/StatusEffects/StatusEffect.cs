using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class StatusEffect : MonoBehaviour
{
    // outside references
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    private ParticleSystem particles;
    private AudioManager audioManager;

    // default values
    public float totalTimePassed = 0.0f;
    public float intervalToApply = 1.0f;
    public float counterToApplyAffect = 0;
    public bool applyStatusEffect;

    // should be filled in the child Status Effect
    public float effectDuration;
    public int damageToPass;
    public bool affectsMovement;
    public string nameOfSFXToPlay;
    public bool loopSFX;


    public virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();

        TryGetComponent(out particles);
        spriteRenderer.enabled = false;
    }

    public virtual void Execute() { applyStatusEffect = true; }

    public virtual void FixedUpdate()
    {
        if (applyStatusEffect)
        {
            // player feedback reg. status effects
            SFXHandler(true);
            VFXHandler(true);

            // pass status effects
            MovementHandler(true);
            DamageHandler();

            // update conditions
            totalTimePassed += Time.deltaTime;
            if (totalTimePassed >= effectDuration) { Reset(); }
        }
    }

    // Status Impacts

    public virtual void MovementHandler(bool state)
    {
        if (affectsMovement)
        {
            if (GetComponentInParent<EnemyController>() != null) { GetComponentInParent<EnemyController>().isStunned = state; }
            //else if (gameObject.layer == LayerMask.NameToLayer("Player")) { // affect movement }
        }
    }

    public virtual void DamageHandler()
    {
        if (totalTimePassed - counterToApplyAffect >= intervalToApply)
        {
            TakeDamage(damageToPass);
            counterToApplyAffect += intervalToApply;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (GetComponentInParent<EnemyController>() != null) { GetComponentInParent<EnemyController>().TakeDamage(damage); }
        //else if (gameObject.layer == LayerMask.NameToLayer("Player")) { gameObject.GetComponent<PlayerHealth>().TakeDamage(damage); }
    }

    public virtual void SFXHandler(bool state)
    {
        if(nameOfSFXToPlay != "")
        {
            if (state == true)
            {
                if (loopSFX) { audioManager.LoopSFX(nameOfSFXToPlay, loopSFX); }
                else { audioManager.PlaySFX(nameOfSFXToPlay); }
            }
            else { if (loopSFX) { audioManager.LoopSFX(nameOfSFXToPlay, !loopSFX); } }
        }
    }

    public virtual void VFXHandler(bool state)
    {
        if (state != true) { if (particles != null) { particles.Stop(); } }
        else { if (particles != null) { particles.Play(); } }
        spriteRenderer.enabled = state;
    }

    public virtual void Reset()
    {
        applyStatusEffect = false;
        VFXHandler(false);
        SFXHandler(false);
        MovementHandler(false);

        totalTimePassed = 0;
        counterToApplyAffect = 0;
    }
}
