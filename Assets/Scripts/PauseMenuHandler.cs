using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenu;

    public GameObject controlsMenu;

    public CanvasGroup homeScreen;

    public CanvasGroup jobScreen;

    public CanvasGroup performanceScreen;

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

        if (homeScreen.alpha == 1)
        {
            homeScreen.interactable = true;
        }
        if (jobScreen.alpha == 1)
        {
            jobScreen.interactable = true;
        }
        if (performanceScreen.alpha == 1)
        {
            performanceScreen.interactable = true;
        }
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        if (homeScreen.alpha == 1)
        {
            homeScreen.interactable = false;
        }
        if (jobScreen.alpha == 1)
        {
            jobScreen.interactable = false;
        }
        if (performanceScreen.alpha == 1)
        {
            performanceScreen.interactable = false;
        }

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
        SceneManager.LoadScene("MainMenuExample");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game.");
        Application.Quit();
    }
}
