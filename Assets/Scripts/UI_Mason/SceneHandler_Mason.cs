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

    public Button mainMenuButton;
    public Button loadMenuButton;
    public Button controlsMenuButton;
    public Button optionsScreenButton;

    public Dictionary<GameObject, Button> menuToButtonMap = new Dictionary<GameObject, Button>();

    [SerializeField] private string currentControlScheme;

    [SerializeField]
    private GameObject quitButton;
    public GameController gameController;

    private void Start() 
    { 
        gameController = FindObjectOfType<GameController>();
        // Initialize the dictionary with GameObjects and Buttons
        menuToButtonMap.Add(mainMenuScreen, mainMenuButton);
        menuToButtonMap.Add(loadGameScreen, loadMenuButton);
        menuToButtonMap.Add(controlsScreen, controlsMenuButton);
        menuToButtonMap.Add(optionsScreen, optionsScreenButton);
    }

    private void Update() 
    {
        if (gameController.CurrentControlScheme != currentControlScheme)
        {
            currentControlScheme = gameController.CurrentControlScheme; // prevents constant reassignment
            OnControlsChanged();
        }
    }

    void OnControlsChanged() // if player switches to Gamepad, select the top button of a menu
    {
        if (currentControlScheme == "Gamepad") { if (mainMenuScreen.activeSelf) { mainMenuButton.Select(); } }
        if (currentControlScheme == "Gamepad") { if (loadGameScreen.activeSelf) { loadMenuButton.Select(); } }
        if (currentControlScheme == "Gamepad") { if (controlsScreen.activeSelf) { controlsMenuButton.Select(); } }
        if (currentControlScheme == "Gamepad") { if (optionsScreen.activeSelf) { optionsScreenButton.Select(); } }
    }

    void OnMenuChanged()
    {
        OnControlsChanged();
    }

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

        OnMenuChanged();
    }

    //closes controls menu
    public void CloseControls()
    {
        //set the mainmenu canvas to on
        mainMenuScreen.SetActive(true);

        //set the controlscreen canvas to off
        controlsScreen.SetActive(false);

        OnMenuChanged();
    }

    public void CloseLoadGameOptions() { mainMenuScreen.SetActive(true); loadGameScreen.SetActive(false); OnMenuChanged(); }
    public void OpenLoadGameOptions() { mainMenuScreen.SetActive(false); loadGameScreen.SetActive(true); OnMenuChanged(); }    
    public void CloseOptionsMenu() { mainMenuScreen.SetActive(true); optionsScreen.SetActive(false); OnMenuChanged(); }
    public void OpenOptionsMenu() { mainMenuScreen.SetActive(false); optionsScreen.SetActive(true); OnMenuChanged(); }

    void SelectRelevantButton(GameObject menuThatClosed)
    {
        if (currentControlScheme == "Gamepad")
        {
            if (menuToButtonMap.ContainsKey(menuThatClosed))
            {
                // Get the corresponding button and select it
                menuToButtonMap[menuThatClosed].Select();
            }
        }
    }
}


