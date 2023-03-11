using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Burnable : StatusEffect
{
    // These variables control the damage and duration of the burn.
    private StatusEffectData statusData = new StatusEffectData();
    public int burnDamage; // Damage per second.
    public float burnDuration; // In seconds.
    private float burnCounter;
    public bool isBurning = false;

    public override void Start()
    {
        base.Start();
        burnDamage = statusData.burnDamage;
        burnDuration = statusData.burnDuration;
        burnCounter = 0;
    }

    public void Execute() { isBurning = true; }

    void Update()
    {
        if (isBurning)
        {
            spriteRenderer.enabled = true;
            totalTimePassed += Time.deltaTime;
            

            if (totalTimePassed - burnCounter >= intervalToApply)
            {
                Debug.Log("Passed damage to enemy");
                TakeDamage(burnDamage);
                burnCounter += intervalToApply;
            }
            if(totalTimePassed >= burnDuration)
            {
                Debug.Log("Burning stopped");
                isBurning = false;
                totalTimePassed = 0;
                burnCounter = 0;
                animator.SetTrigger("Stop");
                spriteRenderer.enabled = false;
            }
        }
            
    }

    // This function applies damage to the object with the burnable status.
    private void TakeDamage(float damage)
    {
        Debug.Log("Attempting to do damage to enemy");
        if (gameObject.GetComponentInParent<EnemyController>() != null) { gameObject.GetComponentInParent<EnemyController>().TakeDamage(burnDamage); }
        else if (gameObject.layer == LayerMask.NameToLayer("Player")) { gameObject.GetComponent<PlayerHealth>().TakeDamage(burnDamage); }
    }

}
