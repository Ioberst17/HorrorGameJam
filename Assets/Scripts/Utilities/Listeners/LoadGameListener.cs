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
        if (SceneManager.GetActiveScene().buildIndex == 0) // only due this on the title screen
        {
            LoadGameButton[] loadGameButtons = Resources.FindObjectsOfTypeAll<LoadGameButton>(); // find all objects LoadGameButton.cs
            foreach (LoadGameButton loadGameButton in loadGameButtons) { loadGameButton.OnLoadGameButtonClick += OnLoadGame; } // subscribe to listener

            LoadCombatModeButton[] combatModeButtons = Resources.FindObjectsOfTypeAll<LoadCombatModeButton>();  //repeat for load combat mode
            foreach (LoadCombatModeButton combatModeButton in combatModeButtons) { combatModeButton.OnLoadCombatModeClick += OnCombatModeOpen; }
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) // only due this on the title screen
        {
            LoadGameButton[] loadGameButtons = Resources.FindObjectsOfTypeAll<LoadGameButton>(); // find all objects LoadGameButton.cs
            foreach (LoadGameButton loadGameButton in loadGameButtons) { loadGameButton.OnLoadGameButtonClick -= OnLoadGame; } // subscribe to listener

            LoadCombatModeButton[] combatModeButtons = Resources.FindObjectsOfTypeAll<LoadCombatModeButton>();  //repeat for load combat mode
            foreach (LoadCombatModeButton combatModeButton in combatModeButtons) { combatModeButton.OnLoadCombatModeClick -= OnCombatModeOpen; }
        }
    }

    private void OnLoadGame(int fileNumber)
    {
        if (DataManager.Instance.GetFilePlayTime(fileNumber) > 0) { DataManager.Instance.LoadGame(fileNumber); }
    }

    private void OnCombatModeOpen()
    {
        SceneManager.LoadScene("CombatMode");
    }
}
