using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PopUpHandler_Mason : MonoBehaviour
{
    //import scenemanagerindex script so we can use ChangeScene method later.
    SceneManagerIndexBased_Mason sceneChanger;
    
    //create empty gameobject variables to hold my popups.
    [SerializeField] private  GameObject popUpDoor;
    [SerializeField] private  GameObject popUpCharacter;
    [SerializeField] private  GameObject popUpItem;

    //create empty game object variable to hold dialogue box.
    [SerializeField] private GameObject dialogueBox;

    //create empty game object variable to hold the item object (this was mainly here to test picking up items)
    //we will need to tweak how we do this later on.
    [SerializeField] private GameObject item;

    //boolean to tell whether we picked up the item or not.
    private bool itemPickup;

    //booleans to tell whether or not we are collididng with specific objects
    //that way we can utilize the collision in the update method.
    private bool doorCollision;
    private bool npcCollision;
    private bool itemCollision;

    //create pointer to scenemanagerindex script so we can access ChangeScene() mehtod.
    void Start()
    {
        //import script methods into sceneChanger var.
        sceneChanger = GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<SceneManagerIndexBased_Mason>();
    }

    //need to check for the player pressing E and utilize the boolean vars to dictate why the player is pressing 'e'.
    void Update()
    {
        if (npcCollision && Input.GetKeyDown(KeyCode.E)) // check to see if the player is colliding with the npc gameobject and the player presses 'e'.
        {
            dialogueBox.SetActive(true); // make the dialogue box appear.
            popUpCharacter.SetActive(false); // make the popup box disappear.
        }
        if (itemCollision && Input.GetKeyDown(KeyCode.E)) // check to see if the player is colliding with the item and presses 'e'
        {
            item.SetActive(false); // turn the item off on the screen, make it disappear
            itemPickup = true; // turn the itempickup boolean to true (we can use this for inventory).
            popUpItem.SetActive(false); // close the popup box for the item.
        }
        if (doorCollision && Input.GetKeyDown(KeyCode.E)) // check to see if the player is colliding with the door and if they are also pressing 'e'.
        {
            sceneChanger.ChangeScene(); // time to utilize the sceneChanger var which is holding access to the scenemanagerindex script, this is how we change scenes!
        }
    }


    //Next is trigger and collision based logic.
    private void OnTriggerEnter2D(Collider2D collision) //when the player collides with something
    {

        if (collision.gameObject.tag == "DoorTrigger") // check to see if the player is colliding with an object with the DoorTrigger tag.
        {
            popUpDoor.SetActive(true); //if so set the door pop up to true showing "Press 'e' to open the door."
        }
        else if (collision.gameObject.tag == "NPCTrigger") // check to see if the player is colliding with an object with the NPCTrigger tag.
        {
            popUpCharacter.SetActive(true); // if so set the npc pop up to true showing "press 'e' to talk to the character."
        }
        else if(collision.gameObject.tag == "ItemTrigger") // check to see if the player is colliding with an object with the ItemTrigger tag.
        {
            popUpItem.SetActive(true); // if so set the item pop up to true showing "press 'e' to pick up the item."
        }
        else if (collision.gameObject.tag == "NPC") // check to seeif the player is colliding with a game object with the tag NPC
        {
            npcCollision = true; // if so set the npcCollision boolean to true, this is used in the update.
            
            //Debug.Log("trigger enter NPC boolean = " + npcCollision + "\n");    I used this to test the logic.
        }
        else if (collision.gameObject.tag == "Item") // check to see if the player is colliding with the game object with the Item Tag
        {
            itemCollision = true; // if so set the itemCollision boolean to true, this is used in the update method.

            //Debug.Log("trigger enter Item boolean = " + itemCollision + "\n");    I used this to test the logic.
        }

    }

    // when the player leaves the trigger area
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "DoorTrigger") //check to see if the colliding object has a DoorTrigger tag.
        {
            popUpDoor.SetActive(false); // if so set the door pop up off.
        }
        else if (collision.gameObject.tag == "NPCTrigger") // check to see if the colliding object has a NPCTrigger tag.
        {
            popUpCharacter.SetActive(false); // if so set the Npc pop up off.
        }
        else if (collision.gameObject.tag == "ItemTrigger") // check to see if the colliding object has a ItemTrigger tag.
        {
            popUpItem.SetActive(false); // if so set the item pop up off.
        }
        else if (collision.gameObject.tag == "NPC") // check to see if the colliding object has a NPC tag.
        {
            npcCollision = false; // if so set the npcCollision boolean to false. this makes it so the player cant press 'e' outside of the npc collision area and open the dialogue box.
        }
        else if (collision.gameObject.tag == "Item") // check to see if the colliding object has a item tag.
        {
            itemCollision = false; // if so set the itemcollision boolean to false. this makes it so the player cant press 'e' outside of the item collision area and interact with the item.
            
            //Debug.Log("trigger exit Item boolean = " + itemCollision + "\n");     i used this to test
        }

    }


    //the door is a but different the door not set to  trigger so it has to be oncollision instead of ontrigger.
    private void OnCollisionEnter2D(Collision2D collision)
    {

        //Debug.Log("made it into collision enter...\n");    used for testing

        if (collision.gameObject.tag == "Door") //check to see if the player is colliding with an object with the Door tag
        {
            doorCollision = true; // if so set the doorCollision boolean to true so we can use it in the update method and change the scene.
        }
        
        //Debug.Log("leaving collision enter...\n");     used for testing.

    }

    //when the player leaves the door collision area
    private void OnCollisionExit2D(Collision2D collision)
    {
        
        //Debug.Log("entering collision exit...\n");    used for testing.

        if (collision.gameObject.tag == "Door") //check to see if the colliding object has the Door tag
        {
            doorCollision = false; // if so we want to turn the boolean false so the player cant/wont open the door away from its collision area.
        }

        //Debug.Log("NPC boolean = " + doorCollision);
        //Debug.Log("leaving collision exit...\n");     more testing debug logs.

    }

    //a simple method so the player can close the dialogue box. this method is attatched to a button on the dialogue box.
    public void CloseDialogue()
    {
        dialogueBox.SetActive(false); // turns off the dialogue box.
    }

}
