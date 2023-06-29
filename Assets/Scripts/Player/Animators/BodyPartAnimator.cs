using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BodyPartAnimator : MonoBehaviour
{
    [Header("Common Variables Across All Body Part Animators")]
    [SerializeField] public Animator animator;

    // used for overriding animations
    public RuntimeAnimatorController runtimeAnimator;
    public AnimatorOverrideController animatorOverrideController;
    public AnimationClipOverrides clipOverrides;
    public string specificFilePathToAnimations;

    // information about animations
    private AnimatorStateInfo stateInfo;
    private AnimatorClipInfo clipInfo;
    [SerializeField] private AnimationProperties currentPriorityAnimationState; // list of animations and their priority levels
    [SerializeField] string currentPriorityAnimationClipName; // keeps track of current priority animation being played

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

    public SimpleSerializableDictionary<string, AnimationProperties> priorityAnimationStates = new SimpleSerializableDictionary<string, AnimationProperties>(); // animation state names

    virtual public void Start() 
    { 
        animator = GetComponentInChildren<Animator>();
    }

    virtual public void Play(string animationName)
    {
        if (CheckIfNewAnimationCanPlay(animationName)) // if a priority animation is running, do nothing
        {
            animator.Play(animationName); 
        }
        else
        {
            Debug.Log("Did not play while priority animation is running named: " + currentPriorityAnimationClipName);
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
    private bool CheckIfNewAnimationCanPlay(string newAnimation)
    {

        if (CheckCurrentAnimationState()) // if there is a current animation with priority
        {
            // keep evaluating
        }
        else { return true; } // otherwise, new animation should play 

        if (priorityAnimationStates.ContainsKey(newAnimation)) // if the new animation also has priority
        {
            // check to see if it has a higher priority level or whether the current animation has ended
            clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
            if (priorityAnimationStates[newAnimation].priorityLevel > currentPriorityAnimationState.priorityLevel || stateInfo.normalizedTime >= 1f) { Debug.Log("Existing animation can play"); return true; } // if so, then it can play
            else return false;
        }
        else { return false; } // else the existing animation takes priority
    }

    // gets the current animation states and updates currentAnimationState with its value
    bool CheckCurrentAnimationState()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0); // currently playing animation

        foreach (string animation in priorityAnimationStates.GetKeys()) 
        {
            if (stateInfo.IsName(animation)) // if a match exists
            {
                currentPriorityAnimationClipName = animation;
                if(stateInfo.normalizedTime < 1f) // and the clip hasn't finished
                {
                    currentPriorityAnimationState = priorityAnimationStates[animation];
                    return true;
                }
            }
        }
        currentPriorityAnimationState = null;
        return false;
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
