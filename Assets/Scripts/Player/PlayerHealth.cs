using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    // External References
    private PlayerAnimator animator;
    public PlayerController playerController;
    private ChargePunch chargePunch;


    [SerializeField] private UIHealthChangeDisplay damageDisplay;
    public float lucidityDamageModifier;

    private void Start() 
    { 
        EventSystem.current.onPlayerHitCalcTrigger += Hit;
        playerController = GetComponent<PlayerController>(); 
        shield = GetComponentInChildren<Shield>();
        chargePunch = GetComponentInChildren<ChargePunch>();
        damageDisplay = GetComponentInChildren<UIHealthChangeDisplay>();
        animator = ComponentFinder.GetComponentInChildrenByNameAndType<PlayerAnimator>("Animator", this.gameObject, true);
        lucidityDamageModifier = 1;
    }

    public void Hit(Vector3 enemyPos, int damageNumber, int damageType, float damageMod, float knockbackMod, bool hitInActiveShieldZone)
    {
        if (hitInActiveShieldZone && !playerController.IsInvincible)
        {
            TakeDamage(damageNumber, damageMod);
            playerController.HandleHitPhysics(enemyPos, knockbackMod);
        }
        else if (!playerController.IsInvincible)
        {
            TakeDamage(damageNumber, damageMod);
            playerController.HandleHitPhysics(enemyPos, knockbackMod);
            StartCoroutine(playerController.HitStun());
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

    private void OnDestroy()
    {
        EventSystem.current.onPlayerHitCalcTrigger -= Hit;
    }
}
