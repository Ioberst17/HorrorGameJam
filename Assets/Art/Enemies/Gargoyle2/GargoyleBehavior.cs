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
    public int SoulPointsDropped;
    public bool ragemode = false;
    private float DistanceToPlayerX;
    private float DistanceToPlayerY;
    public int AntiAirTimer;
    public bool AntiAirTrigger;
    public bool isAttacking;
    public int actionRandomizer;


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
        stateTimer = 60;
        AntiAirTrigger = false;
        GargRB = GetComponent<Rigidbody2D>();
        enemyController.knockbackForce = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GargoylePassover()
    {
        if(bossAction != "Dormant")
        {
            DistanceToPlayerX = transform.position.x - enemyController.playerLocation.position.x;
            DistanceToPlayerY = transform.position.y - enemyController.playerLocation.position.y;
            if (DistanceToPlayerY < -1)
            {

                if (AntiAirTimer > 75)
                {
                    AntiAirTrigger = true;
                }
                else
                {
                    ++AntiAirTimer;
                }

            }
            else if (AntiAirTimer > -75)
            {
                AntiAirTimer--;
            }

            if(AntiAirTimer < 0)
            {
                AntiAirTrigger = false;
            }

            if (stateTimer > 0)
            {
                stateTimer--;
            }
        }
        if(bossAction == "Dormant" && enemyController.playerInZone)
        {
            bossAction = "Waking";
            animator.Play("GargoyleWake");
            stateTimer = 60;
            enemyController.invincibilityCount = 60;
        }

        if(stateTimer == 0)
        {
            bossAction = "Standing";
            actionRandomizer = Random.Range(1, 10);

            if (enemyController.playerLocation.position.x > transform.position.x && enemyController.facingDirection == 1)
            {
                enemyController.Flip();
            }
            else if (enemyController.playerLocation.position.x < transform.position.x && enemyController.facingDirection == -1)
            {
                enemyController.Flip();
            }

            if (AntiAirTrigger)
            {
                if(bossState == "Grounded")
                {
                    if (actionRandomizer == 1)
                    {
                        FireBreath();
                    }
                    else if (actionRandomizer == 2)
                    {
                        TakeOff();
                    }
                }
                else if(bossState == "Airborne")
                {
                    if (actionRandomizer == 4)
                    {
                        Hover();
                    }
                    else if (actionRandomizer == 5)
                    {
                        Glide();
                    }
                    else if (actionRandomizer == 6)
                    {
                        AirAttack();
                    }
                }
            }
            else
            {
                if (bossAction == "Standing" && bossState =="Grounded" && (Mathf.Abs(DistanceToPlayerX)) < 2)
                {
                    GroundAttack();
                }
                else if (bossAction == "Standing" && bossState == "Grounded" && (Mathf.Abs(DistanceToPlayerX)) > 2)
                {
                    Walk();
                }
                else if (actionRandomizer == 7 && bossState == "Grounded")
                {
                    GroundSlam();
                }
                if (actionRandomizer == 3 && bossState == "Airborne")
                {
                    Land();
                }
            }
        }
        AttackHelper();
    }


    void GroundAttack()
    {
        bossAction = "Ground Attack";
        GargRB.velocity = new Vector2(0, GargRB.velocity.y);
        animator.Play("GargoyleSlashGround");
        stateTimer = 60;
        Attack(0);
    }
    void TakeOff()
    {
        bossAction = "Takeoff";
        bossState = "Airborne";
        animator.Play("GargoyleLaunch");
        GargRB.gravityScale = 0;
        stateTimer = 60;
        takeoffHelper();
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
        GargRB.velocity = new Vector2(3.0f*-enemyController.facingDirection, GargRB.velocity.y);
        animator.Play("GargoyleWalk");
        stateTimer = 15;
    }
    void Hover()
    {
        bossAction = "Hover";
        animator.Play("GargoyleFly");
        stateTimer = 48;
        if(transform.position.y < topLeftPoint.position.y)
        {
            GargRB.velocity = new Vector2(3.0f * -enemyController.facingDirection, 3.0f);
        }
        else
        {
            GargRB.velocity = new Vector2(3.0f * -enemyController.facingDirection, 0);
        }
    }
    void GroundSlam()
    {
        bossAction = "GroundSlam";
        Debug.Log("Groundslam");
        Attack(2);
        animator.Play("GargoyleLaunch2");
        //40
        stateTimer = 100;
        animator.Play("GargoyleLand2");
        //48
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
    void AirAttack()
    {
        
        bossAction = "Airattack";
        animator.Play("GargoyleSlashAir");
        stateTimer = 91;
        Attack(1);
        if (transform.position.y > DistanceToPlayerY - 2)
        {
            GargRB.velocity = new Vector2(6.0f * -enemyController.facingDirection, -3.0f);
        }
        else
        {
            GargRB.velocity = new Vector2(6.0f * -enemyController.facingDirection, 0);
        }
    }

    //0 is groundswipe, 1 is airSwipe, 2 is stomp, and 3 is firebreath
    public void Attack(int attackDirection)
    {
            isAttacking = true;
            StartCoroutine(AttackActiveFrames(attackDirection));
    }

    IEnumerator takeoffHelper()
    {
        yield return new WaitForSeconds(0.6f);
        GargRB.AddForce(new Vector2(0, 25.0f));
    }

   //controlls the attack frame data
    IEnumerator AttackActiveFrames(int attackDirection) // is called by the trigger event for powerups to countdown how long the power lasts
    {
        switch (attackDirection)
        {
            case 0:
                yield return new WaitForSeconds(0.6f);
                groundswipe.SetActive(true);
                yield return new WaitForSeconds(1); // waits a certain number of seconds
                groundswipe.SetActive(false);
                yield return new WaitForSeconds(0.6f);
                isAttacking = false;
                break;
            case 1:
                yield return new WaitForSeconds(0.6f);
                airSwipe.SetActive(true);
                yield return new WaitForSeconds(4); // waits a certain number of seconds
                airSwipe.SetActive(false);
                yield return new WaitForSeconds(0.6f);
                isAttacking = false;
                break;
            case 2:
                yield return new WaitForSeconds(0.6f);
                groundSlam.SetActive(true);
                yield return new WaitForSeconds(4); // waits a certain number of seconds
                groundSlam.SetActive(false);
                yield return new WaitForSeconds(0.6f);
                isAttacking = false;
                break;
            case 3:
                yield return new WaitForSeconds(0.6f);
                firebreath.SetActive(true);
                yield return new WaitForSeconds(2); // waits a certain number of seconds
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
                playerController.takeDamage(transform.position, groundswipeDamage, 1);
            }
        }
        else if (airSwipe.activeSelf)
        {

            if (Physics2D.OverlapArea(airSwipePoint1.position, airSwipePoint2.position, whatIsPlayer))
            {
                playerController.takeDamage(transform.position, airSwipeDamage, 1); 
            }
        }
        else if(groundSlam.activeSelf)
        {

            if (Physics2D.OverlapArea(GroundSlamPoint1.position, GroundSlamPoint2.position, whatIsPlayer))
            {
                playerController.takeDamage(transform.position, groundSlamDamage, 1);
            }

        }
        else if(firebreath.activeSelf)
        {

            if (Physics2D.OverlapArea(firebreathPoint1.position, firebreathPoint2.position, whatIsPlayer))
            {
                playerController.takeDamage(transform.position, firebreathDamage, 1);
            }
        }
    }
}
