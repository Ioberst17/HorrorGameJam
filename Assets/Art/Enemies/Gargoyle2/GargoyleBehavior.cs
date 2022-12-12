using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoyleBehavior : MonoBehaviour
{
    [SerializeField] private Transform topLeftPoint;
    [SerializeField] private Transform bottomRightPoint;
    [SerializeField] private Animator animator;
    private EnemyController enemyController;
    private Rigidbody2D GargRB;
    public string bossState;
    public int stateTimer = 0;
    public int HP;
    public int SoulPointsDropped;
    public bool ragemode = false;
    private float DistanceToPlayerX;
    private float DistanceToPlayerY;
    public int AntiAirTimer;
    public bool AntiAirTrigger;
    public int actionRandomizer;


    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        //animator = GetComponentInParent<Animator>();
        bossState = "Dormant";
        AntiAirTimer = 0;
        AntiAirTrigger = false;
        GargRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GargoylePassover()
    {
        if(bossState != "Dormant")
        {
            DistanceToPlayerX = transform.position.x - enemyController.playerLocation.position.x;
            DistanceToPlayerY = transform.position.y - enemyController.playerLocation.position.y;
            if (DistanceToPlayerY < -1)
            {

                if (AntiAirTimer > 260)
                {
                    AntiAirTrigger = true;
                }
                else
                {
                    AntiAirTimer++;
                }

            }
            else if (AntiAirTimer > -260)
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
        if(bossState == "Dormant" && enemyController.playerInZone)
        {
            bossState = "Standing";
            animator.Play("GargoyleWake");
            stateTimer = 60;
            enemyController.invincibilityCount = 60;
        }

        if(stateTimer == 0)
        {
            bossState = "Standing";
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
                if (actionRandomizer == 1)
                {
                    FireBreath();
                }
                else if (actionRandomizer == 2)
                {
                    TakeOff();
                }
                else if (actionRandomizer == 3)
                {
                    Land();
                }
                else if (actionRandomizer == 4)
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
            else
            {
                if (bossState == "Standing" && (Mathf.Abs(DistanceToPlayerX)) < 2)
                {
                    GroundAttack();
                }
                else if (bossState == "Standing" && (Mathf.Abs(DistanceToPlayerX)) > 2)
                {
                    Walk();
                }
                else if (actionRandomizer == 7)
                {
                    GroundSlam();
                }
            }
        }
    }
    void GroundAttack()
    {
        bossState = "Ground Attack";
        GargRB.velocity = new Vector2(0, GargRB.velocity.y);
        animator.Play("GargoyleSlashGround");
        stateTimer = 80;
    }
    void TakeOff()
    {
        bossState = "Takeoff";
        animator.Play("GargoyleLaunch");
        GargRB.gravityScale = 0;
        stateTimer = 60;
    }
    void Land()
    {
        bossState = "Land";
        GargRB.gravityScale = 1;
        animator.Play("GargoyleLand");
        stateTimer = 89;
    }
    void FireBreath()
    {
        bossState = "FireBreath";
        animator.Play("GargoyleBreathGround");
        stateTimer = 165;
    }
    void Walk()
    {
        bossState = "Walk";
        GargRB.velocity = new Vector2(3.0f*-enemyController.facingDirection, GargRB.velocity.y);
        animator.Play("GargoyleWalk");
        stateTimer = 15;
    }
    void Hover()
    {
        bossState = "Hover";
        animator.Play("GargoyleFly");
        stateTimer = 48;
    }
    void GroundSlam()
    {
        bossState = "GroundSlam";
        Debug.Log("Groundslam");
        animator.Play("GargoyleLaunch2");
        //40
        stateTimer = 100;
        animator.Play("GargoyleLand2");
        //48
    }
    void Glide()
    {
        bossState = "Glide";
        animator.Play("GargoyleGlideStart");
        //111 frames
        animator.Play("GargoyleGlide");
        
        stateTimer = 54;
        animator.Play("GargoyleGlideRecover");
    }
    void AirAttack()
    {
        bossState = "Airattack";
        animator.Play("GargoyleSlashAir");
        stateTimer = 91;
    }


//    if (Physics2D.OverlapArea(ADPoint1.position, ADPoint2.position, whatIsEnemy))
//            {
//                newVelocity.Set(0.0f, 0.0f);
//                rb.velocity = newVelocity;
//                newForce.Set(0.0f, jumpForce* 0.66f);
//                rb.AddForce(newForce, ForceMode2D.Impulse);

//                hitlist = Physics2D.OverlapAreaAll(ADPoint1.position, ADPoint2.position, whatIsEnemy);
//                int i = 0;
//                while (i<hitlist.Length)
//                {
//                    //Debug.Log(hitlist[i]);
//                    if (hitlist[i].GetType() == typeof(UnityEngine.CapsuleCollider2D))
//                    {
//                        GameController.passHit(hitlist[i].name, AttackDamage, transform.position);
//                    }
//i++;
//                }

//            }

}
