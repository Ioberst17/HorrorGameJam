using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    DataManager dataManager;
    AudioManager audioManager;

    // play times to update
    TextMeshProUGUI file1PlayTime;
    TextMeshProUGUI file2PlayTime;
    Image file1Button;
    Image file2Button;
    List<UIOscillate> oscillatableFileLoadButtons = new List<UIOscillate>();

    // Start is called before the first frame update
    void Start()
    {
        dataManager = DataManager.Instance;
        audioManager = FindObjectOfType<AudioManager>();
        UpdateLoadButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateLoadButtons()
    {
        // Get references
        file1Button = ComponentFinder.FindComponent<Image>("File1Button");
        file2Button = ComponentFinder.FindComponent<Image>("File2Button");

        oscillatableFileLoadButtons.Add(file1Button.GetComponent<UIOscillate>()); 
        oscillatableFileLoadButtons.Add(file2Button.GetComponent<UIOscillate>());

        file1PlayTime = ComponentFinder.FindComponent<TextMeshProUGUI>("File1PlayTimeNumber");
        file2PlayTime = ComponentFinder.FindComponent<TextMeshProUGUI>("File2PlayTimeNumber");

        // make changes
        file1PlayTime.text = dataManager.GetFilePlayTimePrettyPrint(1);
        file2PlayTime.text = dataManager.GetFilePlayTimePrettyPrint(2);

        if(dataManager.GetFilePlayTime(1) == 0) { file1Button.color = new Color(.57f, .57f, .57f, 1f); }
        if(dataManager.GetFilePlayTime(2) == 0) { file2Button.color = new Color(.57f, .57f, .57f, 1f); }
    }

    private void HandleLoadGameFailFX(int fileNumber)
    {
        if (DataManager.Instance.GetFilePlayTime(fileNumber) == 0) 
        { 
            audioManager.PlaySFX("InsufficientStamina");
            oscillatableFileLoadButtons[fileNumber - 1].hasBeenTriggered = true;        
        }
    }

    // SUBSCRIBE / UNSUBSCRIBE FROM IMPORTANT EVENTS

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; SceneManager.sceneUnloaded += OnSceneUnloaded; }

    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; SceneManager.sceneUnloaded -= OnSceneUnloaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadGameButton[] loadGameButtons = Resources.FindObjectsOfTypeAll<LoadGameButton>();
        foreach (LoadGameButton loadGameButton in loadGameButtons)
        {
            loadGameButton.OnLoadGameButtonClick += HandleLoadGameFailFX;
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        LoadGameButton[] loadGameButtons = Resources.FindObjectsOfTypeAll<LoadGameButton>();
        foreach (LoadGameButton loadGameButton in loadGameButtons)
        {
            loadGameButton.OnLoadGameButtonClick -= HandleLoadGameFailFX;
        }
    }
}
