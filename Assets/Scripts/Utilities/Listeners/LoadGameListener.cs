using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameListener : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadGameButton[] loadGameButtons = Resources.FindObjectsOfTypeAll<LoadGameButton>();
        foreach (LoadGameButton loadGameButton in loadGameButtons)
        {
            loadGameButton.OnLoadGameButtonClick += OnLoadGame;
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        LoadGameButton[] loadGameButtons = Resources.FindObjectsOfTypeAll<LoadGameButton>();
        foreach (LoadGameButton loadGameButton in loadGameButtons)
        {
            loadGameButton.OnLoadGameButtonClick += OnLoadGame;
        }
    }

    private void OnLoadGame(int fileNumber)
    {
        if (DataManager.Instance.GetFilePlayTime(fileNumber) > 0) { DataManager.Instance.LoadGame(fileNumber); }
    }
}
