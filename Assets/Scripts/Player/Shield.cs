using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] // for detection
[RequireComponent(typeof(SpriteRenderer))] // for shield image
public class Shield : MonoBehaviour
{
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private Color startingSpriteColor;
    public bool shieldOn;
    private bool checkStatus;
    [SerializeField] private string shieldedObject;

    private Vector2 blockingVec2;
    private Vector2 attackingVec2;
    [SerializeField] private string shieldDirectionRelativeToAttack;
    [SerializeField] private string shieldPositionRelativeToAttack;
    [SerializeField] private float collisionAngle;


    private List<int?> layersToCheck;
    [SerializeField] private int? layer1ToCheck;
    [SerializeField] private int? layer2ToCheck;


    [SerializeField] public List<ShieldZone> shieldZones = new List<ShieldZone>();
    private ShieldZone hitShield;
    private bool isBlocked;
    // particle system for lock hitting


    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Player")) { playerController = GetComponentInParent<PlayerController>(); }
        spriteRenderer = GetComponent<SpriteRenderer>();
        ShieldStatus("Off");
        CheckObjectType();
    }

    // Update is called once per frame
    void Update() { if (Input.GetKey(KeyCode.F)) { ShieldStatus("On"); } else { ShieldStatus("Off"); } }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < layersToCheck.Count; i++)
        { if (collision.gameObject.layer == layersToCheck[i]) 
            {
                if (shieldOn) 
                {
                    checkStatus = false;
                    if(shieldedObject == "Player" && collision.gameObject.GetComponent<EnemyController>().isAttacking) { checkStatus = true; }
                    else if(shieldedObject == "Enemy") { checkStatus = true; }
                    if(checkStatus == true) 
                    {
                        hitShield = ShieldZoneCollisionCheck(CheckCollisionAngle(collision));
                        if (hitShield != null)
                        {
                            Instantiate(Resources.Load("VFXPrefabs/BulletImpact"), collision.transform.position, Quaternion.identity);
                            PassThroughDamage(collision, hitShield.damageAbsorption, hitShield.knockbackAbsorption);
                        }
                        else
                        {
                            Instantiate(Resources.Load("VFXPrefabs/BulletImpact"), collision.transform.position, Quaternion.identity);
                            PassThroughDamage(collision, 0, 0);
                        }
                    }
                }
                else
                {
                    Instantiate(Resources.Load("VFXPrefabs/DamageImpact"), collision.transform.position, Quaternion.identity);
                    PassThroughDamage(collision, 0, 0);
                }
            } 
        }
    }

    // START FUNCTIONS
    private void CheckObjectType()
    {
        if(gameObject.layer == LayerMask.NameToLayer("Player")) { shieldedObject = "Player"; }
        else if(gameObject.layer == LayerMask.NameToLayer("Enemy")) { shieldedObject = "Enemy";  }
        else { Debug.Log("Shield.cs is attached to an object that does not need it"); }

        AddCollisionsToCheckFor(shieldedObject);
    }

    void AddCollisionsToCheckFor(string shieldedObject)
    {
        if (shieldedObject == "Player") { layer1ToCheck = LayerMask.NameToLayer("Enemy"); layer2ToCheck = null; }
        else if (shieldedObject == "Enemy") { layer1ToCheck = LayerMask.NameToLayer("Player"); layer2ToCheck = LayerMask.NameToLayer("Ammo"); }

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

        Debug.Log("CollisionAngle's edited value is: " + collisionAngle);
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

    void PassThroughDamage(Collider2D collision, float damageMod, float knockbackMod)
    {
        if (shieldedObject == "Player")
        {
            EventSystem.current.PlayerHitTrigger(
                              collision.gameObject.transform.position,
                              collision.gameObject.GetComponent<EnemyController>().damageValue,
                              1,
                              damageMod,
                              knockbackMod);
        }
        if(shieldedObject == "Enemy") { /*TO BE FILLED IF ENEMY USES SHIELD*/ }
    }

}
