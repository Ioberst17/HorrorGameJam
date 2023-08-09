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
        numOfColumns = 62;
        textAssetData = Resources.Load<TextAsset>("TextFiles/EnemyDatabase");
        string[] data = ReadCSV();
        CreateDatabase(data);
    }

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
