using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPrimaryWeapon : MonoBehaviour
{
    public WeaponDatabase weaponDatabase;
    public PlayerController playerController;
    private GameController gameController;
    private AudioManager audioManager;
    Animator animator;

    //these are all related to attack information
    public int minDamage;
    public int damageToPass;
    public bool isAttacking;

    //List of objects hit by an attack, used to let the player hit multiple things with one swing
    private List<Collider2D> hitList;
    private int hitListLength;

    //These are all the objects used for physical hit detection
    [SerializeField]
    private GameObject attackHori, attackUp, attackDown, chargePunchSprite;
    [SerializeField]
    private Transform AHPoint1, AHPoint2, AUPoint1, AUPoint2, ADPoint1, ADPoint2;
    [SerializeField]
    private float activeFrames, recoveryFrames, startupFrames;

    // need these data points from PlayerController
    private bool canDash;
    private bool isDashing;

    public int attackLagValue, attackLagTimer;

    [SerializeField] ContactFilter2D normalCollisionFilter;

    //abilities
    GroundSlam groundSlam;
    ChargePunch chargePunch;

    void Start()
    {
        GetSupportingReferences();
        InitializeValues();
    }

    void FixedUpdate()
    {
        playerController.isAttacking = isAttacking;
        if (attackLagTimer > 0) { attackLagTimer -= 1; }
        AttackHelper();
    }

    private void GetSupportingReferences()
    {
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        groundSlam = GetComponent<GroundSlam>();
        chargePunch = GetComponent<ChargePunch>();
        playerController = GetComponentInParent<PlayerController>();
        audioManager = FindObjectOfType<AudioManager>();
        animator = GetComponentInParent<Animator>();
        gameController = FindObjectOfType<GameController>();
    }
    private void InitializeValues()
    {
        attackLagTimer = 0;
        minDamage = 10;
        damageToPass = minDamage;
        isAttacking = false;
        AddLayersToCheck();
    }

    private void AddLayersToCheck() 
    {
        hitList = new List<Collider2D>();
        normalCollisionFilter = new ContactFilter2D();
        normalCollisionFilter.SetLayerMask((1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("BreakableEnviro")));
    }

    public void Attack(int attackDirection)
    {
        //Debug.Log("attack called 2");
        if (!isAttacking && !DialogueManager.GetInstance().DialogueIsPlaying)
        {
            if(attackLagTimer == 0)
            {
                isAttacking = true;

                if (attackDirection == 1 && !playerController.IsGrounded && !groundSlam.IsGroundSlam) { groundSlam.Execute(); Debug.Log("Executing groundslam"); }
                else
                {
                    if (chargePunch != null) { chargePunch.Execute(); }
                    else { StartCoroutine(AttackActiveFrames(attackDirection)); }
                }
            }
        }
    }

    public void Release(int attackDirection) { if (chargePunch != null) { chargePunch.Release(attackDirection); } }

    public IEnumerator AttackActiveFrames(int attackDirection) // is called by the trigger event for powerups to countdown how long the power lasts
    {
        if (attackLagTimer == 0)
        {
            animator.Play("PlayerBasicAttack");
            FindObjectOfType<AudioManager>().PlaySFX("PlayerMelee");
            attackLagTimer = attackLagValue;
            if (attackDirection >= 0 && attackDirection <= 2)
            {

                yield return new WaitForSeconds(startupFrames / 60);
                CheckAttackDirection(attackDirection, true);
                yield return new WaitForSeconds(activeFrames / 60); // waits a certain number of seconds
                CheckAttackDirection(attackDirection, false);
                yield return new WaitForSeconds(recoveryFrames / 60);
                isAttacking = false;
            }
            else { isAttacking = false; Debug.LogFormat("Attack direction value should be between 0 and 2, but it is {0}", attackDirection); }
        }
        
    }

    private void CheckAttackDirection(int attackDirection, bool setCondition)
    {
        if (attackDirection == 0) { attackUp.SetActive(setCondition); }
        else if (attackDirection == 1) { attackDown.SetActive(setCondition); }
        else if (attackDirection == 2) { attackHori.SetActive(setCondition); }
    }

    public void AttackHelper()
    {
        //normal attack
        if (attackHori.activeSelf) { CheckForCollisions(AHPoint1.position, AHPoint2.position);}
        //up attack
        if (attackUp.activeSelf) { CheckForCollisions(AUPoint1.position, AUPoint2.position); }
        //down attack
        if (attackDown.activeSelf)
        {
            CheckForCollisions(ADPoint1.position, ADPoint2.position);
            //Debug.Log("Swinging");
            //animator.Play("PlayerAttackDown");
        }
        if (chargePunch.PunchSprite.gameObject.activeSelf) { CheckForCollisions(chargePunch.UpperRightCorner, chargePunch.BottomLeftCorner, true); }
    }
    private void CheckForCollisions(Vector2 point1, Vector2 point2, bool isChargePunch = false)
    {
        hitListLength = Physics2D.OverlapArea(point1, point2, normalCollisionFilter, hitList);
        if (hitListLength > 0)
        {
            if (attackDown.activeSelf) { SetPlayerVelocityForDownSwing(); }
            int i = 0;
            while (i < hitList.Count)
            {
                Debug.LogFormat("has hit something. It's named {0}", hitList[i].gameObject.name);
                if (hitList[i].GetComponent<IDamageable>() != null && hitList[i]) 
                { hitList[i].GetComponent<IDamageable>().Hit(damageToPass, transform.position);
                    if (isChargePunch)
                    {
                        CameraBehavior cameraBehavior = FindObjectOfType<CameraBehavior>();
                        cameraBehavior.ShakeScreen(0.5f);
                        audioManager.PlaySFX("ChargePunchHit");
                        StartCoroutine(gameController.PlayHaptics());
                    }
                }
                i++;
            }
        }
        damageToPass = minDamage;
    }

    private void SetPlayerVelocityForDownSwing()
    {
        playerController.SetVelocity();
        playerController.AddForce(0f, 10f);
    }


}
