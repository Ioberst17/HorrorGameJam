using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class DataManager : MonoBehaviour
{
    /* DECLARATIONS */

    public static DataManager Instance;
    // made to be referencable by any other script - it is static, i.e. every instance has the same values
    // there is a "get" accessor to make values read only, and "private set" to write within class
    // i.e. there is a "get" (read-access), but not a "set" (write-access) except for "private set" (write-access within it's class)
    // this is also known as Singleton structure

    public SessionData seshData = new SessionData(); // creates a new instance of class SessionData
    public GameData gameData = new GameData(); // creates a new instance of class GameData

    /* CLASS STRUCTURES */



    [System.Serializable]
    public class SessionData
    {
        // class attributes of TBD session data
        // e.g. public int hitPoints;
    }

    [System.Serializable] // called to make class serializable i.e. turn from an object to bytes for storage; eventually, deserialized when used again (from bytes back to object)
    public class GameData // data to be saved between sessions in Json format - n.b Unity Json utility does not support arrays
    {
        public int timesPlayed = 1;
        // Player Data
        public float playerEXP = 0.0f;
        public int playerLevel = 1;
        // Inventory Data
        public List<PlayerWeapons> inventory = new List<PlayerWeapons>();
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

    private void Start()
    {
        gameData = LoadData();
    }

    public void ClearData()
    {
        GameData gameData = new GameData();
        SaveData(gameData);
        Instance.gameData = LoadData();
    }

    private void SaveData(GameData gameData) // used to save data to a file
    {
        string json = JsonUtility.ToJson(gameData); // turns data into a json string

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json); // uses System.IO namespace to write to a consistent folder, with name savefile.json
        Debug.Log(json);
    }

    private GameData LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json"; // writes a string with the path file to check
        if (File.Exists(path)) // check if file exists
        {
            string json = File.ReadAllText(path); // reads file content to json string
            GameData gameData = JsonUtility.FromJson<GameData>(json); // reads json string data to variable data
            Debug.Log(gameData);
            return gameData;
        }
        else
        {
            GameData gameData = new GameData();
            Debug.Log(gameData);
            return gameData;
        }
    }

    private void OnApplicationQuit()
    {
        SaveData(gameData);
    }
}