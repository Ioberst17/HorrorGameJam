using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for projectiles e.g. EnemyProjectile; it stores data in the form of the Projectile class
/// </summary>
public class ProjectileBase : MonoBehaviour
{
    [Header("Core Data")]
    public Projectile projectile;
    public int referenceID; // must match the refernce ID in the projectile database; used to load in data
    protected int BreakableEnviroLayer;
    public int instanceID;
    public int weaponID;
    public int weaponLevel;
    public int ammoID;
    
    protected Collider2D hitCollider;
    protected bool HitColliderEnabled { get { return hitCollider.enabled; } set { hitCollider.enabled = value; } }
    protected Rigidbody2D rb;
    protected bool RigidbodyEnabled { get { return rb.isKinematic; } set { rb.isKinematic = value; } }
    public Animator animator;

    [Header("Animation Destroy Alternative")]
    [Tooltip("Mark true in Inspector, if an animator OnStateExit behavior is meant to destroy the object vs being destroyed after instantiating a default animation")]
    public bool usesDestroyOnAnimationEnd;
    public string destroyAnimation;
    // additional and standard end animations
    [Tooltip("Use if VFX on hitting enemy differs from standard")]
    public bool usesAdditionalEnemyHitVFX;
    public string pathToAdditionalEnemyHitVFX;

    [Tooltip("Used as an addon to add additional VFX to a projectile hitting")]
    public bool usesAdditionalEndVFX;    
    public string pathToAdditionalEndVFX;
    protected string pathToBasicEndAnimation = "VFXPrefabs/DamageImpact";

    protected void OnDestroy()
    {
        if (!projectile.isFixedDistance)
        {
            EventSystem.current.projectileColliderOn -= ColliderOn;
            EventSystem.current.projectileColliderOff -= ColliderOff;
        }
    }

    virtual protected void Start()
    {
        if (!projectile.isFixedDistance)
        {
            EventSystem.current.projectileColliderOn += ColliderOn;
            EventSystem.current.projectileColliderOff += ColliderOff;
        }
        animator = GetComponentInChildren<Animator>();
        BreakableEnviroLayer = LayerMask.NameToLayer("BreakableEnviro");
        hitCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    virtual protected void UpdateInstanceID() { }

    private void OnCollisionEnter2D(Collision2D col) { if (!projectile.isFixedDistance) { HandleStandardAmmo(col); } }

    // only needed for 'static projectiles' like area of effect casts
    virtual protected void ColliderOn(int instanceID) { }
    virtual protected void ColliderOff(int instanceID) { }

    private void HandleStandardAmmo(Collision2D col)
    {
        HitEnemyObject(col);
        HitEnvironmentObject(col);
    }

    virtual protected void HitEnemyObject(Collision2D col) { }

    protected void HitEnvironmentObject(Collision2D col)
    {
        if (!projectile.playTillEnd)
        {
            if (col.gameObject.CompareTag("Boundary") || col.gameObject.CompareTag("Platform") || 
                    col.gameObject.layer == BreakableEnviroLayer)
            {
                if (projectile.isExplosive) { ExplosionHandler(); }
                else
                {
                    if (col.gameObject.GetComponent<IDamageable>() != null) { col.gameObject.GetComponent<IDamageable>().Hit(); }
                    Instantiate(Resources.Load(pathToBasicEndAnimation), transform.position, Quaternion.identity);
                    Remove();
                }
            }
        }
    }

    protected void ExplosionHandler()
    {
        var projectile = GetComponent<ProjectileBase>().projectile;
        if (GetComponent<Explode>() != null) { GetComponent<Explode>().AmmoExplosion(2f, 10f, projectile.owner); }
        else { Debug.Log("Trying to call Explode function, but does not have Explode.cs script attached"); }
        DisableInteractions();
    }

    protected void DisableInteractions() { HitColliderEnabled = false; GetComponent<SpriteRenderer>().enabled = false; }

    protected void Remove(bool hitEnemy = false)
    {
        // if using add on end VFX
        if(usesAdditionalEndVFX)
        {
            Instantiate(Resources.Load(pathToAdditionalEndVFX), transform.position, Quaternion.identity);
        }

        // if using add on VFX specifically when hitting an enemy
        if (hitEnemy && usesAdditionalEnemyHitVFX) 
        {
            Instantiate(Resources.Load(pathToAdditionalEnemyHitVFX), transform.position, Quaternion.identity); 
        }

        // if destroyed at the end of animation that calls its destruction via keyframe or state-based animation event
        if (usesDestroyOnAnimationEnd)
        {
            animator.Play(destroyAnimation);
        }
        else
        {
            Instantiate(Resources.Load(pathToBasicEndAnimation), transform.position, Quaternion.identity);

            // disable spriteRenderer and use delayed destruction (to ensure collisions happen)
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            Destroy(gameObject, .1f);
        }
    }
}
