using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerPrimaryWeapon))]
public class GroundSlam : MonoBehaviour
{
    PlayerPrimaryWeapon playerPrimaryWeapon;
    PlayerHealth playerHealth;
    PlayerController playerController;
    public List<Collider2D> groundSlamHitList;
    private int groundSlamHitListLength;
    [SerializeField] ContactFilter2D groundSlamCollisionFilter;
    [SerializeField] bool groundSlamStop = true;
    SpriteRenderer spriteRenderer;
    Collider2D detectionCollider;
    [SerializeField] private bool _isGroundSlam; public bool IsGroundSlam { get => _isGroundSlam; set => _isGroundSlam = value; }
    private bool hasHit;
    private GameObject dustCloudPrefab;
    private bool damagableFlag, nonDamagableFlag, nonDamagableVFXFlag; 

    [Header("Ground Slam Settings")]
    [SerializeField] private float groundSlamSpeed = -20f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float reboundForceX = 10f;
    [SerializeField] private float reboundForceY = 10f;
    [SerializeField] private int framesSinceLastGroundSlamVFX;
    [SerializeField] private int minFramesBetweenGroundSlamVFX;
    private Vector2 detectionColliderCenter, detectionColliderHalfSize, bottomLeftCorner, upperRightCorner, detectionColliderUpperCenter; 
    
    

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = transform.Find("GroundSlamDetector").GetComponent<SpriteRenderer>();
        detectionCollider = transform.Find("GroundSlamDetector").GetComponent<Collider2D>();
        playerHealth = GetComponentInParent<PlayerHealth>();
        ActivateDetection(false);
        playerPrimaryWeapon = GetComponent<PlayerPrimaryWeapon>();
        playerController = playerPrimaryWeapon.GetComponentInParent<PlayerController>();
        groundSlamHitList = new List<Collider2D>();
        groundSlamCollisionFilter = new ContactFilter2D();
        groundSlamCollisionFilter.SetLayerMask((1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("BreakableEnviro")) | 1 << LayerMask.NameToLayer("Environment"));
    }

    private void FixedUpdate()
    {
        if (framesSinceLastGroundSlamVFX <= minFramesBetweenGroundSlamVFX + 1) { framesSinceLastGroundSlamVFX++; }

        if (groundSlamStop == false && !playerController.IsGrounded)
        {
            if (CheckIfFinished(DoGroundSlam())) { }
            if(groundSlamStop == true)
            {
                if(damagableFlag == true) { HitDamagableSoundAndVFX(); }
                if (nonDamagableFlag == true) { HitNonDamagableSoundAndVFX(); }
                damagableFlag = false; nonDamagableFlag = false;
            }
        }
    }

    void ActivateDetection(bool state)
    {
        spriteRenderer.enabled = true;
        detectionCollider.enabled = state;
    }

    public void Execute()
    {
        _isGroundSlam = true; playerHealth.isInvincible = true; groundSlamStop = false; ActivateDetection(true);
        FindObjectOfType<AudioManager>().PlaySFX("PlayerMelee");
        Instantiate(Resources.Load("VFXPrefabs/GroundSlamImpact"), transform.position, Quaternion.identity);
    }

    List<Collider2D> DoGroundSlam()
    {
        if (framesSinceLastGroundSlamVFX >= minFramesBetweenGroundSlamVFX)
        {
            Instantiate(Resources.Load("VFXPrefabs/GroundSlamImpact"), transform.position, Quaternion.identity);
            framesSinceLastGroundSlamVFX = 0;
        }
        
        playerController.SetVelocity(0, groundSlamSpeed);
        return DetectHits();
    }

    List<Collider2D> DetectHits()
    {
        SetDetectionBoundaries();
        groundSlamHitList = Physics2D.OverlapAreaAll(bottomLeftCorner, upperRightCorner, groundSlamCollisionFilter.layerMask).ToList();
        return groundSlamHitList;
    }

    void SetDetectionBoundaries()
    {
        detectionColliderCenter = spriteRenderer.gameObject.transform.position;
        detectionColliderHalfSize = new Vector2(spriteRenderer.bounds.size.x / 2f, spriteRenderer.bounds.size.y / 2f);
        bottomLeftCorner = detectionColliderCenter - detectionColliderHalfSize;
        upperRightCorner = detectionColliderCenter + detectionColliderHalfSize;
        detectionColliderUpperCenter = detectionColliderCenter + new Vector2(0, spriteRenderer.bounds.size.y / 2f);

        if (detectionColliderHalfSize == Vector2.zero || bottomLeftCorner == Vector2.zero || upperRightCorner == Vector2.zero || detectionColliderUpperCenter == Vector2.zero)
        {
            Debug.LogError("Failed to set detection boundaries.");
        }
    }

    private bool CheckIfFinished(List<Collider2D> groundSlamHits)
    {
        hasHit = false;
        if (groundSlamHits.Count > 0)
        {
            foreach (Collider2D hit in groundSlamHits)
            {
                if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.gameObject.layer == LayerMask.NameToLayer("BreakableEnviro"))
                { ResetConditions(hit); damagableFlag = true; hasHit = true;  }

                else if (hit.gameObject.layer == LayerMask.NameToLayer("Environment") || hit.gameObject.tag == "Boundary" || playerController.IsGrounded)
                { ResetConditions(hit); nonDamagableFlag = true; hasHit = true; nonDamagableVFXFlag = true; }
            }
        }

        return hasHit;
    }

    void HitDamagableSoundAndVFX()
    {
        // handle sound and VFX
    }

    void HitNonDamagableSoundAndVFX()
    {
        FindObjectOfType<AudioManager>().PlaySFX("GroundSlam");
        SetDetectionBoundaries();
        Instantiate(Resources.Load("VFXPrefabs/DustCloud"), detectionColliderUpperCenter, Quaternion.identity);
        nonDamagableVFXFlag = false;
    }


    void Bounce(Collider2D hit)
    {
        playerController.SetVelocity(0, reboundForceY);
        if (hit.GetComponent<IDamageable>() != null) { hit.gameObject.GetComponent<IDamageable>().Hit(attackDamage, transform.position); }
        Debug.Log("Sending player up at " + reboundForceY + " velocity");
    }

    void ResetConditions(Collider2D hit)
    {
        groundSlamStop = true; playerPrimaryWeapon.isAttacking = false; _isGroundSlam = false; ActivateDetection(false); 
        if(nonDamagableFlag == true) { playerController.SetVelocity(); }
        else if(damagableFlag == true) { Bounce(hit); }
        Debug.Log("Hit gameObject named: " + hit.gameObject.name);
        Invoke("InvincibilityOff", .5f);
    }

    void InvincibilityOff() { playerHealth.isInvincible = false; }
}
