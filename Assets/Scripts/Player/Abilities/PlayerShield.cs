using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : Shield
{
    // Outside references
    Stamina stamina; // limits shield usage
    PlayerController playerController;
    PlayerAnimator animator; // shield animation

    // internal variables and trackers
    int parryDamage = 10;
    [SerializeField] private int shieldCost = 20;
    [SerializeField] float staminaRate = 50f;
    bool shieldButtonDown;

    // vfx
    int impactVFXCoolDown;

    override protected void Start()
    {
        base.Start();

        stamina = SiblingComponentUtils.GetSiblingComponent<Stamina>(gameObject);
        playerController = GetComponentInParent<PlayerController>();
        animator = FindObjectOfType<PlayerAnimator>();
        impactVFXCoolDown = 0;
    }

    override protected void Update()
    {
        base.Update();

        if (stamina.SP < 0)
        {
            ShieldOn(false);
            shieldButtonDown = false;
            FindObjectOfType<AudioManager>().PlaySFX("InsufficientStamina");
        }
    }

    new private void FixedUpdate()
    {
        if (!shieldButtonDown) { ChangeSP(staminaRate / 2, Time.deltaTime); }
        if(impactVFXCoolDown > 0) { impactVFXCoolDown--; }
    }

    public void ShieldButtonDown()
    {
        if (stamina.SP > shieldCost)
        {
            stamina.SP -= shieldCost;
            ShieldOn(true); shieldButtonDown = true;
            if (parry != null) { parry.Execute(); }
        }
        else { FindObjectOfType<AudioManager>().PlaySFX("InsufficientStamina"); }
    }

    public void ShieldButtonHeld()
    {
        if(shieldButtonDown == true) 
        { 
            if (stamina.SP > 0) { ChangeSP(-staminaRate, Time.deltaTime); }
            if (!playerController.IsCrouching){ animator.Play("PlayerShield"); }
        }
    }

    public void ShieldButtonUp() { ShieldOn(false); shieldButtonDown = false; }


    protected override void SpecificDamageChecks(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<EnemyProjectile>()) { checkStatus = true; }
        else if (collision.gameObject.GetComponent<Explode>() != null) { checkStatus = true; }
    }

    protected override void PassThroughDamage(Collider2D collision, float damageMod, float knockbackMod) 
    {
        if (collision.gameObject.GetComponentInParent<AttackManager>())
        {
            lastReceivedAttack = collision.gameObject.GetComponentInParent<AttackManager>().MostRecentAttack;
            damageToPass = lastReceivedAttack.baseDamage;

            Debug.Log("Player was hit by enemy " + collision.gameObject.name);
            EventSystem.current.PlayerHitHealthTrigger(
                collision.gameObject.transform.position,
                damageToPass,
                1,
                damageMod,
                knockbackMod,
                hitWithinActiveShieldZone);
        }
        else if (collision.gameObject.GetComponent<EnemyProjectile>() != null)
        {
            EventSystem.current.PlayerHitHealthTrigger(
                              collision.gameObject.transform.position,
                              collision.gameObject.GetComponent<ProjectileBase>().projectile.baseDamage,
                              1,
                              damageMod,
                              knockbackMod,
                              hitWithinActiveShieldZone);
        }
        else if (collision.gameObject.GetComponent<Explode>() != null)
        {
            EventSystem.current.PlayerHitHealthTrigger(
                              collision.gameObject.transform.position,
                              10,
                              1,
                              damageMod,
                              knockbackMod,
                              hitWithinActiveShieldZone);
        }
    }

    protected override void ReturnDamage(Collider2D collision)
    {
        FindObjectOfType<AudioManager>().PlaySFX("Parry");
        if(impactVFXCoolDown == 0)
        {
            Instantiate(Resources.Load("VFXPrefabs/BulletImpact"), collision.transform.position, Quaternion.identity); // TO-DO: Swap out with a more appropriate impact
            impactVFXCoolDown = 30;
        }
        
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        { 
            Debug.Log("Parried!");
            EventSystem.current.EnemyParryHitTrigger(collision.gameObject.GetComponentInParent<AttackManager>().MostRecentAttack.baseDamage, 
                                                        transform.position, 
                                                        null, 
                                                        collision.gameObject.GetComponent<EnemyController>());
        }
    }

    public void ChangeSP(float rate, float deltaTime) 
    {
        var spChecker = stamina.SP + rate * deltaTime;
        if (spChecker > stamina.maxSP) { stamina.SP = stamina.maxSP; }
        else { stamina.SP = spChecker; }
    }

}
