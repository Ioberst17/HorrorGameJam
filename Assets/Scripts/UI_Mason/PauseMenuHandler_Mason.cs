using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;

public class PauseMenuHandler_Mason : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject controlsMenu;
    public GameObject optionsMenu;

    public Button resumeButton;
    public Button controlsButton;
    public Button settingsButton;
    public Button mainmenuButton;
    public Button quitButton;

    [SerializeField] private string currentControlScheme;

    [SerializeField]
    public GameController gameController;


    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        resumeButton.onClick.AddListener(() => gameController.PauseHandler("Pause")); 
    }
    private void Update()
    {
        //if (!gameController.isPaused && pauseMenu.activeSelf)
        //{
        //    Debug.Log("Pause trigger");
        //    Resume();
        //}
        if (gameController.CurrentControlScheme != currentControlScheme) 
        { 
            currentControlScheme = gameController.CurrentControlScheme; // prevents constant reassignment
            OnControlsChanged();
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
        //Time.timeScale = 1.0f;
    }

    public void TogglePauseUI()
    {
        if (gameController.IsPaused) { Resume(); }
        else { Pause(); }
    }

    void OnControlsChanged() // if player switches to Gamepad, select the first button
    {
        if (currentControlScheme == "Gamepad") { if (pauseMenu.activeSelf) { resumeButton.Select(); } }
        if (currentControlScheme == "Gamepad") { if (controlsMenu.activeSelf) { controlsButton.Select(); } }
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        currentControlScheme = gameController.CurrentControlScheme;
        if (currentControlScheme == "Gamepad") { resumeButton.Select(); }
        //quitButton.SetActive(false);
        //Time.timeScale = 0f;
    }

    public void LoadControls()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(true);
        currentControlScheme = gameController.CurrentControlScheme;
        if (currentControlScheme == "Gamepad") { controlsButton.Select(); }
    }

    public void CloseControlsAndOpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        currentControlScheme = gameController.CurrentControlScheme;
        if (currentControlScheme == "Gamepad") { controlsButton.Select(); }
        controlsMenu.SetActive(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game.");
        Application.Quit();
    }

    public void CloseOptionsMenu() { pauseMenu.SetActive(true); optionsMenu.SetActive(false); }
    public void OpenOptionsMenu() { pauseMenu.SetActive(false); optionsMenu.SetActive(true); }


}

