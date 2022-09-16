using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private SpriteRenderer enemySpriteRenderer;
    private Rigidbody2D rb;
    private CapsuleCollider2D CC2D;
    private PlayerController playerController;
    private Transform playerLocation;
    private Vector2 newVelocity;
    [SerializeField] public int damageValue;
    private Vector3 startingLocation;
    [SerializeField] private Transform patrol1;
    private Vector3 patrol1Point;
    [SerializeField] private Transform patrol2;
    private Vector3 patrol2Point;
    [SerializeField] private float patrolSpeed;
    private int patrolID;
    private int HP;
    private int invincibilityCount;
    [SerializeField]
    private int invincibilitySet;
    private bool isDead;
    public bool playerInZone;
    public WeaponDatabase weaponDatabase; // used for ammo damage calcs

    [SerializeField]
    private int SoulPointsDropped;

    public int facingDirection = 1;

    void Awake()
    {
        playerInZone = false;
        isDead = false;
        rb = GetComponent<Rigidbody2D>();
        CC2D = GetComponent<CapsuleCollider2D>();
        playerController = GameObject.Find("PlayerModel").GetComponent<PlayerController>();
        playerLocation = GameObject.Find("PlayerModel").transform;
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        startingLocation = transform.position;
        patrol1Point = patrol1.position;
        patrol2Point = patrol2.position;
        invincibilityCount = 0;
        patrolID = 0;
        HP = 50;
        enemySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    public void OnTriggerEnter2DHelper(Collider2D collider)
    {
        if (!isDead)
        {
            if (playerInZone)
            {
                if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
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
            if (transform.position.x >= patrol1Point.x)
            {
                patrolID = 1;
            }
            if (transform.position.x <= patrol2Point.x)
            {
                patrolID = 2;
            }
            if (!playerInZone)
            {
                patrol();
            }
            else
            {
                chase();
            }
            if (rb.velocity.x >= 0.5f && facingDirection == -1)
            {
                Flip();
            }
            else if (rb.velocity.x <= -0.5f && facingDirection == 1)
            {
                Flip();
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
    private void patrol()
    {
        switch (patrolID)
        {
            case 0:
                newVelocity.Set(patrolSpeed, 0);
                rb.velocity = newVelocity;
                break;
            case 1:
                newVelocity.Set(-patrolSpeed, 0);
                rb.velocity = newVelocity;
                break;
            case 2:
                newVelocity.Set(patrolSpeed, 0);
                rb.velocity = newVelocity;
                break;
            default:
                break;
        }
    }
    private void chase()
    {
        if(playerLocation.position.x >= transform.position.x)
        {
            newVelocity.Set(patrolSpeed*1.5f, 0);
            rb.velocity = newVelocity;
        }
        else
        {
            newVelocity.Set(-patrolSpeed*1.5f, 0);
            rb.velocity = newVelocity;
        }
    }
    public bool calculateHit(int attackDamage)
    {
        Debug.Log(name + " called");
        if (!isDead)
        {
            if (invincibilityCount == 0)
            {
                //rb.AddForce(new Vector2(5.0f * facingDirection, 5.0f), ForceMode2D.Impulse);
                Debug.Log(name + " has been hit for " + attackDamage + "!");
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
                calculateHit(weaponDatabase.weaponDatabase.entries[weaponID].level1Damage);
                break;
            case 1:
                calculateHit(weaponDatabase.weaponDatabase.entries[weaponID].level2Damage);
                break;
            case 2:
                calculateHit(weaponDatabase.weaponDatabase.entries[weaponID].level3Damage);
                break;
        }
        
        
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        //EventSystem.current.onAttackCollision -= AmmoDamage;
    }

}
