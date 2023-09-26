using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : Controller
{
    [Header("Enemy Controller Values & References")]
    // EXTERNAL REFERENCES
    // needed for knockback
    PlayerController playerController;
    public Transform playerLocation;

    // OTHER ENEMY COMPONENTS
    // common across enemies
    protected EnemyDataLoader enemyData;
    private SpriteRenderer _spriteRenderer;  public SpriteRenderer SpriteRenderer {get { return _spriteRenderer; }set { _spriteRenderer = value; } }    
    public Animator animator;

    //The points and values regarding the patrol code
    [SerializeField] public Transform patrol1; public Vector3 patrol1Point;
    [SerializeField] public Transform patrol2; public Vector3 patrol2Point;
    
    //to vary different types of patrols
    public int patrolID;

    [SerializeField] bool _playerInZone; public bool PlayerInZone { get { return _playerInZone; } set {_playerInZone = value; } }
    public int KnockBackForce { get; set; }


    private void Awake()
    {
        enemyData = GetComponent<EnemyDataLoader>();
        if (enemyData != null) { enemyData.DataLoaded += LoadInValues; }
    }

    override protected void Start() 
    { 
        base.Start();
        // EXTERNAL REFERENCES
        playerController = GameObject.Find("PlayerModel").GetComponent<PlayerController>();
        playerLocation = playerController.transform;

        // OTHER ENEMY COMPONENTS
        var animatorObject = transform.Find("Animator");
        animator = animatorObject.GetComponent<Animator>();
        SpriteRenderer = animatorObject.GetComponent<SpriteRenderer>();

        // INITIALIZE VARIABLES
        StartingLocation = transform.position;
        patrol1Point = patrol1.position;
        patrol2Point = patrol2.position;
        patrolID = 0;
        LoadInValues();
    }

    public float XDistanceFromPlayer()
    {
        return transform.position.x - playerLocation.position.x;
    }

    // called in FixedUpdate of the parent when hit
    override protected void HitStunBlink()
    {
        if (InHitStun)
        {
            // calculate the blink time based on frequency
            float blinkTime = Mathf.Sin(Time.time * blinkFrequency);
            // set the sprite renderer to be visible if blink time is positive, otherwise invisible
            SpriteRenderer.enabled = (blinkTime > 0f);
        }
        else { SpriteRenderer.enabled = true; /*hitBox.enabled = true;*/ }
    }

    override public void HandleHitPhysics(Vector3 playerPosition, float knockbackMod) 
    {
        if (!IsDead)
        {
            // if player is to the right, knock left; otherwise knock right
            if (transform.position.x <= playerPosition.x) { SetVelocity(-KnockBackForce, 0.0f); }
            else { SetVelocity(KnockBackForce, 0.0f); }
            
            // add y degree force
            AddForce(0.0f, KnockBackForce);
        }
    }

    void LoadInValues() // to load in relevant enemy stats and behavior script
    {
        KnockBackForce = (int)(enemyData.data.knockback); 
        MovementSpeed = (float)(enemyData.data.movementSpeed); 
    }

}
