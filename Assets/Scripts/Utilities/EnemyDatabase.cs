using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

public class EnemyDatabase : MonoBehaviour
{
    // must be attached to a game object in the scene hierarchy
    // creates an enemy database (the store of enemies and information about them), and reads it from the enemyDatbase.csv in /Resources

    private TextAsset textAssetData; // the CSV to read from, must be assigned in Inspector
    private EnemyData enemyChecker;

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

    void ReadCSV() // adds database to the enemyDatabase game object that should be in scene, from a csv file
    {
        List<Dictionary<string, string>> testData = CSVReader.Read("TextFiles/EnemyDatabase");

        enemyDatabase.entries = new EnemyData[testData.Count];

        for (int i = 0; i < testData.Count; i++) 
        {
            enemyDatabase.entries[i] = new EnemyData(); // creates new row entry in memory

            FieldInfo[] fields = enemyDatabase.entries[i].GetType().GetFields(); // used to validate the data type of fields that need to be read in (below)

            for (int j = 0; j < fields.Length; j++)
            {
                if(typeof(int) == fields[j].FieldType) { // if the variable that's being loaded is meant to be an INT, use the below
                    try { fields[j].SetValue(enemyDatabase.entries[i], int.Parse(testData[i].Values.ElementAt(j))); }
                    catch { fields[j].SetValue(enemyDatabase.entries[i], 0); } // if can't parse the value set this default value*/
                }
                else if (typeof(bool) == fields[j].FieldType) // else, if it's meant to be a bool, use the below to parse in data
                {
                    try { fields[j].SetValue(enemyDatabase.entries[i], bool.Parse(testData[i].Values.ElementAt(j))); }
                    catch { fields[j].SetValue(enemyDatabase.entries[i], false); } // if can't parse the value set this default value*/
                }
                else if(typeof(string) == fields[j].FieldType){ // selse if it's a string...
                    try { fields[j].SetValue(enemyDatabase.entries[i], testData[i].Values.ElementAt(j)); }
                    catch { fields[j].SetValue(enemyDatabase.entries[i], "No Value"); } // if can't parse the value set this default value*/
                }
            }
        }
    }

    private EnemyData GetEnemy(string nameNoSpace) 
    {
        enemyChecker = Array.Find(enemyDatabase.entries, x => x.nameNoSpace == nameNoSpace); //enemyDatabase.entries.Find(x => x.nameNoSpace == nameNoSpace);
        if(enemyChecker == null) { Debug.LogFormat("Enemy data requested in 'name' was not properly input, no match was found in EnemyDatabase. Check the input and database names"); }
        return enemyChecker; 
    }

    private int GetAttackDamage(EnemyData enemy, int attackNumber)
    {
        if (attackNumber == 1) { return enemy.attack1Damage; }
        else if (attackNumber == 2) { return enemy.attack2Damage; }
        else if (attackNumber == 3) { return enemy.attack3Damage; }
        else if (attackNumber == 4) { return enemy.attack4Damage; }
        else if (attackNumber == 5) { return enemy.attack5Damage; }
        else if (attackNumber == 6) { return enemy.attack6Damage; }

        Debug.Log("No attackNumber matches the number listed in EnemyDatabase.cs");
        return -1;
    }

    public int GetAttackDamage(string enemyName, int attackNumber) 
    {
        return GetAttackDamage(GetEnemy(enemyName), attackNumber);
    }
}
