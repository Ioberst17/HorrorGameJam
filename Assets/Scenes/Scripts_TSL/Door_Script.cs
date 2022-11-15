using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Script : MonoBehaviour
{
    public bool playerDetected;
    [SerializeField] Transform posToGo;
    [SerializeField] Transform player;



    // Start is called before the first frame update
    void Start()
    {
        playerDetected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDetected)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                player.transform.position = posToGo.position;
                playerDetected = false;
            }
        }
    }

    // These methods should be added to the EventManager system as "onDoorTransition"
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {
            playerDetected = true;
            //playerGo = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {
            playerDetected = false;
            //playerGo = collision.gameObject;
        }
    }
}
