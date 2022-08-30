using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Script : MonoBehaviour
{
    private bool playerDetected;
    [SerializeField] Transform posToGo;
    GameObject playerGo;


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
                playerGo.transform.position = posToGo.position;
                playerDetected = false;
            }
        }
    }

    // These methods should be added to the EventManager system as "onDoorTransition"
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetected = true;
            playerGo = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetected = false;
            playerGo = collision.gameObject;
        }
    }
}
