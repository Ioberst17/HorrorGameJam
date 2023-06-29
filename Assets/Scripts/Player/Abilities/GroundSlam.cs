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
    PlayerAnimator playerAnimator;
    SpriteRenderer detectorSprite;
    [SerializeField] bool groundSlamStop = true;
    public enum TypeOfHit { Damagable, NonDamagable } // i.e. an enemy or breakable object vs the ground
    [SerializeField] private bool _isGroundSlam; public bool IsGroundSlam { get => _isGroundSlam; set => _isGroundSlam = value; }

    [Header("Ground Slam Settings")]
    [SerializeField] private float groundSlamSpeed = -20f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float reboundForceX = 10f;
    [SerializeField] private float reboundForceY = 10f;
    [SerializeField] private int framesSinceLastGroundSlamVFX;
    [SerializeField] private int minFramesBetweenGroundSlamVFX;
    private Vector2 detectionColliderCenter, detectionColliderUpperCenter; 
    
    

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
        playerPrimaryWeapon = GetComponent<PlayerPrimaryWeapon>();
        playerController = playerPrimaryWeapon.GetComponentInParent<PlayerController>();
        playerAnimator = playerController.GetComponentInChildren<PlayerAnimator>();

        // used for nondamagable VFX on landing
        detectorSprite = GetComponentInChildren<PlayerGroundSlamDetector>().GetComponent<SpriteRenderer>();
        detectionColliderCenter = detectorSprite.transform.position;
        detectionColliderUpperCenter = detectionColliderCenter + new Vector2(0, detectorSprite.bounds.size.y / 2f);
    }

    // Note: groundslam is finished / triggered off by the PlayerGroundSlamDetector, which handles checks for overlaps
    private void Update()
    {
        if (framesSinceLastGroundSlamVFX <= minFramesBetweenGroundSlamVFX + 1) { framesSinceLastGroundSlamVFX++; }

        if (IsGroundSlam == true) { GroundSlamContinuedFX(); }
    }

    public void Execute()
    {
        IsGroundSlam = true; playerHealth.IsInvincible = true; groundSlamStop = false; //ActivateDetection(true); // turn on update conditions
        playerController.SetVelocity(0, groundSlamSpeed); // trigger speed change, once
        
        // One-time SFX and VFX updates
        FindObjectOfType<AudioManager>().PlaySFX("PlayerMelee");
        Instantiate(Resources.Load("VFXPrefabs/GroundSlamImpact"), transform.position, Quaternion.identity);
    }

    void GroundSlamContinuedFX()
    {
        if (framesSinceLastGroundSlamVFX >= minFramesBetweenGroundSlamVFX)
        {
            Instantiate(Resources.Load("VFXPrefabs/GroundSlamImpact"), transform.position, Quaternion.identity);
            framesSinceLastGroundSlamVFX = 0;
        }
    }

    void HitDamagableSoundAndVFX()
    {
        // handle sound and VFX
    }

    void HitNonDamagableSoundAndVFX()
    {
        FindObjectOfType<AudioManager>().PlaySFX("GroundSlam");
        detectionColliderCenter = detectorSprite.transform.position;
        detectionColliderUpperCenter = detectionColliderCenter + new Vector2(0, detectorSprite.bounds.size.y / 2f);
        //if (playerController.IsGrounded) 
        //{
            playerAnimator.Play("PlayerLand");
            Instantiate(Resources.Load("VFXPrefabs/DustCloud"), detectionColliderUpperCenter, Quaternion.identity); 
        //}
    }


    void Bounce(GameObject hitObject)
    {
        playerController.SetVelocity(0, reboundForceY);
        hitObject.GetComponent<IDamageable>().Hit(attackDamage, transform.position); 
        Debug.Log("Sending player up at " + reboundForceY + " velocity");
    }

    public void Finished(GameObject hitObject, TypeOfHit typeOfHit)
    {
        // Reset conditions
        IsGroundSlam = false; groundSlamStop = true; playerPrimaryWeapon.isAttacking = false; //ActivateDetection(false);
        
        // Unique behaviors depending what type of object has been hit
        if (typeOfHit == TypeOfHit.NonDamagable) { playerController.SetVelocity(); HitNonDamagableSoundAndVFX(); }
        else if(typeOfHit == TypeOfHit.Damagable) { Bounce(hitObject); HitDamagableSoundAndVFX(); }
        Debug.Log("Hit gameObject named: " + hitObject.name);
        Invoke("InvincibilityOff", .5f);
    }

    void InvincibilityOff() { playerHealth.IsInvincible = false; }
}
