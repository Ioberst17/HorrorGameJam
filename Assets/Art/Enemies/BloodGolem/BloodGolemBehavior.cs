using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodGolemBehavior : MonoBehaviour
{
    private Vector2 newVelocity;
    EnemyController enemyController;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyController.isAttacking = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GolemPassover()
    {

    }
}
