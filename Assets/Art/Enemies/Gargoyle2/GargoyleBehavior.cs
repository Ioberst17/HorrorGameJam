using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoyleBehavior : MonoBehaviour
{
    [SerializeField] private Transform topLeftPoint;
    [SerializeField] private Transform bottomRightPoint;
    [SerializeField] private Animator animator;
    private EnemyController enemyController;
    private PlayerController playerController;
    private Rigidbody2D GargRB;
    public string bossAction;
    public string bossState;
    public int stateTimer;
    public int HP;
    public int StartingHP;
    public int SoulPointsDropped;
    public bool phase2 = false;
    public bool ragemode = false;
    private float DistanceToPlayerX;
    private float DistanceToPlayerY;
    private float HeightOfPlayer;
    public bool playerIsOnLeftSide;
    private int FlightTimer;
    public int AntiAirTimer;
    public bool AntiAirTrigger;
    public bool isAttacking;
    public bool ongoingPattern = false;
    public int actionRandomizer;
    [SerializeField]
    private int groundSpeed;


    [SerializeField]
    private GameObject groundswipe;
    [SerializeField]
    private Transform groundswipePoint1;
    [SerializeField]
    private Transform groundswipePoint2;
    [SerializeField]
    private int groundswipeDamage;
    [SerializeField]
    private GameObject groundSlam;
    [SerializeField]
    private Transform GroundSlamPoint1;
    [SerializeField]
    private Transform GroundSlamPoint2;
    [SerializeField]
    private int groundSlamDamage;
    [SerializeField]
    private GameObject airSwipe;
    [SerializeField]
    private Transform airSwipePoint1;
    [SerializeField]
    private Transform airSwipePoint2;
    [SerializeField]
    private int airSwipeDamage;
    [SerializeField]
    private GameObject firebreath;
    [SerializeField]
    private Transform firebreathPoint1;
    [SerializeField]
    private Transform firebreathPoint2;
    [SerializeField]
    private int firebreathDamage;
    [SerializeField]
    private LayerMask whatIsPlayer;

    private Collider2D[] hitlist;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        playerController = GameObject.Find("PlayerModel").GetComponent<PlayerController>();
        //animator = GetComponentInParent<Animator>();
        bossAction = "Dormant";
        bossState = "Grounded";
        AntiAirTimer = 0;
        FlightTimer = 0;
        stateTimer = 60;
        AntiAirTrigger = false;
        GargRB = GetComponent<Rigidbody2D>();
        enemyController.knockbackForce = 0;
        enemyController.HP = 200;
        StartingHP = enemyController.HP;
        HP = StartingHP;
        enemyController.HP = HP;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GargoylePassover()
    {
        HP = enemyController.HP;
        DistanceToPlayerX = transform.position.x - enemyController.playerLocation.position.x;
        DistanceToPlayerY = transform.position.y - enemyController.playerLocation.position.y;
        HeightOfPlayer = bottomRightPoint.position.y - enemyController.playerLocation.position.y;
        playerIsOnLeftSide = (Mathf.Abs(bottomRightPoint.position.x - enemyController.playerLocation.position.x) > Mathf.Abs(topLeftPoint.position.x - enemyController.playerLocation.position.x));
        if (!phase2)
        {
            phase2 = (HP <= StartingHP * (3 / 4));
        }
        if (!ragemode)
        {
            ragemode = (HP <= StartingHP * (1 / 3));
        }
        if (bossAction == "Dormant" && enemyController.playerInZone)
        {
            bossAction = "Waking";
            animator.Play("GargoyleWake");
            stateTimer = 105;
            enemyController.invincibilityCount = 105;
        }
        else if (stateTimer > 0)
        {
            --stateTimer;
        }
        AttackHelper();
        if (stateTimer == 0)
        {
            if (enemyController.playerLocation.position.x > transform.position.x && enemyController.facingDirection == 1)
            {
                enemyController.Flip();
            }
            else if (enemyController.playerLocation.position.x < transform.position.x && enemyController.facingDirection == -1)
            {
                enemyController.Flip();
            }
            if (!ongoingPattern)
            {
                if (!phase2)
                {
                    ongoingPattern = true;
                    StartCoroutine(attackPattern1());
                }
                else if (!ragemode)
                {
                    ongoingPattern = true;
                    StartCoroutine(attackPattern2());
                }
                else
                {
                    ongoingPattern = true;
                    StartCoroutine(attackPattern3());
                }
            }
        }
    }
    IEnumerator attackPattern1()
    {
        Debug.Log("Pattern1");
        int count = 0;
        int prevCount = -1;
        while (count < 3)
        {
            TurnCheck();
            yield return new WaitForSeconds(0.01f);
            if (prevCount < count)
            {
                Debug.Log("Count is: " + count);
                prevCount = count;
            }

            while (Mathf.Abs(DistanceToPlayerX) > 2 && !isAttacking)
            {
                yield return new WaitForSeconds(0.01f);
                Walk();
                yield return new WaitForSeconds(0.01f);
            }
            if (Mathf.Abs(DistanceToPlayerX) <= 2)
            {
                isAttacking = true;
                yield return new WaitForSeconds(0.01f);
                GroundAttack();
                yield return new WaitForSeconds(1.10f);
                ++count;
            }

        }
        TurnCheck();
        while (isAttacking)
        {
            yield return new WaitForSeconds(0.01f);
        }
        while (Mathf.Abs(DistanceToPlayerX) > 3)
        {
            TurnCheck();
            Walk();
            yield return new WaitForSeconds(0.15f);
        }
        isAttacking = true;
        GroundSlam();
        yield return new WaitWhile(() => isAttacking);
        ongoingPattern = false;
        stateTimer = 3;
        yield return new WaitForSeconds(0.01f);
    }

    IEnumerator attackPattern2()
    {
        Debug.Log("Pattern2");
        ongoingPattern = true;
        TakeOff();
        yield return new WaitForSeconds(1.6f);
        Hover();
        yield return new WaitForSeconds(1.06f);
        while (!ragemode)
        {
            TurnCheck();
            Hover();
            yield return new WaitForSeconds(2f);
            if (Mathf.Abs(DistanceToPlayerX) > 2)
            {
                StartCoroutine(AirAttack());
                yield return new WaitWhile(() => isAttacking);
                //yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(0.5f);
            TurnCheck();
            Hover();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitWhile(() => isAttacking);
        GargRB.velocity = new Vector2(0, -3.0f);
        Land();

        ongoingPattern = false;

    }
    IEnumerator attackPattern3()
    {
        Debug.Log("Pattern3");
        ongoingPattern = true;
        FireBreath();
        yield return new WaitForSeconds(2f);
        int count = 0;
        while (count < 3)
        {
            yield return new WaitForSeconds(0.01f);
            Debug.Log("Count is: " + count);
            while (Mathf.Abs(DistanceToPlayerX) > 2)
            {
                yield return new WaitForSeconds(0.01f);
                Walk();
                yield return new WaitForSeconds(0.6f);
            }
            if (!isAttacking)
            {
                Debug.Log("attacking");
                GroundAttack();
                yield return new WaitForSeconds(1f);
                ++count;
            }
            TurnCheck();
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.01f);
        TurnCheck();
        while (Mathf.Abs(DistanceToPlayerX) > 3)
        {
            TurnCheck();
            yield return new WaitForSeconds(0.01f);
            Walk();
            yield return new WaitForSeconds(0.6f);
        }
        GroundSlam();
        ongoingPattern = false;


    }
    void TurnCheck()
    {
        if (enemyController.playerLocation.position.x > transform.position.x && enemyController.facingDirection == 1)
        {
            enemyController.Flip();
        }
        else if (enemyController.playerLocation.position.x < transform.position.x && enemyController.facingDirection == -1)
        {
            enemyController.Flip();
        }
    }
    void GroundAttack()
    {
        bossAction = "Ground Attack";
        Debug.Log("Ground Attack");
        GargRB.velocity = new Vector2(0, GargRB.velocity.y);
        animator.Play("GargoyleSlashGround");
        stateTimer = 70;
        Attack(0);
    }
    void TakeOff()
    {
        bossAction = "Takeoff";
        bossState = "Airborne";
        animator.Play("GargoyleLaunch");
        GargRB.gravityScale = 0;
        stateTimer = 60;
        StartCoroutine(takeoffHelper());
    }
    IEnumerator takeoffHelper()
    {
        yield return new WaitForSeconds(0.6f);
        GargRB.AddForce(new Vector2(0, 250.0f));
        yield return new WaitForSeconds(0.01f);
    }

    void Land()
    {
        bossState = "Grounded";
        bossAction = "Land";
        GargRB.gravityScale = 1;
        animator.Play("GargoyleLand");
        stateTimer = 89;
    }
    void FireBreath()
    {
        bossAction = "FireBreath";
        animator.Play("GargoyleBreathGround");
        stateTimer = 200;
        Attack(3);
    }
    void Walk()
    {
        bossAction = "Walk";
        GargRB.velocity = new Vector2(groundSpeed * -enemyController.facingDirection, GargRB.velocity.y);
        animator.Play("GargoyleWalk");
        stateTimer = 0;
    }
    void Hover()
    {
        bossAction = "Hover";
        animator.Play("GargoyleFly");
        stateTimer = 48;
        if (playerIsOnLeftSide)
        {
            GargRB.velocity = new Vector2(10.0f, 6.0f);
        }
        else
        {
            GargRB.velocity = new Vector2(-10.0f, 6);
        }
    }
    void GroundSlam()
    {
        stateTimer = 100;
        isAttacking = true;
        bossAction = "GroundSlam";
        Debug.Log("Groundslam");
        animator.Play("GargoyleLaunch2");
        StartCoroutine(SlamHelper());
    }
    IEnumerator SlamHelper()
    {
        yield return new WaitForSeconds(0.40f);
        GargRB.velocity = new Vector2(1.0f * -enemyController.facingDirection, 10.0f);
        yield return new WaitForSeconds(0.10f);
        GargRB.velocity = new Vector2(1.0f * -enemyController.facingDirection, 5.0f);
        yield return new WaitForSeconds(0.05f);
        GargRB.velocity = new Vector2(1.0f * -enemyController.facingDirection, -10.0f);
        yield return new WaitForSeconds(0.15f);
        animator.Play("GargoyleLand2");
        Attack(2);
        GargRB.velocity = new Vector2(0 * -enemyController.facingDirection, 0);
        yield return new WaitForSeconds(0.01f);

    }
    void Glide()
    {
        bossAction = "Glide";
        animator.Play("GargoyleGlideStart");
        //111 frames
        animator.Play("GargoyleGlide");

        stateTimer = 54;
        animator.Play("GargoyleGlideRecover");
    }
    IEnumerator AirAttack()
    {
        yield return new WaitForSeconds(0.01f);
        isAttacking = true;
        bossAction = "Airattack";
        animator.Play("GargoyleSlashAir");
        stateTimer = 91;
        Attack(1);
        yield return new WaitForSeconds(0.2f);
        GargRB.velocity = new Vector2(15.0f * -enemyController.facingDirection, -12.0f);
        yield return new WaitForSeconds(0.7f);
        GargRB.velocity = new Vector2(10.0f * -enemyController.facingDirection, 7.0f);
        yield return new WaitForSeconds(0.7f);
        isAttacking = false;
    }

    //0 is groundswipe, 1 is airSwipe, 2 is stomp, and 3 is firebreath
    public void Attack(int attackDirection)
    {
        isAttacking = true;
        StartCoroutine(AttackActiveFrames(attackDirection));
    }


    //controlls the attack frame data 0 is groundswipe, 1 is airSwipe, 2 is stomp, and 3 is firebreath
    IEnumerator AttackActiveFrames(int attackDirection) // is called by the trigger event for powerups to countdown how long the power lasts
    {
        switch (attackDirection)
        {
            case 0:
                yield return new WaitForSeconds(0.5f);
                groundswipe.SetActive(true);
                yield return new WaitForSeconds(0.11f); // waits a certain number of seconds
                groundswipe.SetActive(false);
                yield return new WaitForSeconds(0.17f);
                isAttacking = false;
                //yield return new WaitForSeconds(0.11f);
                break;
            case 1:
                yield return new WaitForSeconds(0.54f);
                airSwipe.SetActive(true);
                yield return new WaitForSeconds(0.5f); // waits a certain number of seconds
                airSwipe.SetActive(false);
                break;
            case 2:
                //yield return new WaitForSeconds(0.2f);
                groundSlam.SetActive(true);
                yield return new WaitForSeconds(0.20f); // waits a certain number of seconds
                groundSlam.SetActive(false);
                yield return new WaitForSeconds(0.20f);
                isAttacking = false;
                break;
            case 3:
                yield return new WaitForSeconds(0.6f);
                firebreath.SetActive(true);
                yield return new WaitForSeconds(1.5f); // waits a certain number of seconds
                firebreath.SetActive(false);
                yield return new WaitForSeconds(0.6f);
                isAttacking = false;
                break;
            default:
                isAttacking = false;
                break;
        }
    }



    //This opperates the attack hit detection
    public void AttackHelper()
    {
        if (groundswipe.activeSelf)
        {

            if (Physics2D.OverlapArea(groundswipePoint1.position, groundswipePoint2.position, whatIsPlayer))
            {
                //playerController.Hit(transform.position, groundswipeDamage, 1);
                playerController.Hit(transform.position, 1);
            }
        }
        else if (airSwipe.activeSelf)
        {

            if (Physics2D.OverlapArea(airSwipePoint1.position, airSwipePoint2.position, whatIsPlayer))
            {
                //playerController.takeDamage(transform.position, airSwipeDamage, 1);
                playerController.Hit(transform.position, 1);
            }
        }
        else if (groundSlam.activeSelf)
        {

            if (Physics2D.OverlapArea(GroundSlamPoint1.position, GroundSlamPoint2.position, whatIsPlayer))
            {
                //playerController.takeDamage(transform.position, groundSlamDamage, 1);
                playerController.Hit(transform.position, 1);
            }

        }
        else if (firebreath.activeSelf)
        {

            if (Physics2D.OverlapArea(firebreathPoint1.position, firebreathPoint2.position, whatIsPlayer))
            {
                //playerController.takeDamage(transform.position, firebreathDamage, 1);
                playerController.Hit(transform.position, 1);
            }
        }
    }
}