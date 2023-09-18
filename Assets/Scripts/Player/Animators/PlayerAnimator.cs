using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Rendering;
using UnityEngine;
using static ComponentFinder;
using static PlayerAnimator;


/// <summary>
/// Interface into the players different animation parts: Base, RightArm, LeftArm, Weapon
/// </summary>
public class PlayerAnimator : PlayerBaseAnimator 
{
    [Header("PlayerAnimator Interfaces Into Player Parts")]
    [SerializeField] Transform body, rightArm, leftArm, weapon;
    [SerializeField] Animator baseAnimator, rightArmAnimator, leftArmAnimator, weaponAnimator;
    [SerializeField] float maxIdleNormalizedTime, baseIdleNormalizedTime, rightArmIdleNormalizedTime, leftArmIdleNormalizedTime, weaponIdleNormalizedTime;
    [SerializeField] PlayerBodyAnimator baseAnimatorScript;
    [SerializeField] PlayerRightArmAnimator rightArmScript;
    [SerializeField] PlayerLeftArmAnimator leftArmScript;
    [SerializeField] PlayerWeaponAnimator weaponAnimatorScript;
    public enum PlayerPart { All, Body, RightArm, LeftArm, Weapon }

    public float synchronizationThreshold = 0.05f; // the threshold of difference needed before idle animations need to be synced

    [System.Serializable]
    public class AnimatorAndScript<T> where T : ObjectAnimator
    {
        public Animator anim;
        [SerializeField]
        public T script;

        public AnimatorAndScript(Animator anim, T script)
        {
            this.anim = anim;
            this.script = script;
        }
    }

    // dictionary that maps a enum to a relevant animator and a script (for calling more specific animations)
    [SerializeField] private SimpleSerializableDictionary<PlayerPart, AnimatorAndScript<ObjectAnimator>> EnumToAnimatorMap = new SimpleSerializableDictionary<PlayerPart, AnimatorAndScript<ObjectAnimator>>();

    [SerializeField] private SpriteRenderer[] spriteRenderers;

    // Get all references
    private void Awake()
    {
        body = GetComponentInChildrenByNameAndType<Transform>("Base", gameObject);
        baseAnimator = GetComponentInChildrenByNameAndType<Animator>("SpriteAndAnimations", body.gameObject);
        baseAnimatorScript = GetComponentInChildrenByNameAndType<PlayerBodyAnimator>("SpriteAndAnimations", body.gameObject);
        EnumToAnimatorMap.Add(PlayerPart.Body, new AnimatorAndScript<ObjectAnimator>(baseAnimator, baseAnimatorScript)); 

        rightArm = GetComponentInChildrenByNameAndType<Transform>("RightArm", gameObject);
        rightArmAnimator = GetComponentInChildrenByNameAndType<Animator>("SpriteAndAnimations", rightArm.gameObject);
        rightArmScript = GetComponentInChildrenByNameAndType<PlayerRightArmAnimator>("SpriteAndAnimations", rightArm.gameObject);
        EnumToAnimatorMap.Add(PlayerPart.RightArm, new AnimatorAndScript<ObjectAnimator>(rightArmAnimator, rightArmScript));

        leftArm = GetComponentInChildrenByNameAndType<Transform>("LeftArm", gameObject);
        leftArmAnimator = GetComponentInChildrenByNameAndType<Animator>("SpriteAndAnimations", leftArm.gameObject);
        leftArmScript = GetComponentInChildrenByNameAndType<PlayerLeftArmAnimator>("SpriteAndAnimations", leftArm.gameObject);
        EnumToAnimatorMap.Add(PlayerPart.LeftArm, new AnimatorAndScript<ObjectAnimator>(leftArmAnimator, leftArmScript));         
        
        weapon = GetComponentInChildrenByNameAndType<Transform>("Weapon", gameObject);
        weaponAnimator = GetComponentInChildrenByNameAndType<Animator>("SpriteAndAnimations", weapon.gameObject);
        weaponAnimatorScript = GetComponentInChildrenByNameAndType<PlayerWeaponAnimator>("SpriteAndAnimations", weapon.gameObject);
        EnumToAnimatorMap.Add(PlayerPart.Weapon, new AnimatorAndScript<ObjectAnimator>(weaponAnimator, weaponAnimatorScript)); 

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        CheckIfIdleAnimationIsSynced();
    }

    // play an animation
    public void Play(string animationName, PlayerPart playerPart = PlayerPart.All,
        bool playBase = true, bool playRightArm = true, bool playLeftArm = true, bool playWeapon = true) // optional: used to exclude by turning to false
    {
        if (playerPart == PlayerPart.All)
        {
            if (playBase) { if (animationName == "PlayerBasicAttack1") { Debug.Log("Attempting to play PlayerBasicAttack1"); } baseAnimatorScript.Play(animationName); }
            if (playRightArm) { rightArmScript.Play(animationName); }
            if (playLeftArm) { leftArmScript.Play(animationName); }
            if (playWeapon) { weaponAnimatorScript.Play(animationName);  }
        }
        else 
        { 
            EnumToAnimatorMap[playerPart].anim.Play(animationName);
        }
    }    
    
    // invokes a function from an animator; calls all unless a specific animator is needed, in which case it gets it from the enum
    public void PlayFunction(string animationName, PlayerPart playerPartPassedIn = PlayerPart.All)
    {
        if (playerPartPassedIn == PlayerPart.All)
        {
            foreach (PlayerPart playerPart in Enum.GetValues(typeof(PlayerPart)))
            {
                if(playerPart != PlayerPart.All)
                {
                    EnumToAnimatorMap[playerPart].script.Invoke(animationName, 0);
                }
            }
        }
        else 
        {
            EnumToAnimatorMap[playerPartPassedIn].script.Invoke(animationName, 0);
        }
    }

    // play a coroutine that triggers a sequence of animations
    virtual public void PlayCoroutine(string animationName, PlayerPart playerPart = PlayerPart.All)
    {
        if (playerPart == PlayerPart.All)
        {
            baseAnimator.Play(animationName);
            rightArmAnimator.Play(animationName);
            leftArmAnimator.Play(animationName);
            weaponAnimator.Play(animationName);
        }
        else
        {
            EnumToAnimatorMap[playerPart].script.PlayCoroutine(animationName);
        }
    }

    override public void UpdateAnimationStatePriority(string name, int priorityLevel, PlayerPart playerPart = PlayerPart.All)
    {
        if (playerPart == PlayerPart.All)
        {
            baseAnimatorScript.UpdateAnimationStatePriority(name, priorityLevel);
            rightArmScript.UpdateAnimationStatePriority(name, priorityLevel);
            leftArmScript.UpdateAnimationStatePriority(name, priorityLevel);
            weaponAnimatorScript.UpdateAnimationStatePriority(name, priorityLevel);
        }
        else
        {

        }
    }

    public bool CheckIfAnimationIsPlaying(string animationName, PlayerPart playerPart = PlayerPart.All)
    {
        if (playerPart == PlayerPart.All)
        {
            return CheckAnimationState(animationName, baseAnimator);
            //return CheckAnimationState(animationName, rightArmAnimator); // find a way to better implement reaching any animator
            //return CheckAnimationState(animationName, leftArmAnimator);
        }
        else
        {
            return CheckAnimationState(animationName, EnumToAnimatorMap[playerPart].anim);
        }
    }

    public void SpriteEnabled(bool state)
    {
        foreach(SpriteRenderer sprite in spriteRenderers) { sprite.enabled = state; }
    }

    void CheckIfIdleAnimationIsSynced()
    {
        //if idle animations are playing
        if (IsIdleAnimationPlaying())
        {
            // get there normalized times
            baseIdleNormalizedTime = baseAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            rightArmIdleNormalizedTime = rightArmAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            leftArmIdleNormalizedTime = leftArmAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            weaponIdleNormalizedTime = weaponAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            // synchronize them if they exceed the need to sync threshold
            if (Mathf.Abs(baseIdleNormalizedTime - rightArmIdleNormalizedTime) > synchronizationThreshold ||
                Mathf.Abs(baseIdleNormalizedTime - leftArmIdleNormalizedTime) > synchronizationThreshold ||
                Mathf.Abs(rightArmIdleNormalizedTime - leftArmIdleNormalizedTime) > synchronizationThreshold)
            {
                maxIdleNormalizedTime = Mathf.Max(baseIdleNormalizedTime, rightArmIdleNormalizedTime, leftArmIdleNormalizedTime);

                baseAnimator.Play("PlayerIdle", 0, maxIdleNormalizedTime);
                rightArmAnimator.Play("PlayerIdle", 0, maxIdleNormalizedTime);
                leftArmAnimator.Play("PlayerIdle", 0, maxIdleNormalizedTime);
                weaponAnimator.Play("PlayerIdle", 0, maxIdleNormalizedTime);
            }
        }
    }

    // check if idle animation is playing in all animators
    private bool IsIdleAnimationPlaying()
    {
        return baseAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle") &&
               rightArmAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle") &&
               leftArmAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle") &&
               weaponAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle");
    }
}
