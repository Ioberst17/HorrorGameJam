using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public class EnemyDatabase : MonoBehaviour
{
    // must be attached to a game object in the scene hierarchy
    // creates an enemy database (the store of enemies and information about them), and reads it from the enemyDatbase.csv in /Resources

    private TextAsset textAssetData; // the CSV to read from, must be assigned in Inspector

    [System.Serializable]
    public class Database // create the a database of all game items
    {
        public EnemyData[] entries;
    }

    public Database enemyDatabase = new Database();

    private void Awake() // load CSV from Resources folder then add it to the Weapon Database game object
    {
        ReadCSV();
    }

    void ReadCSV() // adds database to the weaponDatabase game object that should be in scene, from a csv file
    {
        List<Dictionary<string, string>> testData = CSVReader.Read("TextFiles/EnemyDatabase");

        enemyDatabase.entries = new EnemyData[testData.Count];

        for (int i = 0; i < testData.Count; i++) 
        {
            enemyDatabase.entries[i] = new EnemyData(); // creates new row entry in memory

            FieldInfo[] fields = enemyDatabase.entries[i].GetType().GetFields(); // stores an array of a data row of the database

            for (int j = 0; j < fields.Length; j++)
            {
                if(typeof(int) == fields[j].FieldType) { // if the variable that's being loaded is meant to be an INT, use the below
                    try { fields[j].SetValue(enemyDatabase.entries[i], int.Parse(testData[i].Values.ElementAt(j))); }
                    catch { fields[j].SetValue(enemyDatabase.entries[i], 0); } // if can't parse the value set this default value*/
                }
                else if (typeof(bool) == fields[j].FieldType)
                {
                    try { fields[j].SetValue(enemyDatabase.entries[i], bool.Parse(testData[i].Values.ElementAt(j))); }
                    catch { fields[j].SetValue(enemyDatabase.entries[i], false); } // if can't parse the value set this default value*/
                }
                else if(typeof(string) == fields[j].FieldType){
                    try { fields[j].SetValue(enemyDatabase.entries[i], testData[i].Values.ElementAt(j)); }
                    catch { fields[j].SetValue(enemyDatabase.entries[i], "No Value"); } // if can't parse the value set this default value*/
                }
            }
        }
    }
}
