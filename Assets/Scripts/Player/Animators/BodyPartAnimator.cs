using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Animations;
using UnityEngine;
using static BodyPartAnimator;

public class BodyPartAnimator : MonoBehaviour
{
    [Header("Common Variables Across All Body Part Animators")]
    [SerializeField] public Animator animator;
    protected bool priorityAnimationFlag; // used by child classes as a flag to enable priority animation playing e.g. arms and player shooting

    // used for overriding animations
    public RuntimeAnimatorController runtimeAnimator;
    public AnimatorOverrideController animatorOverrideController;
    public AnimationClipOverrides clipOverrides;
    public string specificFilePathToAnimations;

    // information about animations
    protected AnimationStates animationStateList = new AnimationStates();
    protected AnimatorStateInfo currentAnimation;
    protected AnimatorClipInfo clipInfo;
    [SerializeField] protected AnimationProperties currentAnimationState; // list of animations and their priority levels
    [SerializeField] protected string currentPriorityAnimationClipName; // keeps track of current priority animation being played

    protected string animationToPlay;
    protected bool shouldNewAnimationPlay;

    [Serializable]
    public class AnimationProperties
    {
        [SerializeField] public string animationName; // the animation state name, not the animation clip name
        [SerializeField] public int priorityLevel; // an animation with higher priority will not be played if a higher priority animation is playing
        [SerializeField] public int animationHash; // hashes are needed because Unity animator stores animation states as hashes, so these need to be converted from the strings that are their names

        public AnimationProperties(string animationName, int priorityLevel)
        {
            this.animationName = animationName;
            this.priorityLevel = priorityLevel;
            this.animationHash = Animator.StringToHash(animationName);
        }
    }

    public SimpleSerializableDictionary<string, AnimationProperties> animationStates = new SimpleSerializableDictionary<string, AnimationProperties>(); // animation state names
    public SimpleSerializableDictionary<int, AnimationProperties> animationStatesWithHashAsKey = new SimpleSerializableDictionary<int, AnimationProperties>(); 

    // used to ensure updates are standardized between the two dictionaries
    virtual protected void UpdateAnimationStatePriority(string name, int priorityLevel)
    {
        animationStates[name] = new AnimationProperties(name, priorityLevel);
        animationStatesWithHashAsKey[animationStates[name].animationHash] = new AnimationProperties(name, priorityLevel);
    }


    virtual public void Start() 
    { 
        animator = GetComponentInChildren<Animator>();
        BuildAnimationStateDictionary();

        // update base animation hierarchy
        UpdateAnimationStatePriority("PlayerJump", -1);
        UpdateAnimationStatePriority("PlayerWallLand", -1);
        UpdateAnimationStatePriority("PlayerRun", -2);
        UpdateAnimationStatePriority("PlayerIdle", -3);
        UpdateAnimationStatePriority("PlayerCrouch", -3);
    }

    // used to build a dictionary that lets you reference animation states, note: Unity does not allow you to directly reference an animation by name you need to use a hash
    virtual protected void BuildAnimationStateDictionary()
    {
        // Get the current Animator Controller (runtimeAnimatorController)
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        // Now, populate the dictionaries with the animation state names and priority levels
        animationStates.Clear();
        animationStatesWithHashAsKey.Clear();

        foreach (string state in animationStateList.animationStates)
        {
            var animationProperties = new AnimationProperties(state, 0);
            animationStates.Add(state, animationProperties);
            animationStatesWithHashAsKey.Add(animationProperties.animationHash, animationProperties);
        }
    }


    virtual public void Play(string animationName)
    {
        animationToPlay = CheckIfNewAnimationCanPlay(animationName);

        if (shouldNewAnimationPlay) // if a priority animation is running, do nothing
        {
            animator.Play(animationToPlay); 
        }
        else
        {
            // only do this if not trying to overwrite the same animation
            if(!currentAnimation.IsName(animationName)) { Debug.Log("Did not play while priority animation is running named: " + currentPriorityAnimationClipName); }
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

        if (animationStates[newAnimation].priorityLevel > currentAnimationState.priorityLevel || currentAnimation.normalizedTime >= 1f) { shouldNewAnimationPlay = true; return newAnimation; } // if so, then it can play
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

    // used for any animator overrides done by child classes
    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }

}
