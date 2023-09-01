using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static ObjectAnimator;

public class PlayerAttackManager : AttackManager
{
    PlayerController playerController;
    PlayerAnimator playerAnimator;
    GameController gameController;
    ComboSystem comboSystem;
    AudioManager audioManager;

    //these are all related to attack information
    int minDamage;
    int damageToPass;

    //abilities
    GroundSlam groundSlam;
    ChargePunch chargePunch;

    override protected void Start()
    {
        base.Start();
        GetSupportingReferences();
        BuildAttackInfoDictionaries();
        EventSystem.current.inActiveMeleeAttack += HitBoxOn;
        EventSystem.current.endActiveMeleeAttack += HitBoxOff;
    }

    private void OnDestroy()
    {
        EventSystem.current.inActiveMeleeAttack -= HitBoxOn;
        EventSystem.current.endActiveMeleeAttack -= HitBoxOff;
    }

    private void GetSupportingReferences()
    {
        playerAnimator = FindObjectOfType<PlayerAnimator>();
        comboSystem = GetComponent<ComboSystem>();
        groundSlam = GetComponent<GroundSlam>();
        chargePunch = GetComponent<ChargePunch>();
        playerController = GetComponentInParent<PlayerController>();
        audioManager = FindObjectOfType<AudioManager>();
        gameController = FindObjectOfType<GameController>();
    }

    override protected void AddLayersToCheck() 
    {
        normalCollisionFilter.SetLayerMask(1 << LayerMask.NameToLayer("Enemy") | 
                                           1 << LayerMask.NameToLayer("BreakableEnviro") | 
                                           1 << LayerMask.NameToLayer("Environment"));
    }

    protected override void BuildAttackInfoDictionaries()
    {
        attacks = FindObjectOfType<PlayerAttackDatabase>().data.entries.ToList();
        base.BuildAttackInfoDictionaries();
    }

    public void Attack(int attackDirection, GameController.ButtonState state)
    {
        if (!DialogueManager.GetInstance().DialogueIsPlaying)
        {
            if(AttackLagTimer == 0)
            {
                // AIR ATTACKS
                if (!playerController.IsGrounded) 
                {
                    if (attackDirection == 1 && (!groundSlam.IsGroundSlam || !groundSlam.IsPreDropGroundSlam)) 
                    { 
                        groundSlam.Execute();
                        StartCoroutine(groundSlam.GroundSlamStartHelper(attackDirection));
                    }
                    else if (attackDirection == 2) { StartAttack(attackDirection, "PlayerNeutralAir"); }
                }
                
                // GROUND ATTACKS
                else
                {
                    if (playerController.IsRunning && (playerController.ControlMomentum > 5 || playerController.ControlMomentum < -5)) { StartAttack(attackDirection, "PlayerSideKnee"); }
                    else if (chargePunch != null && state == GameController.ButtonState.Held) { chargePunch.Execute(); }
                    else if (playerController.IsCrouching) { StartAttack(attackDirection, "PlayerNeutralCrouchAttack"); }
                    else { comboSystem.PerformCombo(attackDirection); }
                }
            }
            else { comboSystem.PerformCombo(attackDirection); }
        }
    }

    override public void StartAttack(int attackDirection,  string animationToPlay)
    {
        LoadHitBox(animationToPlay);
        CacheMostRecentAttack(animationToPlay);

        // load animation
        playerAnimator.Play(animationToPlay); 

        // to-do: load sound from cached attack
        // e.g.FindObjectOfType<AudioManager>().PlaySFX(MostRecentAttack.soundToPlay);

        AttackLagTimer = MostRecentAttack.attackBuffer;
    }

    /// <summary>
    /// Used for when the attack button is released; in the case of the player, this is for the release of charge punch
    /// </summary>
    /// <param name="attackDirection"></param>
    public void Release(int attackDirection) 
    { 
        Debug.Log("ChargePunchRelease"); 
        if (chargePunch != null) { chargePunch.Release(attackDirection); } 
    }

    override protected void LoadHitBox(string animationToPlay) 
    {
        if (animationToPlay == "PlayerChargePunch")
        {
            hitBoxPoint1.transform.localPosition = chargePunch.UpperRightCorner;
            hitBoxPoint2.transform.localPosition = chargePunch.BottomLeftCorner;
        }
        else { base.LoadHitBox(animationToPlay); } 
    }

    override protected void HitBoxOn() { HitBox.SetActive(true);  playerController.IsAttacking = true;  }

    override protected void HitBoxOff() { HitBox.SetActive(false); /*playerController.IsAttacking = false;*/  }

    override protected void CheckForCollisions(Vector2 point1, Vector2 point2)
    {
        base.CheckForCollisions(point1, point2);
        damageToPass = minDamage;
    }

    /// <summary>
    /// An override for abilities to run any additional checks after hit on damagable objects
    /// </summary>
    /// <param name="hitObject"></param>
    override protected void AdditionalAbilitySpecificChecksDamagable(Collider2D hitObject)
    {
        if (chargePunch.IsCharging)
        {
            CameraBehavior cameraBehavior = FindObjectOfType<CameraBehavior>();
            cameraBehavior.ShakeScreen(0.5f);
            audioManager.PlaySFX("ChargePunchHit");
            StartCoroutine(gameController.PlayHaptics());
        }
        if (groundSlam.IsGroundSlam)
        {
            Debug.Log("Found a damagable hit object named: " + hitObject.gameObject.name);
            groundSlam.Finished(hitObject.gameObject, GroundSlam.TypeOfHit.Damagable);
        }
    }
    /// <summary>
    /// An override for abilities to run any additional checks after hitting non-damagable objects
    /// </summary>
    /// <param name="hitObject"></param>
    override protected void AdditionalAbilitySpecificChecksNonDamagable(Collider2D hitObject)
    {
        if (groundSlam.IsGroundSlam)
        {
            groundSlam.Finished(hitObject.gameObject, GroundSlam.TypeOfHit.NonDamagable);
        }
    }
}
