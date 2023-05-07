using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine.SceneManagement;
using System;

public class DataManager : MonoBehaviour
{
    /* DECLARATIONS */

    public static DataManager Instance;
    // made to be referencable by any other script - it is static, i.e. every instance has the same values
    // there is a "get" accessor to make values read only, and "private set" to write within class
    // i.e. there is a "get" (read-access), but not a "set" (write-access) except for "private set" (write-access within it's class)
    // this is also known as Singleton structure

    public GameData sessionData = new GameData(); // creates a new instance of class SessionData
    public GameData gameData = new GameData(); // creates a new instance of class GameData
    public GameData tempGameData = new GameData();

    [SerializeField] int currentFile;
    int currentSceneIndex = 0;
    bool loadedGameFromFile;

    /* CLASS STRUCTURES */

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update() 
    { 
        //if (Input.GetKeyDown(KeyCode.C)) { ClearData(); } 
        if(currentSceneIndex != 0) { sessionData.timePlayed += Time.deltaTime; }
    }


    [System.Serializable] // called to make class serializable i.e. turn from an object to bytes for storage; eventually, deserialized when used again (from bytes back to object)
    public class GameData // data to be saved between sessions in Json format - n.b Unity Json utility does not support arrays
    {
        public int timesPlayed = 0;
        public float timePlayed = 0;
        [Header("Player Data")]
        public float playerEXP = 0.0f;
        public int playerLevel = 1;
        public PlayerSkills playerSkills = new PlayerSkills();
        public int currentSceneBuildIndex = 1; // defaults to first scene (non-title)
        public float lastKnownWorldLocationX;
        public float lastKnownWorldLocationY;
        [Header("Inventory Data")]
        public List<NarrativeItems> narrativeItems = new List<NarrativeItems>();
        [SerializeField]
        public List<PlayerConsumables> consumables = new List<PlayerConsumables>();
        [SerializeField]
        public List<PlayerWeapons> primaryWeapons = new List<PlayerWeapons>();
        [SerializeField]
        public List<PlayerWeapons> secondaryWeapons = new List<PlayerWeapons>();
        public int activePrimaryWeapon = 1;
        public int activeSecondaryWeapon = 1;
        [Header("Environment Data")]
        public AreaHistory areaHistory = new AreaHistory();
        [Header("Story Data")]
        public StoryHistory storyHistory = new StoryHistory();
    }

    /* FUNCTIONS */

    void Awake() // manages creation and limiting of GameManager Instances
    {
        // GameManager instance mgmt.
        if (Instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    public void ClearData()
    {
        sessionData = new GameData();
        gameData = new GameData();
        SaveData(1); // defaults to file 1
        Instance.sessionData = LoadData(1);
    }

    public void SaveData(int fileNumber) // used to save data to a file
    {
        foreach (var field in typeof(GameData).GetFields()) { field.SetValue(gameData, field.GetValue(sessionData)); }
        currentFile = fileNumber;

        string json = JsonConvert.SerializeObject(gameData);
        File.WriteAllText(Application.persistentDataPath + "/savefile" + fileNumber.ToString() + ".json", json);
        Debug.Log(json);
    }

    public void SaveData() { if(currentFile != 0 ) SaveData(currentFile); }

    public void LoadGame(int fileNumber)
    {
        loadedGameFromFile = true;
        sessionData = LoadData(fileNumber);
        gameData = LoadData(fileNumber);
        sessionData.timesPlayed++;
        currentFile = fileNumber;
        LoadPlayerInNewScene();
    }

    private GameData LoadData(int fileNumber)
    {
        string path = Application.persistentDataPath + "/savefile" + fileNumber.ToString() + ".json"; // writes a string with the path file to check
        if (File.Exists(path)) { return LoadingCode(path); }
        else
        {
            gameData = new GameData();
            sessionData = new GameData();
            return sessionData;
        }
    }

    private void LoadPlayerInNewScene()
    {
        SceneManager.LoadScene(sessionData.currentSceneBuildIndex);
    }

    private GameData CacheFileData(int fileNumber)
    {
        string path = Application.persistentDataPath + "/savefile" + fileNumber.ToString() + ".json"; // writes a string with the path file to check
        if (File.Exists(path)) { return LoadingCode(path); }
        else { return new GameData(); }
    }

    private GameData LoadingCode(string path)
    {
        string json = File.ReadAllText(path); // reads file content to json string
        return JsonConvert.DeserializeObject<GameData>(json);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        sessionData.currentSceneBuildIndex = currentSceneIndex;
        if (currentFile != 0 ) { EventSystem.current.GameFileLoadedTrigger(gameData); }
    }

    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnApplicationQuit()
    {
        //SaveData(sessionData);
    }

    // GET SPECIFIC DATA

    public bool SeeIfFileHasBeenSavedBefore() { Debug.Log("Time played is greater than 0: " + (gameData.timePlayed > 0.0f).ToString());  return gameData.timePlayed > 0.0f; }

    public float GetFilePlayTime(int fileNumber)
    {
        tempGameData = CacheFileData(fileNumber);
        Debug.Log("File " + fileNumber + " time played is " + tempGameData.timePlayed);
        return tempGameData.timePlayed;
    }

    public string GetFilePlayTimePrettyPrint(int fileNumber)
    {
        return FloatToTimeString(GetFilePlayTime(fileNumber));
    }

    public string FloatToTimeString(float timeInSeconds)
    {
        int hours = Mathf.FloorToInt(timeInSeconds / 3600f);
        int minutes = Mathf.FloorToInt((timeInSeconds % 3600f) / 60f);
        return hours.ToString("0") + ":" + minutes.ToString("00");
    }


}