using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPrimaryWeapon : MonoBehaviour
{
    public WeaponDatabase weaponDatabase;
    public PlayerController playerController;
    Animator animator;

    //these are all related to attack information
    public int AttackDamage;
    public bool isAttacking;

    //List of objects hit by an attack, used to let the player hit multiple things with one swing
    private List<Collider2D> hitList;
    private int hitListLength;

    //These are all the objects used for physical hit detection
    [SerializeField]
    private GameObject attackHori, attackUp, attackDown;
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

    void Start()
    {
        GetSupportingReferences();
        InitializeValues();
    }

    void FixedUpdate()
    {
        if (attackLagTimer > 0) { attackLagTimer -= 1; }
        AttackHelper();
    }

    private void GetSupportingReferences()
    {
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        groundSlam = GetComponent<GroundSlam>(); 
        playerController = GetComponentInParent<PlayerController>();
        animator = GetComponentInParent<Animator>();
    }
    private void InitializeValues()
    {
        attackLagTimer = 0;
        AttackDamage = 10;
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
        if (!isAttacking && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            isAttacking = true;
            FindObjectOfType<AudioManager>().PlaySFX("PlayerMelee");
            if (attackDirection == 1 && !playerController.isGrounded) { groundSlam.Execute(ADPoint1.position, ADPoint2.position); }
            else { animator.Play("PlayerBasicAttack"); StartCoroutine(AttackActiveFrames(attackDirection)); }
        }
    }

    IEnumerator AttackActiveFrames(int attackDirection) // is called by the trigger event for powerups to countdown how long the power lasts
    {
        if(attackDirection >= 0 && attackDirection <= 2)
        {
            yield return new WaitForSeconds(startupFrames);
            CheckAttackDirection(attackDirection, true);
            yield return new WaitForSeconds(activeFrames); // waits a certain number of seconds
            CheckAttackDirection(attackDirection, false);
            yield return new WaitForSeconds(recoveryFrames);
            isAttacking = false;
        }
        else  {  isAttacking = false; Debug.LogFormat("Attack direction value should be between 0 and 2, but it is {0}", attackDirection); }
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
            SetPlayerVelocityForDownSwing();
            //Debug.Log("Swinging");
            //animator.Play("PlayerAttackDown");
        }
    }
    private void CheckForCollisions(Vector2 point1, Vector2 point2)
    {
        hitListLength = Physics2D.OverlapArea(point1, point2, normalCollisionFilter, hitList);
        if (hitListLength > 0)
        {
            int i = 0;
            while (i < hitList.Count)
            {
                Debug.LogFormat("has hit something. It's named {0}", hitList[i].gameObject.name);
                if (hitList[i].GetComponent<IDamageable>() != null) 
                { hitList[i].GetComponent<IDamageable>().Hit(AttackDamage, transform.position); }
                i++;
            }
        }
    }

    private void SetPlayerVelocityForDownSwing()
    {
        playerController.SetVelocity();
        playerController.AddForce(0f, 10f);
    }


}
