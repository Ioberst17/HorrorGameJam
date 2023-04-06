using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink;

public class Morality : MonoBehaviour
{
    public float morality;

    public void AddToMoralityLevel(float toAdd) { morality += toAdd; Debug.LogFormat("Added {0} to morality score", toAdd); }
}
