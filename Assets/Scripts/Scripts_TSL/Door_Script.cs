using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Script : MonoBehaviour
{
    public bool playerDetected;
    public GameObject door;
    [SerializeField] Transform posToGo;
    [SerializeField] Transform player;
    [SerializeField] ObjectiveUI objectiveUI;

    // For Quests
    QuestUpdaterSupport questUpdater;


    // Start is called before the first frame update
    void Start()
    {
        playerDetected = false;
        questUpdater = GetComponent<QuestUpdaterSupport>();
    }

     public void InteractWithPlayer()
    {
        player.transform.position = posToGo.position;
        playerDetected = false;

        if(questUpdater != null) { questUpdater.UpdateQuest(); }

        // call the objectiveUI and update it when specific doors are used
        if (door.name == "Door_House1")
        {
            objectiveUI.UpdateObjectiveUI(2); // This calls ObjectiveUI Update function and for now passes ID2 meaning "Kill all enemies in room"
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
