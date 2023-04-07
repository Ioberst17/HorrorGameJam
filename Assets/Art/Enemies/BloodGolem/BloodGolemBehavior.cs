using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodGolemBehavior : MonoBehaviour
{
    private Vector2 newVelocity;
    EnemyController enemyController;
    public int ShotCountdown;
    public int ChargeCountdown;
    public int Shotvalue;
    public bool bloodballActive;
    public bool chargeInterupt;
    [SerializeField] private Transform Bloodball;
    //[SerializeField] private Rigidbody2D BloodBallrb;
    [SerializeField] private Transform Bloodballstart;
    public int IDNumber;
    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyController.isAttacking = true;
        enemyController.knockbackForce = 0;
        ShotCountdown = Shotvalue;
        bloodballActive = false;
        chargeInterupt = false;
        //Bloodballstart = Bloodball.transform.position;
        IDNumber = Random.Range(0, 10000);
        name += IDNumber;
        ChargeCountdown = 15;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GolemPassover()
    {
        if (!bloodballActive)
        {
            if (enemyController.playerInZone && !enemyController.damageInterupt)
            {
                ShotCountdown--;
                //Debug.Log(ShotCountdown);
                if (ShotCountdown <= 0)
                {
                    if (ChargeCountdown == 15)
                    {
                        //StartCoroutine(BloodballCharge());
                        ChargeCountdown--;
                    }
                    else
                    {
                        ChargeCountdown--;   
                    }
                    if(ChargeCountdown == 0 && !enemyController.damageInterupt)
                    {
                        Fire();
                    }
                }
            }
            else
            {
                ShotCountdown = Shotvalue;
                ChargeCountdown = 15;
                enemyController.damageInterupt = false;
                chargeInterupt = true;
                
            }
        }
        else
        {
            ShotCountdown = Shotvalue;
            ChargeCountdown = 15;
        }

    }
    private void Fire()
    {
        Vector3 tempVector = new Vector3(enemyController.playerLocation.position.x, enemyController.playerLocation.position.y + 1, enemyController.playerLocation.position.z);
        tempVector = (tempVector - transform.position).normalized;
        //Debug.Log(tempVector);
        Transform BloodBallObject = Instantiate(Bloodball, Bloodballstart.position, Quaternion.identity);
        //Debug.Log(Bloodballstart.position);
        bloodballActive = true;
        enemyController.isAttacking = true;
        //Debug.Log("Firing laser");
        //Debug.Log(transform.position + " " + enemyController.playerLocation.position);
       
        Debug.DrawLine(transform.position, enemyController.playerLocation.position, Color.blue, 100f);
        BloodBallObject.GetComponent<BloodGolemProjectile>().Setup(tempVector, this.name);
        
    }
    public void BloodBallReset()
    {
        bloodballActive = false;
    }
    IEnumerator BloodballCharge()
    {
        
        while (ChargeCountdown > 1)
        {
            yield return new WaitForSeconds(0.01f);
        }
        if (chargeInterupt)
        {
            Vector3 tempVector = new Vector3(enemyController.playerLocation.position.x, enemyController.playerLocation.position.y + 1, enemyController.playerLocation.position.z);
            tempVector = (tempVector - transform.position).normalized;
            //Debug.Log(tempVector);
            Transform BloodBallObject = Instantiate(Bloodball, Bloodballstart.position, Quaternion.identity);
            //Debug.Log(Bloodballstart.position);
            bloodballActive = true;
            enemyController.isAttacking = true;
            //Debug.Log("Firing laser");
            //Debug.Log(transform.position + " " + enemyController.playerLocation.position);

            Debug.DrawLine(transform.position, enemyController.playerLocation.position, Color.blue, 100f);
            BloodBallObject.GetComponent<BloodGolemProjectile>().Setup(tempVector, this.name);
        }
        chargeInterupt = false;
        yield return null;
    }
}
