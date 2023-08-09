using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodGolemBehavior : EnemyAttackBehavior
{
    public int ChargeCountdown;
    public bool chargeInterupt;
    public int ShotCountdown;
    public int Shotvalue;
    public bool bloodballActive;
    [SerializeField] private Transform Bloodball;
    [SerializeField] private Transform Bloodballstart;

    [SerializeField] private GameObject particleEffect;
    public int IDNumber;

    override protected void Start()
    {
        base.Start();

        ShotCountdown = Shotvalue;
        IDNumber = Random.Range(0, 10000);
        name += IDNumber;
        ChargeCountdown = 15;
    }

    override protected void Passover()
    {
        if (!bloodballActive)
        {
            if (enemyController.playerInZone && !enemyHealth.damageInterupt)
            {
                ShotCountdown--;
                if (ShotCountdown <= 0)
                {
                    if (ChargeCountdown == 15)
                    {
                        StartCoroutine(BloodballCharge());
                        ChargeCountdown--;
                    }
                    else
                    {
                        ChargeCountdown--;   
                    }
                    if(ChargeCountdown == 0 && !enemyHealth.damageInterupt) { Fire(); }
                }
            }
            else
            {
                ShotCountdown = Shotvalue;
                ChargeCountdown = 60;
                enemyHealth.damageInterupt = false;
                chargeInterupt = true;
            }
        }
        else
        {
            ShotCountdown = Shotvalue;
            ChargeCountdown = 60;
        }

        FlipToFacePlayer();

    }
    private void Fire()
    {
        Vector3 tempVector = new Vector3(enemyController.playerLocation.position.x, enemyController.playerLocation.position.y + 1, enemyController.playerLocation.position.z);
        tempVector = (tempVector - transform.position).normalized;
        //Debug.Log(tempVector);
        Transform BloodBallObject = Instantiate(Bloodball, Bloodballstart.position, Quaternion.identity);
        //Debug.Log(Bloodballstart.position);
        bloodballActive = true;
        enemyController.IsAttacking = true;
        //Debug.Log("Firing laser");
        //Debug.Log(transform.position + " " + enemyController.playerLocation.position);
       
        Debug.DrawLine(transform.position, enemyController.playerLocation.position, Color.blue, 100f);
        BloodBallObject.GetComponent<BloodGolemProjectile>().Setup(tempVector, this.name);
        
    }
    public void BloodBallReset() { bloodballActive = false; }

    IEnumerator BloodballCharge()
    {
        particleEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        particleEffect.SetActive(false);
        yield return null;
    }
}
