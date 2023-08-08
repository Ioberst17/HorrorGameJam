using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static BodyPartAnimator;

public class PlayerPrimaryWeapon : MonoBehaviour
{
    public WeaponDatabase weaponDatabase;
    public PlayerController playerController;
    private GameController gameController;
    private ComboSystem comboSystem;
    private AudioManager audioManager;
    PlayerAnimator animator;

    //these are all related to attack information
    public int minDamage;
    public int damageToPass;
    [SerializeField] private bool _isAttacking; public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }

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

    public class HitBox
    {
        [SerializeField] public Vector3 point1; // upper left corner of attack
        [SerializeField] public Vector3 point2; // bottom right corner of attack

        public HitBox(Vector3 point1, Vector3 point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }
    }

    public SimpleSerializableDictionary<string, HitBox> horizontalAttackDimensions = new SimpleSerializableDictionary<string, HitBox>();
    public SimpleSerializableDictionary<string, HitBox> upwardAttackDimensions = new SimpleSerializableDictionary<string, HitBox>();
    public SimpleSerializableDictionary<string, HitBox> downwardAttackDimensions = new SimpleSerializableDictionary<string, HitBox>();

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
        playerController.isAttacking = IsAttacking;
        if (attackLagTimer > 0) { attackLagTimer -= 1; }
        AttackHelper();
    }

    private void GetSupportingReferences()
    {
        weaponDatabase = GameObject.Find("WeaponDatabase").GetComponent<WeaponDatabase>();
        comboSystem = GetComponent<ComboSystem>();
        groundSlam = GetComponent<GroundSlam>();
        chargePunch = GetComponent<ChargePunch>();
        playerController = GetComponentInParent<PlayerController>();
        audioManager = FindObjectOfType<AudioManager>();
        animator = ComponentFinder.GetComponentInChildrenByNameAndType<PlayerAnimator>("Animator", transform.parent.gameObject);
        gameController = FindObjectOfType<GameController>();
    }
    private void InitializeValues()
    {
        attackLagTimer = 0;
        minDamage = 10;
        damageToPass = minDamage;
        IsAttacking = false;
        AddLayersToCheck();
        InitializeAttackHitBoxDimensions();
    }

    private void AddLayersToCheck() 
    {
        hitList = new List<Collider2D>();
        normalCollisionFilter = new ContactFilter2D();
        normalCollisionFilter.SetLayerMask((1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("BreakableEnviro")));
    }

    void InitializeAttackHitBoxDimensions()
    {
        // horizontal
        horizontalAttackDimensions.Add("PlayerBasicAttack1", new HitBox(new Vector3(.29f, 1.02f), new Vector3(2.62f, -.348f)));
        horizontalAttackDimensions.Add("PlayerBasicAttack2", new HitBox(new Vector3(.29f, 1.02f), new Vector3(2.62f, -.348f)));
        horizontalAttackDimensions.Add("PlayerBasicAttack3", new HitBox(new Vector3(.85f, 1.65f), new Vector3(2.62f, -1.91f)));
        horizontalAttackDimensions.Add("PlayerNeutralAir", new HitBox(new Vector3(-0.6f, .5f), new Vector3(1.41f, -1.218f)));
        horizontalAttackDimensions.Add("PlayerNeutralCrouchAttack", new HitBox(new Vector3(.596f, -.441f), new Vector3(2.827f, -2.302f)));
        
        // upward
        
        
        // downward
        
    }

    public void Attack(int attackDirection, GameController.ButtonState state)
    {
        if (!IsAttacking && !DialogueManager.GetInstance().DialogueIsPlaying)
        {
            if(attackLagTimer == 0)
            {
                IsAttacking = true;

                // AIR ATTACKS
                if (!playerController.IsGrounded) 
                {
                    if (attackDirection == 1 && !groundSlam.IsGroundSlam) { groundSlam.Execute(); }
                    else if (attackDirection == 2) { StartCoroutine(AttackActiveFrames(attackDirection, "PlayerNeutralAir")); }
                }
                
                // GROUND ATTACKS
                else
                {
                    if (playerController.IsRunning && (playerController.ControlMomentum > 5 || playerController.ControlMomentum < -5)) { StartCoroutine(AttackActiveFrames(attackDirection, "PlayerSideKnee")); }
                    else if (chargePunch != null && state == GameController.ButtonState.Held) { chargePunch.Execute(); }
                    else if (playerController.IsCrouching) { StartCoroutine(AttackActiveFrames(attackDirection, "PlayerNeutralCrouchAttack")); }
                    else { comboSystem.PerformCombo(attackDirection); }
                }
            }
        }
    }

    public void Release(int attackDirection) { Debug.Log("ChargePunchRelease"); if (chargePunch != null) { chargePunch.Release(attackDirection); } }

    public IEnumerator AttackActiveFrames(int attackDirection, string animationToPlay) // is called by the trigger event for attack to countdown how long the power lasts
    {
        if (attackLagTimer == 0)
        {
            UpdateHitBox(attackDirection, animationToPlay);
            animator.Play(animationToPlay);
            FindObjectOfType<AudioManager>().PlaySFX("PlayerMelee");
            attackLagTimer = attackLagValue;
            if (attackDirection >= 0 && attackDirection <= 2)
            {

                yield return new WaitForSeconds(startupFrames / 60);
                CheckAttackDirection(attackDirection, true);
                yield return new WaitForSeconds(activeFrames / 60); // waits a certain number of seconds
                CheckAttackDirection(attackDirection, false);
                yield return new WaitForSeconds(recoveryFrames / 60);
                IsAttacking = false;
            }
            else { IsAttacking = false; Debug.LogFormat("Attack direction value should be between 0 and 2, but it is {0}", attackDirection); }
        }
        
    }

    private void UpdateHitBox(int attackDirection, string animationToPlay)
    {
        if(attackDirection == 0) { UpdateHitBoxHelper(upwardAttackDimensions, animationToPlay, AUPoint1, AUPoint2); }
        else if(attackDirection == 1) { UpdateHitBoxHelper(downwardAttackDimensions, animationToPlay, ADPoint1, ADPoint2); }
        else if(attackDirection == 2) { UpdateHitBoxHelper(horizontalAttackDimensions, animationToPlay, AHPoint1, AHPoint2); }

    }

    private void UpdateHitBoxHelper(SimpleSerializableDictionary<string, HitBox> attackDimensions, string animationToPlay, Transform point1, Transform point2) 
    {
        if (attackDimensions.TryGetValue(animationToPlay, out HitBox value))
        {
            point1.transform.localPosition = value.point1;
            point2.transform.localPosition = value.point2;
        }
        else
        {
            Debug.Log("Key '" + animationToPlay + "' not found in the dictionary: " + attackDimensions);
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
        if (attackDown.activeSelf) { CheckForCollisions(ADPoint1.position, ADPoint2.position); }
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
