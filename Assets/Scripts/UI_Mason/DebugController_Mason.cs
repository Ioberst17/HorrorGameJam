using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class DebugController_Mason : MonoBehaviour
{
    //track console on/off
    bool showConsole;

    bool showHelp;

    public static bool gameIsPaused = false;

    //create a string to hold user input
    string input;

    //create the commandlist options
    public static DebugCommands_Mason INVINCIBLE;
    public static DebugCommands_Mason SPBOOST;
    public static DebugCommands_Mason MPBOOST;
    public static DebugCommands_Mason RESTART;
    public static DebugCommands_Mason HELP;

    public static DebugCommands_Mason<int> SPAWN_ENEMY;

    //create the commandlist
    public List<object> commandList;

    //create refrences to needed scripts
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private SceneManagerIndexBased_Mason sceneManagerIndexBased_Mason;
    [SerializeField] private EnemyCreationForTesting enemySpawner;

    [SerializeField] private DebugCommandBase debugCommandBase;
    [SerializeField] private DebugCommands_Mason debugCommands;

    // Update is called once per frame
    //void Update()
    //{
    //    //check for the '`' key to be hit
    //    if(Input.GetKeyDown(KeyCode.BackQuote))
    //    {
    //        if (gameIsPaused)
    //        {
    //            Resume();
    //        }
    //        else
    //        {
    //            Pause();
    //        }

    //        showConsole = !showConsole; //if the player hits '`' turn console on/off.
    //    }

    //    if((Input.GetKeyUp(KeyCode.Return)) && (showConsole)) //check for the enter key when the console is up
    //    {
    //        HandleInput(); //if so run the handleinput function that runs through the commandlist and looks for a match to run its action.

    //        input.ToLower();

    //        if (input.Contains("restart"))
    //        {
    //            Resume();
    //        }

    //        input = ""; //reset input to empty
    //    }
    //}
    private void Start()
    {
        
    }

    public void ToggleDebugConsole()
    {
        if (gameIsPaused) { Resume(); }
        else { Pause(); }

        showConsole = !showConsole; //if the player hits '`' turn console on/off.
    }

    public void EnterInput()
    {
        HandleInput(); //if so run the handleinput function that runs through the commandlist and looks for a match to run its action.

        input.ToLower();

        if (input.Contains("restart")) { Resume();}

        input = ""; //reset input to empty
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
            playerHealth.IsInvincible = true;
        });

        RESTART = new DebugCommands_Mason("restart", "Restarts the scene.", "restart", () =>
        {
            sceneManagerIndexBased_Mason.ReloadScene();
        });

        MPBOOST = new DebugCommands_Mason("mpboost", "Adds 50 mp.", "mpboost", () =>
        {
            //playerController.MP += 50;
        });

        SPBOOST = new DebugCommands_Mason("spboost", "Adds 50 sp.", "spboost", () =>
        {
            //playerController.gainSP(50);
        });

        SPAWN_ENEMY = new DebugCommands_Mason<int>("spawn_enemy", "Spawns an enemy based on its idNum.", "spawn_enemy <idNum>", (x) =>
        {
            enemySpawner.SpawnEnemy(x);
        });

        HELP = new DebugCommands_Mason("help", "Show list of commands.", "help", () =>
        {
            showHelp = true;
        });

        

        //put the above commands into the list. add a comma before each new entry
        commandList = new List<object> 
        {
            INVINCIBLE,
            RESTART,
            MPBOOST,
            SPBOOST,
            SPAWN_ENEMY,
            HELP
            
        };

    }


    Vector2 scroll;

    //create the ui console
    private void OnGUI()
    {
        if(!showConsole)
        {
            return;
        }

        float y = 0f;

        if (showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");

            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);

            scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), scroll, viewport);

            for (int i=0; i<commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;

                string label = $"{command.commandFormat} - {command.commandDescription}";

                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);

                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();

            y += 100;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");

        GUI.backgroundColor = new Color(0, 0, 0, 0);

        //gather the user input and insert into the local input string
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
    }

    //loop through the commandlist and check to see if there is a command for the string that was enetered in console
    private void HandleInput()
    {

        string[] properties = input.Split(' ');

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
                else if(commandList[i] as DebugCommands_Mason<int> != null)
                {
                    (commandList[i] as DebugCommands_Mason<int>).Invoke(int.Parse(properties[1]));
                }
            }
            else
            {

            }
        }
    }


    public void Resume()
    {
        Time.timeScale = 1.0f;
        gameIsPaused = false;
    }

    void Pause()
    {
        Time.timeScale = 0f;
        gameIsPaused = true;
    }
}
