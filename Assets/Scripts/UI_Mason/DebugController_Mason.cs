using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DebugController_Mason : MonoBehaviour
{
    //track console on/off
    bool showConsole;

    //create a string to hold user input
    string input;

    //create the commandlist options
    public static DebugCommands_Mason INVINCIBLE;
    public static DebugCommands_Mason SPBOOST;
    public static DebugCommands_Mason MPBOOST;
    public static DebugCommands_Mason RESTART;

    //create the commandlist
    public List<object> commandList;

    //create refrences to needed scripts
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SceneManagerIndexBased_Mason sceneManagerIndexBased_Mason;

    [SerializeField] private DebugCommandBase debugCommandBase;
    [SerializeField] private DebugCommands_Mason debugCommands;

    // Update is called once per frame
    void Update()
    {
        //check for the '`' key to be hit
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            showConsole = !showConsole; //if the player hits '`' turn console on/off.
        }

        if((Input.GetKeyUp(KeyCode.Return)) && (showConsole)) //check for the enter key when the console is up
        {
            HandleInput(); //if so run the handleinput function that runs through the commandlist and looks for a match to run its action.
            input = ""; //reset input to empty
        }
    }


    private void Awake()
    {
        /*KILL_ALL = new DebugCommands_Mason("kill_all", "Removes all enemies from the scene.", "kill_all", () =>
        {
            //need to find or make code that holds enemy data and then create a function to destroy them all then refrence here.
        });*/


        //this is where you create the entries for the commandList

        INVINCIBLE = new DebugCommands_Mason("invincible", "Player doesn't take damage.", "invincible", () =>
        {
            playerController.isInvincible = true;
        });

        RESTART = new DebugCommands_Mason("restart", "Restarts the scene.", "restart", () =>
        {
            sceneManagerIndexBased_Mason.ReloadScene();
        });

        MPBOOST = new DebugCommands_Mason("mpboost", "Adds 50 mp.", "mpboost", () =>
        {
            playerController.MP += 50;
        });

        SPBOOST = new DebugCommands_Mason("spboost", "Adds 50 sp.", "spboost", () =>
        {
            playerController.gainSP(50);
        });

        
        //put the above commands into the list. add a comma before each new entry
        commandList = new List<object> 
        {
            INVINCIBLE,
            RESTART,
            MPBOOST,
            SPBOOST
        };

    }

    //create the ui console
    private void OnGUI()
    {
        if(!showConsole)
        {
            return;
        }

        float y = 0f;

        GUI.Box(new Rect(0, y, Screen.width, 30), "");

        GUI.backgroundColor = new Color(0, 0, 0, 0);

        //gather the user input and insert into the local input string
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
    }

    //loop through the commandlist and check to see if there is a command for the string that was enetered in console
    private void HandleInput()
    {

        for (int i=0; i<commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            input = input.ToLower();

            if (input.Contains(commandBase.commandId))
            {
                if (commandList[i] as DebugCommands_Mason != null)
                {
                    //call invoke from DebugCommands_Mason, essentailly runs the action that is defined above before the commands were eneterd in the list.
                    (commandList[i] as DebugCommands_Mason).Invoke();
                }
            }
        }
    }
}
