using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// Stores the behavior code of enemies and how they interact with the world / player (e.g. when and how they attack)
/// </summary>
public class EnemyAttackManager : AttackManager
{
    EnemyHealth enemyHealth;
    Animator animator;
    EnemyDataLoader enemyData;
    EnemyController enemyController;
    protected int instanceID;

    override protected void Start() 
    {
        base.Start();
        animator = ComponentFinder.GetComponentInChildrenByNameAndType<Animator>("Animator");
        enemyHealth = GetComponentInParent<EnemyHealth>();
        instanceID = transform.parent.gameObject.GetInstanceID(); // used to validate which specific enemy should take a hit
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void Awake()
    {
        enemyData = GetComponentInParent<EnemyDataLoader>();
        if (enemyData != null) { enemyData.DataLoaded += BuildAttackInfoDictionaries; }
        EventSystem.current.enemyInActiveMeleeAttack += HitBoxOn;
        EventSystem.current.enemyEndActiveMeleeAttack += HitBoxOff;
    }

    private void OnDestroy()
    {
        enemyData.DataLoaded -= BuildAttackInfoDictionaries;
        EventSystem.current.enemyInActiveMeleeAttack -= HitBoxOn;
        EventSystem.current.enemyEndActiveMeleeAttack -= HitBoxOff;
    }

    override protected void AddLayersToCheck()
    {
        normalCollisionFilter.SetLayerMask(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("BreakableEnviro"));
    }

    protected override void BuildAttackInfoDictionaries()
    {
        attacks = enemyData.attacks;
        base.BuildAttackInfoDictionaries();
    }

    /// <summary>
    /// Handles attack based on an animation; typically the entities Attack state is toggled on the animation state that is being called 
    /// with scripts
    /// </summary>
    /// <param name="attackDirection"></param>
    /// <param name="animationToPlay"></param>
    override public void StartAttack(int attackDirection, string animationToPlay)
    {
        if(AttackLagTimer <= 0)
        {
            LoadHitBox(animationToPlay);
            CacheMostRecentAttack(animationToPlay);

            // load animation
            animator.Play(animationToPlay);

            // to-do: load sound from cached attack
            // e.g.FindObjectOfType<AudioManager>().PlaySFX(MostRecentAttack.soundToPlay);

            AttackLagTimer = MostRecentAttack.attackBuffer;
        }
    }

    override public void StartChargeAttack(int attackDirection, string animationToPlay, string successAnimationToPlay, IEnumerator AdditionalEndConditions = null)
    {
        if (AttackLagTimer <= 0)
        {
            // update state
            enemyController.IsChargingAttack = true;

            // play animation
            animator.Play(animationToPlay + "Charge");

            // load the hitbox of the attack
            LoadHitBox(animationToPlay);

            // also cache it
            CacheMostRecentAttack(animationToPlay);

            // attempt attack release
            StartCoroutine(ReleaseChargeAttack(MostRecentAttack, animationToPlay, successAnimationToPlay, AdditionalEndConditions));
        }
    }

    override protected IEnumerator ReleaseChargeAttack(Attack currentAttack, string animationToPlay, string successAnimationToPlay, IEnumerator AdditionalReleaseConditions = null)
    {
        // wait for the startup time
        yield return new WaitForSeconds(currentAttack.startupTime);

        // if was attacked then follow interrupt logic
        if (enemyHealth.DamageInterrupt)
        {
            enemyHealth.DamageInterrupt = false;
            enemyController.IsChargingAttack = false;
            EventSystem.current.EnemyEndActiveMeleeTrigger(gameObject.GetInstanceID());
        }
        // else, run attack
        else
        {
            enemyController.IsChargingAttack = false;
            animator.Play(animationToPlay);
            enemyController.AddForce(currentAttack.launchForceX * enemyController.FacingDirection,
                                    currentAttack.launchForceY);
            yield return new WaitForSeconds(currentAttack.activeTime);
            yield return AdditionalReleaseConditions;
            HitBoxOff();
            yield return new WaitForSeconds(currentAttack.recoveryTime);
            animator.Play(successAnimationToPlay, 0, 0);
            AttackLagTimer = MostRecentAttack.attackBuffer;
        }
    }

    protected void HitBoxOn(int instanceID) { if (this.instanceID == instanceID) { HitBox.SetActive(true); enemyController.IsAttacking = true; } }
    protected void HitBoxOff(int instanceID) { if (this.instanceID == instanceID) { HitBox.SetActive(false); enemyController.IsAttacking = false; enemyController.IsChargingAttack = false; } }
}

