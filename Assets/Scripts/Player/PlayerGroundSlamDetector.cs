using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class PlayerGroundSlamDetector : MonoBehaviour // meant to handle hit detection and pass it back up to GroundSlam
{
    private GroundSlam groundSlam;
    private PlayerController playerController;

    public SpriteRenderer SpriteRenderer;
    private BoxCollider2D detectionCollider;
    [SerializeField] Collider2D[] nonDamagableObjects;

    [SerializeField] Collider2D[] damagableObjects;

    [SerializeField] ContactFilter2D damagableFilter, nonDamagableFilter;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        groundSlam = GetComponentInParent<GroundSlam>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        detectionCollider = GetComponent<BoxCollider2D>();

        // Set layers to check
        damagableFilter = new ContactFilter2D();
        nonDamagableFilter = new ContactFilter2D();
        damagableFilter.SetLayerMask((1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("BreakableEnviro")));
        nonDamagableFilter.SetLayerMask(1 << LayerMask.NameToLayer("Environment"));
    }
    private void Update()
    {
        if (groundSlam.IsGroundSlam)
        {
            CheckForNonDamagable(CheckForDamagable()); // check to see if hit something that takes damage, if it hasn't check for ground 'nondamageable')
        }
    }

    bool CheckForDamagable() 
    {
        // Detect Enemies: objects with Rigidbody2D and BoxCollider2D
        damagableObjects = Array.Empty<Collider2D>();
        damagableObjects = Physics2D.OverlapBoxAll(SpriteRenderer.bounds.center,
                                                                  SpriteRenderer.bounds.size,
                                                                  0f,
                                                                  damagableFilter.layerMask);

        foreach (Collider2D hit in damagableObjects)
        {
            Debug.Log("Damagable hits are not null");
            // if has rigidbody + a box collider
            if (hit.gameObject.GetComponent<Rigidbody2D>() != null) 
            {
                Debug.Log("Hit something");
                // if it has a isDamagable interface implemented 
                if (ComponentFinder.CheckForComponentInObjectHierarchy<IDamageable>(hit.gameObject))
                {
                    Debug.Log("Found a damagable hit object named: " + hit.gameObject.name);
                    groundSlam.IsGroundSlam = false; // called early to prevent additional calls
                    groundSlam.Finished(hit.gameObject, GroundSlam.TypeOfHit.Damagable);
                    return true;
                }
            }
        }
        return false;
    }

    void CheckForNonDamagable(bool alreadyHitSomething)
    {
        if (!alreadyHitSomething)
        {
            // Detect non-damagables i.e. the ground: objects with Collider2D (non-trigger)
            nonDamagableObjects = Array.Empty<Collider2D>();
            nonDamagableObjects = Physics2D.OverlapBoxAll(SpriteRenderer.bounds.center,
                                                                  SpriteRenderer.bounds.size,
                                                                  0f,
                                                                  nonDamagableFilter.layerMask);

            foreach (Collider2D hit in nonDamagableObjects)
            {
                if (hit.GetComponent<Rigidbody2D>() == null)
                {
                    groundSlam.Finished(hit.gameObject, GroundSlam.TypeOfHit.NonDamagable);
                    return;
                }
            }
        }
    }
}
