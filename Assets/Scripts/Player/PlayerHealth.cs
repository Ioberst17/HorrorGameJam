using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    // External References
    public PlayerController playerController;

    [SerializeField] private UIHealthChangeDisplay damageDisplay;
    public float lucidityDamageModifier;

    private void Start() 
    { 
        playerController = GetComponent<PlayerController>(); 
        shield = GetComponentInChildren<Shield>();
        damageDisplay = GetComponentInChildren<UIHealthChangeDisplay>();
        lucidityDamageModifier = 1;
        EventSystem.current.onPlayerHitCalcTrigger += Hit;
    }

    private void OnDestroy()
    {
        EventSystem.current.onPlayerHitCalcTrigger -= Hit;
    }

    public void Hit(Vector3 enemyPos, int damageNumber, string statusEffect, float damageMod, float knockbackMod, bool hitInActiveShieldZone)
    {
        if (hitInActiveShieldZone || !playerController.IsInvincible)
        {
            TakeDamage(damageNumber, damageMod);
            if (statusEffect != null) { playerController.StatusModifier(statusEffect); }
            EventSystem.current.PlayerHitPostHealthTrigger(enemyPos, knockbackMod, hitInActiveShieldZone);
        }
    }

    public void TakeDamage(int damageNumber, float damageMod) 
    {
        damageTaken = (int)(damageNumber * (1 - damageMod) * lucidityDamageModifier);
        HP -= damageTaken;
        if (HP <= 0) { HPZero(); }
    }

    public void TakeDamage(int damageNumber) { TakeDamage(damageNumber, 0); }

    public new void HPZero() { Debug.Log("Player Death"); HP = MaxHealth; EventSystem.current.PlayerDeathTrigger(); }
}
