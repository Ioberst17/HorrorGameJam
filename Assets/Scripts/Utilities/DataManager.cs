using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System.Collections.Generic;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearData();
        }
    }

    [System.Serializable]
    public class SessionData
    {
        // class attributes of TBD session data
        // e.g. public int hitPoints;
    }

    [System.Serializable] // called to make class serializable i.e. turn from an object to bytes for storage; eventually, deserialized when used again (from bytes back to object)
    public class GameData // data to be saved between sessions in Json format - n.b Unity Json utility does not support arrays
    {
        public int timesPlayed = 0;
        // Player Data
        public float playerEXP = 0.0f;
        public int playerLevel = 1;
        // Inventory Data
        [SerializeField]
        public List<Consumables> consumables = new List<Consumables>();
        [SerializeField]
        public List<PlayerWeapons> primaryWeapons = new List<PlayerWeapons>();
        [SerializeField]
        public List<PlayerWeapons> secondaryWeapons = new List<PlayerWeapons>();
        public int activePrimaryWeapon = 1;
        public int activeSecondaryWeapon = 1; 
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

        gameData = LoadData();
        gameData.timesPlayed++;
    }

    public void ClearData()
    {
        GameData gameData = new GameData();
        SaveData(gameData);
        Instance.gameData = LoadData();
    }

    private void SaveData(GameData gameData) // used to save data to a file
    {
        //string json = JsonUtility.ToJson(gameData); // turns data into a json string
        JSchema schemaToUse = GameDataSchema();
        string json = JsonConvert.SerializeObject(gameData);

        //string json = EditorJsonUtility.ToJson(gameData);

        //var options = new JsonSerializerOptions { WriteIndented = true };

        /*JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new JavaScriptDateTimeConverter());
        serializer.NullValueHandling = NullValueHandling.Ignore;*/

        //string json = JsonSerializer.Serialize(gameData, options);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json); // uses System.IO namespace to write to a consistent folder, with name savefile.json

        Debug.Log(json);
    }

    private GameData LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json"; // writes a string with the path file to check
        if (File.Exists(path)) // check if file exists
        {
            string json = File.ReadAllText(path); // reads file content to json string
            //GameData gameData = JsonUtility.FromJson<GameData>(json); // reads json string data to variable data

            gameData = JsonConvert.DeserializeObject<GameData>(json);
            //gameData = JsonSerializer.Deserialize<GameData>(json);

            return gameData;
        }
        else
        {
            GameData gameData = new GameData();
            return gameData;
        }
    }

    private JSchema GameDataSchema()
    {
        JSchemaGenerator generator = new JSchemaGenerator();

        JSchema schema = generator.Generate(typeof(GameData));

        return schema;
    }

    private void OnApplicationQuit()
    {
        SaveData(gameData);
    }
}