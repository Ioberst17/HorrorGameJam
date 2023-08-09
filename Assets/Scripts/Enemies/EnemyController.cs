using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : Controller
{
    [Header("Enemy Controller Values & References")]
    // EXTERNAL REFERENCES
    private PlayerController playerController;
    public Transform playerLocation;

    // OTHER ENEMY COMPONENTS
    // common across enemies
    private SpriteRenderer _spriteRenderer;  public SpriteRenderer SpriteRenderer {get { return _spriteRenderer; }set { _spriteRenderer = value; } }    
    public Animator animator;
    public Collider2D hitBox;

    //The points and values regarding the patrol code
    [SerializeField] public Transform patrol1;
    public Vector3 patrol1Point;
    [SerializeField] public Transform patrol2;
    public Vector3 patrol2Point;
    public int patrolID;

    // Attack Power
    public EnemyDatabase enemyDatabase; // External reference: used to load in values for enemies e.g. health data, attack info
    private EnemyData _enemyInfo; public EnemyData EnemyInfo { get { return _enemyInfo; } set { _enemyInfo = value; } }
    public int dmgVal1, dmgVal2, dmgVal3, dmgVal4, dmgVal5, dmgVal6;
    public int mostRecentAttack;

    // EnemyHealth
    Health enemyHealth;
    [SerializeField] float hpMultiplier;
    [SerializeField] float apMultiplier;
    
    private string statusToPass;
    public bool playerInZone;
    public int knockbackForce;
    private int SoulPointsDropped;
    private int EnemytypeID;

    void Awake()
    {
        // EXTERNAL REFERENCES
        playerController = GameObject.Find("PlayerModel").GetComponent<PlayerController>();
        playerLocation = playerController.transform;
        enemyDatabase = GameObject.Find("EnemyDatabase").GetComponent<EnemyDatabase>();

        // OTHER ENEMY COMPONENTS
        
        hitBox = GetComponent<CapsuleCollider2D>();
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // INITIALIZE VARIABLES
        StartingLocation = transform.position;
        patrol1Point = patrol1.position;
        patrol2Point = patrol2.position;
        patrolID = 0;
    }

    override protected void Start() { base.Start(); SetupHelper(); }

    // called in FixedUpdate of the parent when a player is hit
    override protected void HitStunBlink()
    {
        if (InHitStun)
        {
            // calculate the blink time based on frequency
            float blinkTime = Mathf.Sin(Time.time * blinkFrequency);
            // set the sprite renderer to be visible if blink time is positive, otherwise invisible
            SpriteRenderer.enabled = (blinkTime > 0f);
        }
        else { SpriteRenderer.enabled = true; hitBox.enabled = true; }
    }

    public void StatusModifier(string mod)
    {
        if (mod == "DemonBlood") { if (GetComponentInChildren<Poisoned>() != null) { GetComponentInChildren<Poisoned>().Execute(); } }
        else if (mod == "Burn") { if (GetComponentInChildren<Burnable>() != null) { GetComponentInChildren<Burnable>().Execute(); } }
        else if (mod == "Stunned") { if (GetComponentInChildren<Stunned>() != null) { GetComponentInChildren<Stunned>().Execute(); } }
    }

    override public void HandleHitPhysics(Vector3 playerPosition, float knockbackMod) 
    {
        if (!IsDead)
        {
            // if player is to the right, knock left; otherwise knock right
            if (transform.position.x <= playerPosition.x) { SetVelocity(-knockbackForce, 0.0f); }
            else { SetVelocity(knockbackForce, 0.0f); }
            
            // add y degree force
            AddForce(0.0f, knockbackForce);
        }
    }

    void SetupHelper() // to load in relevant enemy stats and behavior script
    {
        if (CompareTag("Hellhound")) { EnemytypeID = 0; } // add enemyID as in enemy database + behavior component
        else if (CompareTag("Bat")) { EnemytypeID = 1; }
        else if (CompareTag("ParalysisDemon")) { EnemytypeID = 2; }
        else if (CompareTag("Spider")) { EnemytypeID = 3; }
        else if (CompareTag("BloodGolem")) { EnemytypeID = 4; }
        else if (CompareTag("Gargoyle")) { EnemytypeID = 5; }
        else EnemytypeID = -1;

        //this is for setting up the values of hp, damage, etc
        if(EnemytypeID  >= 0)
        {
            var loadedValue = enemyDatabase.data.entries[EnemytypeID];

            UpdateMultipliers(loadedValue);

            EnemyInfo = loadedValue;
            (enemyHealth.MaxHealth, enemyHealth.HP) = ((int)(loadedValue.health * hpMultiplier), (int)(loadedValue.health * hpMultiplier));
            dmgVal1 = (int)(loadedValue.attack1Damage * apMultiplier); //10;
            dmgVal2 = (int)(loadedValue.attack2Damage * apMultiplier);
            dmgVal3 = (int)(loadedValue.attack3Damage * apMultiplier);
            dmgVal4 = (int)(loadedValue.attack4Damage * apMultiplier);
            dmgVal5 = (int)(loadedValue.attack5Damage * apMultiplier);
            dmgVal6 = (int)(loadedValue.attack6Damage * apMultiplier);
            SoulPointsDropped = (int)(loadedValue.soulPointsDropped); //45;
            knockbackForce = (int)(loadedValue.knockback); //3
            MovementSpeed = (float)(loadedValue.movementSpeed); //10
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

}
