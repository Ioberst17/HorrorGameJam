using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuHandler_Mason : MonoBehaviour
{

    public GameObject pauseMenu;
    public GameObject controlsMenu;

    [SerializeField]
    private GameObject quitButton;
    public GameController gameController;

    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
    private void LateUpdate()
    {
        if (!gameController.isPaused && pauseMenu.activeSelf)
        {
            Debug.Log("Pause trigger");
            Resume();
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
        if (gameController.isPaused) { Resume(); }
        else { Pause(); }
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        quitButton.SetActive(false);
        //Time.timeScale = 0f;
    }

    public void LoadControls()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void LoadPauseMenu()
    {
        pauseMenu.SetActive(true);
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

