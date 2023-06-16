using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BodyPartAnimator : MonoBehaviour
{
    [SerializeField] public Animator animator;
    private AnimatorStateInfo stateInfo;

    public List<string> priorityAnimationStates; // animation state names
    public List<int> priorityAnimationHashes; // hashes are needed because Unity animator stores states as hashes, so these need to be converted from strings

    virtual public void Start()
    {
        animator = GetComponentInChildren<Animator>();
        TurnPriorityStateNamesIntoHashes();
    }

    virtual public void TurnPriorityStateNamesIntoHashes() // needed because Unity's animator does not store animator states as strings, but rather as hashes (ints)
    {
        foreach (string clipName in priorityAnimationStates) 
        {
            priorityAnimationHashes.Add(Animator.StringToHash(clipName));
        }
    }

    virtual public void Play(string animationName)
    {
        
        if (PriorityAnimationIsPlaying()) // if a priority animation is running, do nothing
        {
            // do nothing
            Debug.Log("Did not play while priority animation is running");
        }
        else
        {
            animator.Play(animationName);
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

    private bool PriorityAnimationIsPlaying()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        foreach (int animationStateHash in priorityAnimationHashes)
        {
            if (stateInfo.shortNameHash == animationStateHash) // compare animation hash "an int" against the 'hash'  of the string input
            { return true; }
        }
        return false;
    }

    public bool CheckAnimationState(string animationName, Animator animator)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f) { return true; } // if the current state is that animation, and it's normalized play time is less than 1, it's in play
        return false;
    }
}
