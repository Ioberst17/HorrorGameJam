using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.ComponentModel.Design;

public class SceneHandler_Mason : MonoBehaviour
{
    //variables for the menu items, assigned in inspector
    public GameObject mainMenuScreen;
    public GameObject loadGameScreen;
    public GameObject controlsScreen;
    public GameObject optionsScreen;
    

    //open main scene
    public void StartGame()
    {
        //load a scene by the scene name
        SceneManager.LoadScene("GameShell");
    }

    //quit the .exe
    public void QuitGame()
    {
        //print "quit game" to the console.
        Debug.Log("Quit Game.");

        //quit the .exe
        Application.Quit();
    }

    //opens controls menu
    public void OpenControls()
    {
        //set the mainmenu canvas to off
        mainMenuScreen.SetActive(false);

        //set the controlscreen canvas to on
        controlsScreen.SetActive(true);
    }

    //closes controls menu
    public void CloseControls()
    {
        //set the mainmenu canvas to on
        mainMenuScreen.SetActive(true);

        //set the controlscreen canvas to off
        controlsScreen.SetActive(false);
    }

    public void CloseLoadGameOptions() { mainMenuScreen.SetActive(true); loadGameScreen.SetActive(false); }
    public void OpenLoadGameOptions() { mainMenuScreen.SetActive(false); loadGameScreen.SetActive(true); }    
    public void CloseOptionsMenu() { mainMenuScreen.SetActive(true); optionsScreen.SetActive(false); }
    public void OpenOptionsMenu() { mainMenuScreen.SetActive(false); optionsScreen.SetActive(true); }
}


