using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuHandler_Mason : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject controlsMenu;

    public Button pauseMenuFirstButton;
    public Button controlsMenuFirstButton;

    [SerializeField] private string currentControlScheme;

    [SerializeField]
    private GameObject quitButton;
    public GameController gameController;

    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
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
        if (currentControlScheme == "Gamepad") { if (pauseMenu.activeSelf) { pauseMenuFirstButton.Select(); } }
        if (currentControlScheme == "Gamepad") { if (controlsMenu.activeSelf) { controlsMenuFirstButton.Select(); } }
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        currentControlScheme = gameController.CurrentControlScheme;
        if (currentControlScheme == "Gamepad") { pauseMenuFirstButton.Select(); }
        quitButton.SetActive(false);
        //Time.timeScale = 0f;
    }

    public void LoadControls()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(true);
        currentControlScheme = gameController.CurrentControlScheme;
        if (currentControlScheme == "Gamepad") { controlsMenuFirstButton.Select(); }
    }

    public void LoadPauseMenu()
    {
        pauseMenu.SetActive(true);
        currentControlScheme = gameController.CurrentControlScheme;
        if (currentControlScheme == "Gamepad") { pauseMenuFirstButton.Select(); }
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
}

