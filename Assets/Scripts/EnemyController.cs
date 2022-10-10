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
    public int damageValue;
    private int HP;
    private int invincibilityCount;
    [SerializeField]
    private int invincibilitySet;
    private bool isDead;
    public bool isAttacking;
    public bool playerInZone;
    public WeaponDatabase weaponDatabase; // used for ammo damage calcs
    private Vector2 newForce;
    private int knockbackForce;
    private int SoulPointsDropped;

    private int EnemytypeID;
    public int facingDirection = 1;

    [SerializeField] private HellhoundBehavior hellhoundBehavior;


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
        animator = GetComponent<Animator>();
        startingLocation = transform.position;
        patrol1Point = patrol1.position;
        patrol2Point = patrol2.position;
        invincibilityCount = 0;
        patrolID = 0;
       
        enemySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        
        setupHelper();
    }

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
                    rb.AddForce(new Vector2(5.0f * facingDirection, 0.0f), ForceMode2D.Impulse);
                }
            }
        }

    }

   // Start is called before the first frame update
    void Start()
    {
        //subscribe to important messages
        EventSystem.current.onAttackCollision += AmmoDamage;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead && invincibilityCount==0)
        {
            switch (EnemytypeID)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    hellhoundBehavior.HellhoundPassover();
                    break;
                case 4:
                    break;
                case 5:
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
        }

    }
    //private void patrol()
    //{
    //    switch (patrolID)
    //    {
    //        case 0:
    //            newVelocity.Set(patrolSpeed, 0);
    //            rb.velocity = newVelocity;
    //            break;
    //        case 1:
    //            newVelocity.Set(-patrolSpeed, 0);
    //            rb.velocity = newVelocity;
    //            break;
    //        case 2:
    //            newVelocity.Set(patrolSpeed, 0);
    //            rb.velocity = newVelocity;
    //            break;
    //        default:
    //            break;
    //    }
    //}
    //private void chase()
    //{
    //    if(playerLocation.position.x >= transform.position.x)
    //    {
    //        newVelocity.Set(patrolSpeed*1.5f, 0);
    //        rb.velocity = newVelocity;
    //    }
    //    else
    //    {
    //        newVelocity.Set(-patrolSpeed*1.5f, 0);
    //        rb.velocity = newVelocity;
    //    }
    //}
    public bool calculateHit(int attackDamage, Vector3 playerPosition)
    {
        Debug.Log(name + " called");
        if (!isDead)
        {
            if (invincibilityCount == 0)
            {
                //rb.AddForce(new Vector2(5.0f * facingDirection, 5.0f), ForceMode2D.Impulse);
                Debug.Log(name + " has been hit for " + attackDamage + "!");

                //might have to make change this later
                isAttacking = false;

                invincibilityCount = invincibilitySet;
                HP -= attackDamage;
                if (HP <= 0)
                {
                    Debug.Log(name + " is dead!");
                    isDead = true;
                    CC2D.enabled = false;
                    enemySpriteRenderer.enabled = false;
                    playerController.gainSP(SoulPointsDropped);
                }
                else
                {
                    Debug.Log("Health remaining is " + HP);
                    if (transform.position.x <= playerPosition.x)
                    {
                        newVelocity.Set(-5.0f, 0.0f);
                        rb.velocity = newVelocity;
                        newForce.Set(0.0f, knockbackForce);
                        rb.AddForce(newForce, ForceMode2D.Impulse);
                    }
                    else
                    {
                        newVelocity.Set(5.0f, 0.0f);
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

    public void AmmoDamage(int ammoID)
    {
        var weaponID = ammoID / 3; // since every weapon has 3 levels - this INT - will auto round down to the weaponID's database position in the array e.g. 2 > [0]th weapon in database list,; 7> [1]st weapon in database list
        var ammoLevel = ammoID % 3;
        Debug.Log("Weapon ID is: " + weaponID);
        Debug.Log("Ammo ID is: " + ammoLevel);

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
    void setupHelper()
    {
        if (CompareTag("Bat")) { EnemytypeID = 1; }
        else if (CompareTag("ParalysisDemon")) { EnemytypeID = 2; }
        else if (CompareTag("Hellhound")) { EnemytypeID = 3; }
        else if (CompareTag("Bloodgolem")) { EnemytypeID = 4; }
        else if (CompareTag("Spider")) { EnemytypeID = 5; }

        //this is for setting up the values of hp, damage, etc
        //should probably be coming from a database in the long run. 
        switch (EnemytypeID)
        {
            case 1:
                HP = 50;
                damageValue = 10;
                SoulPointsDropped = 45;
                knockbackForce = 3;
                break;
            case 2:
                HP = 50;
                damageValue = 10;
                SoulPointsDropped = 45;
                knockbackForce = 3;
                break;
            case 3:
                HP = 50;
                damageValue = 10;
                SoulPointsDropped = 45;
                knockbackForce = 3;
                hellhoundBehavior = GetComponent<HellhoundBehavior>();
                break;
            case 4:
                HP = 50;
                damageValue = 10;
                SoulPointsDropped = 45;
                knockbackForce = 3;
                break;
            case 5:
                HP = 50;
                damageValue = 10;
                SoulPointsDropped = 45;
                knockbackForce = 3;
                break;
            default:
                HP = 50;
                damageValue = 10;
                SoulPointsDropped = 45;
                knockbackForce = 3;
                break;
        }
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        EventSystem.current.onAttackCollision -= AmmoDamage;
    }

}
