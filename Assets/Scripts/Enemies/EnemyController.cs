using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    private SpriteRenderer spriteRenderer;
    public Animator animator;
    public Rigidbody2D rb;
    private CapsuleCollider2D CC2D;
    private PlayerController playerController;
    public Transform playerLocation;
    private Vector2 newVelocity;
    
    private Vector3 startingLocation;

    //The points and values regarding the patrol code
    [SerializeField] public Transform patrol1;
    public Vector3 patrol1Point;
    [SerializeField] public Transform patrol2;
    public Vector3 patrol2Point;
    [SerializeField] public float patrolSpeed;
    public int patrolID;

    //the attack power of the enemy and other internal values
    public EnemyDatabase enemyDatabase; // used to load in values for enemies e.g. health data, attack info
    public int dmgVal1, dmgVal2, dmgVal3, dmgVal4, dmgVal5, dmgVal6;
    public int mostRecentAttack;

    private EnemyHealth enemyHealth;
    [SerializeField] float hpMultiplier;
    [SerializeField] float apMultiplier;
    public int HP_MAX;
    public int HP;
    private int damageToPass;
    private string statusToPass;
    public bool isStunned;
    [SerializeField]
    [Tooltip("The higher blinkFrequency is, the faster blinking is for hit stun")]
    private float blinkFrequency = 30f; // higher, isfaster blinking for hit stun

    public int invincibilityCount;
    [SerializeField]
    private int invincibilitySet;
    private bool isDead;
    public event Action<EnemyController> OnDeath;
    public bool isAttacking;
    public bool playerInZone;
    public WeaponDatabase weaponDatabase; // used for ammo damage calcs
    private Vector2 newForce;

    public int knockbackForce;

    private int SoulPointsDropped;

    private int EnemytypeID;
    public int facingDirection = 1;

    public bool damageInterupt = false;

    private HellhoundBehavior hellhoundBehavior;
    private ParalysisDemonBehavior paralysisDemonBehavior;
    private BatBehavior batBehavior;
    private BloodGolemBehavior GolemBehavior;
    private SpiderBehavior SpiderBehavior;
    private GargoyleBehavior GargoyleBehavior;

    void Awake()
    {
        playerInZone = false;
        isDead = false;
        isAttacking = false;
        rb = GetComponent<Rigidbody2D>();
        CC2D = GetComponent<CapsuleCollider2D>();
        enemyHealth = GetComponent<EnemyHealth>();
        playerController = GameObject.Find("PlayerModel").GetComponent<PlayerController>();
        playerLocation = playerController.transform;
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        enemyDatabase = GameObject.Find("EnemyDatabase").GetComponent<EnemyDatabase>();
        animator = GetComponent<Animator>();
        startingLocation = transform.position;
        patrol1Point = patrol1.position;
        patrol2Point = patrol2.position;
        invincibilityCount = 0;
        patrolID = 0;
       
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }


   // Start is called before the first frame update
    void Start()
    {
        setupHelper();
        //subscribe to important messages
        EventSystem.current.onEnemyEnviroDamage += Hit;
        EventSystem.current.onEnemyHitCollision += Hit;
    }

    // Update is called once per frame
    //1 is bat, 2 is paralysis demon, 3 is hellhound, 4 is blood golem, 5 is Gargoyle.
    void FixedUpdate()
    {
        if (!isStunned) // do something, if not stunned
        {
            if (EnemytypeID == 5)
            {
                GargoyleBehavior.GargoylePassover();
            }
            else if (!isDead && invincibilityCount == 0)
            {
                switch (EnemytypeID)
                {
                    case 0:
                        hellhoundBehavior.HellhoundPassover();
                        break;
                    case 1:
                        batBehavior.BatPassover();
                        break;
                    case 2:
                        paralysisDemonBehavior.PDPassover();
                        break;
                    case 3:
                        SpiderBehavior.spiderPassover();
                        break;
                    case 4:
                        GolemBehavior.GolemPassover();
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            // do nothing
        }
        if (invincibilityCount > 0)
        {
            invincibilityCount -= 1;
            if(!playerInZone && invincibilityCount == 0)
            {
                CC2D.enabled = true;
            }
            // hitStunBlink
            // calculate the blink time based on frequency
            float blinkTime = Mathf.Sin(Time.time * blinkFrequency);
            // set the sprite renderer to be visible if blink time is positive, otherwise invisible
            spriteRenderer.enabled = (blinkTime > 0f);
        }
        else { spriteRenderer.enabled = true; }
    }

    public void Hit(int attackDamage, Vector3 playerPosition, string statusEffect, EnemyController enemyController)
    {
        if (!isDead)
        {
            if(enemyController == this || enemyController == null) // latter condition is used by PlayerPrimaryWeapon
            {
                if (invincibilityCount == 0)
                {
                    //rb.AddForce(new Vector2(5.0f * facingDirection, 5.0f), ForceMode2D.Impulse);
                    damageInterupt = true; //might have to make change this later //isAttacking = false;
                    invincibilityCount = invincibilitySet;
                    Debug.Log("Hit is being called");
                    if (statusEffect != null) { StatusModifier(statusEffect); }
                    TakeDamage(attackDamage);
                    if (!isDead) { HandleHitPhysics(playerPosition); }
                }
            } 
        }
    }

    public void Hit(int attackDamage, Vector3 playerPosition) { Hit(attackDamage, playerPosition, null, null); } // used 

    //public void Hit(int weaponID, int LevelOfWeapon, Vector3 playerPosition)  
    //{
    //    statusToPass = weaponDatabase.GetWeaponEffect(weaponID);
    //    Hit(weaponID, LevelOfWeapon, playerPosition, statusToPass); 
    //}

    public void Hit(int weaponID, int LevelOfWeapon, Vector3 playerPosition, string statusEffect, EnemyController enemyController) 
    {
        damageToPass = weaponDatabase.GetWeaponDamage(weaponID, LevelOfWeapon);
        statusEffect = weaponDatabase.GetWeaponEffect(weaponID);
        Hit(damageToPass, playerPosition, statusEffect, enemyController);
    }

    public void StatusModifier(string mod)
    {
        if (mod == "DemonBlood") { if (GetComponentInChildren<Poisoned>() != null) { GetComponentInChildren<Poisoned>().Execute(); } }
        else if (mod == "Burn") { if (GetComponentInChildren<Burnable>() != null) { GetComponentInChildren<Burnable>().Execute(); } }
        else if (mod == "Stunned") { if (GetComponentInChildren<Stunned>() != null) { GetComponentInChildren<Stunned>().Execute(); } }
    }

    public void HandleHitPhysics(Vector3 playerPosition) 
    {
        if (transform.position.x <= playerPosition.x)
        {
            newVelocity.Set(-knockbackForce, 0.0f);
            rb.velocity = newVelocity;
            newForce.Set(0.0f, knockbackForce);
            rb.AddForce(newForce, ForceMode2D.Impulse);
        }
        else
        {
            newVelocity.Set(knockbackForce, 0.0f);
            rb.velocity = newVelocity;
            newForce.Set(0.0f, knockbackForce);
            rb.AddForce(newForce, ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(int damage) 
    { 

        HP -= damage;
        enemyHealth.UpdateHealthUI(HP);
        Debug.Log("Enemy " + gameObject.name + " was damaged! It took: " + damage + "damage. It's current HP is: " + HP);
        if (HP <= 0) { HPZero(); }
    }

    public void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    public void HPZero()
    {
        Instantiate(Resources.Load("VFXPrefabs/EnemyDeath"), transform.position, Quaternion.identity);
        Debug.Log(name + " is dead!");
        isDead = true;
        if(OnDeath != null) { OnDeath(this); }
        CC2D.enabled = false;
        spriteRenderer.enabled = false;
        string enemyDeathSound = gameObject.tag.ToString() + "Death"; // creates string that AudioManager recognizes; tag should match asset in AudioManager
        FindObjectOfType<AudioManager>().PlaySFX(enemyDeathSound);
        if(GetComponent<EnemyLoot>() != null) { GetComponent<EnemyLoot>().InstantiateLoot(transform.position);}
        Destroy(gameObject);

    }

    void setupHelper() // to load in relevant enemy stats and behavior script
    {
        if (CompareTag("Hellhound")) { EnemytypeID = 0; hellhoundBehavior = GetComponent<HellhoundBehavior>(); } // add enemyID as in enemy database + behavior component
        else if (CompareTag("Bat")) { EnemytypeID = 1; batBehavior = GetComponent<BatBehavior>(); }
        else if (CompareTag("ParalysisDemon")) { EnemytypeID = 2; paralysisDemonBehavior = GetComponent<ParalysisDemonBehavior>(); }
        else if (CompareTag("Spider")) { EnemytypeID = 3; SpiderBehavior = GetComponent<SpiderBehavior>(); }
        else if (CompareTag("BloodGolem")) { EnemytypeID = 4; GolemBehavior = GetComponent<BloodGolemBehavior>(); }
        else if (CompareTag("Gargoyle")) { EnemytypeID = 5; GargoyleBehavior = GetComponent<GargoyleBehavior>(); }
        else EnemytypeID = -1;

        //this is for setting up the values of hp, damage, etc
        if(EnemytypeID  >= 0)
        {
            var loadedValue = enemyDatabase.data.entries[EnemytypeID];

            UpdateMultipliers(loadedValue);

            (HP, HP_MAX) = ((int)(loadedValue.health * hpMultiplier), (int)(loadedValue.health * hpMultiplier));
            dmgVal1 = (int)(loadedValue.attack1Damage * apMultiplier); //10;
            dmgVal2 = (int)(loadedValue.attack2Damage * apMultiplier);
            dmgVal3 = (int)(loadedValue.attack3Damage * apMultiplier);
            dmgVal4 = (int)(loadedValue.attack4Damage * apMultiplier);
            dmgVal5 = (int)(loadedValue.attack5Damage * apMultiplier);
            dmgVal6 = (int)(loadedValue.attack6Damage * apMultiplier);
            SoulPointsDropped = (int)(loadedValue.soulPointsDropped); //45;
            knockbackForce = (int)(loadedValue.knockback); //3
        }
    }

    void UpdateMultipliers(EnemyData data)
    {
        // assume medium, but otherwise get the difficulty level from player prefs
        string difficultyLevel = "Medium";
        if (PlayerPrefs.HasKey("DifficultyLevel")) { difficultyLevel = PlayerPrefs.GetString("DifficultyLevel"); }

        // set multipliers based on value
        if(difficultyLevel == "Easy") { hpMultiplier = data.easyHPMultiplier; apMultiplier = data.easyAPMultiplier; }
        else if(difficultyLevel == "Hard") { hpMultiplier = data.hardHPMultiplier; apMultiplier = data.hardAPMultiplier; }
        else { hpMultiplier = data.mediumHPMultiplier; apMultiplier = data.mediumAPMultiplier; }
    }
    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onEnemyEnviroDamage -= Hit;
        EventSystem.current.onEnemyHitCollision -= Hit;
    }

}
