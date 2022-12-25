using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
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
                    playerController.takeDamage(transform.position, damageValue, 1);
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
        EventSystem.current.onAttackCollision += AmmoDamage;
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
    public bool calculateHit(int attackDamage, Vector3 playerPosition)
    {
        Debug.Log(name + " called");
        if (!isDead)
        {
            if (invincibilityCount == 0)
            {
                //rb.AddForce(new Vector2(5.0f * facingDirection, 5.0f), ForceMode2D.Impulse);
                Debug.Log(name + " has been hit for " + attackDamage + "!");
                damageInterupt = true;
                //might have to make change this later
                //isAttacking = false;

                invincibilityCount = invincibilitySet;
                HP -= attackDamage;
                if (HP <= 0)
                {
                    Death();
                }
                else
                {
                    Debug.Log("Health remaining is " + HP);
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
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
        
    }
    public void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    public void Death()
    {
        Debug.Log(name + " is dead!");
        isDead = true;
        CC2D.enabled = false;
        enemySpriteRenderer.enabled = false;
        playerController.gainSP(SoulPointsDropped);
        string enemyDeathSound = gameObject.tag.ToString() + "Death"; // creates string that AudioManager recognizes; tag should match asset in AudioManager
        FindObjectOfType<AudioManager>().PlaySFX(enemyDeathSound);

    }

    public void AmmoDamage(int ammoID)
    {
        var weaponID = ammoID / 3; // since every weapon has 3 levels - this INT - will auto round down to the weaponID's database position in the array e.g. 2 > [0]th weapon in database list,; 7> [1]st weapon in database list
        var ammoLevel = ammoID % 3;

        switch (ammoLevel)
        {
            case 0:
                calculateHit(weaponDatabase.weaponDatabase.entries[weaponID].level1Damage, playerController.transform.position);
                break;
            case 1:
                calculateHit(weaponDatabase.weaponDatabase.entries[weaponID].level2Damage, playerController.transform.position);
                break;
            case 2:
                calculateHit(weaponDatabase.weaponDatabase.entries[weaponID].level3Damage, playerController.transform.position);
                break;
        }
        
        
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
        EventSystem.current.onAttackCollision -= AmmoDamage;
    }

}
