using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] // for detection
[RequireComponent(typeof(SpriteRenderer))] // for shield image
public class Shield : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public bool shieldOn;
    public bool checkStatus;
    public string shieldedObject;
    private Parry parry;
    private string VFXPath;

    [SerializeField] private string shieldDirectionRelativeToAttack;
    [SerializeField] private string shieldPositionRelativeToAttack;
    [SerializeField] private float collisionAngle;


    public List<int?> layersToCheck;
    [SerializeField] public int? layer1ToCheck;
    [SerializeField] public int? layer2ToCheck;


    [SerializeField] public List<ShieldZone> shieldZones = new List<ShieldZone>();
    private ShieldZone hitShield;
    // particle system for lock hitting


    // Start is called before the first frame update
    public void Start()
    {
        EventSystem.current.onPlayerShieldHitTrigger += DamageHandler;

        TryGetComponent(out parry);
        spriteRenderer = GetComponent<SpriteRenderer>();
        ShieldStatus("Off");
        CheckObjectType();
    }

    // Update is called once per frame
    void Update() 
    { if (Input.GetKey(KeyCode.F)) { ShieldStatus("On"); } else { ShieldStatus("Off"); }
        if (Input.GetKeyDown(KeyCode.F)) { if(parry != null) { parry.Execute(); } }
    }

    private void OnTriggerEnter2D(Collider2D collision) { DamageHandler(collision);}

    public void DamageHandler(Collider2D collision)
    {
        for (int i = 0; i < layersToCheck.Count; i++)
        {
            if (collision.gameObject.layer == layersToCheck[i])
            {
                if (shieldOn)
                {
                    checkStatus = false;
                    SpecificDamageChecks(collision);
                    if (shieldedObject == "Player" && collision.gameObject.GetComponent<Explode>() != null) { checkStatus = true; }
                    else if (shieldedObject == "Enemy") { checkStatus = true; }
                    if (checkStatus == true)
                    {
                        hitShield = ShieldZoneCollisionCheck(CheckCollisionAngle(collision));

                        if(parry != null && parry.GetParryStatus() == true) { ReturnDamage(collision); }
                        else
                        {
                            if (hitShield != null) { HandleDamagePass(collision, hitShield.damageAbsorption, hitShield.knockbackAbsorption, "BulletImpact"); }
                            else { HandleDamagePass(collision, 0, 0, "BulletImpact");}
                        }
                    }
                }
                else { HandleDamagePass(collision, 0, 0, "DamageImpact"); }
            }
        }
    }

    private void HandleDamagePass(Collider2D collision, float damageAbsorption, float knockbackAbsorption, string VFXToLoad)
    {
        VFXPath = "VFXPrefabs/" + VFXToLoad;
        Instantiate(Resources.Load(VFXPath), collision.transform.position, Quaternion.identity);
        PassThroughDamage(collision, damageAbsorption, knockbackAbsorption);
    }


    virtual public void SpecificDamageChecks(Collider2D collision) { }

    // START FUNCTIONS
    virtual public void CheckObjectType()
    {
        //if(gameObject.layer == LayerMask.NameToLayer("Player")) { shieldedObject = "Player"; }
        if(gameObject.layer == LayerMask.NameToLayer("Enemy")) { shieldedObject = "Enemy";  }
        else { Debug.Log("Shield.cs is attached to an object that does not need it"); }

        AddCollisionsToCheckFor(shieldedObject);
    }

    virtual public void AddCollisionsToCheckFor(string shieldedObject)
    {
        if (shieldedObject == "Enemy") { layer1ToCheck = LayerMask.NameToLayer("Player"); layer2ToCheck = LayerMask.NameToLayer("Ammo"); }
        layersToCheck = new List<int?> { layer1ToCheck, layer2ToCheck };
    }

    // UPDATE FUNCTIONS

    private ShieldZone ShieldZoneCollisionCheck(float collisionAngle)
    {
        if(shieldZones != null)
        {
            foreach (ShieldZone shield in shieldZones)
            {
                if (collisionAngle > shield.minAngle && collisionAngle < shield.maxAngle)
                {
                    return shield;
                }
            }
        }
        else { Debug.Log("Shield.cs is equipped to gameobject " + gameObject.name + "; however, no shield zones have been added in the inspector"); }

        return null;
    }

    private float CheckCollisionAngle(Collider2D collision)
    {
        shieldDirectionRelativeToAttack = CheckShieldDirection(gameObject.transform, collision.gameObject.transform);
        shieldPositionRelativeToAttack = CheckShieldPosition(gameObject.transform, collision.gameObject.transform);
        collisionAngle = AngleBetweenVectors(gameObject.transform.position, collision.gameObject.transform.position);

        collisionAngle = AdjustAngleWithDirAndPosition(collisionAngle, shieldDirectionRelativeToAttack, shieldPositionRelativeToAttack);

        return collisionAngle;
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

    void ShieldStatus(string OnOff)
    {
        if (OnOff == "On") { spriteRenderer.enabled = true; shieldOn = true; }
        else if (OnOff == "Off") { spriteRenderer.enabled = false; shieldOn = false; }
        else { Debug.Log("ActivateShield function is misfiring in Shield.cs"); }   
    }

    public virtual void PassThroughDamage(Collider2D collision, float damageMod, float knockbackMod) { }

    public virtual void ReturnDamage(Collider2D collision)
    {
    }

    private void OnDestroy()
    {
        EventSystem.current.onPlayerShieldHitTrigger += DamageHandler;
    }

}
