using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // Start is called before the first frame update
    void Start()
    {
        playerPrimaryWeapon = GetComponent<PlayerPrimaryWeapon>();
        groundSlamHitList = new List<Collider2D>();
        groundSlamCollisionFilter = new ContactFilter2D();
        groundSlamCollisionFilter.SetLayerMask((1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("BreakableEnviro")) | 1 << LayerMask.NameToLayer("Environment"));
    }
    public void Execute(Vector2 point1, Vector2 point2)
    {
        //PlayRandomJumpSound();

        groundSlamStop = false; groundSlamCounter = 0;
        while (groundSlamStop == false && groundSlamCounter < 100)
        {
            playerPrimaryWeapon.playerController.SetVelocity(0, groundSlamSpeed);
            groundSlamHitListLength = Physics2D.OverlapArea(point1, point2, groundSlamCollisionFilter, groundSlamHitList);
            if (isFinished(groundSlamHitList)) { groundSlamStop = true; }
            groundSlamCounter++;
        }
        playerPrimaryWeapon.isAttacking = false;
    }

    private bool isFinished(List<Collider2D> groundSlamHits)
    {
        if (groundSlamHits.Count > 0)
        {
            foreach(Collider2D hit in groundSlamHits)
            {
                if(hit.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.gameObject.layer == LayerMask.NameToLayer("BreakableEnviro"))
                {
                    hit.gameObject.GetComponent<IDamageable>().Hit(attackDamage, transform.position);

                    if (hit.gameObject.transform.position.x >= transform.position.x) 
                    { 
                        playerPrimaryWeapon.playerController.AddForce(-reboundForceX, reboundForceY); 
                    }
                    else { playerPrimaryWeapon.playerController.AddForce(reboundForceX, reboundForceY); }

                    /* Handle Particles and Sound */
                }
                else if (hit.gameObject.layer == LayerMask.NameToLayer("Environment") || hit.gameObject.tag == "Boundary") 
                {
                    /* Handle Particles and Sound */
                }

                Instantiate(Resources.Load("VFXPrefabs/GroundSlamImpact"), transform.position, Quaternion.identity);
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
