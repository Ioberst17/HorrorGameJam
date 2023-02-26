using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuHandler_Mason : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenu;

    public GameObject controlsMenu;

    [SerializeField]
    private GameObject quitButton;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
        Time.timeScale = 1.0f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        quitButton.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
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

