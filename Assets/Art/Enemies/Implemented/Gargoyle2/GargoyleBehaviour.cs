using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoyleBehaviour : EnemyBehaviour
{
    // STATES
    public string bossAction;
    public string bossState;
    public int stateTimer;
    public bool phase2 = false;
    public bool rageMode = false;

    // ADDITIONAL STATES
    public int AntiAirTimer;
    public bool AntiAirTrigger;
    // keeps track of if gargoyle is in an attack loop
    public bool OnGoingPattern { get; set; } = false;
    public int actionRandomizer;

    // BOUNDARIES
    [SerializeField] private Transform topLeftPoint;
    [SerializeField] private Transform bottomRightPoint;

    // PLAYER RELATED
    float DistanceToPlayerX;
    public bool playerIsOnLeftSide;

    override protected void Start()
    {
        base.Start();
        enemyController.IsAttacking = false;
        bossAction = "Dormant";
        bossState = "Grounded";
        stateTimer = 60;
    }

    override protected void Passover()
    {
        GetPlayerPosition();

        UpdatePhase();

        WakingLogic();

        if (stateTimer == 0)
        {
            FlipToFacePlayer();
            if (!OnGoingPattern)
            {
                if (!phase2)
                {
                    OnGoingPattern = true;
                    StartCoroutine(AttackPattern1());
                }
                else if (!rageMode)
                {
                    OnGoingPattern = true;
                    StartCoroutine(AttackPattern2());
                }
                else
                {
                    OnGoingPattern = true;
                    StartCoroutine(AttackPattern3());
                }
            }
        }
    }

    void UpdatePhase()
    {
        if (!phase2)
        {
            phase2 = (enemyHealth.HP <= enemyHealth.MaxHealth * 0.75);
            if (phase2) { enemyController.InvincibilityCount = 100; }
        }
        if (!rageMode)
        {
            rageMode = (enemyHealth.HP <= enemyHealth.MaxHealth * 0.33);
            if (rageMode) { enemyController.InvincibilityCount = 100; }
        }
    }

    void WakingLogic()
    {
        if (bossAction == "Dormant" && enemyController.PlayerInZone)
        {
            bossAction = "Waking";
            enemyController.animator.Play("GargoyleWake");
            stateTimer = 105;
            enemyController.InvincibilityCount = 105;
        }
        else if (stateTimer > 0) { --stateTimer; }
    }

    void GetPlayerPosition()
    {
        DistanceToPlayerX = transform.position.x - enemyController.playerLocation.position.x;
        playerIsOnLeftSide = (Mathf.Abs(bottomRightPoint.position.x - enemyController.playerLocation.position.x) > Mathf.Abs(topLeftPoint.position.x - enemyController.playerLocation.position.x));
    }

    IEnumerator AttackPattern1()
    {
        Debug.Log("Pattern1");
        int count = 0;
        int prevCount = -1;
        while (count < 3)
        {
            FlipToFacePlayer();
            yield return new WaitForSeconds(0.01f);
            if (prevCount < count)
            {
                Debug.Log("Count is: " + count);
                prevCount = count;
            }

            while (Mathf.Abs(DistanceToPlayerX) > 2 && !enemyController.IsAttacking)
            {
                yield return new WaitForSeconds(0.01f);
                Walk();
                yield return new WaitForSeconds(0.01f);
            }
            if (Mathf.Abs(DistanceToPlayerX) <= 2)
            {
                yield return new WaitForSeconds(0.01f);
                GroundAttack();
                yield return new WaitForSeconds(1.10f);
                ++count;
            }

        }
        FlipToFacePlayer();
        while (enemyController.IsAttacking)
        {
            yield return new WaitForSeconds(0.01f);
        }
        while (Mathf.Abs(DistanceToPlayerX) > 3)
        {
            FlipToFacePlayer();
            Walk();
            yield return new WaitForSeconds(0.15f);
        }
        GroundSlam();
        yield return new WaitWhile(() => enemyController.IsAttacking);
        OnGoingPattern = false;
        stateTimer = 3;
        yield return new WaitForSeconds(0.01f);
    }

    IEnumerator AttackPattern2()
    {
        Debug.Log("Pattern2");
        OnGoingPattern = true;
        TakeOff();
        yield return new WaitForSeconds(1.6f);
        Hover();
        yield return new WaitForSeconds(1.06f);
        while (!rageMode)
        {
            FlipToFacePlayer();
            Hover();
            yield return new WaitForSeconds(2f);
            if (Mathf.Abs(DistanceToPlayerX) > 2)
            {
                StartCoroutine(AirAttack());
                enemyController.IsAttacking = true;
                yield return new WaitWhile(() => enemyController.IsAttacking);
                //yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitWhile(() => enemyController.IsAttacking);
        enemyController.SetVelocity(0, -3.0f);
        Land();

        OnGoingPattern = false;

    }
    IEnumerator AttackPattern3()
    {
        Debug.Log("Pattern3");
        OnGoingPattern = true;
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
            if (!enemyController.IsAttacking)
            {
                Debug.Log("attacking");
                GroundAttack();
                yield return new WaitForSeconds(1f);
                ++count;
            }
            FlipToFacePlayer();
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.01f);
        FlipToFacePlayer();
        while (Mathf.Abs(DistanceToPlayerX) > 3)
        {
            FlipToFacePlayer();
            yield return new WaitForSeconds(0.01f);
            Walk();
            yield return new WaitForSeconds(0.6f);
        }
        GroundSlam();
        OnGoingPattern = false;
    }

    void GroundAttack()
    {
        bossAction = "Ground Attack";
        enemyController.SetVelocity(0, null);
        stateTimer = 70;
        attackManager.StartAttack(0, "GargoyleGroundSwipe");
    }
    void TakeOff()
    {
        bossAction = "Takeoff";
        bossState = "Airborne";
        enemyController.animator.Play("GargoyleLaunch");
        enemyController.SetGravityScale(0);
        stateTimer = 60;
        StartCoroutine(TakeOffHelper());
    }
    IEnumerator TakeOffHelper()
    {
        yield return new WaitForSeconds(0.6f);
        enemyController.AddForce(0, 250.0f);
        yield return new WaitForSeconds(0.01f);
    }

    void Land()
    {
        bossState = "Grounded";
        bossAction = "Land";
        enemyController.SetGravityScale(1);
        enemyController.animator.Play("GargoyleLand");
        stateTimer = 89;
    }
    void FireBreath()
    {
        bossAction = "FireBreath";
        enemyController.animator.Play("GargoyleBreathGround");
        stateTimer = 200;
        attackManager.StartAttack(0, "GargoyleBreathGround");
    }
    void Walk()
    {
        bossAction = "Walk";
        enemyController.SetVelocity(enemyController.MovementSpeed * -enemyController.FacingDirection, null);
        enemyController.animator.Play("GargoyleWalk");
        stateTimer = 0;
    }
    void Hover()
    {
        bossAction = "Hover";
        enemyController.animator.Play("GargoyleFly");
        stateTimer = 48;

        if (playerIsOnLeftSide) { enemyController.SetVelocity(10.0f, 6.0f); }
        else { enemyController.SetVelocity(-10.0f, 6.0f); }
    }
    void GroundSlam()
    {
        stateTimer = 100;
        //enemyController.IsAttacking = true;
        bossAction = "GroundSlam";
        Debug.Log("Groundslam");
        enemyController.animator.Play("GargoyleLaunch2");
        StartCoroutine(SlamHelper());
    }
    IEnumerator SlamHelper()
    {
        yield return new WaitForSeconds(0.40f);
        enemyController.SetVelocity(1.0f * -enemyController.FacingDirection, 10.0f);
        yield return new WaitForSeconds(0.10f);
        enemyController.SetVelocity(1.0f * -enemyController.FacingDirection, 5.0f);
        yield return new WaitForSeconds(0.05f);
        enemyController.SetVelocity(1.0f * -enemyController.FacingDirection, -10.0f);
        yield return new WaitForSeconds(0.15f);
        attackManager.StartAttack(0, "GargoyleGroundSlam");
        enemyController.SetVelocity(0 * -enemyController.FacingDirection, 0);
        yield return new WaitForSeconds(0.01f);

    }
    void Glide()
    {
        bossAction = "Glide";
        enemyController.animator.Play("GargoyleGlideStart");
        //111 frames
        enemyController.animator.Play("GargoyleGlide");

        stateTimer = 54;
        enemyController.animator.Play("GargoyleGlideRecover");
    }
    IEnumerator AirAttack()
    {
        yield return new WaitForSeconds(0.01f);
        bossAction = "Airattack";
        stateTimer = 91;
        attackManager.StartAttack(0, "GargoyleSlashAir");
        yield return new WaitForSeconds(0.2f);
        enemyController.SetVelocity(15.0f * -enemyController.FacingDirection, -12.0f);
        yield return new WaitForSeconds(0.7f);
        enemyController.SetVelocity(10.0f * -enemyController.FacingDirection, 7.0f);
        yield return new WaitForSeconds(0.7f);
    }
}
