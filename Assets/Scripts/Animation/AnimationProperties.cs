using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
