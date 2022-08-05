using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerController playerController;
    [SerializeField] public int damageValue;
    private Vector3 startingLocation;
    private int HP;
    private int invincibilityCount;
    [SerializeField]
    private int invincibilitySet;
    private bool isDead;

    void Awake()
    {
        isDead = false;
        rb = GetComponent<Rigidbody2D>();
        playerController = GameObject.Find("PlayerModel").GetComponent<PlayerController>();
        invincibilityCount = 0;
        HP = 50;
}
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!isDead)
        {
            if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
            {
                playerController.takeDamage(damageValue, 1);
            }
        }
        
    }
   // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(invincibilityCount > 0)
        {
            invincibilityCount -= 1;
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
}
