using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerIndexBased_Mason : MonoBehaviour
{
    // create an scene variable which will refer to the active scene based on its index number
    Scene activeScene;

    // create a static integer to enumerate as we loop through scenes
    static int phaseNum = 1;


    // Start is called before the first frame update
    void Start()
    {

        //set the activeScene var so we can track what scene we are on in the index
        activeScene = SceneManager.GetActiveScene();

        //print the current scene name and index number to the console
        Debug.Log("Active Scene name is: " + activeScene.name + "\nActive Scene index: " + activeScene.buildIndex);
    }

    public void ChangeScene()
    {
        //crude scene managment logic
        //keeps track of scene index number and the phase number
        //creates the loop of "overworld" scene to "phase 1" scene, back to "overworld" scene then goes to "phase 2" scene etc.
        //
        //**IMPORTANT**
        //we will need to change the number we are basing the loop on depending on what index number the "overworld" scene will have.
        //we could create an easier way to change the overworld index number so this loop will be easier to change based on each persons build settings.
        //the phaseNum integer will not need to change from person to person.

        //if the scenes index is higher than index#1 reload scene with index # 1
        //add one to the phasenumber to prime the load to phase 2
        if (activeScene.buildIndex > 1)
        {
            phaseNum++;
            SceneManager.LoadScene(1);
        }
        else if (activeScene.buildIndex <= 1 && phaseNum == 1) //first loop, main menu -> overworld -> phase 1
        {
            SceneManager.LoadScene(activeScene.buildIndex + 1);
        }
        else if (activeScene.buildIndex <= 1 && phaseNum == 2) // second loop, overworld -> phase 2
        {
            SceneManager.LoadScene(activeScene.buildIndex + 2);
        }
        else if (activeScene.buildIndex <= 1 && phaseNum == 3) // third loop, overworld -> phase 3
        {
            SceneManager.LoadScene(activeScene.buildIndex + 3);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
