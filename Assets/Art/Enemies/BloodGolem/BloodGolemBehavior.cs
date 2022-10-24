using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodGolemBehavior : MonoBehaviour
{
    private Vector2 newVelocity;
    EnemyController enemyController;
    private int ShotCountdown;
    public int Shotvalue;
    public bool bloodballActive;
    [SerializeField] private Transform Bloodball;
    //[SerializeField] private Rigidbody2D BloodBallrb;
    [SerializeField] private Transform Bloodballstart;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyController.isAttacking = true;
        ShotCountdown = Shotvalue;
        bloodballActive = false;
        //Bloodballstart = Bloodball.transform.position;
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
                Debug.Log(ShotCountdown);
                if (ShotCountdown == 0)
                {
                    Fire();
                }
            }
            else
            {
                ShotCountdown = Shotvalue;
                enemyController.damageInterupt = false;
            }
        }
        else
        {
            ShotCountdown = Shotvalue;
        }

    }
    private void Fire()
    {
        float tempX, tempY;
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
}
