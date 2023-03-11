using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class StatusEffect : MonoBehaviour
{
    public float totalTimePassed = 0.0f;
    public float intervalToApply = 1.0f;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        spriteRenderer.enabled = false;
    }
}
