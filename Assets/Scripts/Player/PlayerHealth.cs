using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public int StartingHP;
    public bool isInvincible;
    public bool inHitstun;
    public Animator animator;
    public PlayerController playerController;
    private Shield shield;
    [SerializeField] private UIHealthChangeDisplay damageDisplay;
    public float lucidityDamageModifier;
    public int damageTaken;

    private void Start() 
    { 
        EventSystem.current.onPlayerHitCalcTrigger += Hit;
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>(); 
        shield = GetComponentInChildren<Shield>();
        damageDisplay = GetComponentInChildren<UIHealthChangeDisplay>();
        isInvincible = false;
        inHitstun = false;
        lucidityDamageModifier = 1;
    }

    public void Hit(Vector3 enemyPos, int damageNumber, int damageType, float damageMod, float knockbackMod)
    {
        playerController.Hit(enemyPos, knockbackMod);
        if (!isInvincible && !shield.shieldOn) { StartCoroutine(hitStun()); }
        TakeDamage(damageNumber, damageMod);
    }

    public void TakeDamage(int damageNumber, float damageMod) 
    {
        damageTaken = (int)(damageNumber * (1 - damageMod) * lucidityDamageModifier);
        HP -= damageTaken;
        if (HP <= 0) { HPZero(); }
    }

    public void TakeDamage(int damageNumber) { TakeDamage(damageNumber, 0); }

    public new void HPZero() { Debug.Log("Player Death"); HP = StartingHP; EventSystem.current.PlayerDeathTrigger(); }

    IEnumerator hitStun()
    {
        inHitstun = true;
        StartCoroutine(Invincibility());
        animator.Play("PlayerHurt");
        FindObjectOfType<AudioManager>().PlaySFX("PlayerHit");
        yield return new WaitForSeconds(1); // waits a certain number of seconds
        inHitstun = false;
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        //animator.Play("PlayerHit");
        yield return new WaitForSeconds(1.5f); // waits a certain number of seconds
        isInvincible = false;
    }

    private void OnDestroy()
    {
        EventSystem.current.onPlayerHitCalcTrigger -= Hit;
    }
}
