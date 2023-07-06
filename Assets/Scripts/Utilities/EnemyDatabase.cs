using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

public class EnemyDatabase : Database<EnemyData> 
{
    // must be attached to a game object in the scene hierarchy
    // creates an enemy database (the store of enemies and information about them), and reads it from the enemyDatbase.csv in /Resources

    private EnemyData enemyChecker;

    private void Awake()
    {
        numOfColumns = 61;
        textAssetData = Resources.Load<TextAsset>("TextFiles/EnemyDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }

    //private void Awake() // load CSV from Resources folder then add it to the Weapon Database game object
    //{
    //    ReadCSV();
    //}

    //void ReadCSV() // adds database to the enemyDatabase game object that should be in scene, from a csv file
    //{
    //    List<Dictionary<string, string>> testData = CSVReader.Read("TextFiles/EnemyDatabase");

    //    enemyDatabase.entries = new EnemyData[testData.Count];

    //    for (int i = 0; i < testData.Count; i++) 
    //    {
    //        enemyDatabase.entries[i] = new EnemyData(); // creates new row entry in memory

    //        FieldInfo[] fields = enemyDatabase.entries[i].GetType().GetFields(); // used to validate the data type of fields that need to be read in (below)

    //        for (int j = 0; j < fields.Length; j++)
    //        {
    //            if(typeof(int) == fields[j].FieldType) { // if the variable that's being loaded is meant to be an INT, use the below
    //                try { fields[j].SetValue(enemyDatabase.entries[i], int.Parse(testData[i].Values.ElementAt(j))); }
    //                catch { fields[j].SetValue(enemyDatabase.entries[i], 0); } // if can't parse the value set this default value*/
    //            }
    //            else if (typeof(bool) == fields[j].FieldType) // else, if it's meant to be a bool, use the below to parse in data
    //            {
    //                try { fields[j].SetValue(enemyDatabase.entries[i], bool.Parse(testData[i].Values.ElementAt(j))); }
    //                catch { fields[j].SetValue(enemyDatabase.entries[i], false); } // if can't parse the value set this default value*/
    //            }
    //            else if(typeof(string) == fields[j].FieldType){ // selse if it's a string...
    //                try { fields[j].SetValue(enemyDatabase.entries[i], testData[i].Values.ElementAt(j)); }
    //                catch { fields[j].SetValue(enemyDatabase.entries[i], "No Value"); } // if can't parse the value set this default value*/
    //            }
    //        }
    //    }
    //}

    private int GetSpecificAttackDamage(EnemyData enemy, int attackNumber)
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

    public int GetAttackDamages(string enemyName, int attackNumber) 
    {
        return GetSpecificAttackDamage(ReturnItemFromName(enemyName), attackNumber);
    }
}
