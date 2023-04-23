using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{

    //player objective ui elements
    //[SerializeField] private Gameobject objectivePanel;
    //[SerializeField] private Image objectiveBulletpoint;
    [SerializeField] private TextMeshProUGUI objectiveTitle;
    [SerializeField] private TextMeshProUGUI objectiveText;
    private int areaNum; // PLACEHOLDER we'll use the roomNum to dictate the objective for the room (ex. kill all enemies, find the door, kill boss, etc.)

    //[SerializeField] public EnemySpawnManager enemySpawnManager;


    //[SerializeField] public EnemySpawnManager enemySpawnManager;


    // Start is called before the first frame update
    void Start()
    {
        areaNum = 1; // using this var to test changing text
        UpdateObjectiveUI(areaNum);
    }

    // Update is called once per frame
    void Update()
    {
        /*TODO: add if that checks if all enemies in room have died and if so run 
         * the UpdateObjectiveUI with a number correlating to the room being cleared*/
        //if (enemySpawnManager.enemiesCleared) { UpdateObjectiveUI(7); }

        // UpdateObjectiveUI(areaNum); // only here for testing, function will mainly be called in script that handles moving rooms, as well as a room clear check inside this update.
    }

    // create a function that takes in the roomNumber and then changes the text of the objective ui based on that
     
     /* need to correlate roomNumbers with objective text 
     * (ex. kill all enemies = roomNumber 1, find the door = roomNumber 2, kill boss = roomNumber 3, etc.)*/

    public void UpdateObjectiveUI(int areaID) //allow this function to be called when the player moves rooms 
    {
        if (areaID == 1)
        {
            objectiveText.text = "Leave the house.";
            // Completed when house1 door is used.
        }
        else if (areaID == 2)
        {
            objectiveText.text = "Speak to townspeople.";
        }
        else if (areaID == 3)
        {
            // This objective will need to be more complex than just going through door.
            // Use if statement if npcsSpokenTo = totalNPCCount, completed objective.
            objectiveText.text = "Go to bed";
        }
        else if (areaID == 4)
        {
            objectiveText.text = "Investigate town.";
            // This will be complete when they go down the sewers.
        }
        else if (areaID == 5)
        {
            objectiveText.text = "Defeat the hellhound.";
            // When hellhound is defeated.
        }
        else if (areaID == 6)
        {
            objectiveText.text = "Explore the storeroom.";
            // This is complete when player breaks crate and reveals cave entrance.
            // If crate == inactive, update.
        }
        else if (areaID == 7)
        {
            objectiveText.text = "Enter the cave.";
            // Completed when player enters the cave.
        }
        else if (areaID == 8)
        {
            objectiveText.text = "Find a way out.";
            // Completed when player enters the town hall basement.
        }
        else if (areaID == 9)
        {
            objectiveText.text = "Defeat the demon.";
            // Complete when gargoyle is killed.
            // Clear objective if run fails (i.e. player dies in this fight).
        }
        // Generic room objectives (like side-quests after tutorial (i.e. go to the shop, buy an item).
        // objectiveText.text = "Kill all enemies!";
        // objectiveText.text = "Room cleared!";
    }
}
