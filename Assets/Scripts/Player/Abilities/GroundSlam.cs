using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerAttackManager))]
public class GroundSlam : MonoBehaviour
{
    PlayerAttackManager playerPrimaryWeapon;
    PlayerController playerController;
    PlayerAnimator playerAnimator;
    public enum TypeOfHit { Damagable, NonDamagable } // i.e. an enemy or breakable object vs the ground
    [SerializeField] private bool _isPreDropGroundSlam; 
    public bool IsPreDropGroundSlam
    {
        get { return _isPreDropGroundSlam; }
        set 
        {
            // if true lock player movement and set gravity scale to 0
            if (value == true) { _isPreDropGroundSlam = true; playerController.CanMove = false; playerController.SetGravityScale(0); } 
            // when set to false enable x-y input and return gravity scale to normal
            else { _isPreDropGroundSlam = false; playerController.CanMove = true; playerController.SetGravityScale(3); } 
        } 
    }    [SerializeField] private bool _isGroundSlam; 
    public bool IsGroundSlam
    {
        get { return _isGroundSlam; }
        set 
        {
            // if groundslam is dropping lock x movement, and set groundslam to false
            if (value == true) { _isGroundSlam = true; playerController.CanMoveX = false; IsPreDropGroundSlam = false; } 
            // once groundslam is finished return player's x-y movement
            else { _isGroundSlam = false; playerController.CanMoveX = true; } 
        } 
    }

    [Header("Ground Slam Settings")]
    [SerializeField] float groundSlamDelayBeforeDrop = 0.5f;
    [SerializeField] private float groundSlamSpeed = -20f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float reboundForceY = 10f;
    [SerializeField] private int framesSinceLastGroundSlamVFX;
    [SerializeField] private int minFramesBetweenGroundSlamVFX;
    
    void Start()
    {
        EventSystem.current.onGroundSlamDrop += GroundSlamDrop;
        playerPrimaryWeapon = GetComponent<PlayerAttackManager>();
        playerController = playerPrimaryWeapon.GetComponentInParent<PlayerController>();
        playerAnimator = playerController.GetComponentInChildren<PlayerAnimator>();
    }

    private void OnDestroy()
    {
        EventSystem.current.onGroundSlamDrop -= GroundSlamDrop;
    }

    private void Update()
    {
        if (framesSinceLastGroundSlamVFX <= minFramesBetweenGroundSlamVFX + 1) { framesSinceLastGroundSlamVFX++; }

        if (IsGroundSlam) { GroundSlamContinuedFX(); }
    }

    /// <summary>
    /// Triggers the start of groundslam, before the keyframe animation drop
    /// </summary>
    public void Execute() { playerController.IsInvincible = true; IsPreDropGroundSlam = true; }


    /// <summary>
    /// Triggers Groundslam animation after delay
    /// </summary>
    /// <param name="attackDirection"></param>
    /// <returns></returns>
    public IEnumerator GroundSlamStartHelper(int attackDirection)
    {
        yield return new WaitForSeconds(groundSlamDelayBeforeDrop); // Wait for 1 second
        playerPrimaryWeapon.StartAttack(attackDirection, "PlayerGroundSlam");
    }

    /// <summary>
    /// Triggered by a specific frame in the GroundSlam animation on the base animator; starts slam
    /// </summary>
    void GroundSlamDrop()
    {
        EventSystem.current.ActiveMeleeTrigger();
        Debug.Log("Groundslam triggered");
        IsGroundSlam = true;
        SetPlayerVelocityForDownSwing();
        FindObjectOfType<AudioManager>().PlaySFX("PlayerMelee");
        Instantiate(Resources.Load("VFXPrefabs/GroundSlamImpact"), transform.position, Quaternion.identity);
    }

    private void SetPlayerVelocityForDownSwing()
    {
        playerController.SetVelocity();
        playerController.AddForce(0f, groundSlamSpeed);
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
        playerAnimator.Play("PlayerLand");
        Instantiate(Resources.Load("VFXPrefabs/DustCloud"), playerController.GroundCheck.transform.position, Quaternion.identity); 
    }


    void Bounce()
    {
        playerController.SetVelocity();
        playerController.AddForce(0, reboundForceY);
        Debug.Log("Sending player up at " + reboundForceY + " velocity");
    }

    public void Finished(GameObject hitObject, TypeOfHit typeOfHit)
    {
        // Reset conditions
        IsGroundSlam = false; EventSystem.current.EndActiveMeleeTrigger();

        // Unique behaviors depending what type of object has been hit
        if (typeOfHit == TypeOfHit.Damagable) { Bounce(); HitDamagableSoundAndVFX(); }
        else if (typeOfHit == TypeOfHit.NonDamagable) { playerController.SetVelocity(); HitNonDamagableSoundAndVFX(); }
        Debug.Log("Hit gameObject named: " + hitObject.name);
        Invoke("InvincibilityOff", .5f);
    }

    void InvincibilityOff() { playerController.IsInvincible = false; }
}
