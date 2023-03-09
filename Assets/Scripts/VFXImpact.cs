using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class VFXImpact : MonoBehaviour
{
    private float delay = 0.3f;

    // Update is called once per frame
    void Start() { Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay); }
}
