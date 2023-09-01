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
        numOfColumns = 26;
        textAssetData = Resources.Load<TextAsset>("TextFiles/EnemyDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }

    private int GetSpecificAttackDamage(EnemyData enemy, int attackNumber)
    {

        if (attackNumber == 1) { return 1; }
        else if (attackNumber == 2) { return 1; }
        else if (attackNumber == 3) { return 1; }
        else if (attackNumber == 4) { return 1; }
        else if (attackNumber == 5) { return 1; }
        else if (attackNumber == 6) { return 1; }

        Debug.Log("No attackNumber matches the number listed in EnemyDatabase.cs");
        return -1;
    }

    public int GetAttackDamages(string enemyName, int attackNumber) 
    {
        return GetSpecificAttackDamage(ReturnItemFromName(enemyName), attackNumber);
    }
}
