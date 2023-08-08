using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : Shield
{
    // Outside references
    private PlayerStamina playerStamina;
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private PlayerAnimator animator;

    // internal variables and trackers
    private int parryDamage = 10;
    [SerializeField] private int shieldCost = 20;
    [SerializeField] float staminaRate = 50f;
    private float spChecker;
    private bool shieldButtonDown;
    private EnemyController currentAttacker;
    [SerializeField] private int enemyAttackNumber;
    private int enemyDamageVal;
    private int effectcooldown;

    new void Start()
    {
        base.Start();
        playerStamina = SiblingComponentUtils.GetSiblingComponent<PlayerStamina>(this.gameObject);
        playerController = GetComponentInParent<PlayerController>();
        playerHealth = GetComponentInParent<PlayerHealth>();
        animator = FindObjectOfType<PlayerAnimator>();
        effectcooldown = 0;
    }

    new void Update()
    {
        // handle invincibility
        if (playerHealth.IsInvincible) { Invincibility = true; }
        else { Invincibility = false; }

        base.Update();

        if (playerStamina.SP < 0)
        {
            ShieldStatus("Off");
            shieldButtonDown = false;
            FindObjectOfType<AudioManager>().PlaySFX("InsufficientStamina");
        }
    }

    private void FixedUpdate()
    {
        if (!shieldButtonDown) { ChangeSP(staminaRate / 2, Time.deltaTime); }
        if(effectcooldown > 0) { effectcooldown--; }
    }



    public void ShieldButtonDown()
    {
        if (playerStamina.SP > shieldCost)
        {
            playerStamina.SP -= shieldCost;
            ShieldStatus("On"); shieldButtonDown = true;
            if (parry != null) { parry.Execute(); }
        }
        else { FindObjectOfType<AudioManager>().PlaySFX("InsufficientStamina"); }
    }

    public void ShieldButtonHeld()
    {
        if(shieldButtonDown == true) 
        { 
            if (playerStamina.SP > 0) { ChangeSP(-staminaRate, Time.deltaTime); }
            if (!playerController.IsCrouching){ animator.Play("PlayerShield"); }
        }
    }

    public void ShieldButtonUp()
    {
        ShieldStatus("Off");
        shieldButtonDown = false;
    }


    public override void SpecificDamageChecks(Collider2D collision)
    {
        if(collision.gameObject.tag == "EnemyAttack") { checkStatus = true;}
        else if (collision.gameObject.GetComponent<EnemyController>() != null && collision.gameObject.GetComponent<EnemyController>().isAttacking) { checkStatus = true;  }
        else if(collision.gameObject.GetComponent<EnemyProjectile>()) { checkStatus = true; }
        else if (collision.gameObject.GetComponent<Explode>() != null) { checkStatus = true; }
    }
    public override void CheckObjectType()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Player")) { shieldedObject = "Player"; }
        else { Debug.Log("Shield.cs is attached to an object that does not need it"); }

        AddLayersToCheckOn(shieldedObject);
    }

    public override void AddLayersToCheckOn(string shieldedObject)
    {
        if (shieldedObject == "Player") { attackerFilter.SetLayerMask((1 << LayerMask.NameToLayer("Enemy") /*| 1 << LayerMask.NameToLayer("Player Ammo")*/)); }

        //if (shieldedObject == "Player") { layer1ToCheck = LayerMask.NameToLayer("EnemyAttack"); layer2ToCheck = LayerMask.NameToLayer("Player Ammo"); }

        base.AddLayersToCheckOn(shieldedObject);
    }

    EnemyController CheckForEnemyController(Collider2D collision)
    {
        currentAttacker = collision.GetComponentInParent<EnemyController>();
        if(currentAttacker != null) { return currentAttacker; }
        currentAttacker = collision.GetComponentInChildren<EnemyController>();
        if(currentAttacker != null) { return currentAttacker; }
        return null;
    }

    int GetEnemyDamageVal(Collider2D collision)
    {
        enemyAttackNumber = collision.gameObject.GetComponent<EnemyAttackNumber>().enemyAttackNumber;
        if(CheckForEnemyController(collision) != null)
        {
            if(enemyAttackNumber == 1) { return currentAttacker.GetComponent<EnemyController>().dmgVal1; }
            if(enemyAttackNumber == 2) { return currentAttacker.GetComponent<EnemyController>().dmgVal2; }
            if(enemyAttackNumber == 3) { return currentAttacker.GetComponent<EnemyController>().dmgVal3; }
            if(enemyAttackNumber == 4) { return currentAttacker.GetComponent<EnemyController>().dmgVal4; }
            if(enemyAttackNumber == 5) { return currentAttacker.GetComponent<EnemyController>().dmgVal5; }
            if(enemyAttackNumber == 6) { return currentAttacker.GetComponent<EnemyController>().dmgVal6; }

        }
        else { Debug.Log("Player Shield is detecting an Enemy Attack tag on game object: " + collision.gameObject.name + ", but it contains no Enemy Controller in its parent or children"); }
        return -1;
    }

    public override void PassThroughDamage(Collider2D collision, float damageMod, float knockbackMod) 
    {
        if (collision.gameObject.tag == "EnemyAttack")
        {
            enemyDamageVal = GetEnemyDamageVal(collision);
            if (enemyDamageVal != -1) 
            {
                EventSystem.current.PlayerHitCalcTrigger(
                  collision.gameObject.transform.position,
                  enemyDamageVal,
                  1,
                  damageMod,
                  knockbackMod,
                  hitWithinActiveShieldZone);
            }
            else { Debug.Log("Couldn't find a damage value in the enemy controller of the attacking object that matches the enemyAttackNumber listed on the attacking object"); }
        }
        else if (collision.gameObject.GetComponent<EnemyController>() != null)
        {
            EventSystem.current.PlayerHitCalcTrigger(
                              collision.gameObject.transform.position,
                              collision.gameObject.GetComponent<EnemyController>().dmgVal1,
                              1,
                              damageMod,
                              knockbackMod,
                              hitWithinActiveShieldZone);
        }
        else if (collision.gameObject.GetComponent<EnemyProjectile>() != null)
        {
            EventSystem.current.PlayerHitCalcTrigger(
                              collision.gameObject.transform.position,
                              collision.gameObject.GetComponent<EnemyProjectile>().damageValue,
                              1,
                              damageMod,
                              knockbackMod,
                              hitWithinActiveShieldZone);
        }
        else if (collision.gameObject.GetComponent<Explode>() != null)
        {
            EventSystem.current.PlayerHitCalcTrigger(
                              collision.gameObject.transform.position,
                              10,
                              1,
                              damageMod,
                              knockbackMod,
                              hitWithinActiveShieldZone);
        }
    }

    public override void ReturnDamage(Collider2D collision)
    {
        FindObjectOfType<AudioManager>().PlaySFX("Parry");
        if(effectcooldown == 0)
        {
            Instantiate(Resources.Load("VFXPrefabs/BulletImpact"), collision.transform.position, Quaternion.identity); // TO-DO: Swap out with a more appropriate impact
            effectcooldown = 30;
        }
        
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        { 
            collision.gameObject.GetComponent<IDamageable>().Hit(collision.gameObject.GetComponent<EnemyController>().dmgVal1, transform.position); 
        }
    }

    public void ChangeSP(float rate, float deltaTime) 
    {
        var spChecker = playerStamina.SP + rate * deltaTime;
        if (spChecker > playerStamina.maxSP) { playerStamina.SP = playerStamina.maxSP; }
        else { playerStamina.SP = spChecker; }
    }

}
