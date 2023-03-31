using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatformPlayerDetection : MonoBehaviour
{
    public bool PlayerInZone;
    public OneWayPlatform oneWayPlatform;
    // Start is called before the first frame update
    void Start()
    {
        oneWayPlatform = GetComponentInParent<OneWayPlatform>();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {
            if (!oneWayPlatform.PlayerInZone)
            {
                oneWayPlatform.PlayerInZone = true;
                //Debug.Log(playerInZone);
            }
        }

    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {

            oneWayPlatform.PlayerInZone = false;
            //Debug.Log(playerInZone);
        }

    }
}
