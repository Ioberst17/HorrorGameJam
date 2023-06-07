using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] // for detection
[RequireComponent(typeof(SpriteRenderer))] // for shield image
public class Shield : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D shieldCollider;
    public bool shieldOn;
    public bool Invincibility { get; set; }
    public bool checkStatus;
    public string shieldedObject;
    public Parry parry;
    private string VFXPath;

    [SerializeField] private string shieldDirectionRelativeToAttack;
    [SerializeField] private string shieldPositionRelativeToAttack;
    [SerializeField] private float collisionAngle;
    public bool hitWithinActiveShieldZone;

    public Collider2D[] attackingObjects = new Collider2D[10];
    public ContactFilter2D attackerFilter;

    [SerializeField] public List<ShieldZone> shieldZones = new List<ShieldZone>();
    private ShieldZone hitShield;
    // particle system for lock hitting


    // Start is called before the first frame update
    public void Start()
    {
        //EventSystem.current.onPlayerShieldHitTrigger += DamageHandler;

        TryGetComponent(out parry);
        spriteRenderer = GetComponent<SpriteRenderer>();
        shieldCollider = GetComponent<Collider2D>();
        ShieldStatus("Off");
        CheckObjectType();
    }

    public void Update() 
    {
        attackingObjects = Physics2D.OverlapCircleAll(shieldCollider.bounds.center,
                                                shieldCollider.bounds.extents.x,
                                                attackerFilter.layerMask);

        DamageHandler(attackingObjects);
    }

    public void DamageHandler(Collider2D[] overlaps)
    {
        if (overlaps != null)
        {
            foreach(Collider2D overlap in overlaps)
            {
                if(Invincibility == true) // e.g. if player is in hitStun or otherwise invincible, pass no dmg; this is set by child shields for there specific conditions
                {
                    HandleDamagePass(overlap, 0, 0, "BulletImpact");
                }
                else // check hit conditions
                {
                    checkStatus = CheckForRelevantDamageConditions(overlap);

                    // If a type of overlap worth checking, continue
                    if (checkStatus == true)
                    {
                        collisionAngle = GetCollisionAngle(overlap); // get the collision angle
                        hitShield = ShieldZoneCollisionCheck(collisionAngle); // check based on angle whether an active shieldzone is hit

                        if (parry != null && parry.GetParryStatus() == true && hitWithinActiveShieldZone) { ReturnDamage(overlap); } // parry conditions met
                        else if(hitWithinActiveShieldZone) 
                        {
                            HandleDamagePass(overlap, hitShield.damageAbsorption, hitShield.knockbackAbsorption, "BulletImpact"); // reduced dmg pass, if shieldzone is hit
                        }
                        else
                        {
                            HandleDamagePass(overlap, 0, 0, "DamageImpact"); } // regular damage pass
                        }
                }

                // else { Debug.Log("Regular damage"); HandleDamagePass(overlap, 0, 0, "DamageImpact"); } // regular damage pass through to shielded object
            }
        }

    }
    private void HandleDamagePass(Collider2D collision, float damageAbsorption, float knockbackAbsorption, string VFXToLoad)
    {
        VFXPath = "VFXPrefabs/" + VFXToLoad;
        Instantiate(Resources.Load(VFXPath), collision.transform.position, Quaternion.identity);
        PassThroughDamage(collision, damageAbsorption, knockbackAbsorption);
    }

    bool CheckForRelevantDamageConditions(Collider2D overlap)
    {
        // assume an overlap isn't worth lookin at
        checkStatus = false;

        // IF CHECKSTATUS IS TRUE, DAMAGE IS EVALUATED
        SpecificDamageChecks(overlap); // an override for different types of child shields to use e.g. player or enemy specific checks

        // GENERIC DAMAGE CHECKS
        if (shieldedObject == "Player" && overlap.gameObject.GetComponent<Explode>() != null) { checkStatus = true; }
        else if (shieldedObject == "Enemy") { checkStatus = true; }

        return checkStatus;
    }
    

    virtual public void SpecificDamageChecks(Collider2D collision) { }

    // START FUNCTIONS TO UPDATE INERNAL VARIABLES ON WHAT OBJECT IS BEING SHIELDED
    virtual public void CheckObjectType()
    {
        //if(gameObject.layer == LayerMask.NameToLayer("Player")) { shieldedObject = "Player"; }
        if(gameObject.layer == LayerMask.NameToLayer("Enemy")) { shieldedObject = "Enemy";  }
        else { Debug.Log("Shield.cs is attached to an object that does not need it"); }

        AddLayersToCheckOn(shieldedObject);
    }

    virtual public void AddLayersToCheckOn(string shieldedObject)
    {
        if (shieldedObject == "Enemy") { attackerFilter.SetLayerMask((1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Player Ammo"))); }
    }

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

    public void ShieldStatus(string OnOff)
    {
        if (OnOff == "On") { spriteRenderer.enabled = true; shieldOn = true; }
        else if (OnOff == "Off") { spriteRenderer.enabled = false; shieldOn = false; }
        else { Debug.Log("ActivateShield function is misfiring in Shield.cs"); }   
    }

    public virtual void PassThroughDamage(Collider2D collision, float damageMod, float knockbackMod) { } // handled differently by child shields

    public virtual void ReturnDamage(Collider2D collision) { } // parry, handled differently by chield shields

    private void OnDestroy()
    {
        //EventSystem.current.onPlayerShieldHitTrigger += DamageHandler;
    }

}
