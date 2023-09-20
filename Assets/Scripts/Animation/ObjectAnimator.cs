using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Animations;
using UnityEngine;
using static ObjectAnimator;
using static PlayerAnimator;

/// <summary>
/// Provides functions and coroutines for animators to use, as well as tools and data to manage animation state
/// </summary>
public class ObjectAnimator : MonoBehaviour
{
    [Header("Common Variables Across All Body Part Animators")]
    public Animator animator;
    protected bool priorityAnimationFlag; // used by child classes as a flag to enable priority animation playing e.g. arms and player shooting

    // used for overriding animations
    public string specificFilePathToAnimations;

    // information about animations
    protected PlayerAnimationStates playerAnimationStateList = new PlayerAnimationStates();
    protected AnimatorStateInfo currentAnimation;
    protected AnimatorClipInfo clipInfo;
    [SerializeField] protected AnimationProperties currentAnimationState; // list of animations and their priority levels
    [SerializeField] protected string currentPriorityAnimationClipName; // keeps track of current priority animation being played

    protected string animationToPlay;
    private string _currentAnimationName; public string CurrentAnimationName { get {return _currentAnimationName; } set { _currentAnimationName = value; } }
    [SerializeField] protected string previousAnimationName;
    [SerializeField] protected bool shouldNewAnimationPlay;

    public SimpleSerializableDictionary<string, AnimationProperties> animationStates = new SimpleSerializableDictionary<string, AnimationProperties>(); // animation state names
    public SimpleSerializableDictionary<int, AnimationProperties> animationStatesWithHashAsKey = new SimpleSerializableDictionary<int, AnimationProperties>(); 

    // used to ensure updates are standardized between the two dictionaries
    virtual public void UpdateAnimationStatePriority(string name, int priorityLevel, PlayerPart playerPart = PlayerPart.All)
    {
        animationStates[name] = new AnimationProperties(name, priorityLevel);
        animationStatesWithHashAsKey[animationStates[name].animationHash] = new AnimationProperties(name, priorityLevel);
    }


    virtual public void Start() 
    { 
        animator = GetComponentInChildren<Animator>();
        BuildAnimationStateDictionary();
    }

    /// <summary>
    /// Used to build a dictionary that lets you reference animation states, note: Unity does not allow you to directly reference an animation by name you need to use a hash
    /// </summary>
    virtual protected void BuildAnimationStateDictionary() { }


    virtual public void Play(string animationName)
    {
        animationToPlay = CheckIfNewAnimationCanPlay(animationName);

        if (shouldNewAnimationPlay) // if a priority animation is running, do nothing
        {
            if(animationStatesWithHashAsKey[currentAnimation.shortNameHash] != null) { previousAnimationName = animationStatesWithHashAsKey[currentAnimation.shortNameHash].animationName; }
            StartCoroutine(SetAnimState(animator, animationToPlay));
            CurrentAnimationName = animationToPlay;
        }
        else
        {
            // only do this if not trying to overwrite the same animation
            if (!currentAnimation.IsName(animationName)) { }
            if(animationName == "PlayerBasicAttack1") { Debug.Log("can't play combo start; playing " + animationName + " instead");  }
        }
    }

    /// <summary>
    ///  Used to continuously set animation state
    /// </summary>
    /// <param name="targetAnimator"></param>
    /// <param name="targetAnimState"></param>
    /// <returns></returns>
    public IEnumerator SetAnimState(Animator targetAnimator, string targetAnimState)
    {
        float timeOut = 1f;
        //If animator is not playing a state called targetAnimState,
        while (!targetAnimator.GetCurrentAnimatorStateInfo(0).IsName(targetAnimState))
        {
            //Play it
            targetAnimator.Play(targetAnimState);

            timeOut -= Time.deltaTime;
            if (timeOut <= 0)
                yield break;

            yield return 0;
        }
    }

    public void PlayCoroutine(string coroutineName)
    {
        Debug.Log("Attempting to fire coroutine");
        MethodInfo method = GetType().GetMethod(coroutineName);

        if (method != null)
        {
            StartCoroutine((IEnumerator)method.Invoke(this, null)); // all child classes must null currentCoroutine at the end of the function
            Debug.Log("Coroutine fired");
        }
        else
        {
            Debug.LogError("Coroutine not found: " + coroutineName);
        }
    }

    // checks if the input string name for the animation has greater priority than the currently playing animation
    virtual public string CheckIfNewAnimationCanPlay(string newAnimation)
    {
        GetCurrentAnimationInfo();

        if (animationStates[newAnimation].priorityLevel > currentAnimationState.priorityLevel || currentAnimation.normalizedTime >= 0.9f) { shouldNewAnimationPlay = true; return newAnimation; } // if so, then it can play
        else { shouldNewAnimationPlay = false; return currentAnimationState.animationName; }
    }

    // gets the current animation states and updates currentAnimationState with its value
    virtual protected void GetCurrentAnimationInfo() 
    {
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        currentAnimationState = animationStatesWithHashAsKey[currentAnimation.shortNameHash];  
    }

    public bool CheckAnimationState(string animationName, Animator animator)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f) { return true; } // if the current state is that animation, and it's normalized play time is less than 1, it's in play
        return false;
    }

    virtual public void AssignNewAnimations(string objectName)
    {
        string animatorOverridePath = specificFilePathToAnimations + objectName;
        var animatorOverrideController = Resources.Load<AnimatorOverrideController>(animatorOverridePath);
        if (animatorOverrideController != null)
        {
            animator.runtimeAnimatorController = Resources.Load<AnimatorOverrideController>(animatorOverridePath) as RuntimeAnimatorController;
        }
        else { Debug.LogError("No animator override found at " + animatorOverridePath); } // The prefab was not found
    }
}
