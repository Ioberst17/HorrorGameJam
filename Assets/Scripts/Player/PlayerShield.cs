using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : Shield
{
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private int parryDamage = 10;
    [SerializeField] private int shieldCost = 20;
    [SerializeField] float staminaRate = 50f;
    private float spChecker;
    private bool shieldButtonDown;

    new void Start()
    {
        base.Start();
        playerController = GetComponentInParent<PlayerController>();
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (playerController.SP > shieldCost)
            {
                playerController.SP -= shieldCost;
                ShieldStatus("On"); shieldButtonDown = true;
                if (parry != null) { parry.Execute(); }
            }
            else { FindObjectOfType<AudioManager>().PlaySFX("InsufficientStamina"); }
        }
        if (playerController.SP < 0)
        {
            ShieldStatus("Off");
            shieldButtonDown = false;
            FindObjectOfType<AudioManager>().PlaySFX("InsufficientStamina");
        }
        if (Input.GetKey(KeyCode.F) && shieldButtonDown == true)
        {
            if (playerController.SP > 0) { ChangeSP(-staminaRate, Time.deltaTime); }
        }
        else { ChangeSP(staminaRate / 2, Time.deltaTime); }
    }

    public override void SpecificDamageChecks(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyController>() != null && collision.gameObject.GetComponent<EnemyController>().isAttacking) { checkStatus = true; }
        else if(collision.gameObject.GetComponent<EnemyProjectile>()) { checkStatus = true; }
        else if (collision.gameObject.GetComponent<Explode>() != null) { checkStatus = true; }
    }
    public override void CheckObjectType()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Player")) { shieldedObject = "Player"; }
        else { Debug.Log("Shield.cs is attached to an object that does not need it"); }

        AddCollisionsToCheckFor(shieldedObject);
    }

    public override void AddCollisionsToCheckFor(string shieldedObject)
    {
        if (shieldedObject == "Player") { layer1ToCheck = LayerMask.NameToLayer("Enemy"); layer2ToCheck = LayerMask.NameToLayer("Player Ammo"); }
        base.AddCollisionsToCheckFor(shieldedObject);
    }

    public override void PassThroughDamage(Collider2D collision, float damageMod, float knockbackMod) 
    {
        if (collision.gameObject.GetComponent<EnemyController>() != null)
        {
            EventSystem.current.PlayerHitCalcTrigger(
                              collision.gameObject.transform.position,
                              collision.gameObject.GetComponent<EnemyController>().damageValue,
                              1,
                              damageMod,
                              knockbackMod);
        }
        else if (collision.gameObject.GetComponent<EnemyProjectile>() != null)
        {
            EventSystem.current.PlayerHitCalcTrigger(
                              collision.gameObject.transform.position,
                              collision.gameObject.GetComponent<EnemyProjectile>().damageValue,
                              1,
                              damageMod,
                              knockbackMod);
        }
        else if (collision.gameObject.GetComponent<Explode>() != null)
        {
            EventSystem.current.PlayerHitCalcTrigger(
                              collision.gameObject.transform.position,
                              10,
                              1,
                              damageMod,
                              knockbackMod);
        }
    }

    public override void ReturnDamage(Collider2D collision)
    {
        FindObjectOfType<AudioManager>().PlaySFX("Parry");
        Instantiate(Resources.Load("VFXPrefabs/BulletImpact"), collision.transform.position, Quaternion.identity); // TO-DO: Swap out with a more appropriate impact
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        { 
            collision.gameObject.GetComponent<IDamageable>().Hit(collision.gameObject.GetComponent<EnemyController>().damageValue, transform.position); 
        }
    }

    public void ChangeSP(float rate, float deltaTime) 
    {
        var spChecker = playerController.SP + rate * deltaTime;
        if (spChecker > playerController.SP_MAX) { playerController.SP = playerController.SP_MAX; }
        else { playerController.SP = spChecker; }
    }

}
