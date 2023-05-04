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
    public bool isGroundSlam;
    private GameObject dustCloudPrefab;
    private bool damagableFlag, nonDamagableFlag;

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

        if (groundSlamStop == false && !playerController.isGrounded)
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
        spriteRenderer.enabled = state;
        detectionCollider.enabled = state;
    }

    public void Execute()
    {
        isGroundSlam = true; playerHealth.isInvincible = true; groundSlamStop = false; ActivateDetection(true);
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
        return Detect();
    }

    List<Collider2D> Detect()
    {
        DetectionSetup();
        groundSlamHitList = Physics2D.OverlapAreaAll(bottomLeftCorner, upperRightCorner, groundSlamCollisionFilter.layerMask).ToList();
        return groundSlamHitList;
    }

    void DetectionSetup()
    {
        detectionColliderCenter = spriteRenderer.gameObject.transform.position;
        detectionColliderHalfSize = new Vector2(spriteRenderer.bounds.size.x / 2f, spriteRenderer.bounds.size.y / 2f);
        bottomLeftCorner = detectionColliderCenter - detectionColliderHalfSize;
        upperRightCorner = detectionColliderCenter + detectionColliderHalfSize;
        detectionColliderUpperCenter = detectionColliderCenter + new Vector2(0, spriteRenderer.bounds.size.y / 2f);
    }

    private bool CheckIfFinished(List<Collider2D> groundSlamHits)
    {
        if (groundSlamHits.Count > 0)
        {
            foreach (Collider2D hit in groundSlamHits)
            {
                if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.gameObject.layer == LayerMask.NameToLayer("BreakableEnviro"))
                { OnHit(hit); Bounce(hit); damagableFlag = true; }

                else if (hit.gameObject.layer == LayerMask.NameToLayer("Environment") || hit.gameObject.tag == "Boundary")
                { OnHit(hit); nonDamagableFlag = true; }
            }
        }
        return false;
    }

    bool OnHit(Collider2D hit)
    {
        ResetConditions(hit);
        return true;
    }

    void HitDamagableSoundAndVFX()
    {
        // handle sound and VFX
    }

    void HitNonDamagableSoundAndVFX()
    {
        FindObjectOfType<AudioManager>().PlaySFX("GroundSlam");
        DetectionSetup();
        Instantiate(Resources.Load("VFXPrefabs/DustCloud"), detectionColliderUpperCenter, Quaternion.identity);
    }


    void Bounce(Collider2D hit)
    {
        playerController.SetVelocity(0, reboundForceY);
        if (hit.GetComponent<IDamageable>() != null) { hit.gameObject.GetComponent<IDamageable>().Hit(attackDamage, transform.position); }
        Debug.Log("Sending player up at " + reboundForceY + " velocity");
    }

    void ResetConditions(Collider2D hit)
    {
        groundSlamStop = true; playerPrimaryWeapon.isAttacking = false; isGroundSlam = false; ActivateDetection(false);
        Debug.Log("Hit gameObject named: " + hit.gameObject.name);
        Invoke("InvincibilityOff", .5f);
    }

    void InvincibilityOff() { playerHealth.isInvincible = false; }
}
