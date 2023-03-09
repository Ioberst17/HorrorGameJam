using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    private SpriteRenderer enemySpriteRenderer;
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
    public int damageValue;

    public int HP;
    private int damageToPass;

    public int invincibilityCount;
    [SerializeField]
    private int invincibilitySet;
    private bool isDead;
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
        playerController = GameObject.Find("PlayerModel").GetComponent<PlayerController>();
        playerLocation = GameObject.Find("PlayerModel").transform;
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        enemyDatabase = GameObject.Find("EnemyDatabase").GetComponent<EnemyDatabase>();
        animator = GetComponent<Animator>();
        startingLocation = transform.position;
        patrol1Point = patrol1.position;
        patrol2Point = patrol2.position;
        invincibilityCount = 0;
        patrolID = 0;
       
        enemySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    //this is what actually deals the damage
    public void OnTriggerEnter2DHelper(Collider2D collider)
    {
        if (!isDead)
        {
            if (playerInZone)
            {
                if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player") && isAttacking)
                {
                    //playerController.takeDamage(transform.position, damageValue, 1);
                    rb.AddForce(new Vector2(knockbackForce * -facingDirection, 0.0f), ForceMode2D.Impulse);
                }
            }
        }

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
        else
        {
            //newVelocity.Set(0, 0);
            //rb.velocity = newVelocity;
        }
        if (invincibilityCount > 0)
        {
            invincibilityCount -= 1;
            if(!playerInZone && invincibilityCount == 0)
            {
                CC2D.enabled = true;

            }
        }

    }
    public void Hit(int attackDamage, Vector3 playerPosition)
    {
        if (!isDead)
        {
            if (invincibilityCount == 0)
            {
                //rb.AddForce(new Vector2(5.0f * facingDirection, 5.0f), ForceMode2D.Impulse);
                damageInterupt = true; //might have to make change this later //isAttacking = false;
                invincibilityCount = invincibilitySet;
                TakeDamage(attackDamage);
                if(!isDead) { HandleHitPhysics(playerPosition); }
            }
        }
    }

    public void Hit(int weaponID, int LevelOfWeapon, Vector3 playerPosition) 
    {
        damageToPass = weaponDatabase.GetWeaponDamage(weaponID, LevelOfWeapon);
        Hit(damageToPass, playerPosition);
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
        GetComponent<EnemyHealth>().UpdateHealthUI(HP);
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
        Debug.Log(name + " is dead!");
        isDead = true;
        CC2D.enabled = false;
        enemySpriteRenderer.enabled = false;
        playerController.gainSP(SoulPointsDropped);
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
        else if (CompareTag("Bloodgolem")) { EnemytypeID = 4; GolemBehavior = GetComponent<BloodGolemBehavior>(); }
        else if (CompareTag("Gargoyle")) { EnemytypeID = 5; GargoyleBehavior = GetComponent<GargoyleBehavior>(); }
        else EnemytypeID = -1;

        //this is for setting up the values of hp, damage, etc
        if(EnemytypeID  >= 0)
        {
            var loadedValue = enemyDatabase.enemyDatabase.entries[EnemytypeID];
            HP = loadedValue.health; //50;
            damageValue = loadedValue.attack1Damage; //10;
            SoulPointsDropped = loadedValue.soulPointsDropped; //45;
            knockbackForce = loadedValue.knockback; //3
        }

    }
    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onEnemyEnviroDamage -= Hit;
        EventSystem.current.onEnemyHitCollision -= Hit;
    }

}
