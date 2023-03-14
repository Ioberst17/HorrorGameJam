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
    PlayerController playerController;
    private List<Collider2D> groundSlamHitList;
    private int groundSlamHitListLength;
    [SerializeField] ContactFilter2D groundSlamCollisionFilter;
    [SerializeField] bool groundSlamStop;
    [SerializeField] int groundSlamCounter;
    [SerializeField] private float groundSlamSpeed = -20f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float reboundForceX = 10f;
    [SerializeField] private float reboundForceY = 10f;
    private int framesSinceGroundSlamFinish;
    private int minFramesBetweenGroundSlamVFX = 100;
    
    SpriteRenderer spriteRenderer;
    Collider2D detectionCollider;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = transform.Find("GroundSlamDetector").GetComponent<SpriteRenderer>();
        detectionCollider = transform.Find("GroundSlamDetector").GetComponent<Collider2D>();
        ActivateDetection(false);
        playerPrimaryWeapon = GetComponent<PlayerPrimaryWeapon>();
        groundSlamHitList = new List<Collider2D>();
        groundSlamCollisionFilter = new ContactFilter2D();
        groundSlamCollisionFilter.SetLayerMask((1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("BreakableEnviro")) | 1 << LayerMask.NameToLayer("Environment"));
    }

    void Update()
    {
        framesSinceGroundSlamFinish++;
    }

    void ActivateDetection(bool state)
    {
        spriteRenderer.enabled = state;
        detectionCollider.enabled = state;
    }

    public void Execute()
    {
        //PlayRandomJumpSound();
        FindObjectOfType<AudioManager>().PlaySFX("PlayerMelee");
        Instantiate(Resources.Load("VFXPrefabs/GroundSlamImpact"), transform.position, Quaternion.identity);

        groundSlamStop = false; groundSlamCounter = 0;
        while (groundSlamStop == false && groundSlamCounter < 1000)
        {
            if (CheckIfFinished(DoGroundSlam())){ groundSlamStop = true; }
            groundSlamCounter++;
        }
        ActivateDetection(false);
        playerPrimaryWeapon.isAttacking = false;
    }

    List<Collider2D> DoGroundSlam()
    {
        if (framesSinceGroundSlamFinish > minFramesBetweenGroundSlamVFX)
        {
            Instantiate(Resources.Load("VFXPrefabs/GroundSlamImpact"), transform.position, Quaternion.identity);
            framesSinceGroundSlamFinish = 0;
        }
        ActivateDetection(true);
        playerPrimaryWeapon.playerController.SetVelocity(0, groundSlamSpeed);
        Vector2 center = spriteRenderer.gameObject.transform.position;
        Vector2 halfSize = new Vector2(spriteRenderer.bounds.size.x / 2f, spriteRenderer.bounds.size.y / 2f);
        groundSlamHitListLength = Physics2D.OverlapArea(center - halfSize, center + halfSize, groundSlamCollisionFilter, groundSlamHitList);
        return groundSlamHitList;

    }

    private bool CheckIfFinished(List<Collider2D> groundSlamHits)
    {
        if (groundSlamHits.Count > 0)
        {
            foreach (Collider2D hit in groundSlamHits)
            {
                if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.gameObject.layer == LayerMask.NameToLayer("BreakableEnviro"))
                {
                    hit.gameObject.GetComponent<IDamageable>().Hit(attackDamage, transform.position);

                    if (hit.gameObject.transform.position.x >= transform.position.x) { playerPrimaryWeapon.playerController.AddForce(-reboundForceX, reboundForceY); }
                    else { playerPrimaryWeapon.playerController.AddForce(reboundForceX, reboundForceY); }

                    /* Handle Particles and Sound */
                    return true;
                }
                else if (hit.gameObject.layer == LayerMask.NameToLayer("Environment") || hit.gameObject.tag == "Boundary")
                {
                    /* Handle Particles and Sound */
                    return true;
                }
            }
        }
        return false;
    }

    private void PlayRandomJumpSound()
    {
        int jumpAssetChoice = Random.Range(1, 9);
        string jumpAssetToUse = "Jump" + jumpAssetChoice.ToString();
        FindObjectOfType<AudioManager>().PlaySFX(jumpAssetToUse);
    }
}
