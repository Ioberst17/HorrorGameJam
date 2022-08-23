using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyDetectionBehavior : MonoBehaviour
{

    //[SerializeField]
    private EnemyController enemyController;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {
            if (!enemyController.playerInZone)
            {
                enemyController.playerInZone = true;
                //Debug.Log(playerInZone);
            }
        }

    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {

            enemyController.playerInZone = false;
            //Debug.Log(playerInZone);
        }

    }
}
