using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public int StartingHP;
    public bool isInvincible;
    public bool inHitstun;
    public Animator animator;
    private Shield shield;

    private void Start() 
    { 
        EventSystem.current.onPlayerHitTrigger += TakeDamage;
        animator = GetComponent<Animator>();
        shield = GetComponentInChildren<Shield>();
        isInvincible = false;
        inHitstun = false;
    }

    public void TakeDamage(Vector3 enemyPos, int damageNumber, int damageType, float damageMod, float knockbackMod)
    {
        if (!isInvincible && !shield.shieldOn) { StartCoroutine(hitStun()); }
        HP -= (int)(damageNumber * (1 - damageMod));
        if (HP <= 0) { HPZero(); }
    }

    public new void HPZero()
    {
        Debug.Log("Player Death"); HP = StartingHP; EventSystem.current.PlayerDeathTrigger();
    }

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
        EventSystem.current.onPlayerHitTrigger -= TakeDamage;
    }
}
