using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_PopUpHandler_Mason : MonoBehaviour
{

    //create empty gameobject variables to hold my popups.
    [SerializeField] private GameObject popUpImage;
    [SerializeField] private GameObject collider;

    //[SerializeField] private GameObject colliderOfPopUp;


    // Start is called before the first frame update
    void Start()
    {
        popUpImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Next is trigger and collision based logic.
    private void OnTriggerEnter2D(Collider2D collision) //when the player collides with something
    {
        if (collision.gameObject.tag == "Player") // check to see if the player is colliding with an object with the DoorTrigger tag.
        {
            popUpImage.SetActive(true); //if so set the door pop up to true showing "Press 'e' to open the door."
        }
    }

    // when the player leaves the trigger area
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player") //check to see if the colliding object has a DoorTrigger tag.
        {
            popUpImage.SetActive(false); // if so set the door pop up off.
        }
    }

    }
