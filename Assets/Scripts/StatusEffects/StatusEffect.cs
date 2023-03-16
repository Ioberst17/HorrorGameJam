using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class StatusEffect : MonoBehaviour
{
    // default values
    public float totalTimePassed = 0.0f;
    public float intervalToApply = 1.0f;
    public float counterToApplyAffect = 0;
    public bool applyStatusEffect;

    // should be filled in the child Status Effect
    public float effectDuration;
    public int damageToPass;
    
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    private ParticleSystem particles;

    public virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        TryGetComponent(out particles);
        spriteRenderer.enabled = false;
    }

    public virtual void Execute() { applyStatusEffect = true; }

    public virtual void Update()
    {
        if (applyStatusEffect)
        {
            spriteRenderer.enabled = true;
            if (particles != null) { particles.Play(); }
            totalTimePassed += Time.deltaTime;

            AffectMovement();
            if (totalTimePassed - counterToApplyAffect >= intervalToApply)
            {
                TakeDamage(damageToPass);
                counterToApplyAffect += intervalToApply;
            }
            if (totalTimePassed >= effectDuration) { Reset(); }
        }
    }

    // Status Impacts

    public virtual void AffectMovement()
    {
        if (gameObject.GetComponentInParent<EnemyController>() != null) { /*gameObject.GetComponentInParent<EnemyController>().AffectMovement();*/ }
        //else if (gameObject.layer == LayerMask.NameToLayer("Player")) { // affect movement }
    }

    public virtual void TakeDamage(int damage)
    {
        if (gameObject.GetComponentInParent<EnemyController>() != null) { gameObject.GetComponentInParent<EnemyController>().TakeDamage(damage); }
        //else if (gameObject.layer == LayerMask.NameToLayer("Player")) { gameObject.GetComponent<PlayerHealth>().TakeDamage(damage); }
    }

    public virtual void Reset()
    {
        applyStatusEffect = false;
        totalTimePassed = 0;
        counterToApplyAffect = 0;
        spriteRenderer.enabled = false;
        if(particles != null) { particles.Stop(); }
    }
}
