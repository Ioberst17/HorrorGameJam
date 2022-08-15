using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
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
    private bool playerInZone;

    public int facingDirection = 1;

    void Awake()
    {
        playerInZone = false;
        isDead = false;
        rb = GetComponent<Rigidbody2D>();
        playerController = GameObject.Find("PlayerModel").GetComponent<PlayerController>();
        playerLocation = GameObject.Find("PlayerModel").transform;
        startingLocation = transform.position;
        patrol1Point = patrol1.position;
        patrol2Point = patrol2.position;
        invincibilityCount = 0;
        patrolID = 0;
        HP = 50;
}
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isDead)
        {
            if (playerInZone)
            {
                if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
                {
                    playerController.takeDamage(transform.position, damageValue, 1);
                }
            }
        }

    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {
            if (!playerInZone)
            {
                playerInZone = true;
                Debug.Log(playerInZone);
            }
        }
        
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {
            
            playerInZone = false;
            Debug.Log(playerInZone);
        }
            
    }
    
   // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
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
            if (invincibilityCount > 0)
            {
                invincibilityCount -= 1;
            }
        }
        else
        {
            newVelocity.Set(0, 0);
            rb.velocity = newVelocity;
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
        if (!isDead)
        {
            if (invincibilityCount == 0)
            {
                Debug.Log(name + " has been hit for " + attackDamage + "!");
                invincibilityCount = invincibilitySet;
                HP -= attackDamage;
                if (HP <= 0)
                {
                    Debug.Log(name + " is dead!");
                    isDead = true;
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
}
