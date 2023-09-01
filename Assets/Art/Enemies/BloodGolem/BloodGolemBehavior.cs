using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodGolemBehavior : EnemyBehaviour
{
    public int ChargeCountdown;
    public bool chargeInterupt;
    public int ShotCountdown;
    public int ShotValue;

    [SerializeField] private GameObject particleEffect;
    public int IDNumber;

    override protected void Start()
    {
        base.Start();

        ShotCountdown = ShotValue;
        ChargeCountdown = 15;
    }

    override protected void Passover()
    {
        if (enemyController.playerInZone && !enemyHealth.DamageInterrupt)
        {
            if (!enemyController.IsAttackingOrChargingAttack)
            {
                StartCoroutine(BloodBallCharge());
                projectileManager.Shoot(projectileManager.projectilesToUse[0],
                                                enemyController.playerLocation.position);
            }
        }
        FlipToFacePlayer();
    }

    IEnumerator BloodBallCharge()
    {
        particleEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        particleEffect.SetActive(false);
        yield return null;
    }
}
