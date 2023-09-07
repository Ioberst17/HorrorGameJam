using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Collider2D))] // for detection
public class Shield : MonoBehaviour
{
    // for visualization and hit detection
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D shieldCollider; // used when shield is active
    [SerializeField] Collider2D bodyCollider; // used when shield is not active

    // internal variables to track
    public bool shieldOn;
    public bool checkStatus;
    public string shieldedObject;
    protected Attack lastReceivedAttack;
    protected int damageToPass;
    
    // used for angle calculation and understanding where a shield is hit
    [SerializeField] string shieldDirectionRelativeToAttack;
    [SerializeField] string shieldPositionRelativeToAttack;
    [SerializeField] float collisionAngle;
    public bool hitWithinActiveShieldZone;

    // used to filter and store collisions
    // IMPORTANT FILTER: SPECIFIC SHIELD TO ONLY REGISTER SPECIFIC LAYERS E.G. Player shield will check on Enemy layer
    public ContactFilter2D attackerFilter;
    public Collider2D[] attackingObjects = new Collider2D[10];

    [SerializeField] public List<ShieldZone> shieldZones = new List<ShieldZone>();
    ShieldZone hitShield;

    // optional ability to attach and use
    public Parry parry;

    // VFX when shield is hit
    string VFXPath;
    int objectHitVFXCooldown = 0;

    virtual protected void Start()
    {
        TryGetComponent(out parry);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shieldCollider = GetComponent<Collider2D>();
        bodyCollider = transform.parent.GetComponent<Collider2D>();
        ShieldOn(false);
        CheckObjectType();
    }

    virtual protected void Update() 
    {
        if(shieldCollider.enabled) { GetOverlap(shieldCollider); }
        else { GetOverlap(bodyCollider); }

        DamageHandler(attackingObjects);
    }

    void GetOverlap(Collider2D colliderToUse)
    {
        attackingObjects = Physics2D.OverlapCircleAll(colliderToUse.bounds.center,
                                                      colliderToUse.bounds.extents.x,
                                                      attackerFilter.layerMask);
    }

    public void FixedUpdate()
    {
        if (objectHitVFXCooldown > 0)
        {
            objectHitVFXCooldown--;
        }
    }

    /// <summary>
    /// Main logic for the shield to determine how to treat damage for collisions
    /// </summary>
    /// <param name="overlaps"></param>
    protected void DamageHandler(Collider2D[] overlaps)
    {
        if (overlaps != null)
        {
            foreach (Collider2D overlap in overlaps)
            {
                checkStatus = CheckIfHasHitDamagingObject(overlap);

                // If a type of overlap worth checking, continue
                if (checkStatus == true)
                {
                    collisionAngle = GetCollisionAngle(overlap); // get the collision angle
                    hitShield = ShieldZoneCollisionCheck(collisionAngle); // check based on angle whether an active shieldzone is hit

                    if (parry != null && parry.GetParryStatus() == true && hitWithinActiveShieldZone) { ReturnDamage(overlap); } // parry conditions met
                    else if (hitWithinActiveShieldZone)
                    {
                        HandleDamagePass(overlap, hitShield.damageAbsorption, hitShield.knockbackAbsorption, "BulletImpact"); // reduced dmg pass, if shieldzone is hit
                    }
                    else { HandleDamagePass(overlap, 0, 0, "DamageImpact"); } // regular 
                }
            }
        }
    }

    private void HandleDamagePass(Collider2D collision, float damageAbsorption, float knockbackAbsorption, string VFXToLoad)
    {
        DamageVFX(collision.transform.position, VFXToLoad);
        PassThroughDamage(collision, damageAbsorption, knockbackAbsorption);
    }

    private void DamageVFX(Vector3 positionToLoad, string VFXToLoad)
    {
        if (objectHitVFXCooldown == 0)
        {
            VFXPath = "VFXPrefabs/" + VFXToLoad;
            Instantiate(Resources.Load(VFXPath), positionToLoad, Quaternion.identity);
            objectHitVFXCooldown = 30;
        }
    }

    bool CheckIfHasHitDamagingObject(Collider2D overlap)
    {
        // assume an overlap isn't worth lookin at, but if turned true in subsequent functions - damage will be evaluated
        checkStatus = false;

        // GENERIC DAMAGE CHECKS
        if (overlap.gameObject.tag == "EnemyAttack") { checkStatus = true; Debug.Log(transform.parent.gameObject.name + " was hit by EnemyAttack"); }
        else if (overlap.gameObject.tag == "PlayerAttack" && overlap.enabled) { checkStatus = true; Debug.Log(transform.parent.gameObject.name + " was hit by PlayerAttack"); }
        else if(overlap.gameObject.layer == LayerMask.NameToLayer("PlayerAmmo")) { checkStatus = true; Debug.Log(transform.parent.gameObject.name + " was hit by Ammo"); }
        else if(overlap.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile")) { checkStatus = true; Debug.Log(transform.parent.gameObject.name + " was hit by Enemy Projectile"); }

        SpecificDamageChecks(overlap);

        return checkStatus;
    }

    /// <summary>
    /// An override for different types of child shields to use e.g. player or enemy specific checks
    /// </summary>
    /// <param name="collision"></param>
    virtual protected void SpecificDamageChecks(Collider2D collision) 
    {
    }

    /// <summary>
    /// Called in Start to label who is being shielded, and what layers to check for collisions
    /// </summary>
    virtual protected void CheckObjectType()
    {
        if(gameObject.layer == LayerMask.NameToLayer("Player")) { shieldedObject = "Player"; }
        else if(gameObject.layer == LayerMask.NameToLayer("Enemy")) { shieldedObject = "Enemy";  }
        else { Debug.Log("Shield.cs is attached to an object that does not need it"); }

        AddLayersToCheckOn(shieldedObject);
    }

    virtual protected void AddLayersToCheckOn(string shieldedObject)
    {
        if (shieldedObject == "Player") { attackerFilter.SetLayerMask((1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("EnemyProjectile"))); }
        else if (shieldedObject == "Enemy") { attackerFilter.SetLayerMask((1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("PlayerAmmo"))); }
        else { Debug.Log("Shielded object needs a tag; make sure it is tagged correctly in CheckObjectType() of this script"); }
    }


    protected void ShieldOn(bool value)
    {
        shieldCollider.enabled = value; spriteRenderer.enabled = value; shieldOn = value;
    }

    protected virtual void PassThroughDamage(Collider2D collision, float damageMod, float knockbackMod) { } // handled differently by child shields

    protected virtual void ReturnDamage(Collider2D collision) { } // parry, handled differently by child shields


    // GENERIC + UPDATE FUNCTIONS - PRIMARILY USED FOR SHIELDZONE DETECTION

    private ShieldZone ShieldZoneCollisionCheck(float collisionAngle)
    {
        if(shieldZones != null)
        {
            foreach (ShieldZone shield in shieldZones)
            {
                if (collisionAngle > shield.minAngle && collisionAngle < shield.maxAngle && shieldOn)
                {
                    hitWithinActiveShieldZone = true;
                    return shield;
                }
                else { hitWithinActiveShieldZone = false; }
            }
            return null;
        }
        else { Debug.Log("Shield.cs is equipped to gameobject " + gameObject.name + "; however, no shield zones have been added in the inspector"); }

        return null;
    }

    private float GetCollisionAngle(Collider2D collision)
    {
        shieldDirectionRelativeToAttack = CheckShieldDirection(gameObject.transform, collision.gameObject.transform);
        shieldPositionRelativeToAttack = CheckShieldPosition(gameObject.transform, collision.gameObject.transform);
        collisionAngle = AngleBetweenVectors(gameObject.transform.position, collision.gameObject.transform.position);

        return AdjustAngleWithDirAndPosition(collisionAngle, shieldDirectionRelativeToAttack, shieldPositionRelativeToAttack);
    }

    private float AngleBetweenVectors(Vector2 vec1, Vector2 vec2)
    {
        Vector2 difference = vec2 - vec1;
        return Vector2.SignedAngle(Vector2.right, difference);
    }

    private float AdjustAngleWithDirAndPosition(float collisionAngle, string shieldDirectionRelativeToAttack, string shieldPositionRelativeToAttack)
    {
        if (shieldPositionRelativeToAttack == "Left" && shieldDirectionRelativeToAttack == "Away") { collisionAngle -= -180; }
        else if (shieldPositionRelativeToAttack == "Left" && shieldDirectionRelativeToAttack == "Towards") { collisionAngle += 0; }
        else if (shieldPositionRelativeToAttack == "Right" && shieldDirectionRelativeToAttack == "Away") { collisionAngle += 0; }
        else if (shieldPositionRelativeToAttack == "Right" && shieldDirectionRelativeToAttack == "Towards") { collisionAngle += 180; }
        else if (shieldPositionRelativeToAttack == "UpOrDown") { return collisionAngle; }
        else if(shieldDirectionRelativeToAttack == "Neither") { return collisionAngle; }
        else { Debug.Log("No transformations needed, evaluate this function's code"); }

        if(collisionAngle >= 180 || collisionAngle <= -180) { collisionAngle = 360 - collisionAngle; return collisionAngle; }
        else { return collisionAngle; }
    }

    private string CheckShieldPosition(Transform shielded, Transform attack)
    {
        if (attack.position.x - shielded.position.x > 0) { return "Left"; }
        else if (attack.position.x - shielded.position.x < 0) { return "Right"; }
        else { return "UpOrDown"; }
    }

    private string CheckShieldDirection(Transform shielded, Transform attack)
    {
        if (Vector2.Dot(-shielded.right, attack.right) == 1) { return "Towards"; }
        else if (Vector2.Dot(-shielded.right, attack.right) == -1) { return "Away"; }
        else { Debug.Log("Check CheckShieldDirection function in this script for proper functioning"); return "Neither"; }
    }
}
