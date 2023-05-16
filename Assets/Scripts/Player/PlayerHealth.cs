using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public bool IsInvincible { get; set; }
    public bool inHitStun;
    private float blinkFrequency = 30f; // higher, isfaster blinking for hit stun
    private SpriteRenderer spriteRenderer;
    public Animator animator;
    public PlayerController playerController;
    private Shield shield;
    [SerializeField] private UIHealthChangeDisplay damageDisplay;
    public float lucidityDamageModifier;
    public int damageTaken;
    private float standardInvincibilityLength = 1.5f;

    private void Start() 
    { 
        EventSystem.current.onPlayerHitCalcTrigger += Hit;
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>(); 
        shield = GetComponentInChildren<Shield>();
        damageDisplay = GetComponentInChildren<UIHealthChangeDisplay>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        IsInvincible = false;
        inHitStun = false;
        lucidityDamageModifier = 1;
    }

    private void FixedUpdate()
    {
        if(inHitStun == true)
        {
            // hitStunBlink
            // calculate the blink time based on frequency
            float blinkTime = Mathf.Sin(Time.time * blinkFrequency);
            // set the sprite renderer to be visible if blink time is positive, otherwise invisible
            spriteRenderer.enabled = (blinkTime > 0f);
        }
        else { spriteRenderer.enabled = true; }
    }

    public void Hit(Vector3 enemyPos, int damageNumber, int damageType, float damageMod, float knockbackMod, bool hitInActiveShieldZone)
    {
        if (hitInActiveShieldZone && !IsInvincible)
        {
            TakeDamage(damageNumber, damageMod);
            playerController.Hit(enemyPos, knockbackMod);
        }
        else if (!IsInvincible)
        {
            TakeDamage(damageNumber, damageMod);
            playerController.Hit(enemyPos, knockbackMod);
            StartCoroutine(hitStun());
        }
    }

    public void TakeDamage(int damageNumber, float damageMod) 
    {
        damageTaken = (int)(damageNumber * (1 - damageMod) * lucidityDamageModifier);
        HP -= damageTaken;
        if (HP <= 0) { HPZero(); }
    }

    public void TakeDamage(int damageNumber) { TakeDamage(damageNumber, 0); }

    public new void HPZero() { Debug.Log("Player Death"); HP = maxHealth; EventSystem.current.PlayerDeathTrigger(); }

    IEnumerator hitStun()
    {
        inHitStun = true;
        StartCoroutine(Invincibility(standardInvincibilityLength));
        animator.Play("PlayerHurt");
        FindObjectOfType<AudioManager>().PlaySFX("PlayerHit");
        yield return new WaitForSeconds(1); // waits a certain number of seconds
        inHitStun = false;
    }

    public IEnumerator Invincibility(float length)
    {
        IsInvincible = true;
        //animator.Play("PlayerHit");
        yield return new WaitForSeconds(length); // waits a certain number of seconds
        IsInvincible = false;
    }

    private void OnDestroy()
    {
        EventSystem.current.onPlayerHitCalcTrigger -= Hit;
    }
}
